using UnityEngine;

public class EndingButton : TriggerBase
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    private bool _isFinished;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenDoor(Player player)
    {
        if (!_isFinished)
        {
            player.IsPlayerNearButton = false;
            _isFinished = true;
            _animator.Play("MoveBars");
            _boxCollider.enabled = false;
            InteractableDistanceChecker IDC = player.GetComponent<InteractableDistanceChecker>();
            IDC.ClearColliderList();
        }
    }
}
