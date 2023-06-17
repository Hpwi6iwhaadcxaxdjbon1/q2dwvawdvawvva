using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class SimpleShark : BaseCombatEntity
{
	// Token: 0x0400125D RID: 4701
	public Vector3 destination;

	// Token: 0x0400125E RID: 4702
	public float minSpeed;

	// Token: 0x0400125F RID: 4703
	public float maxSpeed;

	// Token: 0x04001260 RID: 4704
	public float idealDepth;

	// Token: 0x04001261 RID: 4705
	public float minTurnSpeed = 0.25f;

	// Token: 0x04001262 RID: 4706
	public float maxTurnSpeed = 2f;

	// Token: 0x04001263 RID: 4707
	public float attackCooldown = 7f;

	// Token: 0x04001264 RID: 4708
	public float aggroRange = 15f;

	// Token: 0x04001265 RID: 4709
	public float obstacleDetectionRadius = 1f;

	// Token: 0x04001266 RID: 4710
	public Animator animator;

	// Token: 0x04001267 RID: 4711
	public GameObjectRef bloodCloud;

	// Token: 0x04001268 RID: 4712
	public GameObjectRef corpsePrefab;

	// Token: 0x04001269 RID: 4713
	private const string SPEARGUN_KILL_STAT = "shark_speargun_kills";

	// Token: 0x0400126A RID: 4714
	[ServerVar]
	public static float forceSurfaceAmount = 0f;

	// Token: 0x0400126B RID: 4715
	[ServerVar]
	public static bool disable = false;

	// Token: 0x0400126C RID: 4716
	private Vector3 spawnPos;

	// Token: 0x0400126D RID: 4717
	private float stoppingDistance = 3f;

	// Token: 0x0400126E RID: 4718
	private float currentSpeed;

	// Token: 0x0400126F RID: 4719
	private float lastStartleTime;

	// Token: 0x04001270 RID: 4720
	private float startleDuration = 1f;

	// Token: 0x04001271 RID: 4721
	private SimpleShark.SimpleState[] states;

	// Token: 0x04001272 RID: 4722
	private SimpleShark.SimpleState _currentState;

	// Token: 0x04001273 RID: 4723
	private bool sleeping;

	// Token: 0x04001274 RID: 4724
	public List<Vector3> patrolPath = new List<Vector3>();

	// Token: 0x04001275 RID: 4725
	private BasePlayer target;

	// Token: 0x04001276 RID: 4726
	private float lastSeenTargetTime;

	// Token: 0x04001277 RID: 4727
	private float nextTargetSearchTime;

	// Token: 0x04001278 RID: 4728
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x04001279 RID: 4729
	private float minFloorDist = 2f;

	// Token: 0x0400127A RID: 4730
	private float minSurfaceDist = 1f;

	// Token: 0x0400127B RID: 4731
	private float lastTimeAttacked;

	// Token: 0x0400127C RID: 4732
	private float nextAttackTime;

	// Token: 0x0400127D RID: 4733
	private Vector3 cachedObstacleNormal;

	// Token: 0x0400127E RID: 4734
	private float cachedObstacleDistance;

	// Token: 0x0400127F RID: 4735
	private float obstacleAvoidanceScale;

	// Token: 0x04001280 RID: 4736
	private float obstacleDetectionRange = 5f;

	// Token: 0x04001281 RID: 4737
	private float timeSinceLastObstacleCheck;

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x060019BB RID: 6587 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x000BBB28 File Offset: 0x000B9D28
	private void GenerateIdlePoints(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int layerMask = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = 1f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 vector2 = Vector3Ex.Direction(vector, center);
			RaycastHit raycastHit;
			if (Physics.SphereCast(center, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, layerMask))
			{
				vector = center + vector2 * (raycastHit.distance - 6f);
			}
			else
			{
				vector = center + vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000BBC50 File Offset: 0x000B9E50
	private void GenerateIdlePoints_Shrinkwrap(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int layerMask = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = radius * 2f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 vector2 = Vector3Ex.Direction(center, vector);
			RaycastHit raycastHit;
			if (Physics.SphereCast(vector, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, layerMask))
			{
				vector = raycastHit.point - vector2 * 6f;
			}
			else
			{
				vector += vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000BBD7C File Offset: 0x000B9F7C
	public override void ServerInit()
	{
		base.ServerInit();
		if (SimpleShark.disable)
		{
			base.Invoke(new Action(base.KillMessage), 0.01f);
			return;
		}
		base.transform.position = this.WaterClamp(base.transform.position);
		this.Init();
		base.InvokeRandomized(new Action(this.CheckSleepState), 0f, 1f, 0.5f);
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x000BBDF4 File Offset: 0x000B9FF4
	public void CheckSleepState()
	{
		bool flag = BaseNetworkable.HasCloseConnections(base.transform.position, 100f);
		this.sleeping = !flag;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x000BBE24 File Offset: 0x000BA024
	public void Init()
	{
		this.GenerateIdlePoints_Shrinkwrap(base.transform.position, 20f, 2f, 3f);
		this.states = new SimpleShark.SimpleState[2];
		this.states[0] = new SimpleShark.IdleState(this);
		this.states[1] = new SimpleShark.AttackState(this);
		base.transform.position = this.patrolPath[0];
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000BBE90 File Offset: 0x000BA090
	private void Think(float delta)
	{
		if (this.states == null)
		{
			return;
		}
		if (SimpleShark.disable)
		{
			if (!base.IsInvoking(new Action(base.KillMessage)))
			{
				base.Invoke(new Action(base.KillMessage), 0.01f);
			}
			return;
		}
		if (this.sleeping)
		{
			return;
		}
		SimpleShark.SimpleState simpleState = null;
		float num = -1f;
		foreach (SimpleShark.SimpleState simpleState2 in this.states)
		{
			float num2 = simpleState2.State_Weight();
			if (num2 > num)
			{
				simpleState = simpleState2;
				num = num2;
			}
		}
		if (simpleState != this._currentState && (this._currentState == null || this._currentState.CanInterrupt()))
		{
			if (this._currentState != null)
			{
				this._currentState.State_Exit();
			}
			simpleState.State_Enter();
			this._currentState = simpleState;
		}
		this.UpdateTarget(delta);
		this._currentState.State_Think(delta);
		this.UpdateObstacleAvoidance(delta);
		this.UpdateDirection(delta);
		this.UpdateSpeed(delta);
		this.UpdatePosition(delta);
		base.SetFlag(BaseEntity.Flags.Open, this.HasTarget() && this.CanAttack(), false, true);
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000BBFA0 File Offset: 0x000BA1A0
	public Vector3 WaterClamp(Vector3 point)
	{
		float height = WaterSystem.GetHeight(point);
		float min = TerrainMeta.HeightMap.GetHeight(point) + this.minFloorDist;
		float max = height - this.minSurfaceDist;
		if (SimpleShark.forceSurfaceAmount != 0f)
		{
			max = (min = WaterSystem.GetHeight(point) + SimpleShark.forceSurfaceAmount);
		}
		point.y = Mathf.Clamp(point.y, min, max);
		return point;
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x000BC000 File Offset: 0x000BA200
	public bool ValidTarget(BasePlayer newTarget)
	{
		float maxDistance = Vector3.Distance(newTarget.eyes.position, base.transform.position);
		Vector3 direction = Vector3Ex.Direction(newTarget.eyes.position, base.transform.position);
		int layerMask = 10551552;
		if (Physics.Raycast(base.transform.position, direction, maxDistance, layerMask))
		{
			return false;
		}
		if (newTarget.isMounted)
		{
			if (newTarget.GetMountedVehicle())
			{
				return false;
			}
			if (!newTarget.GetMounted().GetComponent<WaterInflatable>().buoyancy.enabled)
			{
				return false;
			}
		}
		else if (!WaterLevel.Test(newTarget.CenterPoint(), true, newTarget))
		{
			return false;
		}
		return true;
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x000BC0A4 File Offset: 0x000BA2A4
	public void ClearTarget()
	{
		this.target = null;
		this.lastSeenTargetTime = 0f;
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x000BC0B8 File Offset: 0x000BA2B8
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (base.isServer)
		{
			if (GameInfo.HasAchievements && hitInfo != null && hitInfo.InitiatorPlayer != null && !hitInfo.InitiatorPlayer.IsNpc && hitInfo.Weapon != null && hitInfo.Weapon.ShortPrefabName.Contains("speargun"))
			{
				hitInfo.InitiatorPlayer.stats.Add("shark_speargun_kills", 1, Stats.All);
				hitInfo.InitiatorPlayer.stats.Save(true);
			}
			BaseCorpse baseCorpse = base.DropCorpse(this.corpsePrefab.resourcePath);
			if (baseCorpse)
			{
				baseCorpse.Spawn();
				baseCorpse.TakeChildren(this);
			}
			base.Invoke(new Action(base.KillMessage), 0.5f);
		}
		base.OnKilled(hitInfo);
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x000BC188 File Offset: 0x000BA388
	public void UpdateTarget(float delta)
	{
		if (this.target != null)
		{
			bool flag = Vector3.Distance(this.target.eyes.position, base.transform.position) > this.aggroRange * 2f;
			bool flag2 = Time.realtimeSinceStartup > this.lastSeenTargetTime + 4f;
			if (!this.ValidTarget(this.target) || flag || flag2)
			{
				this.ClearTarget();
			}
			else
			{
				this.lastSeenTargetTime = Time.realtimeSinceStartup;
			}
		}
		if (Time.realtimeSinceStartup < this.nextTargetSearchTime)
		{
			return;
		}
		if (this.target == null)
		{
			this.nextTargetSearchTime = Time.realtimeSinceStartup + 1f;
			if (BaseNetworkable.HasCloseConnections(base.transform.position, this.aggroRange))
			{
				int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(base.transform.position, this.aggroRange, SimpleShark.playerQueryResults, null);
				for (int i = 0; i < playersInSphere; i++)
				{
					BasePlayer basePlayer = SimpleShark.playerQueryResults[i];
					if (!basePlayer.isClient && this.ValidTarget(basePlayer))
					{
						this.target = basePlayer;
						this.lastSeenTargetTime = Time.realtimeSinceStartup;
						return;
					}
				}
			}
		}
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x000BC2B4 File Offset: 0x000BA4B4
	public float TimeSinceAttacked()
	{
		return Time.realtimeSinceStartup - this.lastTimeAttacked;
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x000BC2C4 File Offset: 0x000BA4C4
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.lastTimeAttacked = Time.realtimeSinceStartup;
		if (info.damageTypes.Total() > 20f)
		{
			this.Startle();
		}
		if (info.InitiatorPlayer != null && this.target == null && this.ValidTarget(info.InitiatorPlayer))
		{
			this.target = info.InitiatorPlayer;
			this.lastSeenTargetTime = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x000BC33C File Offset: 0x000BA53C
	public bool HasTarget()
	{
		return this.target != null;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x000BC34A File Offset: 0x000BA54A
	public BasePlayer GetTarget()
	{
		return this.target;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x000BC352 File Offset: 0x000BA552
	public override string Categorize()
	{
		return "Shark";
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x000BC359 File Offset: 0x000BA559
	public bool CanAttack()
	{
		return Time.realtimeSinceStartup > this.nextAttackTime;
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x000BC368 File Offset: 0x000BA568
	public void DoAttack()
	{
		if (!this.HasTarget())
		{
			return;
		}
		this.GetTarget().Hurt(UnityEngine.Random.Range(30f, 70f), DamageType.Bite, this, true);
		Vector3 posWorld = this.WaterClamp(this.GetTarget().CenterPoint());
		Effect.server.Run(this.bloodCloud.resourcePath, posWorld, Vector3.forward, null, false);
		this.nextAttackTime = Time.realtimeSinceStartup + this.attackCooldown;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x000BC3D8 File Offset: 0x000BA5D8
	public void Startle()
	{
		this.lastStartleTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x000BC3E5 File Offset: 0x000BA5E5
	public bool IsStartled()
	{
		return this.lastStartleTime + this.startleDuration > Time.realtimeSinceStartup;
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x000BC3FB File Offset: 0x000BA5FB
	private float GetDesiredSpeed()
	{
		if (!this.IsStartled())
		{
			return this.minSpeed;
		}
		return this.maxSpeed;
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x000BC412 File Offset: 0x000BA612
	public float GetTurnSpeed()
	{
		if (this.IsStartled())
		{
			return this.maxTurnSpeed;
		}
		if (this.obstacleAvoidanceScale != 0f)
		{
			return Mathf.Lerp(this.minTurnSpeed, this.maxTurnSpeed, this.obstacleAvoidanceScale);
		}
		return this.minTurnSpeed;
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x000BC44E File Offset: 0x000BA64E
	private float GetCurrentSpeed()
	{
		return this.currentSpeed;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x000BC458 File Offset: 0x000BA658
	private void UpdateObstacleAvoidance(float delta)
	{
		this.timeSinceLastObstacleCheck += delta;
		if (this.timeSinceLastObstacleCheck < 0.5f)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		Vector3 position = base.transform.position;
		int layerMask = 1503764737;
		RaycastHit raycastHit;
		if (Physics.SphereCast(position, this.obstacleDetectionRadius, forward, out raycastHit, this.obstacleDetectionRange, layerMask))
		{
			Vector3 point = raycastHit.point;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(position + Vector3.down * 0.25f + base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit2, this.obstacleDetectionRange, layerMask))
			{
				vector = raycastHit2.point;
			}
			RaycastHit raycastHit3;
			if (Physics.SphereCast(position + Vector3.down * 0.25f - base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit3, this.obstacleDetectionRange, layerMask))
			{
				vector2 = raycastHit3.point;
			}
			if (vector != Vector3.zero && vector2 != Vector3.zero)
			{
				Plane plane = new Plane(point, vector, vector2);
				Vector3 normal = plane.normal;
				if (normal != Vector3.zero)
				{
					raycastHit.normal = normal;
				}
			}
			this.cachedObstacleNormal = raycastHit.normal;
			this.cachedObstacleDistance = raycastHit.distance;
			this.obstacleAvoidanceScale = 1f - Mathf.InverseLerp(2f, this.obstacleDetectionRange * 0.75f, raycastHit.distance);
		}
		else
		{
			this.obstacleAvoidanceScale = Mathf.MoveTowards(this.obstacleAvoidanceScale, 0f, this.timeSinceLastObstacleCheck * 2f);
			if (this.obstacleAvoidanceScale == 0f)
			{
				this.cachedObstacleDistance = 0f;
			}
		}
		this.timeSinceLastObstacleCheck = 0f;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x000BC644 File Offset: 0x000BA844
	private void UpdateDirection(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = Vector3Ex.Direction(this.WaterClamp(this.destination), base.transform.position);
		if (this.obstacleAvoidanceScale != 0f)
		{
			Vector3 a;
			if (this.cachedObstacleNormal != Vector3.zero)
			{
				Vector3 lhs = QuaternionEx.LookRotationForcedUp(this.cachedObstacleNormal, Vector3.up) * Vector3.forward;
				if (Vector3.Dot(lhs, base.transform.right) > Vector3.Dot(lhs, -base.transform.right))
				{
					a = base.transform.right;
				}
				else
				{
					a = -base.transform.right;
				}
			}
			else
			{
				a = base.transform.right;
			}
			vector = a * this.obstacleAvoidanceScale;
			vector.Normalize();
		}
		if (vector != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(vector, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, delta * this.GetTurnSpeed());
		}
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x000BC760 File Offset: 0x000BA960
	private void UpdatePosition(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = base.transform.position + forward * this.GetCurrentSpeed() * delta;
		vector = this.WaterClamp(vector);
		base.transform.position = vector;
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x000BC7B0 File Offset: 0x000BA9B0
	private void UpdateSpeed(float delta)
	{
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.GetDesiredSpeed(), delta * 4f);
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x000BC7D0 File Offset: 0x000BA9D0
	public void Update()
	{
		if (base.isServer)
		{
			this.Think(Time.deltaTime);
		}
	}

	// Token: 0x02000C46 RID: 3142
	public class SimpleState
	{
		// Token: 0x040042AD RID: 17069
		public SimpleShark entity;

		// Token: 0x040042AE RID: 17070
		private float stateEnterTime;

		// Token: 0x06004E55 RID: 20053 RVA: 0x001A2C63 File Offset: 0x001A0E63
		public SimpleState(SimpleShark owner)
		{
			this.entity = owner;
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public virtual float State_Weight()
		{
			return 0f;
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x001A2C72 File Offset: 0x001A0E72
		public virtual void State_Enter()
		{
			this.stateEnterTime = Time.realtimeSinceStartup;
		}

		// Token: 0x06004E58 RID: 20056 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void State_Think(float delta)
		{
		}

		// Token: 0x06004E59 RID: 20057 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void State_Exit()
		{
		}

		// Token: 0x06004E5A RID: 20058 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004E5B RID: 20059 RVA: 0x001A2C7F File Offset: 0x001A0E7F
		public virtual float TimeInState()
		{
			return Time.realtimeSinceStartup - this.stateEnterTime;
		}
	}

	// Token: 0x02000C47 RID: 3143
	public class IdleState : SimpleShark.SimpleState
	{
		// Token: 0x040042AF RID: 17071
		private int patrolTargetIndex;

		// Token: 0x06004E5C RID: 20060 RVA: 0x001A2C8D File Offset: 0x001A0E8D
		public IdleState(SimpleShark owner) : base(owner)
		{
		}

		// Token: 0x06004E5D RID: 20061 RVA: 0x001A2C96 File Offset: 0x001A0E96
		public Vector3 GetTargetPatrolPosition()
		{
			return this.entity.patrolPath[this.patrolTargetIndex];
		}

		// Token: 0x06004E5E RID: 20062 RVA: 0x00006CA5 File Offset: 0x00004EA5
		public override float State_Weight()
		{
			return 1f;
		}

		// Token: 0x06004E5F RID: 20063 RVA: 0x001A2CB0 File Offset: 0x001A0EB0
		public override void State_Enter()
		{
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < this.entity.patrolPath.Count; i++)
			{
				float num3 = Vector3.Distance(this.entity.patrolPath[i], this.entity.transform.position);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			this.patrolTargetIndex = num2;
			base.State_Enter();
		}

		// Token: 0x06004E60 RID: 20064 RVA: 0x001A2D1C File Offset: 0x001A0F1C
		public override void State_Think(float delta)
		{
			if (Vector3.Distance(this.GetTargetPatrolPosition(), this.entity.transform.position) < this.entity.stoppingDistance)
			{
				this.patrolTargetIndex++;
				if (this.patrolTargetIndex >= this.entity.patrolPath.Count)
				{
					this.patrolTargetIndex = 0;
				}
			}
			if (this.entity.TimeSinceAttacked() >= 120f && this.entity.healthFraction < 1f)
			{
				this.entity.health = this.entity.MaxHealth();
			}
			this.entity.destination = this.entity.WaterClamp(this.GetTargetPatrolPosition());
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x001A2DD4 File Offset: 0x001A0FD4
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004E62 RID: 20066 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}
	}

	// Token: 0x02000C48 RID: 3144
	public class AttackState : SimpleShark.SimpleState
	{
		// Token: 0x06004E63 RID: 20067 RVA: 0x001A2C8D File Offset: 0x001A0E8D
		public AttackState(SimpleShark owner) : base(owner)
		{
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x001A2DDC File Offset: 0x001A0FDC
		public override float State_Weight()
		{
			if (!this.entity.HasTarget() || !this.entity.CanAttack())
			{
				return 0f;
			}
			return 10f;
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x001A2E03 File Offset: 0x001A1003
		public override void State_Enter()
		{
			base.State_Enter();
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x001A2E0C File Offset: 0x001A100C
		public override void State_Think(float delta)
		{
			BasePlayer target = this.entity.GetTarget();
			if (target == null)
			{
				return;
			}
			if (this.TimeInState() >= 10f)
			{
				this.entity.nextAttackTime = Time.realtimeSinceStartup + 4f;
				this.entity.Startle();
				return;
			}
			if (this.entity.CanAttack())
			{
				this.entity.Startle();
			}
			float num = Vector3.Distance(this.entity.GetTarget().eyes.position, this.entity.transform.position);
			bool flag = num < 4f;
			if (this.entity.CanAttack() && num <= 2f)
			{
				this.entity.DoAttack();
			}
			if (!flag)
			{
				Vector3 a = Vector3Ex.Direction(this.entity.GetTarget().eyes.position, this.entity.transform.position);
				Vector3 vector = target.eyes.position + a * 10f;
				vector = this.entity.WaterClamp(vector);
				this.entity.destination = vector;
			}
		}

		// Token: 0x06004E67 RID: 20071 RVA: 0x001A2DD4 File Offset: 0x001A0FD4
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}
	}
}
