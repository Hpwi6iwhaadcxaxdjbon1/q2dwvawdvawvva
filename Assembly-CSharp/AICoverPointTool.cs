﻿using System;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class AICoverPointTool : MonoBehaviour
{
	// Token: 0x0600192D RID: 6445 RVA: 0x000B92D4 File Offset: 0x000B74D4
	[ContextMenu("Place Cover Points")]
	public void PlaceCoverPoints()
	{
		foreach (object obj in base.transform)
		{
			UnityEngine.Object.DestroyImmediate(((Transform)obj).gameObject);
		}
		Vector3 pos = new Vector3(base.transform.position.x - 50f, base.transform.position.y, base.transform.position.z - 50f);
		for (int i = 0; i < 50; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				AICoverPointTool.TestResult testResult = this.TestPoint(pos);
				if (testResult.Valid)
				{
					this.PlacePoint(testResult);
				}
				pos.x += 2f;
			}
			pos.x -= 100f;
			pos.z += 2f;
		}
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x000B93DC File Offset: 0x000B75DC
	private AICoverPointTool.TestResult TestPoint(Vector3 pos)
	{
		pos.y += 0.5f;
		AICoverPointTool.TestResult result = default(AICoverPointTool.TestResult);
		result.Position = pos;
		if (this.HitsCover(new Ray(pos, Vector3.forward), 1218519041, 1f))
		{
			result.Forward = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.right), 1218519041, 1f))
		{
			result.Right = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.back), 1218519041, 1f))
		{
			result.Backward = true;
			result.Valid = true;
		}
		if (this.HitsCover(new Ray(pos, Vector3.left), 1218519041, 1f))
		{
			result.Left = true;
			result.Valid = true;
		}
		return result;
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x000B94C0 File Offset: 0x000B76C0
	private void PlacePoint(AICoverPointTool.TestResult result)
	{
		if (result.Forward)
		{
			this.PlacePoint(result.Position, Vector3.forward);
		}
		if (result.Right)
		{
			this.PlacePoint(result.Position, Vector3.right);
		}
		if (result.Backward)
		{
			this.PlacePoint(result.Position, Vector3.back);
		}
		if (result.Left)
		{
			this.PlacePoint(result.Position, Vector3.left);
		}
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x000B9531 File Offset: 0x000B7731
	private void PlacePoint(Vector3 pos, Vector3 dir)
	{
		AICoverPoint aicoverPoint = new GameObject("CP").AddComponent<AICoverPoint>();
		aicoverPoint.transform.position = pos;
		aicoverPoint.transform.forward = dir;
		aicoverPoint.transform.SetParent(base.transform);
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x000B956C File Offset: 0x000B776C
	public bool HitsCover(Ray ray, int layerMask, float maxDistance)
	{
		RaycastHit raycastHit;
		return !ray.origin.IsNaNOrInfinity() && !ray.direction.IsNaNOrInfinity() && !(ray.direction == Vector3.zero) && GamePhysics.Trace(ray, 0f, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null);
	}

	// Token: 0x02000C3A RID: 3130
	private struct TestResult
	{
		// Token: 0x04004291 RID: 17041
		public Vector3 Position;

		// Token: 0x04004292 RID: 17042
		public bool Valid;

		// Token: 0x04004293 RID: 17043
		public bool Forward;

		// Token: 0x04004294 RID: 17044
		public bool Right;

		// Token: 0x04004295 RID: 17045
		public bool Backward;

		// Token: 0x04004296 RID: 17046
		public bool Left;
	}
}
