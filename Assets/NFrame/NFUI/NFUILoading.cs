using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using NFSDK;
using NFrame;
using UnityEngine.UI;

public class NFUILoading : NFUIDialog
{

    private AsyncOperation asy;
    private int mnProgress = 0;


	public Sprite[] xTextureList;
    public Image GUIHolder;
    public Slider slider;
    public Image bar;

    private int mnSceneID = 0;

	private NFLoginModule mLoginModule;
    private NFUIModule mUIModule;
    private NFSceneModule mSceneModule;
	private NFIEventModule mEventModule;
	private NFIElementModule mElementModule;

    private void Awake()
    {
        NFIPluginManager xPluginManager = NFPluginManager.Instance();
        mLoginModule = xPluginManager.FindModule<NFLoginModule>();
        mUIModule = xPluginManager.FindModule<NFUIModule>();
        mSceneModule = xPluginManager.FindModule<NFSceneModule>();

        mEventModule = xPluginManager.FindModule<NFIEventModule>();
        mElementModule = xPluginManager.FindModule<NFIElementModule>();

    }

    // Use this for initialization
    public override void Init()
	{
	}
    
	void Start()
    {
    }

    void OnGUI()
    {
    }

    private void Update()
    {
        if (asy != null)
        {
            if (!asy.isDone)
            {
                Debug.Log("mnProgress " + asy.progress + " " + mnProgress + " " + Time.time);

                bar.fillAmount = mnProgress / 100f;
            }
            else
            {
                bar.fillAmount = 0;
            }
        }

    }

    public void LoadLevel(int nSceneID, Vector3 vector)
    {
   
        mnSceneID = nSceneID;
        bar.fillAmount = 0;

        mnProgress = 0;
		NFSDK.NFIElement xElement = mElementModule.GetElement(nSceneID.ToString());
		if (null != xElement) 
		{
			string strName = xElement.QueryString (NFrame.Scene.SceneName);
			string strUIName = xElement.QueryString (NFrame.Scene.LoadingUI);

			UnityEngine.SceneManagement.Scene xSceneInfo = SceneManager.GetActiveScene ();
			if (xSceneInfo.name == strName)
			{
				//Debug.LogWarning ("begin the same scene" + strSceneID);
				//SceneManager.LoadScene (xSceneInfo.buildIndex);
				//Debug.LogWarning ("end the same scene" + strSceneID);
				//load a empty scene then load this scene asy

				//SceneManager.LoadScene ("EmptyScene");
			}

            //Debug.Log("mnProgress----start " + mnProgress + " " + Time.time);

            StartCoroutine(LoadLevel (nSceneID, strName, vector, strUIName));
		}
		else 
		{
			//Debug.LogError ("LoadLevel error: " + nSceneID);
		}

        //NFRender.Instance.SetMainRoleAgentState(true);
    }

    private IEnumerator LoadLevel(int nSceneID, string strSceneID, Vector3 vector, string strUI)
    {
        mnProgress += Random.Range(6, 19);
        yield return new WaitForEndOfFrame();

        asy = SceneManager.LoadSceneAsync (strSceneID);
        asy.allowSceneActivation = false;

        DirectoryInfo mydir = new DirectoryInfo(strUI);
        if (mydir.Exists)
        {
            //xTexture = GameObject.Instantiate(Resources.Load(strUI)) as Texture;
        }

		if (xTextureList != null && xTextureList.Length > 0)
		{
			int nIndex = Random.Range(0, xTextureList.Length);
            Sprite xSprite = xTextureList[nIndex];
            Image xImage = GUIHolder.GetComponent<Image>();
            xImage.overrideSprite = xSprite;
		}

        #region 优化进度的 
        while (asy.progress < 0.9f)
        {
            mnProgress += Random.Range(9, 20);
            if (mnProgress > 100) { mnProgress = 100; }
            //Debug.Log("mnProgress-----0-- " + asy.progress + " " + mnProgress + " " + Time.time);
            yield return new WaitForEndOfFrame();
        }

        while (mnProgress < 90)
        {
            if (asy.allowSceneActivation == false)
            {
                asy.allowSceneActivation = true;
            }

            if (asy.isDone)
            {
                mnProgress += Random.Range(11, 19);
                if (mnProgress > 100) { mnProgress = 100; }
            }
            else
            {
                mnProgress += Random.Range(8, 15);
                if (mnProgress > 100) { mnProgress = 100; }
            }

            //Debug.Log("mnProgress-----1-- " + asy.isDone + asy.progress + " " + mnProgress + " " + Time.time);
            yield return new WaitForEndOfFrame();
        }

        if (asy.allowSceneActivation == false)
        {
            asy.allowSceneActivation = true;
        }

        while (mnProgress < 100)
        {
            if (asy.isDone)
            {
                mnProgress += Random.Range(4, 8);
                if (mnProgress > 100) { mnProgress = 100; }
            }
            else
            {
                mnProgress += Random.Range(2, 5);
                if (mnProgress > 100) { mnProgress = 100; }
            }

            //Debug.Log("mnProgress-----2--  " + asy.isDone + " " + asy.progress + " " + mnProgress + " " + Time.time);
            yield return new WaitForEndOfFrame();
        }

        #endregion

        while (!asy.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log("mnProgress-----3-- " + asy.progress + " " + mnProgress + " " + Time.time);

        mSceneModule.LoadSceneEnd(mnSceneID);
        mUIModule.CloseUI<NFUILoading>();
    }
}