using UnityEngine;

public class EnemySearchingState : EnemyState
{
    private float _timer;
    public EnemySearchingState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.index = 0;
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.StartSearching = false;
        _timer = 0;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el jugador no esta siendo visto, pasa un tiempo de busqueda.
        if (!enemy.PlayerSpotted)
        {
            _timer += Time.deltaTime;

            if (_timer >= enemy.SearchingTime)
            {
                _timer = 0;
                if (enemy.StandStill) enemy.MovementStateMachine.ChangeState(enemy.ReturnPositionState);    //Si esta configurado el enemigo para quedarse en una posicion, vuelve.
                else enemy.MovementStateMachine.ChangeState(enemy.PatrolState);                             //Si no lo esta, sigue patruyando.
            }
        }
        else
        {
            _timer = 0;
            enemy.MovementStateMachine.ChangeState(enemy.FollowTargetState);
        }

        if (!enemy.StartSearching) 
        { 
            enemy.GPS.SetDestination(enemy.TargetLastKnownPosition);

            if (Vector3.Distance(enemy.transform.position, enemy.TargetLastKnownPosition) <= 1f)
            {
                enemy.StartSearching = true;
                RandomizeWaypoint();
            }
        }

        if (enemy.StartSearching)
        {
            enemy.GPS.SetDestination(enemy.CurrentWaypoint.transform.position);

            if (Vector3.Distance(enemy.transform.position, enemy.CurrentWaypoint.transform.position) <= 1f)
            {
                RandomizeWaypoint();
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void RandomizeWaypoint()
    {
        //Cuando este tan cerca del waypoint, aleatorizar
        int randomizedWaypoint = Random.Range(0, enemy.RoomWaypoints.Count);
        while (randomizedWaypoint == enemy.index) randomizedWaypoint = Random.Range(0, enemy.RoomWaypoints.Count);
        enemy.index = randomizedWaypoint;
        enemy.CurrentWaypoint = enemy.RoomWaypoints[randomizedWaypoint];
    }
}
