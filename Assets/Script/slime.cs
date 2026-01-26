using UnityEngine;

public class slime : MonoBehaviour
{
    public Transform target;          // object you're following
    public LayerMask groundMask;
    public float rayDistance = 5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hit, rayDistance, groundMask))
        {
            transform.position = hit.point + Vector3.up * 0.02f; // slight offset to avoid z-fighting
        }
    }
}
