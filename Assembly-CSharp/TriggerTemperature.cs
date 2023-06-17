using System;
using ConVar;
using UnityEngine;

// Token: 0x02000599 RID: 1433
public class TriggerTemperature : TriggerBase
{
	// Token: 0x0400234D RID: 9037
	public float Temperature = 50f;

	// Token: 0x0400234E RID: 9038
	public float triggerSize;

	// Token: 0x0400234F RID: 9039
	public float minSize;

	// Token: 0x04002350 RID: 9040
	public bool sunlightBlocker;

	// Token: 0x04002351 RID: 9041
	public float sunlightBlockAmount;

	// Token: 0x04002352 RID: 9042
	[Range(0f, 24f)]
	public float blockMinHour = 8.5f;

	// Token: 0x04002353 RID: 9043
	[Range(0f, 24f)]
	public float blockMaxHour = 18.5f;

	// Token: 0x06002BA5 RID: 11173 RVA: 0x0010839C File Offset: 0x0010659C
	private void OnValidate()
	{
		if (base.GetComponent<SphereCollider>() != null)
		{
			this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
			return;
		}
		Vector3 v = Vector3.Scale(base.GetComponent<BoxCollider>().size, base.transform.localScale);
		this.triggerSize = v.Max() * 0.5f;
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x00108408 File Offset: 0x00106608
	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		if (this.sunlightBlocker)
		{
			float time = Env.time;
			if (time >= this.blockMinHour && time <= this.blockMaxHour)
			{
				Vector3 position2 = TOD_Sky.Instance.Components.SunTransform.position;
				if (!GamePhysics.LineOfSight(position, position2, 256, null))
				{
					return oldTemperature - this.sunlightBlockAmount;
				}
			}
			return oldTemperature;
		}
		float value = Vector3.Distance(base.gameObject.transform.position, position);
		float t = Mathf.InverseLerp(this.triggerSize, this.minSize, value);
		return Mathf.Lerp(oldTemperature, this.Temperature, t);
	}

	// Token: 0x06002BA7 RID: 11175 RVA: 0x0010849C File Offset: 0x0010669C
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
