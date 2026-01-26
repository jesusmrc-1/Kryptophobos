using UnityEngine;

public class BlockWay : MonoBehaviour
{
    [SerializeField] private GameObject _mesh;
    [SerializeField] private GameObject _collider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            //Instanciar objeto que bloquee un camino
            if (_mesh != null) _mesh.SetActive(true);
            if (_collider != null) _collider.SetActive(true);
        }
    }
}
