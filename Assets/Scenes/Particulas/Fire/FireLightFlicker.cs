using UnityEngine;

public class FireLightFlicker : MonoBehaviour
{
    [Header("Intensidad")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.4f;

    [Header("Velocidad del parpadeo")]
    public float flickerSpeed = 0.1f;

    private Light fireLight;
    private float targetIntensity;

    void Start()
    {
        fireLight = GetComponent<Light>();
        targetIntensity = fireLight.intensity;
    }

    void Update()
    {
        // Si estamos cerca del valor objetivo, elegimos otro
        if (Mathf.Abs(fireLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        // Interpolación suave para imitar el movimiento orgánico del fuego
        fireLight.intensity = Mathf.Lerp(
            fireLight.intensity,
            targetIntensity,
            flickerSpeed * Time.deltaTime * 60f
        );
    }
}
