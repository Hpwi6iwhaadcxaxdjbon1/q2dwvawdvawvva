using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class PatrolHelicopterAI : BaseMonoBehaviour
{
	// Token: 0x04001BB4 RID: 7092
	public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();

	// Token: 0x04001BB5 RID: 7093
	public Vector3 interestZoneOrigin;

	// Token: 0x04001BB6 RID: 7094
	public Vector3 destination;

	// Token: 0x04001BB7 RID: 7095
	public bool hasInterestZone;

	// Token: 0x04001BB8 RID: 7096
	public float moveSpeed;

	// Token: 0x04001BB9 RID: 7097
	public float maxSpeed = 25f;

	// Token: 0x04001BBA RID: 7098
	public float courseAdjustLerpTime = 2f;

	// Token: 0x04001BBB RID: 7099
	public Quaternion targetRotation;

	// Token: 0x04001BBC RID: 7100
	public Vector3 windVec;

	// Token: 0x04001BBD RID: 7101
	public Vector3 targetWindVec;

	// Token: 0x04001BBE RID: 7102
	public float windForce = 5f;

	// Token: 0x04001BBF RID: 7103
	public float windFrequency = 1f;

	// Token: 0x04001BC0 RID: 7104
	public float targetThrottleSpeed;

	// Token: 0x04001BC1 RID: 7105
	public float throttleSpeed;

	// Token: 0x04001BC2 RID: 7106
	public float maxRotationSpeed = 90f;

	// Token: 0x04001BC3 RID: 7107
	public float rotationSpeed;

	// Token: 0x04001BC4 RID: 7108
	public float terrainPushForce = 100f;

	// Token: 0x04001BC5 RID: 7109
	public float obstaclePushForce = 100f;

	// Token: 0x04001BC6 RID: 7110
	public HelicopterTurret leftGun;

	// Token: 0x04001BC7 RID: 7111
	public HelicopterTurret rightGun;

	// Token: 0x04001BC8 RID: 7112
	public static PatrolHelicopterAI heliInstance;

	// Token: 0x04001BC9 RID: 7113
	public BaseHelicopter helicopterBase;

	// Token: 0x04001BCA RID: 7114
	public PatrolHelicopterAI.aiState _currentState;

	// Token: 0x04001BCB RID: 7115
	private Vector3 _aimTarget;

	// Token: 0x04001BCC RID: 7116
	private bool movementLockingAiming;

	// Token: 0x04001BCD RID: 7117
	private bool hasAimTarget;

	// Token: 0x04001BCE RID: 7118
	private bool aimDoorSide;

	// Token: 0x04001BCF RID: 7119
	private Vector3 pushVec = Vector3.zero;

	// Token: 0x04001BD0 RID: 7120
	private Vector3 _lastPos;

	// Token: 0x04001BD1 RID: 7121
	private Vector3 _lastMoveDir;

	// Token: 0x04001BD2 RID: 7122
	private bool isDead;

	// Token: 0x04001BD3 RID: 7123
	private bool isRetiring;

	// Token: 0x04001BD4 RID: 7124
	private float spawnTime;

	// Token: 0x04001BD5 RID: 7125
	private float lastDamageTime;

	// Token: 0x04001BD6 RID: 7126
	private float deathTimeout;

	// Token: 0x04001BD7 RID: 7127
	private float destination_min_dist = 2f;

	// Token: 0x04001BD8 RID: 7128
	private float currentOrbitDistance;

	// Token: 0x04001BD9 RID: 7129
	private float currentOrbitTime;

	// Token: 0x04001BDA RID: 7130
	private bool hasEnteredOrbit;

	// Token: 0x04001BDB RID: 7131
	private float orbitStartTime;

	// Token: 0x04001BDC RID: 7132
	private float maxOrbitDuration = 30f;

	// Token: 0x04001BDD RID: 7133
	private bool breakingOrbit;

	// Token: 0x04001BDE RID: 7134
	public List<MonumentInfo> _visitedMonuments;

	// Token: 0x04001BDF RID: 7135
	public float arrivalTime;

	// Token: 0x04001BE0 RID: 7136
	public GameObjectRef rocketProjectile;

	// Token: 0x04001BE1 RID: 7137
	public GameObjectRef rocketProjectile_Napalm;

	// Token: 0x04001BE2 RID: 7138
	private bool leftTubeFiredLast;

	// Token: 0x04001BE3 RID: 7139
	private float lastRocketTime;

	// Token: 0x04001BE4 RID: 7140
	private float timeBetweenRockets = 0.2f;

	// Token: 0x04001BE5 RID: 7141
	private int numRocketsLeft = 12;

	// Token: 0x04001BE6 RID: 7142
	private const int maxRockets = 12;

	// Token: 0x04001BE7 RID: 7143
	private Vector3 strafe_target_position;

	// Token: 0x04001BE8 RID: 7144
	private bool puttingDistance;

	// Token: 0x04001BE9 RID: 7145
	private const float strafe_approach_range = 175f;

	// Token: 0x04001BEA RID: 7146
	private const float strafe_firing_range = 150f;

	// Token: 0x04001BEB RID: 7147
	private bool useNapalm;

	// Token: 0x04001BEC RID: 7148
	[NonSerialized]
	private float lastNapalmTime = float.NegativeInfinity;

	// Token: 0x04001BED RID: 7149
	[NonSerialized]
	private float lastStrafeTime = float.NegativeInfinity;

	// Token: 0x04001BEE RID: 7150
	private float _lastThinkTime;

	// Token: 0x06002381 RID: 9089 RVA: 0x000E2D9C File Offset: 0x000E0F9C
	public void UpdateTargetList()
	{
		Vector3 strafePos = Vector3.zero;
		bool flag = false;
		bool shouldUseNapalm = false;
		for (int i = this._targetList.Count - 1; i >= 0; i--)
		{
			PatrolHelicopterAI.targetinfo targetinfo = this._targetList[i];
			if (targetinfo == null || targetinfo.ent == null)
			{
				this._targetList.Remove(targetinfo);
			}
			else
			{
				if (UnityEngine.Time.realtimeSinceStartup > targetinfo.nextLOSCheck)
				{
					targetinfo.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1f;
					if (this.PlayerVisible(targetinfo.ply))
					{
						targetinfo.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
						targetinfo.visibleFor += 1f;
					}
					else
					{
						targetinfo.visibleFor = 0f;
					}
				}
				bool flag2 = targetinfo.ply ? targetinfo.ply.IsDead() : (targetinfo.ent.Health() <= 0f);
				if (targetinfo.TimeSinceSeen() >= 6f || flag2)
				{
					bool flag3 = UnityEngine.Random.Range(0f, 1f) >= 0f;
					if ((this.CanStrafe() || this.CanUseNapalm()) && this.IsAlive() && !flag && !flag2 && (targetinfo.ply == this.leftGun._target || targetinfo.ply == this.rightGun._target) && flag3)
					{
						shouldUseNapalm = (!this.ValidStrafeTarget(targetinfo.ply) || UnityEngine.Random.Range(0f, 1f) > 0.75f);
						flag = true;
						strafePos = targetinfo.ply.transform.position;
					}
					this._targetList.Remove(targetinfo);
				}
			}
		}
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.InSafeZone() && Vector3Ex.Distance2D(base.transform.position, basePlayer.transform.position) <= 150f)
			{
				bool flag4 = false;
				using (List<PatrolHelicopterAI.targetinfo>.Enumerator enumerator2 = this._targetList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.ply == basePlayer)
						{
							flag4 = true;
							break;
						}
					}
				}
				if (!flag4 && basePlayer.GetThreatLevel() > 0.5f && this.PlayerVisible(basePlayer))
				{
					this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
				}
			}
		}
		if (flag)
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(strafePos, shouldUseNapalm);
		}
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x000E3078 File Offset: 0x000E1278
	public bool PlayerVisible(BasePlayer ply)
	{
		Vector3 position = ply.eyes.position;
		if (TOD_Sky.Instance.IsNight && Vector3.Distance(position, this.interestZoneOrigin) > 40f)
		{
			return false;
		}
		Vector3 vector = base.transform.position - Vector3.up * 6f;
		float num = Vector3.Distance(position, vector);
		Vector3 normalized = (position - vector).normalized;
		RaycastHit raycastHit;
		return GamePhysics.Trace(new Ray(vector + normalized * 5f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == ply;
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000E3140 File Offset: 0x000E1340
	public void WasAttacked(HitInfo info)
	{
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if (basePlayer != null)
		{
			this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
		}
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000E3174 File Offset: 0x000E1374
	public void Awake()
	{
		if (PatrolHelicopter.lifetimeMinutes == 0f)
		{
			base.Invoke(new Action(this.DestroyMe), 1f);
			return;
		}
		base.InvokeRepeating(new Action(this.UpdateWind), 0f, 1f / this.windFrequency);
		this._lastPos = base.transform.position;
		this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		this.InitializeAI();
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000E31EC File Offset: 0x000E13EC
	public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
	{
		this.hasInterestZone = true;
		this.interestZoneOrigin = dest;
		float x = TerrainMeta.Size.x;
		float y = dest.y + 25f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * mapScaleDistance;
		vector.y = y;
		if (mapScaleDistance == 0f)
		{
			vector = this.interestZoneOrigin + new Vector3(0f, 10f, 0f);
		}
		base.transform.position = vector;
		this.ExitCurrentState();
		this.State_Move_Enter(dest);
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000E3298 File Offset: 0x000E1498
	public void Retire()
	{
		if (this.isRetiring)
		{
			return;
		}
		this.isRetiring = true;
		base.Invoke(new Action(this.DestroyMe), 240f);
		float x = TerrainMeta.Size.x;
		float y = 200f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 20f;
		vector.y = y;
		this.ExitCurrentState();
		this.State_Move_Enter(vector);
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000E3324 File Offset: 0x000E1524
	public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
	{
		float num = (rotationSpeedOverride == -1f) ? Mathf.Clamp01(this.moveSpeed / (this.maxSpeed * 0.5f)) : rotationSpeedOverride;
		this.rotationSpeed = num * this.maxRotationSpeed;
		this.targetRotation = newTargetRot;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000E336C File Offset: 0x000E156C
	public Quaternion GetYawRotationTo(Vector3 targetDest)
	{
		Vector3 a = targetDest;
		a.y = 0f;
		Vector3 position = base.transform.position;
		position.y = 0f;
		return Quaternion.LookRotation((a - position).normalized);
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000E33B4 File Offset: 0x000E15B4
	public void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
	{
		this.destination = targetDest;
		this.destination_min_dist = minDist;
		float num = Vector3.Distance(targetDest, base.transform.position);
		if (num > minDistForFacingRotation && !this.IsTargeting())
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.destination), -1f);
		}
		this.targetThrottleSpeed = this.GetThrottleForDistance(num);
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000E3411 File Offset: 0x000E1611
	public bool AtDestination()
	{
		return Vector3.Distance(base.transform.position, this.destination) < this.destination_min_dist;
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x000E3434 File Offset: 0x000E1634
	public void MoveToDestination()
	{
		Vector3 vector = Vector3.Lerp(this._lastMoveDir, (this.destination - base.transform.position).normalized, UnityEngine.Time.deltaTime / this.courseAdjustLerpTime);
		this._lastMoveDir = vector;
		this.throttleSpeed = Mathf.Lerp(this.throttleSpeed, this.targetThrottleSpeed, UnityEngine.Time.deltaTime / 3f);
		float d = this.throttleSpeed * this.maxSpeed;
		this.TerrainPushback();
		base.transform.position += vector * d * UnityEngine.Time.deltaTime;
		this.windVec = Vector3.Lerp(this.windVec, this.targetWindVec, UnityEngine.Time.deltaTime);
		base.transform.position += this.windVec * this.windForce * UnityEngine.Time.deltaTime;
		this.moveSpeed = Mathf.Lerp(this.moveSpeed, Vector3.Distance(this._lastPos, base.transform.position) / UnityEngine.Time.deltaTime, UnityEngine.Time.deltaTime * 2f);
		this._lastPos = base.transform.position;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000E3574 File Offset: 0x000E1774
	public void TerrainPushback()
	{
		if (this._currentState == PatrolHelicopterAI.aiState.DEATH)
		{
			return;
		}
		Vector3 vector = base.transform.position + new Vector3(0f, 2f, 0f);
		Vector3 normalized = (this.destination - vector).normalized;
		float b = Vector3.Distance(this.destination, base.transform.position);
		Ray ray = new Ray(vector, normalized);
		float num = 5f;
		float num2 = Mathf.Min(100f, b);
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		Vector3 vector2 = Vector3.zero;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num, out raycastHit, num2 - num * 0.5f, mask))
		{
			float num3 = 1f - raycastHit.distance / num2;
			float d = this.terrainPushForce * num3;
			vector2 = Vector3.up * d;
		}
		Ray ray2 = new Ray(vector, this._lastMoveDir);
		float num4 = Mathf.Min(10f, b);
		RaycastHit raycastHit2;
		if (UnityEngine.Physics.SphereCast(ray2, num, out raycastHit2, num4 - num * 0.5f, mask))
		{
			float num5 = 1f - raycastHit2.distance / num4;
			float d2 = this.obstaclePushForce * num5;
			vector2 += this._lastMoveDir * d2 * -1f;
			vector2 += Vector3.up * d2;
		}
		this.pushVec = Vector3.Lerp(this.pushVec, vector2, UnityEngine.Time.deltaTime);
		base.transform.position += this.pushVec * UnityEngine.Time.deltaTime;
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000E372C File Offset: 0x000E192C
	public void UpdateRotation()
	{
		if (this.hasAimTarget)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			Vector3 aimTarget = this._aimTarget;
			aimTarget.y = 0f;
			Vector3 normalized = (aimTarget - position).normalized;
			Vector3 vector = Vector3.Cross(normalized, Vector3.up);
			float num = Vector3.Angle(normalized, base.transform.right);
			float num2 = Vector3.Angle(normalized, -base.transform.right);
			if (this.aimDoorSide)
			{
				if (num < num2)
				{
					this.targetRotation = Quaternion.LookRotation(vector);
				}
				else
				{
					this.targetRotation = Quaternion.LookRotation(-vector);
				}
			}
			else
			{
				this.targetRotation = Quaternion.LookRotation(normalized);
			}
		}
		this.rotationSpeed = Mathf.Lerp(this.rotationSpeed, this.maxRotationSpeed, UnityEngine.Time.deltaTime / 2f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.rotationSpeed * UnityEngine.Time.deltaTime);
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000E3844 File Offset: 0x000E1A44
	public void UpdateSpotlight()
	{
		if (this.hasInterestZone)
		{
			this.helicopterBase.spotlightTarget = new Vector3(this.interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(this.interestZoneOrigin), this.interestZoneOrigin.z);
			return;
		}
		this.helicopterBase.spotlightTarget = Vector3.zero;
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000E38A0 File Offset: 0x000E1AA0
	public void Update()
	{
		if (this.helicopterBase.isClient)
		{
			return;
		}
		PatrolHelicopterAI.heliInstance = this;
		this.UpdateTargetList();
		this.MoveToDestination();
		this.UpdateRotation();
		this.UpdateSpotlight();
		this.AIThink();
		this.DoMachineGuns();
		if (!this.isRetiring)
		{
			float num = Mathf.Max(this.spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, this.lastDamageTime + 120f);
			if (UnityEngine.Time.realtimeSinceStartup > num)
			{
				this.Retire();
			}
		}
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000E3920 File Offset: 0x000E1B20
	public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastDamageTime;
		this.lastDamageTime = UnityEngine.Time.realtimeSinceStartup;
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		bool flag = this.ValidStrafeTarget(basePlayer);
		bool flag2 = flag && this.CanStrafe();
		bool flag3 = !flag && this.CanUseNapalm();
		if (num < 5f && basePlayer != null && (flag2 || flag3))
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(info.Initiator.transform.position, flag3);
		}
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000E39A2 File Offset: 0x000E1BA2
	public void CriticalDamage()
	{
		this.isDead = true;
		this.ExitCurrentState();
		this.State_Death_Enter();
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000E39B8 File Offset: 0x000E1BB8
	public void DoMachineGuns()
	{
		if (this._targetList.Count > 0)
		{
			if (this.leftGun.NeedsNewTarget())
			{
				this.leftGun.UpdateTargetFromList(this._targetList);
			}
			if (this.rightGun.NeedsNewTarget())
			{
				this.rightGun.UpdateTargetFromList(this._targetList);
			}
		}
		this.leftGun.TurretThink();
		this.rightGun.TurretThink();
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000E3A28 File Offset: 0x000E1C28
	public void FireGun(Vector3 targetPos, float aimCone, bool left)
	{
		if (PatrolHelicopter.guns == 0)
		{
			return;
		}
		Vector3 vector = (left ? this.helicopterBase.left_gun_muzzle.transform : this.helicopterBase.right_gun_muzzle.transform).position;
		Vector3 normalized = (targetPos - vector).normalized;
		vector += normalized * 2f;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
		RaycastHit hit;
		if (GamePhysics.Trace(new Ray(vector, modifiedAimConeDirection), 0f, out hit, 300f, 1219701521, QueryTriggerInteraction.UseGlobal, null))
		{
			targetPos = hit.point;
			if (hit.collider)
			{
				BaseEntity entity = hit.GetEntity();
				if (entity && entity != this.helicopterBase)
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					HitInfo info = new HitInfo(this.helicopterBase, entity, DamageType.Bullet, this.helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, hit.point);
					if (baseCombatEntity)
					{
						baseCombatEntity.OnAttacked(info);
						if (baseCombatEntity is BasePlayer)
						{
							Effect.server.ImpactEffect(new HitInfo
							{
								HitPositionWorld = hit.point - modifiedAimConeDirection * 0.25f,
								HitNormalWorld = -modifiedAimConeDirection,
								HitMaterial = StringPool.Get("Flesh")
							});
						}
					}
					else
					{
						entity.OnAttacked(info);
					}
				}
			}
		}
		else
		{
			targetPos = vector + modifiedAimConeDirection * 300f;
		}
		this.helicopterBase.ClientRPC<bool, Vector3>(null, "FireGun", left, targetPos);
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000E3BBB File Offset: 0x000E1DBB
	public bool CanInterruptState()
	{
		return this._currentState != PatrolHelicopterAI.aiState.STRAFE && this._currentState != PatrolHelicopterAI.aiState.DEATH;
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000E3BD4 File Offset: 0x000E1DD4
	public bool IsAlive()
	{
		return !this.isDead;
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000E3BDF File Offset: 0x000E1DDF
	public void DestroyMe()
	{
		this.helicopterBase.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000E3BED File Offset: 0x000E1DED
	public Vector3 GetLastMoveDir()
	{
		return this._lastMoveDir;
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000E3BF8 File Offset: 0x000E1DF8
	public Vector3 GetMoveDirection()
	{
		return (this.destination - base.transform.position).normalized;
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000E3C23 File Offset: 0x000E1E23
	public float GetMoveSpeed()
	{
		return this.moveSpeed;
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000E3C2B File Offset: 0x000E1E2B
	public float GetMaxRotationSpeed()
	{
		return this.maxRotationSpeed;
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000E3C33 File Offset: 0x000E1E33
	public bool IsTargeting()
	{
		return this.hasAimTarget;
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000E3C3B File Offset: 0x000E1E3B
	public void UpdateWind()
	{
		this.targetWindVec = UnityEngine.Random.onUnitSphere;
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000E3C48 File Offset: 0x000E1E48
	public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
	{
		if (this.movementLockingAiming)
		{
			return;
		}
		this.hasAimTarget = true;
		this._aimTarget = aimTarg;
		this.aimDoorSide = isDoorSide;
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000E3C68 File Offset: 0x000E1E68
	public void ClearAimTarget()
	{
		this.hasAimTarget = false;
		this._aimTarget = Vector3.zero;
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000E3C7C File Offset: 0x000E1E7C
	public void State_Death_Think(float timePassed)
	{
		float num = UnityEngine.Time.realtimeSinceStartup * 0.25f;
		float x = Mathf.Sin(6.2831855f * num) * 10f;
		float z = Mathf.Cos(6.2831855f * num) * 10f;
		Vector3 b = new Vector3(x, 0f, z);
		this.SetAimTarget(base.transform.position + b, true);
		Ray ray = new Ray(base.transform.position, this.GetLastMoveDir());
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, 3f, out raycastHit, 5f, mask) || UnityEngine.Time.realtimeSinceStartup > this.deathTimeout)
		{
			this.helicopterBase.Hurt(this.helicopterBase.health * 2f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000E3D6C File Offset: 0x000E1F6C
	public void State_Death_Enter()
	{
		this.maxRotationSpeed *= 8f;
		this._currentState = PatrolHelicopterAI.aiState.DEATH;
		Vector3 randomOffset = this.GetRandomOffset(base.transform.position, 20f, 60f, 20f, 30f);
		int intVal = 1236478737;
		Vector3 targetDest;
		Vector3 vector;
		TransformUtil.GetGroundInfo(randomOffset - Vector3.up * 2f, out targetDest, out vector, 500f, intVal, null);
		this.SetTargetDestination(targetDest, 5f, 30f);
		this.targetThrottleSpeed = 0.5f;
		this.deathTimeout = UnityEngine.Time.realtimeSinceStartup + 10f;
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Death_Leave()
	{
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000E3E14 File Offset: 0x000E2014
	public void State_Idle_Think(float timePassed)
	{
		this.ExitCurrentState();
		this.State_Patrol_Enter();
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000E3E22 File Offset: 0x000E2022
	public void State_Idle_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Idle_Leave()
	{
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000E3E2C File Offset: 0x000E202C
	public void State_Move_Think(float timePassed)
	{
		float distToTarget = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(distToTarget);
		if (this.AtDestination())
		{
			this.ExitCurrentState();
			this.State_Idle_Enter();
		}
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000E3E74 File Offset: 0x000E2074
	public void State_Move_Enter(Vector3 newPos)
	{
		this._currentState = PatrolHelicopterAI.aiState.MOVE;
		this.destination_min_dist = 5f;
		this.SetTargetDestination(newPos, 5f, 30f);
		float distToTarget = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(distToTarget);
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Move_Leave()
	{
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000E3EC8 File Offset: 0x000E20C8
	public void State_Orbit_Think(float timePassed)
	{
		if (this.breakingOrbit)
		{
			if (this.AtDestination())
			{
				this.ExitCurrentState();
				this.State_Idle_Enter();
			}
		}
		else
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.destination) > 15f)
			{
				return;
			}
			if (!this.hasEnteredOrbit)
			{
				this.hasEnteredOrbit = true;
				this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			float num = 6.2831855f * this.currentOrbitDistance;
			float num2 = 0.5f * this.maxSpeed;
			float num3 = num / num2;
			this.currentOrbitTime += timePassed / (num3 * 1.01f);
			float rate = this.currentOrbitTime;
			Vector3 orbitPosition = this.GetOrbitPosition(rate);
			this.ClearAimTarget();
			this.SetTargetDestination(orbitPosition, 0f, 1f);
			this.targetThrottleSpeed = 0.5f;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.orbitStartTime > this.maxOrbitDuration && !this.breakingOrbit)
		{
			this.breakingOrbit = true;
			Vector3 appropriatePosition = this.GetAppropriatePosition(base.transform.position + base.transform.forward * 75f, 40f, 50f);
			this.SetTargetDestination(appropriatePosition, 10f, 0f);
		}
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000E4000 File Offset: 0x000E2200
	public Vector3 GetOrbitPosition(float rate)
	{
		float x = Mathf.Sin(6.2831855f * rate) * this.currentOrbitDistance;
		float z = Mathf.Cos(6.2831855f * rate) * this.currentOrbitDistance;
		Vector3 vector = new Vector3(x, 20f, z);
		vector = this.interestZoneOrigin + vector;
		return vector;
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000E4054 File Offset: 0x000E2254
	public void State_Orbit_Enter(float orbitDistance)
	{
		this._currentState = PatrolHelicopterAI.aiState.ORBIT;
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
		Vector3 vector = base.transform.position - this.interestZoneOrigin;
		this.currentOrbitTime = Mathf.Atan2(vector.x, vector.z);
		this.currentOrbitDistance = orbitDistance;
		this.ClearAimTarget();
		this.SetTargetDestination(this.GetOrbitPosition(this.currentOrbitTime), 20f, 0f);
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000E40D8 File Offset: 0x000E22D8
	public void State_Orbit_Leave()
	{
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.currentOrbitTime = 0f;
		this.ClearAimTarget();
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000E40FC File Offset: 0x000E22FC
	public Vector3 GetRandomPatrolDestination()
	{
		Vector3 vector = Vector3.zero;
		if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			MonumentInfo monumentInfo = null;
			if (this._visitedMonuments.Count > 0)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (!monumentInfo2.IsSafeZone)
					{
						bool flag = false;
						foreach (MonumentInfo y in this._visitedMonuments)
						{
							if (monumentInfo2 == y)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							monumentInfo = monumentInfo2;
							break;
						}
					}
				}
			}
			if (monumentInfo == null)
			{
				this._visitedMonuments.Clear();
				for (int i = 0; i < 5; i++)
				{
					monumentInfo = TerrainMeta.Path.Monuments[UnityEngine.Random.Range(0, TerrainMeta.Path.Monuments.Count)];
					if (!monumentInfo.IsSafeZone)
					{
						break;
					}
				}
			}
			if (monumentInfo)
			{
				vector = monumentInfo.transform.position;
				this._visitedMonuments.Add(monumentInfo);
				vector.y = TerrainMeta.HeightMap.GetHeight(vector) + 200f;
				RaycastHit raycastHit;
				if (TransformUtil.GetGroundInfo(vector, out raycastHit, 300f, 1235288065, null))
				{
					vector.y = raycastHit.point.y;
				}
				vector.y += 30f;
			}
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float y2 = 30f;
			vector = Vector3Ex.Range(-1f, 1f);
			vector.y = 0f;
			vector.Normalize();
			vector *= x * UnityEngine.Random.Range(0f, 0.75f);
			vector.y = y2;
		}
		return vector;
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000E431C File Offset: 0x000E251C
	public void State_Patrol_Think(float timePassed)
	{
		float num = Vector3Ex.Distance2D(base.transform.position, this.destination);
		if (num <= 25f)
		{
			this.targetThrottleSpeed = this.GetThrottleForDistance(num);
		}
		else
		{
			this.targetThrottleSpeed = 0.5f;
		}
		if (this.AtDestination() && this.arrivalTime == 0f)
		{
			this.arrivalTime = UnityEngine.Time.realtimeSinceStartup;
			this.ExitCurrentState();
			this.maxOrbitDuration = 20f;
			this.State_Orbit_Enter(75f);
		}
		if (this._targetList.Count > 0)
		{
			this.interestZoneOrigin = this._targetList[0].ply.transform.position + new Vector3(0f, 20f, 0f);
			this.ExitCurrentState();
			this.maxOrbitDuration = 10f;
			this.State_Orbit_Enter(75f);
		}
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000E4404 File Offset: 0x000E2604
	public void State_Patrol_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.PATROL;
		Vector3 randomPatrolDestination = this.GetRandomPatrolDestination();
		this.SetTargetDestination(randomPatrolDestination, 10f, 30f);
		this.interestZoneOrigin = randomPatrolDestination;
		this.arrivalTime = 0f;
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000063A5 File Offset: 0x000045A5
	public void State_Patrol_Leave()
	{
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000E4442 File Offset: 0x000E2642
	public int ClipRocketsLeft()
	{
		return this.numRocketsLeft;
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000E444A File Offset: 0x000E264A
	public bool CanStrafe()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastStrafeTime >= 20f && this.CanInterruptState();
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000E4467 File Offset: 0x000E2667
	public bool CanUseNapalm()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastNapalmTime >= 30f;
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000E4480 File Offset: 0x000E2680
	public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
	{
		if (this.CanUseNapalm() && shouldUseNapalm)
		{
			this.useNapalm = shouldUseNapalm;
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		this._currentState = PatrolHelicopterAI.aiState.STRAFE;
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		Vector3 vector;
		Vector3 vector2;
		if (TransformUtil.GetGroundInfo(strafePos, out vector, out vector2, 100f, mask, base.transform))
		{
			this.strafe_target_position = vector;
		}
		else
		{
			this.strafe_target_position = strafePos;
		}
		this.numRocketsLeft = 12;
		this.lastRocketTime = 0f;
		this.movementLockingAiming = true;
		Vector3 randomOffset = this.GetRandomOffset(strafePos, 175f, 192.5f, 20f, 30f);
		this.SetTargetDestination(randomOffset, 10f, 30f);
		this.SetIdealRotation(this.GetYawRotationTo(randomOffset), -1f);
		this.puttingDistance = true;
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000E4574 File Offset: 0x000E2774
	public void State_Strafe_Think(float timePassed)
	{
		if (this.puttingDistance)
		{
			if (this.AtDestination())
			{
				this.puttingDistance = false;
				this.SetTargetDestination(this.strafe_target_position + new Vector3(0f, 40f, 0f), 10f, 30f);
				this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
				return;
			}
		}
		else
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
			float num = Vector3Ex.Distance2D(this.strafe_target_position, base.transform.position);
			if (num <= 150f && this.ClipRocketsLeft() > 0 && UnityEngine.Time.realtimeSinceStartup - this.lastRocketTime > this.timeBetweenRockets)
			{
				float num2 = Vector3.Distance(this.strafe_target_position, base.transform.position) - 10f;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (!UnityEngine.Physics.Raycast(base.transform.position, (this.strafe_target_position - base.transform.position).normalized, num2, LayerMask.GetMask(new string[]
				{
					"Terrain",
					"World"
				})))
				{
					this.FireRocket();
				}
			}
			if (this.ClipRocketsLeft() <= 0 || num <= 15f)
			{
				this.ExitCurrentState();
				this.State_Move_Enter(this.GetAppropriatePosition(this.strafe_target_position + base.transform.forward * 120f, 20f, 30f));
			}
		}
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000E470B File Offset: 0x000E290B
	public bool ValidStrafeTarget(BasePlayer ply)
	{
		return !ply.IsNearEnemyBase();
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000E4716 File Offset: 0x000E2916
	public void State_Strafe_Leave()
	{
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.useNapalm)
		{
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.useNapalm = false;
		this.movementLockingAiming = false;
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000E4744 File Offset: 0x000E2944
	public void FireRocket()
	{
		this.numRocketsLeft--;
		this.lastRocketTime = UnityEngine.Time.realtimeSinceStartup;
		float num = 4f;
		bool flag = this.leftTubeFiredLast;
		this.leftTubeFiredLast = !this.leftTubeFiredLast;
		Transform transform = flag ? this.helicopterBase.rocket_tube_left.transform : this.helicopterBase.rocket_tube_right.transform;
		Vector3 vector = transform.position + transform.forward * 1f;
		Vector3 vector2 = (this.strafe_target_position - vector).normalized;
		if (num > 0f)
		{
			vector2 = AimConeUtil.GetModifiedAimConeDirection(num, vector2, true);
		}
		float maxDistance = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector, vector2, out raycastHit, maxDistance, 1236478737))
		{
			maxDistance = raycastHit.distance - 0.1f;
		}
		Effect.server.Run(this.helicopterBase.rocket_fire_effect.resourcePath, this.helicopterBase, StringPool.Get(flag ? "rocket_tube_left" : "rocket_tube_right"), Vector3.zero, Vector3.forward, null, true);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.useNapalm ? this.rocketProjectile_Napalm.resourcePath : this.rocketProjectile.resourcePath, vector, default(Quaternion), true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(vector2 * component.speed);
		}
		baseEntity.Spawn();
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000E48C7 File Offset: 0x000E2AC7
	public void InitializeAI()
	{
		this._lastThinkTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000E48D4 File Offset: 0x000E2AD4
	public void OnCurrentStateExit()
	{
		switch (this._currentState)
		{
		default:
			this.State_Idle_Leave();
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Leave();
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Leave();
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Leave();
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Leave();
			return;
		}
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000E4924 File Offset: 0x000E2B24
	public void ExitCurrentState()
	{
		this.OnCurrentStateExit();
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000E4933 File Offset: 0x000E2B33
	public float GetTime()
	{
		return UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000E493C File Offset: 0x000E2B3C
	public void AIThink()
	{
		float time = this.GetTime();
		float timePassed = time - this._lastThinkTime;
		this._lastThinkTime = time;
		switch (this._currentState)
		{
		default:
			this.State_Idle_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.DEATH:
			this.State_Death_Think(timePassed);
			return;
		}
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000E49B8 File Offset: 0x000E2BB8
	public Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		maxRange = Mathf.Max(minRange, maxRange);
		Vector3 origin2 = origin + onUnitSphere * UnityEngine.Random.Range(minRange, maxRange);
		return this.GetAppropriatePosition(origin2, minHeight, maxHeight);
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000E4A08 File Offset: 0x000E2C08
	public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
	{
		float num = 100f;
		Ray ray = new Ray(origin + new Vector3(0f, num, 0f), Vector3.down);
		float num2 = 5f;
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num2, out raycastHit, num * 2f - num2, mask))
		{
			origin = raycastHit.point;
		}
		origin.y += UnityEngine.Random.Range(minHeight, maxHeight);
		return origin;
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000E4AA0 File Offset: 0x000E2CA0
	public float GetThrottleForDistance(float distToTarget)
	{
		float result;
		if (distToTarget >= 75f)
		{
			result = 1f;
		}
		else if (distToTarget >= 50f)
		{
			result = 0.75f;
		}
		else if (distToTarget >= 25f)
		{
			result = 0.33f;
		}
		else if (distToTarget >= 5f)
		{
			result = 0.05f;
		}
		else
		{
			result = 0.05f * (1f - distToTarget / 5f);
		}
		return result;
	}

	// Token: 0x02000CDB RID: 3291
	public class targetinfo
	{
		// Token: 0x0400452D RID: 17709
		public BasePlayer ply;

		// Token: 0x0400452E RID: 17710
		public BaseEntity ent;

		// Token: 0x0400452F RID: 17711
		public float lastSeenTime = float.PositiveInfinity;

		// Token: 0x04004530 RID: 17712
		public float visibleFor;

		// Token: 0x04004531 RID: 17713
		public float nextLOSCheck;

		// Token: 0x06004FC3 RID: 20419 RVA: 0x001A70CD File Offset: 0x001A52CD
		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			this.ply = initPly;
			this.ent = initEnt;
			this.lastSeenTime = float.PositiveInfinity;
			this.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1.5f;
		}

		// Token: 0x06004FC4 RID: 20420 RVA: 0x001A710A File Offset: 0x001A530A
		public bool IsVisible()
		{
			return this.TimeSinceSeen() < 1.5f;
		}

		// Token: 0x06004FC5 RID: 20421 RVA: 0x001A7119 File Offset: 0x001A5319
		public float TimeSinceSeen()
		{
			return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTime;
		}
	}

	// Token: 0x02000CDC RID: 3292
	public enum aiState
	{
		// Token: 0x04004533 RID: 17715
		IDLE,
		// Token: 0x04004534 RID: 17716
		MOVE,
		// Token: 0x04004535 RID: 17717
		ORBIT,
		// Token: 0x04004536 RID: 17718
		STRAFE,
		// Token: 0x04004537 RID: 17719
		PATROL,
		// Token: 0x04004538 RID: 17720
		GUARD,
		// Token: 0x04004539 RID: 17721
		DEATH
	}
}
