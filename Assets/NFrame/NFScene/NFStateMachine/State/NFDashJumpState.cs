using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;

public class NFDashJumpState : NFIState
{
    public NFDashJumpState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

		NFHeroMotor xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);
    }
}