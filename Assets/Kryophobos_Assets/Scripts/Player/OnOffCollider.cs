using UnityEngine;

public class OnOffCollider : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void DisableCollider()
    {
        if(_collider != null) _collider.enabled = false;
    }

    public void EnableCollider()
    {
        if (_collider != null) _collider.enabled = true;
    }
}
