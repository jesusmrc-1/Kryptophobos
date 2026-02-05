using UnityEngine;

public class EnemyFlashedState : EnemyState
{
    private float _timer;
    public EnemyFlashedState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);

    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.Animator.SetTrigger("FlashStun");
        enemy.GPS.isStopped = true;

        enemy.FlashedScreamSFX.Play();

        if (enemy.AmbienceManager != null)
        {
            enemy.AmbienceManager.enemiesSearching.Remove(enemy);
            enemy.AmbienceManager.TensionSFX(false);
            
            enemy.AmbienceManager.enemiesFollowing.Remove(enemy);
            enemy.AmbienceManager.PersecutionSFX(false);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.ActionStateMachine.ChangeState(enemy.WatchingState);
        enemy.Animator.SetBool("IsTargetSpotted", false);
        enemy.Animator.SetBool("EndSearching", false);
        enemy.Animator.SetFloat("AnimSpeed", 2f);
        enemy.GPS.isStopped = false;
        enemy.PreparingAttack = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Despues de pasar el tiempo de reposicion, volver a donde estaba el jugador.
        _timer += Time.deltaTime;

        if (_timer >= 3.8f)
        {
            _timer = 0;
            enemy.MovementStateMachine.ChangeState(enemy.RepositionState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
