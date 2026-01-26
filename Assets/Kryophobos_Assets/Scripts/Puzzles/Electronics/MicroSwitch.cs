using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicroSwitch : MonoBehaviour
{
    private NewUIManager _newUIManager;

    private EventsManager _gameEvents;

    private PCB _puzzle;

    private Animator _animator;
    [SerializeField] private Animator _redLEDAnimator;
    [SerializeField] private Animator _greenLEDAnimator;

    [SerializeField] private InputAction _press;

    private RaycastHit _hitInfo;
    private bool _isPuzzleSolved;

    [SerializeField] private float _holdingTime;

    [SerializeField] private string _newObjectiveText;

    [SerializeField] private float _endingDelayTime;
    void Start()
    {
        _puzzle = GetComponentInParent<PCB>();
        _animator = GetComponent<Animator>();
        _press.Enable();

        _newUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<NewUIManager>();
        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_puzzle != null)
        {
            if (_press.triggered)
            {
                _hitInfo = _puzzle.GetRaycastHit();

                if (_hitInfo.collider != null)
                {
                    MicroSwitch thisMicroSwitch = _hitInfo.collider.GetComponent<MicroSwitch>();

                    if (thisMicroSwitch != null)
                    {
                        _animator.SetBool("IsPressed", true);
                        _puzzle.CheckPuzzleStatus();
                        StopAllCoroutines();
                        StartCoroutine(HoldingButton());
                    }
                }
            }
        }

        //Si suelto el boton
        if (_press.ReadValue<float>() == 0) _animator.SetBool("IsPressed", false);
    }

    private IEnumerator HoldingButton()
    {
        float timer = 0f;

        while (_press.ReadValue<float>() == 1)
        {
            timer += Time.deltaTime;

            if (timer >= _holdingTime)
            {
                //Si el puzzle esta completado, entonces se enciende la luz, sino peta y se quitan las resistencias.
                _isPuzzleSolved = _puzzle.GetPuzzleStatus();

                if (!_isPuzzleSolved)
                {
                    //FALSE = LUZ ROJA ANIMADA, PETAN LAS RESISTENCIAS, SE REINICIA.
                    if (_redLEDAnimator != null) _redLEDAnimator.SetTrigger("On");
                    _puzzle.ResetPuzzle();
                }
                else if (_isPuzzleSolved)
                {
                    //TRUE =  LUZ VERDE ANIMADA, FUNCIONA.
                    if (_greenLEDAnimator != null) _greenLEDAnimator.SetTrigger("On");
                    StartCoroutine(Delay());
                }

                break;
            }
            yield return null;
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(_endingDelayTime);

        _gameEvents.PuzzleSolved(_puzzle);
        _newUIManager.ChangeObjective(_newObjectiveText);
    }
}
