using System;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public static class TerrainCheckEx
{
	// Token: 0x06002FD0 RID: 12240 RVA: 0x0011F5BC File Offset: 0x0011D7BC
	public static bool ApplyTerrainChecks(this Transform transform, TerrainCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (TerrainCheck terrainCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainCheck.worldPosition, scale);
			if (terrainCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			if (!terrainCheck.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
