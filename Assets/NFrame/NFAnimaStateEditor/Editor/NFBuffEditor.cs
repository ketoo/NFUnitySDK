#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Animations;

using NFSDK;

namespace NFrame
{
	[CustomEditor(typeof(NFBuffController))]
	public class NFBuffEditor : Editor
	{
		#region 数据成员
		private NFBuffController mASD;
		private NFBuffData mData;

		private bool isFoldoutVirtualPoint;
		private string virtualPointName;
		#endregion

		#region 界面重写
		/// <summary>
		/// 界面重写
		/// </summary>
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			mASD = target as NFBuffController;
			mData = mASD.mxBuffData;

			if (mData)
			{
				DrawVirtualPoint();
				DrawBuff();
			}
			else
			{
				EditorGUILayout.HelpBox("Please specify a \"AnimationSkillData\" data .", MessageType.Error);
			}

			EditorUtility.SetDirty(mASD);
			if (mData != null)
				EditorUtility.SetDirty(mData);
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

		void DrawBuff()
		{

		}

	}
#endif
}