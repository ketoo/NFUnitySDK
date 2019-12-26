using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFUIGray
{
    private static Material grayMat;

    /// <summary>
    /// 创建置灰材质球
    /// </summary>
    /// <returns></returns>
    private static Material GetGrayMat()
    {
        if (grayMat == null)
        {
            Shader shader = Shader.Find("Custom/UI-Gray");
            if (shader == null)
            {
                Debug.Log("null");
                return null;
            }
            Material mat = new Material(shader);
            grayMat = mat;
        }

        return grayMat;
    }

    /// <summary>
    /// 图片置灰
    /// </summary>
    /// <param name="img"></param>
    public static void SetUIGray(Image img)
    {
        img.material = GetGrayMat();
        img.SetMaterialDirty();
    }

    /// <summary>
    /// 图片回复
    /// </summary>
    /// <param name="img"></param>
    public static void Recovery(Image img)
    {
        img.material = null;
    }

}