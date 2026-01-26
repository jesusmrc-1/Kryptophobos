using UnityEngine;

public class Flash : MonoBehaviour
{
    /*
    private Player _player;
    [SerializeField] float _blindTime;
    [SerializeField] GameObject rayOrigin;
    [SerializeField] private LayerMask IgnorePlayer;
    [SerializeField] private LayerMask IgnoreEnemyVision;
    [SerializeField] private LayerMask IgnoreRaycast;
    [SerializeField] private LayerMask IgnoreInteractable;
    private int _ignoreLayerMasks;
    [SerializeField] private float _xOffset;

    private void Awake()
    {
        LayerMask ignoredMasks = IgnorePlayer | IgnoreEnemyVision | IgnoreRaycast | IgnoreInteractable;
        _ignoreLayerMasks = ~ignoredMasks;
    }

    private void Start()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            _player = playerGO.GetComponent<Player>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;

        Vector3 rayDirection = other.transform.position - rayOrigin.transform.position;
        rayDirection.x += _xOffset;

        float rayDistance = rayDirection.magnitude;
        rayDirection.Normalize();

        Debug.DrawRay(rayOrigin.transform.position, rayDirection * rayDistance, Color.red, 5f);

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
                if (enemy != null)
                {
                    if (!enemy.IsEnemyFlashed)
                    {
                        enemy.IsEnemyFlashed = true;
                        enemy.TargetLastKnownPosition = _player.gameObject.transform.position;
                        enemy.StartCoroutine(enemy.Blind(_blindTime));
                    }
                }
            }
        }
    }
    */
}
