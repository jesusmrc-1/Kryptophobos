using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public string Player = "Player";

    [Header("Referencia al LightManager global")]
    public LightManager manager;

    private LightSharedManager shared;

    private void Awake()
    {
        // Buscar el LightSharedManager en el objeto PADRE
        shared = GetComponentInParent<LightSharedManager>();

        if (shared == null)
            Debug.LogError("No se encontró LightSharedManager en el padre de: " + gameObject.name);

        if (manager == null)
            Debug.LogError("No se asignó LightManager en: " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Player)) return;
        if (shared == null || manager == null) return;

        manager.ActivarLuces(shared);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Player)) return;
        if (shared == null || manager == null) return;

        manager.DesactivarLuces(shared);
    }
}
