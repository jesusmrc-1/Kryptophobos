using UnityEngine;

public class GiveFlashlight : MonoBehaviour
{
    [SerializeField] GameObject _flashlight;
    void Start()
    {
        Invoke("AddFlashlight",3f);
    }

    private void AddFlashlight()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) 
        {
            Player player = playerGO.GetComponent<Player>();
            if (player != null)
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                player.FlashLight = _flashlight.GetComponent<Flashlight>();
                if (inventory != null) inventory.HasFlashlight = true;

                _flashlight.SetActive(true);

                //GameManager.Instance.PlayerHasFlashlight = true;
            }
        }
    }
}
