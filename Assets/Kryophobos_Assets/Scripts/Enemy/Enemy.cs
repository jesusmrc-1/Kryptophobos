using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

// NO USAR EL CANDADO DEL INSPECTOR PARA VER ENEMY Y LUEGO DARLE PLAY.

/*
 * Cuando bloqueas el inspector de un objeto con el candado, Unity sigue intentando dibujar ese objeto aunque cambie su estado en Play Mode.

Con tu Enemy, que tiene propiedades complejas y referencias a sí mismo (StateMachines, estados, etc.), el inspector no puede resolver todas las referencias mientras el juego corre.

Resultado → MissingReferenceException, aunque el objeto en sí no esté destruido.

✅ Lecciones importantes:

El candado del inspector es solo para ver un objeto mientras navegas por otros.

Si bloqueas el inspector y entras en Play Mode con referencias complejas, Unity puede pensar que algunos objetos desaparecieron.

Quitando el candado, el inspector vuelve a seguir al objeto activo y ya no intenta dibujar referencias inválidas.

💡 Tip profesional:
Para depuración de IA compleja, es mejor usar paneles UI o Debug.Log en lugar de dejar el inspector bloqueado durante Play Mode, especialmente cuando tienes StateMachines con referencias circulares.
*/


//Enemigo idle (EnemyIdleState)
//Enemigo patrullando waypoints random / Ruta fija (EnemyPatrolState)
//Enemigo ve al jugador y lo sigue (EnemyFollowTargetState)
//Enemigo esta al alcance del jugador, se prepara para atacar (EnemyPreparingAttackState)
//Enemigo se recoloca (se vuelve invisible) 'reset ataque' y recuerda la posicion donde estaba el jugador al hacer el ataque (EnemyRepositionState)
//Si vuelve a la posicion del jugador (EnemyFollowTargetState, pero a la ultima posicion conocida) y si no esta (Busqueda?). POR ahora vuelve a patrullar.

//Si en cualquier estado se le flashea, isFlashed = true, se va a una zona X y se espera X segundos y luego vuelve a la pos del jugador.







public class Enemy : MonoBehaviour
{
    private EventsManager eventsManager;

    public string CurrentMovementState;
    public string CurrentActionState;

    public AmbienceManager AmbienceManager;

    public GameObject Player;

    public GameObject[] Waypoints;

    public GameObject RepositionCoords;

    public bool RandomizePatrol;

    public NavMeshAgent GPS;
    public GameObject CurrentWaypoint;

    public int index;

    public float MinDistanceToChangeWaypoint;

    public float AttackDistance;
    public float AttackDelay;
    public float PostAttackDelay;
    public float AttackImpulse;
    public float RepositionTime;
    public Vector3 TargetLastKnownPosition;
    public float FollowTime;
    public float SearchingTime;

    public float RepositionSpeed;
    public float NormalSpeed;

    public bool IsEnemyFlashed;

    public List<GameObject> RoomWaypoints = new List<GameObject>();

    public bool Attacking;
    public bool HoldingPosition;
    public int ChanceToHoldPosition;
    public float MinTimeToHoldPosition;
    public float MaxTimeToHoldPosition;

    public bool IsPlayerInVisionRange;
    public bool PlayerSpotted;
    public bool PreparingAttack;
    public bool FollowingTarget;
    public bool StartSearching;

    public bool IsEnemyOnGround;

    public bool AlwaysChase;

    //Variables configuración quieto vigilando.
    public Vector3 StartingPosition;
    public Vector3 LookingDirection;
    public bool StandStill;
    public bool SpawnJump;

    public float RotationSpeed;

    public int IgnoredLayers;

    public LayerMask IgnoreEnemy;
    public LayerMask IgnoreEnemyVision;
    public LayerMask IgnoreRaycast;
    public LayerMask IgnoreInteractable;

    public float YOffset;

    public Collider AttackingTrigger;

    public Rigidbody RB;
    public Animator Animator;

    public AudioSource AudioSource;

    public AudioSource PrepareAttackSFX;
    public AudioSource AttackingSFX;
    public AudioSource FlashedScreamSFX;

    #region Finite State Machines
    public EnemyStateMachine MovementStateMachine { get; set; }
    public EnemyStateMachine ActionStateMachine { get; set; }
    #endregion

    #region FSM Movement vars.
    public EnemySpawnState SpawnState { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyStandStillState StandStillState { get; set; }
    public EnemyReturnPositionState ReturnPositionState { get; set; }
    public EnemyPatrolState PatrolState { get; set; }
    public EnemyFollowTargetState FollowTargetState { get; set; }
    public EnemySearchingState SearchingState { get; set; }
    public EnemyRepositionState RepositionState { get; set; }
    public EnemyFlashedState FlashedState { get; set; }
    #endregion

    #region FSM Actions vars.
    public EnemyNonActionState NonActionState { get; set; }
    public EnemyWatchingState WatchingState { get; set; }
    public EnemyPreparingAttackState PreparingAttackState { get; set; }
    public EnemyAttackingState AttackingState { get; set; }
    #endregion

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GPS = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody>();
        AudioSource = GetComponent<AudioSource>();
        this.Animator = GetComponent<Animator>();

        //FSM
        MovementStateMachine = new EnemyStateMachine();
        ActionStateMachine = new EnemyStateMachine();

        //Al crear una nueva clase por ejemplo 'EnemyIdleState' y guardarla en 'IdleState', al tener un constructor se tiene que crear pasandole los paramteros
        //se le pasa esta clase 'this' y la clase StateMachine (EnemyStateMachine).
        //MOVEMENT
        SpawnState = new EnemySpawnState(this, MovementStateMachine);
        IdleState = new EnemyIdleState(this, MovementStateMachine);
        StandStillState = new EnemyStandStillState(this, MovementStateMachine);
        ReturnPositionState = new EnemyReturnPositionState(this, MovementStateMachine);

        PatrolState = new EnemyPatrolState(this, MovementStateMachine);
        FollowTargetState = new EnemyFollowTargetState(this, MovementStateMachine);
        SearchingState = new EnemySearchingState(this, MovementStateMachine);
        RepositionState = new EnemyRepositionState(this, MovementStateMachine);

        FlashedState = new EnemyFlashedState(this, MovementStateMachine);

        //ACTION
        NonActionState = new EnemyNonActionState(this, ActionStateMachine);
        WatchingState = new EnemyWatchingState(this, ActionStateMachine);
        PreparingAttackState = new EnemyPreparingAttackState(this, ActionStateMachine);
        AttackingState = new EnemyAttackingState(this, ActionStateMachine);

        AttackingTrigger.enabled = false;
    }

    private void Start()
    {
        GameObject AmbienceManagerGO = GameObject.FindGameObjectWithTag("AmbienceManager");
        if (AmbienceManagerGO != null)
        {
            AmbienceManager = AmbienceManagerGO.GetComponent<AmbienceManager>();
        }

        LayerMask ignoredLayers = IgnoreEnemy | IgnoreEnemyVision | IgnoreRaycast | IgnoreInteractable;

        IgnoredLayers = ~ignoredLayers;

        //Encuentra los waypoitns, toma los nombres y los pasa a valor INT y los guarda en orden en el array.
        //Waypoints = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => int.Parse(go.name)).ToArray();

        /*  1️⃣ FindGameObjectsWithTag("Waypoint")
        *   ➡ Devuelve un array desordenado de GameObjects.
        *
        *   2️⃣ .OrderBy(...)
        *   ➡ Toma ese array
        *   ➡ No lo ordena inmediatamente, sino que crea una lista ordenable(IEnumerable)
        *   ➡ Decide el orden según la función que pasas
        *   ➡ "go => go.GetComponent<Waypoint>().id" = ordenar por ID numérico
        *
        *   💡 Todavía NO se convierte a array, aquí solo se define la lógica de orden.
        *
        *   3️⃣ .ToArray()
        *   ➡ Aquí es donde realmente crea el array final ordenado
        *   ➡ Y se asigna a Waypoints
        */

        //Se guarda la posicion y la direccion en la que mira el enemigo al empezar.
        StartingPosition = transform.position;
        LookingDirection = transform.forward;

        Waypoints = GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => go.GetComponent<Waypoint>().ID).ToArray();

        CurrentWaypoint = Waypoints[0];

        if (!SpawnJump)
        {
            //Si no esta configurado para que se quede quieto vigilando, entoces inicia patrullando
            if (!StandStill) MovementStateMachine.Initialize(PatrolState);
            else MovementStateMachine.Initialize(StandStillState);
        }

        if (AlwaysChase)
        {
            MovementStateMachine.Initialize(FollowTargetState);
        }

        ActionStateMachine.Initialize(WatchingState);

        RepositionCoords = GameObject.FindGameObjectWithTag("Reposition");
    }

    private void Update()
    {
        CurrentMovementState = MovementStateMachine.CurrentEnemyState.ToString();
        CurrentActionState = ActionStateMachine.CurrentEnemyState.ToString();


        //Llama a la funcion Update del estado actual
        if (MovementStateMachine.CurrentEnemyState != null) MovementStateMachine.CurrentEnemyState.FrameUpdate();
        if (ActionStateMachine.CurrentEnemyState != null) ActionStateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        //Llama a la funcion FixedUpdate del estado actual
        if (MovementStateMachine.CurrentEnemyState != null) MovementStateMachine.CurrentEnemyState.PhysicsUpdate();
        if (ActionStateMachine.CurrentEnemyState != null) ActionStateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) IsPlayerInVisionRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) IsPlayerInVisionRange = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Collisionable"))
        if (Attacking)
        {
            Attacking = false;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) IsEnemyOnGround = true;
    }

    #region Path functions

    public void NextWaypointFunctions()
    {
        //Busca el GameObject 'CurrentWaypoint' dentro del array 'Waypoints' y devuelve el indice a 'index'.
        index = System.Array.IndexOf(Waypoints, CurrentWaypoint);

        //Randomizar si mantiene la posición.
        ChanceToHoldPosition = Random.Range(0, 100);
        Debug.Log(ChanceToHoldPosition);

        //Si se cumple la probabilidad, entonces el enemigo se quedara quieto.
        if (ChanceToHoldPosition >= 70) HoldingPosition = true;

        if (RandomizePatrol) RandomizeNextWaypoint();   //Si esta randomizado, seleccionar uno random de la lista
        else if (!RandomizePatrol) ChangeNextWaypoint();  //Si no esta randomizado, tomamos el numero del waypoint actual y buscamos el siguiente,si no hay volver al waypoint 0
    }

    void RandomizeNextWaypoint()
    {
        int randomizedWaypoint = Random.Range(0, Waypoints.Length);
        while (randomizedWaypoint == index) randomizedWaypoint = Random.Range(0, Waypoints.Length);
        CurrentWaypoint = Waypoints[randomizedWaypoint];
    }



    void ChangeNextWaypoint()
    {
        //Incrementamos waypoint para ver si hay otro waypoint en el array Waypoints
        index++;

        if (index >= Waypoints.Length)
        {
            //CurrentWaypoint toma el GameObject del indice 0 de Waypoints
            CurrentWaypoint = Waypoints[0];
        }
        else if (Waypoints[index] != null)
        {
            //CurrentWaypoint toma el GameObject del siguiente indice de Waypoints
            CurrentWaypoint = Waypoints[index];
        }
    }
    #endregion

    #region Holding Position Behaviour
    public IEnumerator HoldingPositionTime()
    {
        float randomTime = Random.Range(MinTimeToHoldPosition, MaxTimeToHoldPosition);
        yield return new WaitForSeconds(randomTime);
        HoldingPosition = false;
        ChanceToHoldPosition = 100;
    }

    #endregion

    #region Blinded Behaviour
    public IEnumerator Blind(float blindTime)
    {
        Debug.Log("ENEMY GOT BLINDED");
        //Mantenemos el tiempo de reposición original en 'repositionTime'.
        float repositionTime = RepositionTime;

        //Aumentamos el tiempo de reposición al ser cegado el enemigo.
        RepositionTime = blindTime;


        PreparingAttack = false;
        PlayerSpotted = false;
        MovementStateMachine.ChangeState(FlashedState);
        ActionStateMachine.ChangeState(NonActionState);

        float _timer = 0f;

            /*
        while (MovementStateMachine.CurrentEnemyState == FlashedState || MovementStateMachine.CurrentEnemyState == RepositionState)
        {
            yield return null;
        }
            */

        while (_timer < RepositionTime)
            {

            _timer += Time.deltaTime;
            yield return null;
            }

        //yield return new WaitForSeconds(blindTime);

        ActionStateMachine.ChangeState(WatchingState);

        //Volvemos a tomar el valor original.
        RepositionTime = repositionTime;
        IsEnemyFlashed = false;
    }
    #endregion

    #region Animation Triggers

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        //MovementStateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    //Crear nombres para eventos de triggers del animator
    public enum AnimationTriggerType
    {
        EnemyFlashed
    }
    #endregion
}
