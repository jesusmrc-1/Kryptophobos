
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] float _blindTime;
    [SerializeField] GameObject rayOrigin;
    [SerializeField] private LayerMask IgnorePlayer;
    [SerializeField] private LayerMask IgnoreEnemyVision;
    [SerializeField] private LayerMask IgnoreRaycast;
    [SerializeField] private LayerMask IgnoreInteractable;
    private int _ignoreLayerMasks;

    private void Awake()
    {
        LayerMask ignoredMasks = IgnorePlayer | IgnoreEnemyVision | IgnoreRaycast | IgnoreInteractable;
        _ignoreLayerMasks = ~ignoredMasks;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;

        Vector3 rayDirection = other.transform.position - rayOrigin.transform.position;
        float rayDistance = rayDirection.magnitude;
        rayDirection.Normalize();

        Debug.DrawRay(rayOrigin.transform.position, rayDirection * rayDistance, Color.red, 1f);

        if (Physics.Raycast(
            rayOrigin.transform.position,
            rayDirection,
            out RaycastHit hitInfo,
            rayDistance,
            _ignoreLayerMasks,
            QueryTriggerInteraction.Ignore
        ))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.LogWarning("Enemigo flasheado");
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.StartCoroutine(enemy.Blind(_blindTime));
            }
        }
    }
}
