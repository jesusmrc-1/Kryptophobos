using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    //REMINDER: HABRÁ QUE TENER EN CUENTA CUANDO HAYAN ENEMIGOS, PUEDE QUE NO FUNCIONE CORRECTAMENTE Y HABRÁ QUE CAMBIAR CODIGO.

    [Tooltip("Aquí se referencia el Animator del objeto que tiene una animación, ya sea una puerta o un puzle.")]
    [SerializeField] private Animator _animator;

    [Tooltip("El tiempo que tiene que transcurrir una vez el sensor no detecte presencia, configurarlo a gusto personal.")]
    [SerializeField] private float _sensorTimerDelay;

    [SerializeField] private bool _doorLocked;

    private float _remainingTime;

    //[SerializeField] private string _ID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer") || 
            other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _remainingTime = _sensorTimerDelay;
            if (!_doorLocked) OpenDoor();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer") ||
        other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _remainingTime = _sensorTimerDelay;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer") ||
        other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {        
            if (!_doorLocked)
            {
                StopAllCoroutines();
                StartCoroutine(Decrease_remainingTime());
            }
        }
    }

    public void UnlockDoor()
    {
        _doorLocked = false;
        //_gameManager.Doors.Add(_ID);
    }

    void OpenDoor()
    {
        if (_animator != null) _animator.SetBool("DoorStatus", true);
    }

    void CloseDoor()
    {
        if (_animator != null) _animator.SetBool("DoorStatus", false);
    }

    IEnumerator Decrease_remainingTime()
    {
        while (true)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0f)
            {
                CloseDoor();
                yield break; //Detener corrutina
            }
            yield return null; //Esperar al siguiente frame
        }
    }
}
