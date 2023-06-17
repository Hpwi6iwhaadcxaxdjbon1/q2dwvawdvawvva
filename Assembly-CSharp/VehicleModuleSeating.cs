using System;
using System.Collections.Generic;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E8 RID: 232
public class VehicleModuleSeating : BaseVehicleModule, IPrefabPreProcess
{
	// Token: 0x04000CE3 RID: 3299
	[SerializeField]
	private ProtectionProperties passengerProtection;

	// Token: 0x04000CE4 RID: 3300
	[SerializeField]
	private ModularCarCodeLockVisuals codeLockVisuals;

	// Token: 0x04000CE5 RID: 3301
	[SerializeField]
	private VehicleModuleSeating.Seating seating;

	// Token: 0x04000CE6 RID: 3302
	[SerializeField]
	[HideInInspector]
	private Vector3 steerAngle;

	// Token: 0x04000CE7 RID: 3303
	[SerializeField]
	[HideInInspector]
	private Vector3 accelAngle;

	// Token: 0x04000CE8 RID: 3304
	[SerializeField]
	[HideInInspector]
	private Vector3 brakeAngle;

	// Token: 0x04000CE9 RID: 3305
	[SerializeField]
	[HideInInspector]
	private Vector3 speedometerAngle;

	// Token: 0x04000CEA RID: 3306
	[SerializeField]
	[HideInInspector]
	private Vector3 fuelAngle;

	// Token: 0x04000CEB RID: 3307
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04000CEC RID: 3308
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04000CED RID: 3309
	private const BaseEntity.Flags FLAG_HORN = BaseEntity.Flags.Reserved8;

	// Token: 0x04000CEE RID: 3310
	private float steerPercent;

	// Token: 0x04000CEF RID: 3311
	private float throttlePercent;

	// Token: 0x04000CF0 RID: 3312
	private float brakePercent;

	// Token: 0x04000CF1 RID: 3313
	private bool? checkEngineLightOn;

	// Token: 0x04000CF2 RID: 3314
	private bool? fuelLightOn;

	// Token: 0x04000CF4 RID: 3316
	protected IVehicleLockUser VehicleLockUser;

	// Token: 0x04000CF5 RID: 3317
	private MaterialPropertyBlock dashboardLightPB;

	// Token: 0x04000CF6 RID: 3318
	private static int emissionColorID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000CF7 RID: 3319
	private BasePlayer hornPlayer;

	// Token: 0x06001479 RID: 5241 RVA: 0x000A1E34 File Offset: 0x000A0034
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleSeating.OnRpcMessage", 0))
		{
			if (rpc == 2791546333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DestroyLock ");
				}
				using (TimeWarning.New("RPC_DestroyLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2791546333U, "RPC_DestroyLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DestroyLock(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_DestroyLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x0600147A RID: 5242 RVA: 0x000A1F9C File Offset: 0x000A019C
	public override bool HasSeating
	{
		get
		{
			return this.mountPoints.Count > 0;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x0600147B RID: 5243 RVA: 0x000A1FAC File Offset: 0x000A01AC
	// (set) Token: 0x0600147C RID: 5244 RVA: 0x000A1FB4 File Offset: 0x000A01B4
	public ModularCar Car { get; private set; }

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x0600147D RID: 5245 RVA: 0x000A1FBD File Offset: 0x000A01BD
	protected bool IsOnACar
	{
		get
		{
			return this.Car != null;
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x0600147E RID: 5246 RVA: 0x000A1FCB File Offset: 0x000A01CB
	protected bool IsOnAVehicleLockUser
	{
		get
		{
			return this.VehicleLockUser != null;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x0600147F RID: 5247 RVA: 0x000A1FD6 File Offset: 0x000A01D6
	public bool DoorsAreLockable
	{
		get
		{
			return this.seating.doorsAreLockable;
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x000A1FE4 File Offset: 0x000A01E4
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (this.seating.steeringWheel != null)
		{
			this.steerAngle = this.seating.steeringWheel.localEulerAngles;
		}
		if (this.seating.accelPedal != null)
		{
			this.accelAngle = this.seating.accelPedal.localEulerAngles;
		}
		if (this.seating.brakePedal != null)
		{
			this.brakeAngle = this.seating.brakePedal.localEulerAngles;
		}
		if (this.seating.speedometer != null)
		{
			this.speedometerAngle = new Vector3(-160f, 0f, -40f);
		}
		if (this.seating.fuelGauge != null)
		{
			this.fuelAngle = this.seating.fuelGauge.localEulerAngles;
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x000A20D4 File Offset: 0x000A02D4
	public virtual bool IsOnThisModule(BasePlayer player)
	{
		BaseMountable mounted = player.GetMounted();
		return mounted != null && mounted.GetParentEntity() as VehicleModuleSeating == this;
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x000A2104 File Offset: 0x000A0304
	public bool HasADriverSeat()
	{
		using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
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

	// Token: 0x06001483 RID: 5251 RVA: 0x000A2160 File Offset: 0x000A0360
	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		base.ModuleAdded(vehicle, firstSocketIndex);
		this.Car = (vehicle as ModularCar);
		this.VehicleLockUser = (vehicle as IVehicleLockUser);
		if (this.HasSeating && base.isServer)
		{
			using (List<BaseVehicle.MountPointInfo>.Enumerator enumerator = this.mountPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModularCarSeat modularCarSeat;
					if ((modularCarSeat = (enumerator.Current.mountable as ModularCarSeat)) != null)
					{
						modularCarSeat.associatedSeatingModule = this;
					}
				}
			}
		}
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x000A21F0 File Offset: 0x000A03F0
	public override void ModuleRemoved()
	{
		base.ModuleRemoved();
		this.Car = null;
		this.VehicleLockUser = null;
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x000A2208 File Offset: 0x000A0408
	public bool PlayerCanDestroyLock(BasePlayer player)
	{
		return this.IsOnAVehicleLockUser && !(player == null) && !base.Vehicle.IsDead() && this.HasADriverSeat() && this.VehicleLockUser.PlayerCanDestroyLock(player, this) && (!player.isMounted || !this.VehicleLockUser.PlayerHasUnlockPermission(player));
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x000A226B File Offset: 0x000A046B
	protected BaseVehicleSeat GetSeatAtIndex(int index)
	{
		return this.mountPoints[index].mountable as BaseVehicleSeat;
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x000A2283 File Offset: 0x000A0483
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		if (this.passengerProtection != null)
		{
			this.passengerProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x000A22B4 File Offset: 0x000A04B4
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.hornLoop != null && this.IsOnThisModule(player))
		{
			bool flag = inputState.IsDown(BUTTON.FIRE_PRIMARY);
			if (flag != base.HasFlag(BaseEntity.Flags.Reserved8))
			{
				base.SetFlag(BaseEntity.Flags.Reserved8, flag, false, true);
			}
			if (flag)
			{
				this.hornPlayer = player;
			}
		}
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x000A2312 File Offset: 0x000A0512
	public override void OnPlayerDismountedVehicle(BasePlayer player)
	{
		base.OnPlayerDismountedVehicle(player);
		if (base.HasFlag(BaseEntity.Flags.Reserved8) && player == this.hornPlayer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		}
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x000A2344 File Offset: 0x000A0544
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DestroyLock(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (this.IsOnAVehicleLockUser)
		{
			if (!this.PlayerCanDestroyLock(player))
			{
				return;
			}
			this.VehicleLockUser.RemoveLock();
		}
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000A2375 File Offset: 0x000A0575
	protected virtual Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		return desiredPos;
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x000A2378 File Offset: 0x000A0578
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		if (!GameInfo.HasAchievements || base.isClient || newState != VehicleEngineController<GroundVehicle>.EngineState.On || this.mountPoints == null)
		{
			return;
		}
		bool flag = true;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.Car.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleEngine vehicleModuleEngine;
				if ((vehicleModuleEngine = (enumerator.Current as VehicleModuleEngine)) != null && !vehicleModuleEngine.AtPeakPerformance)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable.GetMounted() != null)
				{
					mountPointInfo.mountable.GetMounted().GiveAchievement("BUCKLE_UP");
				}
			}
		}
	}

	// Token: 0x02000C12 RID: 3090
	[Serializable]
	public class MountHotSpot
	{
		// Token: 0x040041DF RID: 16863
		public Transform transform;

		// Token: 0x040041E0 RID: 16864
		public Vector2 size;
	}

	// Token: 0x02000C13 RID: 3091
	[Serializable]
	public class Seating
	{
		// Token: 0x040041E1 RID: 16865
		[Header("Seating & Controls")]
		public bool doorsAreLockable = true;

		// Token: 0x040041E2 RID: 16866
		[Obsolete("Use BaseVehicle.mountPoints instead")]
		[HideInInspector]
		public BaseVehicle.MountPointInfo[] mountPoints;

		// Token: 0x040041E3 RID: 16867
		public Transform steeringWheel;

		// Token: 0x040041E4 RID: 16868
		public Transform accelPedal;

		// Token: 0x040041E5 RID: 16869
		public Transform brakePedal;

		// Token: 0x040041E6 RID: 16870
		public Transform steeringWheelLeftGrip;

		// Token: 0x040041E7 RID: 16871
		public Transform steeringWheelRightGrip;

		// Token: 0x040041E8 RID: 16872
		public Transform accelPedalGrip;

		// Token: 0x040041E9 RID: 16873
		public Transform brakePedalGrip;

		// Token: 0x040041EA RID: 16874
		public VehicleModuleSeating.MountHotSpot[] mountHotSpots;

		// Token: 0x040041EB RID: 16875
		[Header("Dashboard")]
		public Transform speedometer;

		// Token: 0x040041EC RID: 16876
		public Transform fuelGauge;

		// Token: 0x040041ED RID: 16877
		public Renderer dashboardRenderer;

		// Token: 0x040041EE RID: 16878
		[Range(0f, 3f)]
		public int checkEngineLightMatIndex = 2;

		// Token: 0x040041EF RID: 16879
		[ColorUsage(true, true)]
		public Color checkEngineLightEmission;

		// Token: 0x040041F0 RID: 16880
		[Range(0f, 3f)]
		public int fuelLightMatIndex = 3;

		// Token: 0x040041F1 RID: 16881
		[ColorUsage(true, true)]
		public Color fuelLightEmission;
	}
}
