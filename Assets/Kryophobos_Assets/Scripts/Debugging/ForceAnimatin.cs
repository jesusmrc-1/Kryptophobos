using UnityEngine;

public class ForceAnimatin : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        anim.Play("finalCam");
    }
}
