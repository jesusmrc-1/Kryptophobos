using UnityEngine;

public class DetectNearbyTargets : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            //Mientras no este reposicionandose, preparandose para atacar o atancado, entonces cuando detecte por cercania al objetivo
            //Se preparara para atacar
            if (_enemy.ActionStateMachine.CurrentEnemyState != _enemy.PreparingAttackState &&
                _enemy.ActionStateMachine.CurrentEnemyState != _enemy.AttackingState &&
                _enemy.MovementStateMachine.CurrentEnemyState != _enemy.RepositionState &&
                !_enemy.IsEnemyFlashed)
            {
                _enemy.PreparingAttack = true;
                _enemy.IsPlayerInVisionRange = true;
                _enemy.MovementStateMachine.ChangeState(_enemy.IdleState);
                _enemy.ActionStateMachine.ChangeState(_enemy.PreparingAttackState);
            }
        }
    }
}
