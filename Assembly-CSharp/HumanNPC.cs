using System;
using System.Collections;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001F2 RID: 498
public class HumanNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	// Token: 0x040012A8 RID: 4776
	[Header("LOS")]
	public int AdditionalLosBlockingLayer;

	// Token: 0x040012A9 RID: 4777
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x040012AA RID: 4778
	[Header("Damage")]
	public float aimConeScale = 2f;

	// Token: 0x040012AB RID: 4779
	public float lastDismountTime;

	// Token: 0x040012AD RID: 4781
	[NonSerialized]
	protected bool lightsOn;

	// Token: 0x040012AE RID: 4782
	private float nextZoneSearchTime;

	// Token: 0x040012AF RID: 4783
	private AIInformationZone cachedInfoZone;

	// Token: 0x040012B0 RID: 4784
	private float targetAimedDuration;

	// Token: 0x040012B1 RID: 4785
	private float lastAimSetTime;

	// Token: 0x040012B2 RID: 4786
	private Vector3 aimOverridePosition = Vector3.zero;

	// Token: 0x06001A0D RID: 6669 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x00029E79 File Offset: 0x00028079
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06001A10 RID: 6672 RVA: 0x000BD0D5 File Offset: 0x000BB2D5
	// (set) Token: 0x06001A11 RID: 6673 RVA: 0x000BD0DD File Offset: 0x000BB2DD
	public ScientistBrain Brain { get; private set; }

	// Token: 0x06001A12 RID: 6674 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000BD0E6 File Offset: 0x000BB2E6
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<ScientistBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x000BD109 File Offset: 0x000BB309
	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x000BD117 File Offset: 0x000BB317
	public void LightCheck()
	{
		if ((TOD_Sky.Instance.IsNight && !this.lightsOn) || (TOD_Sky.Instance.IsDay && this.lightsOn))
		{
			base.LightToggle(true);
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x000BD157 File Offset: 0x000BB357
	public override float GetAimConeScale()
	{
		return this.aimConeScale;
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x000BD15F File Offset: 0x000BB35F
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x000BD168 File Offset: 0x000BB368
	public override void DismountObject()
	{
		base.DismountObject();
		this.lastDismountTime = Time.time;
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x000BD17B File Offset: 0x000BB37B
	public bool RecentlyDismounted()
	{
		return Time.time < this.lastDismountTime + 10f;
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x000BD190 File Offset: 0x000BB390
	public virtual float GetIdealDistanceFromTarget()
	{
		return Mathf.Max(5f, this.EngagementRange() * 0.75f);
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x000BD1A8 File Offset: 0x000BB3A8
	public AIInformationZone GetInformationZone(Vector3 pos)
	{
		if (this.VirtualInfoZone != null)
		{
			return this.VirtualInfoZone;
		}
		if (this.cachedInfoZone == null || Time.time > this.nextZoneSearchTime)
		{
			this.cachedInfoZone = AIInformationZone.GetForPoint(pos, true);
			this.nextZoneSearchTime = Time.time + 5f;
		}
		return this.cachedInfoZone;
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000BD20C File Offset: 0x000BB40C
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * this.Brain.AttackRangeMultiplier;
		}
		return this.Brain.SenseRange;
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x000BD25B File Offset: 0x000BB45B
	public void SetDucked(bool flag)
	{
		this.modelState.ducked = flag;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000BCA6A File Offset: 0x000BAC6A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x000BD270 File Offset: 0x000BB470
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x000BD294 File Offset: 0x000BB494
	public void TickAttack(float delta, BaseCombatEntity target, bool targetIsLOS)
	{
		if (target == null)
		{
			return;
		}
		float num = Vector3.Dot(this.eyes.BodyForward(), (target.CenterPoint() - this.eyes.position).normalized);
		if (targetIsLOS)
		{
			if (num > 0.2f)
			{
				this.targetAimedDuration += delta;
			}
		}
		else
		{
			if (num < 0.5f)
			{
				this.targetAimedDuration = 0f;
			}
			base.CancelBurst(0.2f);
		}
		if (this.targetAimedDuration >= 0.2f && targetIsLOS)
		{
			bool flag = false;
			float num2 = 0f;
			if (this != null)
			{
				flag = ((IAIAttack)this).IsTargetInRange(target, out num2);
			}
			else
			{
				AttackEntity attackEntity = base.GetAttackEntity();
				if (attackEntity)
				{
					num2 = ((target != null) ? Vector3.Distance(base.transform.position, target.transform.position) : -1f);
					flag = (num2 < attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f));
				}
			}
			if (flag)
			{
				this.ShotTest(num2);
				return;
			}
		}
		else
		{
			base.CancelBurst(0.2f);
		}
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x000BD3BC File Offset: 0x000BB5BC
	public override void Hurt(HitInfo info)
	{
		if (base.isMounted)
		{
			info.damageTypes.ScaleAll(0.1f);
		}
		base.Hurt(info);
		global::BaseEntity initiator = info.Initiator;
		if (initiator != null && !initiator.EqualNetID(this))
		{
			this.Brain.Senses.Memory.SetKnown(initiator, this, null);
		}
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x000BD419 File Offset: 0x000BB619
	public float GetAimSwayScalar()
	{
		return 1f - Mathf.InverseLerp(1f, 3f, Time.time - this.lastGunShotTime);
	}

	// Token: 0x06001A23 RID: 6691 RVA: 0x000BD43C File Offset: 0x000BB63C
	public override Vector3 GetAimDirection()
	{
		if (this.Brain != null && this.Brain.Navigator != null && this.Brain.Navigator.IsOverridingFacingDirection)
		{
			return this.Brain.Navigator.FacingDirectionOverride;
		}
		return base.GetAimDirection();
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x000BD494 File Offset: 0x000BB694
	public override void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		float num = Time.time - this.lastAimSetTime;
		this.lastAimSetTime = Time.time;
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, this.GetAimSwayScalar());
		}
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			Vector3 eulerAngles = mounted.transform.eulerAngles;
			Quaternion rotation = Quaternion.Euler(Quaternion.LookRotation(newAim, mounted.transform.up).eulerAngles);
			Vector3 vector = Quaternion.LookRotation(base.transform.InverseTransformDirection(rotation * Vector3.forward), base.transform.up).eulerAngles;
			vector = BaseMountable.ConvertVector(vector);
			Quaternion rotation2 = Quaternion.Euler(Mathf.Clamp(vector.x, mounted.pitchClamp.x, mounted.pitchClamp.y), Mathf.Clamp(vector.y, mounted.yawClamp.x, mounted.yawClamp.y), eulerAngles.z);
			newAim = BaseMountable.ConvertVector(Quaternion.LookRotation(base.transform.TransformDirection(rotation2 * Vector3.forward), base.transform.up).eulerAngles);
		}
		else
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				Vector3 vector2 = parentEntity.transform.InverseTransformDirection(newAim);
				Vector3 forward = new Vector3(newAim.x, vector2.y, newAim.z);
				this.eyes.rotation = Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(forward, parentEntity.transform.up), num * 25f);
				this.viewAngles = this.eyes.bodyRotation.eulerAngles;
				this.ServerRotation = this.eyes.bodyRotation;
				return;
			}
		}
		this.eyes.rotation = (base.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), num * 70f) : Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(newAim, base.transform.up), num * 25f));
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x000BD70F File Offset: 0x000BB90F
	public void SetStationaryAimPoint(Vector3 aimAt)
	{
		this.aimOverridePosition = aimAt;
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x000BD718 File Offset: 0x000BB918
	public void ClearStationaryAimPoint()
	{
		this.aimOverridePosition = Vector3.zero;
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x000BD728 File Offset: 0x000BB928
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
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
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				npcplayerCorpse.TakeChildren(this);
				for (int i = 0; i < npcplayerCorpse.containers.Length; i++)
				{
					global::ItemContainer itemContainer = npcplayerCorpse.containers[i];
					if (i != 1)
					{
						itemContainer.Clear();
					}
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int k = 0; k < lootSpawnSlot.numberToSpawn; k++)
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

	// Token: 0x06001A29 RID: 6697 RVA: 0x000BD8E0 File Offset: 0x000BBAE0
	protected virtual string OverrideCorpseName()
	{
		return base.displayName;
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x000BD8E8 File Offset: 0x000BBAE8
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x000BD91E File Offset: 0x000BBB1E
	public bool IsThreat(global::BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000BD927 File Offset: 0x000BBB27
	public bool IsTarget(global::BaseEntity entity)
	{
		return (entity is global::BasePlayer && !entity.IsNpc) || entity is BasePet || entity is ScarecrowNPC;
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000BD950 File Offset: 0x000BBB50
	public bool IsFriendly(global::BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanAttack(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x000BD96B File Offset: 0x000BBB6B
	public bool IsTargetInRange(global::BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x000BD998 File Offset: 0x000BBB98
	public bool CanSeeTarget(global::BaseEntity entity)
	{
		global::BasePlayer basePlayer = entity as global::BasePlayer;
		if (basePlayer == null)
		{
			return true;
		}
		if (this.AdditionalLosBlockingLayer == 0)
		{
			return base.IsPlayerVisibleToUs(basePlayer, 1218519041);
		}
		return base.IsPlayerVisibleToUs(basePlayer, 1218519041 | 1 << this.AdditionalLosBlockingLayer);
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x000BD9E4 File Offset: 0x000BBBE4
	public float CooldownDuration()
	{
		return 5f;
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool IsOnCooldown()
	{
		return false;
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x0000441C File Offset: 0x0000261C
	public bool StartAttacking(global::BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x00065437 File Offset: 0x00063637
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x000BD9EC File Offset: 0x000BBBEC
	public global::BaseEntity GetBestTarget()
	{
		global::BaseEntity result = null;
		float num = -1f;
		foreach (global::BaseEntity baseEntity in this.Brain.Senses.Players)
		{
			if (!(baseEntity == null) && baseEntity.Health() > 0f)
			{
				float value = Vector3.Distance(baseEntity.transform.position, base.transform.position);
				float num2 = 1f - Mathf.InverseLerp(1f, this.Brain.SenseRange, value);
				float value2 = Vector3.Dot((baseEntity.transform.position - this.eyes.position).normalized, this.eyes.BodyForward());
				num2 += Mathf.InverseLerp(this.Brain.VisionCone, 1f, value2) / 2f;
				num2 += (this.Brain.Senses.Memory.IsLOS(baseEntity) ? 2f : 0f);
				if (num2 > num)
				{
					result = baseEntity;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x000BDB34 File Offset: 0x000BBD34
	public void AttackTick(float delta, global::BaseEntity target, bool targetIsLOS)
	{
		BaseCombatEntity target2 = target as BaseCombatEntity;
		this.TickAttack(delta, target2, targetIsLOS);
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x000BDB51 File Offset: 0x000BBD51
	public void UseHealingItem(global::Item item)
	{
		base.StartCoroutine(this.Heal(item));
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x000BDB61 File Offset: 0x000BBD61
	private IEnumerator Heal(global::Item item)
	{
		base.UpdateActiveItem(item.uid);
		global::Item activeItem = base.GetActiveItem();
		MedicalTool heldItem = activeItem.GetHeldEntity() as MedicalTool;
		if (heldItem == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(1f);
		heldItem.ServerUse();
		this.Heal(this.MaxHealth());
		yield return new WaitForSeconds(2f);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x000BDB78 File Offset: 0x000BBD78
	public global::Item FindHealingItem()
	{
		if (this.Brain == null)
		{
			return null;
		}
		if (!this.Brain.CanUseHealingItems)
		{
			return null;
		}
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			global::Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.amount > 1 && slot.GetHeldEntity() as MedicalTool != null)
			{
				return slot;
			}
		}
		return null;
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsOnGround()
	{
		return true;
	}
}
