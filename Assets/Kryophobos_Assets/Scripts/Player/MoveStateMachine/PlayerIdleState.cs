using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        player.Animator.SetBool("playerMoving", false);

        if (!player.inventory.HasFlashlight) player.Animator.SetBool("hasLantern", false);
        else if (player.inventory.HasFlashlight)
        {
            player.Animator.SetBool("hasLantern", true);
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        player.Animator.SetBool("playerMoving", true);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (!player.inventory.Flashlight.HasLanternFullyCharged()) if (player.Move.ReadValue<Vector2>() != Vector2.zero && !player.IsPlayerInteracting && !player.IsPlayerDoingPuzzle) player.MovementStateMachine.ChangeState(player.MoveState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
    }
}
