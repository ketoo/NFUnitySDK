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
        mEventModule = NFPluginManager.Instance().FindModule<NFIEventModule>();
        mKernelModule = NFPluginManager.Instance().FindModule<NFIKernelModule>();
        mElementModule = NFPluginManager.Instance().FindModule<NFIElementModule>();
        mClassModule = NFPluginManager.Instance().FindModule<NFIClassModule>();

        mSceneModule = NFPluginManager.Instance().FindModule<NFSceneModule>();
        mLanguageModule = NFPluginManager.Instance().FindModule<NFLanguageModule>();
        mNetModule = NFPluginManager.Instance().FindModule<NFNetModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mUIModule = NFPluginManager.Instance().FindModule<NFUIModule>();
        mHelpModule = NFPluginManager.Instance().FindModule<NFHelpModule>();

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