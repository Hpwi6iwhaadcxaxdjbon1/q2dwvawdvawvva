using System;
using UnityEngine;

// Token: 0x02000539 RID: 1337
public static class LODUtil
{
	// Token: 0x04002203 RID: 8707
	public const float DefaultDistance = 1000f;

	// Token: 0x060029CF RID: 10703 RVA: 0x000FFC37 File Offset: 0x000FDE37
	public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		return LODUtil.GetDistance(transform.position, mode);
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x000FFC48 File Offset: 0x000FDE48
	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		if (MainCamera.isValid)
		{
			switch (mode)
			{
			case LODDistanceMode.XYZ:
				return Vector3.Distance(MainCamera.position, worldPos);
			case LODDistanceMode.XZ:
				return Vector3Ex.Distance2D(MainCamera.position, worldPos);
			case LODDistanceMode.Y:
				return Mathf.Abs(MainCamera.position.y - worldPos.y);
			}
		}
		return 1000f;
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x000FFCA4 File Offset: 0x000FDEA4
	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}

	// Token: 0x060029D2 RID: 10706 RVA: 0x000FFCB1 File Offset: 0x000FDEB1
	public static LODEnvironmentMode DetermineEnvironmentMode(Transform transform)
	{
		if (transform.CompareTag("OnlyVisibleUnderground") || transform.root.CompareTag("OnlyVisibleUnderground"))
		{
			return LODEnvironmentMode.Underground;
		}
		return LODEnvironmentMode.Default;
	}
}
