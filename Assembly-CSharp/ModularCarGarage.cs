using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A1 RID: 161
public class ModularCarGarage : ContainerIOEntity
{
	// Token: 0x040009A5 RID: 2469
	private global::ModularCar lockedOccupant;

	// Token: 0x040009A6 RID: 2470
	private readonly HashSet<global::BasePlayer> lootingPlayers = new HashSet<global::BasePlayer>();

	// Token: 0x040009A7 RID: 2471
	private MagnetSnap magnetSnap;

	// Token: 0x040009A8 RID: 2472
	[SerializeField]
	private Transform vehicleLift;

	// Token: 0x040009A9 RID: 2473
	[SerializeField]
	private Animation vehicleLiftAnim;

	// Token: 0x040009AA RID: 2474
	[SerializeField]
	private string animName = "LiftUp";

	// Token: 0x040009AB RID: 2475
	[SerializeField]
	private VehicleLiftOccupantTrigger occupantTrigger;

	// Token: 0x040009AC RID: 2476
	[SerializeField]
	private float liftMoveTime = 1f;

	// Token: 0x040009AD RID: 2477
	[SerializeField]
	private EmissionToggle poweredLight;

	// Token: 0x040009AE RID: 2478
	[SerializeField]
	private EmissionToggle inUseLight;

	// Token: 0x040009AF RID: 2479
	[SerializeField]
	private Transform vehicleLiftPos;

	// Token: 0x040009B0 RID: 2480
	[SerializeField]
	[Range(0f, 1f)]
	private float recycleEfficiency = 0.5f;

	// Token: 0x040009B1 RID: 2481
	[SerializeField]
	private Transform recycleDropPos;

	// Token: 0x040009B2 RID: 2482
	[SerializeField]
	private bool needsElectricity;

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private SoundDefinition liftStartSoundDef;

	// Token: 0x040009B4 RID: 2484
	[SerializeField]
	private SoundDefinition liftStopSoundDef;

	// Token: 0x040009B5 RID: 2485
	[SerializeField]
	private SoundDefinition liftStopDownSoundDef;

	// Token: 0x040009B6 RID: 2486
	[SerializeField]
	private SoundDefinition liftLoopSoundDef;

	// Token: 0x040009B7 RID: 2487
	[SerializeField]
	private GameObjectRef addRemoveLockEffect;

	// Token: 0x040009B8 RID: 2488
	[SerializeField]
	private GameObjectRef changeLockCodeEffect;

	// Token: 0x040009B9 RID: 2489
	[SerializeField]
	private GameObjectRef repairEffect;

	// Token: 0x040009BA RID: 2490
	[SerializeField]
	private TriggerBase playerTrigger;

	// Token: 0x040009BB RID: 2491
	public ModularCarGarage.ChassisBuildOption[] chassisBuildOptions;

	// Token: 0x040009BC RID: 2492
	public ItemAmount lockResourceCost;

	// Token: 0x040009C1 RID: 2497
	private ModularCarGarage.VehicleLiftState vehicleLiftState;

	// Token: 0x040009C2 RID: 2498
	private Sound liftLoopSound;

	// Token: 0x040009C3 RID: 2499
	private Vector3 downPos;

	// Token: 0x040009C4 RID: 2500
	public const global::BaseEntity.Flags Flag_DestroyingChassis = global::BaseEntity.Flags.Reserved6;

	// Token: 0x040009C5 RID: 2501
	public const float TimeToDestroyChassis = 10f;

	// Token: 0x040009C6 RID: 2502
	public const global::BaseEntity.Flags Flag_EnteringKeycode = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040009C7 RID: 2503
	public const global::BaseEntity.Flags Flag_PlayerObstructing = global::BaseEntity.Flags.Reserved8;

	// Token: 0x06000ECF RID: 3791 RVA: 0x0007CD80 File Offset: 0x0007AF80
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarGarage.OnRpcMessage", 0))
		{
			if (rpc == 554177909U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeselectedLootItem ");
				}
				using (TimeWarning.New("RPC_DeselectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(554177909U, "RPC_DeselectedLootItem", this, player, 3f))
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
							this.RPC_DeselectedLootItem(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_DeselectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 3683966290U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DiedWithKeypadOpen ");
				}
				using (TimeWarning.New("RPC_DiedWithKeypadOpen", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
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
							this.RPC_DiedWithKeypadOpen(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_DiedWithKeypadOpen");
					}
				}
				return true;
			}
			if (rpc == 3659332720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenEditing ");
				}
				using (TimeWarning.New("RPC_OpenEditing", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
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
							this.RPC_OpenEditing(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenEditing");
					}
				}
				return true;
			}
			if (rpc == 1582295101U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RepairItem ");
				}
				using (TimeWarning.New("RPC_RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
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
							this.RPC_RepairItem(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_RepairItem");
					}
				}
				return true;
			}
			if (rpc == 3710764312U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestAddLock ");
				}
				using (TimeWarning.New("RPC_RequestAddLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestAddLock(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_RequestAddLock");
					}
				}
				return true;
			}
			if (rpc == 3305106830U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestNewCode ");
				}
				using (TimeWarning.New("RPC_RequestNewCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestNewCode(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_RequestNewCode");
					}
				}
				return true;
			}
			if (rpc == 1046853419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestRemoveLock ");
				}
				using (TimeWarning.New("RPC_RequestRemoveLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestRemoveLock(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in RPC_RequestRemoveLock");
					}
				}
				return true;
			}
			if (rpc == 4033916654U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SelectedLootItem ");
				}
				using (TimeWarning.New("RPC_SelectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4033916654U, "RPC_SelectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_SelectedLootItem(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in RPC_SelectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 2974124904U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StartDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartDestroyingChassis(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in RPC_StartDestroyingChassis");
					}
				}
				return true;
			}
			if (rpc == 3872977075U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartKeycodeEntry ");
				}
				using (TimeWarning.New("RPC_StartKeycodeEntry", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3872977075U, "RPC_StartKeycodeEntry", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartKeycodeEntry(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
						player.Kick("RPC Error in RPC_StartKeycodeEntry");
					}
				}
				return true;
			}
			if (rpc == 3830531963U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StopDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StopDestroyingChassis(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
						player.Kick("RPC Error in RPC_StopDestroyingChassis");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x0007DDE4 File Offset: 0x0007BFE4
	private global::ModularCar carOccupant
	{
		get
		{
			if (!(this.lockedOccupant != null))
			{
				return this.occupantTrigger.carOccupant;
			}
			return this.lockedOccupant;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x0007DE06 File Offset: 0x0007C006
	private bool HasOccupant
	{
		get
		{
			return this.carOccupant != null && this.carOccupant.IsFullySpawned();
		}
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0007DE24 File Offset: 0x0007C024
	protected void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.magnetSnap == null)
		{
			return;
		}
		if (this.playerTrigger != null)
		{
			bool hasAnyContents = this.playerTrigger.HasAnyContents;
			if (this.PlayerObstructingLift != hasAnyContents)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, hasAnyContents, false, true);
			}
		}
		this.UpdateCarOccupant();
		if (this.HasOccupant && this.carOccupant.CouldBeEdited() && this.carOccupant.GetSpeed() <= 1f)
		{
			if (base.IsOn() || !this.carOccupant.IsComplete())
			{
				if (this.lockedOccupant == null && !this.carOccupant.rigidBody.isKinematic)
				{
					this.GrabOccupant(this.occupantTrigger.carOccupant);
				}
				this.magnetSnap.FixedUpdate(this.carOccupant.transform);
			}
			if (this.carOccupant.CarLock.HasALock && !this.carOccupant.CarLock.CanHaveALock())
			{
				this.carOccupant.CarLock.RemoveLock();
			}
		}
		else if (this.HasOccupant && this.carOccupant.rigidBody.isKinematic)
		{
			this.ReleaseOccupant();
		}
		if (this.HasOccupant && this.IsDestroyingChassis && this.carOccupant.HasAnyModules)
		{
			this.StopChassisDestroy();
		}
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0007DF7F File Offset: 0x0007C17F
	internal override void DoServerDestroy()
	{
		if (this.HasOccupant)
		{
			this.ReleaseOccupant();
			if (!this.HasDriveableOccupant)
			{
				this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0007DFA9 File Offset: 0x0007C1A9
	public override void ServerInit()
	{
		base.ServerInit();
		this.magnetSnap = new MagnetSnap(this.vehicleLiftPos);
		this.RefreshOnOffState();
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, true);
		this.RefreshLiftState(true);
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x0007DFDC File Offset: 0x0007C1DC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleLift = Facepunch.Pool.Get<VehicleLift>();
		info.msg.vehicleLift.platformIsOccupied = this.PlatformIsOccupied;
		info.msg.vehicleLift.editableOccupant = this.HasEditableOccupant;
		info.msg.vehicleLift.driveableOccupant = this.HasDriveableOccupant;
		info.msg.vehicleLift.occupantLockState = (int)this.OccupantLockState;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x0007E058 File Offset: 0x0007C258
	public override ItemContainerId GetIdealContainer(global::BasePlayer player, global::Item item, bool altMove)
	{
		return default(ItemContainerId);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0007E070 File Offset: 0x0007C270
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		if (player == null)
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (!flag)
		{
			return false;
		}
		if (this.HasEditableOccupant)
		{
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ModuleContainer);
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ChassisContainer);
			player.inventory.loot.SendImmediate();
		}
		this.lootingPlayers.Add(player);
		this.RefreshLiftState(false);
		return flag;
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0007E112 File Offset: 0x0007C312
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (!this.IsEnteringKeycode)
		{
			this.lootingPlayers.Remove(player);
			this.RefreshLiftState(false);
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0007E137 File Offset: 0x0007C337
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.RefreshOnOffState();
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0007E147 File Offset: 0x0007C347
	public bool TryGetModuleForItem(global::Item item, out BaseVehicleModule result)
	{
		if (!this.HasOccupant)
		{
			result = null;
			return false;
		}
		result = this.carOccupant.GetModuleForItem(item);
		return result != null;
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x0007E16C File Offset: 0x0007C36C
	private void RefreshOnOffState()
	{
		bool flag = !this.needsElectricity || this.currentEnergy >= this.ConsumptionAmount();
		if (flag != base.IsOn())
		{
			base.SetFlag(global::BaseEntity.Flags.On, flag, false, true);
		}
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0007E1AC File Offset: 0x0007C3AC
	private void UpdateCarOccupant()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.HasOccupant)
		{
			bool editableOccupant = Vector3.SqrMagnitude(this.carOccupant.transform.position - this.vehicleLiftPos.position) < 1f && this.carOccupant.CouldBeEdited() && !this.PlayerObstructingLift;
			bool driveableOccupant = this.carOccupant.IsComplete();
			ModularCarGarage.OccupantLock occupantLockState;
			if (this.carOccupant.CarLock.CanHaveALock())
			{
				if (this.carOccupant.CarLock.HasALock)
				{
					occupantLockState = ModularCarGarage.OccupantLock.HasLock;
				}
				else
				{
					occupantLockState = ModularCarGarage.OccupantLock.NoLock;
				}
			}
			else
			{
				occupantLockState = ModularCarGarage.OccupantLock.CannotHaveLock;
			}
			this.SetOccupantState(this.HasOccupant, editableOccupant, driveableOccupant, occupantLockState, false);
			return;
		}
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, false);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x0007E26B File Offset: 0x0007C46B
	private void UpdateOccupantMode()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = (this.HasEditableOccupant && this.LiftIsUp);
		this.carOccupant.immuneToDecay = base.IsOn();
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x0007E2A4 File Offset: 0x0007C4A4
	private void WakeNearbyRigidbodies()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.position, 7f, list, 34816, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null && attachedRigidbody.IsSleeping())
			{
				attachedRigidbody.WakeUp();
			}
			global::BaseEntity baseEntity = collider.ToBaseEntity();
			BaseRidableAnimal baseRidableAnimal;
			if (baseEntity != null && (baseRidableAnimal = (baseEntity as BaseRidableAnimal)) != null && baseRidableAnimal.isServer)
			{
				baseRidableAnimal.UpdateDropToGroundForDuration(2f);
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x0007E360 File Offset: 0x0007C560
	private void EditableOccupantEntered()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0007E360 File Offset: 0x0007C560
	private void EditableOccupantLeft()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x0007E368 File Offset: 0x0007C568
	private void RefreshLoot()
	{
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		list.AddRange(this.lootingPlayers);
		foreach (global::BasePlayer basePlayer in list)
		{
			basePlayer.inventory.loot.Clear();
			this.PlayerOpenLoot(basePlayer, "", true);
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x0007E3E8 File Offset: 0x0007C5E8
	private void GrabOccupant(global::ModularCar occupant)
	{
		if (occupant == null)
		{
			return;
		}
		this.lockedOccupant = occupant;
		this.lockedOccupant.DisablePhysics();
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x0007E408 File Offset: 0x0007C608
	private void ReleaseOccupant()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = false;
		this.carOccupant.immuneToDecay = false;
		if (this.lockedOccupant != null)
		{
			this.lockedOccupant.EnablePhysics();
			this.lockedOccupant = null;
		}
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x0007E456 File Offset: 0x0007C656
	private void StopChassisDestroy()
	{
		if (base.IsInvoking(new Action(this.FinishDestroyingChassis)))
		{
			base.CancelInvoke(new Action(this.FinishDestroyingChassis));
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0007E48C File Offset: 0x0007C68C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RepairItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		ItemId itemId = msg.read.ItemID();
		if (player == null || !this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(itemId);
		if (vehicleItem != null)
		{
			RepairBench.RepairAnItem(vehicleItem, player, this, 0f, false);
			Effect.server.Run(this.repairEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			return;
		}
		Debug.LogError(base.GetType().Name + ": Couldn't get item to repair, with ID: " + itemId);
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0007E51C File Offset: 0x0007C71C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || this.LiftIsMoving)
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0007E550 File Offset: 0x0007C750
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DiedWithKeypadOpen(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		this.lootingPlayers.Clear();
		this.RefreshLiftState(false);
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0007E574 File Offset: 0x0007C774
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SelectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		ItemId itemUID = msg.read.ItemID();
		if (player == null || !player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (!this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(itemUID);
		if (vehicleItem != null)
		{
			bool flag = player.inventory.loot.RemoveContainerAt(3);
			BaseVehicleModule baseVehicleModule;
			if (this.TryGetModuleForItem(vehicleItem, out baseVehicleModule))
			{
				VehicleModuleStorage vehicleModuleStorage;
				VehicleModuleCamper vehicleModuleCamper;
				if ((vehicleModuleStorage = (baseVehicleModule as VehicleModuleStorage)) != null)
				{
					IItemContainerEntity container = vehicleModuleStorage.GetContainer();
					if (!container.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container.inventory);
						flag = true;
					}
				}
				else if ((vehicleModuleCamper = (baseVehicleModule as VehicleModuleCamper)) != null)
				{
					IItemContainerEntity container2 = vehicleModuleCamper.GetContainer();
					if (!container2.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container2.inventory);
						flag = true;
					}
				}
			}
			if (flag)
			{
				player.inventory.loot.SendImmediate();
			}
		}
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0007E684 File Offset: 0x0007C884
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DeselectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (player.inventory.loot.RemoveContainerAt(3))
		{
			player.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0007E6E6 File Offset: 0x0007C8E6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StartKeycodeEntry(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0007E6F8 File Offset: 0x0007C8F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestAddLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string code = msg.read.String(256);
		ItemAmount itemAmount = this.lockResourceCost;
		if ((float)player.inventory.GetAmount(itemAmount.itemDef.itemid) >= itemAmount.amount && this.carOccupant.CarLock.TryAddALock(code, player.userID))
		{
			player.inventory.Take(null, itemAmount.itemDef.itemid, Mathf.CeilToInt(itemAmount.amount));
			Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0007E7C4 File Offset: 0x0007C9C4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestRemoveLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		this.carOccupant.CarLock.RemoveLock();
		Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0007E81C File Offset: 0x0007CA1C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestNewCode(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string newCode = msg.read.String(256);
		if (this.carOccupant.CarLock.TrySetNewCode(newCode, player.userID))
		{
			Effect.server.Run(this.changeLockCodeEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0007E89E File Offset: 0x0007CA9E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StartDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		base.Invoke(new Action(this.FinishDestroyingChassis), 10f);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x0007E8D3 File Offset: 0x0007CAD3
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StopDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		this.StopChassisDestroy();
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x0007E8DB File Offset: 0x0007CADB
	private void FinishDestroyingChassis()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x0007E90E File Offset: 0x0007CB0E
	// (set) Token: 0x06000EF2 RID: 3826 RVA: 0x0007E916 File Offset: 0x0007CB16
	public bool PlatformIsOccupied { get; private set; }

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0007E91F File Offset: 0x0007CB1F
	// (set) Token: 0x06000EF4 RID: 3828 RVA: 0x0007E927 File Offset: 0x0007CB27
	public bool HasEditableOccupant { get; private set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0007E930 File Offset: 0x0007CB30
	// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0007E938 File Offset: 0x0007CB38
	public bool HasDriveableOccupant { get; private set; }

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0007E941 File Offset: 0x0007CB41
	// (set) Token: 0x06000EF8 RID: 3832 RVA: 0x0007E949 File Offset: 0x0007CB49
	public ModularCarGarage.OccupantLock OccupantLockState { get; private set; }

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0007E952 File Offset: 0x0007CB52
	private bool LiftIsUp
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Up;
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0007E95D File Offset: 0x0007CB5D
	private bool LiftIsMoving
	{
		get
		{
			return this.vehicleLiftAnim.isPlaying;
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000EFB RID: 3835 RVA: 0x0007E96A File Offset: 0x0007CB6A
	private bool LiftIsDown
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Down;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000EFC RID: 3836 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsDestroyingChassis
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000EFD RID: 3837 RVA: 0x0004B36F File Offset: 0x0004956F
	private bool IsEnteringKeycode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved7);
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000EFE RID: 3838 RVA: 0x00003278 File Offset: 0x00001478
	public bool PlayerObstructingLift
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0007E975 File Offset: 0x0007CB75
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.downPos = this.vehicleLift.transform.position;
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0007E98D File Offset: 0x0007CB8D
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			this.UpdateOccupantMode();
		}
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00007641 File Offset: 0x00005841
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.IsOn();
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x0002179A File Offset: 0x0001F99A
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x0007E9A8 File Offset: 0x0007CBA8
	private void SetOccupantState(bool hasOccupant, bool editableOccupant, bool driveableOccupant, ModularCarGarage.OccupantLock occupantLockState, bool forced = false)
	{
		if (this.PlatformIsOccupied == hasOccupant && this.HasEditableOccupant == editableOccupant && this.HasDriveableOccupant == driveableOccupant && this.OccupantLockState == occupantLockState && !forced)
		{
			return;
		}
		bool hasEditableOccupant = this.HasEditableOccupant;
		this.PlatformIsOccupied = hasOccupant;
		this.HasEditableOccupant = editableOccupant;
		this.HasDriveableOccupant = driveableOccupant;
		this.OccupantLockState = occupantLockState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (hasEditableOccupant && !editableOccupant)
			{
				this.EditableOccupantLeft();
			}
			else if (editableOccupant && !hasEditableOccupant)
			{
				this.EditableOccupantEntered();
			}
		}
		this.RefreshLiftState(false);
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0007EA3C File Offset: 0x0007CC3C
	private void RefreshLiftState(bool forced = false)
	{
		ModularCarGarage.VehicleLiftState desiredLiftState = (base.IsOpen() || this.IsEnteringKeycode || (this.HasEditableOccupant && !this.HasDriveableOccupant)) ? ModularCarGarage.VehicleLiftState.Up : ModularCarGarage.VehicleLiftState.Down;
		this.MoveLift(desiredLiftState, 0f, forced);
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0007EA84 File Offset: 0x0007CC84
	private void MoveLift(ModularCarGarage.VehicleLiftState desiredLiftState, float startDelay = 0f, bool forced = false)
	{
		if (this.vehicleLiftState == desiredLiftState && !forced)
		{
			return;
		}
		ModularCarGarage.VehicleLiftState vehicleLiftState = this.vehicleLiftState;
		this.vehicleLiftState = desiredLiftState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			this.WakeNearbyRigidbodies();
		}
		if (!base.gameObject.activeSelf)
		{
			this.vehicleLiftAnim[this.animName].time = ((desiredLiftState == ModularCarGarage.VehicleLiftState.Up) ? 1f : 0f);
			this.vehicleLiftAnim.Play();
			return;
		}
		if (desiredLiftState == ModularCarGarage.VehicleLiftState.Up)
		{
			base.Invoke(new Action(this.MoveLiftUp), startDelay);
			return;
		}
		base.Invoke(new Action(this.MoveLiftDown), startDelay);
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0007EB2B File Offset: 0x0007CD2B
	private void MoveLiftUp()
	{
		this.vehicleLiftAnim[this.animName].length /= this.liftMoveTime;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0007EB5C File Offset: 0x0007CD5C
	private void MoveLiftDown()
	{
		AnimationState animationState = this.vehicleLiftAnim[this.animName];
		animationState.speed = animationState.length / this.liftMoveTime;
		if (!this.vehicleLiftAnim.isPlaying && Vector3.Distance(this.vehicleLift.transform.position, this.downPos) > 0.01f)
		{
			animationState.time = 1f;
		}
		animationState.speed *= -1f;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x02000BE7 RID: 3047
	[Serializable]
	public class ChassisBuildOption
	{
		// Token: 0x04004143 RID: 16707
		public GameObjectRef prefab;

		// Token: 0x04004144 RID: 16708
		public ItemDefinition itemDef;
	}

	// Token: 0x02000BE8 RID: 3048
	public enum OccupantLock
	{
		// Token: 0x04004146 RID: 16710
		CannotHaveLock,
		// Token: 0x04004147 RID: 16711
		NoLock,
		// Token: 0x04004148 RID: 16712
		HasLock
	}

	// Token: 0x02000BE9 RID: 3049
	private enum VehicleLiftState
	{
		// Token: 0x0400414A RID: 16714
		Down,
		// Token: 0x0400414B RID: 16715
		Up
	}
}
