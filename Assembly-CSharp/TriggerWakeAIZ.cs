using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class TriggerWakeAIZ : TriggerBase, IServerComponent
{
	// Token: 0x04002354 RID: 9044
	public float SleepDelaySeconds = 30f;

	// Token: 0x04002355 RID: 9045
	public List<AIInformationZone> zones = new List<AIInformationZone>();

	// Token: 0x04002356 RID: 9046
	private AIInformationZone aiz;

	// Token: 0x06002BA9 RID: 11177 RVA: 0x00108508 File Offset: 0x00106708
	public void Init(AIInformationZone zone = null)
	{
		if (zone != null)
		{
			this.aiz = zone;
		}
		else if (this.zones == null || this.zones.Count == 0)
		{
			Transform transform = base.transform.parent;
			if (transform == null)
			{
				transform = base.transform;
			}
			this.aiz = transform.GetComponentInChildren<AIInformationZone>();
		}
		this.SetZonesSleeping(true);
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x0010856B File Offset: 0x0010676B
	private void Awake()
	{
		this.Init(null);
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x00108574 File Offset: 0x00106774
	private void SetZonesSleeping(bool flag)
	{
		if (this.aiz != null)
		{
			if (flag)
			{
				this.aiz.SleepAI();
			}
			else
			{
				this.aiz.WakeAI();
			}
		}
		if (this.zones != null && this.zones.Count > 0)
		{
			foreach (AIInformationZone aiinformationZone in this.zones)
			{
				if (aiinformationZone != null)
				{
					if (flag)
					{
						aiinformationZone.SleepAI();
					}
					else
					{
						aiinformationZone.WakeAI();
					}
				}
			}
		}
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x0010861C File Offset: 0x0010681C
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
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if (basePlayer != null && basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x0010867C File Offset: 0x0010687C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		base.CancelInvoke(new Action(this.SleepAI));
		this.SetZonesSleeping(false);
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x001086D0 File Offset: 0x001068D0
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.aiz == null && (this.zones == null || this.zones.Count == 0))
		{
			return;
		}
		if (this.entityContents == null || this.entityContents.Count == 0)
		{
			this.DelayedSleepAI();
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x00108723 File Offset: 0x00106923
	private void DelayedSleepAI()
	{
		base.CancelInvoke(new Action(this.SleepAI));
		base.Invoke(new Action(this.SleepAI), this.SleepDelaySeconds);
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x0010874F File Offset: 0x0010694F
	private void SleepAI()
	{
		this.SetZonesSleeping(true);
	}
}
