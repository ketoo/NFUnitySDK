using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;
using ECM.Controllers;
using ECM.Common;
using ECM.Components;

public sealed class NFHeroMotor : BaseCharacterController
{
    private NFIKernelModule mKernelModule;
    private NFSceneModule mSceneModule;
    private NFLoginModule mLoginModule;
    private NFNetModule mNetModule;
    private NFUIModule mUIModule;

    private NFAnimaStateMachine mAnimaStateMachine;
    private NFAnimatStateController mAnima;
    private NFBodyIdent mBodyIdent;
    private NFGUID mxGUID;

    //private float mfInterval = 0.05f;
    //private float mfLastTime = 0f;
    //private Vector3 mvLastJoyDirect = new Vector3();

    public Vector3 moveToPos = Vector3.zero;

    private NFHeroInput mHeroInput;
    private NFHeroSync mHeroSync;

    //=============
    #region EDITOR EXPOSED FIELDS

    [Tooltip("The character's walk speed.")]
    [SerializeField]
    private float _walkSpeed = 1.5f;

    [Tooltip("The character's run speed.")]
    [SerializeField]
    private float _runSpeed = 3.5f;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The character's walk speed.
    /// </summary>

    public float walkSpeed
    {
        get { return _walkSpeed; }
        set { _walkSpeed = Mathf.Max(0.0f, value); }
    }

    /// <summary>
    /// The character's run speed.
    /// </summary>

    public float runSpeed
    {
        get { return _runSpeed; }
        set { _runSpeed = Mathf.Max(0.0f, value); }
    }

    /// <summary>
    /// Walk input command.
    /// </summary>

    public bool walk { get; private set; }

    #endregion

    #region METHODS

    /// <summary>
    /// Get target speed based on character state (eg: running, walking, etc).
    /// </summary>

    private float GetTargetSpeed()
    {
        return walk ? walkSpeed : runSpeed;
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' CalcDesiredVelocity method to handle different speeds,
    /// eg: running, walking, etc.
    /// </summary>

    protected override Vector3 CalcDesiredVelocity()
    {
        // Set 'BaseCharacterController' speed property based on this character state

        speed = GetTargetSpeed();

        // Return desired velocity vectormovem

        return base.CalcDesiredVelocity();
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' Animate method.
    /// 
    /// This shows how to handle your characters' animation states using the Animate method.
    /// The use of this method is optional, for example you can use a separate script to manage your
    /// animations completely separate of movement controller.
    /// 
    /// </summary>

    protected override void Animate()
    {
        // If no animator, return

        if (animator == null)
            return;
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' HandleInput,
    /// to perform custom controller input.
    /// </summary>
    /// 
    public void ProcessInput(bool left, bool right, bool top)
    {
        if (mLoginModule.mRoleID != mxGUID)
        {
            return;
        }

        //moveDirection = v;

        //jump = top;
    }

    public Vector3 GetMoveDrictor()
    {
        return moveDirection;
    }

    public Vector3 GetMovePos()
    {
        return moveToPos;
    }

    public void Stop()
    {
        moveDirection = Vector3.zero;
        moveToPos = Vector3.zero;
        mAnima.PlayAnimaState(NFAnimaStateType.Idle, -1);
    }

    public void MoveToAttackTarget(Vector3 vPos, NFGUID id)
    {
        moveToPos = vPos;
        moveDirection = (vPos - this.transform.position).normalized;
        mBodyIdent.LookAt(vPos);
    }

    public void MoveTo(Vector3 vPos)
    {
        moveToPos = vPos;
        moveDirection = (vPos - this.transform.position).normalized;
        mBodyIdent.LookAt(vPos);


        if (mLoginModule.mRoleID == mxGUID)
        {
            mHeroSync.SendSyncMessage();
        }

        mAnima.PlayAnimaState(NFAnimaStateType.Run, -1);
    }

    public void MoveToImmune(Vector3 vPos, float fTime, bool bFaceToPos = true)
    {
        if (bFaceToPos)
        {
            mBodyIdent.LookAt(vPos);
        }

        moveToPos = vPos;
        iTween.MoveTo(this.gameObject, vPos, fTime);
    }

    protected override void HandleInput()
    {
      
    }

    #endregion

    #region MONOBEHAVIOUR

    /// <summary>
    /// Overrides 'BaseCharacterController' OnValidate method,
    /// to perform this class editor exposed fields validation.
    /// </summary>

    public override void OnValidate()
    {
        // Validate 'BaseCharacterController' editor exposed fields

        base.OnValidate();

        // Validate this editor exposed fields

        walkSpeed = _walkSpeed;
        runSpeed = _runSpeed;
    }

    #endregion

    public bool isOnGround
    {
        get { return movement.isOnGround; }
    }

    //=================================================================================================================o
    public override void Awake()
    {
        base.Awake();
        _walkSpeed = 1.5f;

        mKernelModule = NFPluginManager.Instance().FindModule<NFIKernelModule>();

        mSceneModule = NFPluginManager.Instance().FindModule<NFSceneModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mNetModule = NFPluginManager.Instance().FindModule<NFNetModule>();

        mUIModule = NFPluginManager.Instance().FindModule<NFUIModule>();

        mAnima = GetComponent<NFAnimatStateController>();
    }

    public override void Update()
    {
        base.Update();

        if (mLoginModule.mRoleID == mxGUID)
        {
            //NFDrawTool.DrawCircle(this.transform, moveToPos, 0.5f, Color.blue);
        }

        mBodyIdent.SetShadowVisible(isGrounded);

        if (Vector3.Distance(moveToPos, Vector3.zero) < 0.1f)
        {
            Stop();
        }
        else if (Mathf.Abs(moveToPos.x - this.transform.position.x) < 0.1f && Mathf.Abs(moveToPos.z - this.transform.position.z) < 0.1f)
        {
            Stop();
        }
        else
        {
            moveDirection = (moveToPos - this.transform.position).normalized;
            mBodyIdent.LookAt(moveToPos);
        }
    }

    void Start()
    {
        mAnima = GetComponent<NFAnimatStateController>();
        mBodyIdent = GetComponent<NFBodyIdent>();
        mAnimaStateMachine = GetComponent<NFAnimaStateMachine>();
        mHeroInput = GetComponent<NFHeroInput>();
        mHeroSync = GetComponent<NFHeroSync>();

        mxGUID = mBodyIdent.GetObjectID();
        moveDirection = new Vector3();

        mKernelModule.RegisterPropertyCallback(mxGUID, NFrame.Player.MOVE_SPEED, PropertyEventHandler);

    }

    void PropertyEventHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        this.runSpeed = newVar.IntVal() / 100.0f;
    }

    void OnDestroy()
	{

	}

}
