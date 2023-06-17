using System;
using UnityEngine;

// Token: 0x02000594 RID: 1428
public class TriggerRadiation : TriggerBase
{
	// Token: 0x04002344 RID: 9028
	public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;

	// Token: 0x04002345 RID: 9029
	public float RadiationAmountOverride;

	// Token: 0x04002346 RID: 9030
	public float falloff = 0.1f;

	// Token: 0x04002347 RID: 9031
	private SphereCollider sphereCollider;

	// Token: 0x06002B8D RID: 11149 RVA: 0x00107EEA File Offset: 0x001060EA
	private float GetRadiationSize()
	{
		if (!this.sphereCollider)
		{
			this.sphereCollider = base.GetComponent<SphereCollider>();
		}
		return this.sphereCollider.radius * base.transform.localScale.Max();
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x00107F24 File Offset: 0x00106124
	public float GetRadiationAmount()
	{
		if (this.RadiationAmountOverride > 0f)
		{
			return this.RadiationAmountOverride;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
		{
			return 10f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x00107F88 File Offset: 0x00106188
	public float GetRadiation(Vector3 position, float radProtection)
	{
		float radiationSize = this.GetRadiationSize();
		float radiationAmount = this.GetRadiationAmount();
		float value = Vector3.Distance(base.gameObject.transform.position, position);
		float num = Mathf.InverseLerp(radiationSize, radiationSize * (1f - this.falloff), value);
		return Mathf.Clamp(radiationAmount - radProtection, 0f, radiationAmount) * num;
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x00107FE0 File Offset: 0x001061E0
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x00108030 File Offset: 0x00106230
	public void OnDrawGizmosSelected()
	{
		float radiationSize = this.GetRadiationSize();
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, radiationSize * (1f - this.falloff));
	}

	// Token: 0x02000D5F RID: 3423
	public enum RadiationTier
	{
		// Token: 0x04004723 RID: 18211
		MINIMAL,
		// Token: 0x04004724 RID: 18212
		LOW,
		// Token: 0x04004725 RID: 18213
		MEDIUM,
		// Token: 0x04004726 RID: 18214
		HIGH
	}
}
