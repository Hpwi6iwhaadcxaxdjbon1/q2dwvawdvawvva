using System;
using System.Collections;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Token: 0x02000043 RID: 67
public class BaseNpc : BaseCombatEntity
{
	// Token: 0x0400034F RID: 847
	[NonSerialized]
	public Transform ChaseTransform;

	// Token: 0x04000350 RID: 848
	public int agentTypeIndex;

	// Token: 0x04000351 RID: 849
	public bool NewAI;

	// Token: 0x04000352 RID: 850
	public bool LegacyNavigation = true;

	// Token: 0x04000354 RID: 852
	private Vector3 stepDirection;

	// Token: 0x04000357 RID: 855
	private float maxFleeTime;

	// Token: 0x04000358 RID: 856
	private float fleeHealthThresholdPercentage = 1f;

	// Token: 0x04000359 RID: 857
	private float blockEnemyTargetingTimeout = float.NegativeInfinity;

	// Token: 0x0400035A RID: 858
	private float blockFoodTargetingTimeout = float.NegativeInfinity;

	// Token: 0x0400035B RID: 859
	private float aggroTimeout = float.NegativeInfinity;

	// Token: 0x0400035C RID: 860
	private float lastAggroChanceResult;

	// Token: 0x0400035D RID: 861
	private float lastAggroChanceCalcTime;

	// Token: 0x0400035E RID: 862
	private const float aggroChanceRecalcTimeout = 5f;

	// Token: 0x0400035F RID: 863
	private float eatTimeout = float.NegativeInfinity;

	// Token: 0x04000360 RID: 864
	private float wakeUpBlockMoveTimeout = float.NegativeInfinity;

	// Token: 0x04000361 RID: 865
	private global::BaseEntity blockTargetingThisEnemy;

	// Token: 0x04000362 RID: 866
	[NonSerialized]
	public float waterDepth;

	// Token: 0x04000363 RID: 867
	[NonSerialized]
	public bool swimming;

	// Token: 0x04000364 RID: 868
	[NonSerialized]
	public bool wasSwimming;

	// Token: 0x04000365 RID: 869
	private static readonly AnimationCurve speedFractionResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04000366 RID: 870
	private bool _traversingNavMeshLink;

	// Token: 0x04000367 RID: 871
	private OffMeshLinkData _currentNavMeshLink;

	// Token: 0x04000368 RID: 872
	private string _currentNavMeshLinkName;

	// Token: 0x04000369 RID: 873
	private float _currentNavMeshLinkTraversalTime;

	// Token: 0x0400036A RID: 874
	private float _currentNavMeshLinkTraversalTimeDelta;

	// Token: 0x0400036B RID: 875
	private Quaternion _currentNavMeshLinkOrientation;

	// Token: 0x0400036C RID: 876
	private Vector3 _currentNavMeshLinkEndPos;

	// Token: 0x0400036D RID: 877
	private float nextAttackTime;

	// Token: 0x0400036E RID: 878
	[SerializeField]
	[global::InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x0400036F RID: 879
	[global::InspectorFlags]
	public BaseNpc.AiFlags aiFlags;

	// Token: 0x04000370 RID: 880
	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(BaseNpc.Facts)).Length];

	// Token: 0x04000371 RID: 881
	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	// Token: 0x04000372 RID: 882
	public float SensesTickRate = 0.5f;

	// Token: 0x04000373 RID: 883
	[NonSerialized]
	public global::BaseEntity[] SensesResults = new global::BaseEntity[64];

	// Token: 0x04000374 RID: 884
	private float lastTickTime;

	// Token: 0x04000375 RID: 885
	private float playerTargetDecisionStartTime;

	// Token: 0x04000376 RID: 886
	private float animalTargetDecisionStartTime;

	// Token: 0x04000377 RID: 887
	private bool isAlreadyCheckingPathPending;

	// Token: 0x04000378 RID: 888
	private int numPathPendingAttempts;

	// Token: 0x04000379 RID: 889
	private float accumPathPendingDelay;

	// Token: 0x0400037A RID: 890
	public const float TickRate = 0.1f;

	// Token: 0x0400037B RID: 891
	private Vector3 lastStuckPos;

	// Token: 0x0400037C RID: 892
	private float nextFlinchTime;

	// Token: 0x0400037D RID: 893
	private float _lastHeardGunshotTime = float.NegativeInfinity;

	// Token: 0x04000380 RID: 896
	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	// Token: 0x04000381 RID: 897
	public BaseNpc.AiStatistics Stats;

	// Token: 0x04000382 RID: 898
	public Vector3 AttackOffset;

	// Token: 0x04000383 RID: 899
	public float AttackDamage = 20f;

	// Token: 0x04000384 RID: 900
	public DamageType AttackDamageType = DamageType.Bite;

	// Token: 0x04000385 RID: 901
	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	// Token: 0x04000386 RID: 902
	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	// Token: 0x04000387 RID: 903
	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	// Token: 0x04000388 RID: 904
	public NavMeshAgent NavAgent;

	// Token: 0x04000389 RID: 905
	public LayerMask movementMask = 429990145;

	// Token: 0x0400038A RID: 906
	public float stuckDuration;

	// Token: 0x0400038B RID: 907
	public float lastStuckTime;

	// Token: 0x0400038C RID: 908
	public float idleDuration;

	// Token: 0x0400038D RID: 909
	private bool _isDormant;

	// Token: 0x0400038E RID: 910
	private float lastSetDestinationTime;

	// Token: 0x04000393 RID: 915
	[NonSerialized]
	public StateTimer BusyTimer;

	// Token: 0x04000394 RID: 916
	[NonSerialized]
	public float Sleep;

	// Token: 0x04000395 RID: 917
	[NonSerialized]
	public VitalLevel Stamina;

	// Token: 0x04000396 RID: 918
	[NonSerialized]
	public VitalLevel Energy;

	// Token: 0x04000397 RID: 919
	[NonSerialized]
	public VitalLevel Hydration;

	// Token: 0x0600045C RID: 1116 RVA: 0x00035454 File Offset: 0x00033654
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseNpc.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00035494 File Offset: 0x00033694
	public void UpdateDestination(Vector3 position)
	{
		if (this.IsStopped)
		{
			this.IsStopped = false;
		}
		if ((this.Destination - position).sqrMagnitude > 0.010000001f)
		{
			this.Destination = position;
		}
		this.ChaseTransform = null;
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x000354D9 File Offset: 0x000336D9
	public void UpdateDestination(Transform tx)
	{
		this.IsStopped = false;
		this.ChaseTransform = tx;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x000354E9 File Offset: 0x000336E9
	public void StopMoving()
	{
		this.IsStopped = true;
		this.ChaseTransform = null;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, 0, true, true);
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00035504 File Offset: 0x00033704
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00035514 File Offset: 0x00033714
	public static Vector3 GetNewNavPosWithVelocity(global::BaseEntity ent, Vector3 velocity)
	{
		global::BaseEntity parentEntity = ent.GetParentEntity();
		if (parentEntity != null)
		{
			velocity = parentEntity.transform.InverseTransformDirection(velocity);
		}
		Vector3 targetPosition = ent.ServerPosition + velocity * UnityEngine.Time.fixedDeltaTime;
		NavMeshHit navMeshHit;
		NavMesh.Raycast(ent.ServerPosition, targetPosition, out navMeshHit, -1);
		if (!navMeshHit.position.IsNaNOrInfinity())
		{
			return navMeshHit.position;
		}
		return ent.ServerPosition;
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000462 RID: 1122 RVA: 0x00035584 File Offset: 0x00033784
	// (set) Token: 0x06000463 RID: 1123 RVA: 0x0003558C File Offset: 0x0003378C
	public int AgentTypeIndex
	{
		get
		{
			return this.agentTypeIndex;
		}
		set
		{
			this.agentTypeIndex = value;
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000464 RID: 1124 RVA: 0x00035595 File Offset: 0x00033795
	// (set) Token: 0x06000465 RID: 1125 RVA: 0x0003559D File Offset: 0x0003379D
	public bool IsStuck { get; set; }

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x000355A6 File Offset: 0x000337A6
	// (set) Token: 0x06000467 RID: 1127 RVA: 0x000355AE File Offset: 0x000337AE
	public bool AgencyUpdateRequired { get; set; }

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000468 RID: 1128 RVA: 0x000355B7 File Offset: 0x000337B7
	// (set) Token: 0x06000469 RID: 1129 RVA: 0x000355BF File Offset: 0x000337BF
	public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

	// Token: 0x0600046A RID: 1130 RVA: 0x000355C8 File Offset: 0x000337C8
	public override string DebugText()
	{
		return base.DebugText() + string.Format("\nBehaviour: {0}", this.CurrentBehaviour) + string.Format("\nAttackTarget: {0}", this.AttackTarget) + string.Format("\nFoodTarget: {0}", this.FoodTarget) + string.Format("\nSleep: {0:0.00}", this.Sleep);
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0003563C File Offset: 0x0003383C
	public void TickAi()
	{
		if (!AI.think)
		{
			return;
		}
		if (TerrainMeta.WaterMap != null)
		{
			this.waterDepth = TerrainMeta.WaterMap.GetDepth(this.ServerPosition);
			this.wasSwimming = this.swimming;
			this.swimming = (this.waterDepth > this.Stats.WaterLevelNeck * 0.25f);
		}
		else
		{
			this.wasSwimming = false;
			this.swimming = false;
			this.waterDepth = 0f;
		}
		using (TimeWarning.New("TickNavigation", 0))
		{
			this.TickNavigation();
		}
		if (!AiManager.ai_dormant || this.GetNavAgent.enabled || this.CurrentBehaviour == BaseNpc.Behaviour.Sleep || this.NewAI)
		{
			using (TimeWarning.New("TickMetabolism", 0))
			{
				this.TickSleep();
				this.TickMetabolism();
				this.TickSpeed();
			}
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00035744 File Offset: 0x00033944
	private void TickSpeed()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		float num = this.Stats.Speed;
		if (this.NewAI)
		{
			num = (this.swimming ? this.ToSpeed(BaseNpc.SpeedEnum.Walk) : this.TargetSpeed);
			num *= 0.5f + base.healthFraction * 0.5f;
			this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
			this.NavAgent.angularSpeed = this.Stats.TurnSpeed;
			this.NavAgent.acceleration = this.Stats.Acceleration;
			return;
		}
		num *= 0.5f + base.healthFraction * 0.5f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			num *= 0.2f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Eat)
		{
			num *= 0.3f;
		}
		float num2 = Mathf.Min(this.NavAgent.speed / this.Stats.Speed, 1f);
		num2 = BaseNpc.speedFractionResponse.Evaluate(num2);
		float num3 = 1f - 0.9f * Vector3.Angle(base.transform.forward, (this.NavAgent.nextPosition - this.ServerPosition).normalized) / 180f * num2 * num2;
		num *= num3;
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
		this.NavAgent.angularSpeed = this.Stats.TurnSpeed * (1.1f - num2);
		this.NavAgent.acceleration = this.Stats.Acceleration;
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x000358EC File Offset: 0x00033AEC
	protected virtual void TickMetabolism()
	{
		float num = 0.00016666666f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			num *= 0.01f;
		}
		if (this.NavAgent.desiredVelocity.sqrMagnitude > 0.1f)
		{
			num *= 2f;
		}
		this.Energy.Add(num * 0.1f * -1f);
		if (this.Stamina.TimeSinceUsed > 5f)
		{
			float num2 = 0.06666667f;
			this.Stamina.Add(0.1f * num2);
		}
		float secondsSinceAttacked = base.SecondsSinceAttacked;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00035981 File Offset: 0x00033B81
	public virtual bool WantsToEat(global::BaseEntity best)
	{
		return best.HasTrait(global::BaseEntity.TraitFlag.Food) && !best.HasTrait(global::BaseEntity.TraitFlag.Alive);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0003599C File Offset: 0x00033B9C
	public virtual float FearLevel(global::BaseEntity ent)
	{
		float num = 0f;
		BaseNpc baseNpc = ent as BaseNpc;
		if (baseNpc != null && baseNpc.Stats.Size > this.Stats.Size)
		{
			if (baseNpc.WantsToAttack(this) > 0.25f)
			{
				num += 0.2f;
			}
			if (baseNpc.AttackTarget == this)
			{
				num += 0.3f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Attack)
			{
				num *= 1.5f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
			{
				num *= 0.1f;
			}
		}
		if (ent as global::BasePlayer != null)
		{
			num += 1f;
		}
		return num;
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float HateLevel(global::BaseEntity ent)
	{
		return 0f;
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00035A3C File Offset: 0x00033C3C
	protected virtual void TickSleep()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			this.IsSleeping = true;
			this.Sleep += 0.00033333336f;
		}
		else
		{
			this.IsSleeping = false;
			this.Sleep -= 2.7777778E-05f;
		}
		this.Sleep = Mathf.Clamp01(this.Sleep);
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00035A98 File Offset: 0x00033C98
	public void TickNavigationWater()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		position.y = 0f - this.Stats.WaterLevelNeck;
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00035B68 File Offset: 0x00033D68
	public void TickNavigation()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00035C20 File Offset: 0x00033E20
	private void TickChase()
	{
		Vector3 vector = this.ChaseTransform.position;
		Vector3 vector2 = base.transform.position - vector;
		if ((double)vector2.magnitude < 5.0)
		{
			vector += vector2.normalized * this.AttackOffset.z;
		}
		if ((this.NavAgent.destination - vector).sqrMagnitude > 0.010000001f)
		{
			this.NavAgent.SetDestination(vector);
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00035CA9 File Offset: 0x00033EA9
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this._traversingNavMeshLink && !this.HandleNavMeshLinkTraversalStart(delta))
		{
			return;
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (!this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this._currentNavMeshLinkTraversalTimeDelta += delta;
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00035CE0 File Offset: 0x00033EE0
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.NavAgent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated || currentOffMeshLinkData.offMeshLink == null)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.NavAgent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this._currentNavMeshLink = currentOffMeshLinkData;
		this._currentNavMeshLinkName = this._currentNavMeshLink.linkType.ToString();
		if (currentOffMeshLinkData.offMeshLink.biDirectional)
		{
			if ((currentOffMeshLinkData.endPos - this.ServerPosition).sqrMagnitude < 0.05f)
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.startPos + Vector3.up * (currentOffMeshLinkData.endPos.y - currentOffMeshLinkData.startPos.y) - currentOffMeshLinkData.endPos);
			}
			else
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
			}
		}
		else
		{
			this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
		}
		this._traversingNavMeshLink = true;
		this.NavAgent.ActivateCurrentOffMeshLink(false);
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		float num = Mathf.Max(this.NavAgent.speed, 2.8f);
		float magnitude = (this._currentNavMeshLink.startPos - this._currentNavMeshLink.endPos).magnitude;
		this._currentNavMeshLinkTraversalTime = magnitude / num;
		this._currentNavMeshLinkTraversalTimeDelta = 0f;
		if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
		{
			this._currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00035F74 File Offset: 0x00034174
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00036058 File Offset: 0x00034258
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkTraversalTimeDelta >= this._currentNavMeshLinkTraversalTime)
		{
			moveToPosition = this._currentNavMeshLink.endPos;
			this._traversingNavMeshLink = false;
			this._currentNavMeshLink = default(OffMeshLinkData);
			this._currentNavMeshLinkTraversalTime = 0f;
			this._currentNavMeshLinkTraversalTimeDelta = 0f;
			this._currentNavMeshLinkName = string.Empty;
			this._currentNavMeshLinkOrientation = Quaternion.identity;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x000360CC File Offset: 0x000342CC
	private void CompleteNavMeshLink()
	{
		this.NavAgent.ActivateCurrentOffMeshLink(true);
		this.NavAgent.CompleteOffMeshLink();
		this.NavAgent.isStopped = false;
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00036100 File Offset: 0x00034300
	private void TickFollowPath(ref Vector3 moveToPosition)
	{
		moveToPosition = this.NavAgent.nextPosition;
		this.stepDirection = this.NavAgent.desiredVelocity.normalized;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00036138 File Offset: 0x00034338
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition,
				" (destroying)"
			}));
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x000361AC File Offset: 0x000343AC
	private void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.ServerPosition = moveToPosition;
		this.UpdateAiRotation();
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x000361BB File Offset: 0x000343BB
	private void TickIdle()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			this.idleDuration += 0.1f;
			return;
		}
		this.idleDuration = 0f;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x000361E4 File Offset: 0x000343E4
	public void TickStuck()
	{
		if (this.IsNavRunning() && !this.NavAgent.isStopped && (this.lastStuckPos - this.ServerPosition).sqrMagnitude < 0.0625f && this.AttackReady())
		{
			this.stuckDuration += 0.1f;
			if (this.stuckDuration >= 5f && Mathf.Approximately(this.lastStuckTime, 0f))
			{
				this.lastStuckTime = UnityEngine.Time.time;
				this.OnBecomeStuck();
				return;
			}
		}
		else
		{
			this.stuckDuration = 0f;
			this.lastStuckPos = this.ServerPosition;
			if (UnityEngine.Time.time - this.lastStuckTime > 5f)
			{
				this.lastStuckTime = 0f;
				this.OnBecomeUnStuck();
			}
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x000362AC File Offset: 0x000344AC
	public void OnBecomeStuck()
	{
		this.IsStuck = true;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x000362B5 File Offset: 0x000344B5
	public void OnBecomeUnStuck()
	{
		this.IsStuck = false;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x000362C0 File Offset: 0x000344C0
	public void UpdateAiRotation()
	{
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return;
		}
		if (this._traversingNavMeshLink)
		{
			Vector3 vector;
			if (this.ChaseTransform != null)
			{
				vector = this.ChaseTransform.localPosition - this.ServerPosition;
			}
			else if (this.AttackTarget != null)
			{
				vector = this.AttackTarget.ServerPosition - this.ServerPosition;
			}
			else
			{
				vector = this.NavAgent.destination - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 1f)
			{
				vector = this._currentNavMeshLinkEndPos - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = this._currentNavMeshLinkOrientation;
				return;
			}
		}
		else if ((this.NavAgent.destination - this.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 forward = this.stepDirection;
			if (forward.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(forward);
				return;
			}
		}
		if (this.ChaseTransform && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector2 = this.ChaseTransform.localPosition - this.ServerPosition;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude < 9f && sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector2.normalized);
				return;
			}
		}
		else if (this.AttackTarget && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector3 = this.AttackTarget.ServerPosition - this.ServerPosition;
			float sqrMagnitude2 = vector3.sqrMagnitude;
			if (sqrMagnitude2 < 9f && sqrMagnitude2 > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector3.normalized);
				return;
			}
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000482 RID: 1154 RVA: 0x00036489 File Offset: 0x00034689
	public float GetAttackRate
	{
		get
		{
			return this.AttackRate;
		}
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00036491 File Offset: 0x00034691
	public bool AttackReady()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextAttackTime;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000364A4 File Offset: 0x000346A4
	public virtual void StartAttack()
	{
		if (!this.AttackTarget)
		{
			return;
		}
		if (!this.AttackReady())
		{
			return;
		}
		if ((this.AttackTarget.ServerPosition - this.ServerPosition).magnitude > this.AttackRange)
		{
			return;
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		BaseCombatEntity combatTarget = this.CombatTarget;
		if (!combatTarget)
		{
			return;
		}
		combatTarget.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		this.BusyTimer.Activate(0.5f, null);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", this.AttackTarget.ServerPosition);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00036568 File Offset: 0x00034768
	public void Attack(BaseCombatEntity target)
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
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		target.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", target.ServerPosition);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00036604 File Offset: 0x00034804
	public virtual void Eat()
	{
		if (!this.FoodTarget)
		{
			return;
		}
		this.BusyTimer.Activate(0.5f, null);
		this.FoodTarget.Eat(this, 0.5f);
		this.StartEating(UnityEngine.Random.value * 5f + 0.5f);
		base.ClientRPC<Vector3>(null, "Eat", this.FoodTarget.transform.position);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00036675 File Offset: 0x00034875
	public virtual void AddCalories(float amount)
	{
		this.Energy.Add(amount / 1000f);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00036689 File Offset: 0x00034889
	public virtual void Startled()
	{
		base.ClientRPC<Vector3>(null, "Startled", base.transform.position);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x000366A2 File Offset: 0x000348A2
	private bool IsAfraid()
	{
		this.SetFact(BaseNpc.Facts.IsAfraid, 0, true, true);
		return false;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x000366B0 File Offset: 0x000348B0
	protected bool IsAfraidOf(BaseNpc.AiStatistics.FamilyEnum family)
	{
		foreach (BaseNpc.AiStatistics.FamilyEnum familyEnum in this.Stats.IsAfraidOf)
		{
			if (family == familyEnum)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x000366E4 File Offset: 0x000348E4
	private bool CheckHealthThresholdToFlee()
	{
		if (base.healthFraction > this.Stats.HealthThresholdForFleeing)
		{
			if (this.Stats.HealthThresholdForFleeing < 1f)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
			if (this.GetFact(BaseNpc.Facts.HasEnemy) == 1)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
		}
		bool flag = UnityEngine.Random.value < this.Stats.HealthThresholdFleeChance;
		this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, flag ? 1 : 0, true, true);
		return flag;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00036760 File Offset: 0x00034960
	private void TickBehaviourState()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 && this.IsNavRunning() && this.NavAgent.pathStatus == NavMeshPathStatus.PathComplete && UnityEngine.Time.realtimeSinceStartup - (this.maxFleeTime - this.Stats.MaxFleeTime) > 0.5f)
		{
			this.TickFlee();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			this.TickBlockEnemyTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.TickBlockFoodTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			this.TickAggro();
		}
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			this.TickEating();
		}
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			this.TickWakeUpBlockMove();
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00036804 File Offset: 0x00034A04
	private void WantsToFlee()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 || !this.IsNavRunning())
		{
			return;
		}
		this.SetFact(BaseNpc.Facts.WantsToFlee, 1, true, true);
		this.maxFleeTime = UnityEngine.Time.realtimeSinceStartup + this.Stats.MaxFleeTime;
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickFlee()
	{
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0003683C File Offset: 0x00034A3C
	public bool BlockEnemyTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, true, true);
		this.blockEnemyTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		this.blockTargetingThisEnemy = this.AttackTarget;
		return true;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0003686D File Offset: 0x00034A6D
	private void TickBlockEnemyTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockEnemyTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00036891 File Offset: 0x00034A91
	public bool BlockFoodTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
		this.blockFoodTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x000368B8 File Offset: 0x00034AB8
	private void TickBlockFoodTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockFoodTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x000368E0 File Offset: 0x00034AE0
	public bool TryAggro(BaseNpc.EnemyRangeEnum range)
	{
		if (Mathf.Approximately(this.Stats.Hostility, 0f) && Mathf.Approximately(this.Stats.Defensiveness, 0f))
		{
			return false;
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 0 && (range == BaseNpc.EnemyRangeEnum.AggroRange || range == BaseNpc.EnemyRangeEnum.AttackRange))
		{
			float num = (range == BaseNpc.EnemyRangeEnum.AttackRange) ? 1f : this.Stats.Defensiveness;
			num = Mathf.Max(num, this.Stats.Hostility);
			if (UnityEngine.Time.realtimeSinceStartup > this.lastAggroChanceCalcTime + 5f)
			{
				this.lastAggroChanceResult = UnityEngine.Random.value;
				this.lastAggroChanceCalcTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (this.lastAggroChanceResult < num)
			{
				return this.StartAggro(this.Stats.DeaggroChaseTime);
			}
		}
		return false;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0003699B File Offset: 0x00034B9B
	public bool StartAggro(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsAggro, 1, true, true);
		this.aggroTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickAggro()
	{
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x000369C3 File Offset: 0x00034BC3
	public bool StartEating(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsEating, 1, true, true);
		this.eatTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x000369EB File Offset: 0x00034BEB
	private void TickEating()
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.eatTimeout)
		{
			this.SetFact(BaseNpc.Facts.IsEating, 0, true, true);
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00036A10 File Offset: 0x00034C10
	public bool WakeUpBlockMove(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanNotMove, 1, true, true);
		this.wakeUpBlockMoveTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00036A38 File Offset: 0x00034C38
	private void TickWakeUpBlockMove()
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.wakeUpBlockMoveTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanNotMove, 0, true, true);
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00036A60 File Offset: 0x00034C60
	private void OnFactChanged(BaseNpc.Facts fact, byte oldValue, byte newValue)
	{
		if (fact <= BaseNpc.Facts.IsAggro)
		{
			switch (fact)
			{
			case BaseNpc.Facts.CanTargetEnemies:
				if (newValue == 1)
				{
					this.blockTargetingThisEnemy = null;
				}
				break;
			case BaseNpc.Facts.Health:
			case BaseNpc.Facts.IsTired:
				break;
			case BaseNpc.Facts.Speed:
				if (newValue == 0)
				{
					this.StopMoving();
					this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
					return;
				}
				if (newValue != 1)
				{
					this.IsStopped = false;
					return;
				}
				this.IsStopped = false;
				this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
				return;
			case BaseNpc.Facts.IsSleeping:
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Sleep;
					this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, false, true);
					this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
					return;
				}
				this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
				this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
				this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
				this.WakeUpBlockMove(this.Stats.WakeupBlockMoveTime);
				this.TickSenses();
				return;
			default:
				if (fact != BaseNpc.Facts.IsAggro)
				{
					return;
				}
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
					return;
				}
				this.BlockEnemyTargeting(this.Stats.DeaggroCooldown);
				return;
			}
		}
		else if (fact != BaseNpc.Facts.FoodRange)
		{
			if (fact != BaseNpc.Facts.IsEating)
			{
				return;
			}
			if (newValue == 0)
			{
				this.FoodTarget = null;
				return;
			}
		}
		else if (newValue == 0)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Eat;
			return;
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00036B68 File Offset: 0x00034D68
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00036B70 File Offset: 0x00034D70
	public bool HasAiFlag(BaseNpc.AiFlags f)
	{
		return (this.aiFlags & f) == f;
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00036B80 File Offset: 0x00034D80
	public void SetAiFlag(BaseNpc.AiFlags f, bool set)
	{
		BaseNpc.AiFlags aiFlags = this.aiFlags;
		if (set)
		{
			this.aiFlags |= f;
		}
		else
		{
			this.aiFlags &= ~f;
		}
		if (aiFlags != this.aiFlags && base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600049E RID: 1182 RVA: 0x00036BCC File Offset: 0x00034DCC
	// (set) Token: 0x0600049F RID: 1183 RVA: 0x00036BD5 File Offset: 0x00034DD5
	public bool IsSitting
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00036BDF File Offset: 0x00034DDF
	// (set) Token: 0x060004A1 RID: 1185 RVA: 0x00036BE8 File Offset: 0x00034DE8
	public bool IsChasing
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060004A2 RID: 1186 RVA: 0x00036BF2 File Offset: 0x00034DF2
	// (set) Token: 0x060004A3 RID: 1187 RVA: 0x00036BFB File Offset: 0x00034DFB
	public bool IsSleeping
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x00036C05 File Offset: 0x00034E05
	public void InitFacts()
	{
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00036C1C File Offset: 0x00034E1C
	public byte GetFact(BaseNpc.Facts fact)
	{
		return this.CurrentFacts[(int)fact];
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00036C28 File Offset: 0x00034E28
	public void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte b = this.CurrentFacts[(int)fact];
		this.CurrentFacts[(int)fact] = value;
		if (triggerCallback && value != b)
		{
			this.OnFactChanged(fact, b, value);
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00036C58 File Offset: 0x00034E58
	public BaseNpc.EnemyRangeEnum ToEnemyRangeEnum(float range)
	{
		if (range <= this.AttackRange)
		{
			return BaseNpc.EnemyRangeEnum.AttackRange;
		}
		if (range <= this.Stats.AggressionRange)
		{
			return BaseNpc.EnemyRangeEnum.AggroRange;
		}
		if (range >= this.Stats.DeaggroRange && this.GetFact(BaseNpc.Facts.IsAggro) > 0)
		{
			return BaseNpc.EnemyRangeEnum.OutOfRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.EnemyRangeEnum.AwareRange;
		}
		return BaseNpc.EnemyRangeEnum.OutOfRange;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00036CAC File Offset: 0x00034EAC
	public float GetActiveAggressionRangeSqr()
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
		}
		return this.Stats.AggressionRange * this.Stats.AggressionRange;
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00036CE8 File Offset: 0x00034EE8
	public BaseNpc.FoodRangeEnum ToFoodRangeEnum(float range)
	{
		if (range <= 0.5f)
		{
			return BaseNpc.FoodRangeEnum.EatRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.FoodRangeEnum.AwareRange;
		}
		return BaseNpc.FoodRangeEnum.OutOfRange;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00036D05 File Offset: 0x00034F05
	public BaseNpc.AfraidRangeEnum ToAfraidRangeEnum(float range)
	{
		if (range <= this.Stats.AfraidRange)
		{
			return BaseNpc.AfraidRangeEnum.InAfraidRange;
		}
		return BaseNpc.AfraidRangeEnum.OutOfRange;
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00036D18 File Offset: 0x00034F18
	public BaseNpc.HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return BaseNpc.HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return BaseNpc.HealthEnum.Medium;
		}
		return BaseNpc.HealthEnum.Low;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00036D30 File Offset: 0x00034F30
	public byte ToIsTired(float energyNormalized)
	{
		bool flag = this.GetFact(BaseNpc.Facts.IsSleeping) == 1;
		if (!flag && energyNormalized < 0.1f)
		{
			return 1;
		}
		if (flag && energyNormalized < 0.5f)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00036D63 File Offset: 0x00034F63
	public BaseNpc.SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return BaseNpc.SpeedEnum.StandStill;
		}
		if (speed <= 0.18f)
		{
			return BaseNpc.SpeedEnum.Walk;
		}
		return BaseNpc.SpeedEnum.Run;
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00036D7A File Offset: 0x00034F7A
	public float ToSpeed(BaseNpc.SpeedEnum speed)
	{
		if (speed == BaseNpc.SpeedEnum.StandStill)
		{
			return 0f;
		}
		if (speed != BaseNpc.SpeedEnum.Walk)
		{
			return this.Stats.Speed;
		}
		return 0.18f * this.Stats.Speed;
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00036DA8 File Offset: 0x00034FA8
	public byte GetPathStatus()
	{
		if (!this.IsNavRunning())
		{
			return 2;
		}
		return (byte)this.NavAgent.pathStatus;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00036DC4 File Offset: 0x00034FC4
	private void TickSenses()
	{
		if (global::BaseEntity.Query.Server == null || this.IsDormant)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.lastTickTime + this.SensesTickRate)
		{
			this.TickHearing();
			this.TickSmell();
			this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		}
		if (!AI.animal_ignore_food)
		{
			this.TickFoodAwareness();
		}
		this.UpdateSelfFacts();
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00036E1F File Offset: 0x0003501F
	private void TickHearing()
	{
		this.SetFact(BaseNpc.Facts.LoudNoiseNearby, 0, true, true);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x000063A5 File Offset: 0x000045A5
	private void TickSmell()
	{
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00036E2C File Offset: 0x0003502C
	private float DecisionMomentumPlayerTarget()
	{
		float num = UnityEngine.Time.time - this.playerTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00036E58 File Offset: 0x00035058
	private float DecisionMomentumAnimalTarget()
	{
		float num = UnityEngine.Time.time - this.animalTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00036E81 File Offset: 0x00035081
	private void TickFoodAwareness()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.FoodTarget = null;
			this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
			return;
		}
		this.SelectFood();
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x000063A5 File Offset: 0x000045A5
	private void SelectFood()
	{
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x000063A5 File Offset: 0x000045A5
	private void SelectClosestFood()
	{
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateSelfFacts()
	{
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00036EA8 File Offset: 0x000350A8
	private byte IsMoving()
	{
		return (this.IsNavRunning() && this.NavAgent.hasPath && this.NavAgent.remainingDistance > this.NavAgent.stoppingDistance && !this.IsStuck && this.GetFact(BaseNpc.Facts.Speed) != 0) ? 1 : 0;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00036EF8 File Offset: 0x000350F8
	private static bool AiCaresAbout(global::BaseEntity ent)
	{
		if (ent is global::BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (!AI.animal_ignore_food)
		{
			if (ent is global::WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			if (ent is CollectibleEntity)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00036F34 File Offset: 0x00035134
	private static bool WithinVisionCone(BaseNpc npc, global::BaseEntity other)
	{
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 normalized = (other.ServerPosition - npc.ServerPosition).normalized;
		return Vector3.Dot(npc.transform.forward, normalized) >= npc.Stats.VisionCone;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00036F98 File Offset: 0x00035198
	public void SetTargetPathStatus(float pendingDelay = 0.05f)
	{
		if (this.isAlreadyCheckingPathPending)
		{
			return;
		}
		if (this.NavAgent.pathPending && this.numPathPendingAttempts < 10)
		{
			this.isAlreadyCheckingPathPending = true;
			base.Invoke(new Action(this.DelayedTargetPathStatus), pendingDelay);
			return;
		}
		this.numPathPendingAttempts = 0;
		this.accumPathPendingDelay = 0f;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00037002 File Offset: 0x00035202
	private void DelayedTargetPathStatus()
	{
		this.accumPathPendingDelay += 0.1f;
		this.isAlreadyCheckingPathPending = false;
		this.SetTargetPathStatus(this.accumPathPendingDelay);
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0003702C File Offset: 0x0003522C
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent != null)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		this.IsStuck = false;
		this.AgencyUpdateRequired = false;
		this.IsOnOffmeshLinkAndReachedNewCoord = false;
		base.InvokeRandomized(new Action(this.TickAi), 0.1f, 0.1f, 0.0050000004f);
		this.Sleep = UnityEngine.Random.Range(0.5f, 1f);
		this.Stamina.Level = UnityEngine.Random.Range(0.1f, 1f);
		this.Energy.Level = UnityEngine.Random.Range(0.5f, 1f);
		this.Hydration.Level = UnityEngine.Random.Range(0.5f, 1f);
		if (this.NewAI)
		{
			this.InitFacts();
			this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0003715B File Offset: 0x0003535B
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x00037163 File Offset: 0x00035363
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0003716C File Offset: 0x0003536C
	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(base.KillMessage), 0.5f);
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x000371C8 File Offset: 0x000353C8
	protected virtual void OnSenseGunshot(Sensation sensation)
	{
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		this.LastHeardGunshotDirection = (sensation.Position - base.transform.localPosition).normalized;
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Flee;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060004C5 RID: 1221 RVA: 0x00037214 File Offset: 0x00035414
	public float SecondsSinceLastHeardGunshot
	{
		get
		{
			return UnityEngine.Time.time - this._lastHeardGunshotTime;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x00037222 File Offset: 0x00035422
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0003722A File Offset: 0x0003542A
	public Vector3 LastHeardGunshotDirection { get; set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060004C8 RID: 1224 RVA: 0x00037233 File Offset: 0x00035433
	// (set) Token: 0x060004C9 RID: 1225 RVA: 0x0003723B File Offset: 0x0003543B
	public float TargetSpeed { get; set; }

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060004CA RID: 1226 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060004CB RID: 1227 RVA: 0x00037244 File Offset: 0x00035444
	// (set) Token: 0x060004CC RID: 1228 RVA: 0x0003724C File Offset: 0x0003544C
	public bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			if (this._isDormant)
			{
				this.StopMoving();
				this.Pause();
				return;
			}
			if (this.GetNavAgent == null || AiManager.nav_disable)
			{
				this.IsDormant = true;
				return;
			}
			this.Resume();
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060004CD RID: 1229 RVA: 0x00037298 File Offset: 0x00035498
	public float SecondsSinceLastSetDestination
	{
		get
		{
			return UnityEngine.Time.time - this.lastSetDestinationTime;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060004CE RID: 1230 RVA: 0x000372A6 File Offset: 0x000354A6
	public float LastSetDestinationTime
	{
		get
		{
			return this.lastSetDestinationTime;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060004CF RID: 1231 RVA: 0x000372AE File Offset: 0x000354AE
	// (set) Token: 0x060004D0 RID: 1232 RVA: 0x000372CF File Offset: 0x000354CF
	public Vector3 Destination
	{
		get
		{
			if (this.IsNavRunning())
			{
				return this.GetNavAgent.destination;
			}
			return this.Entity.ServerPosition;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.destination = value;
				this.lastSetDestinationTime = UnityEngine.Time.time;
			}
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060004D1 RID: 1233 RVA: 0x000372F0 File Offset: 0x000354F0
	// (set) Token: 0x060004D2 RID: 1234 RVA: 0x00037307 File Offset: 0x00035507
	public bool IsStopped
	{
		get
		{
			return !this.IsNavRunning() || this.GetNavAgent.isStopped;
		}
		set
		{
			if (this.IsNavRunning())
			{
				if (value)
				{
					this.GetNavAgent.destination = this.ServerPosition;
				}
				this.GetNavAgent.isStopped = value;
			}
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060004D3 RID: 1235 RVA: 0x00037331 File Offset: 0x00035531
	// (set) Token: 0x060004D4 RID: 1236 RVA: 0x00037348 File Offset: 0x00035548
	public bool AutoBraking
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.autoBraking;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.autoBraking = value;
			}
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060004D5 RID: 1237 RVA: 0x0003735E File Offset: 0x0003555E
	public bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.hasPath;
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00037375 File Offset: 0x00035575
	public bool IsNavRunning()
	{
		return this.GetNavAgent != null && this.GetNavAgent.enabled && this.GetNavAgent.isOnNavMesh;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0003739F File Offset: 0x0003559F
	public void Pause()
	{
		if (this.GetNavAgent != null && this.GetNavAgent.enabled)
		{
			this.GetNavAgent.enabled = false;
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x000373C8 File Offset: 0x000355C8
	public void Resume()
	{
		if (!this.GetNavAgent.isOnNavMesh)
		{
			base.StartCoroutine(this.TryForceToNavmesh());
			return;
		}
		this.GetNavAgent.enabled = true;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000373F1 File Offset: 0x000355F1
	private IEnumerator TryForceToNavmesh()
	{
		yield return null;
		int numTries = 0;
		float waitForRetryTime = 1f;
		float maxDistanceMultiplier = 2f;
		if (SingletonComponent<DynamicNavMesh>.Instance != null)
		{
			while (SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
			{
				yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
				waitForRetryTime += 0.5f;
			}
		}
		waitForRetryTime = 1f;
		while (numTries < 4)
		{
			if (this.GetNavAgent.isOnNavMesh)
			{
				this.GetNavAgent.enabled = true;
				yield break;
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.ServerPosition, out navMeshHit, this.GetNavAgent.height * maxDistanceMultiplier, this.GetNavAgent.areaMask))
			{
				this.ServerPosition = navMeshHit.position;
				this.GetNavAgent.Warp(this.ServerPosition);
				this.GetNavAgent.enabled = true;
				yield break;
			}
			yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
			maxDistanceMultiplier *= 1.5f;
			waitForRetryTime *= 1.5f;
			int num = numTries;
			numTries = num + 1;
		}
		Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[]
		{
			base.name
		});
		base.DieInstantly();
		yield break;
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060004DA RID: 1242 RVA: 0x00037400 File Offset: 0x00035600
	// (set) Token: 0x060004DB RID: 1243 RVA: 0x00037408 File Offset: 0x00035608
	public global::BaseEntity AttackTarget { get; set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060004DC RID: 1244 RVA: 0x00037411 File Offset: 0x00035611
	// (set) Token: 0x060004DD RID: 1245 RVA: 0x00037419 File Offset: 0x00035619
	public Memory.SeenInfo AttackTargetMemory { get; set; }

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060004DE RID: 1246 RVA: 0x00037422 File Offset: 0x00035622
	// (set) Token: 0x060004DF RID: 1247 RVA: 0x0003742A File Offset: 0x0003562A
	public global::BaseEntity FoodTarget { get; set; }

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00037433 File Offset: 0x00035633
	public BaseCombatEntity CombatTarget
	{
		get
		{
			return this.AttackTarget as BaseCombatEntity;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00037440 File Offset: 0x00035640
	// (set) Token: 0x060004E2 RID: 1250 RVA: 0x00037448 File Offset: 0x00035648
	public Vector3 SpawnPosition { get; set; }

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float AttackTargetVisibleFor
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060004E4 RID: 1252 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float TimeAtDestination
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060004E5 RID: 1253 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseCombatEntity Entity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060004E6 RID: 1254 RVA: 0x00037454 File Offset: 0x00035654
	public NavMeshAgent GetNavAgent
	{
		get
		{
			if (base.isClient)
			{
				return null;
			}
			if (this.NavAgent == null)
			{
				this.NavAgent = base.GetComponent<NavMeshAgent>();
				if (this.NavAgent == null)
				{
					Debug.LogErrorFormat("{0} has no nav agent!", new object[]
					{
						base.name
					});
				}
			}
			return this.NavAgent;
		}
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x000374B2 File Offset: 0x000356B2
	public float GetWantsToAttack(global::BaseEntity target)
	{
		return this.WantsToAttack(target);
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060004E8 RID: 1256 RVA: 0x000374BB File Offset: 0x000356BB
	public BaseNpc.AiStatistics GetStats
	{
		get
		{
			return this.Stats;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x000374C3 File Offset: 0x000356C3
	public float GetAttackRange
	{
		get
		{
			return this.AttackRange;
		}
	}

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x060004EA RID: 1258 RVA: 0x000374CB File Offset: 0x000356CB
	public Vector3 GetAttackOffset
	{
		get
		{
			return this.AttackOffset;
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x060004EB RID: 1259 RVA: 0x000374D3 File Offset: 0x000356D3
	public float GetStamina
	{
		get
		{
			return this.Stamina.Level;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x060004EC RID: 1260 RVA: 0x000374E0 File Offset: 0x000356E0
	public float GetEnergy
	{
		get
		{
			return this.Energy.Level;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x060004ED RID: 1261 RVA: 0x000374ED File Offset: 0x000356ED
	public float GetAttackCost
	{
		get
		{
			return this.AttackCost;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x060004EE RID: 1262 RVA: 0x000374F5 File Offset: 0x000356F5
	public float GetSleep
	{
		get
		{
			return this.Sleep;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x060004EF RID: 1263 RVA: 0x000374FD File Offset: 0x000356FD
	public Vector3 CurrentAimAngles
	{
		get
		{
			return base.transform.forward;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x060004F0 RID: 1264 RVA: 0x0003750A File Offset: 0x0003570A
	public float GetStuckDuration
	{
		get
		{
			return this.stuckDuration;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x060004F1 RID: 1265 RVA: 0x00037512 File Offset: 0x00035712
	public float GetLastStuckTime
	{
		get
		{
			return this.lastStuckTime;
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0003751A File Offset: 0x0003571A
	public bool BusyTimerActive()
	{
		return this.BusyTimer.IsActive;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00037527 File Offset: 0x00035727
	public void SetBusyFor(float dur)
	{
		this.BusyTimer.Activate(dur, null);
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00037536 File Offset: 0x00035736
	public Vector3 AttackPosition
	{
		get
		{
			return this.ServerPosition + base.transform.TransformDirection(this.AttackOffset);
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060004F5 RID: 1269 RVA: 0x00037554 File Offset: 0x00035754
	public Vector3 CrouchedAttackPosition
	{
		get
		{
			return this.AttackPosition;
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0003755C File Offset: 0x0003575C
	internal float WantsToAttack(global::BaseEntity target)
	{
		if (target == null)
		{
			return 0f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return 0f;
		}
		if (!target.HasAnyTrait(global::BaseEntity.TraitFlag.Animal | global::BaseEntity.TraitFlag.Human))
		{
			return 0f;
		}
		if (target.GetType() == base.GetType())
		{
			return 1f - this.Stats.Tolerance;
		}
		return 1f;
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x060004F7 RID: 1271 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float currentBehaviorDuration
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060004F8 RID: 1272 RVA: 0x000375C0 File Offset: 0x000357C0
	// (set) Token: 0x060004F9 RID: 1273 RVA: 0x000375C8 File Offset: 0x000357C8
	public BaseNpc.Behaviour CurrentBehaviour { get; set; }

	// Token: 0x060004FA RID: 1274 RVA: 0x000375D1 File Offset: 0x000357D1
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseNPC = Facepunch.Pool.Get<BaseNPC>();
		info.msg.baseNPC.flags = (int)this.aiFlags;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00037600 File Offset: 0x00035800
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseNPC != null)
		{
			this.aiFlags = (BaseNpc.AiFlags)info.msg.baseNPC.flags;
		}
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0003762C File Offset: 0x0003582C
	public override float MaxVelocity()
	{
		return this.Stats.Speed;
	}

	// Token: 0x02000B8F RID: 2959
	[Flags]
	public enum AiFlags
	{
		// Token: 0x04003F92 RID: 16274
		Sitting = 2,
		// Token: 0x04003F93 RID: 16275
		Chasing = 4,
		// Token: 0x04003F94 RID: 16276
		Sleeping = 8
	}

	// Token: 0x02000B90 RID: 2960
	public enum Facts
	{
		// Token: 0x04003F96 RID: 16278
		HasEnemy,
		// Token: 0x04003F97 RID: 16279
		EnemyRange,
		// Token: 0x04003F98 RID: 16280
		CanTargetEnemies,
		// Token: 0x04003F99 RID: 16281
		Health,
		// Token: 0x04003F9A RID: 16282
		Speed,
		// Token: 0x04003F9B RID: 16283
		IsTired,
		// Token: 0x04003F9C RID: 16284
		IsSleeping,
		// Token: 0x04003F9D RID: 16285
		IsAttackReady,
		// Token: 0x04003F9E RID: 16286
		IsRoamReady,
		// Token: 0x04003F9F RID: 16287
		IsAggro,
		// Token: 0x04003FA0 RID: 16288
		WantsToFlee,
		// Token: 0x04003FA1 RID: 16289
		IsHungry,
		// Token: 0x04003FA2 RID: 16290
		FoodRange,
		// Token: 0x04003FA3 RID: 16291
		AttackedLately,
		// Token: 0x04003FA4 RID: 16292
		LoudNoiseNearby,
		// Token: 0x04003FA5 RID: 16293
		CanTargetFood,
		// Token: 0x04003FA6 RID: 16294
		IsMoving,
		// Token: 0x04003FA7 RID: 16295
		IsFleeing,
		// Token: 0x04003FA8 RID: 16296
		IsEating,
		// Token: 0x04003FA9 RID: 16297
		IsAfraid,
		// Token: 0x04003FAA RID: 16298
		AfraidRange,
		// Token: 0x04003FAB RID: 16299
		IsUnderHealthThreshold,
		// Token: 0x04003FAC RID: 16300
		CanNotMove,
		// Token: 0x04003FAD RID: 16301
		PathToTargetStatus
	}

	// Token: 0x02000B91 RID: 2961
	public enum EnemyRangeEnum : byte
	{
		// Token: 0x04003FAF RID: 16303
		AttackRange,
		// Token: 0x04003FB0 RID: 16304
		AggroRange,
		// Token: 0x04003FB1 RID: 16305
		AwareRange,
		// Token: 0x04003FB2 RID: 16306
		OutOfRange
	}

	// Token: 0x02000B92 RID: 2962
	public enum FoodRangeEnum : byte
	{
		// Token: 0x04003FB4 RID: 16308
		EatRange,
		// Token: 0x04003FB5 RID: 16309
		AwareRange,
		// Token: 0x04003FB6 RID: 16310
		OutOfRange
	}

	// Token: 0x02000B93 RID: 2963
	public enum AfraidRangeEnum : byte
	{
		// Token: 0x04003FB8 RID: 16312
		InAfraidRange,
		// Token: 0x04003FB9 RID: 16313
		OutOfRange
	}

	// Token: 0x02000B94 RID: 2964
	public enum HealthEnum : byte
	{
		// Token: 0x04003FBB RID: 16315
		Fine,
		// Token: 0x04003FBC RID: 16316
		Medium,
		// Token: 0x04003FBD RID: 16317
		Low
	}

	// Token: 0x02000B95 RID: 2965
	public enum SpeedEnum : byte
	{
		// Token: 0x04003FBF RID: 16319
		StandStill,
		// Token: 0x04003FC0 RID: 16320
		Walk,
		// Token: 0x04003FC1 RID: 16321
		Run
	}

	// Token: 0x02000B96 RID: 2966
	[Serializable]
	public struct AiStatistics
	{
		// Token: 0x04003FC2 RID: 16322
		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		[Range(0f, 1f)]
		public float Size;

		// Token: 0x04003FC3 RID: 16323
		[Tooltip("How fast we can move")]
		public float Speed;

		// Token: 0x04003FC4 RID: 16324
		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		// Token: 0x04003FC5 RID: 16325
		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		// Token: 0x04003FC6 RID: 16326
		[Tooltip("Determines things like how near we'll allow other species to get")]
		[Range(0f, 1f)]
		public float Tolerance;

		// Token: 0x04003FC7 RID: 16327
		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		// Token: 0x04003FC8 RID: 16328
		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		// Token: 0x04003FC9 RID: 16329
		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		// Token: 0x04003FCA RID: 16330
		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		// Token: 0x04003FCB RID: 16331
		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		// Token: 0x04003FCC RID: 16332
		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		// Token: 0x04003FCD RID: 16333
		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		// Token: 0x04003FCE RID: 16334
		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		// Token: 0x04003FCF RID: 16335
		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		// Token: 0x04003FD0 RID: 16336
		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		// Token: 0x04003FD1 RID: 16337
		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		// Token: 0x04003FD2 RID: 16338
		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		// Token: 0x04003FD3 RID: 16339
		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		// Token: 0x04003FD4 RID: 16340
		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		// Token: 0x04003FD5 RID: 16341
		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		// Token: 0x04003FD6 RID: 16342
		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public BaseNpc.AiStatistics.FamilyEnum Family;

		// Token: 0x04003FD7 RID: 16343
		[Tooltip("List of the types of Npc that we are afraid of.")]
		public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;

		// Token: 0x04003FD8 RID: 16344
		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		// Token: 0x04003FD9 RID: 16345
		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		// Token: 0x04003FDA RID: 16346
		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		// Token: 0x04003FDB RID: 16347
		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		// Token: 0x04003FDC RID: 16348
		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		// Token: 0x04003FDD RID: 16349
		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		// Token: 0x04003FDE RID: 16350
		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		// Token: 0x04003FDF RID: 16351
		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		// Token: 0x04003FE0 RID: 16352
		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		// Token: 0x04003FE1 RID: 16353
		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		// Token: 0x04003FE2 RID: 16354
		public float WaterLevelNeckOffset;

		// Token: 0x04003FE3 RID: 16355
		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		// Token: 0x04003FE4 RID: 16356
		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		// Token: 0x04003FE5 RID: 16357
		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		// Token: 0x04003FE6 RID: 16358
		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		// Token: 0x04003FE7 RID: 16359
		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;

		// Token: 0x02000FC2 RID: 4034
		public enum FamilyEnum
		{
			// Token: 0x040050B1 RID: 20657
			Bear,
			// Token: 0x040050B2 RID: 20658
			Wolf,
			// Token: 0x040050B3 RID: 20659
			Deer,
			// Token: 0x040050B4 RID: 20660
			Boar,
			// Token: 0x040050B5 RID: 20661
			Chicken,
			// Token: 0x040050B6 RID: 20662
			Horse,
			// Token: 0x040050B7 RID: 20663
			Zombie,
			// Token: 0x040050B8 RID: 20664
			Scientist,
			// Token: 0x040050B9 RID: 20665
			Murderer,
			// Token: 0x040050BA RID: 20666
			Player
		}
	}

	// Token: 0x02000B97 RID: 2967
	public enum Behaviour
	{
		// Token: 0x04003FE9 RID: 16361
		Idle,
		// Token: 0x04003FEA RID: 16362
		Wander,
		// Token: 0x04003FEB RID: 16363
		Attack,
		// Token: 0x04003FEC RID: 16364
		Flee,
		// Token: 0x04003FED RID: 16365
		Eat,
		// Token: 0x04003FEE RID: 16366
		Sleep,
		// Token: 0x04003FEF RID: 16367
		RetreatingToCover
	}
}
