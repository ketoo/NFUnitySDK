using UnityEngine;
using System.Collections;

namespace NFSDK
{
	public abstract class NFIModule
	{
		//------------- ½Ó¿Ú -------------------//
		public abstract void Awake();
		public abstract void Init();
		public abstract void AfterInit();
		public abstract void Execute();
		public abstract void BeforeShut();
		public abstract void Shut();

        public NFIPluginManager mPluginManager;
        public string mName;
    };
}