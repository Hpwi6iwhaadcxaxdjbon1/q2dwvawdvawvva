using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class HitNumber : MonoBehaviour
{
	// Token: 0x04000F2E RID: 3886
	public HitNumber.HitType hitType;

	// Token: 0x060016E5 RID: 5861 RVA: 0x000AF7BD File Offset: 0x000AD9BD
	public int ColorToMultiplier(HitNumber.HitType type)
	{
		switch (type)
		{
		case HitNumber.HitType.Yellow:
			return 1;
		case HitNumber.HitType.Green:
			return 3;
		case HitNumber.HitType.Blue:
			return 5;
		case HitNumber.HitType.Purple:
			return 10;
		case HitNumber.HitType.Red:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x000AF7E8 File Offset: 0x000AD9E8
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(base.transform.position, 0.025f);
	}

	// Token: 0x02000C24 RID: 3108
	public enum HitType
	{
		// Token: 0x04004240 RID: 16960
		Yellow,
		// Token: 0x04004241 RID: 16961
		Green,
		// Token: 0x04004242 RID: 16962
		Blue,
		// Token: 0x04004243 RID: 16963
		Purple,
		// Token: 0x04004244 RID: 16964
		Red
	}
}
