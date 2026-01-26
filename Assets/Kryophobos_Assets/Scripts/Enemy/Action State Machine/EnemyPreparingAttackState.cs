using UnityEngine;

public class EnemyPreparingAttackState : EnemyState
{
    private float _timer;
    private Vector3 _lookingDirection;
    public EnemyPreparingAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("PreparingAttack State");

        enemy.Animator.SetBool("IsTargetClose", true);
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.Animator.SetBool("IsTargetClose", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Rotar para mirar al jugador
        _lookingDirection = enemy.Player.transform.position - enemy.transform.position;

        //Cuando pase el tiempo de carga de ataque, atacar.
        _timer += Time.deltaTime;

        if (_timer >= enemy.AttackDelay)
        {
            enemy.PreparingAttack = false;
            enemy.Attacking = true;
            enemy.ActionStateMachine.ChangeState(enemy.AttackingState);
            _timer = 0;
        }

        //Comprobar si el jugador sigue en la linea de vision

        if (enemy.IsPlayerInVisionRange)
        {
            Vector3 lookingDirection = enemy.Player.transform.position - enemy.transform.position;
            lookingDirection.y += enemy.YOffset;

            //Debugging Ray
            Color color = new Color(1, 0, 0, 1);
            Debug.DrawRay(enemy.transform.position, lookingDirection, color);

            if (Physics.Raycast(enemy.transform.position, lookingDirection, out RaycastHit hitInfo, Mathf.Infinity, enemy.IgnoredLayers))
            {

                //Si el collider es el del jugador..
                if (hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    //Se detecta al jugador y mientras sea visible, timer sera siempre 0 hasta perder de vista al jugador.
                    enemy.PlayerSpotted = true;
                }
                else if (!hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    _timer = 0;
                    enemy.PreparingAttack = false;
                    enemy.MovementStateMachine.ChangeState(enemy.FollowTargetState);
                    enemy.ActionStateMachine.ChangeState(enemy.WatchingState);
                }
            }
        }
        else enemy.PlayerSpotted = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Quaternion targetRotation = Quaternion.LookRotation(_lookingDirection.normalized);
        Quaternion smoothedRotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.RotationSpeed);
        enemy.RB.MoveRotation(smoothedRotation);
    }
}
