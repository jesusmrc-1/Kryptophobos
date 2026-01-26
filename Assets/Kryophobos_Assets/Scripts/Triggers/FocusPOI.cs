using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FocusPOI : MonoBehaviour
{
    private GameObject _mainCamera;

    [SerializeField] private GameObject _previousCamera;
    [SerializeField] private GameObject _triggerCamera;

    [SerializeField] private float _focusDuration;
    [SerializeField] private float _focusBlendTime;
    [SerializeField] private float _unfocusBlendTime;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            player.DisableInputs();

            //Buscar la camara activa y guardar referencia en _previousCamera y activar _activeCamera.
            _previousCamera = GameObject.FindGameObjectWithTag("CM_Camera");

            CinemachineBrain cinemachineBrain = _mainCamera.GetComponent<CinemachineBrain>();
            cinemachineBrain.DefaultBlend.Time = _focusBlendTime;

            _triggerCamera.SetActive(true);
            _previousCamera.SetActive(false);

            GameObject eventsManagerGO = GameObject.FindGameObjectWithTag("GameEventsManager");
            EventsManager eventsManager = eventsManagerGO.GetComponent<EventsManager>();

            eventsManager.FocusingPOI();

            StartCoroutine(Timer(cinemachineBrain, eventsManager, player));
        }
    }

    private IEnumerator Timer(CinemachineBrain cinemachineBrain, EventsManager eventsManager, Player player)
    {
        yield return new WaitForSeconds(_focusDuration);

        cinemachineBrain.DefaultBlend.Time = _unfocusBlendTime;

        _previousCamera.SetActive(true);
        _triggerCamera.SetActive(false);

        eventsManager.UnfocusingPOI();

        player.EnableInputs();

        Destroy(gameObject, _unfocusBlendTime);
    }
}
