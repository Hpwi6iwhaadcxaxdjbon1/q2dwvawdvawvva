using System;
using ConVar;
using UnityEngine;

// Token: 0x020003AA RID: 938
public class AttackEntity : HeldEntity
{
	// Token: 0x040019D1 RID: 6609
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	// Token: 0x040019D2 RID: 6610
	public float repeatDelay = 0.5f;

	// Token: 0x040019D3 RID: 6611
	public float animationDelay;

	// Token: 0x040019D4 RID: 6612
	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	// Token: 0x040019D5 RID: 6613
	public float npcDamageScale = 1f;

	// Token: 0x040019D6 RID: 6614
	public float attackLengthMin = -1f;

	// Token: 0x040019D7 RID: 6615
	public float attackLengthMax = -1f;

	// Token: 0x040019D8 RID: 6616
	public float attackSpacing;

	// Token: 0x040019D9 RID: 6617
	public float aiAimSwayOffset;

	// Token: 0x040019DA RID: 6618
	public float aiAimCone;

	// Token: 0x040019DB RID: 6619
	public bool aiOnlyInRange;

	// Token: 0x040019DC RID: 6620
	public float CloseRangeAddition;

	// Token: 0x040019DD RID: 6621
	public float MediumRangeAddition;

	// Token: 0x040019DE RID: 6622
	public float LongRangeAddition;

	// Token: 0x040019DF RID: 6623
	public bool CanUseAtMediumRange = true;

	// Token: 0x040019E0 RID: 6624
	public bool CanUseAtLongRange = true;

	// Token: 0x040019E1 RID: 6625
	public SoundDefinition[] reloadSounds;

	// Token: 0x040019E2 RID: 6626
	public SoundDefinition thirdPersonMeleeSound;

	// Token: 0x040019E3 RID: 6627
	[Header("Recoil Compensation")]
	public float recoilCompDelayOverride;

	// Token: 0x040019E4 RID: 6628
	public bool wantsRecoilComp;

	// Token: 0x040019E5 RID: 6629
	private float nextAttackTime = float.NegativeInfinity;

	// Token: 0x060020F0 RID: 8432 RVA: 0x0002BE49 File Offset: 0x0002A049
	public virtual Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return Vector3.zero;
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float AmmoFraction()
	{
		return 0f;
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool CanReload()
	{
		return false;
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool ServerIsReloading()
	{
		return false;
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerReload()
	{
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void TopUpAmmo()
	{
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		return eulerInput;
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x060020F7 RID: 8439 RVA: 0x000D893C File Offset: 0x000D6B3C
	protected bool UsingInfiniteAmmoCheat
	{
		get
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			return !(ownerPlayer == null) && (ownerPlayer.IsAdmin || ownerPlayer.IsDeveloper) && ownerPlayer.GetInfoBool("player.infiniteammo", false);
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x060020F8 RID: 8440 RVA: 0x000D8977 File Offset: 0x000D6B77
	public float NextAttackTime
	{
		get
		{
			return this.nextAttackTime;
		}
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void GetAttackStats(HitInfo info)
	{
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000D897F File Offset: 0x000D6B7F
	protected void StartAttackCooldownRaw(float cooldown)
	{
		this.nextAttackTime = UnityEngine.Time.time + cooldown;
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000D898E File Offset: 0x000D6B8E
	protected void StartAttackCooldown(float cooldown)
	{
		this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000D89A4 File Offset: 0x000D6BA4
	public void ResetAttackCooldown()
	{
		this.nextAttackTime = float.NegativeInfinity;
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000D89B1 File Offset: 0x000D6BB1
	public bool HasAttackCooldown()
	{
		return UnityEngine.Time.time < this.nextAttackTime;
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x000D89C0 File Offset: 0x000D6BC0
	protected float GetAttackCooldown()
	{
		return Mathf.Max(this.nextAttackTime - UnityEngine.Time.time, 0f);
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000D89D8 File Offset: 0x000D6BD8
	protected float GetAttackIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextAttackTime, 0f);
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000D89F0 File Offset: 0x000D6BF0
	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float time = UnityEngine.Time.time;
		float num = 0f;
		if (base.isServer)
		{
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			num += 0.1f;
			num += cooldown * 0.1f;
			num += (ownerPlayer ? ownerPlayer.desyncTimeClamped : 0.1f);
			num += Mathf.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime);
		}
		if (nextTime < 0f)
		{
			nextTime = Mathf.Max(0f, time + cooldown - num);
		}
		else if (time - nextTime <= num)
		{
			nextTime = Mathf.Min(nextTime + cooldown, time + cooldown);
		}
		else
		{
			nextTime = Mathf.Max(nextTime + cooldown, time + cooldown - num);
		}
		return nextTime;
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x000D8A94 File Offset: 0x000D6C94
	protected bool VerifyClientRPC(BasePlayer player)
	{
		if (player == null)
		{
			Debug.LogWarning("Received RPC from null player");
			return false;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Owner not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "owner_missing");
			return false;
		}
		if (ownerPlayer != player)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_mismatch");
			return false;
		}
		if (player.IsDead())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player dead (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_dead");
			return false;
		}
		if (player.IsWounded())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player down (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_down");
			return false;
		}
		if (player.IsSleeping())
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Player sleeping (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_sleeping");
			return false;
		}
		if (player.desyncTimeRaw > ConVar.AntiHack.maxdesync)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, string.Concat(new object[]
			{
				"Player stalled (",
				base.ShortPrefabName,
				" with ",
				player.desyncTimeRaw,
				"s)"
			}));
			player.stats.combat.LogInvalid(player, this, "player_stalled");
			return false;
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_missing");
			return false;
		}
		if (ownerItem.isBroken)
		{
			global::AntiHack.Log(player, AntiHackType.AttackHack, "Item broken (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_broken");
			return false;
		}
		return true;
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000D8CD4 File Offset: 0x000D6ED4
	protected virtual bool VerifyClientAttack(BasePlayer player)
	{
		if (!this.VerifyClientRPC(player))
		{
			return false;
		}
		if (this.HasAttackCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.CooldownHack, string.Concat(new object[]
			{
				"T-",
				this.GetAttackCooldown(),
				"s (",
				base.ShortPrefabName,
				")"
			}));
			player.stats.combat.LogInvalid(player, this, "attack_cooldown");
			return false;
		}
		return true;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000D8D50 File Offset: 0x000D6F50
	protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos)
	{
		bool flag = true;
		if (eyePos.IsNaNOrInfinity())
		{
			string shortPrefabName = base.ShortPrefabName;
			global::AntiHack.Log(player, AntiHackType.EyeHack, "Contains NaN (" + shortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "eye_nan");
			flag = false;
		}
		if (ConVar.AntiHack.eye_protection > 0)
		{
			float num = 1f + ConVar.AntiHack.eye_forgiveness;
			float eye_clientframes = ConVar.AntiHack.eye_clientframes;
			float eye_serverframes = ConVar.AntiHack.eye_serverframes;
			float num2 = eye_clientframes / 60f;
			float num3 = eye_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float num4 = (player.desyncTimeClamped + num2 + num3) * num;
			int layerMask = ConVar.AntiHack.eye_terraincheck ? 10551296 : 2162688;
			if (ConVar.AntiHack.eye_protection >= 1)
			{
				float num5 = player.MaxVelocity() + player.GetParentVelocity().magnitude;
				float num6 = player.BoundsPadding() + num4 * num5;
				float num7 = Vector3.Distance(player.eyes.position, eyePos);
				if (num7 > num6)
				{
					string shortPrefabName2 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Distance (",
						shortPrefabName2,
						" on attack with ",
						num7,
						"m > ",
						num6,
						"m)"
					}));
					player.stats.combat.LogInvalid(player, this, "eye_distance");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 3)
			{
				float num8 = Mathf.Abs(player.GetMountVelocity().y + player.GetParentVelocity().y);
				float num9 = player.BoundsPadding() + num4 * num8 + player.GetJumpHeight();
				float num10 = Mathf.Abs(player.eyes.position.y - eyePos.y);
				if (num10 > num9)
				{
					string shortPrefabName3 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Altitude (",
						shortPrefabName3,
						" on attack with ",
						num10,
						"m > ",
						num9,
						"m)"
					}));
					player.stats.combat.LogInvalid(player, this, "eye_altitude");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 2)
			{
				Vector3 center = player.eyes.center;
				Vector3 position = player.eyes.position;
				if (!GamePhysics.LineOfSightRadius(center, position, layerMask, ConVar.AntiHack.eye_losradius, null) || !GamePhysics.LineOfSightRadius(position, eyePos, layerMask, ConVar.AntiHack.eye_losradius, null))
				{
					string shortPrefabName4 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"Line of sight (",
						shortPrefabName4,
						" on attack) ",
						center,
						" ",
						position,
						" ",
						eyePos
					}));
					player.stats.combat.LogInvalid(player, this, "eye_los");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 4 && !player.HasParent())
			{
				Vector3 position2 = player.eyes.position;
				float num11 = Vector3.Distance(position2, eyePos);
				Collider collider;
				if (num11 > ConVar.AntiHack.eye_noclip_cutoff)
				{
					if (global::AntiHack.TestNoClipping(position2, eyePos, player.NoClipRadius(ConVar.AntiHack.eye_noclip_margin), ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider, false, null))
					{
						string shortPrefabName5 = base.ShortPrefabName;
						global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
						{
							"NoClip (",
							shortPrefabName5,
							" on attack) ",
							position2,
							" ",
							eyePos
						}));
						player.stats.combat.LogInvalid(player, this, "eye_noclip");
						flag = false;
					}
				}
				else if (num11 > 0.01f && global::AntiHack.TestNoClipping(position2, eyePos, 0.01f, ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider, false, null))
				{
					string shortPrefabName6 = base.ShortPrefabName;
					global::AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(new object[]
					{
						"NoClip (",
						shortPrefabName6,
						" on attack) ",
						position2,
						" ",
						eyePos
					}));
					player.stats.combat.LogInvalid(player, this, "eye_noclip");
					flag = false;
				}
			}
			if (!flag)
			{
				global::AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
			}
			else if (ConVar.AntiHack.eye_protection >= 5 && !player.HasParent() && !player.isMounted)
			{
				player.eyeHistory.PushBack(eyePos);
			}
		}
		return flag;
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000D91ED File Offset: 0x000D73ED
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StartAttackCooldown(this.deployDelay * 0.9f);
	}
}
