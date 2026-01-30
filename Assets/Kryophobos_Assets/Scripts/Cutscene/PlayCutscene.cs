using Unity.Loading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class PlayCutscene : MonoBehaviour
{
    [SerializeField] private string _ID;

    private PlayVideo _playVideo;
    [SerializeField] private VideoClip _clip;
    [SerializeField] private GameObject _playerFinalPosition;
    [SerializeField] private bool _modifyPlayerPosition;

    private void Start()
    {
        _playVideo = GameObject.FindGameObjectWithTag("VideoPlayer").GetComponent<PlayVideo>();

        LoadStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            _playVideo.PlayVideoClip(_clip);
            if (_modifyPlayerPosition) 
            {
                other.gameObject.transform.position = _playerFinalPosition.transform.position;
                other.gameObject.transform.rotation = _playerFinalPosition.transform.rotation;
            }
            other.gameObject.transform.position = _playerFinalPosition.transform.position;
            GameManager.Instance.CutsceneTriggers.Add(_ID);
            Destroy(gameObject);
        }
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.CutsceneTriggers.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            Destroy(gameObject);
        }
    }
}
