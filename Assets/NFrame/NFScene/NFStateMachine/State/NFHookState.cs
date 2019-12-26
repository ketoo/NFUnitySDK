using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;

public class NFHookState : NFIState
{
    public NFHookState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}

	NFHeroInput xInput;
	NFHeroMotor xHeroMotor;

	public override void Enter(GameObject gameObject, int index)
	{
		xInput = gameObject.GetComponent<NFHeroInput>();
		xHeroMotor = gameObject.GetComponent<NFHeroMotor>();

		base.Enter(gameObject, index);



	}

	public override void Execute(GameObject gameObject)
	{
		if (Vector3.Distance(xStateData.vTargetPos, gameObject.transform.position) < 0.1f)
		{
			//GetStateMachine().GetMachineMng().ChangeState (NFAnimaStateType.HookHold);
		}
	}

}
