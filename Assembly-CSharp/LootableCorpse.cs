using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000094 RID: 148
public class LootableCorpse : BaseCorpse, LootPanel.IHasLootPanel
{
	// Token: 0x040008BC RID: 2236
	public string lootPanelName = "generic";

	// Token: 0x040008BD RID: 2237
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x040008BE RID: 2238
	[NonSerialized]
	public string _playerName;

	// Token: 0x040008C1 RID: 2241
	[NonSerialized]
	public global::ItemContainer[] containers;

	// Token: 0x040008C2 RID: 2242
	[NonSerialized]
	private bool firstLooted;

	// Token: 0x06000D9E RID: 3486 RVA: 0x00073DE8 File Offset: 0x00071FE8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LootableCorpse.OnRpcMessage", 0))
		{
			if (rpc == 2278459738U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LootCorpse ");
				}
				using (TimeWarning.New("RPC_LootCorpse", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2278459738U, "RPC_LootCorpse", this, player, 3f))
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
							this.RPC_LootCorpse(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_LootCorpse");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000D9F RID: 3487 RVA: 0x00073F50 File Offset: 0x00072150
	// (set) Token: 0x06000DA0 RID: 3488 RVA: 0x00073F69 File Offset: 0x00072169
	public virtual string playerName
	{
		get
		{
			return NameHelper.Get(this.playerSteamID, this._playerName, base.isClient);
		}
		set
		{
			this._playerName = value;
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000DA1 RID: 3489 RVA: 0x00073F72 File Offset: 0x00072172
	// (set) Token: 0x06000DA2 RID: 3490 RVA: 0x00073F7A File Offset: 0x0007217A
	public virtual string streamerName { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x00073F83 File Offset: 0x00072183
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00073F90 File Offset: 0x00072190
	public Translate.Phrase LootPanelName
	{
		get
		{
			return "N/A";
		}
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00073F9C File Offset: 0x0007219C
	public override void ResetState()
	{
		this.firstLooted = false;
		base.ResetState();
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00073FAB File Offset: 0x000721AB
	// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00073FB3 File Offset: 0x000721B3
	public bool blockBagDrop { get; set; }

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00073FBC File Offset: 0x000721BC
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00073FC4 File Offset: 0x000721C4
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!this.blockBagDrop)
		{
			this.PreDropItems();
			this.DropItems();
		}
		this.blockBagDrop = false;
		if (this.containers != null)
		{
			global::ItemContainer[] array = this.containers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill();
			}
		}
		this.containers = null;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x00074020 File Offset: 0x00072220
	public void TakeFrom(params global::ItemContainer[] source)
	{
		Assert.IsTrue(this.containers == null, "Initializing Twice");
		using (TimeWarning.New("Corpse.TakeFrom", 0))
		{
			this.containers = new global::ItemContainer[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				this.containers[i] = new global::ItemContainer();
				this.containers[i].ServerInitialize(null, source[i].capacity);
				this.containers[i].GiveUID();
				this.containers[i].entityOwner = this;
				foreach (global::Item item in source[i].itemList.ToArray())
				{
					if (!item.MoveToContainer(this.containers[i], -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			base.ResetRemovalTime();
		}
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00074120 File Offset: 0x00072320
	public override bool CanRemove()
	{
		return !base.IsOpen();
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanLoot()
	{
		return true;
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x0007412B File Offset: 0x0007232B
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (!this.firstLooted)
		{
			if (this.playerSteamID <= 10000000UL)
			{
				Analytics.Azure.OnFirstLooted(this, baseEntity);
			}
			this.firstLooted = true;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanLootContainer(global::ItemContainer c, int index)
	{
		return true;
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00074158 File Offset: 0x00072358
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LootCorpse(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.CanLoot())
		{
			return;
		}
		if (this.containers == null)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			for (int i = 0; i < this.containers.Length; i++)
			{
				global::ItemContainer itemContainer = this.containers[i];
				if (this.CanLootContainer(itemContainer, i))
				{
					player.inventory.loot.AddContainer(itemContainer);
				}
			}
			player.inventory.loot.SendImmediate();
			base.ClientRPCPlayer(null, player, "RPC_ClientLootCorpse");
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00074208 File Offset: 0x00072408
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void PreDropItems()
	{
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00074224 File Offset: 0x00072424
	public void DropItems()
	{
		if (Global.disableBagDropping)
		{
			return;
		}
		if (this.containers != null)
		{
			DroppedItemContainer droppedItemContainer = global::ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", base.transform.position, Quaternion.identity, this.containers);
			if (droppedItemContainer != null)
			{
				droppedItemContainer.playerName = this.playerName;
				droppedItemContainer.playerSteamID = this.playerSteamID;
			}
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00074284 File Offset: 0x00072484
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		info.msg.lootableCorpse.streamerName = this.streamerName;
		if (info.forDisk && this.containers != null)
		{
			info.msg.lootableCorpse.privateData = Facepunch.Pool.Get<ProtoBuf.LootableCorpse.Private>();
			info.msg.lootableCorpse.privateData.container = Facepunch.Pool.GetList<ProtoBuf.ItemContainer>();
			foreach (global::ItemContainer itemContainer in this.containers)
			{
				if (itemContainer != null)
				{
					ProtoBuf.ItemContainer itemContainer2 = itemContainer.Save();
					if (itemContainer2 != null)
					{
						info.msg.lootableCorpse.privateData.container.Add(itemContainer2);
					}
				}
			}
		}
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x0007436C File Offset: 0x0007256C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.streamerName = info.msg.lootableCorpse.streamerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			if (info.fromDisk && info.msg.lootableCorpse.privateData != null && info.msg.lootableCorpse.privateData.container != null)
			{
				int count = info.msg.lootableCorpse.privateData.container.Count;
				this.containers = new global::ItemContainer[count];
				for (int i = 0; i < count; i++)
				{
					this.containers[i] = new global::ItemContainer();
					this.containers[i].ServerInitialize(null, info.msg.lootableCorpse.privateData.container[i].slots);
					this.containers[i].GiveUID();
					this.containers[i].entityOwner = this;
					this.containers[i].Load(info.msg.lootableCorpse.privateData.container[i]);
				}
			}
		}
	}
}
