using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NFMsg;
using UnityEngine;
using NFSDK;
using Google.Protobuf;

namespace NFrame
{
	public class NFLoginModule : NFIModule
    {    
		public enum Event : int
        {
			Connected = 0,
            Disconnected,
            ConnectionRefused,

            RoleList = 10,
			LoginSuccess,
            LoginFailure,
            WorldList,
            ServerList,
            SelectServerSuccess,
        };


        public string mAccount;
        public string mKey;
        public ArrayList mWorldServerList = new ArrayList();
        public ArrayList mGameServerList = new ArrayList();


		public NFGUID mRoleID = new NFGUID();
        public string mRoleName;
        public ArrayList mRoleList = new ArrayList();

		private MemoryStream mxBody = new MemoryStream();

        private NFNetModule mNetModule;
        private NFUIModule mUIModule;
        private NFIEventModule mEventModule;
        private NFIKernelModule mKernelModule;

        public NFLoginModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
		}

        public override void Awake()
		{
            mNetModule = mPluginManager.FindModule<NFNetModule>();
            mUIModule = mPluginManager.FindModule<NFUIModule>();
            mEventModule = mPluginManager.FindModule<NFIEventModule>();
            mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
        }

        public override void Init()
		{
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.AckConnectKey, OnConnectKey);
            //mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.ACK_SELECT_SERVER, OnSelectServer);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.AckRoleList, EGMI_ACK_ROLE_LIST);



			mEventModule.RegisterCallback((int)NFLoginModule.Event.Connected, OnConnected);
			mEventModule.RegisterCallback((int)NFLoginModule.Event.Disconnected, OnDisconnected);
        }

        public override void AfterInit()
        {

        }

        public override void Execute()
        {
        }

        public override void BeforeShut()
        {
        }

        public override void Shut()
        {
        }

		public void OnConnected(NFDataList valueList)
        {
			if (mKey != null && mKey.Length > 0)
			{
				//verify token
                //RequireVerifyWorldKey(mAccount, mKey);
			}
        }

		public void OnDisconnected(NFDataList valueList)
        {
			if (mKey != null)
            {
                //reconnect
                mAccount = "";
                mKey = "";
                mWorldServerList.Clear();
                mGameServerList.Clear();
                mRoleID = new NFGUID();
                mRoleName = "";
                mRoleList.Clear();

                //Clear all players and UI objects
                NFDataList xDataList = mKernelModule.GetObjectList();
                for (int i = 0; i < xDataList.Count(); ++i)
                {
                    mKernelModule.DestroyObject(xDataList.ObjectVal(i));
                }

                mUIModule.DestroyAllUI();
                mUIModule.ShowUI<NFUILogin>();
            }
        }

	    public void RequireVerifyWorldKey(string strAccount, string strKey)
        {
            mAccount = strAccount;
            mKey = strKey;

            NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
            xData.Account = ByteString.CopyFromUtf8(strAccount);
            xData.Password = ByteString.CopyFromUtf8("");
            xData.SecurityCode = ByteString.CopyFromUtf8(strKey);
            xData.SignBuff = ByteString.CopyFromUtf8("");
            xData.ClientVersion = 1;
            xData.LoginMode = 0;
            xData.ClientIP = 0;
            xData.ClientMAC = 0;
            xData.DeviceInfo = ByteString.CopyFromUtf8("");
            xData.ExtraInfo = ByteString.CopyFromUtf8("");

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.ReqConnectKey, mxBody);
        }

        private void OnConnectKey(UInt16 id, MemoryStream stream)
        {
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (xData.EventCode == EGameEventCode.VerifyKeySuccess)
            {
                Debug.Log("VerifyKey SUCCESS");
                RequireRoleList();
            }
            else
            {
                Debug.Log("VerifyKey Failed");
            }
        }

        public void RequireRoleList()
        {
            NFMsg.ReqRoleList xData = new NFMsg.ReqRoleList();
            xData.Account = ByteString.CopyFromUtf8(mAccount);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            mNetModule.SendMsg(NFMsg.EGameMsgID.ReqRoleList, mxBody);
        }

        private void EGMI_ACK_ROLE_LIST(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckRoleLiteInfoList xData = NFMsg.AckRoleLiteInfoList.Parser.ParseFrom(xMsg.MsgData);

			Debug.Log("QueryRoleList  SUCCESS : " + xData.CharData.Count);

			mRoleList.Clear();

            for (int i = 0; i < xData.CharData.Count; ++i)
            {
                NFMsg.RoleLiteInfo info = xData.CharData[i];

                Debug.Log("QueryRoleList  SUCCESS : " + info.NoobName.ToStringUtf8());

				mRoleList.Add(info);
            }

			//mEventModule.DoEvent((int)NFLoginModule.Event.RoleList);

            //////////////////
			if (mRoleList.Count > 0)
            {
				NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)mRoleList[0];
                NFGUID xEnterID = new NFGUID();
                xEnterID.nData64 = xLiteInfo.Id.Index;
                xEnterID.nHead64 = xLiteInfo.Id.Svrid;

                mRoleID = xEnterID;
                mRoleName = xLiteInfo.NoobName.ToStringUtf8();

                mNetModule.RequireEnterGameServer();

                Debug.Log("Selected role :" + xLiteInfo.NoobName.ToStringUtf8());
            }
            else
            {
				RequireCreateRole( mAccount, 0, 0);
                Debug.Log("No Role!, require to create a new role! ");
            }
        }


        public void RequireCreateRole(string strRoleName, int byCareer, int bySex)
        {
            if (strRoleName.Length >= 20 || strRoleName.Length < 1)
            {
                return;
            }

            NFMsg.ReqCreateRole xData = new NFMsg.ReqCreateRole();
            xData.Career = byCareer;
            xData.Sex = bySex;
            xData.NoobName = ByteString.CopyFromUtf8(strRoleName);
			xData.Account = ByteString.CopyFromUtf8(mAccount);
            xData.Race = 0;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.ReqCreateRole, mxBody);
        }


        public void RequireDelRole(string strRoleName)
        {
            NFMsg.ReqDeleteRole xData = new NFMsg.ReqDeleteRole();
            xData.Name = ByteString.CopyFromUtf8(strRoleName);
            xData.Account = ByteString.CopyFromUtf8(mAccount);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.ReqDeleteRole, mxBody);
        }
    };
}