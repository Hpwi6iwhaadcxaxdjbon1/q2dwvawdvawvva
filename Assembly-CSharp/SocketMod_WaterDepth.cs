using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200027D RID: 637
public class SocketMod_WaterDepth : SocketMod
{
	// Token: 0x04001586 RID: 5510
	public float MinimumWaterDepth = 2f;

	// Token: 0x04001587 RID: 5511
	public float MaximumWaterDepth = 4f;

	// Token: 0x04001588 RID: 5512
	public bool AllowWaterVolumes;

	// Token: 0x04001589 RID: 5513
	public static Translate.Phrase TooDeepPhrase = new Translate.Phrase("error_toodeep", "Water is too deep");

	// Token: 0x0400158A RID: 5514
	public static Translate.Phrase TooShallowPhrase = new Translate.Phrase("error_shallow", "Water is too shallow");

	// Token: 0x06001CD3 RID: 7379 RVA: 0x000C7D74 File Offset: 0x000C5F74
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		if (!this.AllowWaterVolumes)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(vector, 0.5f, list, 262144, QueryTriggerInteraction.Collide);
			int count = list.Count;
			Pool.FreeList<WaterVolume>(ref list);
			if (count > 0)
			{
				Construction.lastPlacementError = "Failed Check: WaterDepth_VolumeCheck (" + this.hierachyName + ")";
				return false;
			}
		}
		vector.y = WaterSystem.GetHeight(vector) - 0.1f;
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(vector, false, null, false);
		if (overallWaterDepth > this.MinimumWaterDepth && overallWaterDepth < this.MaximumWaterDepth)
		{
			return true;
		}
		if (overallWaterDepth <= this.MinimumWaterDepth)
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooShallowPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooDeepPhrase.translated;
		}
		return false;
	}
}
