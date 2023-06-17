using System;
using Rust;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020003D0 RID: 976
public class Barricade : DecayEntity
{
	// Token: 0x04001A32 RID: 6706
	public float reflectDamage = 5f;

	// Token: 0x04001A33 RID: 6707
	public GameObjectRef reflectEffect;

	// Token: 0x04001A34 RID: 6708
	public bool canNpcSmash = true;

	// Token: 0x04001A35 RID: 6709
	public NavMeshModifierVolume NavMeshVolumeAnimals;

	// Token: 0x04001A36 RID: 6710
	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	// Token: 0x04001A37 RID: 6711
	[NonSerialized]
	public NPCBarricadeTriggerBox NpcTriggerBox;

	// Token: 0x04001A38 RID: 6712
	private static int nonWalkableArea = -1;

	// Token: 0x04001A39 RID: 6713
	private static int animalAgentTypeId = -1;

	// Token: 0x04001A3A RID: 6714
	private static int humanoidAgentTypeId = -1;

	// Token: 0x060021B5 RID: 8629 RVA: 0x000DB960 File Offset: 0x000D9B60
	public override void ServerInit()
	{
		base.ServerInit();
		if (Barricade.nonWalkableArea < 0)
		{
			Barricade.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Barricade.animalAgentTypeId < 0)
		{
			Barricade.animalAgentTypeId = NavMesh.GetSettingsByIndex(1).agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Barricade.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Barricade.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (!this.canNpcSmash)
		{
			if (Barricade.humanoidAgentTypeId < 0)
			{
				Barricade.humanoidAgentTypeId = NavMesh.GetSettingsByIndex(0).agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Barricade.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Barricade.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one;
				return;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			this.NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000DBABC File Offset: 0x000D9CBC
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (basePlayer && this.reflectDamage > 0f)
			{
				basePlayer.Hurt(this.reflectDamage * UnityEngine.Random.Range(0.75f, 1.25f), DamageType.Stab, this, true);
				if (this.reflectEffect.isValid)
				{
					Effect.server.Run(this.reflectEffect.resourcePath, basePlayer, StringPool.closest, base.transform.position, Vector3.up, null, false);
				}
			}
		}
		base.OnAttacked(info);
	}
}
