using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006C RID: 108
public class DroppedItemContainer : BaseCombatEntity, LootPanel.IHasLootPanel, IContainerSounds, ILootableEntity
{
	// Token: 0x040006F0 RID: 1776
	public string lootPanelName = "generic";

	// Token: 0x040006F1 RID: 1777
	public int maxItemCount = 36;

	// Token: 0x040006F2 RID: 1778
	[NonSerialized]
	public ulong playerSteamID;

	// Token: 0x040006F3 RID: 1779
	[NonSerialized]
	public string _playerName;

	// Token: 0x040006F4 RID: 1780
	public bool ItemBasedDespawn;

	// Token: 0x040006F5 RID: 1781
	public bool onlyOwnerLoot;

	// Token: 0x040006F6 RID: 1782
	public SoundDefinition openSound;

	// Token: 0x040006F7 RID: 1783
	public SoundDefinition closeSound;

	// Token: 0x040006F9 RID: 1785
	public global::ItemContainer inventory;

	// Token: 0x06000AA0 RID: 2720 RVA: 0x000612E4 File Offset: 0x0005F4E4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
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
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000AA1 RID: 2721 RVA: 0x0006144C File Offset: 0x0005F64C
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x00061459 File Offset: 0x0005F659
	// (set) Token: 0x06000AA3 RID: 2723 RVA: 0x00061472 File Offset: 0x0005F672
	public string playerName
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

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000AA4 RID: 2724 RVA: 0x0006147B File Offset: 0x0005F67B
	// (set) Token: 0x06000AA5 RID: 2725 RVA: 0x00061483 File Offset: 0x0005F683
	public ulong LastLootedBy { get; set; }

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0006148C File Offset: 0x0005F68C
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && (!this.onlyOwnerLoot || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x000614C5 File Offset: 0x0005F6C5
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x000614D3 File Offset: 0x0005F6D3
	public void RemoveMe()
	{
		if (base.IsOpen())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x000614EC File Offset: 0x0005F6EC
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			base.Invoke(new Action(this.RemoveMe), dur);
		}
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00061534 File Offset: 0x0005F734
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.CalculateRemovalTime());
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00061544 File Offset: 0x0005F744
	public float CalculateRemovalTime()
	{
		if (!this.ItemBasedDespawn)
		{
			return ConVar.Server.itemdespawn * 16f * ConVar.Server.itemdespawn_container_scale;
		}
		float num = ConVar.Server.itemdespawn_quick;
		if (this.inventory != null)
		{
			foreach (global::Item item in this.inventory.itemList)
			{
				num = Mathf.Max(num, item.GetDespawnDuration());
			}
		}
		return num;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x000615CC File Offset: 0x0005F7CC
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x000615F0 File Offset: 0x0005F7F0
	public void TakeFrom(global::ItemContainer[] source, float destroyPercent = 0f)
	{
		Assert.IsTrue(this.inventory == null, "Initializing Twice");
		using (TimeWarning.New("DroppedItemContainer.TakeFrom", 0))
		{
			int num = 0;
			foreach (global::ItemContainer itemContainer in source)
			{
				num += itemContainer.itemList.Count;
			}
			this.inventory = new global::ItemContainer();
			this.inventory.ServerInitialize(null, Mathf.Min(num, this.maxItemCount));
			this.inventory.GiveUID();
			this.inventory.entityOwner = this;
			this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
			List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
			for (int i = 0; i < source.Length; i++)
			{
				global::Item[] array = source[i].itemList.ToArray();
				int j = 0;
				while (j < array.Length)
				{
					global::Item item = array[j];
					if (destroyPercent <= 0f)
					{
						goto IL_F9;
					}
					if (item.amount != 1)
					{
						item.amount = Mathf.CeilToInt((float)item.amount * (1f - destroyPercent));
						goto IL_F9;
					}
					list.Add(item);
					IL_125:
					j++;
					continue;
					IL_F9:
					if (!item.MoveToContainer(this.inventory, -1, true, false, null, true))
					{
						item.DropAndTossUpwards(base.transform.position, 2f);
						goto IL_125;
					}
					goto IL_125;
				}
			}
			if (list.Count > 0)
			{
				int num2 = Mathf.FloorToInt((float)list.Count * destroyPercent);
				int num3 = Mathf.Max(0, list.Count - num2);
				list.Shuffle((uint)UnityEngine.Random.Range(0, int.MaxValue));
				for (int k = 0; k < num3; k++)
				{
					global::Item item2 = list[k];
					if (!item2.MoveToContainer(this.inventory, -1, true, false, null, true))
					{
						item2.DropAndTossUpwards(base.transform.position, 2f);
					}
				}
			}
			Facepunch.Pool.FreeList<global::Item>(ref list);
			this.ResetRemovalTime();
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x000617F8 File Offset: 0x0005F9F8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00061884 File Offset: 0x0005FA84
	public void PlayerStoppedLooting(global::BasePlayer player)
	{
		if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		this.ResetRemovalTime();
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x000618D7 File Offset: 0x0005FAD7
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.ServerInitialize(null, 0);
		this.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00061914 File Offset: 0x0005FB14
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000619B8 File Offset: 0x0005FBB8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				return;
			}
			Debug.LogWarning("Dropped item container without inventory: " + this.ToString());
		}
	}
}
