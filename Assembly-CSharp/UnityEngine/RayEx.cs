using System;

namespace UnityEngine
{
	// Token: 0x02000A27 RID: 2599
	public static class RayEx
	{
		// Token: 0x06003DA6 RID: 15782 RVA: 0x00169C86 File Offset: 0x00167E86
		public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
		{
			return ray.origin + Vector3.Dot(pos - ray.origin, ray.direction) * ray.direction;
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x00169CBC File Offset: 0x00167EBC
		public static float Distance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).magnitude;
		}

		// Token: 0x06003DA8 RID: 15784 RVA: 0x00169CEC File Offset: 0x00167EEC
		public static float SqrDistance(this Ray ray, Vector3 pos)
		{
			return Vector3.Cross(ray.direction, pos - ray.origin).sqrMagnitude;
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x00169D1A File Offset: 0x00167F1A
		public static bool IsNaNOrInfinity(this Ray r)
		{
			return r.origin.IsNaNOrInfinity() || r.direction.IsNaNOrInfinity();
		}
	}
}
