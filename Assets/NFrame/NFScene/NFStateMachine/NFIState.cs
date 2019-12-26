using UnityEngine;
using System.Collections;
using System;
using NFrame;
using NFSDK;

public class NFStateData
{
	public NFStateData()
	{
		vTargetPos = Vector3.zero;
		xMoveDirection = Vector3.zero;
	}

	public void ResetData()
	{
		vTargetPos = Vector3.zero;
		xMoveDirection = Vector3.zero;
		fSpeed = 0.0f;
	}
	public Vector3 vTargetPos;
	public Vector3 xMoveDirection;
	public float fSpeed = 0.0f;
    public NFAnimaStateType nextState = NFAnimaStateType.Idle;
}

public class NFIState
{
	const float PRECISION = 0.001f;

	public NFStateData xStateData = new NFStateData();

	private float mfHeartBeatTime;
	private float mfExitTime;
    private float mfEnterTime;
    private bool mbInput = false;
    private NFAnimaStateType meState;
    protected NFAnimaStateMachine mStateMachine;
    protected NFAnimatStateController mAnimatStateController;
    protected NFHeroMotor mHeroMotor;
    protected NFHeroInput mHeroInput;

    protected GameObject mGameObject;
    protected GameObject mRenderObject;

    public NFIState(GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input)
    {
        mGameObject = gameObject;
        meState = eState;
        mfExitTime = fExitTime;
        mfHeartBeatTime = fHeartBeatTime;
        mStateMachine = xStateMachine;

        mfEnterTime = Time.time;
        mbInput = input;

        mAnimatStateController = gameObject.GetComponent<NFAnimatStateController>();
        mRenderObject = gameObject.GetComponent<NFBodyIdent>().xRenderObject.gameObject;

        mHeroMotor = gameObject.GetComponent<NFHeroMotor>();
        mHeroInput = gameObject.GetComponent<NFHeroInput>();
    }

    public NFAnimaStateType GetState()
    {
        return meState;
    }

    public float GetHeartBeatTime()
    {
        return mfHeartBeatTime;
    }

	public float GetEnterTime()
	{
		return mfEnterTime;
	}

    public float GetExitTime()
    {
        return mfExitTime;
    }

    public GameObject GetObject()
    {
        return mGameObject;
    }

    public GameObject GetRenderObject()
    {
        return mRenderObject;
    }

    public NFAnimaStateMachine GetStateMachine()
    {
        return mStateMachine;
    }

    public virtual void Enter( GameObject gameObject, int index)
	{
        mAnimatStateController.PlayAnimaState(meState, index);
        mHeroInput.SetInputEnable(mbInput);

        //Debug.Log(Time.time + "------- Enter PlayAnimaState " + meState);


        /*
		//Debug.Log("------------" + GetStateMachine().GetMachineType().ToString() + "  " + meState.ToString() + " Enter " + gameObject.transform.position.x + "," + gameObject.transform.position.y);

        mfEnterTime = Time.time;

        //xHeroAnima.SetState(meState);
*/
	}
	public virtual void Enter( GameObject gameObject, NFStateData data)
	{
        /*
		xStateData = data;
		mbEnterData = true;

		Enter (gameObject);
*/
	}

    public virtual void Execute( GameObject gameObject )
	{
        /*
		if(mfExitTime > 0.01f)
        {
            if (Time.time > mfEnterTime + mfExitTime)
            {
            }
        }
        */
	}

	public virtual void Exit( GameObject gameObject )
	{
        /*
		//Debug.Log("------------" + GetStateMachine().GetMachineType().ToString() + "  "  + meState.ToString() + " Exit " + gameObject.transform.position.x + "," + gameObject.transform.position.y);
		mbEnterData = false;
		xStateData.ResetData ();
        */
    }

	public virtual void OnCheckInput(GameObject gameObject)
	{
		
	}

}
