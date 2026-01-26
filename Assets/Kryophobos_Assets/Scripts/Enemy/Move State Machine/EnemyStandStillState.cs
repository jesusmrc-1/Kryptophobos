using UnityEngine;

public class EnemyStandStillState : EnemyState
{
    private float _angle;
    public EnemyStandStillState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        //Esta en idle y esta observando
        if (!enemy.PreparingAttack)
        {
            enemy.Animator.SetBool("IsTargetSpotted", false);
            enemy.Animator.SetBool("EndSearching", true);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el angulo que hay entre la dirección en la que mira el enemigo y la dirección en la que miraba en la posición inicial, es mayor de 0.1
        //entonces rotar hasta que mire en la dirección inicial.
        _angle = Vector3.Angle(enemy.transform.forward,enemy.LookingDirection);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (_angle >= 0.1f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(enemy.LookingDirection.normalized);
            Quaternion smoothedRotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.RotationSpeed);
            enemy.RB.MoveRotation(smoothedRotation);
        }
    }
}
