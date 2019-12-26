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
	public class NFHelpModule : NFIModule
	{
		public NFHelpModule(NFIPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

        public NFMsg.Ident NFToPB(NFGUID xID)
        {
            NFMsg.Ident xIdent = new NFMsg.Ident();
            xIdent.Svrid = xID.nHead64;
            xIdent.Index = xID.nData64;
            return xIdent;
        }

        public NFGUID PBToNF(NFMsg.Ident xID)
        {
            NFGUID xIdent = new NFGUID();
            xIdent.nHead64 = xID.Svrid;
            xIdent.nData64 = xID.Index;

            return xIdent;
        }

        public NFMsg.Vector2 NFToPB(NFVector2 value)
        {
            NFMsg.Vector2 vector = new NFMsg.Vector2();
            vector.X = value.X();
            vector.Y = value.Y();

            return vector;
        }
        public NFVector2 PBToNF(NFMsg.Vector2 xVector)
        {
            NFVector2 xData = new NFVector2(xVector.X, xVector.Y);

            return xData;
        }

        public NFMsg.Vector3 NFToPB(NFVector3 value)
        {
            NFMsg.Vector3 vector = new NFMsg.Vector3();
            vector.X = value.X();
            vector.Y = value.Y();
            vector.Z = value.Z();

            return vector;
        }

        public NFVector3 PBToNF(NFMsg.Vector3 xVector)
        {
            NFVector3 xData = new NFVector3(xVector.X, xVector.Y, xVector.Z);

            return xData;
        }

		public override void Awake()
		{
		}

		public override void Init()
		{
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