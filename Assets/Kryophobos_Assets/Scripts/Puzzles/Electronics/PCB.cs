using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PCB : PuzzleBase
{
    [SerializeField] private string _ID;
    [SerializeField] private ItemData _item;

    [SerializeField] private List<string> _objectIDValues;

    private EventsManager _gameEvents;

    [SerializeField] private InputAction _select;
    [SerializeField] private InputAction _cancelSelection;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignorePlayer;

    [Tooltip("La capa de mascara que tiene que ignorar el raycast.")]
    [SerializeField] private LayerMask _ignoreEnemy;

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
    [SerializeField] private List <bool> _socketStatus = new List<bool>();
    [SerializeField] private List<string> _socketObjectID = new List<string>();
    [SerializeField] private List<string> _socketObjectIDSolution = new List<string>();

    [SerializeField] private float _snapDistance = 0.1f;

    [SerializeField] private PuzzleObjectData _objectData;
    [SerializeField] private GameObject _selectedObject;
    private GameObject _selectedObjectPreview;
    private GameObject _selectedObjectToInstantiate;

   [SerializeField] private GameObject _instanceOfSelectedObjectPreview;

    [SerializeField] private int _index;

    //Configurable según puzle
    [SerializeField] private bool RandomizeObjectQuantity;
    [SerializeField] private bool RandomizeSolution;
    [SerializeField] private bool _isPuzzleSolved;

    [SerializeField] private GameObject[] _puzzleObjects;
    [SerializeField] private List <GameObject> _instantiatedObjects = new List<GameObject>();

    [SerializeField] private GameObject[] _resistorValuesUI;

    private ScaleOnRayhit _highlightedObject;
    private RaycastHit _lastRaycastHit;

    private void Awake()
    {
        //DM_Prop_Resistor_B_#
        //01 ID = 1.2M
        //02 ID = 12M
        //03 ID = 3.1G
        //04 ID = 120K
        //05 ID = 310

        //Si no hay valores en el inspector, entonces por defecto tiene estos.
        _objectIDValues = new List<string>() { "1.2M", "12M", "3.1G", "120K", "310" };
    }

    private void Start()
    {
        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();

        _select.Enable();
        _cancelSelection.Enable();

        //Add a socket occupied status (bool) for each socket in '_sockets' list. (If we add more sockets to the puzzle, it will increment)
        //Add an empty ID for each socket in '_sockets' list. (If we add more sockets to the puzzle, it will increment)
        if (_sockets.Count > 0)
        {
            foreach (var socket in _sockets)
            {
                _socketStatus.Add(false);
                _socketObjectID.Add("Empty ID");
            }
        }

        LoadStatus();

        if (!_isPuzzleSolved)
        {
            if (RandomizeSolution) RandomizePuzzleSolution();

            this.enabled = false;
        }
    }

    private void RandomizePuzzleSolution()
    {
        //Aleatorizar las ID's según la cantidad de objetos necesarios (_sockets), cuando tenga la misma cantidad que los huecos que se pueden llenar, entonces sale
        while (_socketObjectIDSolution.Count < _sockets.Count)
        {
            //Aleatorizar un valor de la lista
            int rng = Random.Range(0, _objectIDValues.Count);
            string randomString = _objectIDValues[rng];

            //Si el valor generado aleatorio no existe en la lista, entonces se añade
            if (!_socketObjectIDSolution.Contains(randomString))
            {
                _socketObjectIDSolution.Add(randomString);
            }
        }

        //Asignar el valor de las resistencias a cada texto del array '_resistorValuesUI'.
        for (int i = 0; i < _socketObjectID.Count; i++)
        {
            TextMeshProUGUI textMeshProUGUI = _resistorValuesUI[i].GetComponent<TextMeshProUGUI>();
            if (textMeshProUGUI != null) textMeshProUGUI.text = _socketObjectIDSolution[i];
        }
    }

    //Si coinciden los ID de '_socketObjectIDSolution' comparados con los de '_socketObjectID', significa que cada objeto esta colocado
    //en la posición correcta, entonces '_isPuzzleSolved' es true.
    public void CheckPuzzleStatus()
    {
        if (!_isPuzzleSolved)
        {
            _isPuzzleSolved = true;

            for (int i = 0; i < _socketObjectIDSolution.Count; i++)
            {
                if (_socketObjectID[i] != _socketObjectIDSolution[i]) 
                {
                    _isPuzzleSolved = false;
                    return;
                }
            }

            GameManager.Instance.Puzzles.Add(_ID);
        }
    }

    private void Update()
    {
        if (!_isPuzzleSolved)
        {
            //ALL CODE

            //CheckPuzzleStatus(); EL MICRO SWITCH SE ENCARGA DE LLAMAR A LA FUNCION

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
                ChangeSelection();
            }
        }
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
        LayerMask ignored = _ignorePlayer | _ignoreEnemy | _ignoreRaycast | _ignoreInteractable;   //Esto es igual a 11000100

        //Invertimos el valor para que las capas en esos bits sean ignoradas ' = 0 '
        int mask = ~ignored;                                                  //Esto es igual a 00111011

        _hasRayImpacted = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~ignored); //El simbolo '~' invierte, entonces ignora lo que este en esa mascara.

        if (_hasRayImpacted)
        {
            _hitInfo = hitInfo;

            if (_hitInfo.collider != null && _lastRaycastHit.collider != null)
            {
                if (_lastRaycastHit.collider != _hitInfo.collider)
                {
                    //Desescalar el objeto y borrar su referencia
                    if (_highlightedObject != null)
                    {
                        _highlightedObject.ScaleObject = false;
                        _highlightedObject.ResetScale();
                        _highlightedObject = null;
                    }
                }
            }
            
            _lastRaycastHit = hitInfo;

            _highlightedObject = _hitInfo.collider.gameObject.GetComponent<ScaleOnRayhit>();

            if (_highlightedObject != null)
            {
                //Escalar objeto
                _highlightedObject.ScaleObject = true;
            }
        }
    }

    private void CheckClickedObject()
    {
        Debug.LogWarning("Clicked to: " + _hitInfo.collider.name);

        _objectData = _hitInfo.collider.gameObject.GetComponent<PuzzleObjectData>();

        if (_objectData != null && !_objectData.IsObjectPlaced) 
        {
            //SI TENEMOS YA EL OBJECT DATA, ENTONCES NO HACE FALTA GUARDARSE NADA, PODEMOS USAR EN LOS DEMAS SITIOS DIRECTAMENTE _objectData.LOQUESEA (PROBAR A VER QUE TAL)

            _selectedObject = _hitInfo.collider.gameObject;
            _selectedObjectPreview = _objectData.PrefabToShow;
            _selectedObjectToInstantiate = _objectData.PrefabToInstantiate;
        }
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
                _instanceOfSelectedObjectPreview.transform.position = _hitInfo.point;
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
            //IGNORAR COLISIONES MIENTRAS LLEVAMOS RESISTENCIA CON LAS COLISIONES DE LAS RESISTENCIAS SELECCIONABLES???
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

        Destroy(_instanceOfSelectedObjectPreview);
        _selectedObject.SetActive(true);
        _isObjectNearSocket = false;
        _nearestSocket = null;
        _holdingObject = false;
    }

    private void SelectObject()
    {
        Debug.LogWarning("Select Object");

        _instanceOfSelectedObjectPreview = Instantiate(_objectData.PrefabToShow/*_selectedObjectPreview*/);
        _holdingObject = true;

        _selectedObject.SetActive(false);
    }

    private void RemoveObject()
    {
        Debug.LogWarning("Remove Object");

        if (_objectData.SelectableObject != null) _objectData.SelectableObject.SetActive(true);
        _holdingObject = false;

        //_selectedObject.SetActive(false); SEGURAMENTE HAY QUE QUITAR ESTO

        _socketObjectID[_objectData.SocketIndex] = "Empty ID";

        //Guardar indice en la clase del objeto seleccionado para saber que bool hay que hacer false
        _socketStatus[_objectData.SocketIndex] = false;
        Destroy(_hitInfo.collider.gameObject);
    }

    private void ChangeSelection()
    {
        Debug.LogWarning("Change Selection");

        //Cambiar a otro objeto y mostrar uno y ocultar el otro

        PuzzleObjectData objectData = _hitInfo.collider.gameObject.GetComponent<PuzzleObjectData>();

        if (objectData != null)
        {
            _selectedObject.SetActive(true);
            Destroy(_instanceOfSelectedObjectPreview);

            CheckClickedObject();

            if (!_objectData.IsObjectPlaced) SelectObject();
            else if (_objectData.IsObjectPlaced) RemoveObject();
        }
    }

    private void CheckSocket()
    {
        if (!_socketStatus[_index]) CheckObjectQuantity();
    }

    private void CheckObjectQuantity()
    {
        PutObject();
    }

    private void PutObject()
    {
        GameObject newObject = Instantiate(_objectData.PrefabToInstantiate /*_selectedObjectToInstantiate*/, _nearestSocket.transform.position, _nearestSocket.transform.rotation);
        newObject.SetActive(true);
        PuzzleObjectData objectData = newObject.GetComponent<PuzzleObjectData>();

        //Guardo la referencia del objeto que tiene que activar en caso de quitar el objeto instanciado.
        objectData.SelectableObject = _selectedObject;
        objectData.SocketIndex = _index;

        _instantiatedObjects.Add(newObject);

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

    public void ResetPuzzle()
    {
        //Cada objeto del puzle se vuelve a activar
        foreach (var obj in _puzzleObjects) if (obj != null) obj.SetActive(true);

        //Cada objeto instanciado se elimina
        foreach (var obj in _instantiatedObjects)
        {
            if (obj != null)
            {
                //Pentan las resistencias
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null) meshRenderer.enabled = false;

                DestroyResistor destroyResistor = obj.GetComponent<DestroyResistor>();
                if (destroyResistor != null) destroyResistor.DestroyResistorComponent();
            }
        }
        _instantiatedObjects.Clear();

        //El estado de cada socket se reinicia
        for (int i = 0; i < _socketStatus.Count; i++) _socketStatus[i] = false;

        //El ID de cada socket se vacía
        for (int i = 0; i < _socketObjectID.Count; i++) _socketObjectID[i] = "Empty ID";
    }

    public RaycastHit GetRaycastHit()
    {
        return _hitInfo;
    }

    public bool GetPuzzleStatus()
    {
        return _isPuzzleSolved;
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.Puzzles.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            //El puzle esta hecho, cambiar variables, llamar metodos etc..
            _isPuzzleSolved = true;
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
                            ItemData itemData = itemStack.Item ;
                            if (itemData == _item)
                            {
                                playerInventory.Items.Remove(itemStack);
                                GameManager.Instance.PlayerItems.Remove(itemStack);
                                break;
                            }
                        }
                    }
                }
            }
            _gameEvents.PuzzleSolved(this);
        }
    }

    public void DisableInputs()
    {
        _select.Disable();
        _cancelSelection.Disable();
    }
}
