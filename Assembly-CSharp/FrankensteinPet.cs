using System;
using System.Collections;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class FrankensteinPet : BasePet, IAISenses, IAIAttack
{
	// Token: 0x0400076D RID: 1901
	[Header("Frankenstein")]
	[ServerVar(Help = "How long before a Frankenstein Pet dies un controlled and not asleep on table")]
	public static float decayminutes = 180f;

	// Token: 0x0400076E RID: 1902
	[Header("Audio")]
	public SoundDefinition AttackVocalSFX;

	// Token: 0x0400076F RID: 1903
	private float nextAttackTime;

	// Token: 0x06000B42 RID: 2882 RVA: 0x00065130 File Offset: 0x00063330
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinPet.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00065170 File Offset: 0x00063370
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.TickDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x000651AC File Offset: 0x000633AC
	public IEnumerator DelayEquipWeapon(ItemDefinition item, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.inventory == null)
		{
			yield break;
		}
		if (this.inventory.containerBelt == null)
		{
			yield break;
		}
		if (item == null)
		{
			yield break;
		}
		this.inventory.GiveItem(ItemManager.Create(item, 1, 0UL), this.inventory.containerBelt, false);
		this.EquipWeapon(false);
		yield break;
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000651CC File Offset: 0x000633CC
	private void TickDecay()
	{
		BasePlayer basePlayer = BasePlayer.FindByID(base.OwnerID);
		if (basePlayer != null && !basePlayer.IsSleeping())
		{
			return;
		}
		if (base.healthFraction <= 0f || base.IsDestroyed)
		{
			return;
		}
		float num = 1f / FrankensteinPet.decayminutes;
		float amount = this.MaxHealth() * num;
		base.Hurt(amount, DamageType.Decay, this, false);
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00065230 File Offset: 0x00063430
	public float EngagementRange()
	{
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * base.Brain.AttackRangeMultiplier;
		}
		return base.Brain.SenseRange;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0006527F File Offset: 0x0006347F
	public bool IsThreat(BaseEntity entity)
	{
		return this.IsTarget(entity);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00065288 File Offset: 0x00063488
	public bool IsTarget(BaseEntity entity)
	{
		return entity is BasePlayer && !entity.IsNpc;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool IsFriendly(BaseEntity entity)
	{
		return false;
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x000652A0 File Offset: 0x000634A0
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		BasePlayer basePlayer;
		return !(entity == null) && entity.gameObject.layer != 21 && entity.gameObject.layer != 8 && !this.NeedsToReload() && !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && !base.InSafeZone() && ((basePlayer = (entity as BasePlayer)) == null || !basePlayer.InSafeZone()) && this.CanSeeTarget(entity);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00065320 File Offset: 0x00063520
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.transform.position);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0006534C File Offset: 0x0006354C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00065375 File Offset: 0x00063575
	public float CooldownDuration()
	{
		return this.BaseAttackRate;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0006537D File Offset: 0x0006357D
	public bool IsOnCooldown()
	{
		return Time.realtimeSinceStartup < this.nextAttackTime;
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0006538C File Offset: 0x0006358C
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		this.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x000653B4 File Offset: 0x000635B4
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
		target.Hurt(this.BaseAttackDamge, this.AttackDamageType, this, true);
		base.SignalBroadcast(BaseEntity.Signal.Attack, null);
		base.ClientRPC(null, "OnAttack");
		this.nextAttackTime = Time.realtimeSinceStartup + this.CooldownDuration();
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00065437 File Offset: 0x00063637
	public float GetAmmoFraction()
	{
		return this.AmmoFractionRemaining();
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00065440 File Offset: 0x00063640
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse("assets/rust.ai/agents/NPCPlayer/pet/frankensteinpet_corpse.prefab") as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new ItemContainer[]
				{
					this.inventory.containerMain,
					this.inventory.containerWear,
					this.inventory.containerBelt
				});
				npcplayerCorpse.playerName = this.OverrideCorpseName();
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00065568 File Offset: 0x00063768
	protected virtual string OverrideCorpseName()
	{
		return "Frankenstein";
	}
}
