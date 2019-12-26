using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;

public class NFJumpingState : NFIState
{
    public NFJumpingState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
    }
    
	private NFHeroInput xInput;
	private NFHeroMotor xHeroMotor;
	private NFBodyIdent xBodyIdent;
	private NFAnimatStateController xHeroAnima;

    public override void Enter(GameObject gameObject, int index)
    {
		xBodyIdent = gameObject.GetComponent<NFBodyIdent>();
        xInput = gameObject.GetComponent<NFHeroInput>();
        xHeroAnima = gameObject.GetComponent<NFAnimatStateController>();
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();


        base.Enter(gameObject, index);
  
    }

    public override void Execute(GameObject gameObject)
    {
		base.Execute(gameObject);
  
        if (xHeroMotor.isOnGround)
		{
            mAnimatStateController.PlayAnimaState(NFAnimaStateType.JumpLand, -1);
		}
	}

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }

    public override void OnCheckInput(GameObject gameObject)
    {
        if (mStateMachine.IsMainRole())
        {

        }
    }
}