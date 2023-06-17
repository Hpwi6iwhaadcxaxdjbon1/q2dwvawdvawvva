using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006A RID: 106
public class DoorCloser : BaseEntity
{
	// Token: 0x040006EC RID: 1772
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemType;

	// Token: 0x040006ED RID: 1773
	public float delay = 3f;

	// Token: 0x06000A96 RID: 2710 RVA: 0x00060FE0 File Offset: 0x0005F1E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorCloser.OnRpcMessage", 0))
		{
			if (rpc == 342802563U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Take ");
				}
				using (TimeWarning.New("RPC_Take", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(342802563U, "RPC_Take", this, player, 3f))
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
							this.RPC_Take(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Take");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00061148 File Offset: 0x0005F348
	public void Think()
	{
		base.Invoke(new Action(this.SendClose), this.delay);
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00061164 File Offset: 0x0005F364
	public void SendClose()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (this.children != null)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						base.Invoke(new Action(this.SendClose), this.delay);
						return;
					}
				}
			}
		}
		if (parentEntity)
		{
			parentEntity.SendMessage("CloseRequest");
		}
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x000611F4 File Offset: 0x0005F3F4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Take(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!rpc.player.CanBuild())
		{
			return;
		}
		Door door = this.GetDoor();
		if (door == null)
		{
			return;
		}
		if (!door.GetPlayerLockPermission(rpc.player))
		{
			return;
		}
		Item item = ItemManager.Create(this.itemType, 1, this.skinID);
		if (item != null)
		{
			rpc.player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00061267 File Offset: 0x0005F467
	public Door GetDoor()
	{
		return base.GetParentEntity() as Door;
	}
}
