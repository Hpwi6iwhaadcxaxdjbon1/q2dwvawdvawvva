using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class CeilingLight : IOEntity
{
	// Token: 0x04000652 RID: 1618
	public float pushScale = 2f;

	// Token: 0x06000988 RID: 2440 RVA: 0x00059D78 File Offset: 0x00057F78
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CeilingLight.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x00059DB8 File Offset: 0x00057FB8
	public override int ConsumptionAmount()
	{
		if (base.IsOn())
		{
			return 2;
		}
		return base.ConsumptionAmount();
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00059DCC File Offset: 0x00057FCC
	public override void Hurt(HitInfo info)
	{
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				base.ClientRPC<int, Vector3, Vector3>(null, "ClientPhysPush", 0, info.attackNormal * 3f * (info.damageTypes.Total() / 50f), info.HitPositionWorld);
			}
			base.Hurt(info);
		}
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x00059E30 File Offset: 0x00058030
	public void RefreshGrowables()
	{
		List<GrowableEntity> list = Facepunch.Pool.GetList<GrowableEntity>();
		global::Vis.Entities<GrowableEntity>(base.transform.position + new Vector3(0f, -ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 512, QueryTriggerInteraction.Collide);
		List<PlanterBox> list2 = Facepunch.Pool.GetList<PlanterBox>();
		foreach (GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter = growableEntity.GetPlanter();
				if (planter != null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceLightUpdate();
				}
				growableEntity.CalculateQualities(false, true, false);
				growableEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		Facepunch.Pool.FreeList<PlanterBox>(ref list2);
		Facepunch.Pool.FreeList<GrowableEntity>(ref list);
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x00059F0C File Offset: 0x0005810C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
		if (flag != base.IsOn())
		{
			if (base.IsOn())
			{
				this.LightsOn();
				return;
			}
			this.LightsOff();
		}
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00059F58 File Offset: 0x00058158
	public void LightsOn()
	{
		this.RefreshGrowables();
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00059F58 File Offset: 0x00058158
	public void LightsOff()
	{
		this.RefreshGrowables();
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00059F60 File Offset: 0x00058160
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.RefreshGrowables();
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00059F70 File Offset: 0x00058170
	public override void OnAttacked(HitInfo info)
	{
		float d = 3f * (info.damageTypes.Total() / 50f);
		base.ClientRPC<NetworkableId, Vector3, Vector3>(null, "ClientPhysPush", (info.Initiator != null && info.Initiator is BasePlayer && !info.IsPredicting) ? info.Initiator.net.ID : default(NetworkableId), info.attackNormal * d, info.HitPositionWorld);
		base.OnAttacked(info);
	}
}
