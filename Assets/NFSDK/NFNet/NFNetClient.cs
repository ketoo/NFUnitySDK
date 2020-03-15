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

namespace NFSDK
{
    public enum NFNetState
    {
        Connecting,
        Connected,
        Disconnected
    }
	
	public enum NFNetEventType
    {
        None,
        Connected,
        Disconnected,
        ConnectionRefused,
        DataReceived
    }

    public class NFSocketPacket
    {
        public NFSocketPacket()
        {
            sb = new NFStringRingBuffer(ConstDefine.NF_PACKET_BUFF_SIZE);
        }

        private NFStringRingBuffer sb;
        internal NFStringRingBuffer Sb { get => sb; set => sb = value; }

        public void Reset()
        {
            sb.Clear();
        }

        public void FromBytes(byte[] by, int bytesCount)
        {
            sb.Clear();
            sb.Push(by, bytesCount);
        }
    }

    public class NFNetEventParams
    {
        public void Reset()
        {
             client = null;
             clientID = 0;
             socket = null;
             eventType = NFNetEventType.None;
             message = "";
             packet = null;
        }

        public NFNetClient client = null;
        public int clientID = 0;
        public TcpClient socket = null;
        public NFNetEventType eventType = NFNetEventType.None;
        public string message = "";
        public NFSocketPacket packet = null;

    }
	
    public class NFNetClient
    {
        
		public NFNetClient(NFNetListener xNetListener)
        {
			mxNetListener = xNetListener;
            Init();
        }

        void Init()
        {

            mxState = NFNetState.Disconnected;
            mxEvents = new Queue<NFNetEventType>();
            mxMessages = new Queue<string>();
            mxPackets = new Queue<NFSocketPacket>();
            mxPacketPool = new Queue<NFSocketPacket>();
        }

        private NFNetState mxState;
        private NetworkStream mxStream;
        private StreamWriter mxWriter;
        private StreamReader mxReader;
        private Thread mxReadThread;
        private TcpClient mxClient;
        private Queue<NFNetEventType> mxEvents;
        private Queue<string> mxMessages;
        private Queue<NFSocketPacket> mxPackets;
        private Queue<NFSocketPacket> mxPacketPool;

        private NFNetListener mxNetListener;

        private byte[] tempReadBytes = new byte[ConstDefine.NF_PACKET_BUFF_SIZE];

        private NFNetEventParams eventParams = new NFNetEventParams();

        public bool IsConnected()
        {
            return mxState == NFNetState.Connected;
        }

        public NFNetState GetState()
        {
            return mxState;
        }

        public NFNetListener GetNetListener()
        {
            return mxNetListener;
        }

        public void Execute()
        {
			
            while (mxEvents.Count > 0)
            {
                lock (mxEvents)
                {
                    NFNetEventType eventType = mxEvents.Dequeue();

                    eventParams.Reset();
                    eventParams.eventType = eventType;
                    eventParams.client = this;
                    eventParams.socket = mxClient;

                    if (eventType == NFNetEventType.Connected)
                    {
                        mxNetListener.OnClientConnect(eventParams);
                    }
                    else if (eventType == NFNetEventType.Disconnected)
                    {
						mxNetListener.OnClientDisconnect(eventParams);

                        mxReader.Close();
                        mxWriter.Close();
                        mxClient.Close();

                    }
                    else if (eventType == NFNetEventType.DataReceived)
                    {
                        lock (mxPackets)
                        {
                            eventParams.packet = mxPackets.Dequeue();
                        
                            mxNetListener.OnDataReceived(eventParams);

                            mxPacketPool.Enqueue(eventParams.packet);
                        }
                    }
                    else if (eventType == NFNetEventType.ConnectionRefused)
                    {

                    }
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.EndConnect(ar);

                SetTcpClient(tcpClient);

            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NFNetEventType.ConnectionRefused);
                }
            }
        }
        private NFSocketPacket GetPacketFromPool()
        {
            if (mxPacketPool.Count <= 0)
            {
                mxPacketPool.Enqueue(new NFSocketPacket());
            }

            NFSocketPacket packet = mxPacketPool.Dequeue();
            packet.Reset();

            return packet;
        }
        private void ReadData()
        {
            bool endOfStream = false;

            while (!endOfStream)
            {
               int bytesRead = 0;
               try
               {
                    Array.Clear(tempReadBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
                    bytesRead = mxStream.Read(tempReadBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
               }
               catch (Exception e)
               {
                   e.ToString();
               }

               if (bytesRead == 0)
               {

                   endOfStream = true;

               }
               else
               {
                   lock (mxEvents)
                   {

                       mxEvents.Enqueue(NFNetEventType.DataReceived);
                   }
                   lock (mxPackets)
                   {
                        NFSocketPacket packet = GetPacketFromPool();
                        packet.FromBytes(tempReadBytes, bytesRead);

                        mxPackets.Enqueue(packet);
                   }

               }
            }

            mxState = NFNetState.Disconnected;

            mxClient.Close();
            lock (mxEvents)
            {
                mxEvents.Enqueue(NFNetEventType.Disconnected);
            }

        }

        // Public
        public void Connect(string hostname, int port)
        {
            if (mxState == NFNetState.Connected)
            {
                return;
            }

            mxState = NFNetState.Connecting;

            mxMessages.Clear();
            mxEvents.Clear();

            mxClient = new TcpClient();
            mxClient.NoDelay = true;
            mxClient.BeginConnect(hostname,
                                 port,
                                 new AsyncCallback(ConnectCallback),
                                 mxClient);

        }

        public void Disconnect()
        {
            mxState = NFNetState.Disconnected;

            try { if (mxReader != null) mxReader.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxWriter != null) mxWriter.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxClient != null) mxClient.Close(); }
            catch (Exception e) { e.ToString(); }

        }

        public void SendBytes(byte[] bytes, int length)
        {
            SendBytes(bytes, 0, length);
        }

        private void SendBytes(byte[] bytes, int offset, int size)
        {

            if (!IsConnected())
                return;
            try
            {
                mxStream.Write(bytes, offset, size);
                mxStream.Flush();
            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NFNetEventType.Disconnected);
                    Disconnect();
                }
            }

        }

        private void SetTcpClient(TcpClient tcpClient)
        {

            mxClient = tcpClient;

            if (mxClient.Connected)
            {

                mxStream = mxClient.GetStream();
                mxReader = new StreamReader(mxStream);
                mxWriter = new StreamWriter(mxStream);

                mxState = NFNetState.Connected;

                mxEvents.Enqueue(NFNetEventType.Connected);

                mxReadThread = new Thread(ReadData);
                mxReadThread.IsBackground = true;
                mxReadThread.Start();
            }
            else
            {
                mxState = NFNetState.Disconnected;
            }
        }
    }
}