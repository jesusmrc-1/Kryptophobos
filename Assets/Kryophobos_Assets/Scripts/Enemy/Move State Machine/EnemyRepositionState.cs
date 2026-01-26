using UnityEngine;

public class EnemyRepositionState : EnemyState
{
    private GameObject _farestWaypoint;
    private float _timer;
    public EnemyRepositionState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.LogWarning("Repositioning State");

        //enemy.GPS.isStopped = false;
        enemy.GPS.speed = 4;

        //MEJOR HACER UN PUNTO AL QUE PUEDAN IR LOS ENEMIGOS PERO NO EL JUGADOR, DE ESA FORMA NO TE LOS ENCUENTRAS EN EL MAPA QUIETOS ESPERANDO,
        //ASI SE ESPERAN FUERA DEL ALCANCE TOTAL DEL JUGADOR.

        /*FUNCIONA, pero vamos a probar un punto especifico, aunque se puede hacer una lista de waypoints inalcanzables para el jugar.
        //Elegir como destino el waypoint mas lejano, y cuando pase un tiempo X entonces que vuelva a la posición donde estaba el jugador.
        float distanceFromWaypoint;
        float farestCheckedDistance = Mathf.NegativeInfinity;

        foreach (GameObject waypoint in enemy.Waypoints)
        {
            distanceFromWaypoint = Vector3.Distance(enemy.transform.position, waypoint.transform.position);

            if (distanceFromWaypoint > farestCheckedDistance)
            {
                //Se referencia la distancia mas lejana
                farestCheckedDistance = distanceFromWaypoint;

                Debug.Log("Distance from waypoint is higher than farest checked distance");

                //Se referencia el waypoint mas lejanos
                _farestWaypoint = waypoint;
                Debug.Log(_farestWaypoint.gameObject.name);
            }
        }
        */
    }

    public override void ExitState()
    {
        base.ExitState();

        enemy.GPS.speed = 2;
        enemy.Animator.SetFloat("AnimSpeed", 1.5f);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Se reposiciona alejandose lejos
        //if (enemy.GPS.isOnNavMesh) enemy.GPS.SetDestination(_farestWaypoint.transform.position);
        if (enemy.GPS.isOnNavMesh) 
        {
            Debug.Log("Repositioning to RepositionCoords");
            enemy.GPS.SetDestination(enemy.RepositionCoords.transform.position);
        }

        //Despues de pasar el tiempo de reposicion, volver a donde estaba el jugador.
        _timer += Time.deltaTime;

        if (_timer >= enemy.RepositionTime)
        {
            _timer = 0;
            if (!enemy.AlwaysChase) enemy.MovementStateMachine.ChangeState(enemy.SearchingState);
            else if (enemy.AlwaysChase) enemy.MovementStateMachine.ChangeState(enemy.FollowTargetState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
