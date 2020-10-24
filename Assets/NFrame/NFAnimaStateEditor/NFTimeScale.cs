using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTimeScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private float sliderValue = 1.0f;
    private bool bStopTime = false;
    private void OnGUI()
    {

        GUI.Label(new Rect(0, 0, 200, 20), "Speed:" + sliderValue.ToString());
        sliderValue = GUI.HorizontalSlider(new Rect(80, 0, Screen.width - 80 * 2, 20), sliderValue, 0.0f, 1.0f);
        if (!bStopTime)
        {
            Time.timeScale = sliderValue;
        }
        if (GUI.Button(new Rect(Screen.width - 80, 0, 40, 20), "暂停"))
        {
            if (bStopTime)
            {
                Time.timeScale = sliderValue;
                bStopTime = false;
            }
            else
            {
                Time.timeScale = 0.0f;
                bStopTime = true;
            }
        }
    }
}
