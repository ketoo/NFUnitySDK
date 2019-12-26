using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NFHideAndScroll : MonoBehaviour {
    public Transform targetObject;
    public Transform targetHidePos;
    public Transform checkMask;
    public Transform backMask;
    public bool InitHide = true;

    private Vector3 initPos;
	// Use this for initialization
	void Start ()
    {
        if (null == targetObject || null == targetHidePos)
        {
            Debug.LogError("error for init object is null");
            return;
        }

		initPos = targetObject.position;
              
        Toggle tgl = this.gameObject.GetComponent<Toggle>();
        tgl.onValueChanged.AddListener(delegate{ this.OnChanged(); });

        if (InitHide)
        {
            tgl.isOn = false;
        }
        else
        {
            tgl.isOn = true;
        }

        Refresh();
    }
	
    void OnChanged()
    {
        Refresh();
    }

    void Refresh()
    {
        Toggle tgl = this.gameObject.GetComponent<Toggle>();
        if (tgl.isOn)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        if (null != checkMask)
        {
            Image xImage = checkMask.GetComponent<Image>();
            if (null != xImage)
            {
                xImage.enabled = false;
            }
        }

        if (null != backMask)
        {
            Image xImage = backMask.GetComponent<Image>();
            if (null != xImage)
            {
                xImage.enabled = true;
            }
        }

        if (targetHidePos.position != targetObject.position)
        {
           iTween.MoveTo(targetObject.gameObject, targetHidePos.position, 0.6f);
        }
    }

    void Show()
    {
        if (null != checkMask)
        {
            Image xImage = checkMask.GetComponent<Image>();
            if (null != xImage)
            {
                xImage.enabled = true;
            }
        }
        if (null != backMask)
        {
            Image xImage = backMask.GetComponent<Image>();
            if (null != xImage)
            {
                xImage.enabled = false;
            }
        }

        if (initPos != targetObject.position)
        {
            iTween.MoveTo(targetObject.gameObject, initPos, 0.6f);
        }
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
