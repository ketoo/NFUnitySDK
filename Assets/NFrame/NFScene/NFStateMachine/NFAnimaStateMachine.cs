using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;

public class NFAnimaStateMachine : MonoBehaviour
{
    private NFIKernelModule mKernelModule;
    private NFIElementModule mElementModule;
    private NFLoginModule mLoginModule;

    private Dictionary<NFAnimaStateType, NFIState> mStateDictionary = new Dictionary<NFAnimaStateType, NFIState>();

    private float mfHeartBeatTime;
    private NFAnimatStateController mAnimatStateController;
    private NFHeroMotor xHeroMotor;
    private NFBodyIdent xBodyIdent;

    private NFAnimaStateType mCurrentState = NFAnimaStateType.NONE;
    private NFAnimaStateType mLastState = NFAnimaStateType.NONE;


    public string curState;
    public string lastState;
  

    public void Awake()
    {
		NFIPluginManager pluginManager = NFRoot.Instance().GetPluginManager();

        xBodyIdent = GetComponent<NFBodyIdent>();

        mKernelModule = pluginManager.FindModule<NFIKernelModule>();
		mElementModule = pluginManager.FindModule<NFIElementModule>();
		mLoginModule = pluginManager.FindModule<NFLoginModule>();

        AddState(NFAnimaStateType.Idle, new NFIdleState(this.gameObject, NFAnimaStateType.Idle, this, 1f, 0f, true));

        AddState(NFAnimaStateType.Run, new NFRunState(this.gameObject, NFAnimaStateType.Run, this, 1f, 0f, true));
        AddState(NFAnimaStateType.Walk, new NFWalkState(this.gameObject, NFAnimaStateType.Walk, this, 1f, 0f, true));
        AddState(NFAnimaStateType.Dizzy, new NFDizzyState(this.gameObject, NFAnimaStateType.Dizzy, this, 1f, 0f));
        AddState(NFAnimaStateType.Freeze, new NFFreezeState(this.gameObject, NFAnimaStateType.Freeze, this, 1f, 0f));
        AddState(NFAnimaStateType.Block, new NFBlockState(this.gameObject, NFAnimaStateType.Block, this, 1f, 0f));
        AddState(NFAnimaStateType.Fall, new NFFallState(this.gameObject, NFAnimaStateType.Fall, this, 1f, 0f));
        AddState(NFAnimaStateType.Dead, new NFDeadState(this.gameObject, NFAnimaStateType.Dead, this, 1f, 0f));
        AddState(NFAnimaStateType.JumpStart, new NFJumpStartState(this.gameObject, NFAnimaStateType.JumpStart, this, 1f, 0f));
        AddState(NFAnimaStateType.Jumping, new NFJumpingState(this.gameObject, NFAnimaStateType.Jumping, this, 1f, 0f));
        AddState(NFAnimaStateType.JumpLand, new NFJumpLandState(this.gameObject, NFAnimaStateType.JumpLand, this, 0.1f, 0.4f));
        AddState(NFAnimaStateType.BeHit1, new NFBeHitState(this.gameObject, NFAnimaStateType.BeHit1, this, 1f, 0f));
        AddState(NFAnimaStateType.BeHit2, new NFBeHitState(this.gameObject, NFAnimaStateType.BeHit2, this, 1f, 0f));
        AddState(NFAnimaStateType.HitFly, new NFHitFlyState(this.gameObject, NFAnimaStateType.HitFly, this, 1f, 0f));
        AddState(NFAnimaStateType.Stun, new NFHitFlyState(this.gameObject, NFAnimaStateType.Stun, this, 1f, 0f));

        AddState(NFAnimaStateType.DashForward, new NFDashForwardState(this.gameObject, NFAnimaStateType.DashForward, this, 1f, 0f));
        AddState(NFAnimaStateType.DashJump, new NFDashForwardState(this.gameObject, NFAnimaStateType.DashJump, this, 1f, 0f));

        AddState(NFAnimaStateType.Buff1, new NFBuff1(this.gameObject, NFAnimaStateType.Buff1, this, 1f, 0f));

        AddState(NFAnimaStateType.NormalSkill1, new NFNormalSkill1(this.gameObject, NFAnimaStateType.NormalSkill1, this, 1f, 0f));
        AddState(NFAnimaStateType.NormalSkill2, new NFNormalSkill2(this.gameObject, NFAnimaStateType.NormalSkill2, this, 1f, 0f));
        AddState(NFAnimaStateType.NormalSkill3, new NFNormalSkill3(this.gameObject, NFAnimaStateType.NormalSkill3, this, 1f, 0f));
        AddState(NFAnimaStateType.NormalSkill4, new NFNormalSkill4(this.gameObject, NFAnimaStateType.NormalSkill4, this, 1f, 0f));
        AddState(NFAnimaStateType.NormalSkill5, new NFNormalSkill5(this.gameObject, NFAnimaStateType.NormalSkill5, this, 1f, 0f));

        AddState(NFAnimaStateType.SpecialSkill1, new NFSpecialSkill1(this.gameObject, NFAnimaStateType.SpecialSkill1, this, 1f, 0f));
        AddState(NFAnimaStateType.SpecialSkill2, new NFSpecialSkill2(this.gameObject, NFAnimaStateType.SpecialSkill2, this, 1f, 0f));
        AddState(NFAnimaStateType.SkillThump, new NFSkillThump(this.gameObject, NFAnimaStateType.SkillThump, this, 1f, 0f));

    }

    void Start()
    {

  
        mAnimatStateController = GetComponent<NFAnimatStateController>();
        xHeroMotor = GetComponent<NFHeroMotor>();

        mAnimatStateController.GetAnimationEvent().AddOnDamageDelegation(OnDamageDelegation);
        mAnimatStateController.GetAnimationEvent().AddOnEndAnimaDelegation(OnEndAnimaDelegation);
        mAnimatStateController.GetAnimationEvent().AddOnStartAnimaDelegation(OnStartAnimaDelegation);
        mAnimatStateController.GetAnimationEvent().AddBulletTouchPosDelegation(OnBulletTouchPositionDelegation);
        mAnimatStateController.GetAnimationEvent().AddBulletTouchTargetDelegation(OnBulletTouchTargetDelegation);

    }

    public void OnStartAnimaDelegation(GameObject self, NFAnimaStateType eAnimaType, int index)
    {
        ChangeState(eAnimaType, index);
    }

    public void OnEndAnimaDelegation(GameObject self, NFAnimaStateType eAnimaType, int index)
    {
        
    }

    public void OnDamageDelegation(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index)
    {
        //float damage = Random.Range(900000f, 1100000f);
        //NFPrefabManager.Instance.textManager.Add(damage.ToStringScientific(), target.transform);
        Debug.Log("show damage --- " + target.ToString() + " " + eAnimaType.ToString() + " " + index.ToString());
    }

    public void OnBulletTouchPositionDelegation(GameObject self, Vector3 position, NFAnimaStateType eAnimaType, int index)
    {
        //show damage
        Debug.Log("show damage --- " + position.ToString() + " " + eAnimaType.ToString() + " " + index.ToString());

    }

    public void OnBulletTouchTargetDelegation(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index)
    {
        //show damage
        Debug.Log("show damage --- " + target.name + " " + eAnimaType.ToString() + " " + index.ToString());

    }

	public void OnStart()
    {
        
    }

    public void Update()
    {
        if (mCurrentState != NFAnimaStateType.NONE)
        {
            mStateDictionary[mCurrentState].Execute(this.gameObject);
            mStateDictionary[mCurrentState].OnCheckInput(this.gameObject);
        }
        else
        {
            ChangeState(NFAnimaStateType.Idle, -1);
        }


        if (Application.isEditor)
        {
            curState = mCurrentState.ToString();
            lastState = mLastState.ToString();
        }
    }

    public void AddState(NFAnimaStateType eState, NFIState xState)
    {
        mStateDictionary[eState] = xState;
    }

    public NFIState GetState(NFAnimaStateType eState)
    {
        return mStateDictionary[eState];
    }

    public NFAnimaStateType LastState()
    {
        return mLastState;
    }

    public NFAnimaStateType CurState()
    {
        return mCurrentState;
    }

    public void ChangeState(NFAnimaStateType eState, int index, NFStateData data = null)
    {
        if (mCurrentState == eState)
        {
            return;
        }
        
        if (mCurrentState != NFAnimaStateType.NONE && mStateDictionary.ContainsKey(mCurrentState))
        {
            mStateDictionary[mCurrentState].Exit(this.gameObject);
        }
  
        if (mStateDictionary.ContainsKey(eState))
        {
            mLastState = mCurrentState;
            mCurrentState = eState;

            mStateDictionary[mCurrentState].xStateData = data;
            mStateDictionary[mCurrentState].Enter(this.gameObject, index);
        }
        else
        {
            Debug.LogError("ChangeState to " + mCurrentState + " from " + mLastState);
        }
    }


    public NFGUID GetGUID()
    {
		return xBodyIdent.GetObjectID();
    }

    public bool IsMainRole()
    {
		if (GetGUID() == mLoginModule.mRoleID)
        {
            return true;
        }

        return false;
    }


    public void OutputStateData(NFAnimaStateType eNewState, float fSpeed, Vector3 vNowPos, Vector3 vTargetPos, Vector3 vMoveDirection)
    {
    }

    public void InputStateData(NFAnimaStateType eNewState, NFAnimaStateType eLastState, float fSpeed, int nTime, Vector3 vNowPos, Vector3 vTargetPos, Vector3 vMoveDirection)
    {
        if (IsMainRole())
        {
            return;
        }

        //Debug.Log ("In SyncData: " + id.ToString() + eNewState.ToString() + " TargetPos: " + vTargetPos.x + "," + vTargetPos.y+ "," + vTargetPos.z );

        NFStateData data = new NFStateData();
        data.vTargetPos = vTargetPos;
        data.fSpeed = fSpeed;
        data.xMoveDirection = vMoveDirection;

    }
}
