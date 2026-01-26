using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string _ID;
    [SerializeField] private GameObject _spawnCoords;
    [SerializeField] private GameObject _lastActiveCamera;

    private void Start()
    {
        LoadStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            GameManager.Instance.CheckpointTriggers.Add(_ID);
            GameManager.Instance.PlayerPosition = _spawnCoords.transform.position;
            GameManager.Instance.LastActiveCamera = _lastActiveCamera.name;
            Destroy(gameObject);
        }
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.NewObjectiveTriggers.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            Destroy(gameObject);
        }
    }
}
