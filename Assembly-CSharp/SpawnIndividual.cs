using System;
using UnityEngine;

// Token: 0x02000567 RID: 1383
public struct SpawnIndividual
{
	// Token: 0x04002283 RID: 8835
	public uint PrefabID;

	// Token: 0x04002284 RID: 8836
	public Vector3 Position;

	// Token: 0x04002285 RID: 8837
	public Quaternion Rotation;

	// Token: 0x06002A71 RID: 10865 RVA: 0x00102F9C File Offset: 0x0010119C
	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		this.PrefabID = prefabID;
		this.Position = position;
		this.Rotation = rotation;
	}
}
