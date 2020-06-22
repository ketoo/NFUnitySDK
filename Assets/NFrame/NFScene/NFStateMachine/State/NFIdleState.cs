using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;

public class NFIdleState : NFIState
{
    private NFBodyIdent xBodyIdent;
	private NFHeroMotor xHeroMotor;
    private NFHeroSync xHeroSync;

    private Vector3 vector3 = new Vector3();


    private NFIKernelModule mKernelModule;

    private NFLoginModule mLoginModule;
    private NFSceneModule mSceneModule;

    public NFIdleState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        NFIPluginManager pluginManager = NFRoot.Instance().GetPluginManager();

        mKernelModule = pluginManager.FindModule<NFIKernelModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();
        mSceneModule = NFRoot.Instance().GetPluginManager().FindModule<NFSceneModule>();
    }

    private bool Fall()
    {
        return false;
    }

    public override void Enter(GameObject gameObject, int index)
    {
        xBodyIdent = gameObject.GetComponent<NFBodyIdent>();
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();
        xHeroSync = gameObject.GetComponent<NFHeroSync>();

        base.Enter(gameObject, index);

		if (mStateMachine.IsMainRole())
		{
			//xHeroSync.SendSyncMessage();
		}
        //看是否还按住移动选项，如果按住，则继续walk

		if (!xHeroMotor.isOnGround)
        {
            mAnimatStateController.PlayAnimaState(NFAnimaStateType.Fall, -1);
		}
    }

    public override void Execute(GameObject gameObject)
    {
        base.Execute(gameObject);
        if (gameObject.transform.position.y < -10)
        {
            GameObject go = mSceneModule.GetObject(mLoginModule.mRoleID);
            if (go != null)
            {
                if (Vector3.Distance(go.transform.position, gameObject.transform.position) < 50)
                {
                    vector3.x = gameObject.transform.position.x;
                    vector3.y = 22;
                    vector3.z = gameObject.transform.position.z;
                    gameObject.transform.position = vector3;
                }
            }
        }
    }

	public override void OnCheckInput(GameObject gameObject)
    {
        if (mStateMachine.IsMainRole())
        {
            //hp
            if (mKernelModule.QueryPropertyInt(xBodyIdent.GetObjectID(), NFrame.NPC.HP) <= 0)
            {
                return;
            }

            if (NFRoot.Instance().GetGameMode() == GAME_MODE.GAME_MODE_2D)
            {
                bool left = false;
                bool right = false;
                bool top = false;

                if (xHeroMotor.GetMoveDrictor().z > 0)
                {
                    top = true;
                }

                if (xHeroMotor.GetMoveDrictor().x < 0)
                {
                    left = true;
                    xHeroMotor.ProcessInput(left, right, top);
                    if (top)
                    {
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.JumpStart, -1);
                    }
                    else
                    {
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.Run, -1);
                    }
                }
                else if (xHeroMotor.GetMoveDrictor().x > 0)
                {
                    right = true;
                    xHeroMotor.ProcessInput(left, right, top);
                    if (top)
                    {
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.JumpStart, -1);
                    }
                    else
                    {
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.Run, -1);
                    }
                }
                else
                {
                    if (top)
                    {
                        xHeroMotor.ProcessInput(left, right, top);
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.JumpStart, -1);
                    }
                }
            }
            else
            {
                if (xHeroMotor.GetMoveDrictor() != Vector3.zero)
                {
                
                }
            }
        }
	}
}