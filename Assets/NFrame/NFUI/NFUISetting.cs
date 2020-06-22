using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;

public class NFUISetting : NFUIDialog
{
    private NFLanguageModule mLanguageModule;
    private NFLoginModule mLoginModule;
    private NFUIModule mUIModule;
    private NFIEventModule mEventModule;

    public Toggle toggleLowQuanlity;
    public Toggle toggleMidQuanlity;
    public Toggle toggleHighQuanlity;


    public Toggle toggleCHINEASE;
    public Toggle toggleENGLISH;
    public Toggle toggleFRENCH;
    public Toggle toggleSPANISH;


    public Button exitButton;

    public Scrollbar musicScrollbar;
    public Scrollbar soundScrollbar;


    private void Awake()
    {
        NFIPluginManager xPluginManager = NFRoot.Instance().GetPluginManager();
        mLoginModule = xPluginManager.FindModule<NFLoginModule>();
        mUIModule = xPluginManager.FindModule<NFUIModule>();
        mEventModule = xPluginManager.FindModule<NFIEventModule>();
        mLanguageModule = xPluginManager.FindModule<NFLanguageModule>();
    }

    public override void Init()
    {

        NFRoot.Instance().mbShowCMDGUI = true;

        toggleLowQuanlity.onValueChanged.AddListener(delegate (bool isOn) {
            this.OnQuanlityValueChanged(isOn, toggleLowQuanlity);
        });

        toggleMidQuanlity.onValueChanged.AddListener(delegate (bool isOn) {
            this.OnQuanlityValueChanged(isOn, toggleMidQuanlity);
        });

        toggleHighQuanlity.onValueChanged.AddListener(delegate (bool isOn) {
            this.OnQuanlityValueChanged(isOn, toggleHighQuanlity);
        });

        toggleCHINEASE.onValueChanged.AddListener(delegate (bool isOn) {
            mLanguageModule.SetLocalLanguage(NFrame.Language.Chinese);
        });

        toggleENGLISH.onValueChanged.AddListener(delegate (bool isOn) {
            mLanguageModule.SetLocalLanguage(NFrame.Language.English);
        });

        toggleFRENCH.onValueChanged.AddListener(delegate (bool isOn) {
            mLanguageModule.SetLocalLanguage(NFrame.Language.French);
        });

        toggleSPANISH.onValueChanged.AddListener(delegate (bool isOn) {
            mLanguageModule.SetLocalLanguage(NFrame.Language.Spanish);
        });

        exitButton.onClick.AddListener(OnExitBtnClick);
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
  
    }

    public void OnCMDChanged(bool ison, Toggle sender)
    {
        NFRoot.Instance().mbShowCMDGUI = ison;
    }

    public void OnQuanlityValueChanged(bool ison, Toggle sender)
    {
        string[] names = QualitySettings.names;

        if (sender.gameObject.GetInstanceID() == toggleLowQuanlity.gameObject.GetInstanceID())
        {
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == "Fast")
                {
                    QualitySettings.SetQualityLevel(i, true);
                    break;
                }
            }
        }
        else if (sender.gameObject.GetInstanceID() == toggleMidQuanlity.gameObject.GetInstanceID())
        {
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == "Good")
                {
                    QualitySettings.SetQualityLevel(i, true);
                    break;
                }
            }
        }
        else if (sender.gameObject.GetInstanceID() == toggleHighQuanlity.gameObject.GetInstanceID())
        {
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == "Beautiful")
                {
                    QualitySettings.SetQualityLevel(i, true);
                    break;
                }
            }
        }
    }

    private void OnExitBtnClick()
    {
        mUIModule.CloseAllUI();

        mUIModule.ShowUI<NFUIMain>();
        mUIModule.ShowUI<NFUIEstateBar>();

        mUIModule.ShowUI<NFUIJoystick>();

    }
}
