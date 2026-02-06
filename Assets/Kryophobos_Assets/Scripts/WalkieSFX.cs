using UnityEngine;

public class WalkieSFX : MonoBehaviour
{
    [SerializeField] private AudioSource WalkieBeeps;
    [SerializeField] private AudioSource WalkieNoise;

    public void WalkieBeeping()
    {
        if (WalkieBeeps != null) WalkieBeeps.Play();
    }

    public void WalkieNoises()
    {
        if (WalkieNoise != null) WalkieNoise.Play();
    }

    public void StopNoises()
    {
        if (WalkieNoise != null) WalkieNoise.Stop();
    }
}
