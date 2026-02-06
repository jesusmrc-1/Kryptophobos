using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GearMachine : PuzzleBase
{
    [SerializeField] private string _ID;
    [SerializeField] private ItemData _item;

    private EventsManager _gameEvents;

    private NewUIManager _newUIManager;

    [SerializeField] private InputAction _select;
    [SerializeField] private InputAction _cancelSelection;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignorePlayer;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignoreEnemy;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignoreEnemyVision;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignoreRaycast;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignoreInteractable;

    [SerializeField] private bool _hasRayImpacted;
    private RaycastHit _hitInfo;

    [SerializeField] private bool _holdingObject;
    [SerializeField] private bool _isObjectNearSocket;
    [SerializeField] private Transform _nearestSocket;
    [SerializeField] private List<GameObject> _sockets = new List<GameObject>();
    [SerializeField] private List<bool> _socketStatus = new List<bool>();
    [SerializeField] private string[] _socketObjectID;

    [SerializeField] private float _snapDistance = 0.1f;

    [SerializeField] private PuzzleObjectData _objectData;
    [SerializeField] private GameObject _selectedObject;
    private GameObject _selectedObjectPreview;
    private GameObject _selectedObjectToInstantiate;

    [SerializeField] private GameObject _instanceOfSelectedObjectPreview;

    [SerializeField] private int _index;

    //Configurable según puzle
    [SerializeField] private bool _randomizeObjectQuantity;
    [SerializeField] private bool _randomizeSolution;
    [SerializeField] private bool _isPuzzleSolved;

    [SerializeField] private GameObject[] _puzzleObjects;
    [SerializeField] private List<GameObject> _instantiatedObjects = new List<GameObject>();

    [SerializeField] private GameObject[] _resistorValuesUI;

    [SerializeField] private int _bigGearQuantity;
    [SerializeField] private int _mediumGearQuantity;
    [SerializeField] private int _smallGearQuantity;

    private int _bigGearMaxCap;
    private int _mediumGearMaxCap;
    private int _smallGearMaxCap;

    //Engranajes que se muestran al completar el puzle.
    [Tooltip("Los engranajes ocultos que se muestran al terminar el puzle para reemplazar los que ha puesto el jugador.")]
    [SerializeField] private GameObject[] _firstRowGearsToShow;

    [Tooltip("Los engranajes ocultos que se muestran al terminar el puzle para reemplazar los que ha puesto el jugador.")]
    [SerializeField] private GameObject[] _secondRowGearsToShow;

    [Tooltip("Los engranajes ocultos que se muestran al terminar el puzle para reemplazar los que ha puesto el jugador.")]
    [SerializeField] private GameObject[] _thirdRowGearsToShow;

    private Animator _animator;

    [SerializeField] private float _offset;

    [SerializeField] private TextMeshProUGUI _bigGearQuantityText;
    [SerializeField] private TextMeshProUGUI _mediumGearQuantityText;
    [SerializeField] private TextMeshProUGUI _smallGearQuantityText;

    [SerializeField] private string _newObjectiveText;

    [SerializeField] private float _endingDelayTime;

    private string _randomRow;

    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _newUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<NewUIManager>();
        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();

        _select.Enable();
        _cancelSelection.Enable();


        _socketObjectID = new string[_sockets.Count];

        //Add a socket occupied status (bool) for each socket in '_sockets' list. (If we add more sockets to the puzzle, it will increment)
        //Add an empty ID for each socket in '_sockets' list. (If we add more sockets to the puzzle, it will increment)
        if (_sockets.Count > 0)
        {
            for (int i = 0; i < _sockets.Count; i++)
            {
                _socketStatus.Add(false);
                _socketObjectID[i] = "Empty ID";
            }
        }

        RandomizePuzzleSolution();

        LoadStatus();

        if (!_isPuzzleSolved) this.enabled = false;
    }

    private void Update()
    {
        if (!_isPuzzleSolved)
        {
            if (_bigGearQuantityText != null) _bigGearQuantityText.text = _bigGearQuantity.ToString();
            if (_mediumGearQuantityText != null) _mediumGearQuantityText.text = _mediumGearQuantity.ToString();
            if (_smallGearQuantityText != null) _smallGearQuantityText.text = _smallGearQuantity.ToString();

            CheckPuzzleStatus();

            CheckObjectCap();

            RaycastFromMouse();

            if (_holdingObject && _cancelSelection.triggered) CancelSelection();

            if (_holdingObject) ShowGrabbedObject();

            //Si no he elegido un engranje, elijo que tipo de engranaje quiero colocar o, quito un engranaje que ya este puesto.
            if (_select.triggered && !_holdingObject)
            {
                CheckClickedObject();

                if (_objectData != null)
                {
                    if (!_objectData.IsObjectPlaced) SelectObject();
                    else if (_objectData.IsObjectPlaced) RemoveObject();
                }
            }

            //Si llevo un engranaje y lo coloco cerca de un hueco y hago click, comprobamos si nos queda algun engranaje del que tengo seleccionado.
            else if (_select.triggered && _holdingObject && _isObjectNearSocket) CheckSocket();

            //Si he elegido un engranaje y quiero cambiar a otro tipo de engranaje.
            else if (_select.triggered && _holdingObject && !_isObjectNearSocket)
            {
                PuzzleObjectData objectData = _hitInfo.collider.GetComponent<PuzzleObjectData>();

                if (objectData != null)
                {
                    if (!objectData.IsObjectPlaced) ChangeSelection();
                }
            }
        }
    }

    private void RandomizePuzzleSolution()
    {
        //Ruta 1 1x Grande  2x Medianos, 4x Pequeños
        //Ruta 2 1x Grande, 3x Medianos, 3x Pequeños
        //Ruta 3 1x Grande, 5x Medianos, 1x Pequeños

        if (!_randomizeObjectQuantity)
        {
            _randomizeObjectQuantity = true;

            int randomNumber = Random.Range(1, 4);

            switch (randomNumber)
            {
                case 1:
                    _bigGearQuantity = 1;
                    _mediumGearQuantity = 2;
                    _smallGearQuantity = 4;
                    _randomRow = "First";
                    break;
                case 2:
                    _bigGearQuantity = 1;
                    _mediumGearQuantity = 3;
                    _smallGearQuantity = 3;
                    _randomRow = "Second";
                    break;
                case 3:
                    _bigGearQuantity = 1;
                    _mediumGearQuantity = 5;
                    _smallGearQuantity = 1;
                    _randomRow = "Third";
                    break;
            }
        }

        _bigGearMaxCap = _bigGearQuantity;
        _mediumGearMaxCap = _mediumGearQuantity;
        _smallGearMaxCap = _smallGearQuantity;
    }

    //Si coinciden los ID de '_socketObjectIDSolution' comparados con los de '_socketObjectID', significa que cada objeto esta colocado
    //en la posición correcta, entonces '_isPuzzleSolved' es true.
    public void CheckPuzzleStatus()
    {

        //COMPROBAR EN UNA LISTA DE IDs, si en la fila uno ha

        if (!_isPuzzleSolved)
        {
            _isPuzzleSolved = true;

            //Ruta 1 1x Grande  2x Medianos, 4x Pequeños
            //Ruta 2 1x Grande, 3x Medianos, 3x Pequeños
            //Ruta 3 1x Grande, 5x Medianos, 1x Pequeños

            string big = "BigGear";
            string medium = "MediumGear";
            string small = "SmallGear";

            //First Row
            if (_socketObjectID[0] == small &&
               _socketObjectID[4] == small &&
               _socketObjectID[5] == small &&
               _socketObjectID[8] == small &&
               _socketObjectID[13] == big &&
               _socketObjectID[17] == medium &&
               _socketObjectID[18] == medium)
            {
                //Decimos de que fila hay que mostrar los engranajes y la variable para el animator.
                StartCoroutine(Delay(_firstRowGearsToShow, "IsFirstRowComplete"));
            }

            //Second Row
            else if (_socketObjectID[3] == medium &&
                _socketObjectID[6] == medium &&
                _socketObjectID[9] == small &&
                _socketObjectID[11] == small &&
                _socketObjectID[12] == big &&
                _socketObjectID[14] == small &&
                _socketObjectID[18] == medium)
            {
                // ...
                StartCoroutine(Delay(_secondRowGearsToShow, "IsSecondRowComplete"));
            }

            //Third Row
            else if (_socketObjectID[1] == small &&
                _socketObjectID[2] == medium &&
                _socketObjectID[7] == medium &&
                _socketObjectID[10] == medium &&
                _socketObjectID[15] == big &&
                _socketObjectID[16] == medium &&
                _socketObjectID[18] == medium)
            {
                // ...
                StartCoroutine(Delay(_thirdRowGearsToShow, "IsThirdRowComplete"));
            }
            else _isPuzzleSolved = false;
        }
    }

    private IEnumerator Delay(GameObject[] gears, string row)
    {
        _audioSource.Play();

        foreach (GameObject gear in gears)
        {
            MeshRenderer meshRenderer = gear.GetComponent<MeshRenderer>();
            meshRenderer.enabled = true;
        }
        _animator.SetBool(row, true);

        yield return new WaitForSeconds(_endingDelayTime);

        PuzzleSolved();
    }

    //Una vez el puzle se complete, cada engranaje de la fila que se ha completado se muestra.
    void PuzzleSolved()
    {
        _isPuzzleSolved = true;
        _gameEvents.PuzzleSolved(this);
        GameManager.Instance.Puzzles.Add(_ID);
        _newUIManager.ChangeObjective(_newObjectiveText);
    }

    private void CheckObjectCap()
    {
        if (_bigGearQuantity > _bigGearMaxCap) _bigGearQuantity = _bigGearMaxCap;
        if (_mediumGearQuantity > _mediumGearMaxCap) _mediumGearQuantity = _mediumGearMaxCap;
        if (_smallGearQuantity > _smallGearMaxCap) _smallGearQuantity = _smallGearMaxCap;
    }

    public override void EnablePuzzle()
    {
        _gameEvents.DoingPuzzle();
    }

    public override void DisablePuzzle()
    {
        _gameEvents.NotDoingPuzzle();
    }

    private void RaycastFromMouse()
    {
        //Lanzamos un rayo de la camara main hacia las coordenadas del select y guardamos si impacta o no en 'hasRayImpacted'.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Combinamos las capas a ignorar.
        LayerMask ignored = _ignorePlayer | _ignoreEnemy | _ignoreEnemyVision | _ignoreRaycast | _ignoreInteractable;   //Esto es igual a 11000100

        //Invertimos el valor para que las capas en esos bits sean ignoradas ' = 0 '
        int mask = ~ignored;                                                  //Esto es igual a 00111011

        _hasRayImpacted = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~ignored); //El simbolo '~' invierte, entonces ignora lo que este en esa mascara.

        if (_hasRayImpacted) _hitInfo = hitInfo;
    }

    private void CheckClickedObject()
    {
        _objectData = _hitInfo.collider.gameObject.GetComponent<PuzzleObjectData>();

        if (_objectData != null && !_objectData.IsObjectPlaced)
        {
            //SI TENEMOS YA EL OBJECT DATA, ENTONCES NO HACE FALTA GUARDARSE NADA, PODEMOS USAR EN LOS DEMAS SITIOS DIRECTAMENTE _objectData.LOQUESEA (PROBAR A VER QUE TAL)

            _selectedObject = _hitInfo.collider.gameObject;
            _selectedObjectPreview = _objectData.PrefabToShow;
            _selectedObjectToInstantiate = _objectData.PrefabToInstantiate;
        }

        Debug.LogWarning(_hitInfo.collider.name);
    }

    private void ShowGrabbedObject()
    {
        //Comprobamos la distancia que hay desde el holograma con respecto a cada uno de los huecos de engranajes
        CheckDistance();

        //Si el engranaje no esta cerca de un hueco.
        if (!_isObjectNearSocket)
        {
            //Si el rayo ha impactado, mostramos el holograma en el punto del mundo en el que impacto.
            if (_hasRayImpacted)
            {
                if (_instanceOfSelectedObjectPreview != null) _instanceOfSelectedObjectPreview.transform.position = _hitInfo.point;
            }

        }

        //Si el engranaje esta cerca de un hueco
        else if (_isObjectNearSocket)
        {
            //Mostramos el holograma en el hueco del engranaje.
            _instanceOfSelectedObjectPreview.transform.position = _nearestSocket.transform.position;
            _instanceOfSelectedObjectPreview.transform.rotation = _nearestSocket.transform.rotation;
            //_instanceOfSelectedObjectPreview.transform.forward = _nearestSocket.transform.forward;
        }
    }

    //Comprobar la distancia del cursor cuando llevamos un engranaje por si esta cerca de un hueco, hacer el snap del engranaje al hueco.
    private void CheckDistance()
    {
        _isObjectNearSocket = false;
        if (_nearestSocket != null) _nearestSocket = null;

        for (int i = 0; i < _sockets.Count; i++)
        {
            if (Vector3.Distance(_hitInfo.point, _sockets[i].transform.position) <= _snapDistance)
            {
                _nearestSocket = _sockets[i].transform;
                _isObjectNearSocket = true;
                _index = i;
            }
        }
    }

    private void CancelSelection()
    {
        Debug.LogWarning("Cancel Selection");

        RecoverObject();

        Destroy(_instanceOfSelectedObjectPreview);
        //_selectedObject.SetActive(true);
        _isObjectNearSocket = false;
        _nearestSocket = null;
        _holdingObject = false;
    }

    private void SelectObject()
    {
        Debug.LogWarning("Select Object");

        switch (_objectData.ID)
        {
            case "BigGear":
                if (_bigGearQuantity > 0) 
                {
                    _bigGearQuantity--;
                    break;
                }
                return;
            case "MediumGear":
                if (_mediumGearQuantity > 0)
                {
                    _mediumGearQuantity--;
                    break;
                }
                return;
            case "SmallGear":
                if (_smallGearQuantity > 0)
                {
                    _smallGearQuantity--;
                    break;
                }
                return;
        }

        _instanceOfSelectedObjectPreview = Instantiate(_objectData.PrefabToShow/*_selectedObjectPreview*/);
        _holdingObject = true;

        //_selectedObject.SetActive(false);
    }

    private void RemoveObject()
    {
        Debug.LogWarning("Remove Object");

        //_objectData.SelectableObject.SetActive(true);

        RecoverObject();

        _holdingObject = false;

        //_selectedObject.SetActive(false); SEGURAMENTE HAY QUE QUITAR ESTO

        _socketObjectID[_objectData.SocketIndex] = "Empty ID";

        //Guardar indice en la clase del objeto seleccionado para saber que bool hay que hacer false
        _socketStatus[_objectData.SocketIndex] = false;
        Destroy(_hitInfo.collider.gameObject);
    }

    private void RecoverObject()
    {
        switch (_objectData.ID)
        {
            case "BigGear":
                _bigGearQuantity++;
                break;
            case "MediumGear":
                _mediumGearQuantity++;
                break;
            case "SmallGear":
                _smallGearQuantity++;
                break;
        }
    }

    public void ReturnObject(string ID)
    {
        switch(ID)
        { 
        case "BigGear":
            _bigGearQuantity++;
            break;
        case "MediumGear":
            _mediumGearQuantity++;
            break;
        case "SmallGear":
            _smallGearQuantity++;
            break;
        }
    }

    private void ChangeSelection()
    {
        Debug.LogWarning("Change Selection");

        //Cambiar a otro objeto y mostrar uno y ocultar el otro

        PuzzleObjectData objectData = _hitInfo.collider.gameObject.GetComponent<PuzzleObjectData>();

        if (_objectData != null && objectData != null)
        {
            if (_objectData.ID != objectData.ID)
            {
                RecoverObject();

                Destroy(_instanceOfSelectedObjectPreview);

                CheckClickedObject();

                if (!_objectData.IsObjectPlaced) SelectObject();
                else if (_objectData.IsObjectPlaced) RemoveObject();
            }
        }
    }

    private void CheckSocket()
    {
        if (!_socketStatus[_index]) CheckObjectQuantity();
    }

    private void CheckObjectQuantity()
    {
        //Comprobar cuantas resistencias tenemos? Investigar a ver como hacerlo
        PutObject();
    }

    private void PutObject()
    {
        GameObject newObject = Instantiate(_objectData.PrefabToInstantiate);

        Gear gear = newObject.GetComponent<Gear>();

        gear.PuzzleGears = this;

        Vector3 spawnPos = new Vector3(_nearestSocket.position.x, _nearestSocket.position.y, (_nearestSocket.position.z - _offset));

        gear.NearestSocket = _nearestSocket;

        //Direccion en la que hay que mover el objeto
        gear.MovingDir =  spawnPos - _nearestSocket.position;

        gear.MovingDir.Normalize();

        newObject.transform.position = spawnPos;                  //Cambiamos la posicion del engranaje delante del hueco, con un margen.

        newObject.SetActive(true);
        PuzzleObjectData objectData = newObject.GetComponent<PuzzleObjectData>();

        //Guardo la referencia del objeto que tiene que activar en caso de quitar el objeto instanciado.
        //objectData.SelectableObject = _selectedObject;
        objectData.SocketIndex = _index;

        //_instantiatedObjects.Add(newObject);

        //Guardamos el estado en la lista para saber que hay un objeto colocado
        _socketStatus[_index] = true;

        //Guardamos el ID en la lista para saber que objeto es el que esta colocado
        //_socketObjectID[_index] = objectData.ID;

        _socketObjectID[_index] = _objectData.ID;

        //Destruir el preview del objeto
        Destroy(_instanceOfSelectedObjectPreview);

        _holdingObject = false;
        _isObjectNearSocket = false;
        _nearestSocket = null;
    }

    public RaycastHit GetRaycastHit()
    {
        return _hitInfo;
    }

    //Una vez el puzle se complete, cada instancia de engranaje se destruye para mostrar los engranajes hechos en Blender con la animación.
    public bool GetPuzzleStatus()
    {
        return _isPuzzleSolved;
    }

    public void SetSocketObjectID(int index, string id)
    {
        _socketObjectID[index] = id;
    }

    public void SetSocketStatus(int index, bool value)
    {
        _socketStatus[index] = value;
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.Puzzles.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            //El puzle esta hecho, cambiar variables, llamar metodos etc..
            _isPuzzleSolved = true;

            _audioSource.Play();

            GameObject[] gears = null;
            string row = "";

            switch (_randomRow)
            {
                case "First":
                    gears = _firstRowGearsToShow;
                    row = "IsFirstRowComplete";
                    break;

                case "Second":
                    gears = _secondRowGearsToShow;
                    row = "IsSecondRowComplete";
                    break;

                case "Third":
                    gears = _thirdRowGearsToShow;
                    row = "IsThirdRowComplete";
                    break;
            }

            foreach (GameObject gear in gears)
            {
                MeshRenderer meshRenderer = gear.GetComponent<MeshRenderer>();
                meshRenderer.enabled = true;
            }
            _animator.SetBool(row, true);
            _gameEvents.PuzzleSolved(this);

            GameObject PlayerGO = GameObject.FindGameObjectWithTag("Player");
            if (PlayerGO != null)
            {
                PlayerInventory playerInventory = PlayerGO.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    //Buscar el ItemStack que contenga el SO ItemData
                    foreach (ItemStack itemStack in playerInventory.Items)
                    {
                        if (itemStack != null)
                        {
                            ItemData itemData = itemStack.Item;
                            if (itemData == _item)
                            {
                                playerInventory.Items.Remove(itemStack);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
