using System;
using UnityEngine;

// Token: 0x02000659 RID: 1625
public class DecorAlign : DecorComponent
{
	// Token: 0x040026AE RID: 9902
	public float NormalAlignment = 1f;

	// Token: 0x040026AF RID: 9903
	public float GradientAlignment = 1f;

	// Token: 0x040026B0 RID: 9904
	public Vector3 SlopeOffset = Vector3.zero;

	// Token: 0x040026B1 RID: 9905
	public Vector3 SlopeScale = Vector3.one;

	// Token: 0x06002F60 RID: 12128 RVA: 0x0011D5B0 File Offset: 0x0011B7B0
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		Vector3 normal = TerrainMeta.HeightMap.GetNormal(pos);
		Vector3 vector = (normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up);
		Vector3 vector2 = Vector3.Cross(normal, vector);
		if (this.SlopeOffset != Vector3.zero || this.SlopeScale != Vector3.one)
		{
			float slope = TerrainMeta.HeightMap.GetSlope01(pos);
			if (this.SlopeOffset != Vector3.zero)
			{
				Vector3 vector3 = this.SlopeOffset * slope;
				pos += vector3.x * vector;
				pos += vector3.y * normal;
				pos -= vector3.z * vector2;
			}
			if (this.SlopeScale != Vector3.one)
			{
				Vector3 vector4 = Vector3.Lerp(Vector3.one, Vector3.one + Quaternion.Inverse(rot) * (this.SlopeScale - Vector3.one), slope);
				scale.x *= vector4.x;
				scale.y *= vector4.y;
				scale.z *= vector4.z;
			}
		}
		Vector3 up = Vector3.Lerp(rot * Vector3.up, normal, this.NormalAlignment);
		Quaternion lhs = QuaternionEx.LookRotationForcedUp(Vector3.Lerp(rot * Vector3.forward, vector2, this.GradientAlignment), up);
		rot = lhs * rot;
	}
}
