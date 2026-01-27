using UnityEngine;

public class ColdBreath : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public void Breath()
    {
        if (_particleSystem != null)
        {
            Debug.Log("Breathing");
            _particleSystem.Play();
            //_particleSystem.Stop(true,ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
