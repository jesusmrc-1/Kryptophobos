using UnityEngine;

public class EnemyReturnPositionState : EnemyState
{
    public EnemyReturnPositionState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.FollowingTarget = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Vuelve a la posición inicial.
        enemy.GPS.SetDestination(enemy.StartingPosition);

        //Cuando este cerca de la posición inicial, cambia de estado.
        float distance = Vector3.Distance(enemy.transform.position, enemy.StartingPosition);

        if(distance <= 0.1f)
        {
            enemy.MovementStateMachine.ChangeState(enemy.StandStillState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
