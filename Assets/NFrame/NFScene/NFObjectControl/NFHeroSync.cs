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
    }

    bool CheckState()
    {
		return true;
	}

    private bool MeetGoalCallBack()
    {
        if (mxSyncBuffer.Size() > 0)
        {
            Update();
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (mxBodyIdent && mxBodyIdent.GetObjectID() != mLoginModule.mRoleID)
		{
            NFHeroSyncBuffer.Keyframe keyframe = mxSyncBuffer.NextKeyframe();
            if (keyframe != null)
            {
                NFAnimaStateType type = (NFrame.NFAnimaStateType)keyframe.status;
                switch (type)
                {
                    case NFAnimaStateType.Run:
                    case NFAnimaStateType.Idle:
                        if (keyframe.Position != Vector3.zero)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, MeetGoalCallBack);
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

        NFMsg.ReqAckPlayerPosSync playerPosSync = new NFMsg.ReqAckPlayerPosSync();

        NFMsg.PosSyncUnit posSyncUnit = new NFMsg.PosSyncUnit();
        posSyncUnit.Mover = mHelpModule.NFToPB(mLoginModule.mRoleID);
        posSyncUnit.Pos = position;
        posSyncUnit.Direction = direction;
        posSyncUnit.Status = (int)mAnimaStateMachine.CurState();
        posSyncUnit.Mover = mHelpModule.NFToPB(mLoginModule.mRoleID);
        playerPosSync.SyncUnit.Add(posSyncUnit);


        mxNetModule.RequireSyncPosition(playerPosSync);
    }


    public void AddSyncData(NFMsg.PosSyncUnit syncUnit)
    {
        Clear();

        Vector3 now = new Vector3();
        Vector3 pos = new Vector3();
        Vector3 dir = new Vector3();

        if (syncUnit.Direction != null)
        {
            now.x = syncUnit.Now.X;
            now.y = syncUnit.Now.Y;
            now.z = syncUnit.Now.Z;
        }

        if (syncUnit.Pos != null)
        {
            pos.x = syncUnit.Pos.X;
            pos.y = syncUnit.Pos.Y;
            pos.z = syncUnit.Pos.Z;
        }

        if (syncUnit.Direction != null)
        {
            dir.x = syncUnit.Direction.X;
            dir.y = syncUnit.Direction.Y;
            dir.z = syncUnit.Direction.Z;
        }

        var keyframe = new NFHeroSyncBuffer.Keyframe();
        keyframe.LastPos = now;
        keyframe.Position = pos;
        keyframe.Director = dir;
        keyframe.status = syncUnit.Status;
        keyframe.InterpolationTime = syncUnit.Time;

        if (mxSyncBuffer)
        {
            Debug.Log(keyframe.InterpolationTime + " move " + this.transform.position.ToString() + " TO " + keyframe.Position.ToString());

            mxSyncBuffer.AddKeyframe(keyframe);
        }
    }

    public void Clear()
    {
        mxSyncBuffer.Clear();
    }
}
