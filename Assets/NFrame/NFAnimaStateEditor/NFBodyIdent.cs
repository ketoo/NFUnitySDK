using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFSDK;
using NFrame;

public class NFBodyIdent : MonoBehaviour 
{
	public string ObjectID;
	public string cnfID;
	public Sprite iCon;

    public Transform xShadow = null;
	public Transform xHealthBar = null;
    public Transform xWeapon = null;
    public Transform xFirePoint = null;
    public Transform xBullet = null;

    public Transform xBeHitPoint = null;
	public Transform xHeadPoint = null;
    public Transform xRenderObject = null;
    public Transform xRenderBase = null;


	private NFGUID mxID;
	private Dictionary<string, Transform> xChildList = new Dictionary<string, Transform>();


    Transform FindTransform(string name)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
                return t;
        }
        return null;
    }

	public NFGUID GetObjectID()
	{
		return mxID;
	}

	public void SetObjectID(NFGUID xID)
	{
		mxID = xID;
		ObjectID = mxID.ToString ();
	}

    public Transform GetChildObject(string strChild)
    {
        if (strChild.Length > 0 && xChildList.ContainsKey(strChild))
        {
            return xChildList[strChild];
        }

        return null;
    }

	void AddChild(Transform x)
	{
		for (int i = 0; i < x.childCount; ++i)
		{
			Transform xChildVar = x.GetChild (i);
			AddChild(xChildVar);
		}

		xChildList[x.name] = x;
	}

	void Awake()
	{
		AddChild (this.transform);

        if (xHeadPoint == null)
        {
            GameObject tGO = new GameObject();
            tGO.transform.SetParent(this.transform);
            tGO.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 2.5f, this.transform.position.z);
           
            this.xHeadPoint = tGO.transform;
        }

        if (xBeHitPoint == null)
        {
            if (xHeadPoint)
            {
                GameObject tGO = new GameObject();
                tGO.transform.SetParent(this.transform);
                tGO.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + xHeadPoint.position.y / 2f, this.transform.position.z);

                this.xBeHitPoint = tGO.transform;
            }
            else
            {
                GameObject tGO = new GameObject();
                tGO.transform.SetParent(this.transform);
                tGO.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);

                this.xBeHitPoint = tGO.transform;
            }
        }

        if (xShadow)
        {
            xShadow.transform.localPosition = new Vector3(0, 0.1f, 0);
        }
	}

    private void Update()
    {
        if (xRenderBase != null)
        {
            xRenderBase.gameObject.SetActive(xRenderObject.gameObject.activeSelf);
        }

        if (xShadow != null && xShadow.gameObject.activeSelf == true && xRenderObject != null)
        {
            xShadow.gameObject.SetActive(xRenderObject.gameObject.activeSelf);
        }
    }

    public void LookAt(Vector3  v)
    {
        if (xRenderObject != null)
        {
            Vector3 vector = new Vector3(v.x, xRenderObject.position.y ,v.z);
            xRenderObject.LookAt(vector);
        }
        else
        {
            Vector3 vector = new Vector3(v.x, this.transform.position.y, v.z);
            //this.transform.LookAt(vector);
        }
    }

    public void SetShadowVisible(bool b)
    {
        if (xShadow != null)
        {
            //当显示的时候，应该慢慢显示出来
            xShadow.gameObject.SetActive(b);
        }
    }
}
