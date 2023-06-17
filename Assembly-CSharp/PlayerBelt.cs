using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000443 RID: 1091
public class PlayerBelt
{
	// Token: 0x04001CA8 RID: 7336
	public static int SelectedSlot = -1;

	// Token: 0x04001CA9 RID: 7337
	protected BasePlayer player;

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06002474 RID: 9332 RVA: 0x000E7DA0 File Offset: 0x000E5FA0
	public static int MaxBeltSlots
	{
		get
		{
			return 6;
		}
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000E7DA3 File Offset: 0x000E5FA3
	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000E7DB4 File Offset: 0x000E5FB4
	public void DropActive(Vector3 position, Vector3 velocity)
	{
		Item activeItem = this.player.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		using (TimeWarning.New("PlayerBelt.DropActive", 0))
		{
			DroppedItem droppedItem = activeItem.Drop(position, velocity, default(Quaternion)) as DroppedItem;
			if (droppedItem != null)
			{
				droppedItem.DropReason = DroppedItem.DropReasonEnum.Death;
				droppedItem.DroppedBy = this.player.userID;
				Analytics.Azure.OnItemDropped(this.player, droppedItem, DroppedItem.DropReasonEnum.Death);
			}
			this.player.svActiveItemID = default(ItemId);
			this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000E7E5C File Offset: 0x000E605C
	public Item GetItemInSlot(int slot)
	{
		if (this.player == null)
		{
			return null;
		}
		if (this.player.inventory == null)
		{
			return null;
		}
		if (this.player.inventory.containerBelt == null)
		{
			return null;
		}
		return this.player.inventory.containerBelt.GetSlot(slot);
	}
}
