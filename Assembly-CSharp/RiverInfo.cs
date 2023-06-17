using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class RiverInfo : MonoBehaviour
{
	// Token: 0x06002A49 RID: 10825 RVA: 0x001019E1 File Offset: 0x000FFBE1
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.RiverObjs.Add(this);
		}
	}
}
