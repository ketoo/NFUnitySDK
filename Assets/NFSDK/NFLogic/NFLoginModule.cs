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
        public int mServerID;
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
        private NFHelpModule mHelpModule;

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
            mHelpModule = mPluginManager.FindModule<NFHelpModule>();
        }

        public override void Init()
		{
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckLogin, OnLoginProcess);
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckWorldList, OnWorldList);
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckConnectWorld, OnConnectWorld);
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckConnectKey, OnConnectKey);
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckSelectServer, OnSelectServer);

            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckRoleList, EGMI_ACK_ROLE_LIST);


            mEventModule.RegisterCallback((int)NFLoginModule.Event.Connected, OnConnected);
			mEventModule.RegisterCallback((int)NFLoginModule.Event.Disconnected, OnDisconnected);

            mEventModule.RegisterCallback((int)NFLoginModule.Event.LoginSuccess, OnLoginSuccess);
            mEventModule.RegisterCallback((int)NFLoginModule.Event.WorldList, OnWorldList);
            mEventModule.RegisterCallback((int)NFLoginModule.Event.ServerList, OnServerList);
            mEventModule.RegisterCallback((int)NFLoginModule.Event.SelectServerSuccess, OnSelectServer);
            mEventModule.RegisterCallback((int)NFLoginModule.Event.RoleList, OnRoleList);
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
                RequireVerifyWorldKey(mAccount, mKey);
			}
        }

		public void OnDisconnected(NFDataList valueList)
        {
			if (mKey != null)
            {
                //reconnect
                mAccount = "";
                mKey = "";
                mServerID = 0;
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

                mUIModule.CloseAllUI();
                mUIModule.ShowUI<NFUILogin>();
            }
        }
        
        // 请求消息
	    public void LoginPB(string strAccount, string strPwd, string strKey)
        {
            NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
            xData.Account = ByteString.CopyFromUtf8(strAccount);
            xData.Password = ByteString.CopyFromUtf8(strPwd);
            xData.SecurityCode = ByteString.CopyFromUtf8(strKey);
            xData.SignBuff = ByteString.CopyFromUtf8("");
            xData.ClientVersion = 1;
            xData.LoginMode = NFMsg.ELoginMode.ElmAutoRegisterLogin;
            xData.ClientIP = 0;
            xData.ClientMAC = 0;
            xData.DeviceInfo = ByteString.CopyFromUtf8("");
            xData.ExtraInfo = ByteString.CopyFromUtf8("");

            mAccount = strAccount;
            /*
            MemoryStream stream = new MemoryStream();
            xData.WriteTo(stream);
            mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_LOGIN, stream);
*/
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqLogin, mxBody);
        }

	    public void RequireWorldList()
        {
            NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
            xData.Type = NFMsg.ReqServerListType.RsltWorldServer;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqWorldList, mxBody);
        }

	    public void RequireConnectWorld(int nWorldID)
        {
            NFMsg.ReqConnectWorld xData = new NFMsg.ReqConnectWorld();
            xData.WorldId = nWorldID;
            xData.LoginId = 0;
            xData.Account = ByteString.CopyFromUtf8("");
            xData.Sender = new Ident();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqConnectWorld, mxBody);
        }

	    public void RequireVerifyWorldKey(string strAccount, string strKey)
        {
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

            mAccount = strAccount;
            mKey = strKey;


            mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqConnectKey, mxBody);
        }

	    public void RequireServerList()
        {
            NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
            xData.Type = NFMsg.ReqServerListType.RsltGamesErver;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqWorldList, mxBody);
        }

	    public void RequireSelectServer(int nServerID)
        {
            NFMsg.ReqSelectServer xData = new NFMsg.ReqSelectServer();
            xData.WorldId = nServerID;
            mServerID = nServerID;
                        
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqSelectServer, mxBody);
        }

        // 接收消息
		private void OnLoginProcess(int id, MemoryStream stream)
        {
			NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (EGameEventCode.AccountSuccess == xData.EventCode)
            {
				Debug.Log("Login  SUCCESS");
				mEventModule.DoEvent((int)NFLoginModule.Event.LoginSuccess);
            }
            else
            {
                Debug.Log("Login Faild,Code: " + xData.EventCode);
                NFDataList varList = new NFDataList();
                varList.AddInt((Int64)xData.EventCode);
				mEventModule.DoEvent((int)NFLoginModule.Event.LoginFailure);
            }
        }

        private void OnWorldList(int id, MemoryStream stream)
        {
            
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckServerList xData = NFMsg.AckServerList.Parser.ParseFrom(xMsg.MsgData);

            if (ReqServerListType.RsltWorldServer == xData.Type)
            {
                for (int i = 0; i < xData.Info.Count; ++i)
                {
                    ServerInfo info = xData.Info[i];
                    Debug.Log("WorldList  ServerId: " + info.ServerId + " Name: " + info.Name.ToStringUtf8() + " Status: " + info.Status);
                    mWorldServerList.Add(info);
                }

				mEventModule.DoEvent((int)NFLoginModule.Event.WorldList);
            }
            else if (ReqServerListType.RsltGamesErver == xData.Type)
            {
                for (int i = 0; i < xData.Info.Count; ++i)
                {
                    ServerInfo info = xData.Info[i];
                    Debug.Log("GameList  ServerId: " + info.ServerId + " Name: " + info.Name.ToStringUtf8() + " Status: " + info.Status);
                    mGameServerList.Add(info);
                }
				mEventModule.DoEvent((int)NFLoginModule.Event.ServerList);
            }
        }

        private void OnConnectWorld(int id, MemoryStream stream)
        {
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckConnectWorldResult xData = NFMsg.AckConnectWorldResult.Parser.ParseFrom(xMsg.MsgData);

            mKey = xData.WorldKey.ToStringUtf8();
            
			mNetModule.BeforeShut();
			mNetModule.Shut();

			String strIP = xData.WorldIp.ToStringUtf8();
			if (strIP == "127.0.0.1")
			{
				strIP = mNetModule.FirstIP();
			}
			mNetModule.StartConnect(strIP, xData.WorldPort);

        }
        private void OnConnectKey(int id, MemoryStream stream)
        {
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (xData.EventCode == EGameEventCode.VerifyKeySuccess)
            {
                Debug.Log("VerifyKey SUCCESS");
                RequireServerList();
            }
            else
            {
                Debug.Log("VerifyKey Failed");
            }
        }

        private void OnSelectServer(int id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream); 

            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (xData.EventCode == EGameEventCode.SelectserverSuccess)
            {
                Debug.Log("SelectGame SUCCESS ");
				mEventModule.DoEvent((int)NFLoginModule.Event.SelectServerSuccess);
            }
            else
            {
                Debug.Log("SelectGame Failed ");
            }
        }


        private void EGMI_ACK_ROLE_LIST(int id, MemoryStream stream)
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


			mEventModule.DoEvent((int)NFLoginModule.Event.RoleList);

            //////////////////
            /*
			if (mRoleList.Count > 0)
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_ROLEHALL);

				NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)mRoleList[0];
                NFGUID xEnterID = new NFGUID();
                xEnterID.nData64 = xLiteInfo.id.index;
                xEnterID.nHead64 = xLiteInfo.id.svrid;

				mNetModule.RequireEnterGameServer(xEnterID, mAccount, xLiteInfo.noob_name.ToStringUtf8(), mServerID);

                //mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME;

                Debug.Log("Selected role :" + xLiteInfo.noob_name.ToStringUtf8());
            }
            else
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_CREATEHALL);
				RequireCreateRole( mAccount, 0, 0, mServerID);
                Debug.Log("No Role!, require to create a new role! ");
            }
            */
        }

        //申请角色列表
        public void RequireRoleList()
        {
            NFMsg.ReqRoleList xData = new NFMsg.ReqRoleList();
			xData.GameId = mServerID;
			xData.Account = ByteString.CopyFromUtf8(mAccount);
            
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqRoleList, mxBody);
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

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqCreateRole, mxBody);
        }


        public void RequireDelRole(string strRoleName)
        {
            NFMsg.ReqDeleteRole xData = new NFMsg.ReqDeleteRole();
            xData.Name = ByteString.CopyFromUtf8(strRoleName);
            xData.Account = ByteString.CopyFromUtf8(mAccount);
			xData.GameId = mServerID;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)NFMsg.EGameMsgID.ReqDeleteRole, mxBody);


            Debug.Log("RequireDelRole:" + strRoleName);
        }


        // Logic Event
        public void OnLoginSuccess(NFDataList valueList)
        {
            //mUIModule.ShowUI<NFUISelectServer>();

            RequireWorldList();
        }

        public void OnWorldList(NFDataList valueList)
        {
            Debug.Log("OnWorldList" + mWorldServerList.Count);

            foreach (NFMsg.ServerInfo info in mWorldServerList)
            {

                RequireConnectWorld(info.ServerId);
                break;
            }
        }

        public void OnSelectServer(NFDataList valueList)
        {
            RequireRoleList();
        }

        public void OnServerList(NFDataList valueList)
        {
            ArrayList serverList = mGameServerList;

            Debug.Log("OnServerList" + serverList.Count);


            foreach (NFMsg.ServerInfo info in serverList)
            {
                RequireSelectServer(info.ServerId);
                break;
            }
        }
        // Logic Event
        public void OnRoleList(NFDataList valueList)
        {
            ArrayList roleList = mRoleList;

            foreach (NFMsg.RoleLiteInfo info in roleList)
            {
                OnRoleClick(0);
                break;
            }

            OnCreateRoleClick();
        }

        private void OnRoleClick(int nIndex)
        {
            ArrayList roleList = mRoleList;
            NFMsg.RoleLiteInfo info = (NFMsg.RoleLiteInfo)roleList[nIndex];

            mRoleID = mHelpModule.PBToNF(info.Id);
            mRoleName = info.NoobName.ToStringUtf8();

            mNetModule.RequireEnterGameServer();
        }

        private void OnCreateRoleClick()
        {

            string strRoleName = mAccount + "_Role";
            //string strRoleName = mLoginModule.mAccount + "_Role" + UnityEngine.Random.Range(1000, 10000);
            RequireCreateRole(strRoleName, 1, 1);
        }
    };
}