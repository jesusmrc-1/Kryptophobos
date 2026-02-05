using UnityEngine;

public class OnOffGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void DisableGravity()
    {
        if (_rigidBody != null) _rigidBody.useGravity = false;
    }

    public void EnableGravity()
    {
        if (_rigidBody != null) _rigidBody.useGravity = true;
    }
}
