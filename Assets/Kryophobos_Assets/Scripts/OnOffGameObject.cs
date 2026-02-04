using UnityEngine;

public class OnOffGameObject : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjects;

    private void Awake()
    {
        ChangeGameObjectStatus(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            ChangeGameObjectStatus(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            ChangeGameObjectStatus(false);
        }
    }

    private void ChangeGameObjectStatus(bool status)
    {
        foreach (GameObject go in gameObjects)
        {
            if (go != null) go.SetActive(status);
        }
    }
}
