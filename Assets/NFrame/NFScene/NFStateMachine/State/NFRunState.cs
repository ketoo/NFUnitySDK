using UnityEngine;
using System.Collections;
using NFrame;
using NFSDK;

public class NFRunState : NFIState
{

    public NFRunState (GameObject gameObject, NFAnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base (gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
	{

	}
	NFBodyIdent xBodyIdent;
    NFAnimatStateController xHeroAnima;
	NFHeroMotor xHeroMotor;

    private int standCount = 0;
    private Vector3 lastPos = new Vector3();

    public override void Enter (GameObject gameObject, int index)
	{
		xBodyIdent = gameObject.GetComponent<NFBodyIdent> ();
        xHeroAnima = gameObject.GetComponent<NFAnimatStateController> ();
		xHeroMotor = gameObject.GetComponent<NFHeroMotor> ();

		base.Enter (gameObject, index);

        xHeroMotor.speed = xHeroMotor.runSpeed;
        standCount = 0;
    }

	public override void Execute (GameObject gameObject)
	{
		base.Execute (gameObject);


		if (mStateMachine.IsMainRole())
		{
            if (Vector3.Distance(gameObject.transform.position, lastPos) < 0.01f)
            {
                standCount++;
            }
            else
            {
                standCount = 0;
            }

            lastPos = gameObject.transform.position;

            if (standCount > 4)
            {
                xHeroMotor.Stop();
            }
        }
	}

	public override void Exit(GameObject gameObject)
    {

    }

	public override void OnCheckInput(GameObject gameObject)
    {
     
    }
}