using System.Collections;
using UnityEngine;

public class Flickering : MonoBehaviour
{
    [Header("⚙️ Configuración del parpadeo")]
    [SerializeField] private float _minFlickeringTime = 0.3f;
    [SerializeField] private float _maxFlickeringTime = 2f;

    private Light _lightBulb;
    private void Start()
    {
        _lightBulb = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        //El estado activo del componente se invierte cada # segundos (time).
        while (true)
        {
            float time = Random.Range(_minFlickeringTime, _maxFlickeringTime);

            _lightBulb.enabled = !_lightBulb.enabled;

            yield return new WaitForSeconds(time);
        }
    }
}
