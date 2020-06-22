using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using NFSDK;
using NFrame;

public class NFRecordRowView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public delegate void RowClickEventHandler(NFRecordRowData data);
	public delegate void RowPressDownEventHandler(NFRecordRowData data);
	public delegate void RowPressUpEventHandler(NFRecordRowData data);
    public delegate void RowViewUpdateEventHandler(NFGUID self, string recordName, int nRow, NFRecordRowView view);

	//must working as the col-view manager
	private Dictionary<int, NFRecordColView> colViewList = new Dictionary<int, NFRecordColView>();
	private NFRecordRowData data;
	private NFRecordController controller;
	private List<RowClickEventHandler> eventHandler = new List<RowClickEventHandler>();

	private List<RowPressDownEventHandler> eventDwonHandler = new List<RowPressDownEventHandler>();
	private List<RowPressUpEventHandler> eventUpHandler = new List<RowPressUpEventHandler>();

    public GameObject selectPanel;
	private static GameObject lastSelect;
    //public Text text;

	private NFIKernelModule mkernelModule;
    private NFIClassModule mClassModule;
    private NFIElementModule mElementModule;
    private NFLoginModule mLoginModule;

	// Use this for initialization
	private void Awake()
	{
		mkernelModule = NFRoot.Instance().GetPluginManager().FindModule<NFIKernelModule>();
		mClassModule = NFRoot.Instance().GetPluginManager().FindModule<NFIClassModule>();
		mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();
		mElementModule = NFRoot.Instance().GetPluginManager().FindModule<NFIElementModule>();

	}

    void Start()
    {
    	Button btn = this.gameObject.GetComponent<Button>();
    	if (btn == null)
    	{
    		btn = this.gameObject.AddComponent<Button> ();
    	}
    
    	btn.enabled = true;
    	btn.onClick.AddListener(delegate () { this.OnClick(this.gameObject); });
    }
    
    public void AddColView(int col, NFRecordColView colView)
    {
    	colViewList.Add (col, colView);
    }
    
    public void RemoveAllClickEvent()
    {
    	eventHandler.Clear ();
    }
    
    public void RegisterClickEvent(RowClickEventHandler handler)
    {
    	eventHandler.Add (handler);
    }

    public void RegisterPressDownEvent(RowPressDownEventHandler handler)
    {
        eventDwonHandler.Add(handler);
    }
    public void RegisterPressUpEvent(RowPressUpEventHandler handler)
    {
        eventUpHandler.Add(handler);
    }

    public NFRecordRowData GetData()
    {
        return data;
    }

    public void SetData(NFGUID xGUID, string strRecordName, NFRecordController xController, NFRecordRowData xData)
    {
		data = xData;
		controller = xController;

		if (data != null)
		{
			foreach (KeyValuePair<int, NFRecordColView> entry in colViewList)
			{
			    NFIRecord xRecord = mkernelModule.FindRecord (xGUID, strRecordName);

				entry.Value.Refresh (xGUID, xRecord.QueryRowCol (data.row, entry.Key));
			}

			xController.UpdateEvent (xData.id, xData.recordName, xData.row, this);
		}
    }

    void OnClick(GameObject go)
    {
    	for (int i = 0; i < eventHandler.Count; ++i)
    	{
    		RowClickEventHandler handler = eventHandler [i];
    		handler(data);
    	}
    
    	controller.ClickEvent (data);

    	if (lastSelect != null)
    	{
    		lastSelect.SetActive (false);
    	}
    
    	if (selectPanel != null)
    	{
    		selectPanel.SetActive (true);
    	}
    
    	lastSelect = selectPanel;
    }

    public void OnMouseEnter()
    {
        OnPointerDown(null);
    }

    public void OnMouseExit()
    {
        OnPointerUp(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < eventDwonHandler.Count; ++i)
        {
            RowPressDownEventHandler handler = eventDwonHandler[i];
            handler(data);
        }

        controller.DownEvent(data);

        if (lastSelect != null)
        {
            lastSelect.SetActive(false);
        }

        if (selectPanel != null)
        {
            selectPanel.SetActive(true);
        }

        lastSelect = selectPanel;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        for (int i = 0; i < eventUpHandler.Count; ++i)
        {
            RowPressUpEventHandler handler = eventUpHandler[i];
            handler(data);
        }

        controller.UpEvent(data);
    }
}