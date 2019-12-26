using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;

public class NFDashForwardState : NFIState
{
    private NFHeroMotor xHeroMotor;

    public NFDashForwardState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        if (xStateData != null)
        {
            iTween.MoveTo(gameObject, xStateData.vTargetPos, xStateData.fSpeed);
        }
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }

}