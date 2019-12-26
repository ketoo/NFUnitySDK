using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;
using System.Text;

public class NFUIMsgBox : NFUIDialog
{

    public enum MsgType
    {
        BOX_TYPE_YES_NO,
    }

	public delegate void ShowMsgCodeDelegation();

	private ShowMsgCodeDelegation mAcceptEventDelegation;
	private ShowMsgCodeDelegation mRejectEventDelegation;

    private NFIEventModule mEventModule;

	private NFNetModule mNetModule;
	private NFLoginModule mLoginModule;
	private NFUIModule mUIModule;
    private NFHelpModule mHelpModule;

    public Text mTitle;
    public Text mText;
	public Button mAcceptButton;
	public Button mRejectButton;
    // Use this for initialization


    public override void Init()
	{
		mEventModule = NFPluginManager.Instance().FindModule<NFIEventModule>();

		mNetModule = NFPluginManager.Instance().FindModule<NFNetModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mUIModule = NFPluginManager.Instance().FindModule<NFUIModule>();
		mHelpModule = NFPluginManager.Instance().FindModule<NFHelpModule>();

        mAcceptButton.onClick.AddListener(OnAcceptClick);
        mRejectButton.onClick.AddListener(OnRejectClick);
	}

	void Start () 
	{
    }

    // UI Event

    private void OnAcceptClick()
    {
		mUIModule.CloseUI<NFUIMsgBox>();

		if (mAcceptEventDelegation != null)
		{
			mAcceptEventDelegation();
		}
	}

    private void OnRejectClick()
    {
		mUIModule.CloseUI<NFUIMsgBox>();

		if (mRejectEventDelegation != null)
		{
			mRejectEventDelegation();
		}
	}

    public void SetData(MsgType type, string title, string text, ShowMsgCodeDelegation acceptHandler, ShowMsgCodeDelegation rejectHandler)
    {
		mText.text = text;
		mTitle.text = title;
		mAcceptEventDelegation = acceptHandler;
		mRejectEventDelegation = rejectHandler;

		if (string.IsNullOrEmpty(text))
		{
			mText.gameObject.SetActive(false);
		}

		if (string.IsNullOrEmpty(title))
		{
			mTitle.gameObject.SetActive(false);
		}
	}
}