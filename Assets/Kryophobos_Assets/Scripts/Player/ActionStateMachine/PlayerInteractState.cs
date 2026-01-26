using UnityEngine;

public class PlayerInteractState : PlayerState
{
    float _timer;
    public PlayerInteractState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        player.IsPlayerInteracting = true;

        player.Animator.SetTrigger("Interact");
    }

    public override void ExitState()
    {
        base.ExitState();

        _timer = 0;
        player.IsPlayerInteracting = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        _timer += Time.deltaTime;

        //
        if (_timer >= player.PickupDelay)
        {

            //Si es un objeto, recogerlo.
            if (player.IsPlayerNearItem) player.IDC.PickupItem();

            //Si es una nota, recogerla.
            else if (player.IsPlayerNearNote) player.IDC.PickupNote();

            //Si es una puerta, interactuar con ella.
            else if (player.IsPlayerNearDoor)
            {
                Door door = player.IDC.ClosestTrigger.GetComponentInParent<Door>();
                if (door != null) door.OpenDoor();
            }

            //Si es un puzle, interactuar con el.
            else if (player.IsPlayerNearPuzzle && player.PlayerHasRequiredItems)
            {
                Puzzle puzzle = player.IDC.ClosestTrigger.GetComponentInParent<Puzzle>();
                if (puzzle != null)
                {
                    puzzle.EnablePuzzle();
                    player.ActionStateMachine.ChangeState(player.DoingPuzzleState);
                }
            }

                //
                if (!player.IsPlayerDoingPuzzle) player.ActionStateMachine.ChangeState(player.NonActionState);
        }

        //


    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        InteractableDistanceChecker IDC = player.GetComponent<InteractableDistanceChecker>();
        Vector3 _nearestItem = IDC.ClosestTrigger.transform.position;

        Vector3 lookDirection = _nearestItem - player.transform.position;
        lookDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        Quaternion smoothedRotation = Quaternion.Slerp(player.RB.rotation, targetRotation, player.PickupRotationSpeed);
        player.RB.MoveRotation(smoothedRotation);
    }
}
