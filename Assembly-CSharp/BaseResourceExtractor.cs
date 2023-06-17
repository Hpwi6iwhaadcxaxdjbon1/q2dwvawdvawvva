using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000401 RID: 1025
public class BaseResourceExtractor : BaseCombatEntity
{
	// Token: 0x04001AF3 RID: 6899
	public bool canExtractLiquid;

	// Token: 0x04001AF4 RID: 6900
	public bool canExtractSolid = true;

	// Token: 0x060022ED RID: 8941 RVA: 0x000DFAC4 File Offset: 0x000DDCC4
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities<SurveyCrater>(base.transform.position, 3f, list, 1, QueryTriggerInteraction.Collide);
		foreach (SurveyCrater surveyCrater in list)
		{
			if (surveyCrater.isServer)
			{
				surveyCrater.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<SurveyCrater>(ref list);
	}
}
