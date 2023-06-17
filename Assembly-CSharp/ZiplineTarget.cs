using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class ZiplineTarget : MonoBehaviour
{
	// Token: 0x04000DB0 RID: 3504
	public Transform Target;

	// Token: 0x04000DB1 RID: 3505
	public bool IsChainPoint;

	// Token: 0x04000DB2 RID: 3506
	public float MonumentConnectionDotMin = 0.2f;

	// Token: 0x04000DB3 RID: 3507
	public float MonumentConnectionDotMax = 1f;

	// Token: 0x06001570 RID: 5488 RVA: 0x000A9F90 File Offset: 0x000A8190
	public bool IsValidPosition(Vector3 position)
	{
		float num = Vector3.Dot((position - this.Target.position.WithY(position.y)).normalized, this.Target.forward);
		return num >= this.MonumentConnectionDotMin && num <= this.MonumentConnectionDotMax;
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000A9FEC File Offset: 0x000A81EC
	public bool IsValidChainPoint(Vector3 from, Vector3 to)
	{
		float num = Vector3.Dot((from - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		float num2 = Vector3.Dot((to - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		if ((num > 0f && num2 > 0f) || (num < 0f && num2 < 0f))
		{
			return false;
		}
		num2 = Mathf.Abs(num2);
		return num2 >= this.MonumentConnectionDotMin && num2 <= this.MonumentConnectionDotMax;
	}
}
