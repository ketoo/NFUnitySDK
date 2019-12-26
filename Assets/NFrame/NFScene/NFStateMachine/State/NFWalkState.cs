using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFWalkState : NFIState
{
    private float fStartTime = 0;
    private NFHeroMotor xHeroMotor;

    public NFWalkState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
    }

    public override void Enter(GameObject gameObject, int index)
    {
        fStartTime = Time.time;
        base.Enter(gameObject, index);
    }

    public override void Execute(GameObject gameObject)
    {
        base.Execute(gameObject);

        if (Time.time - 1 > fStartTime)
        {

            mAnimatStateController.PlayAnimaState(NFAnimaStateType.Run, -1);
        }

        xHeroMotor.speed = xHeroMotor.walkSpeed;
        if (Time.time - fStartTime > 0.35f)
        {
            xHeroMotor.speed = xHeroMotor.walkSpeed + (Time.time - fStartTime) * (xHeroMotor.runSpeed - xHeroMotor.walkSpeed);
        }

        if (xHeroMotor.speed > xHeroMotor.runSpeed)
        {
            xHeroMotor.speed = xHeroMotor.runSpeed;
        }
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);
    }
}