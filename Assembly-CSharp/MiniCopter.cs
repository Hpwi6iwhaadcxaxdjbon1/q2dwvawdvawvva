using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000029 RID: 41
public class MiniCopter : BaseHelicopterVehicle, IEngineControllerUser, IEntity, SamSite.ISamSiteTarget
{
	// Token: 0x04000112 RID: 274
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	// Token: 0x04000113 RID: 275
	public float fuelPerSec = 0.25f;

	// Token: 0x04000114 RID: 276
	public float fuelGaugeMax = 100f;

	// Token: 0x04000115 RID: 277
	private float cachedFuelFraction;

	// Token: 0x04000116 RID: 278
	public Transform waterSample;

	// Token: 0x04000117 RID: 279
	public WheelCollider leftWheel;

	// Token: 0x04000118 RID: 280
	public WheelCollider rightWheel;

	// Token: 0x04000119 RID: 281
	public WheelCollider frontWheel;

	// Token: 0x0400011A RID: 282
	public Transform leftWheelTrans;

	// Token: 0x0400011B RID: 283
	public Transform rightWheelTrans;

	// Token: 0x0400011C RID: 284
	public Transform frontWheelTrans;

	// Token: 0x0400011D RID: 285
	public float cachedrotation_left;

	// Token: 0x0400011E RID: 286
	public float cachedrotation_right;

	// Token: 0x0400011F RID: 287
	public float cachedrotation_front;

	// Token: 0x04000120 RID: 288
	[Header("IK")]
	public Transform joystickPositionLeft;

	// Token: 0x04000121 RID: 289
	public Transform joystickPositionRight;

	// Token: 0x04000122 RID: 290
	public Transform leftFootPosition;

	// Token: 0x04000123 RID: 291
	public Transform rightFootPosition;

	// Token: 0x04000124 RID: 292
	public AnimationCurve bladeEngineCurve;

	// Token: 0x04000125 RID: 293
	public Animator animator;

	// Token: 0x04000126 RID: 294
	public float maxRotorSpeed = 10f;

	// Token: 0x04000127 RID: 295
	public float timeUntilMaxRotorSpeed = 7f;

	// Token: 0x04000128 RID: 296
	public float rotorBlurThreshold = 8f;

	// Token: 0x04000129 RID: 297
	public Transform mainRotorBlur;

	// Token: 0x0400012A RID: 298
	public Transform mainRotorBlades;

	// Token: 0x0400012B RID: 299
	public Transform rearRotorBlades;

	// Token: 0x0400012C RID: 300
	public Transform rearRotorBlur;

	// Token: 0x0400012D RID: 301
	public float motorForceConstant = 150f;

	// Token: 0x0400012E RID: 302
	public float brakeForceConstant = 500f;

	// Token: 0x0400012F RID: 303
	public GameObject preventBuildingObject;

	// Token: 0x04000130 RID: 304
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	// Token: 0x04000131 RID: 305
	[ServerVar(Help = "How long before a minicopter loses all its health while outside")]
	public static float outsidedecayminutes = 480f;

	// Token: 0x04000132 RID: 306
	[ServerVar(Help = "How long before a minicopter loses all its health while indoors")]
	public static float insidedecayminutes = 2880f;

	// Token: 0x04000133 RID: 307
	private VehicleEngineController<MiniCopter> engineController;

	// Token: 0x04000134 RID: 308
	private bool isPushing;

	// Token: 0x04000135 RID: 309
	private float lastEngineOnTime;

	// Token: 0x04000136 RID: 310
	private float cachedPitch;

	// Token: 0x04000137 RID: 311
	private float cachedYaw;

	// Token: 0x04000138 RID: 312
	private float cachedRoll;

	// Token: 0x060000F5 RID: 245 RVA: 0x00007076 File Offset: 0x00005276
	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			this.cachedFuelFraction = Mathf.Clamp01((float)this.GetFuelSystem().GetFuelAmount() / this.fuelGaugeMax);
		}
		return this.cachedFuelFraction;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x000070A4 File Offset: 0x000052A4
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x000070B1 File Offset: 0x000052B1
	public override int StartingFuelUnits()
	{
		return 100;
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000070B5 File Offset: 0x000052B5
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x000070DC File Offset: 0x000052DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		global::BasePlayer driver = base.GetDriver();
		if (driver != null && driver != player)
		{
			return;
		}
		if (base.IsSafe() && player != this.creatorEntity)
		{
			return;
		}
		this.engineController.FuelSystem.LootFuel(player);
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060000FA RID: 250 RVA: 0x0000713C File Offset: 0x0000533C
	public bool IsStartingUp
	{
		get
		{
			return this.engineController != null && this.engineController.IsStarting;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060000FB RID: 251 RVA: 0x00007153 File Offset: 0x00005353
	public VehicleEngineController<MiniCopter>.EngineState CurEngineState
	{
		get
		{
			if (this.engineController == null)
			{
				return VehicleEngineController<MiniCopter>.EngineState.Off;
			}
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x0000716A File Offset: 0x0000536A
	public override void InitShared()
	{
		this.engineController = new VehicleEngineController<MiniCopter>(this, base.isServer, 5f, this.fuelStoragePrefab, this.waterSample, global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060000FD RID: 253 RVA: 0x00007194 File Offset: 0x00005394
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeVehicle;
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x0000719B File Offset: 0x0000539B
	public override float GetServiceCeiling()
	{
		return global::HotAirBalloon.serviceCeiling;
	}

	// Token: 0x060000FF RID: 255 RVA: 0x000071A2 File Offset: 0x000053A2
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return staticRespawn || !base.InSafeZone();
	}

	// Token: 0x06000100 RID: 256 RVA: 0x000071B4 File Offset: 0x000053B4
	public override void PilotInput(InputState inputState, global::BasePlayer player)
	{
		base.PilotInput(inputState, player);
		if (!base.IsOn() && !this.IsStartingUp && inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD))
		{
			this.engineController.TryStartEngine(player);
		}
		this.currentInputState.groundControl = inputState.IsDown(BUTTON.DUCK);
		if (this.currentInputState.groundControl)
		{
			this.currentInputState.roll = 0f;
			this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
			this.currentInputState.throttle -= (inputState.IsDown(BUTTON.BACKWARD) ? 1f : 0f);
		}
		this.cachedRoll = this.currentInputState.roll;
		this.cachedYaw = this.currentInputState.yaw;
		this.cachedPitch = this.currentInputState.pitch;
	}

	// Token: 0x06000101 RID: 257 RVA: 0x000072A1 File Offset: 0x000054A1
	public bool Grounded()
	{
		return this.leftWheel.isGrounded && this.rightWheel.isGrounded;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000072C0 File Offset: 0x000054C0
	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		this.cachedRoll = 0f;
		this.cachedYaw = 0f;
		this.cachedPitch = 0f;
		if (this.Grounded())
		{
			return;
		}
		if (base.HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, base.transform.right);
			float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = ((num < 0f) ? 1f : 0f);
			this.currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < --0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (num2 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
		else
		{
			this.currentInputState.throttle = -1f;
		}
	}

	// Token: 0x06000103 RID: 259 RVA: 0x000073C0 File Offset: 0x000055C0
	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		float brakeScale;
		float num;
		float turning;
		if (this.currentInputState.groundControl)
		{
			brakeScale = ((this.currentInputState.throttle == 0f) ? 50f : 0f);
			num = this.currentInputState.throttle;
			turning = this.currentInputState.yaw;
		}
		else
		{
			brakeScale = 20f;
			turning = 0f;
			num = 0f;
		}
		num *= (base.IsOn() ? 1f : 0f);
		if (this.isPushing)
		{
			brakeScale = 0f;
			num = 0.1f;
			turning = 0f;
		}
		this.ApplyWheelForce(this.frontWheel, num, brakeScale, turning);
		this.ApplyWheelForce(this.leftWheel, num, brakeScale, 0f);
		this.ApplyWheelForce(this.rightWheel, num, brakeScale, 0f);
	}

	// Token: 0x06000104 RID: 260 RVA: 0x000074AC File Offset: 0x000056AC
	public void ApplyWheelForce(WheelCollider wheel, float gasScale, float brakeScale, float turning)
	{
		if (wheel.isGrounded)
		{
			float num = gasScale * this.motorForceConstant;
			float num2 = brakeScale * this.brakeForceConstant;
			float num3 = 45f * turning;
			if (!Mathf.Approximately(wheel.motorTorque, num))
			{
				wheel.motorTorque = num;
			}
			if (!Mathf.Approximately(wheel.brakeTorque, num2))
			{
				wheel.brakeTorque = num2;
			}
			if (!Mathf.Approximately(wheel.steerAngle, num3))
			{
				wheel.steerAngle = num3;
			}
		}
	}

	// Token: 0x06000105 RID: 261 RVA: 0x0000751B File Offset: 0x0000571B
	public override void MovementUpdate()
	{
		if (this.Grounded())
		{
			this.ApplyForceAtWheels();
		}
		if (base.IsOn() && (!this.currentInputState.groundControl || !this.Grounded()))
		{
			base.MovementUpdate();
		}
	}

	// Token: 0x06000106 RID: 262 RVA: 0x00007550 File Offset: 0x00005750
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastEngineOnTime = UnityEngine.Time.realtimeSinceStartup;
		this.rigidBody.inertiaTensor = this.rigidBody.inertiaTensor;
		this.preventBuildingObject.SetActive(true);
		base.InvokeRandomized(new Action(this.UpdateNetwork), 0f, 0.2f, 0.05f);
		base.InvokeRandomized(new Action(this.DecayTick), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x000075DC File Offset: 0x000057DC
	public void DecayTick()
	{
		if (base.healthFraction == 0f)
		{
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f / (this.IsOutside() ? MiniCopter.outsidedecayminutes : MiniCopter.insidedecayminutes);
		base.Hurt(this.MaxHealth() * num, DamageType.Decay, this, false);
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00007641 File Offset: 0x00005841
	public override bool ShouldApplyHoverForce()
	{
		return base.IsOn();
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00007641 File Offset: 0x00005841
	public override bool IsEngineOn()
	{
		return base.IsOn();
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00007649 File Offset: 0x00005849
	public bool MeetsEngineRequirements()
	{
		if (this.engineController.IsOff)
		{
			return base.HasDriver();
		}
		return base.HasDriver() || UnityEngine.Time.time <= this.lastPlayerInputTime + 1f;
	}

	// Token: 0x0600010B RID: 267 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEngineStartFailed()
	{
	}

	// Token: 0x0600010C RID: 268 RVA: 0x0000767F File Offset: 0x0000587F
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && this.CurEngineState == VehicleEngineController<MiniCopter>.EngineState.Off)
		{
			this.lastEngineOnTime = UnityEngine.Time.time;
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x000076A4 File Offset: 0x000058A4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.engineController.CheckEngineState();
		this.engineController.TickFuel(this.fuelPerSec);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x000076CC File Offset: 0x000058CC
	public void UpdateNetwork()
	{
		global::BaseEntity.Flags flags = this.flags;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.leftWheel.isGrounded, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, this.rightWheel.isGrounded, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.frontWheel.isGrounded, false, false);
		if (base.HasDriver())
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (flags != this.flags)
		{
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00007747 File Offset: 0x00005947
	public void UpdateCOM()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00007760 File Offset: 0x00005960
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.miniCopter = Facepunch.Pool.Get<Minicopter>();
		info.msg.miniCopter.fuelStorageID = this.engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.miniCopter.fuelFraction = this.GetFuelFraction();
		info.msg.miniCopter.pitch = this.currentInputState.pitch;
		info.msg.miniCopter.roll = this.currentInputState.roll;
		info.msg.miniCopter.yaw = this.currentInputState.yaw;
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007810 File Offset: 0x00005A10
	public override void DismountAllPlayers()
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				global::BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00007890 File Offset: 0x00005A90
	protected override void DoPushAction(global::BasePlayer player)
	{
		Vector3 a = Vector3Ex.Direction2D(player.transform.position, base.transform.position);
		Vector3 a2 = player.eyes.BodyForward();
		a2.y = 0.25f;
		Vector3 position = base.transform.position + a * 2f;
		float d = this.rigidBody.mass * 2f;
		this.rigidBody.AddForceAtPosition(a2 * d, position, ForceMode.Impulse);
		this.rigidBody.AddForce(Vector3.up * 3f, ForceMode.Impulse);
		this.isPushing = true;
		base.Invoke(new Action(this.DisablePushing), 0.5f);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x0000794C File Offset: 0x00005B4C
	private void DisablePushing()
	{
		this.isPushing = false;
	}

	// Token: 0x06000114 RID: 276 RVA: 0x00007955 File Offset: 0x00005B55
	public float RemapValue(float toUse, float maxRemap)
	{
		return toUse * maxRemap;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x0000795C File Offset: 0x00005B5C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.miniCopter != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.miniCopter.fuelStorageID;
			this.cachedFuelFraction = info.msg.miniCopter.fuelFraction;
			this.cachedPitch = this.RemapValue(info.msg.miniCopter.pitch, 0.5f);
			this.cachedRoll = this.RemapValue(info.msg.miniCopter.roll, 0.2f);
			this.cachedYaw = this.RemapValue(info.msg.miniCopter.yaw, 0.35f);
		}
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00007A1E File Offset: 0x00005C1E
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && pusher.IsOnGround() && !pusher.isMounted;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool InheritedVelocityDirection()
	{
		return false;
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00007A40 File Offset: 0x00005C40
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MiniCopter.OnRpcMessage", 0))
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
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1851540757U, "RPC_OpenFuel", this, player, 6f))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00007C28 File Offset: 0x00005E28
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00007C32 File Offset: 0x00005E32
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}
}
