using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Custom.Utility;
using UnityEngine;

public class EnemyAttackState : State<EnemyFSM>
{
    public EnemyAttackState(EnemyFSM fsm) : base(fsm, "EnemyAttack")
    {
    }

    private bool _canAttack = true;

    public override void Enter()
    {
        _canAttack = true;
    }

    public override void Update()
    {
        if (!_canAttack)
            return;

        Vector3 toTarget = (fsm.Target.position - fsm.NavMeshAgent.transform.position).SetY(0);

        fsm.NavMeshAgent.transform.forward = toTarget;
        
        float distanceToTarget = toTarget.magnitude;

        if (fsm.GrenadeData.TryCalculateSpeed(distanceToTarget, fsm.NavMeshAgent.transform.position.y,
            fsm.NavMeshAgent.transform.forward, out var speed))
        {
            fsm.ProjectileAttackController.InitializeAttack(fsm.AttackerEnemyData.AttackerItemData);
            fsm.ProjectileAttackController.Throw(speed);

            fsm.ProjectileAttackController.StartCoroutine(AttackDelayCoroutine());
            
            return;
        }
        
        fsm.SetNextActionState("EnemyMove");
    }

    public override void Exit()
    {
        
    }

    private IEnumerator AttackDelayCoroutine()
    {
        float time = 0;

        _canAttack = false;
        
        while ((time += Time.deltaTime) < fsm.AttackerEnemyData.AttackDelay)
        {
            yield return null;
        }

        _canAttack = true;
    }
}
