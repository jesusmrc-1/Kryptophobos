using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Una clase es como un molde para hacer una caja con una forma especifica, la forma especifica
//es la información que va dentro, como por ejemplo variables, funciones..
//Los GameObjects de la jerarquia gastan las cajas creadas por ese molde, los existentes en jerarquia
//Una clase existe internamente, y luego varios GameObjects tiene las cajas creadas por ese molde.

public class UIManager : MonoBehaviour
{
    private Player _player;
    private PlayerInventory _inventory;
    private InteractableDistanceChecker _itemDistanceChecker;
    private EventsManager _gameEvents;

    public InputAction IngameMenu;
    private bool _isIngameMenuOpened;

    private Scene _scene;

    [SerializeField] private List<GameObject> _uiGameObjects = new List<GameObject>();

    [SerializeField] private List<string> _objectives = new List<string>();

    public GameObject ItemSlotPrefab;
    public GameObject NoteSlotPrefab;
    public GameObject ObjectiveSlotPrefab; //CREAR PREFAB PARA INSTACIAR LOS OBJETIVOS --------------------------------------------------

    private string _currentScene;

    public GameObject NonDiegeticCanvas;
    public GameObject DiegeticCanvas;

    public TextMeshProUGUI DiegeticTMP;

    public TextMeshProUGUI ObjectivesText;
    [TextArea(1,3)]public string NewObjectiveText;

    //Revisar, puede que no se use por la solución hechar hace un timepo.
    public GameObject WorldSpaceCanvas;

    //SINGLETON
    //Se hace static para ser accesible desde cualquier parte sin necesidad de referencias y guarda la referencia al único UIManager que existe
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        //PROBLEMA SOLUCIONADO DE NUEVO: EL UI MANAGER AL PASARLO A STATIC PUEDE SER ACCEDIDO DESDE CUALQUIER SITIO SIN REFERENCIA, ENTONCES DESDE EL TEXTO
        //BUSCAMOS AL UI MANAGER Y LE GUARDAMOS LA REFERENCIA DEL TMP.

        //PROBLEMA SOLUCIONADO: Como UI Manager se mete en DontDestroyOnLoad, buscar el texto diegetico no funcionaba, ahora UI Manager se encarga de
        //instanciar el prefab que contiene el canvas en wolrd space + el texto diegetico.
        //GameObject canvas = Instantiate(WorldSpaceCanvas, Vector3.zero, Quaternion.identity);
        //DiegeticTMP = canvas.GetComponentInChildren<TextMeshProUGUI>();
        //***********************************************************************************************

        IngameMenu.Enable();

        //CUALQUIER REFERENCIA PERMANTENTE PONERLA DENTRO DE OnSceneChanged, ya que en el update al estar en una nueva escena llama por primera vez y referencia todo de nuevo, evitamos
        //referenciar dos veces al principio.


        //GameObject DiegeticTextGO = GameObject.FindGameObjectWithTag("TEXT_Diegetic");
        //if (DiegeticTextGO != null) DiegeticTMP = DiegeticTextGO.GetComponent<TextMeshProUGUI>();

        ObjectivesText = GameObject.FindGameObjectWithTag("UI_Objectives").GetComponent<TextMeshProUGUI>(); ;
        //DiegeticTMP = GameObject.FindGameObjectWithTag("TEXT_Diegetic").GetComponent<TextMeshProUGUI>();

        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null) 
        {
            _player = player.GetComponent<Player>();
            _inventory = player.GetComponent<PlayerInventory>();
            _itemDistanceChecker = player.GetComponent<InteractableDistanceChecker>();
        }

        //if (_inventory != null ) _inventory.OnInventoryChanged += RefreshInventoryItemsUI;
        if (_itemDistanceChecker != null)
        {
            _itemDistanceChecker.OnPlayerNearTrigger += ChangeTextSettings;
            _itemDistanceChecker.OnNoTriggersNearby += HideDiegeticText;
        }

        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();

        ChangeObjective(ObjectivesText.text);
    }

    private void Update()
    {
        //Comprobar cambio de escena
        string activeScene = SceneManager.GetActiveScene().name;

        if (activeScene != _currentScene)
        {
            _currentScene = activeScene;
            OnSceneChanged();
        }

        if (IngameMenu.triggered && !_isIngameMenuOpened && !_player.IsPlayerDoingPuzzle)
        {
            OpenIngameMenu();
            _isIngameMenuOpened = true;
            if (_gameEvents != null) _gameEvents.PauseGame();
            //RefreshObjectivesUI(_objectives);
        }
        else if (IngameMenu.triggered && _isIngameMenuOpened && !_player.IsPlayerDoingPuzzle)
        {
            CloseIngameMenu();
            _isIngameMenuOpened = false;
            if (_gameEvents != null) _gameEvents.ResumeGame();
        }
    }

    #region Refresh UI Elements
    void RefreshInventoryItemsUI(List<ItemStack> items)
    {
        //Referenciamos el grid donde van los items.
        GameObject gridLayoutGroup = _uiGameObjects.Find(go => go.name == "ItemsContainer");

        //Para evitar que se dupliquen items al pulsar al boton items.
        foreach (Transform child in gridLayoutGroup.transform)
        {
            if (child != null) Destroy (child.gameObject);
        }

        //Por cada item de la lista 'items' se instancia un prefab que toma la imagen y la cantidad de cada item.
        foreach (var item in items)
        {
            // Si no existe uno, instanciamos uno nuevo y lo agregamos al pool
            GameObject newItemSlot = Instantiate(ItemSlotPrefab, gridLayoutGroup.transform);

            //Referenciar componentes del prefab
            Image image = newItemSlot.GetComponent<Image>();
            TextMeshProUGUI tmp = newItemSlot.GetComponentInChildren<TextMeshProUGUI>();

            //El componente 'Image' toma como sprite de la variable 'Icon' del SO ItemData de la clase ItemStack guardada en la lista.
            image.sprite = item.Item.Icon;

            //El componente 'TextMeshPro' toma la cantidad guardada de 'Amount' de la clase ItemStack guardada en la lista.
            tmp.text = item.Amount.ToString();
        }
    }

    void RefreshInventoryNotesUI(List<NoteData> notes)
    {
        //Referenciamos la columna donde van las notas.
        GameObject verticalLayoutGroupNoteContainer = _uiGameObjects.Find(go => go.name == "NotesContainer");

        //Referenciamos el componente TMP donde va escrito el mensaje de la nota
        TextMeshProUGUI textMeshProNoteText = _uiGameObjects.Find(go => go.name == "NoteText").GetComponent<TextMeshProUGUI>();

        //Para evitar que se dupliquen notas al pulsar al boton Notes.
        if (verticalLayoutGroupNoteContainer.transform != null)
        {
            foreach (Transform child in verticalLayoutGroupNoteContainer.transform)
            {
                if (child != null) Destroy(child.gameObject);
            }
        }

        //Por cada item de la lista 'items' se instancia un prefab que toma la imagen y la cantidad de cada item.
        foreach (var note in notes)
        {
            // Si no existe uno, instanciamos uno nuevo y lo agregamos al pool
            GameObject newNoteSlot = Instantiate(NoteSlotPrefab, verticalLayoutGroupNoteContainer.transform);

            //Referenciar componentes del prefab
            TextMeshProUGUI tmp = newNoteSlot.GetComponentInChildren<TextMeshProUGUI>();

            //El componente 'TextMeshPro' toma el nombre de la nota.
            tmp.text = note.NoteName;

            //Al script del boton le referenciamos el TMP 'textMeshProNoteText' donde ira el mensaje 'note.NoteText'.
            ChangeNoteText changeNoteText = newNoteSlot.GetComponent<ChangeNoteText>();
            changeNoteText.TMP = textMeshProNoteText;
            changeNoteText.Text = note.NoteText;
        }
    }

    void RefreshObjectivesUI(List<string> objectives)
    {
        //Referenciamos la columna donde van las notas.
        GameObject verticalLayoutGroupObjectivesContainer = _uiGameObjects.Find(go => go.name == "ObjectivesContainer");

        //Para evitar que se dupliquen items al pulsar al boton items.
        foreach (Transform child in verticalLayoutGroupObjectivesContainer.transform)
        {
            if (child != null) Destroy(child.gameObject);
        }

        //Por cada item de la lista 'items' se instancia un prefab que toma la imagen y la cantidad de cada item.
        foreach (var objective in objectives)
        {
            // Si no existe uno, instanciamos uno nuevo y lo agregamos al pool
            GameObject newObjectiveSlot = Instantiate(ObjectiveSlotPrefab, verticalLayoutGroupObjectivesContainer.transform);

            //Referenciar componentes del prefab
            TextMeshProUGUI tmp = newObjectiveSlot.GetComponentInChildren<TextMeshProUGUI>();

            //El componente 'TextMeshPro' toma el texto del objetivo.
            tmp.text = objective;
        }
    }
    #endregion

    #region Diegetic 3D Text
    void ChangeTextSettings(Collider c)
    {
        if (c.tag == "Item")
        {
            //TMP toma los valores de ItemPickup heredados de la clase abstracta TextSettings.
            ItemPickup itemPickup = c.GetComponentInParent<ItemPickup>();
            DiegeticTMP.text = itemPickup.DiegeticText;
            DiegeticTMP.fontSize = itemPickup.DiegeticFontSize;
            DiegeticTMP.transform.SetParent(c.transform.parent.transform);
            DiegeticTMP.transform.localPosition = itemPickup.DiegeticTextPosition.transform.localPosition;
            DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
        }
        else if(c.tag == "Note")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            NotePickup notePickup = c.GetComponentInParent<NotePickup>();
            DiegeticTMP.text = notePickup.DiegeticText;
            DiegeticTMP.fontSize = notePickup.DiegeticFontSize;
            DiegeticTMP.transform.SetParent(c.transform.parent.transform);
            DiegeticTMP.transform.localPosition = notePickup.DiegeticTextPosition.transform.localPosition;
            DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
        }
        else if (c.tag == "Door")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            Door door = c.GetComponentInParent<Door>();
            DiegeticTMP.text = door.DiegeticText;
            DiegeticTMP.fontSize = door.DiegeticFontSize;
            DiegeticTMP.transform.SetParent(c.transform.parent.transform);
            DiegeticTMP.transform.localPosition = door.DiegeticTextPosition.transform.localPosition;
            DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
        }
        else if (c.tag == "Puzzle")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            Puzzle puzzle = c.GetComponentInParent<Puzzle>();
            DiegeticTMP.text = puzzle.DiegeticText;
            DiegeticTMP.fontSize = puzzle.DiegeticFontSize;
            DiegeticTMP.transform.SetParent(c.transform.parent.transform);
            DiegeticTMP.transform.localPosition = puzzle.DiegeticTextPosition.transform.localPosition;
            DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
        }

        DiegeticText diegeticText = DiegeticTMP.gameObject.GetComponent<DiegeticText>();
        //diegeticText.StopAllCoroutines();
        diegeticText.CallCoroutine("ShowText");
    }

    void HideDiegeticText()
    {
        //HAY QUE ARREGLAR LO DE QUE DESAPAREZCA EL TEXTO, COMO EL COMPORTAMIENTO LO TIENE EL PROPIO TEXTO Y ES UN SISTEMA ANTIGUO, HAY QUE ADAPTARLO A LO NUEVO.

        //HACER QUE EL TEXTO SE QUEDE BAJO DEL OBJETO SEGUN LA POSICION + UN OFFSET; SI ESTA MUY BAJO AL APARECER DA UN GIRO RARO Y ATRAVIESA EL TEXTO UN POCO
        //DiegeticTMP.transform.localPosition = new Vector3(0, -5, 0);
        //DiegeticTMP.text = "";
        DiegeticText diegeticText = DiegeticTMP.gameObject.GetComponent<DiegeticText>();
        //diegeticText.StopAllCoroutines();
        diegeticText.CallCoroutine("HideText");
    }
    #endregion

    /*
    public IEnumerator CallOnSceneChanged()
    {
        Debug.Log("CORROUTINE");
        yield return new WaitForSeconds(3f);
        Debug.Log("CORROUTINE END");
        OnSceneChanged();
    }
    */

    //PUEDE QUE NO HAGA FALTA POR LA SOLUCION QUE SE DIO HACE UN TIEMPO,PERO PUEDE SERVIR PARA OTRAS COSAS..

    //Cada vez que se cambie la escena, volvemos a referenciar el texto. AL HACER DontDestroyOnLoad(gameObject);, no puede referenciar elementos del canvas
    //Pasar canvas a SINGLETON
    void OnSceneChanged()
    {
        //var diegeticText = GameObject.FindGameObjectWithTag("TEXT_Diegetic");
        //if (diegeticText != null) DiegeticTMP = diegeticText.GetComponent<TextMeshProUGUI>();

        NonDiegeticCanvas = GameObject.FindGameObjectWithTag("CANVAS_NonDiegetic");
        DiegeticCanvas = GameObject.FindGameObjectWithTag("CANVAS_Diegetic");

        ObjectivesText = GameObject.FindGameObjectWithTag("UI_Objectives").GetComponent<TextMeshProUGUI>();
        DiegeticTMP = GameObject.FindGameObjectWithTag("TEXT_Diegetic").GetComponent<TextMeshProUGUI>();
    }

    #region Register Buttons
    private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

    //Cada boton de la escena Ingame Menu, llama a RegisterButton, le pasa el nombre de la accion
    //y su clase Button para registar en el Unity Events de cada uno, una funcion de UI Manager.
    public void RegisterButton(string actionName, Button btn)
    {
        //Creamos en el diccionario una palabra 'actionName' que almacena 'btn'
        _buttons[actionName] = btn;

        //Si la accion se llama Objectives, se registra la función 'OnClickObjectives'
        //al Unity Events OnClick del botón que lleva por defecto.
        switch (actionName)
        {
            case "Objectives":
                btn.onClick.AddListener(OnClickObjectives);
                break;
            case "Notes":
                btn.onClick.AddListener(OnClickNotes);
                break;
            case "Inventory":
                btn.onClick.AddListener(OnClickInventory);
                break;
            case "Exit":
                btn.onClick.AddListener(OnClickExit);
                break;
        }
    }
    #endregion

    #region References from Ingame Menu
    //Guardamos las referencias de las ventanas de la UI del menu ingame en '_uiGameObjects'
    public void SaveRefsFromMenuIngame(GameObject uiGameObject)
    {
        _uiGameObjects.Add(uiGameObject);
    }
    #endregion

    #region Ingame Menu 
    void OpenIngameMenu()
    {
        //foreach (GameObject go in _uiGameObjects) if (go == null) _uiGameObjects.Remove(go); DA ERROR.

        //Antes de cargar la escena y que se añadan a la lista '_uiGameObjects' mas referencias, las que sean null se eliminan. 
        _uiGameObjects.RemoveAll(nullRef => nullRef == null);

        //Carga de forma aditiva la escena que contiene el menu ingame.
        SceneManager.LoadScene("UI_Menu", LoadSceneMode.Additive);

        //Referenciamos la escena a _scene.
        _scene = SceneManager.GetSceneByName("UI_Menu");
    }

    void CloseIngameMenu()
    {
        SceneManager.UnloadSceneAsync(_scene);
    }

    //POSIBLEMENTE SE PUEDE HACER UNA MAQUINA DE ESTADOS, YA QUE EL MENU SE ENCUENTRA EN EL ESTADO ShowingObjectives, ShowingNotes etc...

    //Mostrar los objetivos
    public void OnClickObjectives()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();

        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("OBJECTIVES");

        RefreshObjectivesUI(_objectives);
    }

    //Mostrar las notas
    public void OnClickNotes()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();

        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("NOTES");

        //Instanciar los items del inventario en el layout group de inventory
        RefreshInventoryNotesUI(_inventory.Notes);
    }

    //Mostrar el inventario
    public void OnClickInventory()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();

        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("INVENTORY");

        //Instanciar los items del inventario en el layout group de inventory
        RefreshInventoryItemsUI(_inventory.Items);
    }

    //Mostrar opciones de salida al menu principal.
    public void OnClickExit()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();

        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("EXIT");
    }
    #endregion

    #region Enable / Disable UI Game Objects
    void DisableMenuIngameGameObjects()
    {
        foreach (GameObject go in _uiGameObjects)
        {
            if (go != null) go.SetActive(false);
        }
    }

    void EnableMenuIngameGameObject(string gameObjectName)
    {
        GameObject uiGameObject = _uiGameObjects.Find(go => go.name == gameObjectName);

        if (uiGameObject != null)
        {
            uiGameObject.SetActive(true);

            ActivateAllChildren(uiGameObject);
        }
    }

    //Recursividad, por cada hijo se llama a esta funcion, de esa forma en cascada todos los parents que a su vez son hijos de otros se activa.
    void ActivateAllChildren(GameObject parent)
    {
        parent.SetActive(true);

        foreach (Transform child in parent.transform)
        {
            ActivateAllChildren(child.gameObject);
        }
    }
    #endregion


    public void ChangeObjective(string newObjectiveText)
    {
        ObjectivesText.text = newObjectiveText;

        //Llamar al texto no diegetico que se encarga de mostrar el objetivo.
        ShowObjective showObjective = ObjectivesText.GetComponentInParent<ShowObjective>();
        if (showObjective != null) showObjective.NewObjective();

        //Guardamos el objetivo en la lista de _objectives.
        _objectives.Add(newObjectiveText);
    }
}
