using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NFrame
{
	public class NFAnimationEvent
	{
		private Dictionary<int, int> mnDamageMap = new Dictionary<int, int>();

		private List<OnStartAnimaDelegation> OnStartAnimaList = new List<OnStartAnimaDelegation>();
		private List<OnEndAnimaDelegation> OnEndAnimaList = new List<OnEndAnimaDelegation>();
        private List<OnDamageDelegation> OnDamageAnimaList = new List<OnDamageDelegation>();
        private List<OnDamageDelegation> NoDamageAnimaList = new List<OnDamageDelegation>();
		private List<OnBulletTouchPositionDelegation> OnBulletTouchPositionList = new List<OnBulletTouchPositionDelegation>();
		private List<OnBulletTouchTargetDelegation> OnBulletTouchTargetAnimaList = new List<OnBulletTouchTargetDelegation>();

		public delegate void OnStartAnimaDelegation(GameObject self, NFAnimaStateType eAnimaType, int index);
		public delegate void OnEndAnimaDelegation(GameObject self, NFAnimaStateType eAnimaType, int index);
		public delegate void OnDamageDelegation(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index);
		public delegate void OnBulletTouchPositionDelegation(GameObject self, Vector3 position, NFAnimaStateType eAnimaType, int index);
		public delegate void OnBulletTouchTargetDelegation(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index);


		public void AddOnStartAnimaDelegation(OnStartAnimaDelegation msgDelegate)
		{
			OnStartAnimaList.Add(new OnStartAnimaDelegation(msgDelegate));
		}

		public void AddOnEndAnimaDelegation(OnEndAnimaDelegation msgDelegate)
		{
			OnEndAnimaList.Add(new OnEndAnimaDelegation(msgDelegate));
		}

		public void AddOnDamageDelegation(OnDamageDelegation msgDelegate)
		{
			OnDamageAnimaList.Add(new OnDamageDelegation(msgDelegate));
		}

        public void AddNoDamageDelegation(OnDamageDelegation msgDelegate)
        {
            NoDamageAnimaList.Add(new OnDamageDelegation(msgDelegate));
        }

        public void AddBulletTouchPosDelegation(OnBulletTouchPositionDelegation msgDelegate)
		{
			OnBulletTouchPositionList.Add(new OnBulletTouchPositionDelegation(msgDelegate));
		}

		public void AddBulletTouchTargetDelegation(OnBulletTouchTargetDelegation msgDelegate)
		{
			OnBulletTouchTargetAnimaList.Add(new OnBulletTouchTargetDelegation(msgDelegate));
		}

		public void SetDamageNumber(int index, int number)
		{
			mnDamageMap.Add(index, number);
		}

		public void OnStartAnimaEvent(GameObject self, NFAnimaStateType eAnimaType, int index)
		{
			//Debug.Log("Start Anima " + eAnimaType.ToString() + " " + index.ToString());

			for (int i = 0; i < OnStartAnimaList.Count; ++i)
			{
				OnStartAnimaList[i](self, eAnimaType, index);
			}
		}

		public void OnEndAnimaEvent(GameObject self, NFAnimaStateType eAnimaType, int index)
		{
			//Debug.Log("End Anima " + eAnimaType.ToString() + " " + index.ToString());

			for (int i = 0; i < OnEndAnimaList.Count; ++i)
			{
				OnEndAnimaList[i](self, eAnimaType, index);
			}
		}

        public void NoDamageEvent(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index)
        {
            Debug.Log("No Damage " + self.ToString() + " " + target.ToString() + " " + index.ToString());

            for (int i = 0; i < NoDamageAnimaList.Count; ++i)
            {
                NoDamageAnimaList[i](self, target, eAnimaType, index);
            }
        }

        public void OnDamageEvent(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index)
		{
			Debug.Log("On Damage " + self.ToString() + " " + target.ToString() + " " + index.ToString());

			for (int i = 0; i < OnDamageAnimaList.Count; ++i)
			{
				OnDamageAnimaList[i](self, target, eAnimaType, index);
			}
		}

		public void OnBulletTouchPositionEvent(GameObject self, Vector3 position, NFAnimaStateType eAnimaType, int index)
		{
			Debug.Log("TouchPosition " + index);

			for (int i = 0; i < OnBulletTouchPositionList.Count; ++i)
			{
				OnBulletTouchPositionList[i](self, position, eAnimaType, index);
			}
		}
		public void OnBulletTouchTargetEvent(GameObject self, GameObject target, NFAnimaStateType eAnimaType, int index)
		{
			Debug.Log("TouchTarget " + index);

			for (int i = 0; i < OnBulletTouchTargetAnimaList.Count; ++i)
			{
				OnBulletTouchTargetAnimaList[i](self, target, eAnimaType, index);
			}
		}
	}

}