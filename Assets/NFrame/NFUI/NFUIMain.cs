using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;

public class NFUIMain : NFUIDialog
{
	   
	private NFLoginModule mLoginModule;
	private NFNetModule mNetModule;
    private NFUIModule mUIModule;
    private NFIEventModule mEventModule;
    private NFIKernelModule mKernelModule;
    private NFIElementModule mElementModule;

    public Button mHead;
    public Image headIcon;
    public Text headName;
    public Text hpText;
	public Image hpScrollbar;


	public Button mBag;
	public Button mHero;
    public Button mChat;
    public Button mFriend;

    public Button setting;
    public Button s1Button;
    public Button s2Button;
    public Button s3Button;
    public Button s4Button;


    private void Awake()
    {
        NFIPluginManager xPluginManager = NFPluginManager.Instance();
        mNetModule = xPluginManager.FindModule<NFNetModule>();
        mLoginModule = xPluginManager.FindModule<NFLoginModule>();
        mUIModule = xPluginManager.FindModule<NFUIModule>();
        mEventModule = xPluginManager.FindModule<NFIEventModule>();

        mKernelModule = NFPluginManager.Instance().FindModule<NFIKernelModule>();
        mElementModule = NFPluginManager.Instance().FindModule<NFIElementModule>();

    }

    // Use this for initialization
    public override void Init ()
	{
        mHead.onClick.AddListener(OnBagClick);
        mBag.onClick.AddListener(OnBagClick);
		mHero.onClick.AddListener(OnHeroClick);
        mChat.onClick.AddListener(OnChatClick);
        mFriend.onClick.AddListener(OnFriendClick);

        setting.onClick.AddListener(OnSettingClick);
        s1Button.onClick.AddListener(OnS1Click);
        s2Button.onClick.AddListener(OnS2Click);
        s3Button.onClick.AddListener(OnS3Click);
        s4Button.onClick.AddListener(OnS4Click);


		mKernelModule.RegisterPropertyCallback(mLoginModule.mRoleID, NFrame.Player.ConfigID, OnConfigIDChange);
        mKernelModule.RegisterPropertyCallback(mLoginModule.mRoleID, NFrame.Player.HP, OnHPChange);
        mKernelModule.RegisterPropertyCallback(mLoginModule.mRoleID, NFrame.Player.MAXHP, OnHPChange);
	}

	private void OnBagClick()
    {
        mUIModule.CloseAllUI();

        NFUIProfile profile = mUIModule.ShowUI<NFUIProfile>();
        //profile.ShowHeroTab();
    }

	private void OnHeroClick()
    {
        mUIModule.CloseAllUI();

        mUIModule.ShowUI<NFUIEstateBar>();

    }

	private void OnS1Click()
    {

    }
	private void OnS2Click()
	{

	}
	private void OnS3Click()
	{

	}
	private void OnS4Click()
	{

	}

	private void OnChatClick()
    {

    }

    private void OnFriendClick()
    {
    }
    
    private void OnTownClick()
    {
        mNetModule.RequireSwapScene(1);
    }

    private void OnSettingClick()
    {
        mUIModule.CloseAllUI();

        mUIModule.ShowUI<NFUISetting>();
    }

    private void OnConfigIDChange(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        /*
        string strConfigID = newVar.StringVal();
        string strIconName = mElementModule.QueryPropertyString(strConfigID, NFrame.Item.Icon);
        string strIconFileName = mElementModule.QueryPropertyString(strConfigID, NFrame.Item.SpriteFile);
        Sprite xSprite = NFTexturePacker.Instance.GetSprit(strIconFileName, strIconName);
        if (xSprite != null)
        {
            headIcon.overrideSprite = xSprite;
        }
        */
    }

    private void OnHPChange(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        int maxHP = (int)mKernelModule.QueryPropertyInt(self, NFrame.Player.MAXHP);
        int hp = (int)mKernelModule.QueryPropertyInt(self, NFrame.Player.HP);
        if (maxHP > 0)
		{
			hpScrollbar.fillAmount = (float)hp / maxHP;
		}
		else
		{
			hpScrollbar.fillAmount = 0;
		}

		hpText.text = hp.ToString() + "/" + maxHP.ToString();
    }

    private void OnEnable()
    {
   
    }
}
