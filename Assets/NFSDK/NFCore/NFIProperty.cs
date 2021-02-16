//-----------------------------------------------------------------------
// <copyright file="NFIProperty.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSDK
{
    public abstract class NFIProperty
    {
        public delegate void PropertyEventHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar, Int64 reason);

        public abstract string GetKey();

        public abstract NFDataList.VARIANT_TYPE GetType();
        public abstract NFDataList.TData GetData();

        public abstract void SetUpload(bool upload);

        public abstract bool GetUpload();

        public abstract Int64 QueryInt();

        public abstract double QueryFloat();

        public abstract string QueryString();

        public abstract NFGUID QueryObject();

        public abstract NFVector2 QueryVector2();

        public abstract NFVector3 QueryVector3();

        public abstract bool SetInt(Int64 value, Int64 reason = 0);

        public abstract bool SetFloat(double value, Int64 reason = 0);

        public abstract bool SetString(string value, Int64 reason = 0);

        public abstract bool SetObject(NFGUID value, Int64 reason = 0);

        public abstract bool SetVector2(NFVector2 value, Int64 reason = 0);

        public abstract bool SetVector3(NFVector3 value, Int64 reason = 0);

        public abstract bool SetData(NFDataList.TData x);

        public abstract void RegisterCallback(PropertyEventHandler handler);
    }
}