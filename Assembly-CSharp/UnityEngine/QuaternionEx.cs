using System;

namespace UnityEngine
{
	// Token: 0x02000A26 RID: 2598
	public static class QuaternionEx
	{
		// Token: 0x06003DA0 RID: 15776 RVA: 0x00169B3C File Offset: 0x00167D3C
		public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
		{
			return Quaternion.FromToRotation(Vector3.up, normal) * rot;
		}

		// Token: 0x06003DA1 RID: 15777 RVA: 0x00169B4F File Offset: 0x00167D4F
		public static Quaternion LookRotationWithOffset(Vector3 offset, Vector3 forward, Vector3 up)
		{
			return Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.up));
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x00169B74 File Offset: 0x00167D74
		public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
		{
			if (forward == up)
			{
				return Quaternion.LookRotation(up);
			}
			Vector3 rhs = Vector3.Cross(forward, up);
			forward = Vector3.Cross(up, rhs);
			return Quaternion.LookRotation(forward, up);
		}

		// Token: 0x06003DA3 RID: 15779 RVA: 0x00169BAC File Offset: 0x00167DAC
		public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
		{
			Vector3 rhs = (normal == Vector3.up) ? Vector3.forward : Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, rhs), up);
		}

		// Token: 0x06003DA4 RID: 15780 RVA: 0x00169BE8 File Offset: 0x00167DE8
		public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = default(Vector3))
		{
			if (up != Vector3.zero)
			{
				return QuaternionEx.LookRotationForcedUp(up, normal);
			}
			if (normal == Vector3.up)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.forward, normal);
			}
			if (normal == Vector3.down)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.back, normal);
			}
			if (normal.y == 0f)
			{
				return QuaternionEx.LookRotationForcedUp(Vector3.up, normal);
			}
			Vector3 rhs = Vector3.Cross(normal, Vector3.up);
			return QuaternionEx.LookRotationForcedUp(-Vector3.Cross(normal, rhs), normal);
		}

		// Token: 0x06003DA5 RID: 15781 RVA: 0x00169C73 File Offset: 0x00167E73
		public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1E-45f)
		{
			if (Quaternion.Dot(rot, rot) < epsilon)
			{
				return Quaternion.identity;
			}
			return rot;
		}
	}
}
