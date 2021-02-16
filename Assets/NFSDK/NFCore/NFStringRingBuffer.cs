//-----------------------------------------------------------------------
// <copyright file="NFStringRingBuffer.cs">
//     Copyright (C) 2015-2019 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NFSDK
{


    //template<typename T>
    class NFStringRingBuffer
    {
        public NFStringRingBuffer(int size = 1024 * 1024 * 1, bool main = false)
        {
            m_main = main;
            m_capacity = size;
            m_front = 0;
            m_rear = 0;
            m_data = new byte[m_capacity];
            m_trasferTempData = new byte[1024];
        }
        private bool m_main;
        private int m_capacity;
        private int m_front;
        private int m_rear;
        //T* m_data;
        private byte[] m_data;
        private byte[] m_trasferTempData;
        public bool Empty()
        {
            return m_front == m_rear;
        }

        public int Size()
        {
            if (m_rear > m_front)
            {
                return m_rear - m_front;
            }
            else if (m_rear == m_front)
            {
                return 0;
            }

            return (m_rear + Capacity() - m_front);
        }

        public int Capacity()
        {
            return m_capacity;
        }

        public bool Full(int newSize)
        {
            return (Size() + newSize) >= Capacity();
        }

        public bool Push(NFStringRingBuffer source, int size, bool readOnly = false)
        {
            if (size > source.Size())
            {
                return false;
            }

            if (m_trasferTempData.Length < size)
            {
                m_trasferTempData = new byte[size];
            }

            source.Pop(m_trasferTempData, size, readOnly);

            this.Push(m_trasferTempData, size);

            return true;
        }

        public void ToMemoryStream(MemoryStream ms)
        {
            if (Size() <= 0)
            {
                return;
            }

            if (m_rear >= m_front)
            {
                ms.Write(m_data, m_front, Size());
            }
            else
            {
                ms.Write(m_data, m_front, Capacity() - m_front);
                ms.Write(m_data, 0,m_rear);
            }
        }

        Int64 totalNetBandwidth = 0;
        int stackCount = 0;
        public bool Push(byte[] src, int size)
        {
            if (m_main)
            {
                stackCount += size;
                totalNetBandwidth += size;
                //UnityEngine.Debug.LogError("Push:" + size + " stack count:" + stackCount);
            }

            while (Full(size))
            {
                resize();
            }

            if (m_rear >= m_front)
            {
                if (m_rear + size >= Capacity())
                {
                    int offset = Capacity() - m_rear;
                    Array.Copy(src, 0, m_data, m_rear, offset);
                    Array.Copy(src, offset, m_data, 0, size - offset);
                }
                else
                {
                    Array.Copy(src, 0, m_data, m_rear, size);
                }
            }
            else
            {
                if (m_rear + size < m_front)
                {
                    Array.Copy(src, 0, m_data, m_rear, size);
                }
                else
                {
                    Console.WriteLine("impassible!!!!");
                }
            }

            m_rear = (m_rear + size) % Capacity();

            return true;
        }

        public bool Pop(byte[] dst, int size, bool readOnly = false)
        {
            if (size <= 0 || Size() < size)
            {
                return false;
            }

            if (m_main && !readOnly)
            {
                stackCount -= size;
                //UnityEngine.Debug.LogError("Pop:" + size + " stack count:" + stackCount);
            }
    
            if (m_rear > m_front)
            {
                Array.Copy(m_data, m_front, dst, 0, size);
            }
            else
            {
                int tempOffset = Capacity() - m_front;
                if (tempOffset >= size)
                {
                    Array.Copy(m_data, m_front, dst, 0, size);
                }
                else
                {
                    Array.Copy(m_data, m_front, dst, 0, tempOffset);
                    Array.Copy(m_data, 0, dst, tempOffset, size - tempOffset);
                }
            }

            if (!readOnly)
            {
                m_front = (m_front + size) % Capacity();
            }

            return true;
        }

        public void Print()
        {
            Console.WriteLine("-----------size:" + Size().ToString() + " capacity:" + Capacity() + " m_front:" + m_front + " m_rear:" + m_rear);
        }

        private void resize()
        {
            byte[] tmp = new byte[m_capacity * 2];
            if (m_rear < m_front)
            {
                int oldSize = Size();
                int offset = m_capacity - m_front;
                Array.Copy(m_data, m_front, tmp, 0, offset);
                Array.Copy(m_data, 0, tmp, offset, m_rear);
                m_front = 0;
                m_rear = oldSize;
            }
            else
            {
                Array.Copy(m_data, 0, tmp, 0, m_capacity);
            }


            m_capacity *= 2;
            m_data = tmp;
        }

        public void Clear()
        {
            m_front = 0;
            m_rear = 0;
        }
    }
}