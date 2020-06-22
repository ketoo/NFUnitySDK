using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using UnityEngine;
using NFSDK;
using NFMsg;

namespace NFrame
{
	public partial class NFNetModule : NFIModule
    {
		private NFIKernelModule mKernelModule;
		private NFHelpModule mHelpModule;
		private NFLogModule mLogModule;
		private NFLoginModule mLoginModule;

		private NFNetListener mNetListener;
		private NFNetClient mNetClient;

		private string strFirstIP;
		public string strGameServerIP;

        //sender
        private NFMsg.MsgBase mxData = new NFMsg.MsgBase();
		private MemoryStream mxBody = new MemoryStream();
		private MsgHead mxHead = new MsgHead();
        private byte[] sendBytes = new byte[ConstDefine.NF_PACKET_BUFF_SIZE];

        public NFNetModule(NFIPluginManager pluginManager)
        {
            mNetListener = new NFNetListener();
            mPluginManager = pluginManager;
        }
        
        public override void Awake()
        {
        }

		public override void Init()
		{
		}

        public override void Execute()
        {
			if (null != mNetClient)
			{
				mNetClient.Execute();
			}
        }

        public override void BeforeShut()
        {
			if (null != mNetClient)
            {
                mNetClient.Disconnect();
            }
        }

        public override void Shut()
        {
			mNetClient = null;
		}

		public override void AfterInit()
		{
			mHelpModule = mPluginManager.FindModule<NFHelpModule>();
			mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
			mLogModule = mPluginManager.FindModule<NFLogModule>();
			mLoginModule = mPluginManager.FindModule<NFLoginModule>();

		}

		public String FirstIP()
		{
			return strFirstIP;
		}

        public void StartConnect(string strIP, int nPort)
        {
            Debug.Log(Time.realtimeSinceStartup.ToString() + " StartConnect " + strIP + " " + nPort.ToString());

			mNetClient = new NFNetClient(mNetListener);

            mNetClient.Connect(strIP, nPort);

            if (strFirstIP == null)
            {
                strFirstIP = strIP;
            }
            else if(strGameServerIP == null)
            {
                strGameServerIP = strIP;
            }
        }

        public NFNetState GetState()
        {
            return mNetClient.GetState();
        }

        public void ConnectServerByDns(string dns, int port)
		{
		}

		public void AddReceiveCallBack(int eMsg, NFSDK.NFNetListener.MsgDelegation netHandler)
        {
			mNetListener.RegisteredDelegation(eMsg, netHandler);
        }
  
		public void AddNetEventCallBack(NFSDK.NFNetListener.EventDelegation netHandler)
        {
			mNetListener.RegisteredNetEventHandler(netHandler);
        }

        public void SendMsg(int unMsgID, MemoryStream stream)
        {
            //Debug.Log("send message:" + unMsgID);

            if (mNetClient != null)
            {
                //NFMsg.MsgBase
                mxData.PlayerId = mHelpModule.NFToPB(mLoginModule.mRoleID);
                mxData.MsgData = ByteString.CopyFrom(stream.ToArray());

                mxBody.SetLength(0);
                mxData.WriteTo(mxBody);

                mxHead.unMsgID = (UInt16)unMsgID;
                mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

                byte[] bodyByte = mxBody.ToArray();
                byte[] headByte = mxHead.EnCode();

                Array.Clear(sendBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
                headByte.CopyTo(sendBytes, 0);
                bodyByte.CopyTo(sendBytes, headByte.Length);

                mNetClient.SendBytes(sendBytes, bodyByte.Length + headByte.Length);
            }

            /////////////////////////////////////////////////////////////////
        }
      
        ////////////////////////////////////修改自身属性
        public void RequirePropertyInt(NFGUID objectID, string strPropertyName, int newVar)
        {
            NFMsg.ObjectPropertyInt xData = new NFMsg.ObjectPropertyInt();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyInt xPropertyInt = new NFMsg.PropertyInt();
            xPropertyInt.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyInt.Data = newVar;
            xData.PropertyList.Add(xPropertyInt);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload int");
            SendMsg((int)NFMsg.EGameMsgID.AckPropertyInt, mxBody);
        }

        public void RequirePropertyFloat(NFGUID objectID, string strPropertyName, float newVar)
        {
            NFMsg.ObjectPropertyFloat xData = new NFMsg.ObjectPropertyFloat();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyFloat xPropertyFloat = new NFMsg.PropertyFloat();
            xPropertyFloat.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyFloat.Data = newVar;
            xData.PropertyList.Add(xPropertyFloat);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload Float");
            SendMsg((int)NFMsg.EGameMsgID.AckPropertyFloat, mxBody);
        }

        public void RequirePropertyString(NFGUID objectID, string strPropertyName, string newVar)
        {
            NFMsg.ObjectPropertyString xData = new NFMsg.ObjectPropertyString();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyString xPropertyString = new NFMsg.PropertyString();
            xPropertyString.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyString.Data = ByteString.CopyFromUtf8(newVar);
            xData.PropertyList.Add(xPropertyString);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload String");
            SendMsg((int)NFMsg.EGameMsgID.AckPropertyString, mxBody);
        }

        public void RequirePropertyObject(NFGUID objectID, string strPropertyName, NFGUID newVar)
        {
            NFMsg.ObjectPropertyObject xData = new NFMsg.ObjectPropertyObject();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyObject xPropertyObject = new NFMsg.PropertyObject();
            xPropertyObject.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyObject.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xPropertyObject);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload Object");
            SendMsg((int)NFMsg.EGameMsgID.AckPropertyObject, mxBody);
        }

        public void RequirePropertyVector2(NFGUID objectID, string strPropertyName, NFVector2 newVar)
        {
            NFMsg.ObjectPropertyVector2 xData = new NFMsg.ObjectPropertyVector2();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyVector2 xProperty = new NFMsg.PropertyVector2();
            xProperty.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xProperty.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xProperty);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.AckPropertyVector2, mxBody);
        }

        public void RequirePropertyVector3(NFGUID objectID, string strPropertyName, NFVector3 newVar)
        {
            NFMsg.ObjectPropertyVector3 xData = new NFMsg.ObjectPropertyVector3();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            NFMsg.PropertyVector3 xProperty = new NFMsg.PropertyVector3();
            xProperty.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xProperty.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xProperty);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.AckPropertyVector3, mxBody);
        }

		public void RequireAddRow(NFGUID objectID, string strRecordName, int nRow)
        {
            NFMsg.ObjectRecordAddRow xData = new NFMsg.ObjectRecordAddRow();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordAddRowStruct xRecordAddRowStruct = new NFMsg.RecordAddRowStruct();
            xData.RowData.Add(xRecordAddRowStruct);
            xRecordAddRowStruct.Row = nRow;

			NFIObject xObject = mKernelModule.GetObject(objectID);
            NFIRecord xRecord = xObject.GetRecordManager().GetRecord(strRecordName);
            NFDataList xRowData = xRecord.QueryRow(nRow);
            for (int i = 0; i < xRowData.Count(); i++)
            {
                switch (xRowData.GetType(i))
                {
                    case NFDataList.VARIANT_TYPE.VTYPE_INT:
                        {
                            NFMsg.RecordInt xRecordInt = new NFMsg.RecordInt();
                            xRecordInt.Row = nRow;
                            xRecordInt.Col = i;
                            xRecordInt.Data = xRowData.IntVal(i);
                            xRecordAddRowStruct.RecordIntList.Add(xRecordInt);
                        }
                        break;
                    case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                        {
                            NFMsg.RecordFloat xRecordFloat = new NFMsg.RecordFloat();
                            xRecordFloat.Row = nRow;
                            xRecordFloat.Col = i;
                            xRecordFloat.Data = (float)xRowData.FloatVal(i);
                            xRecordAddRowStruct.RecordFloatList.Add(xRecordFloat);
                        }
                        break;
                    case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                        {
                            NFMsg.RecordString xRecordString = new NFMsg.RecordString();
                            xRecordString.Row = nRow;
                            xRecordString.Col = i;
                            xRecordString.Data = ByteString.CopyFromUtf8(xRowData.StringVal(i));
                            xRecordAddRowStruct.RecordStringList.Add(xRecordString);
                        }
                        break;
                    case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                        {
                            NFMsg.RecordObject xRecordObject = new NFMsg.RecordObject();
                            xRecordObject.Row = nRow;
                            xRecordObject.Col = i;
                            xRecordObject.Data = mHelpModule.NFToPB(xRowData.ObjectVal(i));
                            xRecordAddRowStruct.RecordObjectList.Add(xRecordObject);
                        }
                        break;
                    case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                        {
                            NFMsg.RecordVector2 xRecordVector = new NFMsg.RecordVector2();
                            xRecordVector.Row = nRow;
                            xRecordVector.Col = i;
                            xRecordVector.Data = mHelpModule.NFToPB(xRowData.Vector2Val(i));
                            xRecordAddRowStruct.RecordVector2List.Add(xRecordVector);
                        }
                        break;
                    case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                        {
                            NFMsg.RecordVector3 xRecordVector = new NFMsg.RecordVector3();
                            xRecordVector.Row = nRow;
                            xRecordVector.Col = i;
                            xRecordVector.Data = mHelpModule.NFToPB(xRowData.Vector3Val(i));
                            xRecordAddRowStruct.RecordVector3List.Add(xRecordVector);
                        }
                        break;

                }
            }

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record addRow");
            SendMsg((int)NFMsg.EGameMsgID.AckAddRow, mxBody);
        }

		public void RequireRemoveRow(NFGUID objectID, string strRecordName, int nRow)
        {
            NFMsg.ObjectRecordRemove xData = new NFMsg.ObjectRecordRemove();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.RemoveRow.Add(nRow);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record removeRow");
            SendMsg((int)NFMsg.EGameMsgID.AckRemoveRow, mxBody);
        }

		public void RequireSwapRow(NFGUID objectID, string strRecordName, int nOriginRow, int nTargetRow)
        {
            NFMsg.ObjectRecordSwap xData = new NFMsg.ObjectRecordSwap();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.OriginRecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.TargetRecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.RowOrigin = nOriginRow;
            xData.RowTarget = nTargetRow;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record swapRow");
            SendMsg((int)NFMsg.EGameMsgID.AckSwapRow, mxBody);
        }

		public void RequireRecordInt(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordInt xData = new NFMsg.ObjectRecordInt();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordInt xRecordInt = new NFMsg.RecordInt();
            xData.PropertyList.Add(xRecordInt);
            xRecordInt.Row = nRow;
            xRecordInt.Col = nCol;
            xRecordInt.Data = newVar.IntVal();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record int");
            SendMsg((int)NFMsg.EGameMsgID.AckRecordInt, mxBody);
        }

		public void RequireRecordFloat(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordFloat xData = new NFMsg.ObjectRecordFloat();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordFloat xRecordFloat = new NFMsg.RecordFloat();
            xData.PropertyList.Add(xRecordFloat);
            xRecordFloat.Row = nRow;
            xRecordFloat.Col = nCol;
            xRecordFloat.Data = (float)newVar.FloatVal();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record float");
            SendMsg((int)NFMsg.EGameMsgID.AckRecordFloat, mxBody);
        }

		public void RequireRecordString(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordString xData = new NFMsg.ObjectRecordString();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordString xRecordString = new NFMsg.RecordString();
            xData.PropertyList.Add(xRecordString);
            xRecordString.Row = nRow;
            xRecordString.Col = nCol;
            xRecordString.Data = ByteString.CopyFromUtf8(newVar.StringVal());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record string");
            SendMsg((int)NFMsg.EGameMsgID.AckRecordString, mxBody);
        }

		public void RequireRecordObject(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordObject xData = new NFMsg.ObjectRecordObject();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordObject xRecordObject = new NFMsg.RecordObject();
            xData.PropertyList.Add(xRecordObject);
            xRecordObject.Row = nRow;
            xRecordObject.Col = nCol;
            xRecordObject.Data = mHelpModule.NFToPB(newVar.ObjectVal());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record object");
            SendMsg((int)NFMsg.EGameMsgID.AckRecordObject, mxBody);
        }

		public void RequireRecordVector2(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordVector2 xData = new NFMsg.ObjectRecordVector2();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordVector2 xRecordVector = new NFMsg.RecordVector2();
            xRecordVector.Row = nRow;
            xRecordVector.Col = nCol;
            xRecordVector.Data = mHelpModule.NFToPB(newVar.Vector2Val());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.AckRecordVector2, mxBody);
        }

		public void RequireRecordVector3(NFGUID objectID, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
        {
            NFMsg.ObjectRecordVector3 xData = new NFMsg.ObjectRecordVector3();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            NFMsg.RecordVector3 xRecordVector = new NFMsg.RecordVector3();
            xRecordVector.Row = nRow;
            xRecordVector.Col = nCol;
            xRecordVector.Data = mHelpModule.NFToPB(newVar.Vector3Val());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.AckRecordVector3, mxBody);
        }

        ////////////////////////////////////修改自身属性end
        /////////////////////////////////////////////////////////////////

        public void RequireEnterGameServer()
        {
            NFMsg.ReqEnterGameServer xData = new NFMsg.ReqEnterGameServer();
			xData.Name = ByteString.CopyFromUtf8(mLoginModule.mRoleName);
			xData.Account = ByteString.CopyFromUtf8(mLoginModule.mAccount);
			xData.GameId = 0;
			xData.Id = mHelpModule.NFToPB(mLoginModule.mRoleID);
			
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.ReqEnterGame, mxBody);
        }

        public void RequireEnterGameFinish()
        {
            //only use in the first time when player enter game world
            NFMsg.ReqAckEnterGameSuccess xData = new NFMsg.ReqAckEnterGameSuccess();
            xData.Arg = 1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.ReqEnterGameFinish, mxBody);
        }

        //发送心跳
        public void RequireHeartBeat()
        {
            NFMsg.ReqHeartBeat xData = new NFMsg.ReqHeartBeat();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.StsHeartBeat, mxBody);
        }

        //发送心跳
        public void RequireLagTest(int index)
        {
            NFMsg.ReqAckLagTest xData = new NFMsg.ReqAckLagTest();
            xData.Index = index;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.ReqLagTest, mxBody);
        }

        //WSAD移动
        public void RequireMove(NFGUID objectID, int nType, UnityEngine.Vector3 vPos)
        {
            NFMsg.ReqAckPlayerPosSync xData = new NFMsg.ReqAckPlayerPosSync();

            NFMsg.PosSyncUnit posSyncUnit = new PosSyncUnit();
            posSyncUnit.Mover = mHelpModule.NFToPB(objectID);
            posSyncUnit.Pos = new NFMsg.Vector3();
            posSyncUnit.Pos.X = vPos.x;
            posSyncUnit.Pos.Y = vPos.y;
            posSyncUnit.Pos.Z = vPos.z;
            xData.SyncUnit.Add(posSyncUnit);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.ReqMove, mxBody);

            //为了表现，客户端先走，后续同步
        }

        public void RequireMoveImmune(NFGUID objectID, UnityEngine.Vector3 vPos)
        {
            NFMsg.ReqAckPlayerPosSync xData = new NFMsg.ReqAckPlayerPosSync();

            NFMsg.PosSyncUnit posSyncUnit = new PosSyncUnit();
            posSyncUnit.Mover = mHelpModule.NFToPB(objectID);
            posSyncUnit.Pos = new NFMsg.Vector3();
            posSyncUnit.Pos.X = vPos.x;
            posSyncUnit.Pos.Y = vPos.y;
            posSyncUnit.Pos.Z = vPos.z;
            xData.SyncUnit.Add(posSyncUnit);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.ReqMoveImmune, mxBody);
        }

        //有可能是他副本的NPC移动,因此增加64对象ID
        public void RequireUseSkill(NFGUID objectID, string strKillID, Int32 index, List<NFGUID> nTargetIDList)
        {
            NFMsg.ReqAckUseSkill xData = new NFMsg.ReqAckUseSkill();
            xData.User = mHelpModule.NFToPB(objectID);
            xData.SkillId = ByteString.CopyFromUtf8(strKillID);
            xData.UseIndex = index;

            if (nTargetIDList != null)
            {
                foreach (NFGUID id in nTargetIDList)
                {
                    NFMsg.EffectData xEffData = new NFMsg.EffectData();
                    xEffData.EffectIdent = (mHelpModule.NFToPB(id));
                    xEffData.EffectValue = 0;
                    xEffData.EffectRlt = 0;

                    xData.EffectData.Add(xEffData);
                }
            }

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.ReqSkillObjectx, mxBody);
        }

        public void RequireSyncPosition(NFMsg.ReqAckPlayerPosSync reqAckPlayerPosSync)
        {
            mxBody.SetLength(0);
            reqAckPlayerPosSync.WriteTo(mxBody);

            SendMsg((int)NFMsg.EGameMsgID.ReqPosSync, mxBody);
        }

        public void RequireSwapScene(int nTransferType, int nSceneID, int nLineIndex)
        {
            NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
            xData.TransferType = nTransferType;
            xData.SceneId = nSceneID;
            xData.LineId = nLineIndex;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.ReqSwapScene, mxBody);
        }

        public void RequireSwapScene(int nSceneID)
        {
            NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
            xData.TransferType = 0;
            xData.SceneId = nSceneID;
            xData.LineId = -1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.ReqSwapScene, mxBody);
        }

        public void RequireSwapScene(int nTransferType, int nSceneID)
        {
            NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
            xData.TransferType = nTransferType;
            xData.SceneId = nSceneID;
            xData.LineId = -1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)NFMsg.EGameMsgID.ReqSwapScene, mxBody);
        }

        /////////////////////////////////////////////////////////////////
    }
}