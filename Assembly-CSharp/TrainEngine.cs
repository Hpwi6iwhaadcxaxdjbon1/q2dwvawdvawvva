﻿using System;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x020000E4 RID: 228
public class TrainEngine : TrainCar, IEngineControllerUser, IEntity
{
	// Token: 0x04000C88 RID: 3208
	public const float HAZARD_CHECK_EVERY = 1f;

	// Token: 0x04000C89 RID: 3209
	public const float HAZARD_DIST_MAX = 325f;

	// Token: 0x04000C8A RID: 3210
	public const float HAZARD_DIST_MIN = 20f;

	// Token: 0x04000C8B RID: 3211
	public const float HAZARD_SPEED_MIN = 4.5f;

	// Token: 0x04000C8C RID: 3212
	private float buttonHoldTime;

	// Token: 0x04000C8D RID: 3213
	private static readonly global::TrainEngine.EngineSpeeds MaxThrottle = global::TrainEngine.EngineSpeeds.Fwd_Hi;

	// Token: 0x04000C8E RID: 3214
	private static readonly global::TrainEngine.EngineSpeeds MinThrottle = global::TrainEngine.EngineSpeeds.Rev_Hi;

	// Token: 0x04000C8F RID: 3215
	private EngineDamageOverTime engineDamage;

	// Token: 0x04000C90 RID: 3216
	private Vector3 engineLocalOffset;

	// Token: 0x04000C91 RID: 3217
	private int lastSentLinedUpToUnload = -1;

	// Token: 0x04000C92 RID: 3218
	[Header("Train Engine")]
	[SerializeField]
	private Transform leftHandLever;

	// Token: 0x04000C93 RID: 3219
	[SerializeField]
	private Transform rightHandLever;

	// Token: 0x04000C94 RID: 3220
	[SerializeField]
	private Transform leftHandGrip;

	// Token: 0x04000C95 RID: 3221
	[SerializeField]
	private Transform rightHandGrip;

	// Token: 0x04000C96 RID: 3222
	[SerializeField]
	private global::TrainEngine.LeverStyle leverStyle;

	// Token: 0x04000C97 RID: 3223
	[SerializeField]
	private Canvas monitorCanvas;

	// Token: 0x04000C98 RID: 3224
	[SerializeField]
	private RustText monitorText;

	// Token: 0x04000C99 RID: 3225
	[SerializeField]
	private LocomotiveExtraVisuals gauges;

	// Token: 0x04000C9A RID: 3226
	[SerializeField]
	private float engineForce = 50000f;

	// Token: 0x04000C9B RID: 3227
	[SerializeField]
	private float maxSpeed = 12f;

	// Token: 0x04000C9C RID: 3228
	[SerializeField]
	private float engineStartupTime = 1f;

	// Token: 0x04000C9D RID: 3229
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04000C9E RID: 3230
	[SerializeField]
	private float idleFuelPerSec = 0.05f;

	// Token: 0x04000C9F RID: 3231
	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	// Token: 0x04000CA0 RID: 3232
	[SerializeField]
	private ProtectionProperties driverProtection;

	// Token: 0x04000CA1 RID: 3233
	[SerializeField]
	private bool lootablesAreOnPlatform;

	// Token: 0x04000CA2 RID: 3234
	[SerializeField]
	private bool mustMountFromPlatform = true;

	// Token: 0x04000CA3 RID: 3235
	[SerializeField]
	private VehicleLight[] onLights;

	// Token: 0x04000CA4 RID: 3236
	[SerializeField]
	private VehicleLight[] headlights;

	// Token: 0x04000CA5 RID: 3237
	[SerializeField]
	private VehicleLight[] notMovingLights;

	// Token: 0x04000CA6 RID: 3238
	[SerializeField]
	private VehicleLight[] movingForwardLights;

	// Token: 0x04000CA7 RID: 3239
	[FormerlySerializedAs("movingBackwardsLights")]
	[SerializeField]
	private VehicleLight[] movingBackwardLights;

	// Token: 0x04000CA8 RID: 3240
	[SerializeField]
	private ParticleSystemContainer fxEngineOn;

	// Token: 0x04000CA9 RID: 3241
	[SerializeField]
	private ParticleSystemContainer fxLightDamage;

	// Token: 0x04000CAA RID: 3242
	[SerializeField]
	private ParticleSystemContainer fxMediumDamage;

	// Token: 0x04000CAB RID: 3243
	[SerializeField]
	private ParticleSystemContainer fxHeavyDamage;

	// Token: 0x04000CAC RID: 3244
	[SerializeField]
	private ParticleSystemContainer fxEngineTrouble;

	// Token: 0x04000CAD RID: 3245
	[SerializeField]
	private BoxCollider engineWorldCol;

	// Token: 0x04000CAE RID: 3246
	[SerializeField]
	private float engineDamageToSlow = 150f;

	// Token: 0x04000CAF RID: 3247
	[SerializeField]
	private float engineDamageTimeframe = 10f;

	// Token: 0x04000CB0 RID: 3248
	[SerializeField]
	private float engineSlowedTime = 10f;

	// Token: 0x04000CB1 RID: 3249
	[SerializeField]
	private float engineSlowedMaxVel = 4f;

	// Token: 0x04000CB2 RID: 3250
	[SerializeField]
	private ParticleSystemContainer[] sparks;

	// Token: 0x04000CB3 RID: 3251
	[FormerlySerializedAs("brakeSparkLights")]
	[SerializeField]
	private Light[] sparkLights;

	// Token: 0x04000CB4 RID: 3252
	[SerializeField]
	private TrainEngineAudio trainAudio;

	// Token: 0x04000CB5 RID: 3253
	public const global::BaseEntity.Flags Flag_HazardAhead = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000CB6 RID: 3254
	private const global::BaseEntity.Flags Flag_Horn = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000CB7 RID: 3255
	public const global::BaseEntity.Flags Flag_AltColor = global::BaseEntity.Flags.Reserved9;

	// Token: 0x04000CB8 RID: 3256
	public const global::BaseEntity.Flags Flag_EngineSlowed = global::BaseEntity.Flags.Reserved10;

	// Token: 0x04000CB9 RID: 3257
	private VehicleEngineController<global::TrainEngine> engineController;

	// Token: 0x06001417 RID: 5143 RVA: 0x0009F9D8 File Offset: 0x0009DBD8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainEngine.OnRpcMessage", 0))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06001418 RID: 5144 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool networkUpdateOnCompleteTrainChange
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0009FAFC File Offset: 0x0009DCFC
	public override void ServerInit()
	{
		base.ServerInit();
		this.engineDamage = new EngineDamageOverTime(this.engineDamageToSlow, this.engineDamageTimeframe, new Action(this.OnEngineTookHeavyDamage));
		this.engineLocalOffset = base.transform.InverseTransformPoint(this.engineWorldCol.transform.position + this.engineWorldCol.transform.rotation * this.engineWorldCol.center);
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x000070B5 File Offset: 0x000052B5
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0009FB78 File Offset: 0x0009DD78
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.engineController.CheckEngineState();
		if (this.engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(this.idleFuelPerSec, this.maxFuelPerSec, Mathf.Abs(this.GetThrottleFraction()));
			if (this.engineController.TickFuel(fuelPerSecond) > 0)
			{
				base.ClientRPC<int>(null, "SetFuelAmount", this.GetFuelAmount());
			}
			if (this.completeTrain != null && this.completeTrain.LinedUpToUnload != this.lastSentLinedUpToUnload)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
		}
		else if (this.LightsAreOn && !base.HasDriver())
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
		}
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0009FC24 File Offset: 0x0009DE24
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.trainEngine = Facepunch.Pool.Get<ProtoBuf.TrainEngine>();
		info.msg.trainEngine.throttleSetting = (int)this.CurThrottleSetting;
		info.msg.trainEngine.fuelStorageID = this.GetFuelSystem().fuelStorageInstance.uid;
		info.msg.trainEngine.fuelAmount = this.GetFuelAmount();
		info.msg.trainEngine.numConnectedCars = this.completeTrain.NumTrainCars;
		info.msg.trainEngine.linedUpToUnload = this.completeTrain.LinedUpToUnload;
		this.lastSentLinedUpToUnload = this.completeTrain.LinedUpToUnload;
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0009FCDB File Offset: 0x0009DEDB
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0009FCE8 File Offset: 0x0009DEE8
	public override void LightToggle(global::BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0009FD0C File Offset: 0x0009DF0C
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		global::TrainEngine.<>c__DisplayClass19_0 CS$<>8__locals1;
		CS$<>8__locals1.inputState = inputState;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.player = player;
		if (!base.IsDriver(CS$<>8__locals1.player))
		{
			return;
		}
		if (this.engineController.IsOff)
		{
			if ((CS$<>8__locals1.inputState.IsDown(BUTTON.FORWARD) && !CS$<>8__locals1.inputState.WasDown(BUTTON.FORWARD)) || (CS$<>8__locals1.inputState.IsDown(BUTTON.BACKWARD) && !CS$<>8__locals1.inputState.WasDown(BUTTON.BACKWARD)))
			{
				this.engineController.TryStartEngine(CS$<>8__locals1.player);
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		}
		else
		{
			if (!this.<PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON.FORWARD, new Action(this.IncreaseThrottle), ref CS$<>8__locals1))
			{
				this.<PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON.BACKWARD, new Action(this.DecreaseThrottle), ref CS$<>8__locals1);
			}
			base.SetFlag(global::BaseEntity.Flags.Reserved8, CS$<>8__locals1.inputState.IsDown(BUTTON.FIRE_PRIMARY), false, true);
		}
		if (CS$<>8__locals1.inputState.IsDown(BUTTON.LEFT))
		{
			this.SetTrackSelection(TrainTrackSpline.TrackSelection.Left);
			return;
		}
		if (CS$<>8__locals1.inputState.IsDown(BUTTON.RIGHT))
		{
			this.SetTrackSelection(TrainTrackSpline.TrackSelection.Right);
			return;
		}
		this.SetTrackSelection(TrainTrackSpline.TrackSelection.Default);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0009FE24 File Offset: 0x0009E024
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0009FE3C File Offset: 0x0009E03C
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.driverProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0009FE5C File Offset: 0x0009E05C
	public bool MeetsEngineRequirements()
	{
		return (base.HasDriver() || this.CurThrottleSetting != global::TrainEngine.EngineSpeeds.Zero) && (this.completeTrain.AnyPlayersOnTrain() || vehicle.trainskeeprunning);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEngineStartFailed()
	{
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0009FE85 File Offset: 0x0009E085
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (!this.CanMount(player))
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0009FE9C File Offset: 0x0009E09C
	protected override float GetThrottleForce()
	{
		if (this.IsDead() || base.IsDestroyed)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = this.engineController.IsOn ? this.GetThrottleFraction() : 0f;
		float num3 = this.maxSpeed * num2;
		float curTopSpeed = this.GetCurTopSpeed();
		num3 = Mathf.Clamp(num3, -curTopSpeed, curTopSpeed);
		float trackSpeed = base.GetTrackSpeed();
		if (num2 > 0f && trackSpeed < num3)
		{
			num += this.GetCurEngineForce();
		}
		else if (num2 < 0f && trackSpeed > num3)
		{
			num -= this.GetCurEngineForce();
		}
		return num;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0009FF31 File Offset: 0x0009E131
	public override bool HasThrottleInput()
	{
		return this.engineController.IsOn && this.CurThrottleSetting != global::TrainEngine.EngineSpeeds.Zero;
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0009FF50 File Offset: 0x0009E150
	public override void Hurt(HitInfo info)
	{
		if (this.engineDamage != null && Vector3.SqrMagnitude(this.engineLocalOffset - info.HitPositionLocal) < 2f)
		{
			this.engineDamage.TakeDamage(info.damageTypes.Total());
		}
		base.Hurt(info);
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0009FF9F File Offset: 0x0009E19F
	public void StopEngine()
	{
		this.engineController.StopEngine();
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0009FFAC File Offset: 0x0009E1AC
	protected override Vector3 GetExplosionPos()
	{
		return this.engineWorldCol.transform.position + this.engineWorldCol.center;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0009FFCE File Offset: 0x0009E1CE
	private void IncreaseThrottle()
	{
		if (this.CurThrottleSetting == global::TrainEngine.MaxThrottle)
		{
			return;
		}
		this.SetThrottle(this.CurThrottleSetting + 1);
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0009FFEC File Offset: 0x0009E1EC
	private void DecreaseThrottle()
	{
		if (this.CurThrottleSetting == global::TrainEngine.MinThrottle)
		{
			return;
		}
		this.SetThrottle(this.CurThrottleSetting - 1);
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x000A000A File Offset: 0x0009E20A
	private void SetZeroThrottle()
	{
		this.SetThrottle(global::TrainEngine.EngineSpeeds.Zero);
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x000A0014 File Offset: 0x0009E214
	protected override void ServerFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.ServerFlagsChanged(old, next);
		if (next.HasFlag(global::BaseEntity.Flags.On) && !old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
			base.InvokeRandomized(new Action(this.CheckForHazards), 0f, 1f, 0.1f);
			return;
		}
		if (!next.HasFlag(global::BaseEntity.Flags.On) && old.HasFlag(global::BaseEntity.Flags.On))
		{
			base.CancelInvoke(new Action(this.CheckForHazards));
			base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		}
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x000A00C8 File Offset: 0x0009E2C8
	private void CheckForHazards()
	{
		float trackSpeed = base.GetTrackSpeed();
		if (trackSpeed > 4.5f || trackSpeed < -4.5f)
		{
			float maxHazardDist = Mathf.Lerp(40f, 325f, Mathf.Abs(trackSpeed) * 0.05f);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, base.FrontTrackSection.HasValidHazardWithin(this, base.FrontWheelSplineDist, 20f, maxHazardDist, this.localTrackSelection, trackSpeed, base.RearTrackSection, null), false, true);
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x000A014A File Offset: 0x0009E34A
	private void OnEngineTookHeavyDamage()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved10, true, false, true);
		base.Invoke(new Action(this.ResetEngineToNormal), this.engineSlowedTime);
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0000420E File Offset: 0x0000240E
	private void ResetEngineToNormal()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x000A0174 File Offset: 0x0009E374
	private float GetCurTopSpeed()
	{
		float num = this.maxSpeed * this.GetEnginePowerMultiplier(0.5f);
		if (this.EngineIsSlowed)
		{
			num = Mathf.Clamp(num, -this.engineSlowedMaxVel, this.engineSlowedMaxVel);
		}
		return num;
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x000A01B1 File Offset: 0x0009E3B1
	private float GetCurEngineForce()
	{
		return this.engineForce * this.GetEnginePowerMultiplier(0.75f);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x000A01C8 File Offset: 0x0009E3C8
	[global::BaseEntity.RPC_Server]
	public void RPC_OpenFuel(global::BaseEntity.RPCMessage msg)
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
		this.GetFuelSystem().LootFuel(player);
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001434 RID: 5172 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06001435 RID: 5173 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool CloseToHazard
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06001436 RID: 5174 RVA: 0x0008F832 File Offset: 0x0008DA32
	public bool EngineIsSlowed
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved10);
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06001437 RID: 5175 RVA: 0x000A01FC File Offset: 0x0009E3FC
	// (set) Token: 0x06001438 RID: 5176 RVA: 0x000A0204 File Offset: 0x0009E404
	public global::TrainEngine.EngineSpeeds CurThrottleSetting { get; protected set; } = global::TrainEngine.EngineSpeeds.Zero;

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x06001439 RID: 5177 RVA: 0x0000441C File Offset: 0x0000261C
	public override TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Engine;
		}
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x000A0210 File Offset: 0x0009E410
	public override void InitShared()
	{
		base.InitShared();
		this.engineController = new VehicleEngineController<global::TrainEngine>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, null, global::BaseEntity.Flags.Reserved1);
		if (base.isServer)
		{
			bool b = SeedRandom.Range((uint)this.net.ID.Value, 0, 2) == 0;
			base.SetFlag(global::BaseEntity.Flags.Reserved9, b, false, true);
		}
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x000A027C File Offset: 0x0009E47C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.trainEngine != null)
		{
			this.engineController.FuelSystem.fuelStorageInstance.uid = info.msg.trainEngine.fuelStorageID;
			this.SetThrottle((global::TrainEngine.EngineSpeeds)info.msg.trainEngine.throttleSetting);
		}
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x000A02D8 File Offset: 0x0009E4D8
	public override bool CanBeLooted(global::BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		if (player.isMounted)
		{
			return false;
		}
		if (this.lootablesAreOnPlatform)
		{
			return base.PlayerIsOnPlatform(player);
		}
		return base.GetLocalVelocity().magnitude < 2f || base.PlayerIsOnPlatform(player);
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x000A0328 File Offset: 0x0009E528
	private float GetEnginePowerMultiplier(float minPercent)
	{
		if (base.healthFraction > 0.4f)
		{
			return 1f;
		}
		return Mathf.Lerp(minPercent, 1f, base.healthFraction / 0.4f);
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000A0354 File Offset: 0x0009E554
	public float GetThrottleFraction()
	{
		switch (this.CurThrottleSetting)
		{
		case global::TrainEngine.EngineSpeeds.Rev_Hi:
			return -1f;
		case global::TrainEngine.EngineSpeeds.Rev_Med:
			return -0.5f;
		case global::TrainEngine.EngineSpeeds.Rev_Lo:
			return -0.2f;
		case global::TrainEngine.EngineSpeeds.Zero:
			return 0f;
		case global::TrainEngine.EngineSpeeds.Fwd_Lo:
			return 0.2f;
		case global::TrainEngine.EngineSpeeds.Fwd_Med:
			return 0.5f;
		case global::TrainEngine.EngineSpeeds.Fwd_Hi:
			return 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x000A03BC File Offset: 0x0009E5BC
	public bool IsNearDesiredSpeed(float leeway)
	{
		float num = Vector3.Dot(base.transform.forward, base.GetLocalVelocity());
		float num2 = this.maxSpeed * this.GetThrottleFraction();
		if (num2 < 0f)
		{
			return num - leeway <= num2;
		}
		return num + leeway >= num2;
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x000A0409 File Offset: 0x0009E609
	protected override void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		base.SetTrackSelection(trackSelection);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x000A0412 File Offset: 0x0009E612
	private void SetThrottle(global::TrainEngine.EngineSpeeds throttle)
	{
		if (this.CurThrottleSetting == throttle)
		{
			return;
		}
		this.CurThrottleSetting = throttle;
		if (base.isServer)
		{
			base.ClientRPC<sbyte>(null, "SetThrottle", (sbyte)throttle);
		}
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x000A043B File Offset: 0x0009E63B
	private int GetFuelAmount()
	{
		if (base.isServer)
		{
			return this.engineController.FuelSystem.GetFuelAmount();
		}
		return 0;
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x000A0457 File Offset: 0x0009E657
	private bool CanMount(global::BasePlayer player)
	{
		return !this.mustMountFromPlatform || base.PlayerIsOnPlatform(player);
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x00007C28 File Offset: 0x00005E28
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x00007C32 File Offset: 0x00005E32
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x000A0508 File Offset: 0x0009E708
	[CompilerGenerated]
	private bool <PlayerServerInput>g__ProcessThrottleInput|19_0(BUTTON button, Action action, ref global::TrainEngine.<>c__DisplayClass19_0 A_3)
	{
		if (A_3.inputState.IsDown(button))
		{
			if (!A_3.inputState.WasDown(button))
			{
				action();
				this.buttonHoldTime = 0f;
			}
			else
			{
				this.buttonHoldTime += A_3.player.clientTickInterval;
				if (this.buttonHoldTime > 0.55f)
				{
					action();
					this.buttonHoldTime = 0.4f;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x02000C0F RID: 3087
	private enum LeverStyle
	{
		// Token: 0x040041D2 RID: 16850
		WorkCart,
		// Token: 0x040041D3 RID: 16851
		Locomotive
	}

	// Token: 0x02000C10 RID: 3088
	public enum EngineSpeeds
	{
		// Token: 0x040041D5 RID: 16853
		Rev_Hi,
		// Token: 0x040041D6 RID: 16854
		Rev_Med,
		// Token: 0x040041D7 RID: 16855
		Rev_Lo,
		// Token: 0x040041D8 RID: 16856
		Zero,
		// Token: 0x040041D9 RID: 16857
		Fwd_Lo,
		// Token: 0x040041DA RID: 16858
		Fwd_Med,
		// Token: 0x040041DB RID: 16859
		Fwd_Hi
	}
}
