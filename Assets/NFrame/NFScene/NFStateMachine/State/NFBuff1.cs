using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFBuff1 : NFIState
{
    public NFBuff1(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

    }
	
}
