//-----------------------------------------------------------------------
// <copyright file="NFILogicClassModule.cs">
//     Copyright (C) 2015-2019 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NFSDK
{
	public class ConstDefine
	{
		public static int NF_PACKET_BUFF_SIZE = 65535;
		public static int NF_PACKET_HEAD_SIZE = 6;
		public static int MAX_PACKET_LEN = 1024 * 1024 * 20;
	};

	public class MsgHead
	{
		public MsgHead()
		{
			unMsgID = 0;
			unDataLen = 0;
		}
		public UInt16 unMsgID;
		public UInt32 unDataLen;

		public void Reset()
		{
			unMsgID = 0;
			unDataLen = 0;
			Array.Clear(byHead, 0, ConstDefine.NF_PACKET_HEAD_SIZE);
		}

		private byte[] byHead = new byte[ConstDefine.NF_PACKET_HEAD_SIZE];

		public byte[] GetHeadBytes()
		{
			return byHead;
		}

		public byte[] EnCode()
		{
			byte[] tempByMsgID = BitConverter.GetBytes(unMsgID);
			byte[] tempByDataLen = BitConverter.GetBytes(unDataLen);

			bool isLittle = BitConverter.IsLittleEndian;
			if (isLittle)
			{
				Array.Reverse(tempByMsgID);
				Array.Reverse(tempByDataLen);
			}

			Array.Copy(tempByMsgID, 0, byHead, 0, sizeof(UInt16));
			Array.Copy(tempByDataLen, 0, byHead, sizeof(UInt16), sizeof(UInt32));

			return byHead;
		}

		private byte[] byMsgID = new byte[sizeof(UInt16)];
		private byte[] byDataLen = new byte[sizeof(UInt32)];
		public bool DeCode()
		{
			Array.Clear(byMsgID, 0, sizeof(UInt16));
			Array.Clear(byDataLen, 0, sizeof(UInt16));

			Array.Copy(byHead, 0, byMsgID, 0, sizeof(UInt16));
			Array.Copy(byHead, sizeof(UInt16), byDataLen, 0, sizeof(UInt32));

			bool isLittle = BitConverter.IsLittleEndian;
			if (isLittle)
			{
				Array.Reverse(byMsgID);
				Array.Reverse(byDataLen);
			}

			unMsgID = BitConverter.ToUInt16(byMsgID,0);
			unDataLen = BitConverter.ToUInt32(byDataLen,0);

			return true;
		}
	};

    public class NFNetListener
    {      
		private NFStringRingBuffer mPacket = new NFStringRingBuffer(ConstDefine.MAX_PACKET_LEN);

		public delegate void EventDelegation(NFNetEventType eventType);
		private EventDelegation mHandlerDelegation;
        
		public delegate void MsgDelegation(int id, MemoryStream stream);
		private Dictionary<int, MsgDelegation> mhtMsgDelegation = new Dictionary<int, MsgDelegation>();

		private MsgHead head = new MsgHead();
		private MemoryStream dataReceivedBodyStream = new MemoryStream();

		private NFStringRingBuffer body_and_head = new NFStringRingBuffer(ConstDefine.MAX_PACKET_LEN);
		//////////////////////////////////////////////////////////////////
		/// 
		public void OnClientConnect(NFNetEventParams eventParams)
		{
			mPacket.Clear();

			//NFLogModule.Instance.Log(NFLogModule.LOG_LEVEL.DEBUG, "Client connected");
			if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NFNetEventType.Connected);
            }
        }

		public void OnClientDisconnect(NFNetEventParams eventParams)
        {
            if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NFNetEventType.Disconnected);
            }
        }

        public void OnClientConnectionRefused(NFNetEventParams eventParams)
        {
            if (mHandlerDelegation != null)
            {
                mHandlerDelegation(NFNetEventType.ConnectionRefused);
            }
        }

		public void OnDataReceived(NFNetEventParams eventParams)
		{
			mPacket.Push(eventParams.packet.Sb, eventParams.packet.Sb.Size());
			eventParams.packet.Sb.Clear();

			OnDataReceived();
		}

		void OnDataReceived()
		{
			if (mPacket.Size() >= ConstDefine.NF_PACKET_HEAD_SIZE)
			{
				head.Reset();

				if (mPacket.Pop(head.GetHeadBytes(), ConstDefine.NF_PACKET_HEAD_SIZE, true))
                {
					if (head.DeCode())
					{
						if (head.unDataLen == mPacket.Size())
						{
							body_and_head.Clear();
							body_and_head.Push(mPacket, (int)head.unDataLen);

							if (false == OnDataReceived(body_and_head))
							{
								OnClientDisconnect(new NFNetEventParams());
							}
						}
						else if (mPacket.Size() > head.unDataLen)
						{
							body_and_head.Clear();
							body_and_head.Push(mPacket, (int)head.unDataLen);

							if (false == OnDataReceived(body_and_head))
							{
								OnClientDisconnect(new NFNetEventParams());
							}

							OnDataReceived();
						}
					}
				}
			}
		}

		bool OnDataReceived(NFStringRingBuffer sb)
		{
			head.Reset();

			if (sb.Pop(head.GetHeadBytes(), ConstDefine.NF_PACKET_HEAD_SIZE))
            {
				if (head.DeCode() && head.unDataLen == sb.Size() + ConstDefine.NF_PACKET_HEAD_SIZE)
				{
					Int32 nBodyLen = (Int32)sb.Size();
					if (nBodyLen > 0)
					{
						dataReceivedBodyStream.SetLength(0);
						dataReceivedBodyStream.Position = 0;
						sb.ToMemoryStream(dataReceivedBodyStream);

						OnMessageEvent(head, dataReceivedBodyStream);

						return true;
					}
					else
					{
						//space packet, thats impossible
					}
				}
			}


			return false;
		}

		private void OnMessageEvent(MsgHead head, MemoryStream ms)
        {
            if (mhtMsgDelegation.ContainsKey(head.unMsgID))
            {
                MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[head.unMsgID];
				ms.Position = 0;
				myDelegationHandler(head.unMsgID, ms);
            }
            else
            {
				Debug.LogError("ReciveMsg:" + head.unMsgID + "  and no handler!!!!");
            }
        }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool RegisteredNetEventHandler(EventDelegation eventHandler)
        {
            mHandlerDelegation += eventHandler;
            return true;
        }

		public bool RegisteredDelegation(int eMsg, MsgDelegation msgDelegate)
		{
			if(!mhtMsgDelegation.ContainsKey(eMsg))
			{
				MsgDelegation myDelegationHandler = new MsgDelegation(msgDelegate);
				mhtMsgDelegation.Add(eMsg, myDelegationHandler);
			}
			else
			{
				MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[eMsg];
				myDelegationHandler += new MsgDelegation(msgDelegate);
                mhtMsgDelegation[eMsg] = myDelegationHandler;
            }

			return true;
		}

		public void RemoveDelegation(int eMsg)
		{
			mhtMsgDelegation.Remove(eMsg);
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	}
}
