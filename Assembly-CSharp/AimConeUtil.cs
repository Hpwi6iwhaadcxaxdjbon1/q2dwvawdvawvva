using System;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class AimConeUtil
{
	// Token: 0x06001BB0 RID: 7088 RVA: 0x000C2E10 File Offset: 0x000C1010
	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		Quaternion lhs = Quaternion.LookRotation(inputVec);
		Vector2 vector = anywhereInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		return lhs * Quaternion.Euler(vector.x * aimCone * 0.5f, vector.y * aimCone * 0.5f, 0f) * Vector3.forward;
	}

	// Token: 0x06001BB1 RID: 7089 RVA: 0x000C2E70 File Offset: 0x000C1070
	public static Quaternion GetAimConeQuat(float aimCone)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		return Quaternion.Euler(insideUnitSphere.x * aimCone * 0.5f, insideUnitSphere.y * aimCone * 0.5f, 0f);
	}
}
