using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFBeHitState : NFIState
{
    public NFBeHitState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
	}

	public override void Enter(GameObject gameObject, int index)
	{
		base.Enter(gameObject, index);

	}

	public override void Execute(GameObject gameObject)
	{
	}

}
