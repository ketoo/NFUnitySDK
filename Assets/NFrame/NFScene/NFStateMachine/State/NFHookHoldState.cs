using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;
using NFSDK;

public class NFHookHoldState : NFIState
{
	NFHeroInput xInput;
	NFHeroMotor xHeroMotor;

    public NFHookHoldState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}


	public override void Enter(GameObject gameObject, int index)
	{

	}
	
	public override void Execute(GameObject gameObject)
	{
		base.Execute(gameObject);

	}

}
