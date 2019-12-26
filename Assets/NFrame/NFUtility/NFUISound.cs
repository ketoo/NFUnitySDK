using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NFSDK;

public class NFUISound : MonoBehaviour,IPointerDownHandler
{
	public AudioClip[] audioClip;
	// Use this for initialization

    void Awake()
	{
	}

	void Start () 
	{

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (audioClip.Length > 0)
		{
			int nIndex = Random.Range (0, audioClip.Length);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}