using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class GrowableHeatSource : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x04000E8B RID: 3723
	public float heatAmount = 5f;

	// Token: 0x06001654 RID: 5716 RVA: 0x000AD630 File Offset: 0x000AB830
	public float ApplyHeat(Vector3 forPosition)
	{
		if (base.baseEntity == null)
		{
			return 0f;
		}
		IOEntity ioentity;
		if (base.baseEntity.IsOn() || ((ioentity = (base.baseEntity as IOEntity)) != null && ioentity.IsPowered()))
		{
			return Mathx.RemapValClamped(Vector3.Distance(forPosition, base.transform.position), 0f, Server.artificialTemperatureGrowableRange, 0f, this.heatAmount);
		}
		return 0f;
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x000AD6A8 File Offset: 0x000AB8A8
	public void ForceUpdateGrowablesInRange()
	{
		List<GrowableEntity> list = Facepunch.Pool.GetList<GrowableEntity>();
		global::Vis.Entities<GrowableEntity>(base.transform.position, Server.artificialTemperatureGrowableRange, list, 512, QueryTriggerInteraction.Collide);
		List<PlanterBox> list2 = Facepunch.Pool.GetList<PlanterBox>();
		foreach (GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter = growableEntity.GetPlanter();
				if (planter != null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceTemperatureUpdate();
				}
				growableEntity.CalculateQualities(false, false, true);
				growableEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		Facepunch.Pool.FreeList<PlanterBox>(ref list2);
		Facepunch.Pool.FreeList<GrowableEntity>(ref list);
	}
}
