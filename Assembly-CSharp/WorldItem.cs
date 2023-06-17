using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000F1 RID: 241
public class WorldItem : global::BaseEntity
{
	// Token: 0x04000D77 RID: 3447
	private bool _isInvokingSendItemUpdate;

	// Token: 0x04000D78 RID: 3448
	[Header("WorldItem")]
	public bool allowPickup = true;

	// Token: 0x04000D79 RID: 3449
	[NonSerialized]
	public global::Item item;

	// Token: 0x04000D7A RID: 3450
	protected float eatSeconds = 10f;

	// Token: 0x04000D7B RID: 3451
	protected float caloriesPerSecond = 1f;

	// Token: 0x0600152D RID: 5421 RVA: 0x000A7D70 File Offset: 0x000A5F70
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WorldItem.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778075470U, "Pickup", this, player, 3f))
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
							this.Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x000A7ED8 File Offset: 0x000A60D8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x000A7EFA File Offset: 0x000A60FA
	private void DoItemNetworking()
	{
		if (this._isInvokingSendItemUpdate)
		{
			return;
		}
		this._isInvokingSendItemUpdate = true;
		base.Invoke(new Action(this.SendItemUpdate), 0.1f);
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x000A7F24 File Offset: 0x000A6124
	private void SendItemUpdate()
	{
		this._isInvokingSendItemUpdate = false;
		if (this.item == null)
		{
			return;
		}
		using (UpdateItem updateItem = Facepunch.Pool.Get<UpdateItem>())
		{
			updateItem.item = this.item.Save(false, false);
			base.ClientRPC<UpdateItem>(null, "UpdateItem", updateItem);
		}
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000A7F84 File Offset: 0x000A6184
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.item == null)
		{
			return;
		}
		if (!this.allowPickup)
		{
			return;
		}
		base.ClientRPC(null, "PickupSound");
		global::Item item = this.item;
		Analytics.Azure.OnItemPickup(msg.player, this);
		this.RemoveItem();
		msg.player.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		msg.player.SignalBroadcast(global::BaseEntity.Signal.Gesture, "pickup_item", null);
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x000A7FF8 File Offset: 0x000A61F8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.item == null)
		{
			return;
		}
		bool forDisk = info.forDisk;
		info.msg.worldItem = Facepunch.Pool.Get<ProtoBuf.WorldItem>();
		info.msg.worldItem.item = this.item.Save(forDisk, false);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x000A8049 File Offset: 0x000A6249
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.DestroyItem();
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x000A8057 File Offset: 0x000A6257
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, this.parentBone, false, false);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x000A8068 File Offset: 0x000A6268
	public override global::Item GetItem()
	{
		return this.item;
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x000A8070 File Offset: 0x000A6270
	public void InitializeItem(global::Item in_item)
	{
		if (this.item != null)
		{
			this.RemoveItem();
		}
		this.item = in_item;
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty += this.OnItemDirty;
		base.name = this.item.info.shortname + " (world)";
		this.item.SetWorldEntity(this);
		this.OnItemDirty(this.item);
	}

	// Token: 0x06001537 RID: 5431 RVA: 0x000A80EB File Offset: 0x000A62EB
	public void RemoveItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item = null;
	}

	// Token: 0x06001538 RID: 5432 RVA: 0x000A8115 File Offset: 0x000A6315
	public void DestroyItem()
	{
		if (this.item == null)
		{
			return;
		}
		this.item.OnDirty -= this.OnItemDirty;
		this.item.Remove(0f);
		this.item = null;
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000A814F File Offset: 0x000A634F
	protected virtual void OnItemDirty(global::Item in_item)
	{
		Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (this.item != null)
		{
			base.BroadcastMessage("OnItemChanged", this.item, SendMessageOptions.DontRequireReceiver);
		}
		this.DoItemNetworking();
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000A8184 File Offset: 0x000A6384
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem == null)
		{
			return;
		}
		if (info.msg.worldItem.item == null)
		{
			return;
		}
		global::Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
		if (item != null)
		{
			this.InitializeItem(item);
		}
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x0600153B RID: 5435 RVA: 0x000A81E5 File Offset: 0x000A63E5
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			if (this.item != null)
			{
				return this.item.Traits;
			}
			return base.Traits;
		}
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x000A8204 File Offset: 0x000A6404
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (this.eatSeconds <= 0f)
		{
			return;
		}
		this.eatSeconds -= timeSpent;
		baseNpc.AddCalories(this.caloriesPerSecond * timeSpent);
		if (this.eatSeconds < 0f)
		{
			this.DestroyItem();
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600153D RID: 5437 RVA: 0x000A8258 File Offset: 0x000A6458
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				string format = "{1}[{0}] {2}";
				Networkable net = this.net;
				this._name = string.Format(format, (net != null) ? net.ID : default(NetworkableId), base.ShortPrefabName, this.IsUnityNull<global::WorldItem>() ? "NULL" : base.name);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}
}
