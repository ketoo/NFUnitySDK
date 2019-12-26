// -------------------------------------------------------------------------
//    @FileName         :    NFProtocolDefine.ts
//    @Author           :    NFrame Studio
//    @Module           :    NFProtocolDefine
// -------------------------------------------------------------------------

	class DescData
	{
		//Class name
		public static  ThisName = "DescData";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static Atlas = "Atlas";// string
		public static DescText = "DescText";// string
		public static Icon = "Icon";// string
		public static PerformanceEffect = "PerformanceEffect";// string
		public static PerformanceSound = "PerformanceSound";// string
		public static PrefabPath = "PrefabPath";// string
		public static ShowName = "ShowName";// string
		// Record

	}
	class EffectData
	{
		//Class name
		public static  ThisName = "EffectData";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static ATK_DARK = "ATK_DARK";// int
		public static ATK_FIRE = "ATK_FIRE";// int
		public static ATK_ICE = "ATK_ICE";// int
		public static ATK_LIGHT = "ATK_LIGHT";// int
		public static ATK_POISON = "ATK_POISON";// int
		public static ATK_SPEED = "ATK_SPEED";// int
		public static ATK_VALUE = "ATK_VALUE";// int
		public static ATK_WIND = "ATK_WIND";// int
		public static BUFF_GATE = "BUFF_GATE";// int
		public static CRITICAL = "CRITICAL";// int
		public static DEF_DARK = "DEF_DARK";// int
		public static DEF_FIRE = "DEF_FIRE";// int
		public static DEF_ICE = "DEF_ICE";// int
		public static DEF_LIGHT = "DEF_LIGHT";// int
		public static DEF_POISON = "DEF_POISON";// int
		public static DEF_VALUE = "DEF_VALUE";// int
		public static DEF_WIND = "DEF_WIND";// int
		public static DIZZY_GATE = "DIZZY_GATE";// int
		public static HP = "HP";// int
		public static HPREGEN = "HPREGEN";// int
		public static LUCK = "LUCK";// int
		public static MAGIC_GATE = "MAGIC_GATE";// int
		public static MAXHP = "MAXHP";// int
		public static MAXMP = "MAXMP";// int
		public static MAXSP = "MAXSP";// int
		public static MOVE_GATE = "MOVE_GATE";// int
		public static MOVE_SPEED = "MOVE_SPEED";// int
		public static MP = "MP";// int
		public static MPREGEN = "MPREGEN";// int
		public static PHYSICAL_GATE = "PHYSICAL_GATE";// int
		public static REFLECTDAMAGE = "REFLECTDAMAGE";// int
		public static SKILL_GATE = "SKILL_GATE";// int
		public static SP = "SP";// int
		public static SPREGEN = "SPREGEN";// int
		public static SUCKBLOOD = "SUCKBLOOD";// int
		// Record
		public static CommValue = 
		{
			//Class name
			"ThisName":"CommValue",
			"SUCKBLOOD":0,
			"REFLECTDAMAGE":1,
			"CRITICAL":2,
			"MAXHP":3,
			"MAXMP":4,
			"MAXSP":5,
			"HPREGEN":6,
			"SPREGEN":7,
			"MPREGEN":8,
			"ATK_VALUE":9,
			"DEF_VALUE":10,
			"MOVE_SPEED":11,
			"ATK_SPEED":12,
			"ATK_FIRE":13,
			"ATK_LIGHT":14,
			"ATK_DARK":15,
			"ATK_WIND":16,
			"ATK_ICE":17,
			"ATK_POISON":18,
			"DEF_FIRE":19,
			"DEF_LIGHT":20,
			"DEF_DARK":21,
			"DEF_WIND":22,
			"DEF_ICE":23,
			"DEF_POISON":24,
			"DIZZY_GATE":25,
			"MOVE_GATE":26,
			"SKILL_GATE":27,
			"PHYSICAL_GATE":28,
			"MAGIC_GATE":29,
			"BUFF_GATE":30,
			"LUCK":31
		}

	}
	class GM
	{
		//Class name
		public static  ThisName = "GM";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static Level = "Level";// int
		// Record

	}
	class Group
	{
		//Class name
		public static  ThisName = "Group";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		// Record

	}
	class IObject
	{
		//Class name
		public static  ThisName = "IObject";
		// Property
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Record

	}
	class InitProperty
	{
		//Class name
		public static  ThisName = "InitProperty";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static HeroConfigID = "HeroConfigID";// string
		public static Job = "Job";// int
		public static Level = "Level";// int
		// Record

	}
	class Language
	{
		//Class name
		public static  ThisName = "Language";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static Chinese = "Chinese";// string
		public static English = "English";// string
		public static French = "French";// string
		public static Spanish = "Spanish";// string
		// Record

	}
	class NPC
	{
		//Class name
		public static  ThisName = "NPC";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static AIOwnerID = "AIOwnerID";// object
		public static ATK_DARK = "ATK_DARK";// int
		public static ATK_FIRE = "ATK_FIRE";// int
		public static ATK_ICE = "ATK_ICE";// int
		public static ATK_LIGHT = "ATK_LIGHT";// int
		public static ATK_POISON = "ATK_POISON";// int
		public static ATK_SPEED = "ATK_SPEED";// int
		public static ATK_VALUE = "ATK_VALUE";// int
		public static ATK_WIND = "ATK_WIND";// int
		public static BUFF_GATE = "BUFF_GATE";// int
		public static CRITICAL = "CRITICAL";// int
		public static Camp = "Camp";// object
		public static Climb = "Climb";// int
		public static CrisisDis = "CrisisDis";// float
		public static DEF_DARK = "DEF_DARK";// int
		public static DEF_FIRE = "DEF_FIRE";// int
		public static DEF_ICE = "DEF_ICE";// int
		public static DEF_LIGHT = "DEF_LIGHT";// int
		public static DEF_POISON = "DEF_POISON";// int
		public static DEF_VALUE = "DEF_VALUE";// int
		public static DEF_WIND = "DEF_WIND";// int
		public static DIZZY_GATE = "DIZZY_GATE";// int
		public static DescID = "DescID";// string
		public static Diamond = "Diamond";// int
		public static DropPackList = "DropPackList";// string
		public static DropProbability = "DropProbability";// int
		public static EXP = "EXP";// int
		public static EffectData = "EffectData";// string
		public static Gold = "Gold";// int
		public static HP = "HP";// int
		public static HPREGEN = "HPREGEN";// int
		public static Height = "Height";// float
		public static Icon = "Icon";// string
		public static LUCK = "LUCK";// int
		public static LastAttacker = "LastAttacker";// object
		public static Level = "Level";// int
		public static MAGIC_GATE = "MAGIC_GATE";// int
		public static MAXHP = "MAXHP";// int
		public static MAXMP = "MAXMP";// int
		public static MAXSP = "MAXSP";// int
		public static MOVE_GATE = "MOVE_GATE";// int
		public static MOVE_SPEED = "MOVE_SPEED";// int
		public static MP = "MP";// int
		public static MPREGEN = "MPREGEN";// int
		public static MasterID = "MasterID";// object
		public static MasterName = "MasterName";// string
		public static MeleeType = "MeleeType";// int
		public static NPCSubType = "NPCSubType";// int
		public static NPCType = "NPCType";// int
		public static PHYSICAL_GATE = "PHYSICAL_GATE";// int
		public static Prefab = "Prefab";// string
		public static REFLECTDAMAGE = "REFLECTDAMAGE";// int
		public static SKILL_GATE = "SKILL_GATE";// int
		public static SP = "SP";// int
		public static SPREGEN = "SPREGEN";// int
		public static SUCKBLOOD = "SUCKBLOOD";// int
		public static SeedID = "SeedID";// string
		public static ShowCard = "ShowCard";// string
		public static ShowName = "ShowName";// string
		public static SkillNormal = "SkillNormal";// string
		public static SkillSpecial1 = "SkillSpecial1";// string
		public static SkillSpecial2 = "SkillSpecial2";// string
		public static SkillTHUMP = "SkillTHUMP";// string
		public static SpriteFile = "SpriteFile";// string
		public static TargetX = "TargetX";// float
		public static TargetY = "TargetY";// float
		public static VIPEXP = "VIPEXP";// int
		public static Width = "Width";// float
		// Record
		public static CommValue = 
		{
			//Class name
			"ThisName":"CommValue",
			"SUCKBLOOD":0,
			"REFLECTDAMAGE":1,
			"CRITICAL":2,
			"MAXHP":3,
			"MAXMP":4,
			"MAXSP":5,
			"HPREGEN":6,
			"SPREGEN":7,
			"MPREGEN":8,
			"ATK_VALUE":9,
			"DEF_VALUE":10,
			"MOVE_SPEED":11,
			"ATK_SPEED":12,
			"ATK_FIRE":13,
			"ATK_LIGHT":14,
			"ATK_DARK":15,
			"ATK_WIND":16,
			"ATK_ICE":17,
			"ATK_POISON":18,
			"DEF_FIRE":19,
			"DEF_LIGHT":20,
			"DEF_DARK":21,
			"DEF_WIND":22,
			"DEF_ICE":23,
			"DEF_POISON":24,
			"DIZZY_GATE":25,
			"MOVE_GATE":26,
			"SKILL_GATE":27,
			"PHYSICAL_GATE":28,
			"MAGIC_GATE":29,
			"BUFF_GATE":30,
			"LUCK":31
		}

	}
	class NoSqlServer
	{
		//Class name
		public static  ThisName = "NoSqlServer";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static Auth = "Auth";// string
		public static IP = "IP";// string
		public static Port = "Port";// int
		public static ServerID = "ServerID";// int
		// Record

	}
	class Player
	{
		//Class name
		public static  ThisName = "Player";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static ATK_DARK = "ATK_DARK";// int
		public static ATK_FIRE = "ATK_FIRE";// int
		public static ATK_ICE = "ATK_ICE";// int
		public static ATK_LIGHT = "ATK_LIGHT";// int
		public static ATK_POISON = "ATK_POISON";// int
		public static ATK_SPEED = "ATK_SPEED";// int
		public static ATK_VALUE = "ATK_VALUE";// int
		public static ATK_WIND = "ATK_WIND";// int
		public static Account = "Account";// string
		public static BUFF_GATE = "BUFF_GATE";// int
		public static CRITICAL = "CRITICAL";// int
		public static Camp = "Camp";// object
		public static ConnectKey = "ConnectKey";// string
		public static DEF_DARK = "DEF_DARK";// int
		public static DEF_FIRE = "DEF_FIRE";// int
		public static DEF_ICE = "DEF_ICE";// int
		public static DEF_LIGHT = "DEF_LIGHT";// int
		public static DEF_POISON = "DEF_POISON";// int
		public static DEF_VALUE = "DEF_VALUE";// int
		public static DEF_WIND = "DEF_WIND";// int
		public static DIZZY_GATE = "DIZZY_GATE";// int
		public static Diamond = "Diamond";// int
		public static EXP = "EXP";// int
		public static GMLevel = "GMLevel";// int
		public static GameID = "GameID";// int
		public static GateID = "GateID";// int
		public static Gold = "Gold";// int
		public static HP = "HP";// int
		public static HPREGEN = "HPREGEN";// int
		public static Head = "Head";// string
		public static Job = "Job";// int
		public static LUCK = "LUCK";// int
		public static LastOfflineTime = "LastOfflineTime";// object
		public static Level = "Level";// int
		public static MAGIC_GATE = "MAGIC_GATE";// int
		public static MAXEXP = "MAXEXP";// int
		public static MAXHP = "MAXHP";// int
		public static MAXMP = "MAXMP";// int
		public static MAXSP = "MAXSP";// int
		public static MOVE_GATE = "MOVE_GATE";// int
		public static MOVE_SPEED = "MOVE_SPEED";// int
		public static MP = "MP";// int
		public static MPREGEN = "MPREGEN";// int
		public static OnlineCount = "OnlineCount";// int
		public static OnlineTime = "OnlineTime";// object
		public static PHYSICAL_GATE = "PHYSICAL_GATE";// int
		public static REFLECTDAMAGE = "REFLECTDAMAGE";// int
		public static Race = "Race";// int
		public static SKILL_GATE = "SKILL_GATE";// int
		public static SP = "SP";// int
		public static SPREGEN = "SPREGEN";// int
		public static SUCKBLOOD = "SUCKBLOOD";// int
		public static Sex = "Sex";// int
		public static SkillNormal = "SkillNormal";// string
		public static SkillSpecial1 = "SkillSpecial1";// string
		public static SkillSpecial2 = "SkillSpecial2";// string
		public static SkillTHUMP = "SkillTHUMP";// string
		public static TotalTime = "TotalTime";// int
		// Record
		public static CommValue = 
		{
			//Class name
			"ThisName":"CommValue",
			"SUCKBLOOD":0,
			"REFLECTDAMAGE":1,
			"CRITICAL":2,
			"MAXHP":3,
			"MAXMP":4,
			"MAXSP":5,
			"HPREGEN":6,
			"SPREGEN":7,
			"MPREGEN":8,
			"ATK_VALUE":9,
			"DEF_VALUE":10,
			"MOVE_SPEED":11,
			"ATK_SPEED":12,
			"ATK_FIRE":13,
			"ATK_LIGHT":14,
			"ATK_DARK":15,
			"ATK_WIND":16,
			"ATK_ICE":17,
			"ATK_POISON":18,
			"DEF_FIRE":19,
			"DEF_LIGHT":20,
			"DEF_DARK":21,
			"DEF_WIND":22,
			"DEF_ICE":23,
			"DEF_POISON":24,
			"DIZZY_GATE":25,
			"MOVE_GATE":26,
			"SKILL_GATE":27,
			"PHYSICAL_GATE":28,
			"MAGIC_GATE":29,
			"BUFF_GATE":30,
			"LUCK":31
		}
		public static Cooldown = 
		{
			//Class name
			"ThisName":"Cooldown",
			"SkillID":0,
			"Time":1
		}

	}
	class Scene
	{
		//Class name
		public static  ThisName = "Scene";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static ActorID = "ActorID";// int
		public static FilePath = "FilePath";// string
		public static LoadingUI = "LoadingUI";// string
		public static MaxGroup = "MaxGroup";// int
		public static MaxGroupPlayers = "MaxGroupPlayers";// int
		public static NavigationResPath = "NavigationResPath";// string
		public static RelivePos = "RelivePos";// string
		public static ResPath = "ResPath";// string
		public static SceneName = "SceneName";// string
		public static SceneShowName = "SceneShowName";// string
		public static SoundList = "SoundList";// string
		public static Type = "Type";// int
		public static Width = "Width";// int
		// Record

	}
	class Security
	{
		//Class name
		public static  ThisName = "Security";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static SecurityData = "SecurityData";// string
		// Record

	}
	class Server
	{
		//Class name
		public static  ThisName = "Server";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static Area = "Area";// int
		public static Cell = "Cell";// int
		public static CpuCount = "CpuCount";// int
		public static IP = "IP";// string
		public static MaxOnline = "MaxOnline";// int
		public static Port = "Port";// int
		public static ServerID = "ServerID";// int
		public static Type = "Type";// int
		public static WSPort = "WSPort";// int
		public static WebPort = "WebPort";// int
		// Record

	}
	class Skill
	{
		//Class name
		public static  ThisName = "Skill";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static AnimaState = "AnimaState";// string
		public static AtkDis = "AtkDis";// float
		public static ConsumeProperty = "ConsumeProperty";// string
		public static ConsumeType = "ConsumeType";// int
		public static ConsumeValue = "ConsumeValue";// string
		public static CoolDownTime = "CoolDownTime";// float
		public static DamageCount = "DamageCount";// int
		public static DamageDistance = "DamageDistance";// float
		public static DamageIntervalTime = "DamageIntervalTime";// float
		public static DamageProperty = "DamageProperty";// string
		public static DamageType = "DamageType";// int
		public static DamageValue = "DamageValue";// int
		public static DefaultHitTime = "DefaultHitTime";// string
		public static Desc = "Desc";// string
		public static EffectObjType = "EffectObjType";// int
		public static GetBuffList = "GetBuffList";// string
		public static Icon = "Icon";// string
		public static Melee = "Melee";// int
		public static NewObject = "NewObject";// string
		public static NextID = "NextID";// string
		public static PlayerSkill = "PlayerSkill";// int
		public static SendBuffList = "SendBuffList";// string
		public static ShowName = "ShowName";// string
		public static SkillType = "SkillType";// int
		public static SpriteFile = "SpriteFile";// string
		// Record

	}
	class SqlServer
	{
		//Class name
		public static  ThisName = "SqlServer";
		// IObject
		public static ClassName = "ClassName";// string
		public static ConfigID = "ConfigID";// string
		public static GroupID = "GroupID";// int
		public static ID = "ID";// string
		public static MoveTo = "MoveTo";// vector3
		public static Name = "Name";// string
		public static Position = "Position";// vector3
		public static SceneID = "SceneID";// int
		// Property
		public static IP = "IP";// string
		public static Port = "Port";// int
		public static ServerID = "ServerID";// int
		public static SqlIP = "SqlIP";// string
		public static SqlName = "SqlName";// string
		public static SqlPort = "SqlPort";// int
		public static SqlPwd = "SqlPwd";// string
		public static SqlUser = "SqlUser";// string
		// Record

	}
export const NFConfig={
		DescData,EffectData,GM,Group,IObject,InitProperty,Language,NPC,NoSqlServer,Player,Scene,Security,Server,Skill,SqlServer}
