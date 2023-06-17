using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class PathInterestNode : MonoBehaviour, IAIPathInterestNode
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x060018B0 RID: 6320 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x060018B1 RID: 6321 RVA: 0x000B7968 File Offset: 0x000B5B68
	// (set) Token: 0x060018B2 RID: 6322 RVA: 0x000B7970 File Offset: 0x000B5B70
	public float NextVisitTime { get; set; }

	// Token: 0x060018B3 RID: 6323 RVA: 0x000B7979 File Offset: 0x000B5B79
	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
