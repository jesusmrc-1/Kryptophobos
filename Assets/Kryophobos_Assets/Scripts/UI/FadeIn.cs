using UnityEngine;

public class FadeIn : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.Play("FadeIn");
    }
}
