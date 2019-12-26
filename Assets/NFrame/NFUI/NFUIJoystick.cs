using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NFSDK;
using NFrame;

public class NFUIJoystick : NFUIDialog
{
    private NFNetModule mNetModule;
    private NFLoginModule mLoginModule;
    private NFUIModule mUIModule;
    private NFHelpModule mHelpModule;
    private NFSceneModule mSceneModule;

    public Vector3 direction;

    public FloatingJoystick variableJoystick;
    //public VariableJoystick variableJoystick;

    public delegate void JoyOnPointerDownHandler(Vector3 direction);
    public delegate void JoyOnPointerUpHandler(Vector3 direction);
    public delegate void JoyOnPointerDragHandler(Vector3 direction);

    private JoyOnPointerDownHandler mOnPointerDownHandler = null;
    private JoyOnPointerUpHandler mOnPointerUpHandler = null;
    private JoyOnPointerDragHandler mOnPointerDragHandler = null;

    private bool bContinuous = false;

    public void SetPointerDownHandler(JoyOnPointerDownHandler handler)
    {
        mOnPointerDownHandler = handler;
    }

    public void SetPointerUpHandler(JoyOnPointerUpHandler handler)
    {
        mOnPointerUpHandler = handler;
    }

    public void SetPointerDragHandler(JoyOnPointerDragHandler handler)
    {
        mOnPointerDragHandler = handler;
    }

    Vector3 NormalizeDirection(Vector3 direction)
    {
        if (direction.x < 0.35 && direction.x > -0.35)
        {
            direction.x = 0f;
        }
        else if (direction.x > 0.85)
        {
            direction.x = 1f;
        }
        else if (direction.x < -0.85)
        {
            direction.x = -1f;
        }
        else if (direction.x < 0)
        {
            direction.x = -0.7f;
        }
        else if (direction.x > 0)
        {
            direction.x = 0.7f;
        }

        if (direction.z < 0.35 && direction.z > -0.35)
        {
            direction.z = 0f;
        }
        else if (direction.z > 0.85)
        {
            direction.z = 1f;
        }
        else if (direction.z < -0.85)
        {
            direction.z = -1f;
        }
        else if (direction.z < 0)
        {
            direction.z = -0.7f;
        }
        else if (direction.z > 0)
        {
            direction.z = 0.7f;
        }

        direction.Normalize();

        if (direction.x < 0.65 && direction.x > -0.65)
        {
            if (direction.z > 0.75)
            {
                direction.x = 0;
                direction.z = 1;
            }
            else if (direction.z < -0.75)
            {
                direction.x = 0;
                direction.z = -1;
            }
        }
        if (direction.z < 0.65 && direction.z > -0.65)
        {
            if (direction.x > 0.75)
            {
                direction.x = 1;
                direction.z = 0;
            }
            else if (direction.x < -0.75)
            {
                direction.x = -1;
                direction.z = 0;
            }
        }

        return direction;
    }

    private void OnPointerDown(Vector3 direction)
    {
        direction = NormalizeDirection(direction);

        mOnPointerDownHandler?.Invoke(direction);

        //Debug.Log("OnPointerDown:" +direction);
    }

    private void OnPointerUp(Vector3 direction)
    {
        direction = NormalizeDirection(direction);

        mOnPointerUpHandler?.Invoke(direction);

        //Debug.Log("OnPointerUp:" + direction);
    }

    static Vector3 lastDirection;
    private void OnDrag(Vector3 direction)
    {
        direction = NormalizeDirection(direction);
       
        if (Vector3.Distance(lastDirection, direction) > 0.1)
        {
            lastDirection = direction;

            mOnPointerDragHandler?.Invoke(direction);

            //Debug.Log("OnDrag:" + direction);
        }
    }

    private void Awake()
    {
        NFIPluginManager xPluginManager = NFPluginManager.Instance();

        mLoginModule = xPluginManager.FindModule<NFLoginModule>();
        mUIModule = xPluginManager.FindModule<NFUIModule>();
        mNetModule = xPluginManager.FindModule<NFNetModule>();
        mHelpModule = xPluginManager.FindModule<NFHelpModule>();
        mSceneModule = xPluginManager.FindModule<NFSceneModule>();
    }

    public override void Init()
    {
    }

    private void Update()
    {
        Vector3 vDirection = Vector3.zero;

        if (this.direction == Vector3.zero)
        {
            if (Input.GetKey(KeyCode.W)
                        || Input.GetKey(KeyCode.S)
                        || Input.GetKey(KeyCode.A)
                        || Input.GetKey(KeyCode.D))
            {

                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        vDirection = new Vector3(-0.5f, 0f, 0.5f);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        vDirection = new Vector3(0.5f, 0f, 0.5f);
                    }
                    else
                    {
                        vDirection = Vector3.forward;
                    }

                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        vDirection = new Vector3(-0.5f, 0f, -0.5f);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        vDirection = new Vector3(0.5f, 0f, -0.5f);
                    }
                    else
                    {
                        vDirection = Vector3.back;
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    vDirection = Vector3.left;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    vDirection = Vector3.right;
                }


                this.direction = vDirection;
                OnPointerDown(this.direction);
            }
            else if (variableJoystick.Direction != Vector2.zero)
            {
                vDirection = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
                vDirection.Normalize();

                if (vDirection != this.direction)
                {
                    this.direction = vDirection;

                    OnPointerDown(this.direction);
                }
            }
        }
        else
        {
            //on moving now
            if (Input.GetKey(KeyCode.W)
                        || Input.GetKey(KeyCode.S)
                        || Input.GetKey(KeyCode.A)
                        || Input.GetKey(KeyCode.D))
            {

                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        vDirection = new Vector3(-0.5f, 0f, 0.5f);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        vDirection = new Vector3(0.5f, 0f, 0.5f);
                    }
                    else
                    {
                        vDirection = Vector3.forward;
                    }

                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        vDirection = new Vector3(-0.5f, 0f, -0.5f);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        vDirection = new Vector3(0.5f, 0f, -0.5f);
                    }
                    else
                    {
                        vDirection = Vector3.back;
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    vDirection = Vector3.left;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    vDirection = Vector3.right;
                }

                if (vDirection != this.direction)
                {
                    this.direction = vDirection;
                    OnDrag(this.direction);
                }
            }
            else if (variableJoystick.Direction != Vector2.zero)
            {
                vDirection = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
                vDirection.Normalize();

                if (vDirection != this.direction)
                {
                    this.direction = vDirection;
                    OnDrag(this.direction);
                }
            }
            else
            {
                OnPointerUp(this.direction);

                this.direction = Vector3.zero;
            }
        }
    }
}
