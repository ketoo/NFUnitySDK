using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFBlockState : NFIState
{
    public NFBlockState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}
	
}