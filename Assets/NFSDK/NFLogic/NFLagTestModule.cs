using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NFrame;
using NFSDK;
using UnityEngine;

namespace NFrame
{
	public class NFLagTestModule : NFIModule
    {
        private NFNetModule mNetModule;
        private Dictionary<int, float> mLagTestData = new Dictionary<int, float>();
        private int index = 0;
        private Ping ping;

        public int gateLagTime = 0;
        public int gameLagTime = 0;
        public int netLagTime = 0;

        List<int> gateLagTimeList = new List<int>();
        List<int> gameLagTimeList = new List<int>();

        public NFLagTestModule(NFIPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

		public override void Awake()
        {
            mNetModule = mPluginManager.FindModule<NFNetModule>();
        }

		public override void Init()
        {
        }

		public override void AfterInit()
        {
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckGateLagTest, EGEC_ACK_GATE_LAG_TEST);
            mNetModule.AddReceiveCallBack((int)NFMsg.EGameMsgID.AckGameLagTest, EGEC_ACK_GAME_LAG_TEST);
        }

        float lastTime = 0f;
		public override void Execute()
		{
            // send a lag test message per 5 seconds
            if (Time.realtimeSinceStartup - lastTime > 5f)
            {
                lastTime = Time.realtimeSinceStartup;

                SendLagTest();
                //PingTest();
            }
            /*
            if (null != ping && ping.isDone)
            {
                netLagTime = ping.time;
            }
            */
        }

		public override void BeforeShut()
		{
		}

		public override void Shut()
		{
		}

        private void PingTest()
        {
            ping = null;

            if (mNetModule.strGameServerIP != null)
            {
                ping = new Ping(mNetModule.strGameServerIP);
            }
        }

        private void SendLagTest()
        {
            index++;

            mNetModule.RequireLagTest(index);

            mLagTestData.Add(index, Time.realtimeSinceStartup);
        }

        private void EGEC_ACK_GATE_LAG_TEST(int id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ReqAckLagTest xData = NFMsg.ReqAckLagTest.Parser.ParseFrom(xMsg.MsgData);

            float time;
            if (mLagTestData.TryGetValue(xData.Index, out time))
            {
                float lagTime = Time.realtimeSinceStartup - time;
                gateLagTime = (int)(lagTime * 1000);

                if (gateLagTimeList.Count > 10)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("gateLagTime:");
                    foreach (var item in gateLagTimeList)
                    {
                        sb.Append(item);
                        sb.Append(",");
                    }

                    Debug.Log(sb.ToString());

                    gateLagTimeList.Clear();
                }
            }
        }

        void EGEC_ACK_GAME_LAG_TEST(int id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ReqAckLagTest xData = NFMsg.ReqAckLagTest.Parser.ParseFrom(xMsg.MsgData);

            float time;
            if (mLagTestData.TryGetValue(xData.Index, out time))
            {
                float lagTime = Time.realtimeSinceStartup - time;
                gameLagTime = (int)(lagTime * 1000);

                gateLagTimeList.Add(gateLagTime);

                if (gameLagTimeList.Count > 10)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("gameLagTime:");
                    foreach (var item in gateLagTimeList)
                    {
                        sb.Append(item);
                        sb.Append(",");
                    }

                    Debug.Log(sb.ToString());

                    gameLagTimeList.Clear();
                }

                mLagTestData.Remove(xData.Index);
            }
        }
    }

}