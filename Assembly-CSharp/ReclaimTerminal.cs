using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B8 RID: 184
public class ReclaimTerminal : StorageContainer
{
	// Token: 0x04000A93 RID: 2707
	public int itemCount;

	// Token: 0x04000A94 RID: 2708
	public static readonly Translate.Phrase DespawnToast = new Translate.Phrase("softcore.reclaimdespawn", "Items remaining in the reclaim terminal will despawn in two hours.");

	// Token: 0x06001095 RID: 4245 RVA: 0x00089084 File Offset: 0x00087284
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReclaimTerminal.OnRpcMessage", 0))
		{
			if (rpc == 2609933020U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ReloadLoot ");
				}
				using (TimeWarning.New("RPC_ReloadLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2609933020U, "RPC_ReloadLoot", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2609933020U, "RPC_ReloadLoot", this, player, 3f))
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
							this.RPC_ReloadLoot(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_ReloadLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x00089244 File Offset: 0x00087444
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00089260 File Offset: 0x00087460
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_ReloadLoot(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		if (player.inventory.loot.entitySource != this)
		{
			return;
		}
		this.LoadReclaimLoot(player);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x000892AC File Offset: 0x000874AC
	public void LoadReclaimLoot(global::BasePlayer player)
	{
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		List<global::ReclaimManager.PlayerReclaimEntry> list = Facepunch.Pool.GetList<global::ReclaimManager.PlayerReclaimEntry>();
		global::ReclaimManager.instance.GetReclaimsForPlayer(player.userID, ref list);
		this.itemCount = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				this.itemCount++;
			}
		}
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in list)
		{
			for (int j = playerReclaimEntry.inventory.itemList.Count - 1; j >= 0; j--)
			{
				global::Item item = playerReclaimEntry.inventory.itemList[j];
				this.itemCount++;
				item.MoveToContainer(base.inventory, -1, true, false, null, true);
			}
		}
		Facepunch.Pool.FreeList<global::ReclaimManager.PlayerReclaimEntry>(ref list);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x000893B4 File Offset: 0x000875B4
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (global::ReclaimManager.instance == null)
		{
			return false;
		}
		this.LoadReclaimLoot(player);
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x000893D8 File Offset: 0x000875D8
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		if (global::ReclaimManager.instance == null)
		{
			return;
		}
		global::ReclaimManager.instance.DoCleanup();
		if (base.inventory.itemList.Count > 0)
		{
			global::ReclaimManager.instance.AddPlayerReclaim(player.userID, base.inventory.itemList, 0UL, "", -1);
			player.ShowToast(GameTip.Styles.Blue_Long, global::ReclaimTerminal.DespawnToast, Array.Empty<string>());
		}
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x0008944C File Offset: 0x0008764C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.reclaimTerminal = Facepunch.Pool.Get<ProtoBuf.ReclaimTerminal>();
			info.msg.reclaimTerminal.itemCount = this.itemCount;
		}
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00089483 File Offset: 0x00087683
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.reclaimTerminal != null)
		{
			this.itemCount = info.msg.reclaimTerminal.itemCount;
		}
	}
}
