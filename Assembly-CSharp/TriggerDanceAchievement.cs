using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000581 RID: 1409
public class TriggerDanceAchievement : TriggerBase
{
	// Token: 0x04002307 RID: 8967
	public int RequiredPlayerCount = 3;

	// Token: 0x04002308 RID: 8968
	public string AchievementName;

	// Token: 0x04002309 RID: 8969
	[NonSerialized]
	private List<NetworkableId> triggeredPlayers = new List<NetworkableId>();

	// Token: 0x06002B36 RID: 11062 RVA: 0x00106833 File Offset: 0x00104A33
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x0010683B File Offset: 0x00104A3B
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x00106848 File Offset: 0x00104A48
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
		if (!(baseEntity is BasePlayer))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x00106898 File Offset: 0x00104A98
	public void NotifyDanceStarted()
	{
		if (this.entityContents == null)
		{
			return;
		}
		int num = 0;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity.ToPlayer() != null && baseEntity.ToPlayer().CurrentGestureIsDance)
			{
				num++;
				if (num >= this.RequiredPlayerCount)
				{
					break;
				}
			}
		}
		if (num >= this.RequiredPlayerCount)
		{
			foreach (BaseEntity baseEntity2 in this.entityContents)
			{
				if (!this.triggeredPlayers.Contains(baseEntity2.net.ID) && baseEntity2.ToPlayer() != null)
				{
					baseEntity2.ToPlayer().GiveAchievement(this.AchievementName);
					this.triggeredPlayers.Add(baseEntity2.net.ID);
				}
			}
		}
	}
}
