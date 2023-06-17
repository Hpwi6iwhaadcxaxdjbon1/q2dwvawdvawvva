using System;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CF RID: 207
public class SmartAlarm : AppIOEntity, ISubscribable
{
	// Token: 0x04000B87 RID: 2951
	public const global::BaseEntity.Flags Flag_HasCustomMessage = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000B88 RID: 2952
	public static readonly Translate.Phrase DefaultNotificationTitle = new Translate.Phrase("app.alarm.title", "Alarm");

	// Token: 0x04000B89 RID: 2953
	public static readonly Translate.Phrase DefaultNotificationBody = new Translate.Phrase("app.alarm.body", "Your base is under attack!");

	// Token: 0x04000B8A RID: 2954
	[Header("Smart Alarm")]
	public GameObjectRef SetupNotificationDialog;

	// Token: 0x04000B8B RID: 2955
	public Animator Animator;

	// Token: 0x04000B8D RID: 2957
	private readonly NotificationList _subscriptions = new NotificationList();

	// Token: 0x04000B8E RID: 2958
	private string _notificationTitle = "";

	// Token: 0x04000B8F RID: 2959
	private string _notificationBody = "";

	// Token: 0x04000B90 RID: 2960
	private float _lastSentTime;

	// Token: 0x06001277 RID: 4727 RVA: 0x000956B0 File Offset: 0x000938B0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartAlarm.OnRpcMessage", 0))
		{
			if (rpc == 3292290572U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetNotificationTextImpl ");
				}
				using (TimeWarning.New("SetNotificationTextImpl", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3292290572U, "SetNotificationTextImpl", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3292290572U, "SetNotificationTextImpl", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage notificationTextImpl = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetNotificationTextImpl(notificationTextImpl);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetNotificationTextImpl");
					}
				}
				return true;
			}
			if (rpc == 4207149767U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartSetupNotification ");
				}
				using (TimeWarning.New("StartSetupNotification", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4207149767U, "StartSetupNotification", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4207149767U, "StartSetupNotification", this, player, 3f))
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
							this.StartSetupNotification(rpc2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StartSetupNotification");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06001278 RID: 4728 RVA: 0x0004E73F File Offset: 0x0004C93F
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Alarm;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06001279 RID: 4729 RVA: 0x000959E8 File Offset: 0x00093BE8
	// (set) Token: 0x0600127A RID: 4730 RVA: 0x000959F0 File Offset: 0x00093BF0
	public override bool Value { get; set; }

	// Token: 0x0600127B RID: 4731 RVA: 0x000959F9 File Offset: 0x00093BF9
	public bool AddSubscription(ulong steamId)
	{
		return this._subscriptions.AddSubscription(steamId);
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00095A07 File Offset: 0x00093C07
	public bool RemoveSubscription(ulong steamId)
	{
		return this._subscriptions.RemoveSubscription(steamId);
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00095A15 File Offset: 0x00093C15
	public bool HasSubscription(ulong steamId)
	{
		return this._subscriptions.HasSubscription(steamId);
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x00095A23 File Offset: 0x00093C23
	public override void InitShared()
	{
		base.InitShared();
		this._notificationTitle = global::SmartAlarm.DefaultNotificationTitle.translated;
		this._notificationBody = global::SmartAlarm.DefaultNotificationBody.translated;
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x00095A4C File Offset: 0x00093C4C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		this.Value = (inputAmount > 0);
		if (this.Value == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, this.Value, false, true);
		base.BroadcastValueChange();
		float num = Mathf.Max(App.alarmcooldown, 15f);
		if (this.Value && UnityEngine.Time.realtimeSinceStartup - this._lastSentTime >= num)
		{
			BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
			if (buildingPrivilege != null)
			{
				this._subscriptions.IntersectWith(buildingPrivilege.authorizedPlayers);
			}
			this._subscriptions.SendNotification(NotificationChannel.SmartAlarm, this._notificationTitle, this._notificationBody, "alarm");
			this._lastSentTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x00095AFC File Offset: 0x00093CFC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.smartAlarm = Facepunch.Pool.Get<ProtoBuf.SmartAlarm>();
			info.msg.smartAlarm.notificationTitle = this._notificationTitle;
			info.msg.smartAlarm.notificationBody = this._notificationBody;
			info.msg.smartAlarm.subscriptions = this._subscriptions.ToList();
		}
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00095B70 File Offset: 0x00093D70
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.smartAlarm != null)
		{
			this._notificationTitle = info.msg.smartAlarm.notificationTitle;
			this._notificationBody = info.msg.smartAlarm.notificationBody;
			this._subscriptions.LoadFrom(info.msg.smartAlarm.subscriptions);
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00095BE0 File Offset: 0x00093DE0
	protected override void OnPairedWithPlayer(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!this.AddSubscription(player.userID))
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", 7);
		}
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00095C08 File Offset: 0x00093E08
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void StartSetupNotification(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		base.ClientRPCPlayer<string, string>(null, rpc.player, "SetupNotification", this._notificationTitle, this._notificationBody);
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x00095C60 File Offset: 0x00093E60
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void SetNotificationTextImpl(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege != null && !buildingPrivilege.CanAdministrate(rpc.player))
		{
			return;
		}
		string text = rpc.read.String(128);
		string text2 = rpc.read.String(512);
		if (!string.IsNullOrWhiteSpace(text))
		{
			this._notificationTitle = text;
		}
		if (!string.IsNullOrWhiteSpace(text2))
		{
			this._notificationBody = text2;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}
}
