using UnityEngine;

public class Door : TriggerBase
{
    [SerializeField] private string _ID;

    private Player _player;
    private PlayerInventory _inventory;

    [Tooltip("OPCIONAL: Desmarca esta casilla si quieres que la puerta no necesite objetos para abrirla.")]
    private bool _isDoorUnlocked;
    [Tooltip("Si se necesita un objeto (Scriptable Object) para abrir la puerta, entonces referenciarlo aquí.")]
    [SerializeField] private ItemData RequiredItem;

    [Tooltip("Aquí va el mensaje que se mostrara en el texto 3d diegetico cuando la puerta no se pueda abrir.")]
    [SerializeField] private string _lockedDoorText;

    [Tooltip("Aquí va el mensaje que se mostrara en el texto 3d diegetico cuando la puerta se pueda abrir.")]
    [SerializeField] private string _unlockedDoorText;

    [Tooltip("Aquí se referencia el Animator que pertenece a la puerta que queremos que se abra.")]
    [SerializeField] private Animator _animator;

    [SerializeField] private bool _isItemRequired;
    private ItemData _item;

    [SerializeField] private NewUIManager _newUIManager;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        _newUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<NewUIManager>();

        if (_isDoorUnlocked) DiegeticText = _unlockedDoorText;
        else DiegeticText = _lockedDoorText;

        LoadStatus();
    }

    private void Update()
    {
        if (_player.PlayerHasRequiredItems) DiegeticText = _unlockedDoorText;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        Debug.Log("OnTriggerEnter - DOOR");
        //Si se necesitan items, se comprueba que el jugador los tenga
        if (_isItemRequired) CheckRequiredItems();
        else _player.PlayerHasRequiredItems = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;
        Debug.Log("OnTriggerExit - DOOR");
        _player.PlayerHasRequiredItems = false;
    }

    void CheckRequiredItems()
    {
        Debug.Log("CheckRequiredItems");
        foreach (var item in _inventory.Items)
        {
            //Buscamos en el inventario comparando Scriptable Objects Item y RequiredItem, en caso de querer buscar mas objetos, hacer una lista con RequiredItems (Varios SO)
            if (item.Item == RequiredItem)
            {
                _player.PlayerHasRequiredItems = true;
            }
        }
    }

    public void OpenDoor()
    {
        //Animación puerta y desactivar trigger
        _animator.SetTrigger("Open");
        DiegeticText = "";



        //Borrar la llave al abrir la puerta
        ItemStack item = _inventory.Items.Find(itemStack => itemStack.Item == RequiredItem);
        _inventory.Items.Remove(item);

        //Se busca la ID de la puerta, si no esta, se añade.
        string _ID = GameManager.Instance.Doors.Find(_ID => _ID == this._ID);

        if (_ID != this._ID)
        {
            //Si la puerta esta configurada para cambiar el objetivo, lo cambia.
            if (_canChangeObjective) ChangeObjective();
            GameManager.Instance.Doors.Add(this._ID);
        }

        //Por ahora se destruye el trigger, si mas adelante hay puertas que puedas interactuar para cerrar ya se cambiara la forma de interactuar.
        Destroy(gameObject);
    }

    [Tooltip("Marca esta casilla si quieres que cuando se abra la puerta se cambie el objetivo.")]
    [SerializeField] private bool _canChangeObjective;
    [Tooltip("Aquí va el texto que mostrara el nuevo objetivo.")]
    [SerializeField][TextArea(1,3)] private string _newObjectiveText;

    public void ChangeObjective()
    {
        _newUIManager.ChangeObjective(_newObjectiveText);
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.Doors.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            OpenDoor();
        }
    }
}
