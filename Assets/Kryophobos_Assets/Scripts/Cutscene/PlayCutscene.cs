using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class PlayCutscene : MonoBehaviour
{
    private PlayVideo _playVideo;
    [SerializeField] private VideoClip _clip;
    [SerializeField] private GameObject _playerFinalPosition;
    [SerializeField] private bool _modifyPlayerPosition;

    private void Start()
    {
        _playVideo = GameObject.FindGameObjectWithTag("VideoPlayer").GetComponent<PlayVideo>();
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
            Destroy(gameObject);
        }
    }
}
