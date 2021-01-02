using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;
using ECM.Components;
using ECM.Controllers;

public class NFJumpLandState : NFIState
{
    public NFJumpLandState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
    }

    private CharacterMovement mCharacterMovement;
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

        mCharacterMovement = gameObject.GetComponent<CharacterMovement>();

        base.Enter(gameObject, index);

        Vector3 v = new Vector3(gameObject.transform.position.x, mCharacterMovement.groundHit.groundPoint.y, gameObject.transform.position.z);
        gameObject.transform.position = v;
    }

    public override void Execute(GameObject gameObject)
    {
		base.Execute(gameObject);

        mAnimatStateController.PlayAnimaState(NFAnimaStateType.Idle, -1);
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

        //判断是需要idle还是run
        if (mStateMachine.IsMainRole())
        {
       
        }
    }

    public override void OnCheckInput(GameObject gameObject)
    {
 
    }
}