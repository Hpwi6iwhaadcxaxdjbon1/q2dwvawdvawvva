using System;
using UnityEngine;

// Token: 0x0200045E RID: 1118
public class TreeMarkerData : PrefabAttribute, IServerComponent
{
	// Token: 0x04001D4A RID: 7498
	public TreeMarkerData.GenerationArc[] GenerationArcs;

	// Token: 0x04001D4B RID: 7499
	public TreeMarkerData.MarkerLocation[] Markers;

	// Token: 0x04001D4C RID: 7500
	public Vector3 GenerationStartPoint = Vector3.up * 2f;

	// Token: 0x04001D4D RID: 7501
	public float GenerationRadius = 2f;

	// Token: 0x04001D4E RID: 7502
	public float MaxY = 1.7f;

	// Token: 0x04001D4F RID: 7503
	public float MinY = 0.2f;

	// Token: 0x04001D50 RID: 7504
	public bool ProcessAngleChecks;

	// Token: 0x060024F1 RID: 9457 RVA: 0x000E9B1E File Offset: 0x000E7D1E
	protected override Type GetIndexedType()
	{
		return typeof(TreeMarkerData);
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x000E9B2C File Offset: 0x000E7D2C
	public Vector3 GetNearbyPoint(Vector3 point, ref int ignoreIndex, out Vector3 normal)
	{
		int num = this.Markers.Length;
		if (ignoreIndex != -1 && this.ProcessAngleChecks)
		{
			ignoreIndex++;
			if (ignoreIndex >= num)
			{
				ignoreIndex = 0;
			}
			normal = this.Markers[ignoreIndex].LocalNormal;
			return this.Markers[ignoreIndex].LocalPosition;
		}
		int num2 = UnityEngine.Random.Range(0, num);
		float num3 = float.MaxValue;
		int num4 = -1;
		for (int i = 0; i < num; i++)
		{
			if (ignoreIndex != num2)
			{
				TreeMarkerData.MarkerLocation markerLocation = this.Markers[num2];
				if (markerLocation.LocalPosition.y >= this.MinY)
				{
					Vector3 localPosition = markerLocation.LocalPosition;
					localPosition.y = Mathf.Lerp(localPosition.y, point.y, 0.5f);
					float num5 = (localPosition - point).sqrMagnitude;
					num5 *= UnityEngine.Random.Range(0.95f, 1.05f);
					if (num5 < num3)
					{
						num3 = num5;
						num4 = num2;
					}
					num2++;
					if (num2 >= num)
					{
						num2 = 0;
					}
				}
			}
		}
		if (num4 > -1)
		{
			normal = this.Markers[num4].LocalNormal;
			ignoreIndex = num4;
			return this.Markers[num4].LocalPosition;
		}
		normal = this.Markers[0].LocalNormal;
		return this.Markers[0].LocalPosition;
	}

	// Token: 0x02000CEB RID: 3307
	[Serializable]
	public struct MarkerLocation
	{
		// Token: 0x0400459D RID: 17821
		public Vector3 LocalPosition;

		// Token: 0x0400459E RID: 17822
		public Vector3 LocalNormal;
	}

	// Token: 0x02000CEC RID: 3308
	[Serializable]
	public struct GenerationArc
	{
		// Token: 0x0400459F RID: 17823
		public Vector3 CentrePoint;

		// Token: 0x040045A0 RID: 17824
		public float Radius;

		// Token: 0x040045A1 RID: 17825
		public Vector3 Rotation;

		// Token: 0x040045A2 RID: 17826
		public int OverrideCount;
	}
}
