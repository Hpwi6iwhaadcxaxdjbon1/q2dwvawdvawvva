using System;
using System.Collections.Generic;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002DA RID: 730
public interface IPosLerpTarget : ILerpInfo
{
	// Token: 0x06001DBD RID: 7613
	float GetInterpolationInertia();

	// Token: 0x06001DBE RID: 7614
	Vector3 GetNetworkPosition();

	// Token: 0x06001DBF RID: 7615
	Quaternion GetNetworkRotation();

	// Token: 0x06001DC0 RID: 7616
	void SetNetworkPosition(Vector3 pos);

	// Token: 0x06001DC1 RID: 7617
	void SetNetworkRotation(Quaternion rot);

	// Token: 0x06001DC2 RID: 7618
	void DrawInterpolationState(Interpolator<TransformSnapshot>.Segment segment, List<TransformSnapshot> entries);

	// Token: 0x06001DC3 RID: 7619
	void LerpIdleDisable();
}
