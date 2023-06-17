using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Sonar;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using VLB;

// Token: 0x02000049 RID: 73
public class BaseSubmarine : global::BaseVehicle, IPoolVehicle, IEngineControllerUser, IEntity, IAirSupply
{
	// Token: 0x04000512 RID: 1298
	private float targetClimbSpeed;

	// Token: 0x04000513 RID: 1299
	private float maxDamageThisTick;

	// Token: 0x04000514 RID: 1300
	private float nextCollisionDamageTime;

	// Token: 0x04000515 RID: 1301
	private bool prevPrimaryFireInput;

	// Token: 0x04000516 RID: 1302
	private bool primaryFireInput;

	// Token: 0x04000517 RID: 1303
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000518 RID: 1304
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000519 RID: 1305
	private TimeSince timeSinceTorpedoFired;

	// Token: 0x0400051A RID: 1306
	private TimeSince timeSinceFailRPCSent;

	// Token: 0x0400051B RID: 1307
	private float normalDrag;

	// Token: 0x0400051C RID: 1308
	private float highDrag;

	// Token: 0x0400051D RID: 1309
	private bool wasOnSurface;

	// Token: 0x0400051E RID: 1310
	[Header("Submarine Main")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x0400051F RID: 1311
	[SerializeField]
	private Buoyancy buoyancy;

	// Token: 0x04000520 RID: 1312
	[SerializeField]
	protected float maxRudderAngle = 35f;

	// Token: 0x04000521 RID: 1313
	[SerializeField]
	private Transform rudderVisualTransform;

	// Token: 0x04000522 RID: 1314
	[SerializeField]
	private Transform rudderDetailedColliderTransform;

	// Token: 0x04000523 RID: 1315
	[SerializeField]
	private Transform propellerTransform;

	// Token: 0x04000524 RID: 1316
	[SerializeField]
	private float timeUntilAutoSurface = 300f;

	// Token: 0x04000525 RID: 1317
	[SerializeField]
	private Renderer[] interiorRenderers;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private SonarObject sonarObject;

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private BaseSubmarine.ParentTriggerInfo[] parentTriggers;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04000529 RID: 1321
	[Header("Submarine Engine & Fuel")]
	[SerializeField]
	private float engineKW = 200f;

	// Token: 0x0400052A RID: 1322
	[SerializeField]
	private float turnPower = 0.25f;

	// Token: 0x0400052B RID: 1323
	[SerializeField]
	private float engineStartupTime = 0.5f;

	// Token: 0x0400052C RID: 1324
	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	// Token: 0x0400052D RID: 1325
	[SerializeField]
	private float depthChangeTargetSpeed = 1f;

	// Token: 0x0400052E RID: 1326
	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	// Token: 0x0400052F RID: 1327
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000530 RID: 1328
	[FormerlySerializedAs("internalAccessFuelTank")]
	[SerializeField]
	private bool internalAccessStorage;

	// Token: 0x04000531 RID: 1329
	[Header("Submarine Weaponry")]
	[SerializeField]
	private GameObjectRef torpedoStoragePrefab;

	// Token: 0x04000532 RID: 1330
	[SerializeField]
	private Transform torpedoFiringPoint;

	// Token: 0x04000533 RID: 1331
	[SerializeField]
	private float maxFireRate = 1.5f;

	// Token: 0x04000534 RID: 1332
	[Header("Submarine Audio & FX")]
	[SerializeField]
	protected SubmarineAudio submarineAudio;

	// Token: 0x04000535 RID: 1333
	[SerializeField]
	private ParticleSystem fxTorpedoFire;

	// Token: 0x04000536 RID: 1334
	[SerializeField]
	private GameObject internalFXContainer;

	// Token: 0x04000537 RID: 1335
	[SerializeField]
	private GameObject internalOnFXContainer;

	// Token: 0x04000538 RID: 1336
	[SerializeField]
	private ParticleSystem fxIntAmbientBubbleLoop;

	// Token: 0x04000539 RID: 1337
	[SerializeField]
	private ParticleSystem fxIntInitialDiveBubbles;

	// Token: 0x0400053A RID: 1338
	[SerializeField]
	private ParticleSystem fxIntWaterDropSpray;

	// Token: 0x0400053B RID: 1339
	[SerializeField]
	private ParticleSystem fxIntWindowFilm;

	// Token: 0x0400053C RID: 1340
	[SerializeField]
	private ParticleSystemContainer fxIntMediumDamage;

	// Token: 0x0400053D RID: 1341
	[SerializeField]
	private ParticleSystemContainer fxIntHeavyDamage;

	// Token: 0x0400053E RID: 1342
	[SerializeField]
	private GameObject externalFXContainer;

	// Token: 0x0400053F RID: 1343
	[SerializeField]
	private GameObject externalOnFXContainer;

	// Token: 0x04000540 RID: 1344
	[SerializeField]
	private ParticleSystem fxExtAmbientBubbleLoop;

	// Token: 0x04000541 RID: 1345
	[SerializeField]
	private ParticleSystem fxExtInitialDiveBubbles;

	// Token: 0x04000542 RID: 1346
	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustForward;

	// Token: 0x04000543 RID: 1347
	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustReverse;

	// Token: 0x04000544 RID: 1348
	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustForward;

	// Token: 0x04000545 RID: 1349
	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustForwardSubs;

	// Token: 0x04000546 RID: 1350
	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustReverse;

	// Token: 0x04000547 RID: 1351
	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustReverseSubs;

	// Token: 0x04000548 RID: 1352
	[SerializeField]
	private ParticleSystem fxExtBowWave;

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	private ParticleSystem fxExtWakeEffect;

	// Token: 0x0400054A RID: 1354
	[SerializeField]
	private GameObjectRef aboveWatercollisionEffect;

	// Token: 0x0400054B RID: 1355
	[SerializeField]
	private GameObjectRef underWatercollisionEffect;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	private VolumetricLightBeam spotlightVolumetrics;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private float mountedAlphaInside = 0.04f;

	// Token: 0x0400054E RID: 1358
	[SerializeField]
	private float mountedAlphaOutside = 0.015f;

	// Token: 0x0400054F RID: 1359
	[ServerVar(Help = "How long before a submarine loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	// Token: 0x04000550 RID: 1360
	[ServerVar(Help = "How long before a submarine loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	// Token: 0x04000551 RID: 1361
	[ServerVar(Help = "How long a submarine can stay underwater until players start taking damage from low oxygen")]
	public static float oxygenminutes = 10f;

	// Token: 0x04000553 RID: 1363
	public const global::BaseEntity.Flags Flag_Ammo = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000554 RID: 1364
	private float _throttle;

	// Token: 0x04000555 RID: 1365
	private float _rudder;

	// Token: 0x04000556 RID: 1366
	private float _upDown;

	// Token: 0x04000557 RID: 1367
	private float _oxygen = 1f;

	// Token: 0x04000558 RID: 1368
	protected VehicleEngineController<BaseSubmarine> engineController;

	// Token: 0x04000559 RID: 1369
	protected float cachedFuelAmount;

	// Token: 0x0400055A RID: 1370
	protected Vector3 steerAngle;

	// Token: 0x0400055B RID: 1371
	protected float waterSurfaceY;

	// Token: 0x0400055C RID: 1372
	protected float curSubDepthY;

	// Token: 0x0400055D RID: 1373
	private EntityRef<StorageContainer> torpedoStorageInstance;

	// Token: 0x0400055E RID: 1374
	private EntityRef<StorageContainer> itemStorageInstance;

	// Token: 0x0400055F RID: 1375
	private int waterLayerMask;

	// Token: 0x06000794 RID: 1940 RVA: 0x0004D4CC File Offset: 0x0004B6CC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseSubmarine.OnRpcMessage", 0))
		{
			if (rpc == 1851540757U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenFuel ");
				}
				using (TimeWarning.New("RPC_OpenFuel", 0))
				{
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
							this.RPC_OpenFuel(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				return true;
			}
			if (rpc == 924237371U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenItemStorage ");
				}
				using (TimeWarning.New("RPC_OpenItemStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(924237371U, "RPC_OpenItemStorage", this, player, 3f))
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
							this.RPC_OpenItemStorage(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenItemStorage");
					}
				}
				return true;
			}
			if (rpc == 2181221870U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenTorpedoStorage ");
				}
				using (TimeWarning.New("RPC_OpenTorpedoStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2181221870U, "RPC_OpenTorpedoStorage", this, player, 3f))
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
							this.RPC_OpenTorpedoStorage(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenTorpedoStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0004D8DC File Offset: 0x0004BADC
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.timeSinceLastUsed = this.timeUntilAutoSurface;
		this.buoyancy.buoyancyScale = 1f;
		this.normalDrag = this.rigidBody.drag;
		this.highDrag = this.normalDrag * 2.5f;
		this.Oxygen = 1f;
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.SubmarineDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0004D9A0 File Offset: 0x0004BBA0
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (this.isSpawned)
			{
				this.GetFuelSystem().CheckNewChild(child);
			}
			if (child.prefabID == this.itemStoragePrefab.GetEntity().prefabID)
			{
				this.itemStorageInstance.Set((StorageContainer)child);
			}
			if (child.prefabID == this.torpedoStoragePrefab.GetEntity().prefabID)
			{
				this.torpedoStorageInstance.Set((StorageContainer)child);
			}
		}
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0004DA22 File Offset: 0x0004BC22
	private void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (next.HasFlag(global::BaseEntity.Flags.On) && !old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0004DA58 File Offset: 0x0004BC58
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			StorageContainer storageContainer = this.itemStorageInstance.Get(base.isServer);
			if (storageContainer != null && storageContainer.IsValid())
			{
				storageContainer.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0004DA9C File Offset: 0x0004BC9C
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		this.ProcessCollision(collision);
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0004DAAE File Offset: 0x0004BCAE
	public override float MaxVelocity()
	{
		return 10f;
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0004DAB5 File Offset: 0x0004BCB5
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x0004DAC2 File Offset: 0x0004BCC2
	public override int StartingFuelUnits()
	{
		return 50;
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x0004DAC8 File Offset: 0x0004BCC8
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (!this.CanMount(player))
		{
			return;
		}
		if (!this.MountEligable(player))
		{
			return;
		}
		BaseMountable baseMountable;
		if (!base.HasDriver())
		{
			baseMountable = this.mountPoints[0].mountable;
		}
		else
		{
			baseMountable = base.GetIdealMountPointFor(player);
		}
		if (baseMountable != null)
		{
			baseMountable.AttemptMount(player, doMountChecks);
		}
		if (this.PlayerIsMounted(player))
		{
			this.PlayerMounted(player, baseMountable);
		}
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00029A3C File Offset: 0x00027C3C
	public void OnPoolDestroyed()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x0004DB2F File Offset: 0x0004BD2F
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x0004DB65 File Offset: 0x0004BD65
	protected override void OnServerWake()
	{
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x0004DB80 File Offset: 0x0004BD80
	public override void OnKilled(HitInfo info)
	{
		DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
		if (majorityDamageType == DamageType.Explosion || majorityDamageType == DamageType.AntiVehicle)
		{
			foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable != null)
				{
					global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted != null)
					{
						mounted.Hurt(10000f, DamageType.Explosion, this, false);
					}
				}
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x0004DC1C File Offset: 0x0004BE1C
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!base.IsMovingOrOn)
		{
			this.Velocity = Vector3.zero;
			this.targetClimbSpeed = 0f;
			this.buoyancy.ArtificialHeight = null;
			return;
		}
		this.Velocity = base.GetLocalVelocity();
		this.UpdateWaterInfo();
		if (this.IsSurfaced && !this.wasOnSurface && base.transform.position.y > Env.oceanlevel - 1f)
		{
			this.wasOnSurface = true;
		}
		this.buoyancy.ArtificialHeight = new float?(this.waterSurfaceY);
		this.rigidBody.drag = (base.HasDriver() ? this.normalDrag : this.highDrag);
		float num = 2f;
		if (this.IsSurfaced)
		{
			float num2 = 20f * num;
			if (this.Oxygen < 0.5f)
			{
				this.Oxygen = 0.5f;
			}
			else
			{
				this.Oxygen += UnityEngine.Time.deltaTime / num2;
			}
		}
		else if (this.AnyMounted())
		{
			float num3 = BaseSubmarine.oxygenminutes * 60f * num;
			this.Oxygen -= UnityEngine.Time.deltaTime / num3;
		}
		this.engineController.CheckEngineState();
		if (this.engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.ThrottleInput));
			this.engineController.TickFuel(fuelPerSecond);
		}
		if (this.IsInWater)
		{
			float num4 = this.depthChangeTargetSpeed * this.UpDownInput;
			float num5;
			if ((this.UpDownInput > 0f && num4 > this.targetClimbSpeed && this.targetClimbSpeed > 0f) || (this.UpDownInput < 0f && num4 < this.targetClimbSpeed && this.targetClimbSpeed < 0f))
			{
				num5 = 0.7f;
			}
			else
			{
				num5 = 4f;
			}
			this.targetClimbSpeed = Mathf.MoveTowards(this.targetClimbSpeed, num4, num5 * UnityEngine.Time.fixedDeltaTime);
			float num6 = this.rigidBody.velocity.y - this.targetClimbSpeed;
			float value = this.buoyancy.buoyancyScale - num6 * 50f * UnityEngine.Time.fixedDeltaTime;
			this.buoyancy.buoyancyScale = Mathf.Clamp(value, 0.01f, 1f);
			Vector3 torque = Vector3.Cross(Quaternion.AngleAxis(this.rigidBody.angularVelocity.magnitude * 57.29578f * 10f / 200f, this.rigidBody.angularVelocity) * base.transform.up, Vector3.up) * 200f * 200f;
			this.rigidBody.AddTorque(torque);
			float d = 0.1f;
			this.rigidBody.AddForce(Vector3.up * -num6 * d, ForceMode.VelocityChange);
		}
		else
		{
			float b = 0f;
			this.buoyancy.buoyancyScale = Mathf.Lerp(this.buoyancy.buoyancyScale, b, UnityEngine.Time.fixedDeltaTime);
		}
		if (base.IsOn() && this.IsInWater)
		{
			this.rigidBody.AddForce(base.transform.forward * this.engineKW * 40f * this.ThrottleInput, ForceMode.Force);
			float num7 = this.turnPower * this.rigidBody.mass * this.rigidBody.angularDrag;
			float speed = this.GetSpeed();
			float num8 = Mathf.Min(Mathf.Abs(speed) * 0.6f, 6f) + 4f;
			float num9 = num7 * this.RudderInput * num8;
			if (speed < -1f)
			{
				num9 *= -1f;
			}
			this.rigidBody.AddTorque(base.transform.up * num9, ForceMode.Force);
		}
		this.UpdatePhysicalRudder(this.RudderInput, UnityEngine.Time.fixedDeltaTime);
		if (UnityEngine.Time.time >= this.nextCollisionDamageTime && this.maxDamageThisTick > 0f)
		{
			this.nextCollisionDamageTime = UnityEngine.Time.time + 0.33f;
			base.Hurt(this.maxDamageThisTick, DamageType.Collision, this, false);
			this.maxDamageThisTick = 0f;
		}
		StorageContainer torpedoContainer = this.GetTorpedoContainer();
		if (torpedoContainer != null)
		{
			bool b2 = torpedoContainer.inventory.HasAmmo(AmmoTypes.TORPEDO);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, b2, false, true);
		}
		global::BasePlayer driver = base.GetDriver();
		if (driver != null && this.primaryFireInput)
		{
			bool flag = true;
			if (this.IsInWater && this.timeSinceTorpedoFired >= this.maxFireRate)
			{
				float minSpeed = this.GetSpeed() + 2f;
				ServerProjectile serverProjectile;
				if (base.TryFireProjectile(torpedoContainer, AmmoTypes.TORPEDO, this.torpedoFiringPoint.position, this.torpedoFiringPoint.forward, driver, 1f, minSpeed, out serverProjectile))
				{
					this.timeSinceTorpedoFired = 0f;
					flag = false;
					driver.MarkHostileFor(60f);
					base.ClientRPC(null, "TorpedoFired");
				}
			}
			if (!this.prevPrimaryFireInput && flag && this.timeSinceFailRPCSent > 0.5f)
			{
				this.timeSinceFailRPCSent = 0f;
				base.ClientRPCPlayer(null, driver, "TorpedoFireFailed");
			}
		}
		else if (driver == null)
		{
			this.primaryFireInput = false;
		}
		this.prevPrimaryFireInput = this.primaryFireInput;
		if (this.timeSinceLastUsed > 300f && this.LightsAreOn)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
		}
		for (int i = 0; i < this.parentTriggers.Length; i++)
		{
			float num10 = this.parentTriggers[i].triggerWaterLevel.position.y - base.transform.position.y;
			bool flag2 = this.curSubDepthY - num10 <= 0f;
			if (flag2 != this.parentTriggers[i].trigger.enabled)
			{
				this.parentTriggers[i].trigger.enabled = flag2;
			}
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x0004E233 File Offset: 0x0004C433
	public override void LightToggle(global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0004E258 File Offset: 0x0004C458
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		this.timeSinceLastUsed = 0f;
		if (!base.IsDriver(player))
		{
			return;
		}
		if (inputState.IsDown(BUTTON.SPRINT))
		{
			this.UpDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.DUCK))
		{
			this.UpDownInput = -1f;
		}
		else
		{
			this.UpDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.ThrottleInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.ThrottleInput = -1f;
		}
		else
		{
			this.ThrottleInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.RudderInput = -1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.RudderInput = 1f;
		}
		else
		{
			this.RudderInput = 0f;
		}
		this.primaryFireInput = inputState.IsDown(BUTTON.FIRE_PRIMARY);
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD)) || (inputState.IsDown(BUTTON.SPRINT) && !inputState.WasDown(BUTTON.SPRINT)) || (inputState.IsDown(BUTTON.DUCK) && !inputState.WasDown(BUTTON.DUCK))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0004E3A8 File Offset: 0x0004C5A8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.submarine = Facepunch.Pool.Get<Submarine>();
		info.msg.submarine.throttle = this.ThrottleInput;
		info.msg.submarine.upDown = this.UpDownInput;
		info.msg.submarine.rudder = this.RudderInput;
		info.msg.submarine.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.submarine.fuelAmount = this.GetFuelAmount();
		info.msg.submarine.torpedoStorageID = this.torpedoStorageInstance.uid;
		info.msg.submarine.oxygen = this.Oxygen;
		info.msg.submarine.itemStorageID = this.itemStorageInstance.uid;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0003464E File Offset: 0x0003284E
	public bool MeetsEngineRequirements()
	{
		return this.AnyMounted();
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0004E490 File Offset: 0x0004C690
	public void OnEngineStartFailed()
	{
		base.ClientRPC(null, "EngineStartFailed");
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0004E4A0 File Offset: 0x0004C6A0
	public StorageContainer GetTorpedoContainer()
	{
		global::BaseEntity baseEntity = this.torpedoStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0004E4D8 File Offset: 0x0004C6D8
	public StorageContainer GetItemContainer()
	{
		global::BaseEntity baseEntity = this.itemStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0004E510 File Offset: 0x0004C710
	private void ProcessCollision(Collision collision)
	{
		if (base.isClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		float value = collision.impulse.magnitude / UnityEngine.Time.fixedDeltaTime;
		float num = Mathf.InverseLerp(100000f, 2500000f, value);
		if (num > 0f)
		{
			float b = Mathf.Lerp(1f, 200f, num);
			this.maxDamageThisTick = Mathf.Max(this.maxDamageThisTick, b);
		}
		if (num > 0f)
		{
			GameObjectRef effectGO = (this.curSubDepthY > 2f) ? this.underWatercollisionEffect : this.aboveWatercollisionEffect;
			base.TryShowCollisionFX(collision, effectGO);
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0004E5C4 File Offset: 0x0004C7C4
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.ThrottleInput + 1f) * 7f));
			byte b = (byte)((this.UpDownInput + 1f) * 7f);
			byte arg = (byte)(num + ((int)b << 4));
			int arg2 = Mathf.CeilToInt(this.GetFuelAmount());
			base.ClientRPC<float, byte, int, float>(null, "SubmarineUpdate", this.RudderInput, arg, arg2, this.Oxygen);
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0004E62C File Offset: 0x0004C82C
	private void SubmarineDecay()
	{
		BaseBoat.WaterVehicleDecay(this, 60f, this.timeSinceLastUsed, BaseSubmarine.outsidedecayminutes, BaseSubmarine.deepwaterdecayminutes);
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0004E650 File Offset: 0x0004C850
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0004E67C File Offset: 0x0004C87C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenTorpedoStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (!this.PlayerIsMounted(player))
		{
			return;
		}
		StorageContainer torpedoContainer = this.GetTorpedoContainer();
		if (torpedoContainer != null)
		{
			torpedoContainer.PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0004E6C4 File Offset: 0x0004C8C4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenItemStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer itemContainer = this.GetItemContainer();
		if (itemContainer != null)
		{
			itemContainer.PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0004E700 File Offset: 0x0004C900
	public void OnSurfacedInMoonpool()
	{
		if (!this.wasOnSurface || !GameInfo.HasAchievements)
		{
			return;
		}
		this.wasOnSurface = false;
		global::BasePlayer driver = base.GetDriver();
		if (driver != null)
		{
			driver.GiveAchievement("SUBMARINE_MOONPOOL");
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0004E73F File Offset: 0x0004C93F
	public ItemModGiveOxygen.AirSupplyType AirType
	{
		get
		{
			return ItemModGiveOxygen.AirSupplyType.Submarine;
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0004E742 File Offset: 0x0004C942
	public VehicleEngineController<BaseSubmarine>.EngineState EngineState
	{
		get
		{
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0004E74F File Offset: 0x0004C94F
	// (set) Token: 0x060007B4 RID: 1972 RVA: 0x0004E757 File Offset: 0x0004C957
	public Vector3 Velocity { get; private set; }

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060007B5 RID: 1973 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060007B6 RID: 1974 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool HasAmmo
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060007B7 RID: 1975 RVA: 0x0004E760 File Offset: 0x0004C960
	// (set) Token: 0x060007B8 RID: 1976 RVA: 0x0004E77B File Offset: 0x0004C97B
	public float ThrottleInput
	{
		get
		{
			if (!this.engineController.IsOn)
			{
				return 0f;
			}
			return this._throttle;
		}
		protected set
		{
			this._throttle = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060007B9 RID: 1977 RVA: 0x0004E793 File Offset: 0x0004C993
	// (set) Token: 0x060007BA RID: 1978 RVA: 0x0004E79B File Offset: 0x0004C99B
	public float RudderInput
	{
		get
		{
			return this._rudder;
		}
		protected set
		{
			this._rudder = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060007BB RID: 1979 RVA: 0x0004E7B4 File Offset: 0x0004C9B4
	// (set) Token: 0x060007BC RID: 1980 RVA: 0x0004E80D File Offset: 0x0004CA0D
	public float UpDownInput
	{
		get
		{
			if (!base.isServer)
			{
				return this._upDown;
			}
			if (this.timeSinceLastUsed >= this.timeUntilAutoSurface)
			{
				return 0.15f;
			}
			if (!this.engineController.IsOn)
			{
				return Mathf.Max(0f, this._upDown);
			}
			return this._upDown;
		}
		protected set
		{
			this._upDown = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060007BD RID: 1981 RVA: 0x0004E825 File Offset: 0x0004CA25
	// (set) Token: 0x060007BE RID: 1982 RVA: 0x0004E82D File Offset: 0x0004CA2D
	public float Oxygen
	{
		get
		{
			return this._oxygen;
		}
		protected set
		{
			this._oxygen = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060007BF RID: 1983 RVA: 0x0004E848 File Offset: 0x0004CA48
	protected float PhysicalRudderAngle
	{
		get
		{
			float num = this.rudderDetailedColliderTransform.localEulerAngles.y;
			if (num > 180f)
			{
				num -= 360f;
			}
			return num;
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0004E877 File Offset: 0x0004CA77
	protected bool IsInWater
	{
		get
		{
			return this.curSubDepthY > 0.2f;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060007C1 RID: 1985 RVA: 0x0004E886 File Offset: 0x0004CA86
	protected bool IsSurfaced
	{
		get
		{
			return this.curSubDepthY < 1.1f;
		}
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0004E898 File Offset: 0x0004CA98
	public override void InitShared()
	{
		base.InitShared();
		this.waterLayerMask = LayerMask.GetMask(new string[]
		{
			"Water"
		});
		this.engineController = new VehicleEngineController<BaseSubmarine>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, null, global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0004E8E8 File Offset: 0x0004CAE8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.submarine != null)
		{
			this.ThrottleInput = info.msg.submarine.throttle;
			this.UpDownInput = info.msg.submarine.upDown;
			this.RudderInput = info.msg.submarine.rudder;
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.submarine.fuelStorageID;
			this.cachedFuelAmount = info.msg.submarine.fuelAmount;
			this.torpedoStorageInstance.uid = info.msg.submarine.torpedoStorageID;
			this.Oxygen = info.msg.submarine.oxygen;
			this.itemStorageInstance.uid = info.msg.submarine.itemStorageID;
			this.UpdatePhysicalRudder(this.RudderInput, 0f);
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0004E9E6 File Offset: 0x0004CBE6
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public override float WaterFactorForPlayer(global::BasePlayer player)
	{
		return 0f;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0004EA05 File Offset: 0x0004CC05
	public override float AirFactor()
	{
		return this.Oxygen;
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlocksWaterFor(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0004EA0D File Offset: 0x0004CC0D
	public float GetFuelAmount()
	{
		if (base.isServer)
		{
			return (float)this.engineController.FuelSystem.GetFuelAmount();
		}
		return this.cachedFuelAmount;
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0004EA2F File Offset: 0x0004CC2F
	public float GetSpeed()
	{
		if (base.IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(this.Velocity, base.transform.forward);
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0004EA55 File Offset: 0x0004CC55
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || (!this.internalAccessStorage && !base.IsOn()));
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0004EA80 File Offset: 0x0004CC80
	public float GetAirTimeRemaining()
	{
		if (this.Oxygen <= 0.5f)
		{
			return 0f;
		}
		return Mathf.InverseLerp(0.5f, 1f, this.Oxygen) * BaseSubmarine.oxygenminutes * 60f;
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0004EAB6 File Offset: 0x0004CCB6
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !pusher.isMounted && !pusher.IsSwimming() && pusher.IsOnGround() && !pusher.IsStandingOnEntity(this, 8192);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0004EAEC File Offset: 0x0004CCEC
	private void UpdatePhysicalRudder(float turnInput, float deltaTime)
	{
		float num = -turnInput * this.maxRudderAngle;
		float y;
		if (base.IsMovingOrOn)
		{
			y = Mathf.MoveTowards(this.PhysicalRudderAngle, num, 200f * deltaTime);
		}
		else
		{
			y = num;
		}
		Quaternion localRotation = Quaternion.Euler(0f, y, 0f);
		if (base.isClient)
		{
			this.rudderVisualTransform.localRotation = localRotation;
		}
		this.rudderDetailedColliderTransform.localRotation = localRotation;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0004EB54 File Offset: 0x0004CD54
	private bool CanMount(global::BasePlayer player)
	{
		return !player.IsDead();
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0004EB5F File Offset: 0x0004CD5F
	private void UpdateWaterInfo()
	{
		this.waterSurfaceY = this.GetWaterSurfaceY();
		this.curSubDepthY = this.waterSurfaceY - base.transform.position.y;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0004EB8C File Offset: 0x0004CD8C
	private float GetWaterSurfaceY()
	{
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(base.transform.position - Vector3.up * 1.5f, Vector3.up, out raycastHit, 5f, this.waterLayerMask, QueryTriggerInteraction.Collide))
		{
			return raycastHit.point.y;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(base.transform.position, true, this, false);
		if (!waterInfo.isValid)
		{
			return base.transform.position.y - 1f;
		}
		return waterInfo.surfaceLevel;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00007C28 File Offset: 0x00005E28
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00007C32 File Offset: 0x00005E32
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x02000BB7 RID: 2999
	[Serializable]
	public class ParentTriggerInfo
	{
		// Token: 0x040040AA RID: 16554
		public TriggerParent trigger;

		// Token: 0x040040AB RID: 16555
		public Transform triggerWaterLevel;
	}
}
