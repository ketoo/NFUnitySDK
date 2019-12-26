using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFrame;
using NFSDK;

public class NFLanguage : MonoBehaviour 
{
	//maybe for text, maybe for sprite(sprite_name)
	public string strText;

	private NFLanguageModule mLanguageModule;

	void Awake()
	{
		mLanguageModule = NFPluginManager.Instance().FindModule<NFLanguageModule>();
		mLanguageModule.AddLanguageUI (this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		RefreshUIData ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnDestroy()
	{
		mLanguageModule.RemLanguageUI (this.gameObject);
	}

	public void RefreshUIData()
	{
		//language option
		string strData = mLanguageModule.GetLocalLanguage(strText);
		Text xText = GetComponent<Text> ();
		if (xText) 
		{
			xText.text = strData;
		} 
		else		
		{
			//image
		}
	}

}
