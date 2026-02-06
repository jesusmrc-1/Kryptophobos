using UnityEngine;

public class StopSFX : MonoBehaviour
{
    [SerializeField] private AmbienceManager _ambienceManager;

    private void Start()
    {
        GameObject AmbienceManagerGO = GameObject.FindGameObjectWithTag("Player");
        if (AmbienceManagerGO != null )
        {
            _ambienceManager = AmbienceManagerGO.GetComponent<AmbienceManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            if (_ambienceManager != null)
            {
                _ambienceManager.StopPlayingEnemySFX = true;
                _ambienceManager.StopEnemySounds();
            }
        }
    }
}
