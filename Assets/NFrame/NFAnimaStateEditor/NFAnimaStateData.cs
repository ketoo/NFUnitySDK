using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NFrame
{


	public class NFAnimaStateData : ScriptableObject
	{
		public List<string> VirtualPointList = new List<string> { "None" };
		public NFAnimaStateType DefaultAnimationType = NFAnimaStateType.Idle;
		public List<AnimationSkillStruct> AnimationSkillList = new List<AnimationSkillStruct>();
	}

	#region 数据结构
	[System.Serializable]
	public class AnimationSkillStruct
	{
		[HideInInspector] public bool IsFoldout;

		[Tooltip("GUI上显示的名称")]
		public NFAnimaStateType Type;
		public float fTime = 1.0f;
		public NFAnimaStateType NextType = NFAnimaStateType.Idle;

		[Tooltip("播放的动画片断")]
		public AnimationClip AnimationClip;

		[Tooltip("动画预览专用")]
        public float fCurTime = 0.0f;

        public float fSpeed = 1f;
        public bool visible = true;


		[Tooltip("此动作上挂的特效列表")]
		public List<EffectStruct> EffectStructList;
		public List<AudioStruct> AudioStructList;
		public List<BulletStruct> BulletStructList;
		public List<DamageStruct> DamageStructList;
		public List<ActiveStruct> ActiveStructList;

		public List<MovementStruct> MovementStructList;
		public List<CameraStruct> CameraStructList;
	}

	public class EffectBase
	{
		public int Index;
		public NFAnimaStateType AnimationType;
	}

	[System.Serializable]
	public class EffectStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		public GameObject Effect;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用
		[HideInInspector] public int VirtualPointID;
		public string VirtualPointName;
		public Vector3 Offset;
		public Vector3 Rotate;
		public bool IsFollow;
		public bool IsFollowRoot = false;
		public float DelayTime;
		public float LifeTime;
	}

	[System.Serializable]
	public class AudioStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		public AudioClip Audio;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public float DelayTime;
		public float LifeTime;
	}

	[System.Serializable]
	public class ActiveStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;

		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public GameObject gameObject;
		public bool isActive = false;
		public float DelayTime;
	}

	[System.Serializable]
	public class BulletStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public GameObject Bullet;
		public float DelayTime;
		public enum MoveType
		{
			Line,//Speed,Distance
			TargetObject,//Speed,firepoint
            //targetObject-抛物线
		};

		[HideInInspector] public Vector3 TargetPosition;

		public GameObject StartEffect;
		public AudioClip StartAudio;
		[HideInInspector] public float StartEffLifeTime = 3f;

		public MoveType moveType = MoveType.Line;
		public float Speed = 5;
		public float Distance = 5;

        public AudioClip TouchAudio;
		public GameObject TouchEffect;
		[HideInInspector] public float TouchEffLifeTime = 3f;
	}


	[System.Serializable]
	public class MovementStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public float DelayTime;
		public enum MoveType
		{
			Forward,
			Left,
			Right,
			Back,
		};

		public GameObject StartEffect;
		public AudioClip StartAudio;
		[HideInInspector] public float StartEffLifeTime = 3f;

		public MoveType moveType = MoveType.Forward;
		public float Speed = 5;
		public float Distance = 5;

		public AudioClip TouchAudio;
		public GameObject TouchEffect;
		[HideInInspector] public float TouchEffLifeTime = 3f;
	}

	[System.Serializable]
	public class DamageStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public float DelayTime = 0.8f;
		//effect-->damage--by time
	}

	[System.Serializable]
	public class CameraStruct : EffectBase
	{
		[HideInInspector] public bool isFoldout;
		[HideInInspector] public bool isEnabled = true; //是否启用此子特效,在一个特效下包含多个子特效时可直接通过此项来有选择的显示，多用于美术调试效果用

		public float DelayTime = 0.6f;
		public float ShakeTime = 0.4f;
		public float Strength = 0.4f;
		public int Vibrato = 5;

	}
}
#endregion