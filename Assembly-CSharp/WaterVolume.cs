using System;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public class WaterVolume : TriggerBase
{
	// Token: 0x04002360 RID: 9056
	public Bounds WaterBounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x04002361 RID: 9057
	private OBB cachedBounds;

	// Token: 0x04002362 RID: 9058
	private Transform cachedTransform;

	// Token: 0x04002363 RID: 9059
	public Transform[] cutOffPlanes = new Transform[0];

	// Token: 0x04002364 RID: 9060
	public bool waterEnabled = true;

	// Token: 0x06002BDA RID: 11226 RVA: 0x001096E8 File Offset: 0x001078E8
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x00109710 File Offset: 0x00107910
	public bool Test(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(pos))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(pos);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - pos.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x0010981C File Offset: 0x00107A1C
	public bool Test(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		if (!this.cachedBounds.Contains(bounds.ClosestPoint(this.cachedBounds.position)))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(bounds.center))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(bounds.center);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - bounds.min.y);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x0010994C File Offset: 0x00107B4C
	public bool Test(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (!this.waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		this.UpdateCachedTransform();
		Vector3 vector = (start + end) * 0.5f;
		float num = Mathf.Min(start.y, end.y) - radius;
		if (this.cachedBounds.Distance(start) >= radius && this.cachedBounds.Distance(end) >= radius)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		if (!this.CheckCutOffPlanes(vector))
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		Plane plane = new Plane(this.cachedBounds.up, this.cachedBounds.position);
		Vector3 a = plane.ClosestPointOnPlane(vector);
		float y = (a + this.cachedBounds.up * this.cachedBounds.extents.y).y;
		float y2 = (a + -this.cachedBounds.up * this.cachedBounds.extents.y).y;
		info.isValid = true;
		info.currentDepth = Mathf.Max(0f, y - num);
		info.overallDepth = Mathf.Max(0f, y - y2);
		info.surfaceLevel = y;
		return true;
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x00109A98 File Offset: 0x00107C98
	private bool CheckCutOffPlanes(Vector3 pos)
	{
		int num = this.cutOffPlanes.Length;
		bool result = true;
		for (int i = 0; i < num; i++)
		{
			if (this.cutOffPlanes[i] != null && this.cutOffPlanes[i].InverseTransformPoint(pos).y > 0f)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x00109AF0 File Offset: 0x00107CF0
	private void UpdateCachedTransform()
	{
		if (this.cachedTransform != null && this.cachedTransform.hasChanged)
		{
			this.cachedBounds = new OBB(this.cachedTransform, this.WaterBounds);
			this.cachedTransform.hasChanged = false;
		}
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x00109B30 File Offset: 0x00107D30
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
