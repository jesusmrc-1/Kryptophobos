using UnityEngine;

public class ForceFlashlight : MonoBehaviour
{
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private GameObject _staticFlashlight;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();

            if (player != null)
            {
                player.FlashLight = _flashlight.GetComponent<Flashlight>();
            }

            if (inventory != null)
            {
                inventory.HasFlashlight = true;
            }

            _flashlight.SetActive(true);
            GameManager.Instance.PlayerHasFlashlight = true;

            if (_staticFlashlight != null) Destroy(_staticFlashlight);
        }
    }
}
