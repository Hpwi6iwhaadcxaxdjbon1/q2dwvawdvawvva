using System;
using UnityEngine;

// Token: 0x02000549 RID: 1353
public class LakeInfo : MonoBehaviour
{
	// Token: 0x060029DF RID: 10719 RVA: 0x000FFD4F File Offset: 0x000FDF4F
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.LakeObjs.Add(this);
		}
	}
}
