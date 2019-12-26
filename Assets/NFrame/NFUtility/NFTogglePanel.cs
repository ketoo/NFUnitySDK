using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NFTogglePanel : MonoBehaviour {
    public Text text;
    public string messageHandler;
    public Transform[] xPanelObject;
    public Transform[] xReciveMsgObject;

    public int Index = 0;

    private Toggle[] toggles;
    // Use this for initialization
    void Start ()
    {
        toggles = transform.parent.GetComponentsInChildren<Toggle>();

        if(null == xPanelObject)
        {
            Debug.LogError("error for xPanelObject");
        }

        Toggle tgl = this.gameObject.GetComponent<Toggle>();
        tgl.onValueChanged.AddListener(delegate { this.OnChanged(); });
        OnChanged();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    public void OnChanged()
    {
        Toggle tgl = this.gameObject.GetComponent<Toggle>();

        if (toggles != null)
        {
            for (int i = 0; i < toggles.Length; ++i)
            {
                if (toggles[i])
                {
                    NFTogglePanel togglePanel = toggles[i].GetComponent<NFTogglePanel>();
                    if (togglePanel != null && togglePanel.text != null)
                    {
                        togglePanel.text.fontStyle = FontStyle.Normal;
                    }
                }
            }
        }

        for (int i = 0; i < xPanelObject.Length; ++i)
        {
            if (xPanelObject[i])
            {
                xPanelObject[i].gameObject.SetActive(tgl.isOn);
            }
        }

        if (tgl.isOn)
        {
            if (text != null)
            {
                text.fontStyle = FontStyle.Bold;
            }
            //sendmsg to show your ui
            for (int i = 0; i < xReciveMsgObject.Length; ++i)
            {
                if (xReciveMsgObject[i])
                {
                    if (messageHandler.Length > 0)
                    {
                        xReciveMsgObject[i].gameObject.SendMessage(messageHandler, Index);
                    }
                    else
                    {
                        xReciveMsgObject[i].gameObject.SendMessage("OnToggleChanged", Index);
                    }
                }
            }
        }
    }
}
