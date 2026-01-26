using UnityEngine;
using UnityEngine.InputSystem;

public class particleTrigger : MonoBehaviour
{
    public InputAction trigger;
    public InputAction reset;
    public GameObject particle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particle.SetActive(false);
        trigger.Enable();
        reset.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger.triggered)
        {
            particle.SetActive(true);
        }

        if (reset.triggered)
        {
            particle.SetActive(false);
        }
    }
}
