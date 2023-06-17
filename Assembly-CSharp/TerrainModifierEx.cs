using System;
using UnityEngine;

// Token: 0x020006EF RID: 1775
public static class TerrainModifierEx
{
	// Token: 0x0600321B RID: 12827 RVA: 0x00134D1C File Offset: 0x00132F1C
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		foreach (TerrainModifier terrainModifier in modifiers)
		{
			Vector3 point = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 pos2 = pos + rot * point;
			float y = scale.y;
			terrainModifier.Apply(pos2, y);
		}
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x00134D65 File Offset: 0x00132F65
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}
