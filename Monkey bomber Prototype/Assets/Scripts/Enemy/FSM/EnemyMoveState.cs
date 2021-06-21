using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyMoveState : State<EnemyFSM>
{
    public EnemyMoveState(EnemyFSM fsm) : base(fsm, "EnemyMove")
    {
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        fsm.NavMeshAgent.SetDestination(fsm.Target.position);

        if ((fsm.NavMeshAgent.transform.position - fsm.Target.position).sqrMagnitude <= 
            fsm.GrenadeData.MaxDistance * fsm.GrenadeData.MaxDistance)
        {
            fsm.SetNextActionState("EnemyAttack");
        }
    }

    public override void Exit()
    {
        fsm.NavMeshAgent.ResetPath();
    }
}
