using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private float stepDelay = 0.45f;

    [SerializeField] private FootstepSet currentSurface;
    private Coroutine loop;

    private void Update()
    {
        //DetectSurface();
    }

    public void SetCurrentSurface(FootstepSet set)
    {
        currentSurface = set;
    }

    //Llamar desde animación
    public void PlayFootstepsSFX()
    {
        Debug.Log("Stepped, calling AudioManager PlayFootsetp method");
        AudioManager.Instance.PlayFootstep(currentSurface, transform.position);
    }

    /*
    public void StartFootsteps()
    {
        if (loop == null)
            loop = StartCoroutine(FootstepLoop());
        Debug.Log(this + " StartFootsteps");
    }

    public void StopFootsteps()
    {
        if (loop != null)
            StopCoroutine(loop);
            loop = null;
    }

    private IEnumerator FootstepLoop()
    {
        while (true)
        {
            Debug.Log("FootstepLoop");
            AudioManager.Instance.PlayFootstep(currentSurface, transform.position);
            yield return new WaitForSeconds(stepDelay);
        }
    }
    */

    /*
    private void DetectSurface()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            SurfaceType surface = hit.collider.GetComponent<SurfaceType>();
            if (surface != null)
            {
                currentSurface = surface.footstepSet;
            }
        }
    }
    */
}