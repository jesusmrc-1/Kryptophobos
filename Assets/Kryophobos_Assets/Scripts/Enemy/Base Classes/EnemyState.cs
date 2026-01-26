using UnityEngine;

public class EnemyState
{
    //Protected permite que las variables las vean los scripts que heredan de este.
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;

    //De esta forma, TODOS los estados hijos tienen esas variables accesibles sin repetirlas en cada clase.
    public EnemyState(Enemy enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    //Update
    public virtual void FrameUpdate() { }

    //FixedUpdate
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }
}
