using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NFrame;
using NFSDK;
using UnityEngine;

namespace NFrame
{

    public class NFUploadDataModule : NFIModule
    {
        public NFUploadDataModule(NFIPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

        NFIKernelModule mKernelModule;
        NFLoginModule mLoginModule;
        NFNetModule mNetModule;
        NFElementModule mElementModule;
        NFClassModule mClassModule;

        public override void Awake()
        {
            mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
            mLoginModule = mPluginManager.FindModule<NFLoginModule>();
            mNetModule = mPluginManager.FindModule<NFNetModule>();
            mElementModule = mPluginManager.FindModule<NFElementModule>();
            mClassModule = mPluginManager.FindModule<NFClassModule>();
        }

		public override void Init()
        {
            mKernelModule.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
        }

        private void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
            {
                NFIClass classObject = mClassModule.GetElement(strClassName);
                NFIPropertyManager propertyManager = classObject.GetPropertyManager();
                NFIRecordManager recordManager = classObject.GetRecordManager();

                NFDataList propertyList = propertyManager.GetPropertyList();
                NFDataList recordList = recordManager.GetRecordList();

                for (int i = 0; i < propertyList.Count(); ++i)
                {
                    NFIProperty propertyObject = propertyManager.GetProperty(propertyList.StringVal(i));
                    if (propertyObject.GetUpload())
                    {
                        mKernelModule.RegisterPropertyCallback(self, propertyObject.GetKey(), OnPropertyDataHandler);
                    }
                }

                for (int i = 0; i < recordList.Count(); ++i)
                {
                    NFIRecord recordObject = recordManager.GetRecord(recordList.StringVal(i));
                    if (recordObject.GetUpload())
                    {
                        mKernelModule.RegisterRecordCallback(self, recordObject.GetName(), RecordEventHandler);
                    }
                }
            }
        }

        private void OnPropertyDataHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
        {
            switch (newVar.GetType())
            {
                case NFDataList.VARIANT_TYPE.VTYPE_INT:
                    {
                        mNetModule.RequirePropertyInt(self, strProperty, newVar.IntVal());
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                    {
                        mNetModule.RequirePropertyFloat(self, strProperty, newVar.FloatVal());
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                    {
                        mNetModule.RequirePropertyString(self, strProperty, newVar.StringVal());
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                    {
                        mNetModule.RequirePropertyObject(self, strProperty, newVar.ObjectVal());
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                    {
                        mNetModule.RequirePropertyVector2(self, strProperty, newVar.Vector2Val());
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                    {
                        mNetModule.RequirePropertyVector3(self, strProperty, newVar.Vector3Val());
                    }
                    break;
                default:
                    break;
            }

        }

        void RecordEventHandler(NFGUID self, string strRecordName, NFIRecord.ERecordOptype eType, int nRow, int nCol, NFDataList.TData oldVar, NFDataList.TData newVar)
        {

            switch (eType)
            {
                case NFIRecord.ERecordOptype.Add:
                    {
                        mNetModule.RequireAddRow(self, strRecordName, nRow);
                    }
                    break;
                case NFIRecord.ERecordOptype.Del:
                    {
                        mNetModule.RequireRemoveRow(self, strRecordName, nRow);
                    }
                    break;
                case NFIRecord.ERecordOptype.Update:
                    {
                        switch (newVar.GetType())
                        {
                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                {
                                    mNetModule.RequireRecordInt(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                                {
                                    mNetModule.RequireRecordFloat(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    mNetModule.RequireRecordString(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                                {
                                    mNetModule.RequireRecordObject(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                                {
                                    mNetModule.RequireRecordVector2(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                                {
                                    mNetModule.RequireRecordVector3(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case NFIRecord.ERecordOptype.Create:
                    break;
                case NFIRecord.ERecordOptype.Cleared:
                    {
                       
                    }
                    break;
                default:
                    break;
            }

        }

        public override void AfterInit()
        {

        }

		public override void Execute()
		{
           
        }

		public override void BeforeShut()
		{
		}

		public override void Shut()
		{
		}
    }

}