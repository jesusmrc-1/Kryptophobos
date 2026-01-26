using UnityEngine;

public class PlayerDoingPuzzleState : PlayerState
{
    public PlayerDoingPuzzleState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        player.IsPlayerDoingPuzzle = true;
    }

    public override void ExitState()
    {
        base.ExitState();

        Puzzle puzzle = player.IDC.ClosestTrigger.GetComponentInParent<Puzzle>();
        puzzle.DisablePuzzle();

        player.IsPlayerDoingPuzzle = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (player.Exit.triggered) player.ActionStateMachine.ChangeState(player.NonActionState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
