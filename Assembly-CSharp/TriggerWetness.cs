using System;
using UnityEngine;

// Token: 0x0200059B RID: 1435
public class TriggerWetness : TriggerBase
{
	// Token: 0x04002357 RID: 9047
	public float Wetness = 0.25f;

	// Token: 0x04002358 RID: 9048
	public SphereCollider TargetCollider;

	// Token: 0x04002359 RID: 9049
	public Transform OriginTransform;

	// Token: 0x0400235A RID: 9050
	public bool ApplyLocalHeightCheck;

	// Token: 0x0400235B RID: 9051
	public float MinLocalHeight;

	// Token: 0x06002BB2 RID: 11186 RVA: 0x00108778 File Offset: 0x00106978
	public float WorkoutWetness(Vector3 position)
	{
		if (this.ApplyLocalHeightCheck && base.transform.InverseTransformPoint(position).y < this.MinLocalHeight)
		{
			return 0f;
		}
		float num = Vector3Ex.Distance2D(this.OriginTransform.position, position) / this.TargetCollider.radius;
		num = Mathf.Clamp01(num);
		num = 1f - num;
		return Mathf.Lerp(0f, this.Wetness, num);
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x001087EC File Offset: 0x001069EC
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
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
