using UnityEngine;

public class EnemySpawnState : EnemyState
{
    public EnemySpawnState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsEnemyOnGround) 
        {
            enemy.GPS.enabled = true;
            enemy.RB.useGravity = false;
            enemy.RB.isKinematic = true;
            enemy.FollowTime = Mathf.Infinity;
            enemy.MovementStateMachine.Initialize(enemy.FollowTargetState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
