using System;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class ScarecrowNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x040012D2 RID: 4818
	public float BaseAttackRate = 2f;

	// Token: 0x040012D3 RID: 4819
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x040012D4 RID: 4820
	public static float NextBeanCanAllowedTime;

	// Token: 0x040012D5 RID: 4821
	public bool BlockClothingOnCorpse;

	// Token: 0x040012D6 RID: 4822
	public bool RoamAroundHomePoint;

	// Token: 0x06001A98 RID: 6808 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x00029E79 File Offset: 0x00028079
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06001A9B RID: 6811 RVA: 0x000BEEE6 File Offset: 0x000BD0E6
	// (set) Token: 0x06001A9C RID: 6812 RVA: 0x000BEEEE File Offset: 0x000BD0EE
	public ScarecrowBrain Brain { get; protected set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06001A9D RID: 6813 RVA: 0x000BEEF7 File Offset: 0x000BD0F7
	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Murderer;
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x000BEEFA File Offset: 0x000BD0FA
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScarecrowBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000BD109 File Offset: 0x000BB309
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000BCA6A File Offset: 0x000BAC6A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x000BEF1D File Offset: 0x000BD11D
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x000BEF3E File Offset: 0x000BD13E
	public override string Categorize()
	{
		return "Scarecrow";
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x000BEF48 File Offset: 0x000BD148
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		global::HeldEntity heldEntity = base.GetHeldEntity();
		Chainsaw chainsaw;
		if (heldEntity != null && (chainsaw = (heldEntity as Chainsaw)) != null)
		{
			chainsaw.ServerNPCStart();
		}
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x000BEF7C File Offset: 0x000BD17C
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x000BEFCB File Offset: 0x000BD1CB
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x00065288 File Offset: 0x00063488
	public bool IsTarget(global::BaseEntity entity)
	{
		return entity is global::BasePlayer && !entity.IsNpc;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool IsFriendly(global::BaseEntity entity)
	{
		return false;
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x000BEFD4 File Offset: 0x000BD1D4
	public bool CanAttack(global::BaseEntity entity)
	{
		float num;
		global::BasePlayer basePlayer;
		return !(entity == null) && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = (entity as global::BasePlayer)) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x000BF035 File Offset: 0x000BD235
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0006534C File Offset: 0x0006354C
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000BF061 File Offset: 0x000BD261
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x000BF06C File Offset: 0x000BD26C
	public bool IsOnCooldown()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		return !attackEntity || attackEntity.HasAttackCooldown();
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x000BF090 File Offset: 0x000BD290
	public bool StartAttacking(global::BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000BF0B8 File Offset: 0x000BD2B8
	private void Attack(BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.ServerPosition - this.ServerPosition;
		if (vector.magnitude > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(vector.normalized);
		}
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			attackEntity.ServerUse();
		}
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x00065437 File Offset: 0x00063637
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public global::BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x000BF118 File Offset: 0x000BD318
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			string strCorpsePrefab = "assets/prefabs/npc/murderer/murderer_corpse.prefab";
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(strCorpsePrefab) as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = "Scarecrow";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x000BF2C4 File Offset: 0x000BD4C4
	public override void Hurt(HitInfo info)
	{
		if (!info.isHeadshot)
		{
			if ((info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc) || (info.InitiatorPlayer == null && info.Initiator != null && info.Initiator.IsNpc))
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
			}
			else
			{
				info.damageTypes.ScaleAll(2f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x000BD8E8 File Offset: 0x000BBAE8
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}
}
