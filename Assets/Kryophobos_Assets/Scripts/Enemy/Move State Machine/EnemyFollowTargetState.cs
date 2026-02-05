using System.Threading;
using UnityEngine;

public class EnemyFollowTargetState : EnemyState
{
    private float _timer;
    public EnemyFollowTargetState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("FollowTarget State");

        enemy.Animator.SetBool("IsTargetSpotted", true);
        enemy.Animator.SetBool("EndSearching", false);

        enemy.PlayerSpotted = true;
        enemy.GPS.isStopped = false;

        if (enemy.AmbienceManager != null)
        {
            enemy.AmbienceManager.PersecutionSFX(true);
            Enemy returnEnemy = enemy.AmbienceManager.enemiesFollowing.Find(enemyOnList => enemyOnList == enemy);
            if (returnEnemy == null) enemy.AmbienceManager.enemiesFollowing.Add(enemy);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el jugador no esta siendo visto, pasa un tiempo de busqueda.
        if (!enemy.PlayerSpotted && !enemy.AlwaysChase)
        {
            _timer += Time.deltaTime;

            if (_timer >= enemy.FollowTime)
            {
                _timer = 0;

                if (enemy.AmbienceManager != null) 
                {
                    enemy.AmbienceManager.enemiesFollowing.Remove(enemy);
                    enemy.AmbienceManager.PersecutionSFX(false);
                }

                //Forzar enemig patrulla
                enemy.StandStill = false;

                if (enemy.StandStill) enemy.MovementStateMachine.ChangeState(enemy.ReturnPositionState);    //Si esta configurado el enemigo para quedarse en una posicion, vuelve.
                else enemy.MovementStateMachine.ChangeState(enemy.PatrolState);                             //Si no lo esta, sigue patruyando.
            } 
        }
        else _timer = 0; //Si se ve al jugador, se reinicia el tiempo de busqueda.

        //Actualizar destino
        if (enemy.GPS.isOnNavMesh)
        {
            enemy.GPS.SetDestination(enemy.Player.transform.position);
            enemy.GPS.isStopped = false;
        }

        float distanceFromTarget = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

        //Cuando la distancia del enemigo con respecto al jugador sea menor de 'AttackDistance'..
        if (distanceFromTarget <= enemy.AttackDistance && enemy.PlayerSpotted)
        {
            //Se prepara para atacar y se queda quieto.
            enemy.PreparingAttack = true;
            enemy.MovementStateMachine.ChangeState(enemy.IdleState);
            enemy.ActionStateMachine.ChangeState(enemy.PreparingAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
