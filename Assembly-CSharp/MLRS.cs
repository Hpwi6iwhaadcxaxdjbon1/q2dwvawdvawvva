using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009E RID: 158
public class MLRS : BaseMountable
{
	// Token: 0x04000948 RID: 2376
	public const string MLRS_PLAYER_KILL_STAT = "mlrs_kills";

	// Token: 0x04000949 RID: 2377
	private float leftRightInput;

	// Token: 0x0400094A RID: 2378
	private float upDownInput;

	// Token: 0x0400094B RID: 2379
	private Vector3 lastSentTargetHitPos;

	// Token: 0x0400094C RID: 2380
	private Vector3 lastSentTrueHitPos;

	// Token: 0x0400094D RID: 2381
	private int nextRocketIndex;

	// Token: 0x0400094E RID: 2382
	private EntityRef rocketOwnerRef;

	// Token: 0x0400094F RID: 2383
	private TimeSince timeSinceBroken;

	// Token: 0x04000950 RID: 2384
	private int radiusModIndex;

	// Token: 0x04000951 RID: 2385
	private float[] radiusMods = new float[]
	{
		0.1f,
		0.2f,
		0.33333334f,
		0.6666667f
	};

	// Token: 0x04000952 RID: 2386
	private Vector3 trueTargetHitPos;

	// Token: 0x04000953 RID: 2387
	[Header("MLRS Components")]
	[SerializeField]
	private GameObjectRef rocketStoragePrefab;

	// Token: 0x04000954 RID: 2388
	[SerializeField]
	private GameObjectRef dashboardStoragePrefab;

	// Token: 0x04000955 RID: 2389
	[Header("MLRS Rotation")]
	[SerializeField]
	private Transform hRotator;

	// Token: 0x04000956 RID: 2390
	[SerializeField]
	private float hRotSpeed = 25f;

	// Token: 0x04000957 RID: 2391
	[SerializeField]
	private Transform vRotator;

	// Token: 0x04000958 RID: 2392
	[SerializeField]
	private float vRotSpeed = 10f;

	// Token: 0x04000959 RID: 2393
	[SerializeField]
	[Range(50f, 90f)]
	private float vRotMax = 85f;

	// Token: 0x0400095A RID: 2394
	[SerializeField]
	private Transform hydraulics;

	// Token: 0x0400095B RID: 2395
	[Header("MLRS Weaponry")]
	[Tooltip("Minimum distance from the MLRS to a targeted hit point. In metres.")]
	[SerializeField]
	public float minRange = 200f;

	// Token: 0x0400095C RID: 2396
	[Tooltip("The size of the area that the rockets may hit, minus rocket damage radius.")]
	[SerializeField]
	public float targetAreaRadius = 30f;

	// Token: 0x0400095D RID: 2397
	[SerializeField]
	private GameObjectRef mlrsRocket;

	// Token: 0x0400095E RID: 2398
	[SerializeField]
	public Transform firingPoint;

	// Token: 0x0400095F RID: 2399
	[SerializeField]
	private global::MLRS.RocketTube[] rocketTubes;

	// Token: 0x04000960 RID: 2400
	[Header("MLRS Dashboard/FX")]
	[SerializeField]
	private GameObject screensChild;

	// Token: 0x04000961 RID: 2401
	[SerializeField]
	private Transform leftHandGrip;

	// Token: 0x04000962 RID: 2402
	[SerializeField]
	private Transform leftJoystick;

	// Token: 0x04000963 RID: 2403
	[SerializeField]
	private Transform rightHandGrip;

	// Token: 0x04000964 RID: 2404
	[SerializeField]
	private Transform rightJoystick;

	// Token: 0x04000965 RID: 2405
	[SerializeField]
	private Transform controlKnobHeight;

	// Token: 0x04000966 RID: 2406
	[SerializeField]
	private Transform controlKnobAngle;

	// Token: 0x04000967 RID: 2407
	[SerializeField]
	private GameObjectRef uiDialogPrefab;

	// Token: 0x04000968 RID: 2408
	[SerializeField]
	private Light fireButtonLight;

	// Token: 0x04000969 RID: 2409
	[SerializeField]
	private GameObject brokenDownEffect;

	// Token: 0x0400096A RID: 2410
	[SerializeField]
	private ParticleSystem topScreenShutdown;

	// Token: 0x0400096B RID: 2411
	[SerializeField]
	private ParticleSystem bottomScreenShutdown;

	// Token: 0x0400096C RID: 2412
	[ServerVar(Help = "How many minutes before the MLRS recovers from use and can be used again")]
	public static float brokenDownMinutes = 10f;

	// Token: 0x04000972 RID: 2418
	public const global::BaseEntity.Flags FLAG_FIRING_ROCKETS = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000973 RID: 2419
	public const global::BaseEntity.Flags FLAG_HAS_AIMING_MODULE = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000974 RID: 2420
	private EntityRef rocketStorageInstance;

	// Token: 0x04000975 RID: 2421
	private EntityRef dashboardStorageInstance;

	// Token: 0x04000976 RID: 2422
	private float rocketBaseGravity;

	// Token: 0x04000977 RID: 2423
	private float rocketSpeed;

	// Token: 0x04000979 RID: 2425
	private bool isInitialLoad = true;

	// Token: 0x06000E37 RID: 3639 RVA: 0x0007841C File Offset: 0x0007661C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MLRS.OnRpcMessage", 0))
		{
			if (rpc == 455279877U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Fire_Rockets ");
				}
				using (TimeWarning.New("RPC_Fire_Rockets", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(455279877U, "RPC_Fire_Rockets", this, player, 3f))
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
							this.RPC_Fire_Rockets(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Fire_Rockets");
					}
				}
				return true;
			}
			if (rpc == 751446792U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open_Dashboard ");
				}
				using (TimeWarning.New("RPC_Open_Dashboard", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(751446792U, "RPC_Open_Dashboard", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Open_Dashboard(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Open_Dashboard");
					}
				}
				return true;
			}
			if (rpc == 1311007340U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open_Rockets ");
				}
				using (TimeWarning.New("RPC_Open_Rockets", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1311007340U, "RPC_Open_Rockets", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Open_Rockets(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Open_Rockets");
					}
				}
				return true;
			}
			if (rpc == 858951307U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SetTargetHitPos ");
				}
				using (TimeWarning.New("RPC_SetTargetHitPos", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(858951307U, "RPC_SetTargetHitPos", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_SetTargetHitPos(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_SetTargetHitPos");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x000789D4 File Offset: 0x00076BD4
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == this.rocketStoragePrefab.GetEntity().prefabID)
			{
				this.rocketStorageInstance.Set(child);
			}
			if (child.prefabID == this.dashboardStoragePrefab.GetEntity().prefabID)
			{
				this.dashboardStorageInstance.Set(child);
			}
		}
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00078A38 File Offset: 0x00076C38
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.IsBroken())
		{
			if (this.timeSinceBroken < global::MLRS.brokenDownMinutes * 60f)
			{
				global::Item item;
				base.SetFlag(global::BaseEntity.Flags.Reserved8, this.TryGetAimingModule(out item), false, true);
				return;
			}
			this.SetRepaired();
		}
		int rocketAmmoCount = this.RocketAmmoCount;
		this.UpdateStorageState();
		if (this.CanBeUsed && this.AnyMounted())
		{
			Vector3 vector = this.UserTargetHitPos;
			vector += Vector3.forward * this.upDownInput * 75f * UnityEngine.Time.fixedDeltaTime;
			vector += Vector3.right * this.leftRightInput * 75f * UnityEngine.Time.fixedDeltaTime;
			this.SetUserTargetHitPos(vector);
		}
		if (!this.IsFiringRockets)
		{
			float num;
			float num2;
			float num3;
			this.HitPosToRotation(this.trueTargetHitPos, out num, out num2, out num3);
			float num4 = num3 / -UnityEngine.Physics.gravity.y;
			this.IsRealigning = (Mathf.Abs(Mathf.DeltaAngle(this.VRotation, num2)) > 0.001f || Mathf.Abs(Mathf.DeltaAngle(this.HRotation, num)) > 0.001f || !Mathf.Approximately(this.CurGravityMultiplier, num4));
			if (this.IsRealigning)
			{
				if (this.isInitialLoad)
				{
					this.VRotation = num2;
					this.HRotation = num;
					this.isInitialLoad = false;
				}
				else
				{
					this.VRotation = Mathf.MoveTowardsAngle(this.VRotation, num2, UnityEngine.Time.deltaTime * this.vRotSpeed);
					this.HRotation = Mathf.MoveTowardsAngle(this.HRotation, num, UnityEngine.Time.deltaTime * this.hRotSpeed);
				}
				this.CurGravityMultiplier = num4;
				this.TrueHitPos = this.GetTrueHitPos();
			}
		}
		if (this.UserTargetHitPos != this.lastSentTargetHitPos || this.TrueHitPos != this.lastSentTrueHitPos || this.RocketAmmoCount != rocketAmmoCount)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00078C30 File Offset: 0x00076E30
	private Vector3 GetTrueHitPos()
	{
		global::MLRS.TheoreticalProjectile theoreticalProjectile = new global::MLRS.TheoreticalProjectile(this.firingPoint.position, this.firingPoint.forward.normalized * this.rocketSpeed, this.CurGravityMultiplier);
		int num = 0;
		float dt = (theoreticalProjectile.forward.y > 0f) ? 2f : 0.66f;
		while (!this.NextRayHitSomething(ref theoreticalProjectile, dt) && (float)num < 128f)
		{
			num++;
		}
		return theoreticalProjectile.pos;
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00078CB4 File Offset: 0x00076EB4
	private bool NextRayHitSomething(ref global::MLRS.TheoreticalProjectile projectile, float dt)
	{
		float num = UnityEngine.Physics.gravity.y * projectile.gravityMult;
		Vector3 pos = projectile.pos;
		float d = projectile.forward.MagnitudeXZ() * dt;
		float y = projectile.forward.y * dt + num * dt * dt * 0.5f;
		Vector2 vector = projectile.forward.XZ2D().normalized * d;
		Vector3 b = new Vector3(vector.x, y, vector.y);
		projectile.pos += b;
		float y2 = projectile.forward.y + num * dt;
		projectile.forward.y = y2;
		RaycastHit hit;
		if (UnityEngine.Physics.Linecast(pos, projectile.pos, out hit, 1084293393, QueryTriggerInteraction.Ignore))
		{
			projectile.pos = hit.point;
			global::BaseEntity entity = hit.GetEntity();
			bool flag = entity != null && entity.EqualNetID(this);
			if (flag)
			{
				projectile.pos += projectile.forward * 1f;
			}
			return !flag;
		}
		return false;
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00078DD4 File Offset: 0x00076FD4
	private float GetSurfaceHeight(Vector3 pos)
	{
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float height2 = TerrainMeta.WaterMap.GetHeight(pos);
		return Mathf.Max(height, height2);
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x00078DFE File Offset: 0x00076FFE
	private void SetRepaired()
	{
		base.SetFlag(global::BaseEntity.Flags.Broken, false, false, true);
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00078E10 File Offset: 0x00077010
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.upDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.upDownInput = -1f;
		}
		else
		{
			this.upDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.leftRightInput = -1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.leftRightInput = 1f;
			return;
		}
		this.leftRightInput = 0f;
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00078E8C File Offset: 0x0007708C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.mlrs = Facepunch.Pool.Get<ProtoBuf.MLRS>();
		info.msg.mlrs.targetPos = this.UserTargetHitPos;
		info.msg.mlrs.curHitPos = this.TrueHitPos;
		info.msg.mlrs.rocketStorageID = this.rocketStorageInstance.uid;
		info.msg.mlrs.dashboardStorageID = this.dashboardStorageInstance.uid;
		info.msg.mlrs.ammoCount = (uint)this.RocketAmmoCount;
		this.lastSentTargetHitPos = this.UserTargetHitPos;
		this.lastSentTrueHitPos = this.TrueHitPos;
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00078F40 File Offset: 0x00077140
	public bool AdminFixUp()
	{
		if (this.IsDead() || this.IsFiringRockets)
		{
			return false;
		}
		StorageContainer dashboardContainer = this.GetDashboardContainer();
		if (!this.HasAimingModule)
		{
			dashboardContainer.inventory.AddItem(ItemManager.FindItemDefinition("aiming.module.mlrs"), 1, 0UL, global::ItemContainer.LimitStack.Existing);
		}
		StorageContainer rocketContainer = this.GetRocketContainer();
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("ammo.rocket.mlrs");
		if (this.RocketAmmoCount < rocketContainer.inventory.capacity * itemDefinition.stackable)
		{
			int num;
			for (int i = itemDefinition.stackable * rocketContainer.inventory.capacity - this.RocketAmmoCount; i > 0; i -= num)
			{
				num = Mathf.Min(i, itemDefinition.stackable);
				rocketContainer.inventory.AddItem(itemDefinition, itemDefinition.stackable, 0UL, global::ItemContainer.LimitStack.Existing);
			}
		}
		this.SetRepaired();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0007900C File Offset: 0x0007720C
	private void Fire(global::BasePlayer owner)
	{
		this.UpdateStorageState();
		if (!this.CanFire)
		{
			return;
		}
		if (this._mounted == null)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
		this.radiusModIndex = 0;
		this.nextRocketIndex = Mathf.Min(this.RocketAmmoCount - 1, this.rocketTubes.Length - 1);
		this.rocketOwnerRef.Set(owner);
		base.InvokeRepeating(new Action(this.FireNextRocket), 0f, 0.5f);
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00079094 File Offset: 0x00077294
	private void EndFiring()
	{
		base.CancelInvoke(new Action(this.FireNextRocket));
		this.rocketOwnerRef.Set(null);
		global::Item item;
		if (this.TryGetAimingModule(out item))
		{
			item.LoseCondition(1f);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Broken, true, false, false);
		base.SendNetworkUpdate_Flags();
		this.timeSinceBroken = 0f;
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00079108 File Offset: 0x00077308
	private void FireNextRocket()
	{
		this.RocketAmmoCount = this.GetRocketContainer().inventory.GetAmmoAmount(AmmoTypes.MLRS_ROCKET);
		if (this.nextRocketIndex < 0 || this.nextRocketIndex >= this.RocketAmmoCount || base.IsBroken())
		{
			this.EndFiring();
			return;
		}
		StorageContainer rocketContainer = this.GetRocketContainer();
		Vector3 firingPos = this.firingPoint.position + this.firingPoint.rotation * this.rocketTubes[this.nextRocketIndex].firingOffset;
		float d = 1f;
		if (this.radiusModIndex < this.radiusMods.Length)
		{
			d = this.radiusMods[this.radiusModIndex];
		}
		this.radiusModIndex++;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * (this.targetAreaRadius - this.RocketDamageRadius) * d;
		Vector3 targetPos = this.TrueHitPos + new Vector3(vector.x, 0f, vector.y);
		float num;
		Vector3 aimToTarget = this.GetAimToTarget(targetPos, out num);
		ServerProjectile serverProjectile;
		if (base.TryFireProjectile(rocketContainer, AmmoTypes.MLRS_ROCKET, firingPos, aimToTarget, this.rocketOwnerRef.Get(true) as global::BasePlayer, 0f, 0f, out serverProjectile))
		{
			serverProjectile.gravityModifier = num / -UnityEngine.Physics.gravity.y;
			this.nextRocketIndex--;
			return;
		}
		this.EndFiring();
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x00079268 File Offset: 0x00077468
	private void UpdateStorageState()
	{
		global::Item item;
		bool b = this.TryGetAimingModule(out item);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, b, false, true);
		this.RocketAmmoCount = this.GetRocketContainer().inventory.GetAmmoAmount(AmmoTypes.MLRS_ROCKET);
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x000792A8 File Offset: 0x000774A8
	private bool TryGetAimingModule(out global::Item item)
	{
		global::ItemContainer inventory = this.GetDashboardContainer().inventory;
		if (!inventory.IsEmpty())
		{
			item = inventory.itemList[0];
			return true;
		}
		item = null;
		return false;
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x000792E0 File Offset: 0x000774E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SetTargetHitPos(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		this.SetUserTargetHitPos(msg.read.Vector3());
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00079310 File Offset: 0x00077510
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Fire_Rockets(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		this.Fire(player);
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00079338 File Offset: 0x00077538
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Rockets(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity rocketContainer = this.GetRocketContainer();
		if (!rocketContainer.IsUnityNull<IItemContainerEntity>())
		{
			rocketContainer.PlayerOpenLoot(player, "", false);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00079398 File Offset: 0x00077598
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Dashboard(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity dashboardContainer = this.GetDashboardContainer();
		if (!dashboardContainer.IsUnityNull<IItemContainerEntity>())
		{
			dashboardContainer.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000E4A RID: 3658 RVA: 0x000793F8 File Offset: 0x000775F8
	// (set) Token: 0x06000E4B RID: 3659 RVA: 0x00079400 File Offset: 0x00077600
	public Vector3 UserTargetHitPos { get; private set; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000E4C RID: 3660 RVA: 0x00079409 File Offset: 0x00077609
	// (set) Token: 0x06000E4D RID: 3661 RVA: 0x00079411 File Offset: 0x00077611
	public Vector3 TrueHitPos { get; private set; }

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000E4E RID: 3662 RVA: 0x00003278 File Offset: 0x00001478
	public bool HasAimingModule
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0007941A File Offset: 0x0007761A
	private bool CanBeUsed
	{
		get
		{
			return this.HasAimingModule && !base.IsBroken();
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000E50 RID: 3664 RVA: 0x0007942F File Offset: 0x0007762F
	private bool CanFire
	{
		get
		{
			return this.CanBeUsed && this.RocketAmmoCount > 0 && !this.IsFiringRockets && !this.IsRealigning;
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x00079455 File Offset: 0x00077655
	// (set) Token: 0x06000E52 RID: 3666 RVA: 0x00079468 File Offset: 0x00077668
	private float HRotation
	{
		get
		{
			return this.hRotator.eulerAngles.y;
		}
		set
		{
			Vector3 eulerAngles = this.hRotator.eulerAngles;
			eulerAngles.y = value;
			this.hRotator.eulerAngles = eulerAngles;
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000E53 RID: 3667 RVA: 0x00079495 File Offset: 0x00077695
	// (set) Token: 0x06000E54 RID: 3668 RVA: 0x000794A8 File Offset: 0x000776A8
	private float VRotation
	{
		get
		{
			return this.vRotator.localEulerAngles.x;
		}
		set
		{
			Vector3 localEulerAngles = this.vRotator.localEulerAngles;
			if (value < 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, -this.vRotMax, 0f);
			}
			else if (value > 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 360f - this.vRotMax, 360f);
			}
			this.vRotator.localEulerAngles = localEulerAngles;
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000E55 RID: 3669 RVA: 0x00079516 File Offset: 0x00077716
	// (set) Token: 0x06000E56 RID: 3670 RVA: 0x0007951E File Offset: 0x0007771E
	public float CurGravityMultiplier { get; private set; }

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000E57 RID: 3671 RVA: 0x00079527 File Offset: 0x00077727
	// (set) Token: 0x06000E58 RID: 3672 RVA: 0x0007952F File Offset: 0x0007772F
	public int RocketAmmoCount { get; private set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000E59 RID: 3673 RVA: 0x00079538 File Offset: 0x00077738
	// (set) Token: 0x06000E5A RID: 3674 RVA: 0x00079540 File Offset: 0x00077740
	public bool IsRealigning { get; private set; }

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000E5B RID: 3675 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsFiringRockets
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00079549 File Offset: 0x00077749
	// (set) Token: 0x06000E5D RID: 3677 RVA: 0x00079551 File Offset: 0x00077751
	public float RocketDamageRadius { get; private set; }

	// Token: 0x06000E5E RID: 3678 RVA: 0x0007955C File Offset: 0x0007775C
	public override void InitShared()
	{
		base.InitShared();
		GameObject gameObject = this.mlrsRocket.Get();
		ServerProjectile component = gameObject.GetComponent<ServerProjectile>();
		this.rocketBaseGravity = -UnityEngine.Physics.gravity.y * component.gravityModifier;
		this.rocketSpeed = component.speed;
		global::TimedExplosive component2 = gameObject.GetComponent<global::TimedExplosive>();
		this.RocketDamageRadius = component2.explosionRadius;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x000795B8 File Offset: 0x000777B8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mlrs != null)
		{
			this.SetUserTargetHitPos(info.msg.mlrs.targetPos);
			this.TrueHitPos = info.msg.mlrs.curHitPos;
			float hrotation;
			float vrotation;
			float num;
			this.HitPosToRotation(this.TrueHitPos, out hrotation, out vrotation, out num);
			this.CurGravityMultiplier = num / -UnityEngine.Physics.gravity.y;
			if (base.isServer)
			{
				this.HRotation = hrotation;
				this.VRotation = vrotation;
			}
			this.rocketStorageInstance.uid = info.msg.mlrs.rocketStorageID;
			this.dashboardStorageInstance.uid = info.msg.mlrs.dashboardStorageID;
			this.RocketAmmoCount = (int)info.msg.mlrs.ammoCount;
		}
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x0007968F File Offset: 0x0007788F
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return !this.IsFiringRockets;
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x0007969C File Offset: 0x0007789C
	private void SetUserTargetHitPos(Vector3 worldPos)
	{
		if (this.UserTargetHitPos == worldPos)
		{
			return;
		}
		if (base.isServer)
		{
			Vector3 position = TerrainMeta.Position;
			Vector3 vector = position + TerrainMeta.Size;
			worldPos.x = Mathf.Clamp(worldPos.x, position.x, vector.x);
			worldPos.z = Mathf.Clamp(worldPos.z, position.z, vector.z);
			worldPos.y = this.GetSurfaceHeight(worldPos);
		}
		this.UserTargetHitPos = worldPos;
		if (base.isServer)
		{
			this.trueTargetHitPos = this.UserTargetHitPos;
			foreach (TriggerSafeZone triggerSafeZone in TriggerSafeZone.allSafeZones)
			{
				Vector3 center = triggerSafeZone.triggerCollider.bounds.center;
				center.y = 0f;
				float num = triggerSafeZone.triggerCollider.GetRadius(triggerSafeZone.transform.localScale) + this.targetAreaRadius;
				this.trueTargetHitPos.y = 0f;
				if (Vector3.Distance(center, this.trueTargetHitPos) < num)
				{
					Vector3 vector2 = this.trueTargetHitPos - center;
					this.trueTargetHitPos = center + vector2.normalized * num;
					this.trueTargetHitPos.y = this.GetSurfaceHeight(this.trueTargetHitPos);
					break;
				}
			}
		}
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00079824 File Offset: 0x00077A24
	private StorageContainer GetRocketContainer()
	{
		global::BaseEntity baseEntity = this.rocketStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x0007985C File Offset: 0x00077A5C
	private StorageContainer GetDashboardContainer()
	{
		global::BaseEntity baseEntity = this.dashboardStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00079894 File Offset: 0x00077A94
	private void HitPosToRotation(Vector3 hitPos, out float hRot, out float vRot, out float g)
	{
		Vector3 aimToTarget = this.GetAimToTarget(hitPos, out g);
		Vector3 eulerAngles = Quaternion.LookRotation(aimToTarget, Vector3.up).eulerAngles;
		vRot = eulerAngles.x - 360f;
		aimToTarget.y = 0f;
		hRot = eulerAngles.y;
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000798E4 File Offset: 0x00077AE4
	private Vector3 GetAimToTarget(Vector3 targetPos, out float g)
	{
		g = this.rocketBaseGravity;
		float num = this.rocketSpeed;
		Vector3 vector = targetPos - this.firingPoint.position;
		float num2 = vector.Magnitude2D();
		float y = vector.y;
		float num3 = Mathf.Sqrt(num * num * num * num - g * (g * (num2 * num2) + 2f * y * num * num));
		float num4 = Mathf.Atan((num * num + num3) / (g * num2)) * 57.29578f;
		float num5 = Mathf.Clamp(num4, 0f, 90f);
		if (float.IsNaN(num4))
		{
			num5 = 45f;
			g = global::MLRS.ProjectileDistToGravity(num2, y, num5, num);
		}
		else if (num4 > this.vRotMax)
		{
			num5 = this.vRotMax;
			g = global::MLRS.ProjectileDistToGravity(Mathf.Max(num2, this.minRange), y, num5, num);
		}
		vector.Normalize();
		vector.y = 0f;
		Vector3 axis = Vector3.Cross(vector, Vector3.up);
		return Quaternion.AngleAxis(num5, axis) * vector;
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000799E8 File Offset: 0x00077BE8
	private static float ProjectileDistToSpeed(float x, float y, float angle, float g, float fallbackV)
	{
		float num = angle * 0.017453292f;
		float num2 = Mathf.Sqrt(x * x * g / (x * Mathf.Sin(2f * num) - 2f * y * Mathf.Cos(num) * Mathf.Cos(num)));
		if (float.IsNaN(num2) || num2 < 1f)
		{
			num2 = fallbackV;
		}
		return num2;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00079A44 File Offset: 0x00077C44
	private static float ProjectileDistToGravity(float x, float y, float θ, float v)
	{
		float num = θ * 0.017453292f;
		float num2 = (v * v * x * Mathf.Sin(2f * num) - 2f * v * v * y * Mathf.Cos(num) * Mathf.Cos(num)) / (x * x);
		if (float.IsNaN(num2) || num2 < 0.01f)
		{
			num2 = -UnityEngine.Physics.gravity.y;
		}
		return num2;
	}

	// Token: 0x02000BE2 RID: 3042
	[Serializable]
	public class RocketTube
	{
		// Token: 0x04004132 RID: 16690
		public Vector3 firingOffset;

		// Token: 0x04004133 RID: 16691
		public Transform hinge;

		// Token: 0x04004134 RID: 16692
		public Renderer rocket;
	}

	// Token: 0x02000BE3 RID: 3043
	private struct TheoreticalProjectile
	{
		// Token: 0x04004135 RID: 16693
		public Vector3 pos;

		// Token: 0x04004136 RID: 16694
		public Vector3 forward;

		// Token: 0x04004137 RID: 16695
		public float gravityMult;

		// Token: 0x06004DB8 RID: 19896 RVA: 0x001A153A File Offset: 0x0019F73A
		public TheoreticalProjectile(Vector3 pos, Vector3 forward, float gravityMult)
		{
			this.pos = pos;
			this.forward = forward;
			this.gravityMult = gravityMult;
		}
	}
}
