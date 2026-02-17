using UnityEngine;

public class BlockWay : MonoBehaviour
{
    [SerializeField] private GameObject _mesh;
    [SerializeField] private GameObject _boxCollider;
    public GameObject[] BlockingAntennaParts;
    public BoxCollider _collider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            //Instanciar objeto que bloquee un camino
            if (_mesh != null) 
            {
               // _mesh.SetActive(true);
                foreach (var obj in BlockingAntennaParts)
                {
                    if (obj != null)
                    {
                        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                        {
                            meshRenderer.enabled = true;
                        }
                    }
                }
                if (_collider != null) _collider.enabled = true;
            }

            //if (_collider != null) _boxCollider.SetActive(true);
        }
    }
}
