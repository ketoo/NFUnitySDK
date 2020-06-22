using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;
using System.Text;

public class NFUILogin : NFUIDialog
{
	private NFIEventModule mEventModule;


	private NFNetModule mNetModule;
	private NFLoginModule mLoginModule;
	private NFUIModule mUIModule;
    private NFHelpModule mHelpModule;

	public InputField mAccount;
	public InputField mPassword;
	public Button mLogin;
	// Use this for initialization


	public override void Init()
	{
		mEventModule = NFRoot.Instance().GetPluginManager().FindModule<NFIEventModule>();

		mNetModule = NFRoot.Instance().GetPluginManager().FindModule<NFNetModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();
        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();
		mHelpModule = NFRoot.Instance().GetPluginManager().FindModule<NFHelpModule>();

		mLogin.onClick.AddListener(OnLoginClick);

        mEventModule.RegisterCallback((int)NFLoginModule.Event.LoginSuccess, OnLoginSuccess);
	}

	void Start () 
	{
        mAccount.text = PlayerPrefs.GetString("account");
        mPassword.text = PlayerPrefs.GetString("password");
    }

    // UI Event
    private void OnLoginClick()
    {
        PlayerPrefs.SetString("account", mAccount.text);
        PlayerPrefs.SetString("password", mPassword.text);
        //mLoginModule.LoginPB(mAccount.text, mPassword.text, "");
        mLoginModule.RequireVerifyWorldKey(mAccount.text, mPassword.text);
    }
    
    // Logic Event
	public void OnLoginSuccess(NFDataList valueList)
    {
		//mUIModule.ShowUI<NFUISelectServer>();

        //mLoginModule.RequireWorldList();
    }
}
