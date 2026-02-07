using UnityEngine;

public class EnableGameObject : MonoBehaviour
{
    public GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            enemy.SetActive(true);
        }

    }
}
