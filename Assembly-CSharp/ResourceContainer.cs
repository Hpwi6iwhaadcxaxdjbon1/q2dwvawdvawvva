using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BF RID: 191
public class ResourceContainer : EntityComponent<BaseEntity>
{
	// Token: 0x04000ACA RID: 2762
	public bool lootable = true;

	// Token: 0x04000ACB RID: 2763
	[NonSerialized]
	public ItemContainer container;

	// Token: 0x04000ACC RID: 2764
	[NonSerialized]
	public float lastAccessTime;

	// Token: 0x0600113B RID: 4411 RVA: 0x0008D9EC File Offset: 0x0008BBEC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResourceContainer.OnRpcMessage", 0))
		{
			if (rpc == 548378753U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartLootingContainer ");
				}
				using (TimeWarning.New("StartLootingContainer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(548378753U, "StartLootingContainer", this.GetBaseEntity(), player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.StartLootingContainer(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in StartLootingContainer");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x0600113C RID: 4412 RVA: 0x0008DB5C File Offset: 0x0008BD5C
	public int accessedSecondsAgo
	{
		get
		{
			return (int)(UnityEngine.Time.realtimeSinceStartup - this.lastAccessTime);
		}
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0008DB6C File Offset: 0x0008BD6C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void StartLootingContainer(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.lootable)
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(base.baseEntity, true))
		{
			this.lastAccessTime = UnityEngine.Time.realtimeSinceStartup;
			player.inventory.loot.AddContainer(this.container);
		}
	}
}
