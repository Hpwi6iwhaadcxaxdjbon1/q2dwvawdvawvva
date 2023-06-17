using System;
using UnityEngine;

// Token: 0x02000660 RID: 1632
public class DecorSwim : DecorComponent
{
	// Token: 0x06002F70 RID: 12144 RVA: 0x0011DB70 File Offset: 0x0011BD70
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos.y = TerrainMeta.WaterMap.GetHeight(pos);
	}
}
