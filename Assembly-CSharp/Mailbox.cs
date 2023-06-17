using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000096 RID: 150
public class Mailbox : StorageContainer
{
	// Token: 0x04000900 RID: 2304
	public string ownerPanel;

	// Token: 0x04000901 RID: 2305
	public GameObjectRef mailDropSound;

	// Token: 0x04000902 RID: 2306
	public ItemDefinition[] allowedItems;

	// Token: 0x04000903 RID: 2307
	public bool autoSubmitWhenClosed;

	// Token: 0x04000904 RID: 2308
	public bool shouldMarkAsFull;

	// Token: 0x06000DD5 RID: 3541 RVA: 0x000754A4 File Offset: 0x000736A4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Mailbox.OnRpcMessage", 0))
		{
			if (rpc == 131727457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Submit ");
				}
				using (TimeWarning.New("RPC_Submit", 0))
				{
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
							this.RPC_Submit(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Submit");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x000755C8 File Offset: 0x000737C8
	public int mailInputSlot
	{
		get
		{
			return this.inventorySlots - 1;
		}
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000755D2 File Offset: 0x000737D2
	public virtual bool PlayerIsOwner(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x000755DA File Offset: 0x000737DA
	public bool IsFull()
	{
		return this.shouldMarkAsFull && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x000755F1 File Offset: 0x000737F1
	public void MarkFull(bool full)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, this.shouldMarkAsFull && full, false, true);
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00075608 File Offset: 0x00073808
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return base.PlayerOpenLoot(player, this.PlayerIsOwner(player) ? this.ownerPanel : panelToOpen, true);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00075624 File Offset: 0x00073824
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (panelName == this.ownerPanel)
		{
			return this.PlayerIsOwner(player) && base.CanOpenLootPanel(player, panelName);
		}
		return this.HasFreeSpace() || !this.shouldMarkAsFull;
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x0007565B File Offset: 0x0007385B
	private bool HasFreeSpace()
	{
		return this.GetFreeSlot() != -1;
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x0007566C File Offset: 0x0007386C
	private int GetFreeSlot()
	{
		for (int i = 0; i < this.mailInputSlot; i++)
		{
			if (base.inventory.GetSlot(i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x0007569B File Offset: 0x0007389B
	public virtual bool MoveItemToStorage(Item item)
	{
		item.RemoveFromContainer();
		return item.MoveToContainer(base.inventory, -1, true, false, null, true);
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x000756BC File Offset: 0x000738BC
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (this.autoSubmitWhenClosed)
		{
			this.SubmitInputItems(player);
		}
		if (this.IsFull())
		{
			Item slot = base.inventory.GetSlot(this.mailInputSlot);
			if (slot != null)
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
		base.PlayerStoppedLooting(player);
		if (this.PlayerIsOwner(player))
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x0007572C File Offset: 0x0007392C
	[BaseEntity.RPC_Server]
	public void RPC_Submit(BaseEntity.RPCMessage msg)
	{
		if (this.IsFull())
		{
			return;
		}
		BasePlayer player = msg.player;
		this.SubmitInputItems(player);
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00075750 File Offset: 0x00073950
	public void SubmitInputItems(BasePlayer fromPlayer)
	{
		Item slot = base.inventory.GetSlot(this.mailInputSlot);
		if (this.IsFull())
		{
			return;
		}
		if (slot != null)
		{
			if (this.MoveItemToStorage(slot))
			{
				if (slot.position != this.mailInputSlot)
				{
					Effect.server.Run(this.mailDropSound.resourcePath, this.GetDropPosition(), default(Vector3), null, false);
					if (fromPlayer != null && !this.PlayerIsOwner(fromPlayer))
					{
						base.SetFlag(BaseEntity.Flags.On, true, false, true);
						return;
					}
				}
			}
			else
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x000757EC File Offset: 0x000739EC
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		this.MarkFull(!this.HasFreeSpace());
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x00075808 File Offset: 0x00073A08
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		bool flag = this.PlayerIsOwner(player);
		if (!flag)
		{
			flag = (item == base.inventory.GetSlot(this.mailInputSlot));
		}
		return flag && base.CanMoveFrom(player, item);
	}

	// Token: 0x06000DE4 RID: 3556 RVA: 0x00075844 File Offset: 0x00073A44
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (this.allowedItems == null || this.allowedItems.Length == 0)
		{
			return base.ItemFilter(item, targetSlot);
		}
		foreach (ItemDefinition y in this.allowedItems)
		{
			if (item.info == y)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x00075895 File Offset: 0x00073A95
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		if (player == null || this.PlayerIsOwner(player))
		{
			return -1;
		}
		return this.mailInputSlot;
	}
}
