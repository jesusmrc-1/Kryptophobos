using UnityEngine;

public class GiveItemData : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                if (_itemData != null) inventory.AddItem(_itemData);
            }
        }
    }
}
