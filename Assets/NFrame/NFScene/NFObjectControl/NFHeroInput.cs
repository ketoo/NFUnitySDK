using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;
using ECM.Controllers;
using ECM.Common;
using ECM.Components;


public class NFHeroInput : MonoBehaviour
{
    private NFUIModule mUIModule;
    private NFLoginModule mLoginModule;
    private NFIKernelModule mKernelModule;

    private NFAnimaStateMachine mStateMachineMng;
    private NFHeroMotor mHeroMotor;
    private NFBodyIdent mBodyIdent;

    private NFUIJoystick mJoystick;


    public bool mbInputEnable = false;


    public void SetInputEnable(bool bEnable)
    {
        mbInputEnable = bEnable;
    }

    void Start()
    {
        mStateMachineMng = GetComponent<NFAnimaStateMachine>();
        mBodyIdent = GetComponent<NFBodyIdent>();
        mHeroMotor = GetComponent<NFHeroMotor>();

        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<NFLoginModule>();

        mKernelModule = NFRoot.Instance().GetPluginManager().FindModule<NFIKernelModule>();

        mKernelModule.RegisterPropertyCallback(mBodyIdent.GetObjectID(), NFrame.Player.MOVE_SPEED, PropertyMoveSpeedHandler);
        mKernelModule.RegisterPropertyCallback(mBodyIdent.GetObjectID(), NFrame.Player.ATK_SPEED, PropertyAttackSpeedHandler);

        mHeroMotor.angularSpeed = 0f;
    }

    public void PropertyMoveSpeedHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        //set the animations's speed
        //run
        //walk
    }

    public void PropertyAttackSpeedHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        //set the animations's speed
        //normally attack1
        //normally attack2
        //normally attack3
        //normally attack4
        //normally attack5
    }

    bool CheckMove()
    {
        //idle
        //jumpland
        //run
        return true;
    }

    void MoveEvent(Vector3 direction)
    {
        if (mLoginModule.mRoleID == mBodyIdent.GetObjectID())
        {
            // Handle your custom input here...
            {
                {
                    if (mKernelModule.QueryPropertyInt(mBodyIdent.GetObjectID(), NFrame.Player.HP) > 0)
                    {

                        //人工ui输入的，需要和摄像机进行校正才是世界坐标
                        // Transform moveDirection vector to be relative to camera view direction
                        if (Camera.main)
                        {
                            if (direction != Vector3.zero)
                            {
                                if (CheckMove())
                                {
                                    Vector3 vDirection = direction.relativeTo(Camera.main.transform);
                                    mHeroMotor.MoveTo(this.transform.position + vDirection.normalized * 2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void JoyOnPointerDownHandler(Vector3 direction)
    {
        MoveEvent(direction);

        fLastEventTime = Time.time;
        fLastEventdirection = direction;
    }

    void JoyOnPointerUpHandler(Vector3 direction)
    {
        fLastEventTime = 0f;
        fLastEventdirection = direction;
    }

    void JoyOnPointerDragHandler(Vector3 direction)
    {
        MoveEvent(direction);

        fLastEventTime = Time.time;
        fLastEventdirection = direction;
    }

    float fLastEventTime = 0f;
    Vector3 fLastEventdirection;
    public void Update()
    {
        if (mJoystick == null)
        {
            mJoystick = mUIModule.GetUI<NFUIJoystick>();

            if (mJoystick)
            {
                mJoystick.SetPointerDownHandler(JoyOnPointerDownHandler);
                mJoystick.SetPointerDragHandler(JoyOnPointerDragHandler);
                mJoystick.SetPointerUpHandler(JoyOnPointerUpHandler);
            }
        }

        if (fLastEventTime > 0f && Time.time > (fLastEventTime + 0.2f))
        {
            fLastEventTime = Time.time;

            MoveEvent(fLastEventdirection);
        }
    }

    void OnDestroy()
    {
    }
}