using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000040 RID: 64
public class BaseLock : BaseEntity
{
	// Token: 0x0400031C RID: 796
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	// Token: 0x06000409 RID: 1033 RVA: 0x00032A6C File Offset: 0x00030C6C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLock.OnRpcMessage", 0))
		{
			if (rpc == 3572556655U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeLock ");
				}
				using (TimeWarning.New("RPC_TakeLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3572556655U, "RPC_TakeLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeLock(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_TakeLock");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00032BD4 File Offset: 0x00030DD4
	public virtual bool GetPlayerLockPermission(BasePlayer player)
	{
		return this.OnTryToOpen(player);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00032BDD File Offset: 0x00030DDD
	public virtual bool OnTryToOpen(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool OnTryToClose(BasePlayer player)
	{
		return true;
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasLockPermission(BasePlayer player)
	{
		return true;
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00032BE8 File Offset: 0x00030DE8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeLock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		Analytics.Azure.OnEntityPickedUp(rpc.player, this);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00032C42 File Offset: 0x00030E42
	public override float BoundsPadding()
	{
		return 2f;
	}
}
