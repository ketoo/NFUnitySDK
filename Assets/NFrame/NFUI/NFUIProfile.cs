using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NFSDK;
using NFrame;

public class NFUIProfile : NFUIDialog
{
	public Button exitButton;

    public Transform inProgressController;
    public Transform doneController;
    public Transform finishController;

    /// </summary>
    private NFIEventModule mEventModule;
	private NFIKernelModule mKernelModule;
	private NFIElementModule mElementModule;
    private NFIClassModule mClassModule;

    private NFSceneModule mSceneModule;
	private NFLanguageModule mLanguageModule;
    private NFNetModule mNetModule;
    private NFLoginModule mLoginModule;
    private NFUIModule mUIModule;
    private NFHelpModule mHelpModule;

    private void Awake()
    {
        mEventModule = NFRoot.Instance().GetPluginManager().FindModule<NFIEventModule>();
        mKernelModule = NFRoot.Instance().GetPluginManager().FindModule<NFIKernelModule>();
        mElementModule = NFRoot.Instance().GetPluginManager().FindModule<NFIElementModule>();
        mClassModule = NFRoot.Instance().GetPluginManager().FindModule<NFIClassModule>();

        mSceneModule = NFRoot.Instance().GetPluginManager().FindModule<NFSceneModule>();
        mLanguageModule = NFRoot.Instance().GetPluginManager().FindModule<NFLanguageModule>();
        mNetModule = NFRoot.Instance().GetPluginManager().FindModule<NFNetModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();
        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();
        mHelpModule = NFRoot.Instance().GetPluginManager().FindModule<NFHelpModule>();

    }

    public override void Init()
    {
        exitButton.onClick.AddListener(OnExitBtnClick);
    }

    private void OnExitBtnClick()
    {
        mUIModule.CloseUI<NFUIProfile>();

        mUIModule.ShowUI<NFUIMain>();
        mUIModule.ShowUI<NFUIEstateBar>();

        mUIModule.ShowUI<NFUIJoystick>();

    }
}