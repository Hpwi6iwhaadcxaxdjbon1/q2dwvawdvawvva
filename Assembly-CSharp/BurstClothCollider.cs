using System;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class BurstClothCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DBE RID: 3518
	public float Height;

	// Token: 0x04000DBF RID: 3519
	public float Radius;

	// Token: 0x06001574 RID: 5492 RVA: 0x000AA0CC File Offset: 0x000A82CC
	public CapsuleParams GetParams()
	{
		Vector3 position = base.transform.position;
		float d = this.Height / 2f;
		Vector3 a = base.transform.rotation * Vector3.up;
		Vector3 position2 = position + a * d;
		Vector3 position3 = position - a * d;
		return new CapsuleParams
		{
			Transform = base.transform,
			PointA = base.transform.InverseTransformPoint(position2),
			PointB = base.transform.InverseTransformPoint(position3),
			Radius = this.Radius
		};
	}
}
