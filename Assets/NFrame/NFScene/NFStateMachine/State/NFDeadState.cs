using UnityEngine;
using System.Collections;

using NFrame;
using NFSDK;

public class NFDeadState : NFIState
{
    private NFHeroMotor xHeroMotor;
    private float fStartTime = 0f;
    private bool bShowUI = false;

    NFUIModule mUIModule;

    public NFDeadState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();
    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        fStartTime = Time.time;
        bShowUI = false;

        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
        xHeroMotor.Stop();
    }

    public override void Execute(GameObject gameObject)
    {
        if (Time.time - 1f > fStartTime && bShowUI == false)
        {
            if (mStateMachine.IsMainRole())
            {
                bShowUI = true;

                //NFUIHeroDie winHeroDie = mUIModule.ShowUI<NFUIHeroDie>();
                //winHeroDie.ShowReliveUI();
            }
        }

        if (gameObject.transform.position.y < -10)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 22, gameObject.transform.position.z);
        }
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }
}
