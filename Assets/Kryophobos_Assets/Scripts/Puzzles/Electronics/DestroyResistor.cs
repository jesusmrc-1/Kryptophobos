using UnityEngine;

public class DestroyResistor : MonoBehaviour
{
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private ParticleSystem _flash;
    [SerializeField] private ParticleSystem _fire;
    [SerializeField] private ParticleSystem _smoke;

    public void DestroyResistorComponent()
    {
        if (_sparks != null &&
            _flash != null &&
            _fire != null &&
            _smoke != null)
        {
            _sparks.Play();
            _flash.Play();
            _fire.Play();
            _smoke.Play();
        }
        Destroy(gameObject, 2f);
    }
}
