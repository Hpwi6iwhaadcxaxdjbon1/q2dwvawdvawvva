using System;
using UnityEngine;

// Token: 0x0200070A RID: 1802
public static class WaterCheckEx
{
	// Token: 0x060032EF RID: 13039 RVA: 0x00139A4C File Offset: 0x00137C4C
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (WaterCheck waterCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(waterCheck.worldPosition, scale);
			if (waterCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 pos2 = pos + vector;
			if (!waterCheck.Check(pos2))
			{
				return false;
			}
		}
		return true;
	}
}
