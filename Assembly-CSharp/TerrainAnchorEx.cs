using System;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public static class TerrainAnchorEx
{
	// Token: 0x06002FC9 RID: 12233 RVA: 0x0011F3BE File Offset: 0x0011D5BE
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x0011F3D0 File Offset: 0x0011D5D0
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		float num = 0f;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		foreach (TerrainAnchor terrainAnchor in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainAnchor.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			float num4;
			float num5;
			float num6;
			terrainAnchor.Apply(out num4, out num5, out num6, vector2, scale);
			num += num4 - vector.y;
			num2 = Mathf.Max(num2, num5 - vector.y);
			num3 = Mathf.Min(num3, num6 - vector.y);
			if (num3 < num2)
			{
				return false;
			}
		}
		if (num3 > 1f && num2 < 1f)
		{
			num2 = 1f;
		}
		if (mode == TerrainAnchorMode.MinimizeError)
		{
			pos.y = Mathf.Clamp(num / (float)anchors.Length, num2, num3);
		}
		else
		{
			pos.y = Mathf.Clamp(pos.y, num2, num3);
		}
		return true;
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x0011F4E4 File Offset: 0x0011D6E4
	public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
	{
		Vector3 position = transform.position;
		transform.ApplyTerrainAnchors(anchors, ref position, transform.rotation, transform.lossyScale, null);
		transform.position = position;
	}
}
