using System;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class XmasDungeon : HalloweenDungeon
{
	// Token: 0x040010F3 RID: 4339
	public const BaseEntity.Flags HasPlayerOutside = BaseEntity.Flags.Reserved7;

	// Token: 0x040010F4 RID: 4340
	public const BaseEntity.Flags HasPlayerInside = BaseEntity.Flags.Reserved8;

	// Token: 0x040010F5 RID: 4341
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float xmaspopulation = 0f;

	// Token: 0x040010F6 RID: 4342
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float xmaslifetime = 1200f;

	// Token: 0x040010F7 RID: 4343
	[ServerVar(Help = "How far we detect players from our inside/outside", ShowInAdminUI = true)]
	public static float playerdetectrange = 30f;

	// Token: 0x06001810 RID: 6160 RVA: 0x000B4DCB File Offset: 0x000B2FCB
	public override float GetLifetime()
	{
		return XmasDungeon.xmaslifetime;
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x000B4DD2 File Offset: 0x000B2FD2
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.PlayerChecks), 1f, 1f);
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x000B4DF8 File Offset: 0x000B2FF8
	public void PlayerChecks()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		bool b = false;
		bool b2 = false;
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			float num = Vector3.Distance(basePlayer.transform.position, base.transform.position);
			float num2 = Vector3.Distance(basePlayer.transform.position, proceduralDynamicDungeon.GetExitPortal(true).transform.position);
			if (num < XmasDungeon.playerdetectrange)
			{
				b = true;
			}
			if (num2 < XmasDungeon.playerdetectrange * 2f)
			{
				b2 = true;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, b2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, b, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved7, b, false, true);
		proceduralDynamicDungeon.SetFlag(BaseEntity.Flags.Reserved8, b2, false, true);
	}
}
