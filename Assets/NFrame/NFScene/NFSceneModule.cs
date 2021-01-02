using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;
using NFSDK;
using ECM.Components;
using ECM.Controllers;
using System;

namespace NFSDK
{
	public class NFSceneModule : NFIModule
	{
		private static bool mbInitSend = false;
        private static string mTitleData;

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


        public enum PriorityLevel
        {
            SceneObject = 1,
            SceneSound = 2,

            SceneCamera = 700,
            SceneScenario = 800,

            SceneUI = 999,
        }

        public delegate void SceneLoadDelegation(int sceneId);
        class SceneLoadDelegationObject
        {
            public List<SceneLoadDelegation> list = new List<SceneLoadDelegation>();
        }
        private Dictionary<PriorityLevel, SceneLoadDelegationObject> mhtSceneLoadDelegation = new Dictionary<PriorityLevel, SceneLoadDelegationObject>();




        public NFSceneModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake() 
		{ 
			mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
			mElementModule = mPluginManager.FindModule<NFIElementModule>();

			mNetModule = mPluginManager.FindModule<NFNetModule>();
            mEventModule = mPluginManager.FindModule<NFIEventModule>();
            mHelpModule = mPluginManager.FindModule<NFHelpModule>();

			mLoginModule = mPluginManager.FindModule<NFLoginModule>();

			mUIModule = mPluginManager.FindModule<NFUIModule >();
        }

		public override void Init()
		{ 
		}

		public override void AfterInit() 
		{
            mKernelModule.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
            mKernelModule.RegisterClassCallBack(NFrame.NPC.ThisName, OnClassNPCEventHandler);

		}

		public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                local = true;
            }
        }

        public override void BeforeShut()
        {
            foreach (var item in mhtObject)
            {
                GameObject.DestroyImmediate(item.Value);
            }

            mhtObject.Clear();
        }

		public override void Shut()
        {
        }

        public void AddSceneLoadCallBack(PriorityLevel priorityLevel, SceneLoadDelegation handler)
        {
            if (!mhtSceneLoadDelegation.ContainsKey(priorityLevel))
            {
                mhtSceneLoadDelegation[priorityLevel] = new SceneLoadDelegationObject();
            }

            mhtSceneLoadDelegation[priorityLevel].list.Add(handler);
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

                /*
                if (mKernelModule.QueryPropertyObject(xID, NFrame.NPC.MasterID) == mLoginModule.mRoleID)
                {
                    //your building or your clan member building
                    if (!self.GetComponent<FogCharacter>())
                    {
                        FogCharacter fogCharacter = self.AddComponent<FogCharacter>();
                        fogCharacter.enabled = true;
                        fogCharacter.radius = 8;
                    }
                }
                else
                {
                    if (!self.GetComponent<FogAgent>())
                    {
                        FogAgent fogAgent = self.AddComponent<FogAgent>();
                        fogAgent.enabled = true;
                    }
                }
                */
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

                string strCnfID = mKernelModule.QueryPropertyString(self, NFrame.Player.ConfigID);
                NFDataList.TData data = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_STRING);
                if (strCnfID != "")
                {
                    data.Set(strCnfID);
                }
                else
                {
                    data.Set(strConfigIndex);
                }

				if (data.StringVal().Length > 0)
				{
					OnConfigIDChangeHandler(self, NFrame.Player.ConfigID, data, data);
				}

                mKernelModule.RegisterPropertyCallback(self, NFrame.Player.ConfigID, OnConfigIDChangeHandler);
                mKernelModule.RegisterPropertyCallback(self, NFrame.Player.HP, OnHPChangeHandler);
            }
        }

        private void OnClassNPCEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
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

        private void OnConfigIDChangeHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
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

			string strCnfID = newVar.StringVal();
            string strPrefabPath = mElementModule.QueryPropertyString(strCnfID, NPC.Prefab);
            if (strPrefabPath.Length <= 0)
            {
                strPrefabPath = mElementModule.QueryPropertyString("DefaultObject", NPC.Prefab);
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
                    xBodyIdent.cnfID = strCnfID;
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

                if (Camera.main&& self == mLoginModule.mRoleID)
                {
                    NFHeroCameraFollow xHeroCameraFollow = Camera.main.GetComponent<NFHeroCameraFollow>();
                    if (!xHeroCameraFollow)
                    {
                        xHeroCameraFollow = Camera.main.GetComponentInParent<NFHeroCameraFollow>();
                    }

                    xHeroCameraFollow.SetPlayer(xPlayer.transform);
                }

                Debug.Log("Create Object successful" + NFrame.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }
            else
            {
                Debug.LogError("Create Object failed" + NFrame.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }

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


        public int GetCurSceneID()
        {
            return mnScene;
        }


        static bool local = false;
        public void LoadScene(int nSceneID, float fX, float fY, float fZ, string strData)
        {
            mbLoadedScene = true;
            mnScene = nSceneID;
			mTitleData = strData;

            mUIModule.CloseAllUI();
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
                NFHeroMotor heroMotor = v.Value.GetComponent<NFHeroMotor>();
                heroMotor.ResetHeight();
            }

            NFSDK.NFIElement xElement = mElementModule.GetElement(nSceneID.ToString());
            if (null != xElement)
            {
                string strName = xElement.QueryString(NFrame.Scene.SceneName);
                int sceneType = (int)xElement.QueryInt(NFrame.Scene.Type);

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
            mUIModule.CloseAllUI();
            mUIModule.ShowUI<NFUIMain>();
            mUIModule.ShowUI<NFUIEstateBar>();
            mUIModule.ShowUI<NFUIJoystick>();

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
