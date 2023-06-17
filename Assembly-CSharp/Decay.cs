using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public abstract class Decay : PrefabAttribute, IServerComponent
{
	// Token: 0x04001A61 RID: 6753
	private const float hours = 3600f;

	// Token: 0x06002222 RID: 8738 RVA: 0x000DCE40 File Offset: 0x000DB040
	protected float GetDecayDelay(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.delay_override > 0f)
			{
				return ConVar.Decay.delay_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.delay_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.delay_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.delay_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.delay_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.delay_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 64800f;
			case BuildingGrade.Enum.Stone:
				return 64800f;
			case BuildingGrade.Enum.Metal:
				return 64800f;
			case BuildingGrade.Enum.TopTier:
				return 86400f;
			}
		}
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x000DCEF4 File Offset: 0x000DB0F4
	protected float GetDecayDuration(BuildingGrade.Enum grade)
	{
		if (ConVar.Decay.upkeep)
		{
			if (ConVar.Decay.duration_override > 0f)
			{
				return ConVar.Decay.duration_override;
			}
			switch (grade)
			{
			default:
				return ConVar.Decay.duration_twig * 3600f;
			case BuildingGrade.Enum.Wood:
				return ConVar.Decay.duration_wood * 3600f;
			case BuildingGrade.Enum.Stone:
				return ConVar.Decay.duration_stone * 3600f;
			case BuildingGrade.Enum.Metal:
				return ConVar.Decay.duration_metal * 3600f;
			case BuildingGrade.Enum.TopTier:
				return ConVar.Decay.duration_toptier * 3600f;
			}
		}
		else
		{
			switch (grade)
			{
			default:
				return 3600f;
			case BuildingGrade.Enum.Wood:
				return 86400f;
			case BuildingGrade.Enum.Stone:
				return 172800f;
			case BuildingGrade.Enum.Metal:
				return 259200f;
			case BuildingGrade.Enum.TopTier:
				return 432000f;
			}
		}
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000DCFA8 File Offset: 0x000DB1A8
	public static void BuildingDecayTouch(BuildingBlock buildingBlock)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(buildingBlock.transform.position, 40f, list, 2097408, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			DecayEntity decayEntity = list[i];
			BuildingBlock buildingBlock2 = decayEntity as BuildingBlock;
			if (!buildingBlock2 || buildingBlock2.buildingID == buildingBlock.buildingID)
			{
				decayEntity.DecayTouch();
			}
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000DD022 File Offset: 0x000DB222
	public static void EntityLinkDecayTouch(BaseEntity ent)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		ent.EntityLinkBroadcast<DecayEntity>(delegate(DecayEntity decayEnt)
		{
			decayEnt.DecayTouch();
		});
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x000DD054 File Offset: 0x000DB254
	public static void RadialDecayTouch(Vector3 pos, float radius, int mask)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		global::Vis.Entities<DecayEntity>(pos, radius, list, mask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].DecayTouch();
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06002228 RID: 8744
	public abstract float GetDecayDelay(BaseEntity entity);

	// Token: 0x06002229 RID: 8745
	public abstract float GetDecayDuration(BaseEntity entity);

	// Token: 0x0600222A RID: 8746 RVA: 0x000DD09D File Offset: 0x000DB29D
	protected override Type GetIndexedType()
	{
		return typeof(global::Decay);
	}
}
