using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000580 RID: 1408
public class TriggerComfort : TriggerBase
{
	// Token: 0x04002301 RID: 8961
	public float triggerSize;

	// Token: 0x04002302 RID: 8962
	public float baseComfort = 0.5f;

	// Token: 0x04002303 RID: 8963
	public float minComfortRange = 2.5f;

	// Token: 0x04002304 RID: 8964
	private const float perPlayerComfortBonus = 0.25f;

	// Token: 0x04002305 RID: 8965
	private const float bonusComfort = 0f;

	// Token: 0x04002306 RID: 8966
	private List<BasePlayer> _players = new List<BasePlayer>();

	// Token: 0x06002B30 RID: 11056 RVA: 0x00106668 File Offset: 0x00104868
	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x0010668C File Offset: 0x0010488C
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

	// Token: 0x06002B32 RID: 11058 RVA: 0x001066D0 File Offset: 0x001048D0
	public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
	{
		float num = Vector3.Distance(base.gameObject.transform.position, position);
		float num2 = 1f - Mathf.Clamp(num - this.minComfortRange, 0f, num / (this.triggerSize - this.minComfortRange));
		float num3 = 0f;
		foreach (BasePlayer basePlayer in this._players)
		{
			if (!(basePlayer == forPlayer))
			{
				num3 += 0.25f * (basePlayer.IsSleeping() ? 0.5f : 1f) * (basePlayer.IsAlive() ? 1f : 0f);
			}
		}
		float num4 = 0f + num3;
		return (this.baseComfort + num4) * num2;
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x001067B4 File Offset: 0x001049B4
	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Add(basePlayer);
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x001067E0 File Offset: 0x001049E0
	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (!basePlayer)
		{
			return;
		}
		this._players.Remove(basePlayer);
	}
}
