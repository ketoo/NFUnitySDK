using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFNormalSkill5 : NFIState
{
    private NFBodyIdent xBodyIdent;
    private NFHeroInput xInput;
    private NFHeroMotor xHeroMotor;
    private NFHeroSync xHeroSync;

    public NFNormalSkill5(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xBodyIdent = gameObject.GetComponent<NFBodyIdent>();
        xInput = gameObject.GetComponent<NFHeroInput>();
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
        xHeroSync = gameObject.GetComponent<NFHeroSync>();
    }
    // Use this for initialization
    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        //mAnimatStateController.PlayAnimaState(NFAnimaStateType.NormalSkill3);
    }

    public override void Execute(GameObject gameObject)
    {
        xHeroMotor.ProcessInput(false, false, false);
    }
}
