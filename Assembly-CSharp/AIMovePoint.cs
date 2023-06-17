using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class AIMovePoint : AIPoint
{
	// Token: 0x04001247 RID: 4679
	public ListDictionary<AIMovePoint, float> distances = new ListDictionary<AIMovePoint, float>();

	// Token: 0x04001248 RID: 4680
	public ListDictionary<AICoverPoint, float> distancesToCover = new ListDictionary<AICoverPoint, float>();

	// Token: 0x04001249 RID: 4681
	public float radius = 1f;

	// Token: 0x0400124A RID: 4682
	public float WaitTime;

	// Token: 0x0400124B RID: 4683
	public List<Transform> LookAtPoints;

	// Token: 0x06001971 RID: 6513 RVA: 0x000BB098 File Offset: 0x000B9298
	public void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCircleY(base.transform.position, this.radius);
		Gizmos.color = color;
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000BB0C4 File Offset: 0x000B92C4
	public void DrawLookAtPoints()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.gray;
		if (this.LookAtPoints != null)
		{
			foreach (Transform transform in this.LookAtPoints)
			{
				if (!(transform == null))
				{
					Gizmos.DrawSphere(transform.position, 0.2f);
					Gizmos.DrawLine(base.transform.position, transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x000BB160 File Offset: 0x000B9360
	public void Clear()
	{
		this.LookAtPoints = null;
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x000BB169 File Offset: 0x000B9369
	public void AddLookAtPoint(Transform transform)
	{
		if (this.LookAtPoints == null)
		{
			this.LookAtPoints = new List<Transform>();
		}
		this.LookAtPoints.Add(transform);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x000BB18A File Offset: 0x000B938A
	public bool HasLookAtPoints()
	{
		return this.LookAtPoints != null && this.LookAtPoints.Count > 0;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000BB1A4 File Offset: 0x000B93A4
	public Transform GetRandomLookAtPoint()
	{
		if (this.LookAtPoints == null || this.LookAtPoints.Count == 0)
		{
			return null;
		}
		return this.LookAtPoints[UnityEngine.Random.Range(0, this.LookAtPoints.Count)];
	}

	// Token: 0x02000C3B RID: 3131
	public class DistTo
	{
		// Token: 0x04004297 RID: 17047
		public float distance;

		// Token: 0x04004298 RID: 17048
		public AIMovePoint target;
	}
}
