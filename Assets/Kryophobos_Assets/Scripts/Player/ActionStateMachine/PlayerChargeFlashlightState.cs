using UnityEngine;

public class PlayerChargeFlashlightState : PlayerState
{
    public PlayerChargeFlashlightState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        player.Animator.SetBool("isCharging", true);
    }

    public override void ExitState()
    {
        base.ExitState();

        player.Animator.SetBool("isCharging", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el jugador trata de coger un objeto, cambiamos al estado PlayerPickupState
        if (player.Pickup.triggered && player.IsPlayerNearItem) player.ActionStateMachine.ChangeState(player.InteractState);

        if (player.ChargeFlashlight.ReadValue<float>() != 0f)
        {
            //Pasamos la información del input a la linterna
            player.FlashLight.ManualCharge = player.ChargeFlashlight.ReadValue<float>();
        }
        else 
        {
            player.FlashLight.ManualCharge = player.ChargeFlashlight.ReadValue<float>();
            player.ActionStateMachine.ChangeState(player.NonActionState);
        }

        if (player.inventory.Flashlight.HasLanternFullyCharged())
        {
            player.MovementStateMachine.ChangeState(player.IdleState);
            player.ActionStateMachine.ChangeState(player.NonActionState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
