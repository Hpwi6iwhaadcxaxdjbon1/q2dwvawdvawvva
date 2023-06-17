using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D1 RID: 209
public class Snowmobile : GroundVehicle, CarPhysics<global::Snowmobile>.ICar, TriggerHurtNotChild.IHurtTriggerUser, VehicleChassisVisuals<global::Snowmobile>.IClientWheelUser, IPrefabPreProcess
{
	// Token: 0x04000B92 RID: 2962
	private CarPhysics<global::Snowmobile> carPhysics;

	// Token: 0x04000B93 RID: 2963
	private VehicleTerrainHandler serverTerrainHandler;

	// Token: 0x04000B94 RID: 2964
	private CarWheel[] wheels;

	// Token: 0x04000B95 RID: 2965
	private TimeSince timeSinceLastUsed;

	// Token: 0x04000B96 RID: 2966
	private const float DECAY_TICK_TIME = 60f;

	// Token: 0x04000B97 RID: 2967
	private float prevTerrainModDrag;

	// Token: 0x04000B98 RID: 2968
	private TimeSince timeSinceTerrainModCheck;

	// Token: 0x04000B99 RID: 2969
	[Header("Snowmobile")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04000B9A RID: 2970
	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	// Token: 0x04000B9B RID: 2971
	[SerializeField]
	private VisualCarWheel wheelSkiFL;

	// Token: 0x04000B9C RID: 2972
	[SerializeField]
	private VisualCarWheel wheelSkiFR;

	// Token: 0x04000B9D RID: 2973
	[SerializeField]
	private VisualCarWheel wheelTreadFL;

	// Token: 0x04000B9E RID: 2974
	[SerializeField]
	private VisualCarWheel wheelTreadFR;

	// Token: 0x04000B9F RID: 2975
	[SerializeField]
	private VisualCarWheel wheelTreadRL;

	// Token: 0x04000BA0 RID: 2976
	[SerializeField]
	private VisualCarWheel wheelTreadRR;

	// Token: 0x04000BA1 RID: 2977
	[SerializeField]
	private CarSettings carSettings;

	// Token: 0x04000BA2 RID: 2978
	[SerializeField]
	private int engineKW = 59;

	// Token: 0x04000BA3 RID: 2979
	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	// Token: 0x04000BA4 RID: 2980
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000BA5 RID: 2981
	[SerializeField]
	private float airControlStability = 10f;

	// Token: 0x04000BA6 RID: 2982
	[SerializeField]
	private float airControlPower = 40f;

	// Token: 0x04000BA7 RID: 2983
	[SerializeField]
	private float badTerrainDrag = 1f;

	// Token: 0x04000BA8 RID: 2984
	[SerializeField]
	private ProtectionProperties riderProtection;

	// Token: 0x04000BA9 RID: 2985
	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	// Token: 0x04000BAA RID: 2986
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	// Token: 0x04000BAB RID: 2987
	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	// Token: 0x04000BAC RID: 2988
	[Header("Snowmobile Visuals")]
	public float minGroundFXSpeed;

	// Token: 0x04000BAD RID: 2989
	[SerializeField]
	private SnowmobileChassisVisuals chassisVisuals;

	// Token: 0x04000BAE RID: 2990
	[SerializeField]
	private VehicleLight[] lights;

	// Token: 0x04000BAF RID: 2991
	[SerializeField]
	private Transform steeringLeftIK;

	// Token: 0x04000BB0 RID: 2992
	[SerializeField]
	private Transform steeringRightIK;

	// Token: 0x04000BB1 RID: 2993
	[SerializeField]
	private Transform leftFootIK;

	// Token: 0x04000BB2 RID: 2994
	[SerializeField]
	private Transform rightFootIK;

	// Token: 0x04000BB3 RID: 2995
	[SerializeField]
	private Transform starterKey;

	// Token: 0x04000BB4 RID: 2996
	[SerializeField]
	private Vector3 engineOffKeyRot;

	// Token: 0x04000BB5 RID: 2997
	[SerializeField]
	private Vector3 engineOnKeyRot;

	// Token: 0x04000BB6 RID: 2998
	[ServerVar(Help = "How long before a snowmobile loses all its health while outside")]
	public static float outsideDecayMinutes = 1440f;

	// Token: 0x04000BB7 RID: 2999
	[ServerVar(Help = "Allow mounting as a passenger when there's no driver")]
	public static bool allowPassengerOnly = false;

	// Token: 0x04000BB8 RID: 3000
	[ServerVar(Help = "If true, snowmobile goes fast on all terrain types")]
	public static bool allTerrain = false;

	// Token: 0x04000BB9 RID: 3001
	private float _throttle;

	// Token: 0x04000BBA RID: 3002
	private float _brake;

	// Token: 0x04000BBB RID: 3003
	private float _steer;

	// Token: 0x04000BBC RID: 3004
	private float _mass = -1f;

	// Token: 0x04000BBD RID: 3005
	public const global::BaseEntity.Flags Flag_Slowmode = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000BBE RID: 3006
	private EntityRef<StorageContainer> itemStorageInstance;

	// Token: 0x04000BBF RID: 3007
	private float cachedFuelFraction;

	// Token: 0x04000BC0 RID: 3008
	private const float FORCE_MULTIPLIER = 10f;

	// Token: 0x06001296 RID: 4758 RVA: 0x00095FC0 File Offset: 0x000941C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Snowmobile.OnRpcMessage", 0))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001297 RID: 4759 RVA: 0x00096274 File Offset: 0x00094474
	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (this.serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return this.serverTerrainHandler.OnSurface;
		}
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0009628C File Offset: 0x0009448C
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceLastUsed = 0f;
		this.rigidBody.centerOfMass = this.centreOfMassTransform.localPosition;
		this.rigidBody.inertiaTensor = new Vector3(450f, 200f, 200f);
		this.carPhysics = new CarPhysics<global::Snowmobile>(this, base.transform, this.rigidBody, this.carSettings);
		this.serverTerrainHandler = new VehicleTerrainHandler(this);
		base.InvokeRandomized(new Action(this.UpdateClients), 0f, 0.15f, 0.02f);
		base.InvokeRandomized(new Action(this.SnowmobileDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x0009635C File Offset: 0x0009455C
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		float speed = base.GetSpeed();
		this.carPhysics.FixedUpdate(UnityEngine.Time.fixedDeltaTime, speed);
		this.serverTerrainHandler.FixedUpdate();
		if (base.IsOn())
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.ThrottleInput));
			this.engineController.TickFuel(fuelPerSecond);
		}
		this.engineController.CheckEngineState();
		RaycastHit raycastHit;
		if (!this.carPhysics.IsGrounded() && UnityEngine.Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 10f, 1218511105, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector = raycastHit.normal;
			Vector3 right = base.transform.right;
			right.y = 0f;
			vector = Vector3.ProjectOnPlane(vector, right);
			float num = Vector3.Angle(vector, Vector3.up);
			float angle = this.rigidBody.angularVelocity.magnitude * 57.29578f * this.airControlStability / this.airControlPower;
			if (num <= 45f)
			{
				Vector3 torque = Vector3.Cross(Quaternion.AngleAxis(angle, this.rigidBody.angularVelocity) * base.transform.up, vector) * this.airControlPower * this.airControlPower;
				this.rigidBody.AddTorque(torque);
			}
		}
		this.hurtTriggerFront.gameObject.SetActive(speed > this.hurtTriggerMinSpeed);
		this.hurtTriggerRear.gameObject.SetActive(speed < -this.hurtTriggerMinSpeed);
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x000964EC File Offset: 0x000946EC
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		this.timeSinceLastUsed = 0f;
		if (inputState.IsDown(BUTTON.DUCK))
		{
			this.SteerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			this.SteerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.SteerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.SteerInput = 1f;
			}
		}
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		this.ThrottleInput = 0f;
		this.BrakeInput = 0f;
		if (base.GetSpeed() > 3f && num < -0.1f)
		{
			this.ThrottleInput = 0f;
			this.BrakeInput = -num;
		}
		else
		{
			this.ThrottleInput = num;
			this.BrakeInput = 0f;
		}
		if (this.engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			this.engineController.TryStartEngine(player);
		}
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x00096628 File Offset: 0x00094828
	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float bias = Mathf.Lerp(0.3f, 0.75f, this.GetPerformanceFraction());
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, bias);
		return maxDriveForce * num;
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00096664 File Offset: 0x00094864
	public override float GetModifiedDrag()
	{
		float num = base.GetModifiedDrag();
		if (!global::Snowmobile.allTerrain)
		{
			VehicleTerrainHandler.Surface onSurface = this.serverTerrainHandler.OnSurface;
			if (this.serverTerrainHandler.IsGrounded && onSurface != VehicleTerrainHandler.Surface.Frictionless && onSurface != VehicleTerrainHandler.Surface.Sand && onSurface != VehicleTerrainHandler.Surface.Snow && onSurface != VehicleTerrainHandler.Surface.Ice)
			{
				float num2 = Mathf.Max(num, this.badTerrainDrag);
				if (num2 <= this.prevTerrainModDrag)
				{
					num = this.prevTerrainModDrag;
				}
				else
				{
					num = Mathf.MoveTowards(this.prevTerrainModDrag, num2, 0.33f * this.timeSinceTerrainModCheck);
				}
				this.prevTerrainModDrag = num;
			}
			else
			{
				this.prevTerrainModDrag = 0f;
			}
		}
		this.timeSinceTerrainModCheck = 0f;
		this.InSlowMode = (num >= this.badTerrainDrag);
		return num;
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00074975 File Offset: 0x00072B75
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x0009671C File Offset: 0x0009491C
	public CarWheel[] GetWheels()
	{
		if (this.wheels == null)
		{
			this.wheels = new CarWheel[]
			{
				this.wheelSkiFL,
				this.wheelSkiFR,
				this.wheelTreadFL,
				this.wheelTreadFR,
				this.wheelTreadRL,
				this.wheelTreadRR
			};
		}
		return this.wheels;
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x00096779 File Offset: 0x00094979
	public float GetWheelsMidPos()
	{
		return (this.wheelSkiFL.wheelCollider.transform.localPosition.z - this.wheelTreadRL.wheelCollider.transform.localPosition.z) * 0.5f;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000967B8 File Offset: 0x000949B8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.snowmobile = Facepunch.Pool.Get<ProtoBuf.Snowmobile>();
		info.msg.snowmobile.steerInput = this.SteerInput;
		info.msg.snowmobile.driveWheelVel = this.DriveWheelVelocity;
		info.msg.snowmobile.throttleInput = this.ThrottleInput;
		info.msg.snowmobile.brakeInput = this.BrakeInput;
		info.msg.snowmobile.storageID = this.itemStorageInstance.uid;
		info.msg.snowmobile.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int StartingFuelUnits()
	{
		return 0;
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00096870 File Offset: 0x00094A70
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned && child.prefabID == this.itemStoragePrefab.GetEntity().prefabID)
		{
			this.itemStorageInstance.Set((StorageContainer)child);
		}
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000968C0 File Offset: 0x00094AC0
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

	// Token: 0x060012A4 RID: 4772 RVA: 0x00074995 File Offset: 0x00072B95
	public override bool MeetsEngineRequirements()
	{
		return base.HasDriver();
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x00096904 File Offset: 0x00094B04
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (global::Snowmobile.allowPassengerOnly)
		{
			base.AttemptMount(player, doMountChecks);
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

	// Token: 0x060012A6 RID: 4774 RVA: 0x00096974 File Offset: 0x00094B74
	public void SnowmobileDecay()
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.timeSinceLastUsed < 2700f)
		{
			return;
		}
		float num = this.IsOutside() ? global::Snowmobile.outsideDecayMinutes : float.PositiveInfinity;
		if (!float.IsPositiveInfinity(num))
		{
			float num2 = 1f / num;
			base.Hurt(this.MaxHealth() * num2, DamageType.Decay, this, false);
		}
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x000969D4 File Offset: 0x00094BD4
	public StorageContainer GetItemContainer()
	{
		global::BaseEntity baseEntity = this.itemStorageInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x00096A0C File Offset: 0x00094C0C
	private void UpdateClients()
	{
		if (base.HasDriver())
		{
			int num = (int)((byte)((this.ThrottleInput + 1f) * 7f));
			byte b = (byte)(this.BrakeInput * 15f);
			byte arg = (byte)(num + ((int)b << 4));
			base.ClientRPC<float, byte, float, float>(null, "SnowmobileUpdate", this.SteerInput, arg, this.DriveWheelVelocity, this.GetFuelFraction());
		}
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x00096A67 File Offset: 0x00094C67
	public override void OnEngineStartFailed()
	{
		base.ClientRPC(null, "EngineStartFailed");
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x00096A75 File Offset: 0x00094C75
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.riderProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00096A98 File Offset: 0x00094C98
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (!base.IsDriver(player))
		{
			return;
		}
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00096ACC File Offset: 0x00094CCC
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

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x060012AD RID: 4781 RVA: 0x00096B08 File Offset: 0x00094D08
	// (set) Token: 0x060012AE RID: 4782 RVA: 0x00096B23 File Offset: 0x00094D23
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

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x060012AF RID: 4783 RVA: 0x00096B3B File Offset: 0x00094D3B
	// (set) Token: 0x060012B0 RID: 4784 RVA: 0x00096B43 File Offset: 0x00094D43
	public float BrakeInput
	{
		get
		{
			return this._brake;
		}
		protected set
		{
			this._brake = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x060012B1 RID: 4785 RVA: 0x00096B5B File Offset: 0x00094D5B
	public bool IsBraking
	{
		get
		{
			return this.BrakeInput > 0f;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x060012B2 RID: 4786 RVA: 0x00096B6A File Offset: 0x00094D6A
	// (set) Token: 0x060012B3 RID: 4787 RVA: 0x00096B72 File Offset: 0x00094D72
	public float SteerInput
	{
		get
		{
			return this._steer;
		}
		protected set
		{
			this._steer = Mathf.Clamp(value, -1f, 1f);
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x060012B4 RID: 4788 RVA: 0x00096B8A File Offset: 0x00094D8A
	public float SteerAngle
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.SteerAngle;
			}
			return 0f;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00096BA5 File Offset: 0x00094DA5
	public override float DriveWheelVelocity
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelVelocity;
			}
			return 0f;
		}
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060012B6 RID: 4790 RVA: 0x00096BC0 File Offset: 0x00094DC0
	public float DriveWheelSlip
	{
		get
		{
			if (base.isServer)
			{
				return this.carPhysics.DriveWheelSlip;
			}
			return 0f;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060012B7 RID: 4791 RVA: 0x00096BDB File Offset: 0x00094DDB
	public float MaxSteerAngle
	{
		get
		{
			return this.carSettings.maxSteerAngle;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060012B8 RID: 4792 RVA: 0x00003278 File Offset: 0x00001478
	// (set) Token: 0x060012B9 RID: 4793 RVA: 0x00096BE8 File Offset: 0x00094DE8
	public bool InSlowMode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
		private set
		{
			if (this.InSlowMode != value)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, value, false, true);
			}
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060012BA RID: 4794 RVA: 0x00096C01 File Offset: 0x00094E01
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x00096C20 File Offset: 0x00094E20
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.snowmobile == null)
		{
			return;
		}
		this.itemStorageInstance.uid = info.msg.snowmobile.storageID;
		this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.snowmobile.fuelStorageID;
		this.cachedFuelFraction = info.msg.snowmobile.fuelFraction;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x00096C98 File Offset: 0x00094E98
	public float GetMaxDriveForce()
	{
		return (float)this.engineKW * 10f * this.GetPerformanceFraction();
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x00096CAE File Offset: 0x00094EAE
	public override float GetMaxForwardSpeed()
	{
		return this.GetMaxDriveForce() / this.Mass * 15f;
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x00096CC3 File Offset: 0x00094EC3
	public override float GetThrottleInput()
	{
		return this.ThrottleInput;
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00096CCB File Offset: 0x00094ECB
	public override float GetBrakeInput()
	{
		return this.BrakeInput;
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00096CD3 File Offset: 0x00094ED3
	public float GetSteerInput()
	{
		return this.SteerInput;
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool GetSteerModInput()
	{
		return false;
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00096CDC File Offset: 0x00094EDC
	public float GetPerformanceFraction()
	{
		float t = Mathf.InverseLerp(0.25f, 0.5f, base.healthFraction);
		return Mathf.Lerp(0.5f, 1f, t);
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00096D0F File Offset: 0x00094F0F
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelFraction();
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00075376 File Offset: 0x00073576
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && (this.PlayerIsMounted(player) || !base.IsOn());
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00096D30 File Offset: 0x00094F30
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && GameInfo.HasAchievements && !old.HasFlag(global::BaseEntity.Flags.On) && next.HasFlag(global::BaseEntity.Flags.On))
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver != null && driver.FindTrigger<TriggerSnowmobileAchievement>() != null)
			{
				driver.GiveAchievement("DRIVE_SNOWMOBILE");
			}
		}
	}
}
