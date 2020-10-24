using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Animations;
using NFSDK;
using System.IO;

namespace NFrame
{

	[CustomEditor(typeof(NFAnimatStateController))]
	public class NFAnimatStateEditor : Editor
	{
		#region 数据成员
		private Animator mAnimator;
		private NFAnimatStateController mASC;
		private NFAnimaStateData mData;

		private bool isFoldoutVirtualPoint;
		private string virtualPointName;

        //预览专用
        // 当前预览的动画
        private NFAnimaStateType mPlayType = NFAnimaStateType.NONE;
        //手动预览的动画
        private NFAnimaStateType mManulPlayType = NFAnimaStateType.NONE;
        /// 上一次系统时间
        private float m_PreviousTime;
        /// 说动控制的进度时间
        private float m_CurrentTime;

        #endregion



        #region 界面重写
        /// <summary>
        /// 界面重写
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mASC = target as NFAnimatStateController;
            if (mASC.GetComponent<NFBodyIdent>().xRenderObject)
            {
                mAnimator = mASC.GetComponent<NFBodyIdent>().xRenderObject.GetComponent<Animator>();
            }
            mData = mASC.mxSkillData;

            if (mData)
            {
                if (GUILayout.Button("Fix All anims"))
                {
                    FixAllAnims();
                }

                mData.DefaultAnimationType = (NFAnimaStateType)EditorGUILayout.EnumPopup("DefaultAnima:", mData.DefaultAnimationType);


                DrawVirtualPoint();
                DrawAnimation();
            }
            else
            {
                if (GUILayout.Button("Fix All anims"))
                {
                    String strPath = AssetDatabase.GetAssetPath(mAnimator.avatar);

                    mData = ScriptableObject.CreateInstance<NFAnimaStateData>();
                    UnityEditor.AssetDatabase.CreateAsset(mData, strPath + ".asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();

                    mASC.mxSkillData = UnityEditor.AssetDatabase.LoadAssetAtPath<NFAnimaStateData>(strPath + ".asset");
                }


                EditorGUILayout.HelpBox("Please specify a \"AnimationSkillData\" data .", MessageType.Error);
            }

            EditorUtility.SetDirty(mASC);
            if (mData != null)
                EditorUtility.SetDirty(mData);


            if (GUILayout.Button("Fix All particles"))
            {
                FixParticles(mAnimator.avatar.name.Substring(0, 7));
            }
        }
		#endregion

		#region 虚拟体
		void DrawVirtualPoint()
		{
			isFoldoutVirtualPoint = EditorGUILayout.Foldout(isFoldoutVirtualPoint, "Virtual Point");
			if (!isFoldoutVirtualPoint)
			{
				EditorGUILayout.HelpBox("Add Your Custom Virtual Point Filter...", MessageType.Info);
				return;
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			virtualPointName = EditorGUILayout.TextField("Add Point:", virtualPointName);

			if (GUILayout.Button("Add"))
			{
				AddVirtualPoint(virtualPointName);
			}
			EditorGUILayout.EndHorizontal();

			DrawVirtualPointList();
			EditorGUILayout.EndVertical();

		}

		void AddVirtualPoint(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				if (!mData.VirtualPointList.Contains(name))
					mData.VirtualPointList.Add(name);
			}
		}

		void DrawVirtualPointList()
		{
			for (int i = 0; i < mData.VirtualPointList.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("ID: " + i, "name: " + mData.VirtualPointList[i]);
				if (i > 0)
					if (GUILayout.Button("Delete"))
						mData.VirtualPointList.Remove(mData.VirtualPointList[i]);
				EditorGUILayout.EndHorizontal();
			}
		}
        #endregion

        void OnEnable()
        {
            m_PreviousTime = (float)EditorApplication.timeSinceStartup;
            EditorApplication.update += InspectorUpdate;
        }

        void OnDisable()
        {
            EditorApplication.update -= InspectorUpdate;
        }

        private void InspectorUpdate()
        {
            float delta = (float)EditorApplication.timeSinceStartup - m_PreviousTime;
            m_PreviousTime = (float)EditorApplication.timeSinceStartup;
            if (!Application.isPlaying)
            {
                update(delta);
            }
        }

        void FixAllAnims()
		{
            //remove all animation event


			foreach (NFAnimaStateType myCode in Enum.GetValues(typeof(NFAnimaStateType)))
			{
				bool b = false;
				for (int i = 0; i < mData.AnimationSkillList.Count; i++)
				{
					if (mData.AnimationSkillList[i].Type == myCode)
					{
						b = true;
                        break;
					}
				}

				if (!b)
				{
					AnimationSkillStruct _ass = new AnimationSkillStruct();
					_ass.Type = myCode;
					_ass.IsFoldout = true;

					_ass.EffectStructList = new List<EffectStruct>();
					_ass.AudioStructList = new List<AudioStruct>();
					_ass.BulletStructList = new List<BulletStruct>();
					_ass.DamageStructList = new List<DamageStruct>();
					_ass.ActiveStructList = new List<ActiveStruct>();
					_ass.MovementStructList = new List<MovementStruct>();
					_ass.CameraStructList = new List<CameraStruct>();
					mData.AnimationSkillList.Add(_ass);
				}
			}

			for (int i = mData.AnimationSkillList.Count - 1; i >= 0; i--)
			{
				bool b = false;
				foreach (NFAnimaStateType myCode in Enum.GetValues(typeof(NFAnimaStateType)))
				{
					if (mData.AnimationSkillList[i].Type == myCode)
					{
                        b = true;
                        break;
					}
				}

				if (!b)
				{
					mData.AnimationSkillList.RemoveAt(i);
				}
			}

            AnimatorController ctrl;
            if (mAnimator.runtimeAnimatorController == null)
            {
                //根据最新信息重建新的controller
                String strPath = AssetDatabase.GetAssetPath(mAnimator.avatar);
                ctrl = AnimatorController.CreateAnimatorControllerAtPath(strPath + ".controller");
                mAnimator.runtimeAnimatorController = ctrl;
                AnimatorController.SetAnimatorController(mAnimator, ctrl);
            }
            else
            {
                ctrl = (AnimatorController)mAnimator.runtimeAnimatorController;
            }

            AnimatorStateMachine state_machine = ctrl.layers[0].stateMachine;
            for (int i = 0; i < mData.AnimationSkillList.Count; i++)
            {
                AnimationSkillStruct _ass = mData.AnimationSkillList[i];
                String strClipPath = AssetDatabase.GetAssetPath(_ass.AnimationClip);
                AnimationClip anim = AssetDatabase.LoadAssetAtPath(strClipPath, typeof(AnimationClip)) as AnimationClip;

				//Debug.Log(_ass.AnimationClip.name + " " + strClipPath);
				var objs = AssetDatabase.LoadAllAssetsAtPath(strClipPath);
                foreach (var o in objs)
                {
                    if (o is AnimationClip && _ass.AnimationClip.name == o.name)
                    {
                        Debug.Log(o.name + " is clip " + _ass.AnimationClip.name);
                        anim = o as AnimationClip;
                    }
                }

				
				if (anim == null)
                {
					//didn't set a animation we will set idle as the default animation
					//we don't need to do this as if we do like this there will run out a bug (cann't swap the same animation)
					/*
                    for (int index = 0; index < mData.AnimationSkillList.Count; index++)
                    {
                        AnimationSkillStruct tempAss = mData.AnimationSkillList[index];
                        if (tempAss.Type == NFAnimaStateType.Idle)
                        {
                            _ass.AnimationClip = tempAss.AnimationClip;
                            break;
                        }
                    }
                    */
				}
                else
                {
                    //clear all animation events from model

					List<AnimationEvent> animationEvents = new List<AnimationEvent>();
					UnityEditor.AnimationUtility.SetAnimationEvents(anim, animationEvents.ToArray());
				}

				bool bFind = false;
                for (int j = 0; j < state_machine.states.Length; j ++)
                {
                    ChildAnimatorState childAnimatorState = state_machine.states[j];
                    if (childAnimatorState.state.name == _ass.Type.ToString())
                    {
                        bFind = true;
                        childAnimatorState.state.motion = anim;
                        childAnimatorState.state.speed = _ass.fSpeed;
                        break;
                    }
                }

                if (!bFind)
                {
                    AnimatorState state = state_machine.AddState(_ass.Type.ToString());
                    state.motion = anim;
                    state.speed = _ass.fSpeed;
                }
            }

            /*
			AnimatorState[] intArray2 = new AnimatorState[100];

			foreach (NFAnimaStateType myCode in Enum.GetValues(typeof(NFAnimaStateType)))
			{
				AnimatorState state = state_machine.AddState(myCode.ToString());

                intArray2[(int)myCode] = state;
			}


            AnimatorState runState = intArray2[(int)NFAnimaStateType.Run];
            AnimatorState IdleState = intArray2[(int)NFAnimaStateType.Idle];

            runState.AddTransition(IdleState);
            IdleState.AddTransition(runState);
            */

            string prefabPath = "/Prefabs/Object" + mASC.gameObject.name;
			PrefabUtility.ApplyObjectOverride(mASC.gameObject, prefabPath, InteractionMode.AutomatedAction);
        }

        void DrawAnimation()
        {
            if (mData.AnimationSkillList == null)
                return;

            for (int i = 0; i < mData.AnimationSkillList.Count; i++)
            {
                EditorGUILayout.Space();

                AnimationSkillStruct _ass = mData.AnimationSkillList[i];
                if (_ass.Type == NFAnimaStateType.NONE)
                {
                    continue;
                }

                if (_ass.IsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    _ass.fCurTime = EditorGUILayout.Slider(_ass.Type.ToString(), _ass.fCurTime, 0f, _ass.fTime);
                    if (_ass.fCurTime > 0)
                    {
                        for (int j = 0; j < mData.AnimationSkillList.Count; j++)
                        {
                            if (i != j)
                            {
                                mData.AnimationSkillList[j].fCurTime = 0f;
                            }
                        }

                        m_CurrentTime = _ass.fCurTime / _ass.fTime;
                        mManulPlayType = _ass.Type;
                    }

					EditorGUILayout.BeginHorizontal();
					_ass.fSpeed = EditorGUILayout.FloatField("Play Speed:", _ass.fSpeed);
                    _ass.visible = EditorGUILayout.Toggle("Visible:", _ass.visible);

					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Play"))
                    {
                        //mASC.SetAnimator(mASC.GetComponent<Animator>());
                        _ass.fCurTime = 0f;
                        m_CurrentTime = 0f;
                        mManulPlayType = NFAnimaStateType.NONE;
                        mPlayType = _ass.Type;
                        mASC.PlayAnimaState(mPlayType, -1);
                    }

                    if (GUILayout.Button("Stop"))
                    {
                        _ass.fCurTime = 0f;
                        m_CurrentTime = 0f;
                        mManulPlayType = NFAnimaStateType.NONE;
                        mPlayType = NFAnimaStateType.Idle;
                        mASC.PlayAnimaState(NFAnimaStateType.Idle, -1);

                        for (int j = 0; j < mData.AnimationSkillList.Count; j++)
                        {
                            mData.AnimationSkillList[j].fCurTime = 0f;
                        }
                    }

					if (GUILayout.Button("FIX ANIM"))
					{
						FixAllAnims();
					}

					EditorGUILayout.EndHorizontal();

                    _ass.AnimationClip = (AnimationClip)EditorGUILayout.ObjectField("AnimaitonClip:", _ass.AnimationClip, typeof(AnimationClip), true);

                    EditorGUILayout.BeginHorizontal();
                    if (_ass.AnimationClip == null)
                    {
                        _ass.fTime = 1f;
                    }
                    else
                    {
                        _ass.fTime = _ass.AnimationClip.length / _ass.fSpeed;
                    }
                    EditorGUILayout.LabelField("NextAnima After:", _ass.fTime.ToString());
                    _ass.NextType = (NFAnimaStateType)EditorGUILayout.EnumPopup(_ass.NextType);

                    EditorGUILayout.EndHorizontal();



                    mData.AnimationSkillList[i] = _ass;

                    //添加特效与删除片断
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("ADD EFFECT"))
                    {
                        EffectStruct _es = new EffectStruct();
                        _es.LifeTime = 3;
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].EffectStructList.Add(_es);
                    }
                    if (GUILayout.Button("ADD Audio"))
                    {
                        AudioStruct _es = new AudioStruct();
                        _es.LifeTime = 3;
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].AudioStructList.Add(_es);
                    }
                    if (GUILayout.Button("ADD Bullet"))
                    {
                        BulletStruct _es = new BulletStruct();
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].BulletStructList.Add(_es);
                    }



                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("ADD Damage"))
                    {
                        DamageStruct _es = new DamageStruct();
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].DamageStructList.Add(_es);
                    }

                    if (GUILayout.Button("ADD Movement"))
                    {
                        MovementStruct _es = new MovementStruct();
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].MovementStructList.Add(_es);
                    }

                    if (GUILayout.Button("ADD Camera"))
                    {
                        CameraStruct _es = new CameraStruct();
                        _es.isFoldout = true;
                        mData.AnimationSkillList[i].CameraStructList.Add(_es);
                    }
					if (GUILayout.Button("ADD Active"))
					{
						ActiveStruct _es = new ActiveStruct();
						_es.isActive = false;
						mData.AnimationSkillList[i].ActiveStructList.Add(_es);
					}

					EditorGUILayout.EndHorizontal();

                    if (mData.AnimationSkillList.Count > 0)
                    {
                        DrawEffect(i);
                        DrawAudio(i);
                        DrawMovement(i);
                        DrawDamage(i);
                        DrawBullet(i);
                        DrawCamera(i);
                        DrawActive(i);
					}
                    EditorGUILayout.EndVertical();
                }
            }
        }


        void DrawEffect(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].EffectStructList.Count; i++)
			{
				EffectStruct _eff = mData.AnimationSkillList[id].EffectStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = _eff.Effect ? _eff.Effect.name : "Effect" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].EffectStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].EffectStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.Effect = (GameObject)EditorGUILayout.ObjectField("Effect", _eff.Effect, typeof(GameObject), true);

					string[] _nameArry = mData.VirtualPointList.ToArray();
					_eff.VirtualPointID = EditorGUILayout.Popup("Virtual Point", _eff.VirtualPointID, _nameArry);
					_eff.VirtualPointName = _nameArry[_eff.VirtualPointID];

					_eff.Offset = EditorGUILayout.Vector3Field("Offset", _eff.Offset);
					_eff.Rotate = EditorGUILayout.Vector3Field("Rotate", _eff.Rotate);
                    if (_eff.IsFollow)
                    {
						EditorGUILayout.BeginHorizontal();
						_eff.IsFollow = EditorGUILayout.Toggle("Is Follow", _eff.IsFollow);
						_eff.IsFollowRoot = EditorGUILayout.Toggle("Is FollowRoot", _eff.IsFollowRoot);
						EditorGUILayout.EndHorizontal();
					}
                    else
                    {
						_eff.IsFollow = EditorGUILayout.Toggle("Is Follow", _eff.IsFollow);
					}

					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					_eff.LifeTime = EditorGUILayout.FloatField("Life Time", _eff.LifeTime);

					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					mData.AnimationSkillList[id].EffectStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}

		void DrawAudio(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].AudioStructList.Count; i++)
			{
				AudioStruct _eff = mData.AnimationSkillList[id].AudioStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = _eff.Audio ? _eff.Audio.name : "Audio" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].AudioStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].AudioStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.Audio = (AudioClip)EditorGUILayout.ObjectField("Audio", _eff.Audio, typeof(AudioClip), true);

					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					_eff.LifeTime = EditorGUILayout.FloatField("Life Time", _eff.LifeTime);

					mData.AnimationSkillList[id].AudioStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}

		void DrawMovement(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].MovementStructList.Count; i++)
			{
				MovementStruct _eff = mData.AnimationSkillList[id].MovementStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = "Movement" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].MovementStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].MovementStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					string[] _nameArry = mData.VirtualPointList.ToArray();

					_eff.StartAudio = (AudioClip)EditorGUILayout.ObjectField("StartAudio", _eff.StartAudio, typeof(AudioClip), true);
					_eff.StartEffect = (GameObject)EditorGUILayout.ObjectField("StartEffect", _eff.StartEffect, typeof(GameObject), true);
					_eff.StartEffLifeTime = EditorGUILayout.FloatField("StartEffLifeTime", _eff.StartEffLifeTime);

					//运动方式
					_eff.moveType = (MovementStruct.MoveType)EditorGUILayout.EnumPopup("Move Type", _eff.moveType);
					_eff.Distance = EditorGUILayout.FloatField("Distance", _eff.Distance);
					_eff.Speed = EditorGUILayout.FloatField("Speed", _eff.Speed);

					_eff.TouchAudio = (AudioClip)EditorGUILayout.ObjectField("TouchAudio", _eff.TouchAudio, typeof(AudioClip), true);
					_eff.TouchEffect = (GameObject)EditorGUILayout.ObjectField("TouchEffect", _eff.TouchEffect, typeof(GameObject), true);

					_eff.TouchEffLifeTime = EditorGUILayout.FloatField("TouchEffLifeTime", _eff.TouchEffLifeTime);

					mData.AnimationSkillList[id].MovementStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}

		void DrawBullet(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].BulletStructList.Count; i++)
			{
				BulletStruct _eff = mData.AnimationSkillList[id].BulletStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = _eff.Bullet ? _eff.Bullet.name : "Bullet" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].BulletStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].BulletStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.Bullet = (GameObject)EditorGUILayout.ObjectField("Bullet", _eff.Bullet, typeof(GameObject), true);
					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					string[] _nameArry = mData.VirtualPointList.ToArray();
				    //_eff.FirePointID = EditorGUILayout.Popup("Fire Point", _eff.FirePointID, _nameArry);
					//_eff.FirePointName = _nameArry[_eff.FirePointID];
                    //_eff.FirePoint = (Transform)EditorGUILayout.ObjectField("FirePoint", _eff.FirePoint, typeof(Transform), true);
                    //_eff.Offset = EditorGUILayout.Vector3Field("Fire Offset", _eff.Offset);

					_eff.StartAudio = (AudioClip)EditorGUILayout.ObjectField("StartAudio", _eff.StartAudio, typeof(AudioClip), true);
					_eff.StartEffect = (GameObject)EditorGUILayout.ObjectField("StartEffect", _eff.StartEffect, typeof(GameObject), true);
					_eff.StartEffLifeTime = EditorGUILayout.FloatField("StartEffLifeTime", _eff.StartEffLifeTime);

					//特效运动方式
					_eff.moveType = (BulletStruct.MoveType)EditorGUILayout.EnumPopup("Move Type", _eff.moveType);
					switch (_eff.moveType)
					{
						case BulletStruct.MoveType.Line:
							{
								_eff.Distance = EditorGUILayout.FloatField("Distance", _eff.Distance);
                            }
							break;
						case BulletStruct.MoveType.TargetObject:
							{
							}
							break;
					}

					_eff.Speed = EditorGUILayout.FloatField("Speed", _eff.Speed);

					_eff.TouchAudio = (AudioClip)EditorGUILayout.ObjectField("TouchAudio", _eff.TouchAudio, typeof(AudioClip), true);
					_eff.TouchEffect = (GameObject)EditorGUILayout.ObjectField("TouchEffect", _eff.TouchEffect, typeof(GameObject), true);

					_eff.TouchEffLifeTime = EditorGUILayout.FloatField("TouchEffLifeTime", _eff.TouchEffLifeTime);

					mData.AnimationSkillList[id].BulletStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}
		void DrawDamage(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].DamageStructList.Count; i++)
			{
				DamageStruct _eff = mData.AnimationSkillList[id].DamageStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = "Damage" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].DamageStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].DamageStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					string[] _nameArry = mData.VirtualPointList.ToArray();

					mData.AnimationSkillList[id].DamageStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}
		void DrawCamera(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].CameraStructList.Count; i++)
			{
				CameraStruct _eff = mData.AnimationSkillList[id].CameraStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = "Camera" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].CameraStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].CameraStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					_eff.ShakeTime = EditorGUILayout.FloatField("Shake Time", _eff.ShakeTime);
					_eff.Strength = EditorGUILayout.FloatField("Strength", _eff.Strength);
					_eff.Vibrato = EditorGUILayout.IntField("Vibrato", _eff.Vibrato);

					string[] _nameArry = mData.VirtualPointList.ToArray();

					mData.AnimationSkillList[id].CameraStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}


		void DrawActive(int id)
		{
			if (!(id < mData.AnimationSkillList.Count))
				return;

			for (int i = 0; i < mData.AnimationSkillList[id].ActiveStructList.Count; i++)
			{
				ActiveStruct _eff = mData.AnimationSkillList[id].ActiveStructList[i];

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				string _titleName = "Active" + (i + 1).ToString();
				EditorGUILayout.BeginHorizontal();
				//此子特效的界面折叠
				_eff.isFoldout = EditorGUILayout.Foldout(_eff.isFoldout, _titleName);
				GUILayout.FlexibleSpace();
				//此子特效是否可用
				_eff.isEnabled = EditorGUILayout.Toggle("", _eff.isEnabled);

				if (GUILayout.Button("DELETE"))
				{
					mData.AnimationSkillList[id].ActiveStructList.Remove(_eff);
					return;
				}

				EditorGUILayout.EndHorizontal();

				mData.AnimationSkillList[id].ActiveStructList[i] = _eff;

				if (_eff.isFoldout)
				{
					EditorGUI.BeginDisabledGroup(!_eff.isEnabled);
					_eff.DelayTime = EditorGUILayout.FloatField("Delay Time", _eff.DelayTime);
					if (_eff.DelayTime > mData.AnimationSkillList[id].fTime)
					{
						_eff.DelayTime = mData.AnimationSkillList[id].fTime;
					}

					_eff.gameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", _eff.gameObject, typeof(GameObject), true);
					_eff.isActive = EditorGUILayout.Toggle("IsActive", _eff.isActive);

					mData.AnimationSkillList[id].ActiveStructList[i] = _eff;
				}
				EditorGUI.EndDisabledGroup();


				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;
			}
		}
		/// <summary>
		/// 预览播放状态下的更新
		/// </summary>
		private void update(float delta)
        {
            if (Application.isPlaying || mAnimator == null)
            {
                return;
            }
            // 设置回放的时间位置
            if (mManulPlayType != NFAnimaStateType.NONE)
            {
                manualUpdate(mManulPlayType, m_CurrentTime);
            }
            else
            {
                if (mPlayType != NFAnimaStateType.NONE)
                {
                    mAnimator.Update(delta);
                }
            }
        }

        /// <summary>
        /// 非预览播放状态下，通过滑杆来播放当前动画帧
        /// </summary>
        private void manualUpdate(NFAnimaStateType stateType, float time)
        {
            if (mAnimator != null && stateType != NFAnimaStateType.NONE)
            {
                mAnimator.PlayInFixedTime(stateType.ToString(), -1, time);
                mAnimator.Update(0f);
            }
        }



        private void FixParticles(string name)
        {

            String strPath = "Assets/Resources/Prefabs/SFXs/Partner/" + name + "/";

            string[] dir = Directory.GetFiles(strPath);
            if (dir.Length > 0)
            {
                string[] sets = new string[dir.Length];
                for (int i = 0; i < dir.Length; i++)
                {
                    string[] split = dir[i].Split("/"[0]);
                    sets[i] = split[split.Length - 1];
                }

                for (int i = 0; i < sets.Length; ++i)
                {
                    string currentSet = sets[i];
                    if (currentSet.IndexOf(".meta", StringComparison.Ordinal) < 0)
                    {
                        string strFileName = currentSet.Substring(0, currentSet.Length - 7);
                        //GameObject tilePrefabs = Resources.Load<GameObject>("Prefabs/SFXs/Partner/" + name + "/" + strFileName);

                        Debug.Log(currentSet);

                        GameObject tilePrefabs = AssetDatabase.LoadAssetAtPath<GameObject>(strPath + currentSet);
                        List<Component> comList = new List<Component>();
                        foreach (Component component in tilePrefabs.GetComponents<Component>())
                        {
                            if (component.name != "Transform"
                                && component != tilePrefabs.transform)
                            {
                                comList.Add(component);
                            }
                        }
                        foreach (Component item in comList)
                        {
                            DestroyImmediate(item, true);
                        }

                        EditorUtility.SetDirty(tilePrefabs.gameObject);
                        string spritePath = AssetDatabase.GetAssetPath(tilePrefabs.gameObject);
                        AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
        }
    }
}