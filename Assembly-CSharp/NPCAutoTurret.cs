using System;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class NPCAutoTurret : AutoTurret
{
	// Token: 0x04001B0A RID: 6922
	public Transform centerMuzzle;

	// Token: 0x04001B0B RID: 6923
	public Transform muzzleLeft;

	// Token: 0x04001B0C RID: 6924
	public Transform muzzleRight;

	// Token: 0x04001B0D RID: 6925
	private bool useLeftMuzzle;

	// Token: 0x04001B0E RID: 6926
	[ServerVar(Help = "How many seconds until a sleeping player is considered hostile")]
	public static float sleeperhostiledelay = 1200f;

	// Token: 0x06002306 RID: 8966 RVA: 0x000E0884 File Offset: 0x000DEA84
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetOnline();
		base.SetPeacekeepermode(true);
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CheckPeekers()
	{
		return false;
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000E0899 File Offset: 0x000DEA99
	public override float TargetScanRate()
	{
		return 1.25f;
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return true;
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000709A8 File Offset: 0x0006EBA8
	public override float GetMaxAngleForEngagement()
	{
		return 15f;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool HasFallbackWeapon()
	{
		return true;
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000E08A0 File Offset: 0x000DEAA0
	public override Transform GetCenterMuzzle()
	{
		return this.centerMuzzle;
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000E08A8 File Offset: 0x000DEAA8
	public override void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		muzzleToUse = this.muzzleRight;
		base.FireGun(targetPos, aimCone, muzzleToUse, target);
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000E08BD File Offset: 0x000DEABD
	protected override bool Ignore(BasePlayer player)
	{
		return player is ScientistNPC || player is BanditGuard;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000E08D4 File Offset: 0x000DEAD4
	public override bool IsEntityHostile(BaseCombatEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				return !(basePlayer is ScientistNPC) && !(basePlayer is BanditGuard) && !(basePlayer is NPCShopKeeper) && (!(basePlayer is BasePet) || base.IsEntityHostile(basePlayer));
			}
			if (basePlayer.IsSleeping() && basePlayer.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
		}
		return base.IsEntityHostile(ent);
	}
}
