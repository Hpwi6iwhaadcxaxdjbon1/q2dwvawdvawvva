﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000035 RID: 53
public class AutoTurret : ContainerIOEntity, IRemoteControllable
{
	// Token: 0x040001A6 RID: 422
	public GameObjectRef gun_fire_effect;

	// Token: 0x040001A7 RID: 423
	public GameObjectRef bulletEffect;

	// Token: 0x040001A8 RID: 424
	public float bulletSpeed = 200f;

	// Token: 0x040001A9 RID: 425
	public AmbienceEmitter ambienceEmitter;

	// Token: 0x040001AA RID: 426
	public GameObject assignDialog;

	// Token: 0x040001AB RID: 427
	public LaserBeam laserBeam;

	// Token: 0x040001AC RID: 428
	public static global::AutoTurret.UpdateAutoTurretScanQueue updateAutoTurretScanQueue = new global::AutoTurret.UpdateAutoTurretScanQueue();

	// Token: 0x040001AD RID: 429
	[Header("RC")]
	public float rcTurnSensitivity = 4f;

	// Token: 0x040001AE RID: 430
	public Transform RCEyes;

	// Token: 0x040001AF RID: 431
	public GameObjectRef IDPanelPrefab;

	// Token: 0x040001B0 RID: 432
	public RemoteControllableControls rcControls;

	// Token: 0x040001B1 RID: 433
	public string rcIdentifier = "";

	// Token: 0x040001B4 RID: 436
	public TargetTrigger targetTrigger;

	// Token: 0x040001B5 RID: 437
	public Transform socketTransform;

	// Token: 0x040001B6 RID: 438
	private float nextShotTime;

	// Token: 0x040001B7 RID: 439
	private float lastShotTime;

	// Token: 0x040001B8 RID: 440
	private float nextVisCheck;

	// Token: 0x040001B9 RID: 441
	private float lastTargetSeenTime;

	// Token: 0x040001BA RID: 442
	private bool targetVisible = true;

	// Token: 0x040001BB RID: 443
	private bool booting;

	// Token: 0x040001BC RID: 444
	private float nextIdleAimTime;

	// Token: 0x040001BD RID: 445
	private Vector3 targetAimDir = Vector3.forward;

	// Token: 0x040001BE RID: 446
	private const float bulletDamage = 15f;

	// Token: 0x040001BF RID: 447
	private RealTimeSinceEx timeSinceLastServerTick;

	// Token: 0x040001C0 RID: 448
	private float nextForcedAimTime;

	// Token: 0x040001C1 RID: 449
	private Vector3 lastSentAimDir = Vector3.zero;

	// Token: 0x040001C2 RID: 450
	private static float[] visibilityOffsets = new float[]
	{
		0f,
		0.15f,
		-0.15f
	};

	// Token: 0x040001C3 RID: 451
	private int peekIndex;

	// Token: 0x040001C4 RID: 452
	[NonSerialized]
	private int numConsecutiveMisses;

	// Token: 0x040001C5 RID: 453
	[NonSerialized]
	private int totalAmmo;

	// Token: 0x040001C6 RID: 454
	private float nextAmmoCheckTime;

	// Token: 0x040001C7 RID: 455
	private bool totalAmmoDirty = true;

	// Token: 0x040001C8 RID: 456
	private float currentAmmoGravity;

	// Token: 0x040001C9 RID: 457
	private float currentAmmoVelocity;

	// Token: 0x040001CA RID: 458
	private global::HeldEntity AttachedWeapon;

	// Token: 0x040001CB RID: 459
	public float attachedWeaponZOffsetScale = -0.5f;

	// Token: 0x040001CC RID: 460
	public BaseCombatEntity target;

	// Token: 0x040001CD RID: 461
	public Transform eyePos;

	// Token: 0x040001CE RID: 462
	public Transform muzzlePos;

	// Token: 0x040001CF RID: 463
	public Vector3 aimDir;

	// Token: 0x040001D0 RID: 464
	public Transform gun_yaw;

	// Token: 0x040001D1 RID: 465
	public Transform gun_pitch;

	// Token: 0x040001D2 RID: 466
	public float sightRange = 30f;

	// Token: 0x040001D3 RID: 467
	public SoundDefinition turnLoopDef;

	// Token: 0x040001D4 RID: 468
	public SoundDefinition movementChangeDef;

	// Token: 0x040001D5 RID: 469
	public SoundDefinition ambientLoopDef;

	// Token: 0x040001D6 RID: 470
	public SoundDefinition focusCameraDef;

	// Token: 0x040001D7 RID: 471
	public float focusSoundFreqMin = 2.5f;

	// Token: 0x040001D8 RID: 472
	public float focusSoundFreqMax = 7f;

	// Token: 0x040001D9 RID: 473
	public GameObjectRef peacekeeperToggleSound;

	// Token: 0x040001DA RID: 474
	public GameObjectRef onlineSound;

	// Token: 0x040001DB RID: 475
	public GameObjectRef offlineSound;

	// Token: 0x040001DC RID: 476
	public GameObjectRef targetAcquiredEffect;

	// Token: 0x040001DD RID: 477
	public GameObjectRef targetLostEffect;

	// Token: 0x040001DE RID: 478
	public GameObjectRef reloadEffect;

	// Token: 0x040001DF RID: 479
	public float aimCone;

	// Token: 0x040001E0 RID: 480
	public const global::BaseEntity.Flags Flag_Equipped = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040001E1 RID: 481
	public const global::BaseEntity.Flags Flag_MaxAuths = global::BaseEntity.Flags.Reserved4;

	// Token: 0x040001E2 RID: 482
	[NonSerialized]
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x0600016B RID: 363 RVA: 0x00022690 File Offset: 0x00020890
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AutoTurret.OnRpcMessage", 0))
		{
			if (rpc == 1092560690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddSelfAuthorize ");
				}
				using (TimeWarning.New("AddSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1092560690U, "AddSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddSelfAuthorize(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 3057055788U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AssignToFriend ");
				}
				using (TimeWarning.New("AssignToFriend", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3057055788U, "AssignToFriend", this, player, 3f))
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
							this.AssignToFriend(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				return true;
			}
			if (rpc == 253307592U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearList ");
				}
				using (TimeWarning.New("ClearList", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(253307592U, "ClearList", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClearList(rpc3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in ClearList");
					}
				}
				return true;
			}
			if (rpc == 1500257773U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - FlipAim ");
				}
				using (TimeWarning.New("FlipAim", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1500257773U, "FlipAim", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.FlipAim(rpc4);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in FlipAim");
					}
				}
				return true;
			}
			if (rpc == 3617985969U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RemoveSelfAuthorize ");
				}
				using (TimeWarning.New("RemoveSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3617985969U, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RemoveSelfAuthorize(rpc5);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 1770263114U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_AttackAll ");
				}
				using (TimeWarning.New("SERVER_AttackAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1770263114U, "SERVER_AttackAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_AttackAll(rpc6);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in SERVER_AttackAll");
					}
				}
				return true;
			}
			if (rpc == 3265538831U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_Peacekeeper ");
				}
				using (TimeWarning.New("SERVER_Peacekeeper", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3265538831U, "SERVER_Peacekeeper", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_Peacekeeper(rpc7);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in SERVER_Peacekeeper");
					}
				}
				return true;
			}
			if (rpc == 1053317251U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetID ");
				}
				using (TimeWarning.New("Server_SetID", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1053317251U, "Server_SetID", this, player, 3f))
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
							this.Server_SetID(msg3);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in Server_SetID");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600016C RID: 364 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool CanPing
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool PeacekeeperMode()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x000231C1 File Offset: 0x000213C1
	public Transform GetEyes()
	{
		return this.RCEyes;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetFovScale()
	{
		return 1f;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity GetEnt()
	{
		return this;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x000231C9 File Offset: 0x000213C9
	public virtual bool CanControl(ulong playerID)
	{
		return !this.booting && this.IsPowered() && !this.PeacekeeperMode();
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000172 RID: 370 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool RequiresMouse
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000173 RID: 371 RVA: 0x000231E8 File Offset: 0x000213E8
	public float MaxRange
	{
		get
		{
			return 10000f;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000174 RID: 372 RVA: 0x000231EF File Offset: 0x000213EF
	public RemoteControllableControls RequiredControls
	{
		get
		{
			return this.rcControls;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000175 RID: 373 RVA: 0x000231F7 File Offset: 0x000213F7
	// (set) Token: 0x06000176 RID: 374 RVA: 0x000231FF File Offset: 0x000213FF
	public int ViewerCount { get; private set; }

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000177 RID: 375 RVA: 0x00023208 File Offset: 0x00021408
	// (set) Token: 0x06000178 RID: 376 RVA: 0x00023210 File Offset: 0x00021410
	public CameraViewerId? ControllingViewerId { get; private set; }

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000179 RID: 377 RVA: 0x0002321C File Offset: 0x0002141C
	public bool IsBeingControlled
	{
		get
		{
			return this.ViewerCount > 0 && this.ControllingViewerId != null;
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00023244 File Offset: 0x00021444
	public bool InitializeControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount + 1;
		if (this.ControllingViewerId == null)
		{
			this.ControllingViewerId = new CameraViewerId?(viewerID);
			this.SetTarget(null);
			this.SendAimDirImmediate();
			return true;
		}
		return false;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00023290 File Offset: 0x00021490
	public void StopControl(CameraViewerId viewerID)
	{
		int viewerCount = this.ViewerCount;
		this.ViewerCount = viewerCount - 1;
		CameraViewerId? controllingViewerId = this.ControllingViewerId;
		if (controllingViewerId != null && (controllingViewerId == null || controllingViewerId.GetValueOrDefault() == viewerID))
		{
			this.ControllingViewerId = null;
		}
	}

	// Token: 0x0600017C RID: 380 RVA: 0x000232EC File Offset: 0x000214EC
	public void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (viewerID != this.ControllingViewerId)
		{
			return;
		}
		this.UpdateManualAim(inputState);
		if (UnityEngine.Time.time < this.nextShotTime)
		{
			return;
		}
		if (inputState.WasJustPressed(BUTTON.RELOAD))
		{
			this.Reload();
			return;
		}
		if (this.EnsureReloaded(true))
		{
			return;
		}
		if (inputState.IsDown(BUTTON.FIRE_PRIMARY))
		{
			global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
			if (attachedWeapon)
			{
				if (attachedWeapon.primaryMagazine.contents > 0)
				{
					this.FireAttachedGun(Vector3.zero, this.aimCone, null, null);
					float num = attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay;
					num = attachedWeapon.ScaleRepeatDelay(num);
					this.nextShotTime = UnityEngine.Time.time + num;
					return;
				}
				this.nextShotTime = UnityEngine.Time.time + 5f;
				return;
			}
			else
			{
				if (this.HasGenericFireable())
				{
					this.AttachedWeapon.ServerUse();
					this.nextShotTime = UnityEngine.Time.time + 0.115f;
					return;
				}
				this.nextShotTime = UnityEngine.Time.time + 1f;
			}
		}
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00023410 File Offset: 0x00021610
	private bool UpdateManualAim(InputState inputState)
	{
		float x = -inputState.current.mouseDelta.y * this.rcTurnSensitivity;
		float y = inputState.current.mouseDelta.x * this.rcTurnSensitivity;
		Vector3 vector = Quaternion.LookRotation(this.aimDir, base.transform.up).eulerAngles + new Vector3(x, y, 0f);
		if (vector.x >= 0f && vector.x <= 135f)
		{
			vector.x = Mathf.Clamp(vector.x, 0f, 45f);
		}
		if (vector.x >= 225f && vector.x <= 360f)
		{
			vector.x = Mathf.Clamp(vector.x, 285f, 360f);
		}
		Vector3 vector2 = Quaternion.Euler(vector) * Vector3.forward;
		bool result = !Mathf.Approximately(this.aimDir.x, vector2.x) || !Mathf.Approximately(this.aimDir.y, vector2.y) || !Mathf.Approximately(this.aimDir.z, vector2.z);
		this.aimDir = vector2;
		return result;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0002354F File Offset: 0x0002174F
	public override void InitShared()
	{
		base.InitShared();
		this.RCSetup();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0002355D File Offset: 0x0002175D
	public override void DestroyShared()
	{
		this.RCShutdown();
		base.DestroyShared();
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0002356B File Offset: 0x0002176B
	public void RCSetup()
	{
		if (base.isServer)
		{
			RemoteControlEntity.InstallControllable(this);
		}
	}

	// Token: 0x06000181 RID: 385 RVA: 0x0002357C File Offset: 0x0002177C
	public void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00023590 File Offset: 0x00021790
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Server_SetID(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !this.CanChangeID(msg.player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (!string.IsNullOrEmpty(text) && !global::ComputerStation.IsValidIdentifier(text))
		{
			return;
		}
		string text2 = msg.read.String(256);
		if (!global::ComputerStation.IsValidIdentifier(text2))
		{
			return;
		}
		if (text == this.GetIdentifier())
		{
			Debug.Log("SetID success!");
			this.UpdateIdentifier(text2, false);
		}
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00023616 File Offset: 0x00021816
	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = this.rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				this.rcIdentifier = newID;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0002363D File Offset: 0x0002183D
	public string GetIdentifier()
	{
		return this.rcIdentifier;
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00023645 File Offset: 0x00021845
	protected virtual bool CanChangeID(global::BasePlayer player)
	{
		return this.CanChangeSettings(player);
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0002364E File Offset: 0x0002184E
	public override int ConsumptionAmount()
	{
		return 10;
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00023652 File Offset: 0x00021852
	public void SetOnline()
	{
		this.SetIsOnline(true);
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0002365C File Offset: 0x0002185C
	public void SetIsOnline(bool online)
	{
		if (online == base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, online, false, true);
		this.booting = false;
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon != null)
		{
			attachedWeapon.SetLightsOn(online);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (this.IsOffline())
		{
			this.SetTarget(null);
			this.isLootable = true;
			return;
		}
		this.isLootable = false;
	}

	// Token: 0x06000189 RID: 393 RVA: 0x000236BC File Offset: 0x000218BC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int result = Mathf.Min(1, this.GetCurrentEnergy());
		if (outputSlot == 0)
		{
			if (!this.HasTarget())
			{
				return 0;
			}
			return result;
		}
		else if (outputSlot == 1)
		{
			if (this.totalAmmo > 50)
			{
				return 0;
			}
			return result;
		}
		else
		{
			if (outputSlot != 2)
			{
				return 0;
			}
			if (this.totalAmmo != 0)
			{
				return 0;
			}
			return result;
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00023708 File Offset: 0x00021908
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (this.IsPowered() && !base.IsOn())
		{
			this.InitiateStartup();
			return;
		}
		if ((!this.IsPowered() && base.IsOn()) || this.booting)
		{
			this.InitiateShutdown();
		}
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00023748 File Offset: 0x00021948
	public void InitiateShutdown()
	{
		if (this.IsOffline() && !this.booting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.SetOnline));
		this.booting = false;
		Effect.server.Run(this.offlineSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		this.SetIsOnline(false);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x000237A4 File Offset: 0x000219A4
	public void InitiateStartup()
	{
		if (this.IsOnline() || this.booting)
		{
			return;
		}
		Effect.server.Run(this.onlineSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.Invoke(new Action(this.SetOnline), 2f);
		this.booting = true;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x000237FE File Offset: 0x000219FE
	public void SetPeacekeepermode(bool isOn)
	{
		if (this.PeacekeeperMode() == isOn)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, isOn, false, true);
		Effect.server.Run(this.peacekeeperToggleSound.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00023838 File Offset: 0x00021A38
	public bool IsValidWeapon(global::Item item)
	{
		ItemDefinition info = item.info;
		if (item.isBroken)
		{
			return false;
		}
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return false;
		}
		global::HeldEntity component2 = component.entityPrefab.Get().GetComponent<global::HeldEntity>();
		return !(component2 == null) && component2.IsUsableByTurret;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00023890 File Offset: 0x00021A90
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		global::Item slot = base.inventory.GetSlot(0);
		return (this.IsValidWeapon(item) && targetSlot == 0) || (item.info.category == ItemCategory.Ammunition && slot != null && this.GetAttachedWeapon() && targetSlot != 0);
	}

	// Token: 0x06000190 RID: 400 RVA: 0x000238E0 File Offset: 0x00021AE0
	public bool AtMaxAuthCapacity()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000191 RID: 401 RVA: 0x000238F0 File Offset: 0x00021AF0
	public void UpdateMaxAuthCapacity()
	{
		if (this.authorizedPlayers.Count >= 200)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, true, false, true);
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		bool b = activeGameMode != null && activeGameMode.limitTeamAuths && this.authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize();
		base.SetFlag(global::BaseEntity.Flags.Reserved4, b, false, true);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00023960 File Offset: 0x00021B60
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void FlipAim(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsOnline() || !this.IsAuthed(rpc.player) || this.booting)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000193 RID: 403 RVA: 0x000239BE File Offset: 0x00021BBE
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		this.AddSelfAuthorize(rpc.player);
	}

	// Token: 0x06000194 RID: 404 RVA: 0x000239CC File Offset: 0x00021BCC
	private void AddSelfAuthorize(global::BasePlayer player)
	{
		if (this.IsOnline() || !player.CanBuild())
		{
			return;
		}
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = player.userID;
		playerNameID.username = player.displayName;
		this.authorizedPlayers.Add(playerNameID);
		Analytics.Azure.OnEntityAuthChanged(this, player, from x in this.authorizedPlayers
		select x.userid, "added", player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00023AA4 File Offset: 0x00021CA4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (this.booting || this.IsOnline() || !this.IsAuthed(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		Analytics.Azure.OnEntityAuthChanged(this, rpc.player, from x in this.authorizedPlayers
		select x.userid, "removed", rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00023B58 File Offset: 0x00021D58
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void ClearList(global::BaseEntity.RPCMessage rpc)
	{
		if (this.booting || this.IsOnline() || !this.IsAuthed(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.Clear();
		Analytics.Azure.OnEntityAuthChanged(this, rpc.player, from x in this.authorizedPlayers
		select x.userid, "clear", rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00023BE4 File Offset: 0x00021DE4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void AssignToFriend(global::BaseEntity.RPCMessage msg)
	{
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanChangeSettings(msg.player))
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num == 0UL || this.IsAuthed(num))
		{
			return;
		}
		string username = global::BasePlayer.SanitizePlayerNameString(msg.read.String(256), num);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = num;
		playerNameID.username = username;
		Analytics.Azure.OnEntityAuthChanged(this, msg.player, from x in this.authorizedPlayers
		select x.userid, "added", num);
		this.authorizedPlayers.Add(playerNameID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00023CBF File Offset: 0x00021EBF
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void SERVER_Peacekeeper(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.SetPeacekeepermode(true);
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00023CD6 File Offset: 0x00021ED6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void SERVER_AttackAll(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.SetPeacekeepermode(false);
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float TargetScanRate()
	{
		return 1f;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00023CF0 File Offset: 0x00021EF0
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		this.timeSinceLastServerTick = 0.0;
		base.InvokeRepeating(new Action(this.ServerTick), UnityEngine.Random.Range(0f, 1f), 0.015f);
		base.InvokeRandomized(new Action(this.SendAimDir), UnityEngine.Random.Range(0f, 1f), 0.2f, 0.05f);
		base.InvokeRandomized(new Action(this.ScheduleForTargetScan), UnityEngine.Random.Range(0f, 1f), this.TargetScanRate(), 0.2f);
		this.targetTrigger.GetComponent<SphereCollider>().radius = this.sightRange;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00023DD1 File Offset: 0x00021FD1
	public void SendAimDir()
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.nextForcedAimTime || this.HasTarget() || Vector3.Angle(this.lastSentAimDir, this.aimDir) > 0.03f)
		{
			this.SendAimDirImmediate();
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00023E06 File Offset: 0x00022006
	public void SendAimDirImmediate()
	{
		this.lastSentAimDir = this.aimDir;
		base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
		this.nextForcedAimTime = UnityEngine.Time.realtimeSinceStartup + 2f;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00023E38 File Offset: 0x00022038
	public void SetTarget(BaseCombatEntity targ)
	{
		if (targ != this.target)
		{
			Effect.server.Run((targ == null) ? this.targetLostEffect.resourcePath : this.targetAcquiredEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			base.MarkDirtyForceUpdateOutputs();
			this.nextShotTime += 0.1f;
		}
		this.target = targ;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CheckPeekers()
	{
		return true;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00023EAC File Offset: 0x000220AC
	public bool ObjectVisible(BaseCombatEntity obj)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		Vector3 position = this.eyePos.transform.position;
		if (GamePhysics.CheckSphere(position, 0.1f, 2097152, QueryTriggerInteraction.UseGlobal))
		{
			return false;
		}
		Vector3 a = this.AimOffset(obj);
		float num = Vector3.Distance(a, position);
		Vector3 a2 = Vector3.Cross((a - position).normalized, Vector3.up);
		int num2 = 0;
		while ((float)num2 < (this.CheckPeekers() ? 3f : 1f))
		{
			Vector3 normalized = (a + a2 * global::AutoTurret.visibilityOffsets[num2] - position).normalized;
			list.Clear();
			GamePhysics.TraceAll(new Ray(position, normalized), 0f, list, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null);
			for (int i = 0; i < list.Count; i++)
			{
				global::BaseEntity entity = list[i].GetEntity();
				if ((!(entity != null) || !entity.isClient) && (!(entity != null) || !(entity.ToPlayer() != null) || entity.EqualNetID(obj)) && (!(entity != null) || !entity.EqualNetID(this)))
				{
					if (entity != null && (entity == obj || entity.EqualNetID(obj)))
					{
						Facepunch.Pool.FreeList<RaycastHit>(ref list);
						this.peekIndex = num2;
						return true;
					}
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
			num2++;
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00024048 File Offset: 0x00022248
	public virtual void FireAttachedGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon == null)
		{
			return;
		}
		if (this.IsOffline())
		{
			return;
		}
		attachedWeapon.ServerUse(1f, this.IsBeingControlled ? this.RCEyes : this.gun_pitch);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00024090 File Offset: 0x00022290
	public virtual void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		if (this.IsOffline())
		{
			return;
		}
		if (muzzleToUse == null)
		{
			muzzleToUse = this.muzzlePos;
		}
		Vector3 vector = this.GetCenterMuzzle().transform.position - this.GetCenterMuzzle().forward * 0.25f;
		Vector3 vector2 = this.GetCenterMuzzle().transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, vector2, true);
		targetPos = vector + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0f, list, 300f, 1219701521, QueryTriggerInteraction.UseGlobal, null);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			global::BaseEntity entity = hit.GetEntity();
			if ((!(entity != null) || (!(entity == this) && !entity.EqualNetID(this))) && (!this.PeacekeeperMode() || !(target != null) || !(entity != null) || !(entity.GetComponent<global::BasePlayer>() != null) || entity.EqualNetID(target)))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					this.ApplyDamage(baseCombatEntity, hit.point, modifiedAimConeDirection);
					if (baseCombatEntity.EqualNetID(target))
					{
						flag = true;
					}
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					targetPos = hit.point;
					vector2 = (targetPos - vector).normalized;
					break;
				}
			}
		}
		int num = 2;
		if (!flag)
		{
			this.numConsecutiveMisses++;
		}
		else
		{
			this.numConsecutiveMisses = 0;
		}
		if (target != null && this.targetVisible && this.numConsecutiveMisses > num)
		{
			this.ApplyDamage(target, target.transform.position - vector2 * 0.25f, vector2);
			this.numConsecutiveMisses = 0;
		}
		base.ClientRPC<uint, Vector3>(null, "CLIENT_FireGun", StringPool.Get(muzzleToUse.gameObject.name), targetPos);
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x000242A8 File Offset: 0x000224A8
	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		float num = 15f * UnityEngine.Random.Range(0.9f, 1.1f);
		if (entity is global::BasePlayer && entity != this.target)
		{
			num *= 0.5f;
		}
		if (this.PeacekeeperMode() && entity == this.target)
		{
			this.target.MarkHostileFor(300f);
		}
		HitInfo info = new HitInfo(this, entity, DamageType.Bullet, num, point);
		entity.OnAttacked(info);
		if (entity is global::BasePlayer || entity is BaseNpc)
		{
			Effect.server.ImpactEffect(new HitInfo
			{
				HitPositionWorld = point,
				HitNormalWorld = -normal,
				HitMaterial = StringPool.Get("Flesh")
			});
		}
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00024360 File Offset: 0x00022560
	public void IdleTick(float dt)
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.nextIdleAimTime)
		{
			this.nextIdleAimTime = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(4f, 5f);
			Quaternion quaternion = Quaternion.LookRotation(base.transform.forward, Vector3.up);
			quaternion *= Quaternion.AngleAxis(UnityEngine.Random.Range(-45f, 45f), Vector3.up);
			this.targetAimDir = quaternion * Vector3.forward;
		}
		if (!this.HasTarget())
		{
			this.aimDir = Mathx.Lerp(this.aimDir, this.targetAimDir, 2f, dt);
		}
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00024404 File Offset: 0x00022604
	public virtual bool HasClipAmmo()
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		return !(attachedWeapon == null) && attachedWeapon.primaryMagazine.contents > 0;
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00024431 File Offset: 0x00022631
	public virtual bool HasReserveAmmo()
	{
		return this.totalAmmo > 0;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0002443C File Offset: 0x0002263C
	public int GetTotalAmmo()
	{
		int num = 0;
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon == null)
		{
			return num;
		}
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		base.inventory.FindAmmo(list, attachedWeapon.primaryMagazine.definition.ammoTypes);
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].amount;
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x000244A8 File Offset: 0x000226A8
	public AmmoTypes GetValidAmmoTypes()
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon == null)
		{
			return AmmoTypes.RIFLE_556MM;
		}
		return attachedWeapon.primaryMagazine.definition.ammoTypes;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000244D8 File Offset: 0x000226D8
	public ItemDefinition GetDesiredAmmo()
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon == null)
		{
			return null;
		}
		return attachedWeapon.primaryMagazine.ammoType;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00024504 File Offset: 0x00022704
	public void Reload()
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon == null)
		{
			return;
		}
		this.nextShotTime = Mathf.Max(this.nextShotTime, UnityEngine.Time.time + Mathf.Min(attachedWeapon.GetReloadDuration() * 0.5f, 2f));
		AmmoTypes ammoTypes = attachedWeapon.primaryMagazine.definition.ammoTypes;
		if (attachedWeapon.primaryMagazine.contents > 0)
		{
			bool flag;
			if (base.inventory.capacity > base.inventory.itemList.Count)
			{
				flag = true;
			}
			else
			{
				int num = 0;
				foreach (global::Item item in base.inventory.itemList)
				{
					if (item.info == attachedWeapon.primaryMagazine.ammoType)
					{
						num += item.MaxStackable() - item.amount;
					}
				}
				flag = (num >= attachedWeapon.primaryMagazine.contents);
			}
			if (!flag)
			{
				return;
			}
			base.inventory.AddItem(attachedWeapon.primaryMagazine.ammoType, attachedWeapon.primaryMagazine.contents, 0UL, global::ItemContainer.LimitStack.Existing);
			attachedWeapon.primaryMagazine.contents = 0;
		}
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		base.inventory.FindAmmo(list, ammoTypes);
		if (list.Count > 0)
		{
			Effect.server.Run(this.reloadEffect.resourcePath, this, StringPool.Get("WeaponAttachmentPoint"), Vector3.zero, Vector3.zero, null, false);
			this.totalAmmoDirty = true;
			attachedWeapon.primaryMagazine.ammoType = list[0].info;
			int num2 = 0;
			while (attachedWeapon.primaryMagazine.contents < attachedWeapon.primaryMagazine.capacity && num2 < list.Count)
			{
				if (list[num2].info == attachedWeapon.primaryMagazine.ammoType)
				{
					int num3 = attachedWeapon.primaryMagazine.capacity - attachedWeapon.primaryMagazine.contents;
					num3 = Mathf.Min(list[num2].amount, num3);
					list[num2].UseItem(num3);
					attachedWeapon.primaryMagazine.contents += num3;
				}
				num2++;
			}
		}
		ItemDefinition ammoType = attachedWeapon.primaryMagazine.ammoType;
		if (ammoType)
		{
			ItemModProjectile component = ammoType.GetComponent<ItemModProjectile>();
			GameObject gameObject = component.projectileObject.Get();
			if (gameObject)
			{
				if (gameObject.GetComponent<Projectile>())
				{
					this.currentAmmoGravity = 0f;
					this.currentAmmoVelocity = component.GetMaxVelocity();
				}
				else
				{
					ServerProjectile component2 = gameObject.GetComponent<ServerProjectile>();
					if (component2)
					{
						this.currentAmmoGravity = component2.gravityModifier;
						this.currentAmmoVelocity = component2.speed;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		attachedWeapon.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x000247F4 File Offset: 0x000229F4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.totalAmmoDirty = true;
		this.Reload();
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00024809 File Offset: 0x00022A09
	public void UpdateTotalAmmo()
	{
		int num = this.totalAmmo;
		this.totalAmmo = this.GetTotalAmmo();
		if (num != this.totalAmmo)
		{
			base.MarkDirtyForceUpdateOutputs();
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0002482C File Offset: 0x00022A2C
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (item.info.GetComponent<ItemModEntity>())
		{
			if (base.IsInvoking(new Action(this.UpdateAttachedWeapon)))
			{
				this.UpdateAttachedWeapon();
			}
			base.Invoke(new Action(this.UpdateAttachedWeapon), 0.5f);
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00024884 File Offset: 0x00022A84
	public bool EnsureReloaded(bool onlyReloadIfEmpty = true)
	{
		bool flag = this.HasReserveAmmo();
		if (onlyReloadIfEmpty)
		{
			if (flag && !this.HasClipAmmo())
			{
				this.Reload();
				return true;
			}
		}
		else if (flag)
		{
			this.Reload();
			return true;
		}
		return false;
	}

	// Token: 0x060001AF RID: 431 RVA: 0x000248BA File Offset: 0x00022ABA
	public global::BaseProjectile GetAttachedWeapon()
	{
		return this.AttachedWeapon as global::BaseProjectile;
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool HasFallbackWeapon()
	{
		return false;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x000248C7 File Offset: 0x00022AC7
	private bool HasGenericFireable()
	{
		return this.AttachedWeapon != null && this.AttachedWeapon.IsInstrument();
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x000248E4 File Offset: 0x00022AE4
	public void UpdateAttachedWeapon()
	{
		global::Item slot = base.inventory.GetSlot(0);
		global::HeldEntity heldEntity = null;
		if (slot != null && (slot.info.category == ItemCategory.Weapon || slot.info.category == ItemCategory.Fun))
		{
			global::BaseEntity heldEntity2 = slot.GetHeldEntity();
			if (heldEntity2 != null)
			{
				global::HeldEntity component = heldEntity2.GetComponent<global::HeldEntity>();
				if (component != null && component.IsUsableByTurret)
				{
					heldEntity = component;
				}
			}
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, heldEntity != null, false, true);
		if (heldEntity == null)
		{
			if (this.GetAttachedWeapon())
			{
				this.GetAttachedWeapon().SetGenericVisible(false);
				this.GetAttachedWeapon().SetLightsOn(false);
			}
			this.AttachedWeapon = null;
			return;
		}
		heldEntity.SetLightsOn(true);
		Transform transform = heldEntity.transform;
		Transform muzzleTransform = heldEntity.MuzzleTransform;
		heldEntity.SetParent(null, false, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		Quaternion rhs = transform.rotation * Quaternion.Inverse(muzzleTransform.rotation);
		heldEntity.limitNetworking = false;
		heldEntity.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		heldEntity.SetParent(this, StringPool.Get(this.socketTransform.name), false, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.rotation *= rhs;
		Vector3 vector = this.socketTransform.InverseTransformPoint(muzzleTransform.position);
		transform.localPosition = Vector3.left * vector.x;
		float d = Vector3.Distance(muzzleTransform.position, transform.position);
		transform.localPosition += Vector3.forward * d * this.attachedWeaponZOffsetScale;
		heldEntity.SetGenericVisible(true);
		this.AttachedWeapon = heldEntity;
		this.totalAmmoDirty = true;
		this.Reload();
		this.UpdateTotalAmmo();
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00024AC0 File Offset: 0x00022CC0
	public override void OnKilled(HitInfo info)
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (attachedWeapon != null)
		{
			attachedWeapon.SetGenericVisible(false);
			attachedWeapon.SetLightsOn(false);
		}
		this.AttachedWeapon = null;
		base.OnKilled(info);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00024AF9 File Offset: 0x00022CF9
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return this.IsAuthed(baseEntity) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00024B0D File Offset: 0x00022D0D
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateTotalAmmo();
		this.EnsureReloaded(false);
		this.UpdateTotalAmmo();
		this.nextShotTime = UnityEngine.Time.time;
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x00024B38 File Offset: 0x00022D38
	public virtual float GetMaxAngleForEngagement()
	{
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		float result = (attachedWeapon == null) ? 1f : ((1f - Mathf.InverseLerp(0.2f, 1f, attachedWeapon.repeatDelay)) * 7f);
		if (UnityEngine.Time.time - this.lastShotTime > 1f)
		{
			result = 1f;
		}
		return result;
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00024B98 File Offset: 0x00022D98
	public void TargetTick()
	{
		if (UnityEngine.Time.realtimeSinceStartup >= this.nextVisCheck)
		{
			this.nextVisCheck = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(0.2f, 0.3f);
			this.targetVisible = this.ObjectVisible(this.target);
			if (this.targetVisible)
			{
				this.lastTargetSeenTime = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		this.EnsureReloaded(true);
		global::BaseProjectile attachedWeapon = this.GetAttachedWeapon();
		if (UnityEngine.Time.time >= this.nextShotTime && this.targetVisible && Mathf.Abs(this.AngleToTarget(this.target, this.currentAmmoGravity != 0f)) < this.GetMaxAngleForEngagement())
		{
			if (attachedWeapon)
			{
				if (attachedWeapon.primaryMagazine.contents > 0)
				{
					this.FireAttachedGun(this.AimOffset(this.target), this.aimCone, null, this.PeacekeeperMode() ? this.target : null);
					float num = attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay;
					num = attachedWeapon.ScaleRepeatDelay(num);
					this.nextShotTime = UnityEngine.Time.time + num;
				}
				else
				{
					this.nextShotTime = UnityEngine.Time.time + 5f;
				}
			}
			else if (this.HasFallbackWeapon())
			{
				this.FireGun(this.AimOffset(this.target), this.aimCone, null, this.target);
				this.nextShotTime = UnityEngine.Time.time + 0.115f;
			}
			else if (this.HasGenericFireable())
			{
				this.AttachedWeapon.ServerUse();
				this.nextShotTime = UnityEngine.Time.time + 0.115f;
			}
			else
			{
				this.nextShotTime = UnityEngine.Time.time + 1f;
			}
		}
		if (this.target == null || this.target.IsDead() || UnityEngine.Time.realtimeSinceStartup - this.lastTargetSeenTime > 3f || Vector3.Distance(base.transform.position, this.target.transform.position) > this.sightRange || (this.PeacekeeperMode() && !this.IsEntityHostile(this.target)))
		{
			this.SetTarget(null);
		}
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00024DB9 File Offset: 0x00022FB9
	public bool HasTarget()
	{
		return this.target != null && this.target.IsAlive();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x00024DD6 File Offset: 0x00022FD6
	public void OfflineTick()
	{
		this.aimDir = Vector3.up;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00024DE4 File Offset: 0x00022FE4
	public virtual bool IsEntityHostile(BaseCombatEntity ent)
	{
		if (ent is ScarecrowNPC)
		{
			return true;
		}
		BasePet basePet;
		if ((basePet = (ent as BasePet)) != null && basePet.Brain.OwningPlayer != null)
		{
			return basePet.Brain.OwningPlayer.IsHostile() || ent.IsHostile();
		}
		return ent.IsHostile();
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00024E3C File Offset: 0x0002303C
	public bool ShouldTarget(BaseCombatEntity targ)
	{
		BasePet basePet;
		return !(targ is global::AutoTurret) && !(targ is RidableHorse) && ((basePet = (targ as BasePet)) == null || !(basePet.Brain.OwningPlayer != null) || !this.IsAuthed(basePet.Brain.OwningPlayer));
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00024E90 File Offset: 0x00023090
	private void ScheduleForTargetScan()
	{
		global::AutoTurret.updateAutoTurretScanQueue.Add(this);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00024EA0 File Offset: 0x000230A0
	public void TargetScan()
	{
		if (this.HasTarget() || this.IsOffline())
		{
			return;
		}
		if (this.IsBeingControlled)
		{
			return;
		}
		if (this.targetTrigger.entityContents != null)
		{
			foreach (global::BaseEntity baseEntity in this.targetTrigger.entityContents)
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null))
				{
					if (!Sentry.targetall)
					{
						global::BasePlayer basePlayer = baseCombatEntity as global::BasePlayer;
						if (basePlayer != null && (this.IsAuthed(basePlayer) || this.Ignore(basePlayer)))
						{
							continue;
						}
					}
					if ((!this.PeacekeeperMode() || this.IsEntityHostile(baseCombatEntity)) && baseCombatEntity.IsAlive() && this.ShouldTarget(baseCombatEntity) && this.InFiringArc(baseCombatEntity) && this.ObjectVisible(baseCombatEntity))
					{
						this.SetTarget(baseCombatEntity);
						break;
					}
				}
			}
		}
		if (this.PeacekeeperMode() && this.target == null)
		{
			this.nextShotTime = UnityEngine.Time.time + 1f;
		}
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool Ignore(global::BasePlayer player)
	{
		return false;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00024FBC File Offset: 0x000231BC
	public void ServerTick()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		float dt = (float)this.timeSinceLastServerTick;
		this.timeSinceLastServerTick = 0.0;
		if (!this.IsOnline())
		{
			this.OfflineTick();
		}
		else if (!this.IsBeingControlled)
		{
			if (this.HasTarget())
			{
				this.TargetTick();
			}
			else
			{
				this.IdleTick(dt);
			}
		}
		this.UpdateFacingToTarget(dt);
		if (this.totalAmmoDirty && UnityEngine.Time.time > this.nextAmmoCheckTime)
		{
			this.UpdateTotalAmmo();
			this.totalAmmoDirty = false;
			this.nextAmmoCheckTime = UnityEngine.Time.time + 0.5f;
		}
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00025068 File Offset: 0x00023268
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if ((this.IsOnline() && !this.HasTarget()) || !this.targetVisible)
		{
			if (info.Initiator as global::AutoTurret != null)
			{
				return;
			}
			if (info.Initiator as SamSite != null)
			{
				return;
			}
			if (info.Initiator as GunTrap != null)
			{
				return;
			}
			global::BasePlayer basePlayer = info.Initiator as global::BasePlayer;
			if (!basePlayer || !this.IsAuthed(basePlayer))
			{
				this.SetTarget(info.Initiator as BaseCombatEntity);
			}
		}
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00025100 File Offset: 0x00023300
	public void UpdateFacingToTarget(float dt)
	{
		if (this.target != null && this.targetVisible && !this.IsBeingControlled)
		{
			Vector3 vector = this.AimOffset(this.target);
			if (this.peekIndex != 0)
			{
				Vector3 position = this.eyePos.transform.position;
				Vector3.Distance(vector, position);
				Vector3 a = Vector3.Cross((vector - position).normalized, Vector3.up);
				vector += a * global::AutoTurret.visibilityOffsets[this.peekIndex];
			}
			Vector3 vector2 = (vector - this.eyePos.transform.position).normalized;
			if (this.currentAmmoGravity != 0f)
			{
				float num = 0.2f;
				if (this.target is global::BasePlayer)
				{
					float num2 = Mathf.Clamp01(this.target.WaterFactor()) * 1.8f;
					if (num2 > num)
					{
						num = num2;
					}
				}
				vector = this.target.transform.position + Vector3.up * num;
				float angle = this.GetAngle(this.eyePos.transform.position, vector, this.currentAmmoVelocity, this.currentAmmoGravity);
				Vector3 normalized = (vector.XZ3D() - this.eyePos.transform.position.XZ3D()).normalized;
				vector2 = Quaternion.LookRotation(normalized) * Quaternion.Euler(angle, 0f, 0f) * Vector3.forward;
			}
			this.aimDir = vector2;
		}
		this.UpdateAiming(dt);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x000252A4 File Offset: 0x000234A4
	private float GetAngle(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravityScale)
	{
		float num = UnityEngine.Physics.gravity.y * gravityScale;
		float num2 = Vector3.Distance(launchPosition.XZ3D(), targetPosition.XZ3D());
		float num3 = launchPosition.y - targetPosition.y;
		float num4 = Mathf.Pow(launchVelocity, 2f);
		float num5 = Mathf.Pow(launchVelocity, 4f);
		float num6 = Mathf.Atan((num4 + Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		float num7 = Mathf.Atan((num4 - Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		if (float.IsNaN(num6) && float.IsNaN(num7))
		{
			return -45f;
		}
		if (float.IsNaN(num6))
		{
			return num7;
		}
		if (num6 <= num7)
		{
			return num7;
		}
		return num6;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0002538F File Offset: 0x0002358F
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.AddSelfAuthorize(deployedBy);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x000253A4 File Offset: 0x000235A4
	public override ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000253BC File Offset: 0x000235BC
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		bool flag = item.info.category == ItemCategory.Weapon;
		bool flag2 = item.info.category == ItemCategory.Ammunition;
		if (flag)
		{
			return 0;
		}
		if (flag2)
		{
			for (int i = 1; i < base.inventory.capacity; i++)
			{
				if (!base.inventory.SlotTaken(item, i))
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00007641 File Offset: 0x00005841
	public bool IsOnline()
	{
		return base.IsOn();
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00025415 File Offset: 0x00023615
	public bool IsOffline()
	{
		return !this.IsOnline();
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00025428 File Offset: 0x00023628
	public virtual Transform GetCenterMuzzle()
	{
		return this.gun_pitch;
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00025430 File Offset: 0x00023630
	public float AngleToTarget(BaseCombatEntity potentialtarget, bool use2D = false)
	{
		use2D = true;
		Transform centerMuzzle = this.GetCenterMuzzle();
		Vector3 position = centerMuzzle.position;
		Vector3 vector = this.AimOffset(potentialtarget);
		Vector3 to = Vector3.zero;
		if (use2D)
		{
			to = Vector3Ex.Direction2D(vector, position);
		}
		else
		{
			to = (vector - position).normalized;
		}
		return Vector3.Angle(use2D ? centerMuzzle.forward.XZ3D().normalized : centerMuzzle.forward, to);
	}

	// Token: 0x060001CB RID: 459 RVA: 0x0002549F File Offset: 0x0002369F
	public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return Mathf.Abs(this.AngleToTarget(potentialtarget, false)) <= 90f;
	}

	// Token: 0x060001CC RID: 460 RVA: 0x000254B8 File Offset: 0x000236B8
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this.IsOffline() && this.IsAuthed(player);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x000254D4 File Offset: 0x000236D4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Facepunch.Pool.Get<ProtoBuf.AutoTurret>();
		info.msg.autoturret.users = this.authorizedPlayers;
		if (!info.forDisk)
		{
			Connection forConnection = info.forConnection;
			if (!(((forConnection != null) ? forConnection.player : null) != null) || !this.CanChangeID(info.forConnection.player as global::BasePlayer))
			{
				return;
			}
		}
		info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		info.msg.rcEntity.identifier = this.GetIdentifier();
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0002556E File Offset: 0x0002376E
	public override void PostSave(global::BaseNetworkable.SaveInfo info)
	{
		base.PostSave(info);
		info.msg.autoturret.users = null;
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00025588 File Offset: 0x00023788
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.authorizedPlayers = info.msg.autoturret.users;
			info.msg.autoturret.users = null;
		}
		if (info.msg.rcEntity != null)
		{
			this.UpdateIdentifier(info.msg.rcEntity.identifier, false);
		}
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x000255F4 File Offset: 0x000237F4
	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		global::BasePlayer basePlayer = aimat as global::BasePlayer;
		if (!(basePlayer != null))
		{
			return aimat.CenterPoint();
		}
		if (basePlayer.IsSleeping())
		{
			return basePlayer.transform.position + Vector3.up * 0.1f;
		}
		if (basePlayer.IsWounded())
		{
			return basePlayer.transform.position + Vector3.up * 0.25f;
		}
		return basePlayer.eyes.position;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00025673 File Offset: 0x00023873
	public float GetAimSpeed()
	{
		if (this.HasTarget())
		{
			return 5f;
		}
		return 1f;
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x00025688 File Offset: 0x00023888
	public void UpdateAiming(float dt)
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		float speed = 5f;
		if (base.isServer && !this.IsBeingControlled)
		{
			if (this.HasTarget())
			{
				speed = 35f;
			}
			else
			{
				speed = 15f;
			}
		}
		Quaternion quaternion = Quaternion.LookRotation(this.aimDir);
		Quaternion quaternion2 = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		Quaternion quaternion3 = Quaternion.Euler(quaternion.eulerAngles.x, 0f, 0f);
		if (this.gun_yaw.transform.rotation != quaternion2)
		{
			this.gun_yaw.transform.rotation = Mathx.Lerp(this.gun_yaw.transform.rotation, quaternion2, speed, dt);
		}
		if (this.gun_pitch.transform.localRotation != quaternion3)
		{
			this.gun_pitch.transform.localRotation = Mathx.Lerp(this.gun_pitch.transform.localRotation, quaternion3, speed, dt);
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00025798 File Offset: 0x00023998
	public bool IsAuthed(ulong id)
	{
		using (List<PlayerNameID>.Enumerator enumerator = this.authorizedPlayers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.userid == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x000257F4 File Offset: 0x000239F4
	public bool IsAuthed(global::BasePlayer player)
	{
		return this.IsAuthed(player.userID);
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00025802 File Offset: 0x00023A02
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00025812 File Offset: 0x00023A12
	public virtual bool CanChangeSettings(global::BasePlayer player)
	{
		return this.IsAuthed(player) && this.IsOffline() && player.CanBuild();
	}

	// Token: 0x02000B57 RID: 2903
	public static class TurretFlags
	{
		// Token: 0x04003EC2 RID: 16066
		public const global::BaseEntity.Flags Peacekeeper = global::BaseEntity.Flags.Reserved1;
	}

	// Token: 0x02000B58 RID: 2904
	public class UpdateAutoTurretScanQueue : ObjectWorkQueue<global::AutoTurret>
	{
		// Token: 0x06004C94 RID: 19604 RVA: 0x0019EBA2 File Offset: 0x0019CDA2
		protected override void RunJob(global::AutoTurret entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.TargetScan();
		}

		// Token: 0x06004C95 RID: 19605 RVA: 0x0019EBB4 File Offset: 0x0019CDB4
		protected override bool ShouldAdd(global::AutoTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
