using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Header("Triggers que controlan luces")]
    public LightSharedManager[] triggers;

    private Dictionary<Light, int> contadorLuces = new Dictionary<Light, int>();

    private void Awake()
    {
        // Registrar todas las luces de todos los triggers
        foreach (LightSharedManager t in triggers)
        {
            if (t == null) continue;

            foreach (Light l in t.luces)
            {
                if (l == null) continue;

                if (!contadorLuces.ContainsKey(l))
                {
                    contadorLuces[l] = 0;
                    l.enabled = false;
                }
            }
        }
    }

    public void ActivarLuces(LightSharedManager trigger)
    {
        foreach (Light l in trigger.luces)
        {
            if (l == null) continue;

            contadorLuces[l]++;

            if (contadorLuces[l] == 1)
                l.enabled = true;
        }
    }

    public void DesactivarLuces(LightSharedManager trigger)
    {
        foreach (Light l in trigger.luces)
        {
            if (l == null) continue;

            contadorLuces[l]--;

            if (contadorLuces[l] <= 0)
            {
                contadorLuces[l] = 0;
                l.enabled = false;
            }
        }
    }
}
