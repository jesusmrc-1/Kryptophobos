using System.Threading;
using UnityEngine;

public class EnemyWatchingState : EnemyState
{
    private float _timer;
    public EnemyWatchingState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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

        //Si el jugador se encuentra en el rango de visión del enemigo, comprobar si hay algun obstaculo que tape la vision con el jugador.
        if (enemy.IsPlayerInVisionRange && !enemy.IsEnemyFlashed)
        {
            //Calcular la dirección entre el enemigo y el jugador
            Vector3 lookingDirection = enemy.Player.transform.position - enemy.transform.position;

            //Offset en Y para que no detecte el suelo ya que la dirección va a los pies del jugador
            lookingDirection.y += enemy.YOffset;

            //Debugging Ray
            Color color = new Color(1, 0, 0, 1);
            Debug.DrawRay(enemy.transform.position, lookingDirection, color);

            //Si al lanzar el rayo se topa con un collider, entonces..
            if (Physics.Raycast(enemy.transform.position, lookingDirection, out RaycastHit hitInfo, Mathf.Infinity, enemy.IgnoredLayers))
            {
                //Debug Collider Name
                //Debug.Log(hitInfo.collider.gameObject.name);

                //Si el collider es el del jugador..
                if (hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    //Se detecta al jugador y mientras sea visible, timer sera siempre 0 hasta perder de vista al jugador.
                    enemy.PlayerSpotted = true;
                    
                    if (!enemy.FollowingTarget) enemy.MovementStateMachine.ChangeState(enemy.FollowTargetState);
                    enemy.FollowingTarget = true;
                }
            }
        }
        else enemy.PlayerSpotted = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
