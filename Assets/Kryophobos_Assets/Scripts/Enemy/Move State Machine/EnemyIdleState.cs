using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("Idle State");

        //Esta en idle y esta observando
        if (!enemy.PreparingAttack)
        {
            enemy.Animator.SetBool("IsTargetSpotted", false);
            enemy.Animator.SetBool("EndSearching", true);
        }
        

        //Se detiene el enemigo.
        enemy.GPS.velocity = Vector3.zero;

        //Si el enemigo esta configurado para que se pueda quedar quieto aleatoriamente al llegar a un waypoint, entonces llamamos a la corrutina..
        if (enemy.HoldingPosition) enemy.StartCoroutine(enemy.HoldingPositionTime());
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.GPS.isOnNavMesh) enemy.GPS.isStopped = true;

        //Si el enemigo no esta manteniendo la posición y tampoco se esta preparando para atacar ni esta atacando, entonces vuelve a patrullar.
        if (!enemy.HoldingPosition && !enemy.PreparingAttack && !enemy.Attacking)
        {
            enemy.MovementStateMachine.ChangeState(enemy.PatrolState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
