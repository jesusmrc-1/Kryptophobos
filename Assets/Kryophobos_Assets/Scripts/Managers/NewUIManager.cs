using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour
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
    private List<bool> _completedObjectives = new List<bool>();

    public GameObject ItemSlotPrefab;
    public GameObject NoteSlotPrefab;
    public GameObject ObjectiveSlotPrefab; //CREAR PREFAB PARA INSTACIAR LOS OBJETIVOS --------------------------------------------------

    private string _currentScene;

    public GameObject NonDiegeticCanvas;
    public GameObject DiegeticCanvas;

    public TextMeshProUGUI DiegeticTMP;

    public TextMeshProUGUI ObjectivesText;
    [TextArea(1, 3)] public string NewObjectiveText;

    //Revisar, puede que no se use por la solución hechar hace un timepo.
    public GameObject WorldSpaceCanvas;

    public GameObject IngameMenuCanvas;

    private void Start()
    {
        IngameMenu.Enable();

        DiegeticTMP = GameObject.FindGameObjectWithTag("TEXT_Diegetic").GetComponent<TextMeshProUGUI>();
        DiegeticCanvas = GameObject.FindGameObjectWithTag("CANVAS_Diegetic");
    
        ObjectivesText = GameObject.FindGameObjectWithTag("UI_Objectives").GetComponent<TextMeshProUGUI>(); ;
    
        var player = GameObject.FindGameObjectWithTag("Player");
    
        if (player != null)
        {
            _player = player.GetComponent<Player>();
            _inventory = player.GetComponent<PlayerInventory>();
            _itemDistanceChecker = player.GetComponent<InteractableDistanceChecker>();
        }
    
        if (_itemDistanceChecker != null)
        {
            _itemDistanceChecker.OnPlayerNearTrigger += ChangeTextSettings;
            _itemDistanceChecker.OnNoTriggersNearby += HideDiegeticText;
        }
    
        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();

        //if (_objectives.Count == 0) ChangeObjective(ObjectivesText.text);// Poner un trigger en el spawn y ya esta, quitar esta linea

        LoadStatus();
    }
    
    private void Update()
    {
        if (IngameMenu.triggered && !_isIngameMenuOpened && !_player.IsPlayerDoingPuzzle) OpenIngameMenu("Objectives");
        else if (IngameMenu.triggered && _isIngameMenuOpened && !_player.IsPlayerDoingPuzzle) CloseIngameMenu();
    }
    
    #region Refresh UI Elements
    void RefreshInventoryItemsUI(List<ItemStack> items)
    {
        //Referenciamos el grid donde van los items.
        GameObject gridLayoutGroup = _uiGameObjects.Find(go => go.name == "ItemsContainer");
    
        //Para evitar que se dupliquen items al pulsar al boton items.
        foreach (Transform child in gridLayoutGroup.transform)
        {
            if (child != null) Destroy(child.gameObject);
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
        for (int i = 0; i < objectives.Count; i++)
        {
            // Si no existe uno, instanciamos uno nuevo y lo agregamos al pool
            GameObject newObjectiveSlot = Instantiate(ObjectiveSlotPrefab, verticalLayoutGroupObjectivesContainer.transform);

            //Referenciar componentes del prefab
            TextMeshProUGUI tmp = newObjectiveSlot.GetComponentInChildren<TextMeshProUGUI>();

            //El componente 'TextMeshPro' toma el texto del objetivo.
            tmp.text = objectives[i];

            //Si el estado del objetivo esta cumplido, tacharlo
            ChangeVisibility changeVisibility = newObjectiveSlot.GetComponentInChildren<ChangeVisibility>();

            if (changeVisibility != null && _completedObjectives[i])
            {
                Debug.Log("Calling changeVisibility.EnableRenderer() METHOD");
                changeVisibility.EnableRenderer();
            }
        }
        /*
        foreach (var objective in objectives)
        {
            // Si no existe uno, instanciamos uno nuevo y lo agregamos al pool
            GameObject newObjectiveSlot = Instantiate(ObjectiveSlotPrefab, verticalLayoutGroupObjectivesContainer.transform);
    
            //Referenciar componentes del prefab
            TextMeshProUGUI tmp = newObjectiveSlot.GetComponentInChildren<TextMeshProUGUI>();
    
            //El componente 'TextMeshPro' toma el texto del objetivo.
            tmp.text = objective;

            //Si el estado del objetivo esta cumplido, tacharlo
            ChangeVisibility changeVisibility = newObjectiveSlot.GetComponentInChildren<ChangeVisibility>();

            if (changeVisibility != null)
            {
                changeVisibility.EnableRenderer();
            }
        }
        */
    }
    #endregion
    
    #region Diegetic 3D Text
    void ChangeTextSettings(Collider c)
    {
        if (c.tag == "Item")
        {
            //TMP toma los valores de ItemPickup heredados de la clase abstracta TextSettings.
            ItemPickup itemPickup = c.GetComponentInParent<ItemPickup>();
            if (itemPickup != null)
            {
                DiegeticTMP.text = itemPickup.DiegeticText;
                DiegeticTMP.fontSize = itemPickup.DiegeticFontSize;
                DiegeticTMP.transform.SetParent(c.transform.parent.transform);
                DiegeticTMP.transform.localPosition = itemPickup.DiegeticTextPosition.transform.localPosition;
                DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
            }
        }
        else if (c.tag == "Note")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            NotePickup notePickup = c.GetComponentInParent<NotePickup>();
            if (notePickup != null) 
            {
                DiegeticTMP.text = notePickup.DiegeticText;
                DiegeticTMP.fontSize = notePickup.DiegeticFontSize;
                DiegeticTMP.transform.SetParent(c.transform.parent.transform);
                DiegeticTMP.transform.localPosition = notePickup.DiegeticTextPosition.transform.localPosition;
                DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
            }
        }
        else if (c.tag == "Door")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            Door door = c.GetComponentInParent<Door>();
            if (door != null)
            {
                DiegeticTMP.text = door.DiegeticText;
                DiegeticTMP.fontSize = door.DiegeticFontSize;
                DiegeticTMP.transform.SetParent(c.transform.parent.transform);
                DiegeticTMP.transform.localPosition = door.DiegeticTextPosition.transform.localPosition;
                DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
            }
        }
        else if (c.tag == "Puzzle")
        {
            //TMP toma los valores de Door heredados de la clase abstracta TextSettings. 
            Puzzle puzzle = c.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                DiegeticTMP.text = puzzle.DiegeticText;
                DiegeticTMP.fontSize = puzzle.DiegeticFontSize;
                DiegeticTMP.transform.SetParent(c.transform.parent.transform);
                DiegeticTMP.transform.localPosition = puzzle.DiegeticTextPosition.transform.localPosition;
                DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
            }
        }
        else if (c.tag == "Button")
        {
            EndingButton endingButton = c.GetComponentInParent<EndingButton>();
            if (endingButton != null)
            {
                DiegeticTMP.text = endingButton.DiegeticText;
                DiegeticTMP.fontSize = endingButton.DiegeticFontSize;
                DiegeticTMP.transform.SetParent(c.transform);
                DiegeticTMP.transform.localPosition = endingButton.DiegeticTextPosition.transform.localPosition;
                DiegeticTMP.transform.SetParent(DiegeticCanvas.transform);
            }
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
    
    #region References from Ingame Menu
    //Guardamos las referencias de las ventanas de la UI del menu ingame en '_uiGameObjects'
    public void SaveRefsFromMenuIngame(GameObject uiGameObject)
    {
        _uiGameObjects.Add(uiGameObject);
    }
    #endregion
    
    #region Ingame Menu 
    public void OpenIngameMenu(string optionName)
    {
        _isIngameMenuOpened = true;
        IngameMenuCanvas.SetActive(true);
        if (_gameEvents != null) _gameEvents.PauseGame();

        switch (optionName)
        {
            case "Objectives":
                Objectives();
                break;

            case "Notes":
                Notes();
                break;
        }
    }
    
    public void CloseIngameMenu()
    {
        if (_gameEvents != null) _gameEvents.ResumeGame();
        _isIngameMenuOpened = false;
        IngameMenuCanvas.SetActive(false);
    }
    
    //POSIBLEMENTE SE PUEDE HACER UNA MAQUINA DE ESTADOS, YA QUE EL MENU SE ENCUENTRA EN EL ESTADO ShowingObjectives, ShowingNotes etc...
    
    //Mostrar los objetivos
    public void Objectives()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();
    
        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("OBJECTIVES");
    
        RefreshObjectivesUI(_objectives);
    }
    
    //Mostrar las notas
    public void Notes()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();
    
        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("NOTES");
    
        //Instanciar los items del inventario en el layout group de inventory
        RefreshInventoryNotesUI(_inventory.Notes);
    }
    
    //Mostrar el inventario
    public void Inventory()
    {
        //Desactivo los GameObjects correspondientes de la ui.
        DisableMenuIngameGameObjects();
    
        //Busco el que corresponda con lo que esta funcion tiene que hacer y lo activo
        EnableMenuIngameGameObject("INVENTORY");
    
        //Instanciar los items del inventario en el layout group de inventory
        RefreshInventoryItemsUI(_inventory.Items);
    }
    
    //Mostrar opciones de salida al menu principal.
    public void Exit()
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
        if (_completedObjectives.Count != 0)
        {
            //_completedObjectives[^1] es lo mismo que _completedObjectives[_completedObjectives.Count -1], muestra el utlimo valor de la lista
            //_completedObjectives[^2] = penultimo valor
            //_completedObjectives[^3] = ...

            //Cambiamos el estado del ultimo objetivo a TRUE
            _completedObjectives[^1] = true;
            GameManager.Instance.CompletedObjectives[^1] = true;
            //Debug.Log(_completedObjectives[^1]);
        }


        ObjectivesText.text = newObjectiveText;

        //Llamar al texto no diegetico que se encarga de mostrar el objetivo.
        ShowObjective showObjective = ObjectivesText.GetComponentInParent<ShowObjective>();
        if (showObjective != null) showObjective.NewObjective();
    
        //Guardamos el objetivo en la lista de _objectives.
        _objectives.Add(newObjectiveText);
        _completedObjectives.Add(false);

        GameManager.Instance.Objectives.Add(newObjectiveText);
        GameManager.Instance.CompletedObjectives.Add(false);
    }

    private void LoadStatus()
    {
        _objectives.Clear();
        _objectives = new List<string>(GameManager.Instance.Objectives);
        _completedObjectives = new List<bool>(GameManager.Instance.CompletedObjectives);
    }
}
