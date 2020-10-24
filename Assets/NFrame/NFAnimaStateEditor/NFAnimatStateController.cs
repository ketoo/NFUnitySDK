using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if DG_Tweening
using DG.Tweening;
#endif

namespace NFrame
{
	public class NFAnimatStateController : MonoBehaviour
	{
		[SerializeField] public NFAnimaStateData mxSkillData;
		private NFBodyIdent mBodyIdent;
		private Animator mAnimator;
		private NFAnimationEvent mxAnimationEvent = new NFAnimationEvent();
		private NFAnimaStateType meLastPlayID = NFAnimaStateType.NONE;

		private List<GameObject> effectList = new List<GameObject>();

        //for buff and debuff
        private float mfMoveSpeed = 1.0f;
        private float mfAttackSpeed = 1.0f;

        public List<GameObject> EnemyList = new List<GameObject>();

		struct BulletTrace
		{
			public GameObject bullet;
			public GameObject target;
			public int Index;
			public int gameID;
			public BulletStruct.MoveType movetype;
		}

		private List<BulletTrace> mxBulletTraceInfo = new List<BulletTrace>();

        public void SetMoveSpeed(float speed)
        {
            mfMoveSpeed = speed;
        }

        public void SetAttackSpeed(float speed)
        {
            mfAttackSpeed = speed;
        }

        public void SetAnimator(Animator animator)
        {
            mAnimator = animator;
        }

        public NFAnimaStateType GetCurState()
		{
			return meLastPlayID;
		}

        public NFAnimationEvent GetAnimationEvent()
		{
			return mxAnimationEvent;
		}

        private void Awake()
        {
            mBodyIdent = GetComponent<NFBodyIdent>();
            if (mBodyIdent.xRenderObject)
            {
                mAnimator = mBodyIdent.xRenderObject.GetComponent<Animator>();
            }
        }

        void Start()
		{
			PlayAnimaState(NFAnimaStateType.Idle, -1);
		}

		public int PlayAnimaState(NFAnimaStateType eAnimaType, int index, bool force = false)
		{
            foreach (GameObject go in effectList)
            {
                if (go != null)
                {
					Destroy(go);
                }
            }

			effectList.Clear();

            if (eAnimaType == NFAnimaStateType.Idle)
			{
				float f = Random.Range(0, 100);
                if (f < 15.0f)
                {
					for (int i = 0; i < mxSkillData.AnimationSkillList.Count; ++i)
					{
						AnimationSkillStruct xAnimationSkillStruct = mxSkillData.AnimationSkillList[i];
					    if (xAnimationSkillStruct.Type == NFAnimaStateType.Idle1)
						{
							if (xAnimationSkillStruct.AnimationClip != null)
							{
								eAnimaType = NFAnimaStateType.Idle1;
								break;
							}
						}
					}
				}
                else if (f < 30.0f)
                {
					for (int i = 0; i < mxSkillData.AnimationSkillList.Count; ++i)
					{
						AnimationSkillStruct xAnimationSkillStruct = mxSkillData.AnimationSkillList[i];
						if (xAnimationSkillStruct.Type == NFAnimaStateType.Idle1)
						{
							if (xAnimationSkillStruct.AnimationClip != null)
							{
								eAnimaType = NFAnimaStateType.Idle2;
								break;
							}
						}
					}
				}
			}
			else
            {
				if (eAnimaType == meLastPlayID && !force)
                {
					return - 1;
                }
			}

			if (mBodyIdent == null)
            {
                mBodyIdent = GetComponent<NFBodyIdent>();
                if (mBodyIdent.xRenderObject)
                {
                    mAnimator = mBodyIdent.xRenderObject.GetComponent<Animator>();
                }
            }

			mxAnimationEvent.OnEndAnimaEvent(this.gameObject, meLastPlayID, index);

			meLastPlayID = eAnimaType;

			mxAnimationEvent.OnStartAnimaEvent(this.gameObject, eAnimaType, index);

			for (int i = 0; i < mxSkillData.AnimationSkillList.Count; ++i)
			{
				AnimationSkillStruct xAnimationSkillStruct = mxSkillData.AnimationSkillList[i];
				if (xAnimationSkillStruct.Type == eAnimaType)
				{
					if (mBodyIdent.xRenderObject)
                    {
						mBodyIdent.xRenderObject.gameObject.SetActive(xAnimationSkillStruct.visible);
					}

					if (xAnimationSkillStruct.AnimationClip != null)
					{
						//mAnimator.Play(eAnimaType.ToString());
						if (mAnimator)
						{
							mAnimator.CrossFade(eAnimaType.ToString(), 0.1f);
						}
					}
					else
					{
                        //continue;
						//Debug.LogWarning(eAnimaType.ToString() + " The AnimationClip is null!");
						//UnityEditor.EditorUtility.DisplayDialog ("Warning", "The AnimationClip is null!", "OK", "Cancel");
					}

					foreach (EffectStruct es in xAnimationSkillStruct.EffectStructList)
					{
						if (es.Effect != null)
						{
                            es.Index = index;
							StartCoroutine(WaitPlayEffect(es));
						}
					}
					foreach (AudioStruct es in xAnimationSkillStruct.AudioStructList)
					{
						if (es.Audio != null)
						{
							es.Index = index;
							StartCoroutine(WaitPlayAudio(es));
						}
					}

					foreach (BulletStruct es in xAnimationSkillStruct.BulletStructList)
					{
						if (es.Bullet != null)
						{
							es.Index = index;
							StartCoroutine(WaitPlayBullet(es));
						}
					}

					foreach (MovementStruct es in xAnimationSkillStruct.MovementStructList)
					{
						es.Index = index;
						StartCoroutine(WaitPlayMovement(es));
					}

					if (xAnimationSkillStruct.ActiveStructList.Count > 0)
					{
						foreach (ActiveStruct es in xAnimationSkillStruct.ActiveStructList)
						{
							es.Index = index;
							StartCoroutine(WaitPlayActive(es));
						}
					}

					if (xAnimationSkillStruct.DamageStructList.Count > 0)
                    {
                        foreach (DamageStruct es in xAnimationSkillStruct.DamageStructList)
                        {
                            es.Index = index;
                            StartCoroutine(WaitPlayDamage(es));
                        }
                    }
                    else
                    {
                        // no bullet
                        if (xAnimationSkillStruct.BulletStructList.Count <= 0)
                        {
                            for (int j = 0; j < EnemyList.Count; ++j)
                            {
                                if (EnemyList[j] != null)
                                {
                                    mxAnimationEvent.NoDamageEvent(this.gameObject, EnemyList[j], eAnimaType, index);
                                }
                            }
                        }
                    }

					foreach (CameraStruct es in xAnimationSkillStruct.CameraStructList)
					{
						es.Index = index;
						StartCoroutine(WaitPlayCamera(es));
					}

					if (xAnimationSkillStruct.Type != NFAnimaStateType.Idle)
					{
						if (xAnimationSkillStruct.AnimationClip)
						{
							if (!xAnimationSkillStruct.AnimationClip.isLooping)
							{
								StartCoroutine(WaitPlayNextAnim(xAnimationSkillStruct.fTime, xAnimationSkillStruct.NextType, -1));
							}
						}
						else if(xAnimationSkillStruct.Type != xAnimationSkillStruct.NextType)
						{
							StartCoroutine(WaitPlayNextAnim(xAnimationSkillStruct.fTime, xAnimationSkillStruct.NextType, -1));
						}
					}

					//get time
					if (eAnimaType != NFAnimaStateType.Idle)
					{
					}

                    break;
				}
			}

			return index;
		}

		IEnumerator WaitPlayNextAnim(float time, NFAnimaStateType nextType, int index)
		{
			yield return new WaitForSeconds(time);

            PlayAnimaState(nextType, index);
        }

		IEnumerator WaitPlayEffect(EffectStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			Vector3 _pos = Vector3.zero;
			Quaternion _rotation = es.Effect.transform.rotation;
			Transform _parent = null;

            if (es.IsFollow)
			{
				_parent = mBodyIdent.xRenderObject;
				if (es.IsFollowRoot)
                {
					_parent = mBodyIdent.gameObject.transform;
				}
            }
            

            if (es.VirtualPointName != "None")
			{
				Transform _targetTrans = FindTransform(es.VirtualPointName);
				if (_targetTrans)
				{
					_pos = _targetTrans.position + es.Offset;

                    if (es.IsFollow && !es.IsFollowRoot)
                        _parent = _targetTrans;

                }
                else
				{
					Debug.LogError("The specified virtual point can not be found: " + es.VirtualPointName);
				}
			}
			else
			{
                if (!es.IsFollow)
                {
					_pos = transform.position + es.Offset;
				}
			}

			_rotation.eulerAngles += es.Rotate;
			//_rotation.eulerAngles += mBodyIdent.xRenderObject.transform.forward;

			//GameObject _eff = GameObject.Instantiate<GameObject>(es.Effect, _pos, _rotation, _parent);

            GameObject _eff = GameObject.Instantiate<GameObject>(es.Effect);
			_eff.transform.parent = _parent;
			_eff.transform.position = _pos;
			if (es.IsFollow)
			{
				_eff.transform.localPosition = es.Offset;
			}

			_eff.transform.localRotation = _rotation;

			_eff.SetActive(true);

			effectList.Add(_eff);

			Destroy(_eff, es.LifeTime);
		}

		IEnumerator WaitMoveBulletToLine(BulletStruct es, GameObject bullet, Vector3 v)
		{
			BulletTrace xBulletTrace = new BulletTrace();
			xBulletTrace.target = null;
			xBulletTrace.bullet = bullet;
			xBulletTrace.Index = es.Index;
			xBulletTrace.gameID = this.gameObject.GetInstanceID();
			xBulletTrace.movetype = es.moveType;
			mxBulletTraceInfo.Add(xBulletTrace);

			Vector3 vForward = this.gameObject.transform.forward;
			Vector3 vTargetPos = vForward * es.Distance + bullet.transform.position;
			if (v != Vector3.zero)
			{
				vTargetPos = v;
			}
#if DG_Tweening
			float fDis = Vector3.Distance(vTargetPos, bullet.transform.position);
			Tweener t = bullet.transform.DOMove(vTargetPos, fDis / es.Speed);
            t.SetEase(Ease.Linear);
            t.OnComplete(delegate ()
		    {
			   int arrayIndex = -1;

			   for (int i = 0; i < mxBulletTraceInfo.Count; ++i)
			   {
				   if (mxBulletTraceInfo[i].bullet.GetInstanceID() == bullet.GetInstanceID())
				   {
					   arrayIndex = i;
					   break;
				   }
			   }

			   mxAnimationEvent.OnBulletTouchPositionEvent(this.gameObject, vTargetPos, es.AnimationType, mxBulletTraceInfo[arrayIndex].Index);

			   mxBulletTraceInfo.RemoveAt(arrayIndex);


			   Destroy(bullet);

			   if (es.TouchEffect)
			   {
				   GameObject _startEff = GameObject.Instantiate<GameObject>(es.TouchEffect, vTargetPos, Quaternion.identity);
				   _startEff.SetActive(true);
				   Destroy(_startEff, es.TouchEffLifeTime);
			   }
			   if (es.TouchAudio)
			   {
				   AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.TouchAudio, vTargetPos, Quaternion.identity);
				   AudioSource.PlayClipAtPoint(_startEff, transform.position);
				   Destroy(_startEff, es.TouchEffLifeTime);
			   }
		    });
#endif
			yield return new WaitForEndOfFrame();
		}

        void OnBulletTouchTargetComplete()
        {

        }

		IEnumerator WaitMoveBulletToTarget(BulletStruct es, GameObject bullet, GameObject target)
		{
            if (bullet != null
                && target != null
                && es != null)
            {

				BulletTrace xBulletTrace = new BulletTrace();
				xBulletTrace.target = target;
				xBulletTrace.bullet = bullet;
				xBulletTrace.Index = es.Index;
				xBulletTrace.gameID = this.gameObject.GetInstanceID();
				xBulletTrace.movetype = es.moveType;
				mxBulletTraceInfo.Add(xBulletTrace);
         
				Vector3 vTargetPos = target.transform.position;
				float fDis = Vector3.Distance(vTargetPos, bullet.transform.position);
				float fTime = fDis / es.Speed;

#if DG_Tweening
				Tweener tween = bullet.transform.DOMove(vTargetPos, fTime);//.OnComplete(OnBulletTouchTargetComplete);

				float fMinDis = 0.3f;
				while (true)
				{
                    if (target != null && bullet != null)
                    {
                        if (Vector3.Distance(vTargetPos, bullet.transform.position) < fMinDis)
                        {
							{
								int arrayIndex = -1;

								for (int i = 0; i < mxBulletTraceInfo.Count; ++i)
								{
									if (mxBulletTraceInfo[i].bullet.GetInstanceID() == bullet.GetInstanceID())
									{
										arrayIndex = i;
										break;
									}
								}

								mxAnimationEvent.OnBulletTouchTargetEvent(bullet, target, 0, mxBulletTraceInfo[arrayIndex].Index);
								mxBulletTraceInfo.RemoveAt(arrayIndex);
							}

							Destroy(bullet);

							if (es.TouchEffect)
							{
								GameObject _startEff = GameObject.Instantiate<GameObject>(es.TouchEffect, vTargetPos, Quaternion.identity);
								_startEff.SetActive(true);
								Destroy(_startEff, es.TouchEffLifeTime);
							}
							if (es.TouchAudio)
							{
								AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.TouchAudio, vTargetPos, Quaternion.identity);
								AudioSource.PlayClipAtPoint(_startEff, transform.position);
								Destroy(_startEff, es.TouchEffLifeTime);
							}
						}
                        else
                        {
                            if (vTargetPos != target.transform.position)
                            {
								//tween.ChangeEndValue(target.transform.position, true);
							}
						}
					}
                    else
                    {
						break;
                    }

					yield return new WaitForEndOfFrame();
				}
#endif
			}

			yield return null;
		}

		IEnumerator WaitPlayAudio(AudioStruct es)
		{
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			if (es.Audio != null)
			{
				AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.Audio, this.transform.position, Quaternion.identity);
				AudioSource.PlayClipAtPoint(_startEff, transform.position);
				Destroy(_startEff, es.LifeTime);
			}

			yield return new WaitForEndOfFrame();
		}

		IEnumerator WaitPlayCamera(CameraStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			if (Camera.main != null)
			{

#if DG_Tweening
				Camera.main.transform.DOShakePosition(es.ShakeTime, es.Strength);
#endif
			}

			yield return new WaitForEndOfFrame();

		}

		IEnumerator WaitPlayActive(ActiveStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			es.gameObject.SetActive(es.isActive);
			yield return new WaitForEndOfFrame();

		}

		IEnumerator WaitPlayDamage(DamageStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			//if you have bullet, then you can not use damage event because bullet will triggers the callback function
            for (int i = 0; i < EnemyList.Count; ++i)
			{
				if (EnemyList[i] != null)
				{
					mxAnimationEvent.OnDamageEvent(this.gameObject, EnemyList[i], es.AnimationType, es.Index);
				}
			}

			yield return new WaitForEndOfFrame();

		}

		IEnumerator WaitPlayMovement(MovementStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;

			if (es.StartEffect)
			{
				GameObject _startEff = GameObject.Instantiate<GameObject>(es.StartEffect, this.gameObject.transform.position, Quaternion.identity);
				_startEff.SetActive(true);
				Destroy(_startEff, es.StartEffLifeTime);
			}
			if (es.StartAudio)
			{
				AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.StartAudio, this.gameObject.transform.position, Quaternion.identity);
				AudioSource.PlayClipAtPoint(_startEff, transform.position);

				Destroy(_startEff, es.StartEffLifeTime);
			}

			Vector3 vForward = new Vector3();
			Vector3 vTargetPos = new Vector3();
			switch (es.moveType)
			{
				case MovementStruct.MoveType.Forward:
					{
						vForward = this.gameObject.transform.forward;
					}
					break;
				case MovementStruct.MoveType.Back:
					{
						vForward = -this.gameObject.transform.forward;
					}
					break;
				case MovementStruct.MoveType.Left:
					{
						vForward = -this.gameObject.transform.right;
					}
					break;
				case MovementStruct.MoveType.Right:
					{
						vForward = this.gameObject.transform.right;
					}
					break;
				default:
					break;
			}

			vTargetPos = vForward * es.Distance + this.transform.position;

			float fDis = Vector3.Distance(vTargetPos, this.transform.position);
#if DG_Tweening
			Tweener t = this.transform.DOMove(vTargetPos, fDis / es.Speed);
			t.OnComplete(delegate ()
		   {
			   if (es.TouchEffect)
			   {
				   GameObject _startEff = GameObject.Instantiate<GameObject>(es.TouchEffect, vTargetPos, Quaternion.identity);
				   _startEff.SetActive(true);
				   Destroy(_startEff, es.TouchEffLifeTime);
			   }
			   if (es.TouchAudio)
			   {
				   AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.TouchAudio, vTargetPos, Quaternion.identity);
				   AudioSource.PlayClipAtPoint(_startEff, transform.position);

				   Destroy(_startEff, es.TouchEffLifeTime);
			   }
		   });
#endif
			yield return new WaitForEndOfFrame();

		}

        void PlayBulletToTarget(BulletStruct es, Transform firePoint, Transform target)
        {
            GameObject beHitObject = target.gameObject;
            NFBodyIdent bodyIdent =  target.GetComponent<NFBodyIdent>();
            if (bodyIdent && bodyIdent.xBeHitPoint)
            {
                beHitObject = bodyIdent.xBeHitPoint.gameObject;
            }

            Vector3 pos = firePoint.position;
            GameObject _bullet = GameObject.Instantiate<GameObject>(es.Bullet, pos, Quaternion.identity);
            _bullet.transform.position = firePoint.position;
            _bullet.SetActive(true);
			_bullet.transform.LookAt(target.transform.position);

			Destroy(_bullet, es.StartEffLifeTime);

			if (es.StartEffect)
            {
                GameObject _startEff = GameObject.Instantiate<GameObject>(es.StartEffect, pos, Quaternion.identity);
                _startEff.SetActive(true);
                Destroy(_startEff, es.StartEffLifeTime);
            }
            if (es.StartAudio)
            {
                AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.StartAudio, pos, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_startEff, transform.position);

                Destroy(_startEff, es.StartEffLifeTime);
            }

            StartCoroutine(WaitMoveBulletToTarget(es, _bullet, beHitObject));
        }

        void PlayBulletToLine(BulletStruct es, Transform firePoint, Vector3 v)
        {
            Vector3 pos = firePoint.position;
            GameObject _bullet = GameObject.Instantiate<GameObject>(es.Bullet, pos, Quaternion.identity);
            _bullet.transform.position = firePoint.position;
            _bullet.SetActive(true);
			_bullet.transform.LookAt(v);

			if (es.StartEffect)
            {
                GameObject _startEff = GameObject.Instantiate<GameObject>(es.StartEffect, pos, Quaternion.identity);
                _startEff.SetActive(true);
                Destroy(_startEff, es.StartEffLifeTime);
            }
            if (es.StartAudio)
            {
                AudioClip _startEff = GameObject.Instantiate<AudioClip>(es.StartAudio, pos, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_startEff, transform.position);

                Destroy(_startEff, es.StartEffLifeTime);
            }

            StartCoroutine(WaitMoveBulletToLine(es, _bullet, v));
        }

        IEnumerator WaitPlayBullet(BulletStruct es)
		{
			//如果特效没有启用则不进行播放
			if (es.isEnabled)
				yield return new WaitForSeconds(es.DelayTime);
			else
				yield break;


		
			switch (es.moveType)
			{
				case BulletStruct.MoveType.TargetObject:
					{
                        for (int i = 0; i < EnemyList.Count; ++i)
						{
                            Transform firePoint = mBodyIdent.xFirePoint;
                            if (!firePoint)
                            {
                                firePoint = this.transform;
                                firePoint = mBodyIdent.xHeadPoint;
                                if (!firePoint)
                                {
                                    firePoint = this.transform;
                                }
                                PlayBulletToTarget(es, firePoint, EnemyList[i].transform);
                            }
                            else
                            {
                                if (firePoint.childCount > 0)
                                {
                                    for (int j = 0; j < firePoint.childCount; ++j)
                                    {
                                        Transform child = firePoint.GetChild(j);

                                        PlayBulletToTarget(es, child, EnemyList[i].transform);
                                    }
                                }
                                else
                                {
                                    PlayBulletToTarget(es, firePoint, EnemyList[i].transform);
                                }
                            }
						}
					}
					break;
				case BulletStruct.MoveType.Line:
					{
                        Transform firePoint = mBodyIdent.xFirePoint;
                        if (!firePoint)
                        {
                            firePoint = mBodyIdent.xHeadPoint;
                            if (!firePoint)
                            {
                                firePoint = this.transform;
                            }


							Vector3 v = es.Distance * mBodyIdent.xRenderObject.transform.forward + mBodyIdent.transform.position;
							PlayBulletToLine(es, firePoint, v);
                        }
                        else
						{
							Vector3 v = es.Distance * mBodyIdent.xRenderObject.transform.forward + mBodyIdent.transform.position;

							if (firePoint.childCount > 0)
                            {
                                for (int j = 0; j < firePoint.childCount; ++j)
                                {
                                    Transform child = firePoint.GetChild(j);

                                    PlayBulletToLine(es, child, v);
                                }
                            }
                            else
                            {
                                PlayBulletToLine(es, firePoint, v);
                            }
                        }
					}
					break;
				default:
					break;
			}
		}

		Transform FindTransform(string name)
		{
			foreach (Transform t in GetComponentsInChildren<Transform>(true))
			{
				if (t.name == name)
					return t;
			}
			return null;
		}
    }
}