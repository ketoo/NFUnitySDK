using UnityEngine;
using System.Collections;
using NFrame;

namespace NFSDK
{
    public class NFSDKPlugin : NFIPlugin
    {
        public NFSDKPlugin(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
        public override string GetPluginName()
        {
			return "NFSDKPlugin";
        }

        public override void Install()
        {

            AddModule<NFIElementModule>(new NFElementModule(mPluginManager));
            AddModule<NFIClassModule>(new NFClassModule(mPluginManager));
            AddModule<NFIKernelModule>(new NFKernelModule(mPluginManager));
            AddModule<NFIEventModule>(new NFEventModule(mPluginManager));

            AddModule<NFNetEventModule>(new NFNetEventModule(mPluginManager));
            AddModule<NFLagTestModule>(new NFLagTestModule(mPluginManager));
            AddModule<NFLogModule>(new NFLogModule(mPluginManager));
            AddModule<NFLoginModule>(new NFLoginModule(mPluginManager));
            AddModule<NFNetHandlerModule>(new NFNetHandlerModule(mPluginManager));
            AddModule<NFNetModule>(new NFNetModule(mPluginManager));
            AddModule<NFHelpModule>(new NFHelpModule(mPluginManager));
            AddModule<NFLanguageModule>(new NFLanguageModule(mPluginManager));
            AddModule<NFUploadDataModule>(new NFUploadDataModule(mPluginManager));
        }
        public override void Uninstall()
        {

            mPluginManager.RemoveModule<NFUploadDataModule>();
            mPluginManager.RemoveModule<NFNetEventModule>();
            mPluginManager.RemoveModule<NFLanguageModule>();
            mPluginManager.RemoveModule<NFHelpModule>();
            mPluginManager.RemoveModule<NFNetModule>();
            mPluginManager.RemoveModule<NFNetHandlerModule>();
            mPluginManager.RemoveModule<NFLoginModule>();
            mPluginManager.RemoveModule<NFLogModule>();
            mPluginManager.RemoveModule<NFLagTestModule>();

            mPluginManager.RemoveModule<NFIElementModule>();
			mPluginManager.RemoveModule<NFIClassModule>();
			mPluginManager.RemoveModule<NFIKernelModule>();
			mPluginManager.RemoveModule<NFIEventModule>();

            mModules.Clear();
        }
    }
}
