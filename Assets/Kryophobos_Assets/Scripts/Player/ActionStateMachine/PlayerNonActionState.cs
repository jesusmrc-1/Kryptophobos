using UnityEngine;

public class PlayerNonActionState : PlayerState
{
    public PlayerNonActionState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Entered -> PlayerNonActionState");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //COMPROBAR ACCIONES

        //Si el jugador interactua con un puzle, cambiar al estado PlayerDoingPuzzleState
        //if (player.Interact.triggered && player.IsPlayerNearPuzzle && player.PlayerHasRequiredItems) player.ActionStateMachine.ChangeState(player.InteractState);

        //Si la linterna no esta cargada del todo, entonces no esta stuneado el jugador
        if (!player.inventory.Flashlight.HasLanternFullyCharged())
        {
            //Si el jugador trata de coger un objeto, cambiamos al estado PlayerPickupState
            if (player.Interact.triggered && (player.IsPlayerNearItem || player.IsPlayerNearNote || player.IsPlayerNearButton)) player.ActionStateMachine.ChangeState(player.InteractState);

            //No dejar interactuar si no se tienen los objetos necesarios
            if (player.Interact.triggered && (player.IsPlayerNearPuzzle || player.IsPlayerNearDoor) && player.PlayerHasRequiredItems) player.ActionStateMachine.ChangeState(player.InteractState);

            //Si el jugador carga la linterna, llama a la FSM de acciones del jugador y cambia al estado Charge Flashlight.
            if (player.inventory.HasFlashlight && player.ChargeFlashlight.ReadValue<float>() != 0f) player.ActionStateMachine.ChangeState(player.ChargeFlashlightState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
