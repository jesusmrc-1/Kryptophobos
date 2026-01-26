using UnityEngine;

public class EnemyAttackingState : EnemyState
{
    private float _timer;
    public EnemyAttackingState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("Attacking State");

        Vector3 lookingDirection = enemy.Player.transform.position - enemy.transform.position;

        lookingDirection.y = 0f;

        lookingDirection.Normalize();

        //Hacer dash y activar el trigger de ataque (Parar navmesh o desactivarla, despues de dash reactivar o reanudar e incluso recalcular camino)
        enemy.AttackingTrigger.enabled = true;
        enemy.GPS.enabled = false;
        enemy.RB.useGravity = true;
        enemy.RB.isKinematic = false;
        enemy.RB.AddForce(lookingDirection * enemy.AttackImpulse, ForceMode.Impulse);
        enemy.RB.constraints = RigidbodyConstraints.FreezeRotation;

        enemy.Attacking = true;

        
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.RB.useGravity = false;
        enemy.RB.isKinematic = true;
        enemy.AttackingTrigger.enabled = false;
        enemy.GPS.enabled = true;
        enemy.TargetLastKnownPosition = enemy.Player.transform.position;
        enemy.RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el enemigo colisiona, deja de atacar, O si despues del ataque pasa un tiempo, se reposiciona.
        _timer += Time.deltaTime;

        if (!enemy.Attacking || _timer >= enemy.PostAttackDelay)
        {
            enemy.Attacking = false;
            
            enemy.MovementStateMachine.ChangeState(enemy.RepositionState);
            enemy.ActionStateMachine.ChangeState(enemy.WatchingState);
            _timer = 0;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
