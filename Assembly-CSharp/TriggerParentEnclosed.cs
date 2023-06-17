using System;
using ConVar;
using UnityEngine;

// Token: 0x02000590 RID: 1424
public class TriggerParentEnclosed : TriggerParent
{
	// Token: 0x04002339 RID: 9017
	public float Padding;

	// Token: 0x0400233A RID: 9018
	[Tooltip("AnyIntersect: Look for any intersection with the trigger. OriginIntersect: Only consider objects in the trigger if their origin is inside")]
	public TriggerParentEnclosed.TriggerMode intersectionMode;

	// Token: 0x0400233B RID: 9019
	public bool CheckBoundsOnUnparent;

	// Token: 0x0400233C RID: 9020
	private BoxCollider boxCollider;

	// Token: 0x06002B77 RID: 11127 RVA: 0x001079E8 File Offset: 0x00105BE8
	protected void OnEnable()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x001079F6 File Offset: 0x00105BF6
	public override bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		return base.ShouldParent(ent, bypassOtherTriggerCheck) && this.IsInside(ent, this.Padding);
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x00107A14 File Offset: 0x00105C14
	internal override bool SkipOnTriggerExit(Collider collider)
	{
		if (!this.CheckBoundsOnUnparent)
		{
			return false;
		}
		if (!Debugging.checkparentingtriggers)
		{
			return false;
		}
		BaseEntity baseEntity = collider.ToBaseEntity();
		return !(baseEntity == null) && this.IsInside(baseEntity, 0f);
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x00107A54 File Offset: 0x00105C54
	private bool IsInside(BaseEntity ent, float padding)
	{
		Bounds bounds = new Bounds(this.boxCollider.center, this.boxCollider.size);
		if (padding > 0f)
		{
			bounds.Expand(padding);
		}
		OBB obb = new OBB(this.boxCollider.transform, bounds);
		Vector3 target = (this.intersectionMode == TriggerParentEnclosed.TriggerMode.TriggerPoint) ? ent.TriggerPoint() : ent.PivotPoint();
		return obb.Contains(target);
	}

	// Token: 0x02000D5E RID: 3422
	public enum TriggerMode
	{
		// Token: 0x04004720 RID: 18208
		TriggerPoint,
		// Token: 0x04004721 RID: 18209
		PivotPoint
	}
}
