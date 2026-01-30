using UnityEngine;

public class DisableVFX : MonoBehaviour
{
    [SerializeField] private GameObject[] _vfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            foreach (var vfx in _vfx)
            {
                if (vfx != null)
                {
                    vfx.SetActive(false);
                }
            }
        }
    }
}
