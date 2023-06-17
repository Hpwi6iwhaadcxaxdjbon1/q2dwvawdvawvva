using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x0200004A RID: 74
public class BaseVehicle : BaseMountable
{
	// Token: 0x04000560 RID: 1376
	private const float MIN_TIME_BETWEEN_PUSHES = 1f;

	// Token: 0x04000561 RID: 1377
	public TimeSince timeSinceLastPush;

	// Token: 0x04000562 RID: 1378
	private bool prevSleeping;

	// Token: 0x04000563 RID: 1379
	private float nextCollisionFXTime;

	// Token: 0x04000564 RID: 1380
	private CollisionDetectionMode savedCollisionDetectionMode;

	// Token: 0x04000565 RID: 1381
	private ProtoBuf.BaseVehicle pendingLoad;

	// Token: 0x04000566 RID: 1382
	private Queue<global::BasePlayer> recentDrivers = new Queue<global::BasePlayer>();

	// Token: 0x04000567 RID: 1383
	private Action clearRecentDriverAction;

	// Token: 0x04000568 RID: 1384
	private float safeAreaRadius;

	// Token: 0x04000569 RID: 1385
	private Vector3 safeAreaOrigin;

	// Token: 0x0400056A RID: 1386
	private float spawnTime = -1f;

	// Token: 0x0400056B RID: 1387
	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	// Token: 0x0400056C RID: 1388
	public global::BaseVehicle.ClippingCheckMode clippingChecks;

	// Token: 0x0400056D RID: 1389
	public bool checkVehicleClipping;

	// Token: 0x0400056E RID: 1390
	public global::BaseVehicle.DismountStyle dismountStyle;

	// Token: 0x0400056F RID: 1391
	public bool shouldShowHudHealth;

	// Token: 0x04000570 RID: 1392
	public bool ignoreDamageFromOutside;

	// Token: 0x04000571 RID: 1393
	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	// Token: 0x04000572 RID: 1394
	[Header("Mount Points")]
	public List<global::BaseVehicle.MountPointInfo> mountPoints;

	// Token: 0x04000573 RID: 1395
	public bool doClippingAndVisChecks = true;

	// Token: 0x04000574 RID: 1396
	[Header("Damage")]
	public DamageRenderer damageRenderer;

	// Token: 0x04000575 RID: 1397
	[FormerlySerializedAs("explosionDamageMultiplier")]
	public float explosionForceMultiplier = 400f;

	// Token: 0x04000576 RID: 1398
	public float explosionForceMax = 75000f;

	// Token: 0x04000577 RID: 1399
	public const global::BaseEntity.Flags Flag_OnlyOwnerEntry = global::BaseEntity.Flags.Locked;

	// Token: 0x04000578 RID: 1400
	public const global::BaseEntity.Flags Flag_Headlights = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000579 RID: 1401
	public const global::BaseEntity.Flags Flag_Stationary = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400057A RID: 1402
	public const global::BaseEntity.Flags Flag_SeatsFull = global::BaseEntity.Flags.Reserved11;

	// Token: 0x0400057B RID: 1403
	protected const global::BaseEntity.Flags Flag_AnyMounted = global::BaseEntity.Flags.InUse;

	// Token: 0x0400057C RID: 1404
	private readonly List<global::BaseVehicle> childVehicles = new List<global::BaseVehicle>(0);

	// Token: 0x060007D5 RID: 2005 RVA: 0x0004ECD0 File Offset: 0x0004CED0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseVehicle.OnRpcMessage", 0))
		{
			if (rpc == 2115395408U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsPush ");
				}
				using (TimeWarning.New("RPC_WantsPush", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2115395408U, "RPC_WantsPush", this, player, 5f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsPush(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_WantsPush");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060007D6 RID: 2006 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool AlwaysAllowBradleyTargeting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0004EE38 File Offset: 0x0004D038
	protected bool RecentlyPushed
	{
		get
		{
			return this.timeSinceLastPush < 1f;
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0004EE4C File Offset: 0x0004D04C
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0004EE7C File Offset: 0x0004D07C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.ClearOwnerEntry();
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0004EE90 File Offset: 0x0004D090
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.baseVehicle = Facepunch.Pool.Get<ProtoBuf.BaseVehicle>();
			info.msg.baseVehicle.mountPoints = Facepunch.Pool.GetList<ProtoBuf.BaseVehicle.MountPoint>();
			for (int i = 0; i < this.mountPoints.Count; i++)
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[i];
				if (!(mountPointInfo.mountable == null))
				{
					ProtoBuf.BaseVehicle.MountPoint mountPoint = Facepunch.Pool.Get<ProtoBuf.BaseVehicle.MountPoint>();
					mountPoint.index = i;
					mountPoint.mountableId = mountPointInfo.mountable.net.ID;
					info.msg.baseVehicle.mountPoints.Add(mountPoint);
				}
			}
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0004EF4C File Offset: 0x0004D14C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.baseVehicle != null)
		{
			ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
			if (baseVehicle != null)
			{
				baseVehicle.Dispose();
			}
			this.pendingLoad = info.msg.baseVehicle;
			info.msg.baseVehicle = null;
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060007DD RID: 2013 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0004EFAC File Offset: 0x0004D1AC
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.clippingChecks != global::BaseVehicle.ClippingCheckMode.OnMountOnly && this.AnyMounted() && UnityEngine.Physics.OverlapBox(base.transform.TransformPoint(this.bounds.center), this.bounds.extents, base.transform.rotation, this.GetClipCheckMask()).Length != 0)
		{
			this.CheckSeatsForClipping();
		}
		if (this.rigidBody != null)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, this.DetermineIfStationary(), false, true);
			bool flag = this.rigidBody.IsSleeping();
			if (this.prevSleeping && !flag)
			{
				this.OnServerWake();
			}
			else if (!this.prevSleeping && flag)
			{
				this.OnServerSleep();
			}
			this.prevSleeping = flag;
		}
		if (this.OnlyOwnerAccessible() && this.safeAreaRadius != -1f && Vector3.Distance(base.transform.position, this.safeAreaOrigin) > this.safeAreaRadius)
		{
			this.ClearOwnerEntry();
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004F0A4 File Offset: 0x0004D2A4
	private int GetClipCheckMask()
	{
		int num = this.IsFlipped() ? 1218511105 : 1210122497;
		if (this.checkVehicleClipping)
		{
			num |= 8192;
		}
		return num;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004F0D7 File Offset: 0x0004D2D7
	protected virtual bool DetermineIfStationary()
	{
		return this.rigidBody.IsSleeping() && !this.AnyMounted();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004F0F1 File Offset: 0x0004D2F1
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004F114 File Offset: 0x0004D314
	public override Quaternion GetAngularVelocityServer()
	{
		if (this.rigidBody == null)
		{
			return Quaternion.identity;
		}
		if (this.rigidBody.angularVelocity.sqrMagnitude < 0.025f)
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(this.rigidBody.angularVelocity, base.transform.up);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x000445C9 File Offset: 0x000427C9
	public virtual int StartingFuelUnits()
	{
		return -1;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0004F170 File Offset: 0x0004D370
	public bool InSafeZone()
	{
		return global::BaseVehicle.InSafeZone(this.triggers, base.transform.position);
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004F188 File Offset: 0x0004D388
	public static bool InSafeZone(List<TriggerBase> triggers, Vector3 position)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null && !activeGameMode.safeZone)
		{
			return false;
		}
		float num = 0f;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!(triggerSafeZone == null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004F1F8 File Offset: 0x0004D3F8
	public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
	{
		if (!this.doClippingAndVisChecks)
		{
			return true;
		}
		if (mountable == null)
		{
			return false;
		}
		Vector3 p = mountable.transform.position + base.transform.up * 0.15f;
		return GamePhysics.LineOfSight(eyePos, p, mask, null);
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004F24C File Offset: 0x0004D44C
	public virtual bool IsSeatClipping(BaseMountable mountable)
	{
		if (!this.doClippingAndVisChecks)
		{
			return false;
		}
		if (mountable == null)
		{
			return false;
		}
		int clipCheckMask = this.GetClipCheckMask();
		Vector3 position = mountable.eyePositionOverride.transform.position;
		Vector3 position2 = mountable.transform.position;
		Vector3 a = position - position2;
		float num = 0.4f;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 vector = position - a * (num - 0.2f);
		bool result = false;
		if (this.checkVehicleClipping)
		{
			List<Collider> list = Facepunch.Pool.GetList<Collider>();
			if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
			{
				GamePhysics.OverlapSphere(vector, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			else
			{
				Vector3 point = position2 + a * (num + 0.05f);
				GamePhysics.OverlapCapsule(vector, point, num, list, clipCheckMask, QueryTriggerInteraction.Ignore);
			}
			foreach (Collider collider in list)
			{
				global::BaseEntity baseEntity = collider.ToBaseEntity();
				if (baseEntity != this && !base.EqualNetID(baseEntity))
				{
					result = true;
					break;
				}
			}
			Facepunch.Pool.FreeList<Collider>(ref list);
		}
		else if (this.clippingChecks == global::BaseVehicle.ClippingCheckMode.AlwaysHeadOnly)
		{
			result = GamePhysics.CheckSphere(vector, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		else
		{
			Vector3 end = position2 + a * (num + 0.05f);
			result = GamePhysics.CheckCapsule(vector, end, num, clipCheckMask, QueryTriggerInteraction.Ignore);
		}
		return result;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004F3BC File Offset: 0x0004D5BC
	public virtual void CheckSeatsForClipping()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			BaseMountable mountable = mountPointInfo.mountable;
			if (!(mountable == null) && mountable.AnyMounted() && this.IsSeatClipping(mountable))
			{
				this.SeatClippedWorld(mountable);
			}
		}
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004F430 File Offset: 0x0004D630
	public virtual void SeatClippedWorld(BaseMountable mountable)
	{
		mountable.DismountPlayer(mountable.GetMounted(), false);
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void MounteeTookDamage(global::BasePlayer mountee, HitInfo info)
	{
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0004F440 File Offset: 0x0004D640
	public override void DismountAllPlayers()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				mountPointInfo.mountable.DismountAllPlayers();
			}
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0004F4A8 File Offset: 0x0004D6A8
	public override void ServerInit()
	{
		base.ServerInit();
		this.clearRecentDriverAction = new Action(this.ClearRecentDriver);
		this.prevSleeping = false;
		if (this.rigidBody != null)
		{
			this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0004F4E8 File Offset: 0x0004D6E8
	public virtual void SpawnSubEntities()
	{
		this.CheckAndSpawnMountPoints();
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0004F4F0 File Offset: 0x0004D6F0
	public virtual bool AdminFixUp(int tier)
	{
		if (this.IsDead())
		{
			return false;
		}
		EntityFuelSystem fuelSystem = this.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AdminAddFuel();
		}
		base.SetHealth(this.MaxHealth());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0004F52B File Offset: 0x0004D72B
	private void OnPhysicsNeighbourChanged()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0004F548 File Offset: 0x0004D748
	private void CheckAndSpawnMountPoints()
	{
		ProtoBuf.BaseVehicle baseVehicle = this.pendingLoad;
		if (((baseVehicle != null) ? baseVehicle.mountPoints : null) != null)
		{
			foreach (ProtoBuf.BaseVehicle.MountPoint mountPoint in this.pendingLoad.mountPoints)
			{
				EntityRef<BaseMountable> entityRef = new EntityRef<BaseMountable>(mountPoint.mountableId);
				if (!entityRef.IsValid(true))
				{
					Debug.LogError(string.Format("Loaded a mountpoint which doesn't exist: {0}", mountPoint.index), this);
				}
				else if (mountPoint.index < 0 || mountPoint.index >= this.mountPoints.Count)
				{
					Debug.LogError(string.Format("Loaded a mountpoint which has no info: {0}", mountPoint.index), this);
					entityRef.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = this.mountPoints[mountPoint.index];
					if (mountPointInfo.mountable != null)
					{
						Debug.LogError(string.Format("Loading a mountpoint after one was already set: {0}", mountPoint.index), this);
						mountPointInfo.mountable.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					mountPointInfo.mountable = entityRef.Get(true);
					if (!mountPointInfo.mountable.enableSaving)
					{
						mountPointInfo.mountable.EnableSaving(true);
					}
				}
			}
		}
		ProtoBuf.BaseVehicle baseVehicle2 = this.pendingLoad;
		if (baseVehicle2 != null)
		{
			baseVehicle2.Dispose();
		}
		this.pendingLoad = null;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			this.SpawnMountPoint(this.mountPoints[i], this.model);
		}
		this.UpdateMountFlags();
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0004F700 File Offset: 0x0004D900
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer && !Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0004F720 File Offset: 0x0004D920
	public override void Hurt(HitInfo info)
	{
		if (!this.IsDead() && this.rigidBody != null && !this.rigidBody.isKinematic)
		{
			float num = info.damageTypes.Get(DamageType.Explosion) + info.damageTypes.Get(DamageType.AntiVehicle);
			if (num > 3f)
			{
				float explosionForce = Mathf.Min(num * this.explosionForceMultiplier, this.explosionForceMax);
				this.rigidBody.AddExplosionForce(explosionForce, info.HitPositionWorld, 1f, 2.5f);
			}
		}
		base.Hurt(info);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0004F7AC File Offset: 0x0004D9AC
	public int NumMounted()
	{
		if (this.HasMountPoints())
		{
			int num = 0;
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
			{
				if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null)
				{
					num++;
				}
			}
			return num;
		}
		if (!this.AnyMounted())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0004F838 File Offset: 0x0004DA38
	public virtual int MaxMounted()
	{
		if (!this.HasMountPoints())
		{
			return 1;
		}
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable != null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0004F8A4 File Offset: 0x0004DAA4
	public bool HasDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver && mountPointInfo.mountable.AnyMounted())
					{
						return true;
					}
				}
				return false;
			}
		}
		return base.AnyMounted();
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0004F930 File Offset: 0x0004DB30
	public bool IsDriver(global::BasePlayer player)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null && mounted == player)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted == player;
		}
		return false;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0004F9E4 File Offset: 0x0004DBE4
	public global::BasePlayer GetDriver()
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							return mounted;
						}
					}
				}
				goto IL_82;
			}
		}
		if (this._mounted != null)
		{
			return this._mounted;
		}
		IL_82:
		return null;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004FA88 File Offset: 0x0004DC88
	public void GetDrivers(List<global::BasePlayer> drivers)
	{
		if (this.HasMountPoints())
		{
			using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
					if (mountPointInfo != null && mountPointInfo.mountable != null && mountPointInfo.isDriver)
					{
						global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
						if (mounted != null)
						{
							drivers.Add(mounted);
						}
					}
				}
				return;
			}
		}
		if (this._mounted != null)
		{
			drivers.Add(this._mounted);
		}
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0004FB30 File Offset: 0x0004DD30
	public global::BasePlayer GetPlayerDamageInitiator()
	{
		if (this.HasDriver())
		{
			return this.GetDriver();
		}
		if (this.recentDrivers.Count <= 0)
		{
			return null;
		}
		return this.recentDrivers.Peek();
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0004FB5C File Offset: 0x0004DD5C
	public int GetPlayerSeat(global::BasePlayer player)
	{
		if (!this.HasMountPoints() && base.GetMounted() == player)
		{
			return 0;
		}
		int num = 0;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0004FBF4 File Offset: 0x0004DDF4
	public global::BaseVehicle.MountPointInfo GetPlayerSeatInfo(global::BasePlayer player)
	{
		if (!this.HasMountPoints())
		{
			return null;
		}
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() == player)
			{
				return mountPointInfo;
			}
		}
		return null;
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060007FC RID: 2044 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanSwapSeats
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsPlayerSeatSwapValid(global::BasePlayer player, int fromIndex, int toIndex)
	{
		return true;
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0004FC78 File Offset: 0x0004DE78
	public void SwapSeats(global::BasePlayer player, int targetSeat = 0)
	{
		if (!this.HasMountPoints() || !this.CanSwapSeats)
		{
			return;
		}
		int playerSeat = this.GetPlayerSeat(player);
		if (playerSeat == -1)
		{
			return;
		}
		BaseMountable mountable = this.GetMountPoint(playerSeat).mountable;
		int num = playerSeat;
		BaseMountable baseMountable = null;
		if (baseMountable == null)
		{
			int num2 = this.NumSwappableSeats();
			for (int i = 0; i < num2; i++)
			{
				num++;
				if (num >= num2)
				{
					num = 0;
				}
				global::BaseVehicle.MountPointInfo mountPoint = this.GetMountPoint(num);
				if (((mountPoint != null) ? mountPoint.mountable : null) != null && !mountPoint.mountable.AnyMounted() && mountPoint.mountable.CanSwapToThis(player) && !this.IsSeatClipping(mountPoint.mountable) && this.IsSeatVisible(mountPoint.mountable, player.eyes.position, 1218511105) && this.IsPlayerSeatSwapValid(player, playerSeat, num))
				{
					baseMountable = mountPoint.mountable;
					break;
				}
			}
		}
		if (baseMountable != null && baseMountable != mountable)
		{
			mountable.DismountPlayer(player, true);
			baseMountable.MountPlayer(player);
			player.MarkSwapSeat();
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0004FD90 File Offset: 0x0004DF90
	public virtual int NumSwappableSeats()
	{
		return this.MaxMounted();
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0004FD98 File Offset: 0x0004DF98
	public bool HasDriverMountPoints()
	{
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isDriver)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002A4FE File Offset: 0x000286FE
	public bool OnlyOwnerAccessible()
	{
		return base.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0004FDF4 File Offset: 0x0004DFF4
	public bool IsDespawnEligable()
	{
		return this.spawnTime == -1f || this.spawnTime + 300f < UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0004FE18 File Offset: 0x0004E018
	public void SetupOwner(global::BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		if (owner != null)
		{
			this.creatorEntity = owner;
			base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
			this.safeAreaRadius = newSafeAreaRadius;
			this.safeAreaOrigin = newSafeAreaOrigin;
			this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0004FE4E File Offset: 0x0004E04E
	public void ClearOwnerEntry()
	{
		this.creatorEntity = null;
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		this.safeAreaRadius = -1f;
		this.safeAreaOrigin = Vector3.zero;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public virtual EntityFuelSystem GetFuelSystem()
	{
		return null;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004FE78 File Offset: 0x0004E078
	public bool IsSafe()
	{
		return this.OnlyOwnerAccessible() && Vector3.Distance(this.safeAreaOrigin, base.transform.position) <= this.safeAreaRadius;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0004FEA5 File Offset: 0x0004E0A5
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		if (this.IsSafe())
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0004FEC8 File Offset: 0x0004E0C8
	public BaseMountable GetIdealMountPoint(Vector3 eyePos, Vector3 pos, global::BasePlayer playerFor = null)
	{
		if (playerFor == null)
		{
			return null;
		}
		if (!this.HasMountPoints())
		{
			return this;
		}
		global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
		bool flag = basePlayer != null;
		bool flag2 = flag && basePlayer.Team != null;
		bool flag3 = flag && playerFor == basePlayer;
		if (!flag3 && flag && this.OnlyOwnerAccessible() && playerFor != null && (playerFor.Team == null || !playerFor.Team.members.Contains(basePlayer.userID)))
		{
			return null;
		}
		BaseMountable result = null;
		float num = float.PositiveInfinity;
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.allMountPoints)
		{
			if (!mountPointInfo.mountable.AnyMounted())
			{
				float num2 = Vector3.Distance(mountPointInfo.mountable.mountAnchor.position, pos);
				if (num2 <= num)
				{
					if (this.IsSeatClipping(mountPointInfo.mountable))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's clipping", mountPointInfo.mountable));
						}
					}
					else if (!this.IsSeatVisible(mountPointInfo.mountable, eyePos, 1218511105))
					{
						if (UnityEngine.Application.isEditor)
						{
							Debug.Log(string.Format("Skipping seat {0} - it's not visible", mountPointInfo.mountable));
						}
					}
					else if (!this.OnlyOwnerAccessible() || !flag3 || flag2 || mountPointInfo.isDriver)
					{
						result = mountPointInfo.mountable;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0005006C File Offset: 0x0004E26C
	public virtual bool MountEligable(global::BasePlayer player)
	{
		if (this.creatorEntity != null && this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			global::BasePlayer basePlayer = this.creatorEntity as global::BasePlayer;
			if (basePlayer != null && basePlayer.Team != null && !basePlayer.Team.members.Contains(player.userID))
			{
				return false;
			}
		}
		global::BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle != null) || baseVehicle.MountEligable(player);
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x000500F0 File Offset: 0x0004E2F0
	public int GetIndexFromSeat(BaseMountable seat)
	{
		int num = 0;
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mountable == seat)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00050158 File Offset: 0x0004E358
	public virtual void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		this.recentDrivers.Enqueue(player);
		if (!base.IsInvoking(this.clearRecentDriverAction))
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00050188 File Offset: 0x0004E388
	public void TryShowCollisionFX(Collision collision, GameObjectRef effectGO)
	{
		this.TryShowCollisionFX(collision.GetContact(0).point, effectGO);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x000501AC File Offset: 0x0004E3AC
	public void TryShowCollisionFX(Vector3 contactPoint, GameObjectRef effectGO)
	{
		if (UnityEngine.Time.time < this.nextCollisionFXTime)
		{
			return;
		}
		this.nextCollisionFXTime = UnityEngine.Time.time + 0.25f;
		if (effectGO.isValid)
		{
			contactPoint += (base.transform.position - contactPoint) * 0.25f;
			Effect.server.Run(effectGO.resourcePath, contactPoint, base.transform.up, null, false);
		}
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0005021C File Offset: 0x0004E41C
	public void SetToKinematic()
	{
		if (this.rigidBody == null || this.rigidBody.isKinematic)
		{
			return;
		}
		this.savedCollisionDetectionMode = this.rigidBody.collisionDetectionMode;
		this.rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0005026E File Offset: 0x0004E46E
	public void SetToNonKinematic()
	{
		if (this.rigidBody == null || !this.rigidBody.isKinematic)
		{
			return;
		}
		this.rigidBody.isKinematic = false;
		this.rigidBody.collisionDetectionMode = this.savedCollisionDetectionMode;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x000502AC File Offset: 0x0004E4AC
	public override void UpdateMountFlags()
	{
		int num = this.NumMounted();
		base.SetFlag(global::BaseEntity.Flags.InUse, num > 0, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved11, num == this.MaxMounted(), false, true);
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x000502FD File Offset: 0x0004E4FD
	private void ClearRecentDriver()
	{
		if (this.recentDrivers.Count > 0)
		{
			this.recentDrivers.Dequeue();
		}
		if (this.recentDrivers.Count > 0)
		{
			base.Invoke(this.clearRecentDriverAction, 3f);
		}
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00050338 File Offset: 0x0004E538
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable idealMountPointFor = this.GetIdealMountPointFor(player);
		if (idealMountPointFor == null)
		{
			return;
		}
		if (idealMountPointFor == this)
		{
			base.AttemptMount(player, doMountChecks);
		}
		else
		{
			idealMountPointFor.AttemptMount(player, doMountChecks);
		}
		if (player.GetMountedVehicle() == this)
		{
			this.PlayerMounted(player, idealMountPointFor);
		}
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x000503A1 File Offset: 0x0004E5A1
	protected BaseMountable GetIdealMountPointFor(global::BasePlayer player)
	{
		return this.GetIdealMountPoint(player.eyes.position, player.eyes.position + player.eyes.HeadForward() * 1f, player);
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000503DC File Offset: 0x0004E5DC
	public override bool GetDismountPosition(global::BasePlayer player, out Vector3 res)
	{
		global::BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		List<Vector3> list = Facepunch.Pool.GetList<Vector3>();
		foreach (Transform transform in this.dismountPositions)
		{
			if (this.ValidDismountPosition(player, transform.transform.position))
			{
				list.Add(transform.transform.position);
				if (this.dismountStyle == global::BaseVehicle.DismountStyle.Ordered)
				{
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Failed to find dismount position for player :",
				player.displayName,
				" / ",
				player.userID,
				" on obj : ",
				base.gameObject.name
			}));
			Facepunch.Pool.FreeList<Vector3>(ref list);
			res = player.transform.position;
			return false;
		}
		Vector3 pos = player.transform.position;
		list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
		res = list[0];
		Facepunch.Pool.FreeList<Vector3>(ref list);
		return true;
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x00050504 File Offset: 0x0004E704
	private BaseMountable SpawnMountPoint(global::BaseVehicle.MountPointInfo mountToSpawn, Model model)
	{
		if (mountToSpawn.mountable != null)
		{
			return mountToSpawn.mountable;
		}
		Vector3 vector = Quaternion.Euler(mountToSpawn.rot) * Vector3.forward;
		Vector3 pos = mountToSpawn.pos;
		Vector3 up = Vector3.up;
		if (mountToSpawn.bone != "")
		{
			pos = model.FindBone(mountToSpawn.bone).transform.position + base.transform.TransformDirection(mountToSpawn.pos);
			vector = base.transform.TransformDirection(vector);
			up = base.transform.up;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(mountToSpawn.prefab.resourcePath, pos, Quaternion.LookRotation(vector, up), true);
		BaseMountable baseMountable = baseEntity as BaseMountable;
		if (baseMountable != null)
		{
			if (this.enableSaving != baseMountable.enableSaving)
			{
				baseMountable.EnableSaving(this.enableSaving);
			}
			if (mountToSpawn.bone != "")
			{
				baseMountable.SetParent(this, mountToSpawn.bone, true, true);
			}
			else
			{
				baseMountable.SetParent(this, false, false);
			}
			baseMountable.Spawn();
			mountToSpawn.mountable = baseMountable;
		}
		else
		{
			Debug.LogError("MountPointInfo prefab is not a BaseMountable. Cannot spawn mount point.");
			if (baseEntity != null)
			{
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		return baseMountable;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0005064C File Offset: 0x0004E84C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	public void RPC_WantsPush(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.isMounted)
		{
			return;
		}
		if (this.RecentlyPushed)
		{
			return;
		}
		if (!this.CanPushNow(player))
		{
			return;
		}
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.OnlyOwnerAccessible() && player != this.creatorEntity)
		{
			return;
		}
		player.metabolism.calories.Subtract(3f);
		player.metabolism.SendChangesToClient();
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
		this.DoPushAction(player);
		this.timeSinceLastPush = 0f;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x000506F4 File Offset: 0x0004E8F4
	protected virtual void DoPushAction(global::BasePlayer player)
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.IsFlipped())
		{
			float d = this.rigidBody.mass * 8f;
			Vector3 vector = Vector3.forward * d;
			if (Vector3.Dot(base.transform.InverseTransformVector(base.transform.position - player.transform.position), Vector3.right) > 0f)
			{
				vector *= -1f;
			}
			if (base.transform.up.y < 0f)
			{
				vector *= -1f;
			}
			this.rigidBody.AddRelativeTorque(vector, ForceMode.Impulse);
			return;
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.position - player.eyes.position, base.transform.up).normalized;
		float d2 = this.rigidBody.mass * 4f;
		this.rigidBody.AddForce(normalized * d2, ForceMode.Impulse);
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnServerWake()
	{
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnServerSleep()
	{
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0004B36F File Offset: 0x0004956F
	public bool IsStationary()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00050807 File Offset: 0x0004EA07
	public bool IsMoving()
	{
		return !base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600081E RID: 2078 RVA: 0x00050817 File Offset: 0x0004EA17
	public bool IsMovingOrOn
	{
		get
		{
			return this.IsMoving() || base.IsOn();
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x0600081F RID: 2079 RVA: 0x00050829 File Offset: 0x0004EA29
	public override float RealisticMass
	{
		get
		{
			if (this.rigidBody != null)
			{
				return this.rigidBody.mass;
			}
			return base.RealisticMass;
		}
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0005084B File Offset: 0x0004EA4B
	public override bool AnyMounted()
	{
		return base.HasFlag(global::BaseEntity.Flags.InUse);
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x00050858 File Offset: 0x0004EA58
	public override bool PlayerIsMounted(global::BasePlayer player)
	{
		return player.IsValid() && player.GetMountedVehicle() == this;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00050870 File Offset: 0x0004EA70
	protected virtual bool CanPushNow(global::BasePlayer pusher)
	{
		return !base.IsOn();
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0005087C File Offset: 0x0004EA7C
	public bool HasMountPoints()
	{
		if (this.mountPoints.Count > 0)
		{
			return true;
		}
		using (global::BaseVehicle.Enumerator enumerator = this.allMountPoints.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				global::BaseVehicle.MountPointInfo mountPointInfo = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x000508E4 File Offset: 0x0004EAE4
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return this.IsAlive() && !base.IsDestroyed && player != null;
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x000508FF File Offset: 0x0004EAFF
	public bool IsFlipped()
	{
		return Vector3.Dot(Vector3.up, base.transform.up) <= 0f;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsVehicleRoot()
	{
		return true;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00050920 File Offset: 0x0004EB20
	public override bool DirectlyMountable()
	{
		return this.IsVehicleRoot();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public override global::BaseVehicle VehicleParent()
	{
		return null;
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00050928 File Offset: 0x0004EB28
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = (child as global::BaseVehicle)) != null && !baseVehicle.IsVehicleRoot() && !this.childVehicles.Contains(baseVehicle))
		{
			this.childVehicles.Add(baseVehicle);
		}
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0005097C File Offset: 0x0004EB7C
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		global::BaseVehicle baseVehicle;
		if ((baseVehicle = (child as global::BaseVehicle)) != null && !baseVehicle.IsVehicleRoot())
		{
			this.childVehicles.Remove(baseVehicle);
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x0600082B RID: 2091 RVA: 0x000509AF File Offset: 0x0004EBAF
	public global::BaseVehicle.Enumerable allMountPoints
	{
		get
		{
			return new global::BaseVehicle.Enumerable(this);
		}
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x000509B8 File Offset: 0x0004EBB8
	public global::BaseVehicle.MountPointInfo GetMountPoint(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index < this.mountPoints.Count)
		{
			return this.mountPoints[index];
		}
		index -= this.mountPoints.Count;
		int num = 0;
		foreach (global::BaseVehicle baseVehicle in this.childVehicles)
		{
			if (!(baseVehicle == null))
			{
				foreach (global::BaseVehicle.MountPointInfo result in baseVehicle.allMountPoints)
				{
					if (num == index)
					{
						return result;
					}
					num++;
				}
			}
		}
		return null;
	}

	// Token: 0x02000BB8 RID: 3000
	public enum ClippingCheckMode
	{
		// Token: 0x040040AD RID: 16557
		OnMountOnly,
		// Token: 0x040040AE RID: 16558
		Always,
		// Token: 0x040040AF RID: 16559
		AlwaysHeadOnly
	}

	// Token: 0x02000BB9 RID: 3001
	public enum DismountStyle
	{
		// Token: 0x040040B1 RID: 16561
		Closest,
		// Token: 0x040040B2 RID: 16562
		Ordered
	}

	// Token: 0x02000BBA RID: 3002
	[Serializable]
	public class MountPointInfo
	{
		// Token: 0x040040B3 RID: 16563
		public bool isDriver;

		// Token: 0x040040B4 RID: 16564
		public Vector3 pos;

		// Token: 0x040040B5 RID: 16565
		public Vector3 rot;

		// Token: 0x040040B6 RID: 16566
		public string bone = "";

		// Token: 0x040040B7 RID: 16567
		public GameObjectRef prefab;

		// Token: 0x040040B8 RID: 16568
		[HideInInspector]
		public BaseMountable mountable;
	}

	// Token: 0x02000BBB RID: 3003
	public readonly struct Enumerable : IEnumerable<global::BaseVehicle.MountPointInfo>, IEnumerable
	{
		// Token: 0x040040B9 RID: 16569
		private readonly global::BaseVehicle _vehicle;

		// Token: 0x06004D67 RID: 19815 RVA: 0x001A0B02 File Offset: 0x0019ED02
		public Enumerable(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x001A0B1F File Offset: 0x0019ED1F
		public global::BaseVehicle.Enumerator GetEnumerator()
		{
			return new global::BaseVehicle.Enumerator(this._vehicle);
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x001A0B2C File Offset: 0x0019ED2C
		IEnumerator<global::BaseVehicle.MountPointInfo> IEnumerable<global::BaseVehicle.MountPointInfo>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x001A0B2C File Offset: 0x0019ED2C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	// Token: 0x02000BBC RID: 3004
	public struct Enumerator : IEnumerator<global::BaseVehicle.MountPointInfo>, IEnumerator, IDisposable
	{
		// Token: 0x040040BA RID: 16570
		private readonly global::BaseVehicle _vehicle;

		// Token: 0x040040BB RID: 16571
		private global::BaseVehicle.Enumerator.State _state;

		// Token: 0x040040BC RID: 16572
		private int _index;

		// Token: 0x040040BD RID: 16573
		private int _childIndex;

		// Token: 0x040040BE RID: 16574
		private global::BaseVehicle.Enumerator.Box _enumerator;

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06004D6B RID: 19819 RVA: 0x001A0B39 File Offset: 0x0019ED39
		// (set) Token: 0x06004D6C RID: 19820 RVA: 0x001A0B41 File Offset: 0x0019ED41
		public global::BaseVehicle.MountPointInfo Current { get; private set; }

		// Token: 0x06004D6D RID: 19821 RVA: 0x001A0B4A File Offset: 0x0019ED4A
		public Enumerator(global::BaseVehicle vehicle)
		{
			if (vehicle == null)
			{
				throw new ArgumentNullException("vehicle");
			}
			this._vehicle = vehicle;
			this._state = global::BaseVehicle.Enumerator.State.Direct;
			this._index = -1;
			this._childIndex = -1;
			this._enumerator = null;
			this.Current = null;
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x001A0B8C File Offset: 0x0019ED8C
		public bool MoveNext()
		{
			this.Current = null;
			switch (this._state)
			{
			case global::BaseVehicle.Enumerator.State.Direct:
				this._index++;
				if (this._index < this._vehicle.mountPoints.Count)
				{
					this.Current = this._vehicle.mountPoints[this._index];
					return true;
				}
				this._state = global::BaseVehicle.Enumerator.State.EnterChild;
				break;
			case global::BaseVehicle.Enumerator.State.EnterChild:
				break;
			case global::BaseVehicle.Enumerator.State.EnumerateChild:
				goto IL_11B;
			case global::BaseVehicle.Enumerator.State.Finished:
				return false;
			default:
				throw new NotSupportedException();
			}
			do
			{
				IL_76:
				this._childIndex++;
			}
			while (this._childIndex < this._vehicle.childVehicles.Count && this._vehicle.childVehicles[this._childIndex] == null);
			if (this._childIndex >= this._vehicle.childVehicles.Count)
			{
				this._state = global::BaseVehicle.Enumerator.State.Finished;
				return false;
			}
			this._enumerator = Facepunch.Pool.Get<global::BaseVehicle.Enumerator.Box>();
			this._enumerator.Value = this._vehicle.childVehicles[this._childIndex].allMountPoints.GetEnumerator();
			this._state = global::BaseVehicle.Enumerator.State.EnumerateChild;
			IL_11B:
			if (this._enumerator.Value.MoveNext())
			{
				this.Current = this._enumerator.Value.Current;
				return true;
			}
			this._enumerator.Value.Dispose();
			Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			this._state = global::BaseVehicle.Enumerator.State.EnterChild;
			goto IL_76;
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06004D6F RID: 19823 RVA: 0x001A0D0C File Offset: 0x0019EF0C
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x001A0D14 File Offset: 0x0019EF14
		public void Dispose()
		{
			if (this._enumerator != null)
			{
				this._enumerator.Value.Dispose();
				Facepunch.Pool.Free<global::BaseVehicle.Enumerator.Box>(ref this._enumerator);
			}
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x001630EF File Offset: 0x001612EF
		public void Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x02000FC5 RID: 4037
		private enum State
		{
			// Token: 0x040050C0 RID: 20672
			Direct,
			// Token: 0x040050C1 RID: 20673
			EnterChild,
			// Token: 0x040050C2 RID: 20674
			EnumerateChild,
			// Token: 0x040050C3 RID: 20675
			Finished
		}

		// Token: 0x02000FC6 RID: 4038
		private class Box : Facepunch.Pool.IPooled
		{
			// Token: 0x040050C4 RID: 20676
			public global::BaseVehicle.Enumerator Value;

			// Token: 0x0600559D RID: 21917 RVA: 0x001BA8A0 File Offset: 0x001B8AA0
			public void EnterPool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}

			// Token: 0x0600559E RID: 21918 RVA: 0x001BA8A0 File Offset: 0x001B8AA0
			public void LeavePool()
			{
				this.Value = default(global::BaseVehicle.Enumerator);
			}
		}
	}
}
