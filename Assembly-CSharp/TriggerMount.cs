using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000588 RID: 1416
public class TriggerMount : TriggerBase, IServerComponent
{
	// Token: 0x04002322 RID: 8994
	private const float MOUNT_DELAY = 3.5f;

	// Token: 0x04002323 RID: 8995
	private const float MAX_MOVE = 0.5f;

	// Token: 0x04002324 RID: 8996
	private Dictionary<BaseEntity, TriggerMount.EntryInfo> entryInfo;

	// Token: 0x06002B54 RID: 11092 RVA: 0x0010715C File Offset: 0x0010535C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity.ToPlayer();
		if (basePlayer == null || basePlayer.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x0010719C File Offset: 0x0010539C
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.entryInfo == null)
		{
			this.entryInfo = new Dictionary<BaseEntity, TriggerMount.EntryInfo>();
		}
		this.entryInfo.Add(ent, new TriggerMount.EntryInfo(Time.time, ent.transform.position));
		base.Invoke(new Action(this.CheckForMount), 3.6f);
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x001071FB File Offset: 0x001053FB
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryInfo != null)
		{
			this.entryInfo.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x00107224 File Offset: 0x00105424
	private void CheckForMount()
	{
		if (this.entityContents == null || this.entryInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseEntity, TriggerMount.EntryInfo> keyValuePair in this.entryInfo)
		{
			BaseEntity key = keyValuePair.Key;
			if (key.IsValid())
			{
				TriggerMount.EntryInfo value = keyValuePair.Value;
				BasePlayer basePlayer = key.ToPlayer();
				bool flag = (basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying;
				if (basePlayer != null && basePlayer.IsAlive() && !flag)
				{
					bool flag2 = false;
					if (!basePlayer.isMounted && !basePlayer.IsSleeping() && value.entryTime + 3.5f < Time.time && Vector3.Distance(key.transform.position, value.entryPos) < 0.5f)
					{
						BaseVehicle componentInParent = base.GetComponentInParent<BaseVehicle>();
						if (componentInParent != null && !componentInParent.IsDead())
						{
							componentInParent.AttemptMount(basePlayer, true);
							flag2 = true;
						}
					}
					if (!flag2)
					{
						value.Set(Time.time, key.transform.position);
						base.Invoke(new Action(this.CheckForMount), 3.6f);
					}
				}
			}
		}
	}

	// Token: 0x02000D5D RID: 3421
	private class EntryInfo
	{
		// Token: 0x0400471D RID: 18205
		public float entryTime;

		// Token: 0x0400471E RID: 18206
		public Vector3 entryPos;

		// Token: 0x060050CE RID: 20686 RVA: 0x001AA5ED File Offset: 0x001A87ED
		public EntryInfo(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x001AA603 File Offset: 0x001A8803
		public void Set(float entryTime, Vector3 entryPos)
		{
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}
	}
}
