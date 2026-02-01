using System.Collections.Generic;
using UnityEngine;

public class LightSharedManager : MonoBehaviour
{
    [Header("Todas las luces compartidas")]
    public Light[] luces;

    private Dictionary<Light, int> lightCounters = new Dictionary<Light, int>();

    private void Awake()
    {
        foreach (Light l in luces)
        {
            if (l == null) continue;

            lightCounters[l] = 0;
            l.enabled = false; // apagadas al inicio
        }
    }

    // Cuando un trigger necesita estas luces
    public void ActivateLights()
    {
        foreach (Light l in luces)
        {
            if (l == null) continue;

            lightCounters[l]++;

            if (lightCounters[l] == 1)
                l.enabled = true;
        }
    }

    // Cuando un trigger deja de necesitarlas
    public void DeactivateLights()
    {
        foreach (Light l in luces)
        {
            if (l == null) continue;

            lightCounters[l]--;

            if (lightCounters[l] <= 0)
            {
                lightCounters[l] = 0;
                l.enabled = false;
            }
        }
    }
}
