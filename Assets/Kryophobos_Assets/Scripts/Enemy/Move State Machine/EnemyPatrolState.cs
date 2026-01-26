using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("Patrol State");

        //PUEDE DAR ERROR PORQUE SI SE SALE DEL NAVMESH NO DEJA REANUDAR, HAY QUE COMPROBAR SI ESTA Y SI NO LO ESTA HABRA QUE HACER UN WARP A UN PUNTO SEGURO DE NAVMESH
        if (enemy.GPS.isOnNavMesh) enemy.GPS.isStopped = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si se cumple la probabilidad de mantener la posicion, pasa a Idle.
        if (enemy.HoldingPosition)
        {
            enemy.MovementStateMachine.ChangeState(enemy.IdleState);
        }

        //Si no se cumple la probabilidad de mantener la posicion, sigue patrullando.
        if (enemy.GPS.isOnNavMesh && !enemy.HoldingPosition)
        {
            enemy.GPS.isStopped = false;

            //Actualizar destino
            enemy.GPS.SetDestination(enemy.CurrentWaypoint.transform.position);

            //Comprobar distancia entre el enemigo y el destino
            float distanceFromNextWaypoint = Vector3.Distance(enemy.transform.position, enemy.CurrentWaypoint.transform.position);

            //Si el enemigo esta a menos de 'enemy.MinDistanceToChangeWaypoint', cambiamos el destino.
            if (distanceFromNextWaypoint <= enemy.MinDistanceToChangeWaypoint) enemy.NextWaypointFunctions();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
