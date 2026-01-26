using UnityEngine;

public class ItemPickup : TriggerBase
{
    [Tooltip("Aquí se referencia el objeto (Scriptable Object) que da al jugador cuando se recoje.")]
    [SerializeField] private ItemData _itemData;

    private Player _player;
    private PlayerInventory _playerInventory;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        LoadStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearItem = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearItem= false;
    }

    public void PickupItem()
    {
        if (_itemData != null) _playerInventory.AddItem(_itemData);
        else if (_itemData == null) Debug.LogError("ERROR: El trigger no tiene referenciado un item (Scriptable Object).");

        Destroy(gameObject);
    }

    private void LoadStatus()
    {
        ItemData itemData = null;

        foreach (ItemStack item in GameManager.Instance.PlayerItems)
        {
             itemData = item.Item;

            if (itemData == _itemData)
            {
                Destroy(gameObject);
            }
        }
    }
}