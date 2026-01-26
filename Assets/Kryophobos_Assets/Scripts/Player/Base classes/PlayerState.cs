using UnityEngine;

public class PlayerState
{
    //Protected permite que las variables las vean los scripts que heredan de este.
    protected Player player;
    protected PlayerStateMachine playerStateMachine;

    //De esta forma, TODOS los estados hijos tienen esas variables accesibles sin repetirlas en cada clase.
    public PlayerState (Player player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    //Update
    public virtual void FrameUpdate () { }

    //FixedUpdate
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTriggerEvent(Player.AnimationTriggerType triggerType) { }
}
