using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        player.Animator.SetBool("playerMoving", true);
        //player.PlayerFootsteps.StartFootsteps();
    }

    public override void ExitState()
    {
        base.ExitState();

        player.Animator.SetBool("playerMoving", false);
        //player.PlayerFootsteps.StopFootsteps();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Si el jugador trata de coger un objeto, cambiamos al estado Idle
        if (player.IsPlayerInteracting) player.MovementStateMachine.ChangeState(player.IdleState);

        //Si el jugador deja de moverse, llama a la FSM de movimiento del jugador y cambia al estado Idle.
        if (player.Move.ReadValue<Vector2>() == Vector2.zero) player.MovementStateMachine.ChangeState(player.IdleState);

        float frontInput = player.Move.ReadValue<Vector2>().y;  //WS

        if (player.Run.ReadValue<float>() == 0)
        {
            MoveSpeed(frontInput, player.WalkAnimSpeed);
            player.Speed = 1.5f;
        }
        else if (player.Run.ReadValue<float>() == 1)
        {
            MoveSpeed(frontInput, player.RunAnimSpeed);
            player.Speed = 3f;
        }

        // TANK CONTROLS => OFF
        if (!player.TankControls)
        {
            //Leer input
            float sideInput = player.Move.ReadValue<Vector2>().x;   //AD

            Vector3 side = new Vector3();
            Vector3 front = new Vector3();

            //Dirección de la camara
            side = Camera.main.transform.right;
            front = Camera.main.transform.forward;

            //Si la camara esta inclinada en el eje Y, lo ponemos a cero para evitar que el movimiento del jugador no funcione correctamente.
            side.y = 0f;
            front.y = 0f;

            //Al normalizarlo el movimiento no se ralentiza si el vector es muy pequeño, matiene su velocidad.
            side.Normalize();
            front.Normalize();

            //Calcular dirección de movimiento.
            player.Direction = (side * sideInput + front * frontInput).normalized;
        }

        //TANK CONTROLS => ON
        else if (player.TankControls)
        {
            Vector3 front = new Vector3();

            //Dirección del jugador
            front = player.transform.forward;
            front.y = 0f;
            front.Normalize();
            player.Direction = (front * frontInput).normalized;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // TANK CONTROLS => OFF
        if (!player.TankControls)
        {
            //Mover el jugador en la dirección en la que se tiene que mover.
            Vector3 linearVelocity = player.Direction * player.Speed;
            linearVelocity.y = player.RB.linearVelocity.y;
            player.RB.linearVelocity = linearVelocity;

            //Rotar el jugador en la dirección en la que se tiene que mover.
            Quaternion targetRotation = Quaternion.LookRotation(player.Direction.normalized);
            Quaternion smoothedRotation = Quaternion.Slerp(player.RB.rotation, targetRotation, player.RotationSpeed);
            player.RB.MoveRotation(smoothedRotation);
        }

        //TANK CONTROLS => ON
        else if (player.TankControls)
        {
            //Mover el jugador en la dirección en la que se tiene que mover.
            Vector3 linearVelocity = player.Direction * player.Speed;
            linearVelocity.y = player.RB.linearVelocity.y;
            player.RB.linearVelocity = linearVelocity;

            //ROTAR
            float sideInput = player.Move.ReadValue<Vector2>().x;   //AD
            float rotation = sideInput * player.TurnSpeed * Time.deltaTime;
            Vector3 euler = player.RB.rotation.eulerAngles;
            euler.y += rotation;
            Quaternion targetRotation = Quaternion.Euler(euler);
            player.RB.MoveRotation(targetRotation);
        }
    }

    private void MoveSpeed(float frontInput , float speed)
    {
        //Backward
        if (frontInput < 0)
        {
            player.Animator.SetFloat("AnimSpeed", -speed);
        }
        //Forward
        else if (frontInput > 0)
        {
            player.Animator.SetFloat("AnimSpeed", speed);
        }
    }
}
