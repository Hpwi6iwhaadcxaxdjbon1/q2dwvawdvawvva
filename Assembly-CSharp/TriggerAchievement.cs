using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200057C RID: 1404
public class TriggerAchievement : TriggerBase
{
	// Token: 0x040022F6 RID: 8950
	public string statToIncrease = "";

	// Token: 0x040022F7 RID: 8951
	public string achievementOnEnter = "";

	// Token: 0x040022F8 RID: 8952
	public string requiredVehicleName = "";

	// Token: 0x040022F9 RID: 8953
	[Tooltip("Always set to true, clientside does not work, currently")]
	public bool serverSide = true;

	// Token: 0x040022FA RID: 8954
	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();

	// Token: 0x06002B11 RID: 11025 RVA: 0x00105D57 File Offset: 0x00103F57
	public void OnPuzzleReset()
	{
		this.Reset();
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x00105D5F File Offset: 0x00103F5F
	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x00105D6C File Offset: 0x00103F6C
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
		if (baseEntity.isClient && this.serverSide)
		{
			return null;
		}
		if (baseEntity.isServer && !this.serverSide)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x00105DCC File Offset: 0x00103FCC
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		BasePlayer component = ent.GetComponent<BasePlayer>();
		if (component == null || !component.IsAlive() || component.IsSleeping() || component.IsNpc)
		{
			return;
		}
		if (this.triggeredPlayers.Contains(component.userID))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.requiredVehicleName))
		{
			BaseVehicle mountedVehicle = component.GetMountedVehicle();
			if (mountedVehicle == null)
			{
				return;
			}
			if (!mountedVehicle.ShortPrefabName.Contains(this.requiredVehicleName))
			{
				return;
			}
		}
		if (this.serverSide)
		{
			if (!string.IsNullOrEmpty(this.achievementOnEnter))
			{
				component.GiveAchievement(this.achievementOnEnter);
			}
			if (!string.IsNullOrEmpty(this.statToIncrease))
			{
				component.stats.Add(this.statToIncrease, 1, Stats.Steam);
				component.stats.Save(true);
			}
			this.triggeredPlayers.Add(component.userID);
		}
	}
}
