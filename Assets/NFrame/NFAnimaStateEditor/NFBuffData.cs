using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NFrame
{

	[CreateAssetMenu(fileName = "New BuffData", menuName = "New BuffData")]
	public class NFBuffData : ScriptableObject
	{
		public List<string> VirtualPointList = new List<string> { "None" };
		public List<BuffStruct> BuffList = new List<BuffStruct>();
	}

	[System.Serializable]
	public class BuffStruct
	{
		public NFBuffType BuffType;
		public NFAnimaStateType AnimationType;

	}
}