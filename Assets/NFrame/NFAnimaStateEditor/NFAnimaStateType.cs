using System;


namespace NFrame
{

	public enum NFAnimaStateType
	{
		Idle = 0,
		Run = 1,//跑步---需主动切换stop状态to Idle
		Walk = 2,//前走---------------需主动切换stop状态to Idle
		Dizzy = 3,//眩晕---------------需主动切换stop状态to Idle
		Freeze = 4,//冰冻---------------需主动切换stop状态to Idle
		Block = 5,//格挡---------------需主动切换stop状态to Idle
		Fall = 6,//空中降落---------------需主动切换stop状态to Idle
		Fly = 7,//空中降落---------------需主动切换stop状态to Idle
		Dead = 8,//死亡
        JumpStart = 9,//跳跃
        Jumping = 10,//跳跃
        JumpLand = 11,//跳跃
		BeHit1 = 12,
		BeHit2 = 13,
        HitFly = 14,//被击飞
        Stun = 15,

        DashForward = 20,//前冲锋
		DashJump = 21,//跳劈

		Buff1 = 30,

		NormalSkill1 = 40,
		NormalSkill2 = 41,
		NormalSkill3 = 42,
		NormalSkill4 = 43,
		NormalSkill5 = 44,

		SpecialSkill1 = 50,//技能1
		SpecialSkill2 = 51,//技能2
		SkillThump = 52,//大招

		NONE = 100,
	}
}