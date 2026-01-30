using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    public string CurrentMovementState;
    public string CurrentActionState;

    private EventsManager _gameEvents;
    public PlayerFootsteps PlayerFootsteps;

    public InputAction Move;
    public InputAction Run;
    public InputAction ChargeFlashlight;
    public InputAction Pickup;
    public InputAction Interact;
    public InputAction Exit;
    public Vector3 Direction;
    public float Speed;
    public float TurnSpeed = 425f;
    public float RotationSpeed;
    public float PickupRotationSpeed;
    public float PickupDelay;

    public float WalkAnimSpeed;
    public float RunAnimSpeed;

    public PlayerInventory inventory;
    [Tooltip("La referencia de la clase Flashlight se referenciara cuando termine la cinematica de coger la linterna, referencialo manualmente para testearlo y chequea HasFlashlight.")]
    public Flashlight FlashLight;
    public bool IsPlayerNearItem;
    public bool IsPlayerNearNote;
    public bool IsPlayerNearDoor;
    public bool IsPlayerNearPuzzle;
    public bool IsPlayerNearButton;
    public bool IsPlayerInteracting;
    public bool IsPlayerDoingPuzzle;
    public bool PlayerHasRequiredItems;

    public bool TankControls;

    public InteractableDistanceChecker IDC;

    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; set; }
    [Tooltip("El tiempo que tiene que pasar para poder volver a recibir daño despues de haber sido dañado")]
    public float IFramesDelay;
    public bool GodMode;
    public float HealingRate;

    public Rigidbody RB { get; set; }
    public Animator Animator { get; set; }

    public CapsuleCollider CapsuleCollider { get; set; }

    #region Finite State Machines
    public PlayerStateMachine MovementStateMachine { get; set; }
    public PlayerStateMachine ActionStateMachine { get; set; }
    #endregion

    #region FSM Movement vars.
    public PlayerIdleState IdleState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    #endregion

    #region FSM Actions vars.
    public PlayerNonActionState NonActionState { get; set; }
    public PlayerChargeFlashlightState ChargeFlashlightState { get; set; }
    public PlayerInteractState InteractState { get; set; }
    public PlayerDoingPuzzleState DoingPuzzleState { get; set; }
    #endregion

    #region Awake - Start - Update - FixedUpdate
    private void Awake()
    {
        this.PlayerFootsteps = GetComponent<PlayerFootsteps>();
        CapsuleCollider = GetComponent<CapsuleCollider>();

        //FSM
        MovementStateMachine = new PlayerStateMachine();
        ActionStateMachine = new PlayerStateMachine();

        //Al crear una nueva clase por ejemplo 'PlayerIdleState' y guardarla en 'IdleState', al tener un constructor se tiene que crear pasandole los paramteros
        //se le pasa esta clase 'this' y la clase StateMachine (PlayerStateMachine).
        //MOVEMENT
        IdleState = new PlayerIdleState(this, MovementStateMachine);
        MoveState = new PlayerMoveState(this, MovementStateMachine);

        //ACTION
        NonActionState = new PlayerNonActionState(this, ActionStateMachine);
        ChargeFlashlightState = new PlayerChargeFlashlightState(this, ActionStateMachine);
        InteractState = new PlayerInteractState(this, ActionStateMachine);
        DoingPuzzleState = new PlayerDoingPuzzleState(this, ActionStateMachine);
    }
    private void Start()
    {
        Move.Enable();
        Run.Enable();
        ChargeFlashlight.Enable();
        Pickup.Enable();
        Interact.Enable();
        Exit.Enable();

        CurrentHealth = MaxHealth;

        RB = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        IDC = GetComponent<InteractableDistanceChecker>();

        MovementStateMachine.Initialize(IdleState);

        ActionStateMachine.Initialize(NonActionState);

        _gameEvents = GameObject.FindGameObjectWithTag("GameEventsManager").GetComponent<EventsManager>();

        LoadState();
    }
    private void Update()
    {
        Debug.Log(ChargeFlashlight.ReadValue<float>());

        CurrentMovementState = MovementStateMachine.CurrentPlayerState.ToString();
        CurrentActionState = ActionStateMachine.CurrentPlayerState.ToString();

        //Llama a la funcion Update del estado actual
        MovementStateMachine.CurrentPlayerState.FrameUpdate();

        ActionStateMachine.CurrentPlayerState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        MovementStateMachine.CurrentPlayerState.PhysicsUpdate();

        ActionStateMachine.CurrentPlayerState.PhysicsUpdate();
    }
    #endregion

    #region Damage
    public void Damage(float damageAmount)
    {
        if (GodMode) return;
        Debug.Log("Damage method from player");

        float newHealth = CurrentHealth - damageAmount;

        if (newHealth <= 0f)
        {
            CurrentHealth = 0f;
            Die();
        }
        else if (!GodMode)
        {
            StopAllCoroutines();
            CurrentHealth = newHealth;
            GodMode = true;
            StartCoroutine(InvincibleFrames());
        }
    }

    //El personaje muere
    public void Die()
    {
        Debug.Log("El jugador ha muerto");
        _gameEvents.PlayerDead();
    }
    #endregion

    #region I Frames
    IEnumerator InvincibleFrames()
    {
        yield return new WaitForSeconds(IFramesDelay);

        GodMode = false;

        StartCoroutine(Healing());

        yield return null;
    }
    #endregion

    #region Healing

    IEnumerator Healing()
    {
        while(CurrentHealth < MaxHealth)
        {
            CurrentHealth += Time.deltaTime * HealingRate;
            yield return null;
        }
        CurrentHealth = MaxHealth;
    }

    #endregion

    public void EnableGodMode()
    {
        GodMode = true;
        RB.useGravity = false;
        CapsuleCollider.enabled = false;
    }

    public void DisableGodMode()
    {
        GodMode = false;
        CapsuleCollider.enabled = true;
        RB.useGravity = true;
    }

    public void DisableInputs()
    {
        Move.Disable();
        Run.Disable();
        ChargeFlashlight.Disable();
        Pickup.Disable();
        Interact.Disable();
        Exit.Disable();
    }

    public void EnableInputs()
    {
        Move.Enable();
        Run.Enable();
        ChargeFlashlight.Enable();
        Pickup.Enable();
        Interact.Enable();
        Exit.Enable();
    }

    //Se referencia el script de la linterna al entrar a la cinematica de coger la linterna.
    public void AddFlashlight(Flashlight flashlight)
    {
        FlashLight = flashlight;
        inventory.HasFlashlight = true;
    }

    #region Animation Triggers

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        MovementStateMachine.CurrentPlayerState.AnimationTriggerEvent(triggerType);
    }

    //Crear nombres para eventos de triggers del animator
    public enum AnimationTriggerType
    {
        PlayerDamaged,
        PlayFootstepsSound
    }
    #endregion

    private void LoadState()
    {
        if (GameManager.Instance.PlayerPosition != Vector3.zero) transform.position = GameManager.Instance.PlayerPosition;
    }
}