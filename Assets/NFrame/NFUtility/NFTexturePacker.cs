using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine.UI;

public class NFTexturePacker
{
    #region Instance
    private static NFTexturePacker _Instance = null;
    private static readonly object _syncLock = new object();
    public static NFTexturePacker Instance
    {
        get
        {
            lock (_syncLock)
            {
                if (_Instance == null)
                {
                    _Instance = new NFTexturePacker();
                }
                return _Instance;
            }
        }
    }
    #endregion


	static Dictionary<string, Dictionary<string, Sprite>> mxSpriteMap = new Dictionary<string, Dictionary<string, Sprite>>();

	public Sprite GetSprit(string strPath, string strIcon)
	{
		//return Resources.Load<Sprite>(strPath + "/" + strIcon + ".png");
		Dictionary<string, Sprite> xSpriteDic;
		if (!mxSpriteMap.TryGetValue(strPath, out xSpriteDic))
		{
			////---------path为Sprite在Resources中的路径-----------
			/// 
			//Sprite[] group = Resources.LoadAll<Sprite>(path);
			//Sprite[] group1 = Resources.LoadAll<Sprite>("UIResources");
			//Sprite[] group2 = Resources.LoadAll<Sprite>("UIResources/Item");
			Sprite[] xSpriteList = Resources.LoadAll<Sprite>(strPath);
			if (null == xSpriteList || xSpriteList.Length <= 0)
			{
				Debug.LogError ("no texture --- " + strPath);
				return null;
			}

			xSpriteDic = new Dictionary<string, Sprite> ();
			mxSpriteMap[strPath] = xSpriteDic;

			foreach (Sprite uiSprite in xSpriteList)
			{
				xSpriteDic [uiSprite.name] = uiSprite;
			}

			//Debug.Log ("load texture --- " + strPath + " Sprite count: " + xSpriteList.Length);
		}


		if (null != xSpriteDic && xSpriteDic.ContainsKey(strIcon))
		{
			return xSpriteDic[strIcon];
		}

		return null;
	}
}
