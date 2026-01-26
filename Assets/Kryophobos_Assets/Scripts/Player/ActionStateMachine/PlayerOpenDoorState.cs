using UnityEngine;

public class PlayerOpenDoorState : PlayerState
{
    public PlayerOpenDoorState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        //Animacion de abrir puerta
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
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
