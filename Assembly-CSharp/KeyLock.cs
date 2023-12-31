﻿using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008D RID: 141
public class KeyLock : BaseLock
{
	// Token: 0x0400087B RID: 2171
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	// Token: 0x0400087C RID: 2172
	private int keyCode;

	// Token: 0x0400087D RID: 2173
	private bool firstKeyCreated;

	// Token: 0x06000D31 RID: 3377 RVA: 0x00071214 File Offset: 0x0006F414
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("KeyLock.OnRpcMessage", 0))
		{
			if (rpc == 4135414453U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_CreateKey ");
				}
				using (TimeWarning.New("RPC_CreateKey", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4135414453U, "RPC_CreateKey", this, player, 3f))
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
							this.RPC_CreateKey(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_CreateKey");
					}
				}
				return true;
			}
			if (rpc == 954115386U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lock ");
				}
				using (TimeWarning.New("RPC_Lock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(954115386U, "RPC_Lock", this, player, 3f))
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
							this.RPC_Lock(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Lock");
					}
				}
				return true;
			}
			if (rpc == 1663222372U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Unlock ");
				}
				using (TimeWarning.New("RPC_Unlock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1663222372U, "RPC_Unlock", this, player, 3f))
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
							this.RPC_Unlock(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Unlock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x00071670 File Offset: 0x0006F870
	public override bool HasLockPermission(global::BasePlayer player)
	{
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		foreach (global::Item key in player.inventory.FindItemIDs(this.keyItemType.itemid))
		{
			if (this.CanKeyUnlockUs(key))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x000716F8 File Offset: 0x0006F8F8
	private bool CanKeyUnlockUs(global::Item key)
	{
		return key.instanceData != null && key.instanceData.dataInt == this.keyCode;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x0007171A File Offset: 0x0006F91A
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			this.keyCode = info.msg.keyLock.code;
		}
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00071746 File Offset: 0x0006F946
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.OwnerID == 0UL && base.GetParentEntity())
		{
			base.OwnerID = base.GetParentEntity().OwnerID;
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00071774 File Offset: 0x0006F974
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.keyLock = Facepunch.Pool.Get<ProtoBuf.KeyLock>();
			info.msg.keyLock.code = this.keyCode;
		}
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x000717AB File Offset: 0x0006F9AB
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.keyCode = UnityEngine.Random.Range(1, 100000);
		this.Lock(deployedBy);
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x000717CE File Offset: 0x0006F9CE
	public override bool OnTryToOpen(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x000717CE File Offset: 0x0006F9CE
	public override bool OnTryToClose(global::BasePlayer player)
	{
		return this.HasLockPermission(player) || !base.IsLocked();
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x000717E4 File Offset: 0x0006F9E4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Unlock(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(rpc.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x0007181E File Offset: 0x0006FA1E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Lock(global::BaseEntity.RPCMessage rpc)
	{
		this.Lock(rpc.player);
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x0007182C File Offset: 0x0006FA2C
	private void Lock(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (!this.HasLockPermission(player))
		{
			return;
		}
		this.LockLock(player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00071864 File Offset: 0x0006FA64
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_CreateKey(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked() && !this.HasLockPermission(rpc.player))
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.keyItemType.itemid);
		if (itemDefinition == null)
		{
			Debug.LogWarning("RPC_CreateKey: Itemdef is missing! " + this.keyItemType);
			return;
		}
		ItemBlueprint bp = ItemManager.FindBlueprint(itemDefinition);
		if (rpc.player.inventory.crafting.CanCraft(bp, 1, false))
		{
			ProtoBuf.Item.InstanceData instanceData = Facepunch.Pool.Get<ProtoBuf.Item.InstanceData>();
			instanceData.dataInt = this.keyCode;
			rpc.player.inventory.crafting.CraftItem(bp, rpc.player, instanceData, 1, 0, null, false);
			if (!this.firstKeyCreated)
			{
				this.LockLock(rpc.player);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.firstKeyCreated = true;
			}
			return;
		}
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x0007193E File Offset: 0x0006FB3E
	public void LockLock(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		if (player.IsValid())
		{
			player.GiveAchievement("LOCK_LOCK");
		}
	}
}
