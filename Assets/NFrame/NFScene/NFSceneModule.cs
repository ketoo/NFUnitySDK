using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;
using NFSDK;
using ECM.Components;
using ECM.Controllers;

namespace NFSDK
{
	public class NFSceneModule : NFIModule
	{
		private static bool mbInitSend = false;

		private NFIElementModule mElementModule;
		private NFIKernelModule mKernelModule;
		private NFIEventModule mEventModule;
        
		private NFNetModule mNetModule;
		private NFHelpModule mHelpModule;
		private NFLoginModule mLoginModule;

        private NFUIModule mUIModule;

		private Dictionary<NFGUID, GameObject> mhtObject = new Dictionary<NFGUID, GameObject>();
		private int mnScene = 0;
		private bool mbLoadedScene = false;
		private Dictionary<int, Dictionary<int, List<NFGUID>>> mxObjectList = new Dictionary<int, Dictionary<int, List<NFGUID>>>();

        public NFSceneModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake() 
		{ 
			mKernelModule = FindModule<NFIKernelModule>();
			mElementModule = FindModule<NFIElementModule>();

			mNetModule = FindModule<NFNetModule>();
            mEventModule = FindModule<NFIEventModule>();
            mHelpModule = FindModule<NFHelpModule>();

			mLoginModule = FindModule<NFLoginModule>();

			mUIModule = FindModule<NFUIModule >();
        }

		public override void Init()
		{ 
		}

		public override void AfterInit() 
		{
            mKernelModule.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
            mKernelModule.RegisterClassCallBack(NFrame.NPC.ThisName, OnClassNPCEventHandler);

			mEventModule.RegisterCallback((int)NFNetHandlerModule.Event.SwapScene, SwapSceneEventHandler);
   
		}

		public override void Execute() {}
		public override void BeforeShut() {  }
		public override void Shut() {}

		protected void SwapSceneEventHandler(NFDataList valueList)
		{
			string strSceneID = valueList.StringVal(0);
			NFVector3 vPos = valueList.Vector3Val(1);

			//TODO
			UnityEngine.Application.LoadLevel(1);
		}

        public void InitPlayerComponent(NFGUID xID, GameObject self, bool bMainRole)
        {
            if (null == self)
            {
                return;
            }

            if (!self.GetComponent<Rigidbody>())
            {
                self.AddComponent<Rigidbody>();
            }

            if (!self.GetComponent<NFHeroSyncBuffer>())
            {
                self.AddComponent<NFHeroSyncBuffer>();
            }

            if (!self.GetComponent<NFHeroSync>())
            {
                self.AddComponent<NFHeroSync>();
            }
           
			NFHeroInput xInput = self.GetComponent<NFHeroInput>();
			if (!xInput)
            {
                xInput = self.AddComponent<NFHeroInput>();
            }

			if (bMainRole)
            {
                xInput.enabled = true;
                xInput.SetInputEnable(true);
            }
            else
            {
                xInput.enabled = false;
                xInput.SetInputEnable(false);
            }

            if (!self.GetComponent<GroundDetection>())
            {
                GroundDetection groundDetection = self.AddComponent<GroundDetection>();
                groundDetection.enabled = true;
                groundDetection.groundMask = -1;
            }

            if (!self.GetComponent<CharacterMovement>())
            {
                CharacterMovement characterMovement = self.AddComponent<CharacterMovement>();
                characterMovement.enabled = true;
            }

            if (!self.GetComponent<NFHeroMotor>())
            {
                NFHeroMotor xHeroMotor = self.AddComponent<NFHeroMotor>();
                xHeroMotor.enabled = true;
            }

            if (!self.GetComponent<NFAnimatStateController>())
            {
                NFAnimatStateController xHeroAnima = self.AddComponent<NFAnimatStateController>();
                xHeroAnima.enabled = true;
            }

            if (!self.GetComponent<NFAnimaStateMachine>())
            {
                NFAnimaStateMachine xHeroAnima = self.AddComponent<NFAnimaStateMachine>();
                xHeroAnima.enabled = true;
            }

            if (bMainRole)
            {
				
                if (Camera.main)
                {
                    NFHeroCameraFollow xHeroCameraFollow = Camera.main.GetComponent<NFHeroCameraFollow>();
                    if (!xHeroCameraFollow)
                    {
                        xHeroCameraFollow = Camera.main.GetComponentInParent<NFHeroCameraFollow>();
                    }

                    xHeroCameraFollow.target = self.transform;
                }


                CapsuleCollider xHeroCapsuleCollider = self.GetComponent<CapsuleCollider>();
                xHeroCapsuleCollider.isTrigger = false;
               
            }
            else
            {
                CapsuleCollider xHeroCapsuleCollider = self.GetComponent<CapsuleCollider>();
                Rigidbody rigidbody = self.GetComponent<Rigidbody>();

                string configID = mKernelModule.QueryPropertyString(xID, NFrame.IObject.ConfigID);
                NFMsg.ENPCType npcType = (NFMsg.ENPCType)mElementModule.QueryPropertyInt(configID, NFrame.NPC.NPCType);
                //NFMsg.esub npcSubType = (NFMsg.ENPCType)mElementModule.QueryPropertyInt(configID, NFrame.NPC.NPCSubType);
                if (npcType == NFMsg.ENPCType.TurretNpc)
                {
                    //is trigger must false if it is a building
                    // and the kinematic must true
                    xHeroCapsuleCollider.isTrigger = false;
                    //rigidbody.isKinematic = true;
                    //rigidbody.useGravity = true;
                    rigidbody.mass = 10000;
                }
                else
                {

                    xHeroCapsuleCollider.isTrigger = true;
                }
            }
        }

        private void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
            {
            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
                DestroyObject(self);
            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
            {

                string strHeroCnfID = mKernelModule.QueryPropertyString(self, NFrame.Player.ConfigID);
                NFDataList.TData data = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_STRING);
                if (strHeroCnfID != "")
                {
                    data.Set(strHeroCnfID);
                }
                else
                {
                    data.Set(strConfigIndex);
                }

				if (data.StringVal().Length > 0)
				{
					OnConfigChangeHandler(self, NFrame.Player.ConfigID, data, data);
				}

                mKernelModule.RegisterPropertyCallback(self, NFrame.Player.ConfigID, OnConfigChangeHandler);
                mKernelModule.RegisterPropertyCallback(self, NFrame.Player.HP, OnHPChangeHandler);
            }
        }

        private void OnClassNPCEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
                string strConfigID = mKernelModule.QueryPropertyString(self, NFrame.NPC.ConfigID);
                NFVector3 vec3 = mKernelModule.QueryPropertyVector3(self, NFrame.NPC.Position);

                Vector3 vec = new Vector3();
                vec.x = vec3.X();
                vec.y = vec3.Y();
                vec.z = vec3.Z();

                string strPrefabPath = "";
                if (strConfigID.Length <= 0)
                {
                    strPrefabPath = mElementModule.QueryPropertyString("Enemy", NPC.Prefab);
                }
                else
                {
                    strPrefabPath = mElementModule.QueryPropertyString(strConfigID, NPC.Prefab);
                }

                GameObject xNPC = CreateObject(self, strPrefabPath, vec, strClassName);
                if (xNPC == null)
                {
                    Debug.LogError("Create GameObject fail in " + strConfigID + "  " + strPrefabPath);

                    return;
                }

                xNPC.name = strConfigIndex;
                xNPC.transform.Rotate(new Vector3(0, 90, 0));

                NFBodyIdent xBodyIdent = xNPC.GetComponent<NFBodyIdent>();
                if (null != xBodyIdent)
                {//不能没有
                    xBodyIdent.enabled = true;
                    xBodyIdent.SetObjectID(self);
                    xBodyIdent.cnfID = strConfigID;
                }
                else
                {
                    Debug.LogError("No 'BodyIdent' component in " + strConfigID + "  " + strPrefabPath);
                }

                InitPlayerComponent(self, xNPC, false);

            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
            {

            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
                DestroyObject(self);
            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
            {
                //NFCKernelModule.Instance.RegisterPropertyCallback(self, NFrame.Player.PrefabPath, OnClassPrefabEventHandler);
            }
        }

        private Vector3 GetRenderObjectPosition(NFGUID self)
        {
            if (mhtObject.ContainsKey(self))
            {
                GameObject xGameObject = (GameObject)mhtObject[self];
                return xGameObject.transform.position;
            }

            return Vector3.zero;
        }

        private void OnHPChangeHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
        {
            if (newVar.IntVal() <= 0)
            {
                GameObject go = GetObject(self);
                if (go != null)
                {
                    NFAnimaStateMachine xStateMachineMng = go.GetComponent<NFAnimaStateMachine>();
                    if (xStateMachineMng != null)
                    {
                        xStateMachineMng.ChangeState(NFAnimaStateType.Dead, -1);

                        //show ui
                        //NFUIHeroDie winHeroDie = mUIModule.ShowUI<NFUIHeroDie>();
                        //winHeroDie.ShowReliveUI();
                    }
                }
            }
            else if (newVar.IntVal() > 0 && oldVar.IntVal() <= 0)
            {
                GameObject go = GetObject(self);
                if (go != null)
                {
                    NFAnimaStateMachine xStateMachineMng = go.GetComponent<NFAnimaStateMachine>();
                    if (xStateMachineMng != null)
                    {
                        xStateMachineMng.ChangeState (NFAnimaStateType.Idle, -1);
                    }
                }
            }
        }

        private void OnConfigChangeHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
        {
            Vector3 vec = GetRenderObjectPosition(self);

            DestroyObject(self);

            if (vec.Equals(Vector3.zero))
            {
                NFVector3 vec3 = mKernelModule.QueryPropertyVector3(self, NPC.Position);
                vec.x = vec3.X();
                vec.y = vec3.Y();
                vec.z = vec3.Z();
            }

			string strHeroCnfID = newVar.StringVal();
            string strPrefabPath = mElementModule.QueryPropertyString(strHeroCnfID, NPC.Prefab);
            if (strPrefabPath.Length <= 0)
            {
                strPrefabPath = mElementModule.QueryPropertyString("Enemy", NPC.Prefab);
            }
            GameObject xPlayer = CreateObject(self, strPrefabPath, vec, NFrame.Player.ThisName);
            if (xPlayer)
            {
                xPlayer.name = NFrame.Player.ThisName;
                xPlayer.transform.Rotate(new Vector3(0, 90, 0));

                NFBodyIdent xBodyIdent = xPlayer.GetComponent<NFBodyIdent>();
                if (null != xBodyIdent)
                {//不能没有
                    xBodyIdent.enabled = true;
                    xBodyIdent.SetObjectID(self);
                    xBodyIdent.cnfID = strHeroCnfID;
                }
                else
                {
                    Debug.LogError("No 'BodyIdent' component in " + strPrefabPath);
                }

                if (self == mLoginModule.mRoleID)
                {
                    InitPlayerComponent(self, xPlayer, true);
                }
                else
                {
                    InitPlayerComponent(self, xPlayer, false);
                }

                Debug.Log("Create Object successful" + NFrame.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }
            else
            {
                Debug.LogError("Create Object failed" + NFrame.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }

        }
        public bool IsMainRole(GameObject xGameObject)
        {
            if (null == xGameObject)
            {
                return false;
            }
         
			if (GetObject(mLoginModule.mRoleID).GetInstanceID() == xGameObject.GetInstanceID())
            {
                return true;
            }

            return false;
        }

        public int FindTargetObject(NFGUID ident, string strSkillConfigID, ref NFDataList valueList)
        {

            return valueList.Count();
        }

        public GameObject CreateObject(NFGUID ident, string strPrefabName, Vector3 vec, string strTag)
        {
            if (!mhtObject.ContainsKey(ident))
            {
                try
                {
                    GameObject xGameObject = GameObject.Instantiate(Resources.Load(strPrefabName)) as GameObject;

                    mhtObject.Add(ident, xGameObject);
                    GameObject.DontDestroyOnLoad(xGameObject);

                    xGameObject.transform.position = vec;

                    return xGameObject;
                }
                catch
                {
                    Debug.LogError("Load Prefab Failed " + ident.ToString() + " Prefab:" + strPrefabName);
                }

            }

            return null;
        }

        public bool DestroyObject(NFGUID ident)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                mhtObject.Remove(ident);

				UnityEngine.Object.Destroy(xGameObject);

                //找到title，一起干掉
				//mTitleModule.Destroy(ident);

                return true;
            }


            return false;
        }

        public GameObject GetObject(NFGUID ident)
        {
            if (mhtObject.ContainsKey(ident))
            {
                return (GameObject)mhtObject[ident];
            }

            return null;
        }

        public bool AttackObject(NFGUID ident, Hashtable beAttackInfo, string strStateName, Hashtable resultInfo)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                NFHeroMotor motor = xGameObject.GetComponent<NFHeroMotor>();
                //motor.Stop();
            }

            return false;
        }

        public bool MoveTo(NFGUID ident, Vector3 vTar, float fSpeed, bool bRun)
        {
            if (fSpeed <= 0.01f)
            {
                return false;
            }

            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                NFHeroMotor xMotor = xGameObject.GetComponent<NFHeroMotor>();
                NFBodyIdent xBodyIdent = xGameObject.GetComponent<NFBodyIdent>();
                if (xMotor && xBodyIdent)
                {
                    xBodyIdent.LookAt(vTar);
                    xMotor.movement.Move(vTar - xGameObject.transform.position, Vector3.Distance(vTar, xGameObject.transform.position));
                }
            }

            return false;
        }

        public bool MoveImmune(NFGUID ident, Vector3 vPos, float fTime, bool bFaceToPos)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                NFHeroMotor motor = xGameObject.GetComponent<NFHeroMotor>();
                if (motor)
                {
                    motor.Stop();
                    motor.MoveToImmune(vPos, fTime, bFaceToPos);
                }
            }

            return false;
        }

        public bool MoveImmuneBySpeed(NFGUID ident, Vector3 vPos, float fSpeed, bool bFaceToPos)
        {
            if (fSpeed <= 0.01f)
            {
                return false;
            }

            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                float fDis = Vector3.Distance(xGameObject.transform.position, vPos);
                float fTime = fDis / fSpeed;
                MoveImmune(ident, vPos, fTime, bFaceToPos);
            }

            return false;
        }

        public int GetCurSceneID()
        {
            return mnScene;
        }

        public void SetMainRoleAgentState(bool bActive)
        {
			if (!mhtObject.ContainsKey(mLoginModule.mRoleID))
            {
                return;
            }

			GameObject xGameObject = (GameObject)mhtObject[mLoginModule.mRoleID];
            if (null == xGameObject)
            {
                return;
            }

            NFHeroMotor xMotor = xGameObject.GetComponent<NFHeroMotor>();
            if (null == xMotor)
            {
                return;
            }

        }

        public void LoadScene(int nSceneID, float fX, float fY, float fZ, string strData)
        {
            mbLoadedScene = true;
            mnScene = nSceneID;

			NFUILoading xUILoading = mUIModule.ShowUI<NFUILoading>();
            xUILoading.LoadLevel(nSceneID, new Vector3(fX, fY, fZ));

			if (!mhtObject.ContainsKey(mLoginModule.mRoleID))
            {
                return;
            }

         
        }

        public void BeforeLoadSceneEnd(int nSceneID)
        {
            foreach (var v in mhtObject)
            {
                NFVector3 vPos = mKernelModule.QueryPropertyVector3(v.Key, NFrame.NPC.Position);
                v.Value.transform.position = new Vector3 (vPos.X(), vPos.Y() + 0.1f, vPos.Z());
            }
        }

   
        public void LoadSceneEnd(int nSceneID)
        {
			if (!mbInitSend)
            {
                mbInitSend = true;

                //NFNetController.Instance.mxNetSender.RequireEnterGameFinish (NFNetController.Instance.xMainRoleID);
            }

            if (false == mbLoadedScene)
            {
                return;
            }

            BeforeLoadSceneEnd(nSceneID);

            mbLoadedScene = false;

            //主角贴地，出生点
            /*
            GameObject xGameObject = (GameObject)mhtObject[mLoginModule.mRoleID];
            if (null != xGameObject)
            {
                xGameObject.transform.position = mvSceneBornPos;
                //xGameObject.GetComponent<NFCStateMachineMng> ().ChangeState (NFAnimaStateType.Idle);
            }
            */


            NFMsg.ESceneType nType = (NFMsg.ESceneType)mElementModule.QueryPropertyInt(nSceneID.ToString(), NFrame.Scene.Type);
            if (nType == NFMsg.ESceneType.NormalScene)
            {
                mUIModule.CloseAllUI();
                mUIModule.ShowUI<NFUIMain>();
                mUIModule.ShowUI<NFUIEstateBar>();
                mUIModule.ShowUI<NFUIJoystick>();
            }
            else if (nType == NFMsg.ESceneType.SingleCloneScene)
            {
                mUIModule.CloseAllUI();
                mUIModule.ShowUI<NFUIEstateBar>();
            }
            else if (nType == NFMsg.ESceneType.MultiCloneScene)
            {

            }
            else if (nType == NFMsg.ESceneType.HomeScene)
            {
                //要看是自己的，还是在别人的，如果是别人的，则是在战斗
      
                    mUIModule.CloseAllUI();
                    //先播放readyfight ui的过场动画等
                    mUIModule.ShowUI<NFUIJoystick>();

            }

            Debug.Log("LoadSceneEnd: " + nSceneID + " " + nType);

        }

        public void SetVisibleAll(bool bVisible)
        {
            foreach (KeyValuePair<NFGUID, GameObject> kv in mhtObject)
            {
                GameObject go = (GameObject)kv.Value;
                go.SetActive(bVisible);
            }
        }

        public void SetVisible(NFGUID ident, bool bVisible)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                xGameObject.SetActive(bVisible);
            }
        }

        public void DetroyAll()
        {
            foreach (KeyValuePair<NFGUID, GameObject> kv in mhtObject)
            {
                GameObject go = (GameObject)kv.Value;
                GameObject.Destroy(go);
            }

            mhtObject.Clear();
        }

        public float GetDistance(NFGUID xID1, NFGUID xID2)
        {
            GameObject go1 = GetObject(xID1);
            GameObject go2 = GetObject(xID2);
            if (go1 && go2)
            {
                return Vector3.Distance(go1.transform.position, go2.transform.position);
            }

            return 1000000f;
        }
	}
}
