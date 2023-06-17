using System;
using System.Collections.Generic;
using System.Globalization;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000033 RID: 51
public abstract class AppIOEntity : global::IOEntity
{
	// Token: 0x0400019E RID: 414
	private float _cacheTime;

	// Token: 0x0400019F RID: 415
	private BuildingPrivlidge _cache;

	// Token: 0x06000151 RID: 337 RVA: 0x00021E4C File Offset: 0x0002004C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AppIOEntity.OnRpcMessage", 0))
		{
			if (rpc == 3018927126U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PairWithApp ");
				}
				using (TimeWarning.New("PairWithApp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3018927126U, "PairWithApp", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3018927126U, "PairWithApp", this, player, 3f))
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
							this.PairWithApp(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in PairWithApp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000152 RID: 338
	public abstract AppEntityType Type { get; }

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000153 RID: 339 RVA: 0x00007A3C File Offset: 0x00005C3C
	// (set) Token: 0x06000154 RID: 340 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual bool Value
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0002200C File Offset: 0x0002020C
	protected void BroadcastValueChange()
	{
		if (!this.IsValid())
		{
			return;
		}
		EntityTarget target = this.GetTarget();
		AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
		appBroadcast.entityChanged = Facepunch.Pool.Get<AppEntityChanged>();
		appBroadcast.entityChanged.entityId = this.net.ID;
		appBroadcast.entityChanged.payload = Facepunch.Pool.Get<AppEntityPayload>();
		this.FillEntityPayload(appBroadcast.entityChanged.payload);
		CompanionServer.Server.Broadcast(target, appBroadcast);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00022076 File Offset: 0x00020276
	internal virtual void FillEntityPayload(AppEntityPayload payload)
	{
		payload.value = this.Value;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00022084 File Offset: 0x00020284
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this._cacheTime > 5f)
		{
			this._cache = base.GetBuildingPrivilege();
			this._cacheTime = UnityEngine.Time.realtimeSinceStartup;
		}
		return this._cache;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x000220B6 File Offset: 0x000202B6
	public EntityTarget GetTarget()
	{
		return new EntityTarget(this.net.ID);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x000220C8 File Offset: 0x000202C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public async void PairWithApp(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		Dictionary<string, string> playerPairingData = CompanionServer.Util.GetPlayerPairingData(player);
		playerPairingData.Add("entityId", this.net.ID.Value.ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityType", ((int)this.Type).ToString("G", CultureInfo.InvariantCulture));
		playerPairingData.Add("entityName", base.GetDisplayName());
		NotificationSendResult notificationSendResult = await CompanionServer.Util.SendPairNotification("entity", player, base.GetDisplayName(), "Tap to pair with this device.", playerPairingData);
		if (notificationSendResult == NotificationSendResult.Sent)
		{
			this.OnPairedWithPlayer(msg.player);
		}
		else
		{
			player.ClientRPCPlayer<int>(null, player, "HandleCompanionPairingResult", (int)notificationSendResult);
		}
	}

	// Token: 0x0600015A RID: 346 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnPairedWithPlayer(global::BasePlayer player)
	{
	}
}
