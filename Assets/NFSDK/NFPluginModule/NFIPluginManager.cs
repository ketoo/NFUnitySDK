using UnityEngine;
using System.Collections;
using System;

namespace NFSDK
{
    public abstract class NFIPluginManager : NFIModule
    {
        //------------- ½Ó¿Ú -------------------//
        public abstract T _FindModule<T>() where T : NFIModule;
        public abstract NFIModule _FindModule(string strModuleName);
		public abstract void Registered(NFIPlugin plugin);
        public abstract void UnRegistered(NFIPlugin plugin);
        public abstract void AddModule(string strModuleName, NFIModule pModule);
		public abstract void RemoveModule(string strModuleName);
		public abstract void _RemoveModule<T>() where T : NFIModule;

        public abstract Int64 GetInitTime();
        public abstract Int64 GetNowTime();


		public T FindModule<T>() where T : NFIModule
		{
			return _FindModule<T>();
		}

		public NFIModule FindModule(string strModuleName)
		{
			return _FindModule(strModuleName);
		}

		public void RemoveModule<T>() where T : NFIModule
		{
			_RemoveModule<T>();
		}
	};
}