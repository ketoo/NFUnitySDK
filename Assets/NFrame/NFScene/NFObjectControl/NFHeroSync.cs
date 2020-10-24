using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;
using NFSDK;
using ECM.Components;
using ECM.Controllers;

public class NFHeroSync : MonoBehaviour 
{
	private NFHeroSyncBuffer mxSyncBuffer;
	private NFHeroMotor mxHeroMotor;

	private NFBodyIdent mxBodyIdent;
    private NFAnimaStateMachine mAnimaStateMachine;
    private NFAnimatStateController mAnimatStateController;

    private NFNetModule mxNetModule;
	private NFLoginModule mLoginModule;
    private NFHelpModule mHelpModule;
    private NFIKernelModule mKernelModule;

    private int syncMessageCount = 0;
    void Awake () 
	{

	}

    private void Start()
    {
        mxHeroMotor = GetComponent<NFHeroMotor>();
        mxSyncBuffer = GetComponent<NFHeroSyncBuffer>();
        mxBodyIdent = GetComponent<NFBodyIdent>();
        mAnimaStateMachine = GetComponent<NFAnimaStateMachine>();
        mAnimatStateController = GetComponent<NFAnimatStateController>();

        mxNetModule = NFRoot.Instance().GetPluginManager().FindModule<NFNetModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();
        mHelpModule = NFRoot.Instance().GetPluginManager().FindModule<NFHelpModule>();
        mKernelModule = NFRoot.Instance().GetPluginManager().FindModule<NFIKernelModule>();
    }

    bool CheckState()
    {
		return true;
	}

    private bool MeetGoalCallBack()
    {
        if (mxSyncBuffer.Size() > 0)
        {
            FixedUpdate();
            return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (mxBodyIdent && mxBodyIdent.GetObjectID() != mLoginModule.mRoleID)
		{
            NFHeroSyncBuffer.Keyframe keyframe;
            if (mxSyncBuffer.Size() > 1)
            {
                keyframe = mxSyncBuffer.LastKeyframe();
                mxHeroMotor.walkSpeed *= mxSyncBuffer.Size();
                mxHeroMotor.runSpeed *= mxSyncBuffer.Size();
            }
            else
            {
                keyframe = mxSyncBuffer.NextKeyframe();
                mxHeroMotor.walkSpeed = mKernelModule.QueryPropertyInt(mLoginModule.mRoleID, NFrame.Player.MOVE_SPEED) /  100;
                mxHeroMotor.runSpeed = mKernelModule.QueryPropertyInt(mLoginModule.mRoleID, NFrame.Player.MOVE_SPEED) / 100;
            }

            if (keyframe != null)
            {
                NFAnimaStateType type = (NFrame.NFAnimaStateType)keyframe.status;
                switch (type)
                {
                    case NFAnimaStateType.Run:
                    case NFAnimaStateType.Idle:
                        if (keyframe.Position != Vector3.zero)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, true, MeetGoalCallBack);
                        }
                        break;
                    case NFAnimaStateType.Stun:
                        mAnimatStateController.PlayAnimaState(NFAnimaStateType.Stun, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void AddSyncData(int sequence, NFMsg.PosSyncUnit syncUnit)
    {
        Clear();

        Vector3 pos = new Vector3();
        Vector3 dir = new Vector3();

        if (syncUnit.Pos != null)
        {
            pos.x = syncUnit.Pos.X;
            pos.y = syncUnit.Pos.Y;
            pos.z = syncUnit.Pos.Z;
        }

        if (syncUnit.Orientation != null)
        {
            dir.x = syncUnit.Orientation.X;
            dir.y = syncUnit.Orientation.Y;
            dir.z = syncUnit.Orientation.Z;
        }

        var keyframe = new NFHeroSyncBuffer.Keyframe();
        keyframe.Position = pos;
        keyframe.Director = dir;
        keyframe.status = syncUnit.Status;
        keyframe.InterpolationTime = sequence;

        if (mxSyncBuffer)
        {
            Debug.Log(keyframe.InterpolationTime + " move " + this.transform.position.ToString() + " TO " + keyframe.Position.ToString());

            mxSyncBuffer.AddKeyframe(keyframe);
        }
    }

    public void Clear()
    {
        if (mxSyncBuffer)
        {
            mxSyncBuffer.Clear();
        }
    }
}
