using System;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
public static class TerrainPlacementEx
{
	// Token: 0x06003244 RID: 12868 RVA: 0x00135DE8 File Offset: 0x00133FE8
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (placements.Length == 0)
		{
			return;
		}
		Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
		Matrix4x4 inverse = localToWorld.inverse;
		for (int i = 0; i < placements.Length; i++)
		{
			placements[i].Apply(localToWorld, inverse);
		}
	}

	// Token: 0x06003245 RID: 12869 RVA: 0x00135E24 File Offset: 0x00134024
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}
