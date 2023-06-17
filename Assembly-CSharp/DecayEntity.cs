using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class DecayEntity : BaseCombatEntity
{
	// Token: 0x04001A62 RID: 6754
	public GameObjectRef debrisPrefab;

	// Token: 0x04001A63 RID: 6755
	public Vector3 debrisRotationOffset = Vector3.zero;

	// Token: 0x04001A64 RID: 6756
	[NonSerialized]
	public uint buildingID;

	// Token: 0x04001A65 RID: 6757
	private float decayTimer;

	// Token: 0x04001A66 RID: 6758
	private float upkeepTimer;

	// Token: 0x04001A67 RID: 6759
	private Upkeep upkeep;

	// Token: 0x04001A68 RID: 6760
	private global::Decay decay;

	// Token: 0x04001A69 RID: 6761
	private DecayPoint[] decayPoints;

	// Token: 0x04001A6A RID: 6762
	private float lastDecayTick;

	// Token: 0x04001A6B RID: 6763
	private float decayVariance = 1f;

	// Token: 0x0600222C RID: 8748 RVA: 0x000DD0AC File Offset: 0x000DB2AC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.decayEntity = Facepunch.Pool.Get<ProtoBuf.DecayEntity>();
		info.msg.decayEntity.buildingID = this.buildingID;
		if (info.forDisk)
		{
			info.msg.decayEntity.decayTimer = this.decayTimer;
			info.msg.decayEntity.upkeepTimer = this.upkeepTimer;
		}
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000DD11C File Offset: 0x000DB31C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity != null)
		{
			this.decayTimer = info.msg.decayEntity.decayTimer;
			this.upkeepTimer = info.msg.decayEntity.upkeepTimer;
			if (this.buildingID != info.msg.decayEntity.buildingID)
			{
				this.AttachToBuilding(info.msg.decayEntity.buildingID);
				if (info.fromDisk)
				{
					BuildingManager.server.LoadBuildingID(this.buildingID);
				}
			}
		}
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000DD1AF File Offset: 0x000DB3AF
	public override void ResetState()
	{
		base.ResetState();
		this.buildingID = 0U;
		if (base.isServer)
		{
			this.decayTimer = 0f;
		}
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000DD1D1 File Offset: 0x000DB3D1
	public void AttachToBuilding(uint id)
	{
		if (base.isServer)
		{
			BuildingManager.server.Remove(this);
			this.buildingID = id;
			BuildingManager.server.Add(this);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000DD1FF File Offset: 0x000DB3FF
	public BuildingManager.Building GetBuilding()
	{
		if (base.isServer)
		{
			return BuildingManager.server.GetBuilding(this.buildingID);
		}
		return null;
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x000DD21C File Offset: 0x000DB41C
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = this.GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000DD240 File Offset: 0x000DB440
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (this.upkeep == null)
		{
			return;
		}
		float num = this.upkeep.upkeepMultiplier * multiplier;
		if (num == 0f)
		{
			return;
		}
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category == ItemCategory.Resources)
			{
				float num2 = itemAmount.amount * num;
				bool flag = false;
				foreach (ItemAmount itemAmount2 in itemAmounts)
				{
					if (itemAmount2.itemDef == itemAmount.itemDef)
					{
						itemAmount2.amount += num2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					itemAmounts.Add(new ItemAmount(itemAmount.itemDef, num2));
				}
			}
		}
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000DD354 File Offset: 0x000DB554
	public override void ServerInit()
	{
		base.ServerInit();
		this.decayVariance = UnityEngine.Random.Range(0.95f, 1f);
		this.decay = PrefabAttribute.server.Find<global::Decay>(this.prefabID);
		this.decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(this.prefabID);
		this.upkeep = PrefabAttribute.server.Find<Upkeep>(this.prefabID);
		BuildingManager.server.Add(this);
		if (!Rust.Application.isLoadingSave)
		{
			BuildingManager.server.CheckMerge(this);
		}
		this.lastDecayTick = UnityEngine.Time.time;
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000DD3E6 File Offset: 0x000DB5E6
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BuildingManager.server.Remove(this);
		BuildingManager.server.CheckSplit(this);
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000DD404 File Offset: 0x000DB604
	public virtual void AttachToBuilding(global::DecayEntity other)
	{
		if (other != null)
		{
			this.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		global::BuildingBlock nearbyBuildingBlock = this.GetNearbyBuildingBlock();
		if (nearbyBuildingBlock)
		{
			this.AttachToBuilding(nearbyBuildingBlock.buildingID);
		}
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000DD450 File Offset: 0x000DB650
	public global::BuildingBlock GetNearbyBuildingBlock()
	{
		float num = float.MaxValue;
		global::BuildingBlock result = null;
		Vector3 position = base.PivotPoint();
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(position, 1.5f, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer)
			{
				float num2 = buildingBlock.SqrDistance(position);
				if (!buildingBlock.grounded)
				{
					num2 += 1f;
				}
				if (num2 < num)
				{
					num = num2;
					result = buildingBlock;
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return result;
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000DD4E2 File Offset: 0x000DB6E2
	public void ResetUpkeepTime()
	{
		this.upkeepTimer = 0f;
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000DD4EF File Offset: 0x000DB6EF
	public void DecayTouch()
	{
		this.decayTimer = 0f;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000DD4FC File Offset: 0x000DB6FC
	public void AddUpkeepTime(float time)
	{
		this.upkeepTimer -= time;
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000DD50C File Offset: 0x000DB70C
	public float GetProtectedSeconds()
	{
		return Mathf.Max(0f, -this.upkeepTimer);
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000DD520 File Offset: 0x000DB720
	public virtual void DecayTick()
	{
		if (this.decay == null)
		{
			return;
		}
		float num = UnityEngine.Time.time - this.lastDecayTick;
		if (num < ConVar.Decay.tick)
		{
			return;
		}
		this.lastDecayTick = UnityEngine.Time.time;
		if (!this.decay.ShouldDecay(this))
		{
			return;
		}
		float num2 = num * ConVar.Decay.scale;
		if (ConVar.Decay.upkeep)
		{
			this.upkeepTimer += num2;
			if (this.upkeepTimer > 0f)
			{
				BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
				if (buildingPrivilege != null)
				{
					this.upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(this.upkeepTimer, 600f));
				}
			}
			if (this.upkeepTimer < 1f)
			{
				if (base.healthFraction < 1f && ConVar.Decay.upkeep_heal_scale > 0f && base.SecondsSinceAttacked > 600f)
				{
					float num3 = num / this.decay.GetDecayDuration(this) * ConVar.Decay.upkeep_heal_scale;
					this.Heal(this.MaxHealth() * num3);
				}
				return;
			}
			this.upkeepTimer = 1f;
		}
		this.decayTimer += num2;
		if (this.decayTimer < this.decay.GetDecayDelay(this))
		{
			return;
		}
		using (TimeWarning.New("DecayTick", 0))
		{
			float num4 = 1f;
			if (ConVar.Decay.upkeep)
			{
				if (!this.BypassInsideDecayMultiplier && !this.IsOutside())
				{
					num4 *= ConVar.Decay.upkeep_inside_decay_scale;
				}
			}
			else
			{
				for (int i = 0; i < this.decayPoints.Length; i++)
				{
					DecayPoint decayPoint = this.decayPoints[i];
					if (decayPoint.IsOccupied(this))
					{
						num4 -= decayPoint.protection;
					}
				}
			}
			if (num4 > 0f)
			{
				float num5 = num2 / this.decay.GetDecayDuration(this) * this.MaxHealth();
				base.Hurt(num5 * num4 * this.decayVariance, DamageType.Decay, null, true);
			}
		}
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000DD718 File Offset: 0x000DB918
	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		this.DecayTouch();
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000DD728 File Offset: 0x000DB928
	public override void OnKilled(HitInfo info)
	{
		if (this.debrisPrefab.isValid)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisPrefab.resourcePath, base.transform.position, base.transform.rotation * Quaternion.Euler(this.debrisRotationOffset), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x0600223E RID: 8766 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool BypassInsideDecayMultiplier
	{
		get
		{
			return false;
		}
	}
}
