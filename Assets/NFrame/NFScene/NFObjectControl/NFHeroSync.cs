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

    private NFNetModule mxNetModule;
	private NFLoginModule mLoginModule;
    private NFHelpModule mHelpModule;

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

        mxNetModule = NFPluginManager.Instance().FindModule<NFNetModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mHelpModule = NFPluginManager.Instance().FindModule<NFHelpModule>();
    }

    bool CheckState()
    {
		return true;
	}

    private void Update()
    {
		if (mxBodyIdent && mxBodyIdent.GetObjectID() != mLoginModule.mRoleID)
		{
            NFHeroSyncBuffer.Keyframe keyframe = mxSyncBuffer.NextKeyframe();
            if (keyframe != null)
            {
                mxHeroMotor.MoveTo(keyframe.Position);

                NFAnimaStateType type = (NFrame.NFAnimaStateType)keyframe.status;
                switch (type)
                {
                    case NFAnimaStateType.Run:
                        break;
                    default:
                        break;
                }
            }
        }
    }

	public void SendSyncMessage()
    {

        syncMessageCount++;

        NFMsg.Vector3 position = new NFMsg.Vector3();
        position.X = mxHeroMotor.GetMovePos().x;
        position.Y = mxHeroMotor.GetMovePos().y;
        position.Z = mxHeroMotor.GetMovePos().z;

        NFMsg.Vector3 direction = new NFMsg.Vector3();
        direction.X = mxHeroMotor.GetMoveDrictor().x;
        direction.Y = mxHeroMotor.GetMoveDrictor().y;
        direction.Z = mxHeroMotor.GetMoveDrictor().z;

        NFMsg.ReqAckPlayerPosSync reqAckPlayerPosSync = new NFMsg.ReqAckPlayerPosSync();
        reqAckPlayerPosSync.Mover = mHelpModule.NFToPB(mLoginModule.mRoleID);
        reqAckPlayerPosSync.InterpolationTime = 0;
        reqAckPlayerPosSync.Time = Time.frameCount;
        reqAckPlayerPosSync.Status = (int)mAnimaStateMachine.CurState();
        reqAckPlayerPosSync.Frame = Time.frameCount;

        reqAckPlayerPosSync.Position = position;
        reqAckPlayerPosSync.Direction = direction;

        mxNetModule.RequireSyncPosition(reqAckPlayerPosSync);
    }

    public void AddSyncData(NFMsg.ReqAckPlayerPosSync reqAckPlayerPosSync)
    {
        Vector3 v = new Vector3();
        v.x = reqAckPlayerPosSync.Position.X;
        v.y = reqAckPlayerPosSync.Position.Y;
        v.z = reqAckPlayerPosSync.Position.Z;

        Vector3 v1 = new Vector3();
        v1.x = reqAckPlayerPosSync.Direction.X;
        v1.y = reqAckPlayerPosSync.Direction.Y;
        v1.z = reqAckPlayerPosSync.Direction.Z;

        var keyframe = new NFHeroSyncBuffer.Keyframe();
        keyframe.InterpolationTime = reqAckPlayerPosSync.InterpolationTime;
        keyframe.Position = v;
        keyframe.Director = v1;
        keyframe.status = reqAckPlayerPosSync.Status;

        mxSyncBuffer.AddKeyframe(keyframe);
    }
}
