using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class AIBrainSenses
{
	// Token: 0x040018C3 RID: 6339
	[ServerVar]
	public static float UpdateInterval = 0.5f;

	// Token: 0x040018C4 RID: 6340
	[ServerVar]
	public static float HumanKnownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040018C5 RID: 6341
	[ServerVar]
	public static float KnownPlayersLOSUpdateInterval = 0.5f;

	// Token: 0x040018C6 RID: 6342
	private float knownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040018C7 RID: 6343
	public float MemoryDuration = 10f;

	// Token: 0x040018C8 RID: 6344
	public float LastThreatTimestamp;

	// Token: 0x040018C9 RID: 6345
	public float TimeInAgressiveState;

	// Token: 0x040018CB RID: 6347
	private static BaseEntity[] queryResults = new BaseEntity[64];

	// Token: 0x040018CC RID: 6348
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x040018CD RID: 6349
	private float nextUpdateTime;

	// Token: 0x040018CE RID: 6350
	private float nextKnownPlayersLOSUpdateTime;

	// Token: 0x040018CF RID: 6351
	private BaseEntity owner;

	// Token: 0x040018D0 RID: 6352
	private BasePlayer playerOwner;

	// Token: 0x040018D1 RID: 6353
	private IAISenses ownerSenses;

	// Token: 0x040018D2 RID: 6354
	private float maxRange;

	// Token: 0x040018D3 RID: 6355
	private float targetLostRange;

	// Token: 0x040018D4 RID: 6356
	private float visionCone;

	// Token: 0x040018D5 RID: 6357
	private bool checkVision;

	// Token: 0x040018D6 RID: 6358
	private bool checkLOS;

	// Token: 0x040018D7 RID: 6359
	private bool ignoreNonVisionSneakers;

	// Token: 0x040018D8 RID: 6360
	private float listenRange;

	// Token: 0x040018D9 RID: 6361
	private bool hostileTargetsOnly;

	// Token: 0x040018DA RID: 6362
	private bool senseFriendlies;

	// Token: 0x040018DB RID: 6363
	private bool refreshKnownLOS;

	// Token: 0x040018DD RID: 6365
	private EntityType senseTypes;

	// Token: 0x040018DE RID: 6366
	private IAIAttack ownerAttack;

	// Token: 0x040018DF RID: 6367
	public BaseAIBrain brain;

	// Token: 0x040018E0 RID: 6368
	private Func<BaseEntity, bool> aiCaresAbout;

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x06001F59 RID: 8025 RVA: 0x000D4041 File Offset: 0x000D2241
	public float TimeSinceThreat
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.LastThreatTimestamp;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x06001F5A RID: 8026 RVA: 0x000D404F File Offset: 0x000D224F
	// (set) Token: 0x06001F5B RID: 8027 RVA: 0x000D4057 File Offset: 0x000D2257
	public SimpleAIMemory Memory { get; private set; } = new SimpleAIMemory();

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x06001F5C RID: 8028 RVA: 0x000D4060 File Offset: 0x000D2260
	public float TargetLostRange
	{
		get
		{
			return this.targetLostRange;
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x06001F5D RID: 8029 RVA: 0x000D4068 File Offset: 0x000D2268
	// (set) Token: 0x06001F5E RID: 8030 RVA: 0x000D4070 File Offset: 0x000D2270
	public bool ignoreSafeZonePlayers { get; private set; }

	// Token: 0x06001F5F RID: 8031 RVA: 0x000D407C File Offset: 0x000D227C
	public void Init(BaseEntity owner, BaseAIBrain brain, float memoryDuration, float range, float targetLostRange, float visionCone, bool checkVision, bool checkLOS, bool ignoreNonVisionSneakers, float listenRange, bool hostileTargetsOnly, bool senseFriendlies, bool ignoreSafeZonePlayers, EntityType senseTypes, bool refreshKnownLOS)
	{
		this.aiCaresAbout = new Func<BaseEntity, bool>(this.AiCaresAbout);
		this.owner = owner;
		this.brain = brain;
		this.MemoryDuration = memoryDuration;
		this.ownerAttack = (owner as IAIAttack);
		this.playerOwner = (owner as BasePlayer);
		this.maxRange = range;
		this.targetLostRange = targetLostRange;
		this.visionCone = visionCone;
		this.checkVision = checkVision;
		this.checkLOS = checkLOS;
		this.ignoreNonVisionSneakers = ignoreNonVisionSneakers;
		this.listenRange = listenRange;
		this.hostileTargetsOnly = hostileTargetsOnly;
		this.senseFriendlies = senseFriendlies;
		this.ignoreSafeZonePlayers = ignoreSafeZonePlayers;
		this.senseTypes = senseTypes;
		this.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
		this.refreshKnownLOS = refreshKnownLOS;
		this.ownerSenses = (owner as IAISenses);
		this.knownPlayersLOSUpdateInterval = ((owner is HumanNPC) ? AIBrainSenses.HumanKnownPlayersLOSUpdateInterval : AIBrainSenses.KnownPlayersLOSUpdateInterval);
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000D4159 File Offset: 0x000D2359
	public void DelaySenseUpdate(float delay)
	{
		this.nextUpdateTime = UnityEngine.Time.time + delay;
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000D4168 File Offset: 0x000D2368
	public void Update()
	{
		if (this.owner == null)
		{
			return;
		}
		this.UpdateSenses();
		this.UpdateKnownPlayersLOS();
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000D4188 File Offset: 0x000D2388
	private void UpdateSenses()
	{
		if (UnityEngine.Time.time < this.nextUpdateTime)
		{
			return;
		}
		this.nextUpdateTime = UnityEngine.Time.time + AIBrainSenses.UpdateInterval;
		if (this.senseTypes != (EntityType)0)
		{
			if (this.senseTypes == EntityType.Player)
			{
				this.SensePlayers();
			}
			else
			{
				this.SenseBrains();
				if (this.senseTypes.HasFlag(EntityType.Player))
				{
					this.SensePlayers();
				}
			}
		}
		this.Memory.Forget(this.MemoryDuration);
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000D4204 File Offset: 0x000D2404
	public void UpdateKnownPlayersLOS()
	{
		if (UnityEngine.Time.time < this.nextKnownPlayersLOSUpdateTime)
		{
			return;
		}
		this.nextKnownPlayersLOSUpdateTime = UnityEngine.Time.time + this.knownPlayersLOSUpdateInterval;
		foreach (BaseEntity baseEntity in this.Memory.Players)
		{
			if (!(baseEntity == null) && !baseEntity.IsNpc)
			{
				bool flag = this.ownerAttack.CanSeeTarget(baseEntity);
				this.Memory.SetLOS(baseEntity, flag);
				if (this.refreshKnownLOS && this.owner != null && flag && Vector3.Distance(baseEntity.transform.position, this.owner.transform.position) <= this.TargetLostRange)
				{
					this.Memory.SetKnown(baseEntity, this.owner, this);
				}
			}
		}
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000D42FC File Offset: 0x000D24FC
	private void SensePlayers()
	{
		int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.playerQueryResults, this.aiCaresAbout);
		for (int i = 0; i < playersInSphere; i++)
		{
			BasePlayer ent = AIBrainSenses.playerQueryResults[i];
			this.Memory.SetKnown(ent, this.owner, this);
		}
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000D435C File Offset: 0x000D255C
	private void SenseBrains()
	{
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.queryResults, this.aiCaresAbout);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity ent = AIBrainSenses.queryResults[i];
			this.Memory.SetKnown(ent, this.owner, this);
		}
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000D43BC File Offset: 0x000D25BC
	private bool AiCaresAbout(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (!entity.isServer)
		{
			return false;
		}
		if (entity.EqualNetID(this.owner))
		{
			return false;
		}
		if (entity.Health() <= 0f)
		{
			return false;
		}
		if (!this.IsValidSenseType(entity))
		{
			return false;
		}
		BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
		BasePlayer basePlayer = entity as BasePlayer;
		if (basePlayer != null && basePlayer.IsDead())
		{
			return false;
		}
		if (this.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			return false;
		}
		if (this.listenRange > 0f && baseCombatEntity != null && baseCombatEntity.TimeSinceLastNoise <= 1f && baseCombatEntity.CanLastNoiseBeHeard(this.owner.transform.position, this.listenRange))
		{
			return true;
		}
		if (this.senseFriendlies && this.ownerSenses != null && this.ownerSenses.IsFriendly(entity))
		{
			return true;
		}
		float num = float.PositiveInfinity;
		if (baseCombatEntity != null && AI.accuratevisiondistance)
		{
			num = Vector3.Distance(this.owner.transform.position, baseCombatEntity.transform.position);
			if (num > this.maxRange)
			{
				return false;
			}
		}
		if (this.checkVision && !this.IsTargetInVision(entity))
		{
			if (!this.ignoreNonVisionSneakers)
			{
				return false;
			}
			if (basePlayer != null && !basePlayer.IsNpc)
			{
				if (!AI.accuratevisiondistance)
				{
					num = Vector3.Distance(this.owner.transform.position, basePlayer.transform.position);
				}
				if ((basePlayer.IsDucked() && num >= this.brain.IgnoreSneakersMaxDistance) || num >= this.brain.IgnoreNonVisionMaxDistance)
				{
					return false;
				}
			}
		}
		if (this.hostileTargetsOnly && baseCombatEntity != null && !baseCombatEntity.IsHostile() && !(baseCombatEntity is ScarecrowNPC))
		{
			return false;
		}
		if (this.checkLOS && this.ownerAttack != null)
		{
			bool flag = this.ownerAttack.CanSeeTarget(entity);
			this.Memory.SetLOS(entity, flag);
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000D45B8 File Offset: 0x000D27B8
	private bool IsValidSenseType(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				if (ent is BasePet)
				{
					return true;
				}
				if (ent is ScarecrowNPC)
				{
					return true;
				}
				if (this.senseTypes.HasFlag(EntityType.BasePlayerNPC))
				{
					return true;
				}
			}
			else if (this.senseTypes.HasFlag(EntityType.Player))
			{
				return true;
			}
		}
		return (this.senseTypes.HasFlag(EntityType.NPC) && ent is BaseNpc) || (this.senseTypes.HasFlag(EntityType.WorldItem) && ent is WorldItem) || (this.senseTypes.HasFlag(EntityType.Corpse) && ent is BaseCorpse) || (this.senseTypes.HasFlag(EntityType.TimedExplosive) && ent is TimedExplosive) || (this.senseTypes.HasFlag(EntityType.Chair) && ent is BaseChair);
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000D46D4 File Offset: 0x000D28D4
	private bool IsTargetInVision(BaseEntity target)
	{
		Vector3 rhs = Vector3Ex.Direction(target.transform.position, this.owner.transform.position);
		return Vector3.Dot((this.playerOwner != null) ? this.playerOwner.eyes.BodyForward() : this.owner.transform.forward, rhs) >= this.visionCone;
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000D4743 File Offset: 0x000D2943
	public BaseEntity GetNearestPlayer(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Players, rangeFraction);
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001F6A RID: 8042 RVA: 0x000D4757 File Offset: 0x000D2957
	public List<BaseEntity> Players
	{
		get
		{
			return this.Memory.Players;
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000D4764 File Offset: 0x000D2964
	public BaseEntity GetNearestThreat(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Threats, rangeFraction);
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000D4778 File Offset: 0x000D2978
	public BaseEntity GetNearestTarget(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Targets, rangeFraction);
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000D478C File Offset: 0x000D298C
	private BaseEntity GetNearest(List<BaseEntity> entities, float rangeFraction)
	{
		if (entities == null || entities.Count == 0)
		{
			return null;
		}
		float positiveInfinity = float.PositiveInfinity;
		BaseEntity result = null;
		foreach (BaseEntity baseEntity in entities)
		{
			if (!(baseEntity == null) && baseEntity.Health() > 0f)
			{
				float num = Vector3.Distance(baseEntity.transform.position, this.owner.transform.position);
				if (num <= rangeFraction * this.maxRange && num < positiveInfinity)
				{
					result = baseEntity;
				}
			}
		}
		return result;
	}
}
