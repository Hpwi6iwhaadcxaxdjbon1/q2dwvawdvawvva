using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000093 RID: 147
public class Locker : StorageContainer
{
	// Token: 0x040008B5 RID: 2229
	public GameObjectRef equipSound;

	// Token: 0x040008B6 RID: 2230
	private const int maxGearSets = 3;

	// Token: 0x040008B7 RID: 2231
	private const int attireSize = 7;

	// Token: 0x040008B8 RID: 2232
	private const int beltSize = 6;

	// Token: 0x040008B9 RID: 2233
	private const int columnSize = 2;

	// Token: 0x040008BA RID: 2234
	private Item[] clothingBuffer = new Item[7];

	// Token: 0x040008BB RID: 2235
	private const int setSize = 13;

	// Token: 0x06000D91 RID: 3473 RVA: 0x00073880 File Offset: 0x00071A80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Locker.OnRpcMessage", 0))
		{
			if (rpc == 1799659668U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Equip ");
				}
				using (TimeWarning.New("RPC_Equip", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1799659668U, "RPC_Equip", this, player, 3f))
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
							this.RPC_Equip(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Equip");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool IsEquipping()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x000739E8 File Offset: 0x00071BE8
	private Locker.RowType GetRowType(int slot)
	{
		if (slot == -1)
		{
			return Locker.RowType.Clothing;
		}
		if (slot % 13 >= 7)
		{
			return Locker.RowType.Belt;
		}
		return Locker.RowType.Clothing;
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000739FA File Offset: 0x00071BFA
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00073A10 File Offset: 0x00071C10
	public void ClearEquipping()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00073A20 File Offset: 0x00071C20
	public override bool ItemFilter(Item item, int targetSlot)
	{
		return base.ItemFilter(item, targetSlot) && (item.info.category == ItemCategory.Attire || this.GetRowType(targetSlot) == Locker.RowType.Belt);
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00073A48 File Offset: 0x00071C48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Equip(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num < 0 || num >= 3)
		{
			return;
		}
		if (this.IsEquipping())
		{
			return;
		}
		BasePlayer player = msg.player;
		int num2 = num * 13;
		bool flag = false;
		for (int i = 0; i < player.inventory.containerWear.capacity; i++)
		{
			Item slot = player.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				this.clothingBuffer[i] = slot;
			}
		}
		for (int j = 0; j < 7; j++)
		{
			int num3 = num2 + j;
			Item slot2 = base.inventory.GetSlot(num3);
			Item item = this.clothingBuffer[j];
			if (slot2 != null)
			{
				flag = true;
				if (slot2.info.category != ItemCategory.Attire || !slot2.MoveToContainer(player.inventory.containerWear, j, true, false, null, true))
				{
					slot2.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (item != null)
			{
				flag = true;
				if (item.info.category != ItemCategory.Attire || !item.MoveToContainer(base.inventory, num3, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			this.clothingBuffer[j] = null;
		}
		for (int k = 0; k < 6; k++)
		{
			int num4 = num2 + k + 7;
			int iTargetPos = k;
			Item slot3 = base.inventory.GetSlot(num4);
			Item slot4 = player.inventory.containerBelt.GetSlot(k);
			if (slot4 != null)
			{
				slot4.RemoveFromContainer();
			}
			if (slot3 != null)
			{
				flag = true;
				if (!slot3.MoveToContainer(player.inventory.containerBelt, iTargetPos, true, false, null, true))
				{
					slot3.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
			if (slot4 != null)
			{
				flag = true;
				if (!slot4.MoveToContainer(base.inventory, num4, true, false, null, true))
				{
					slot4.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
			}
		}
		if (flag)
		{
			Effect.server.Run(this.equipSound.resourcePath, player, StringPool.Get("spine3"), Vector3.zero, Vector3.zero, null, false);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			base.Invoke(new Action(this.ClearEquipping), 1.5f);
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00073CB8 File Offset: 0x00071EB8
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		int i = 0;
		while (i < this.inventorySlots)
		{
			Locker.RowType rowType = this.GetRowType(i);
			if (item.info.category == ItemCategory.Attire)
			{
				if (rowType == Locker.RowType.Clothing)
				{
					goto IL_23;
				}
			}
			else if (rowType == Locker.RowType.Belt)
			{
				goto IL_23;
			}
			IL_41:
			i++;
			continue;
			IL_23:
			if (!base.inventory.SlotTaken(item, i) && (rowType != Locker.RowType.Clothing || !this.DoesWearableConflictWithRow(item, i)))
			{
				return i;
			}
			goto IL_41;
		}
		return int.MinValue;
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00073D18 File Offset: 0x00071F18
	private bool DoesWearableConflictWithRow(Item item, int pos)
	{
		int num = pos / 13 * 13;
		ItemModWearable itemModWearable = item.info.ItemModWearable;
		if (itemModWearable == null)
		{
			return false;
		}
		for (int i = num; i < num + 7; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModWearable itemModWearable2 = slot.info.ItemModWearable;
				if (!(itemModWearable2 == null) && !itemModWearable2.CanExistWith(itemModWearable))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00073D86 File Offset: 0x00071F86
	public Vector2i GetIndustrialSlotRange(Vector3 localPosition)
	{
		if (localPosition.x < -0.3f)
		{
			return new Vector2i(26, 38);
		}
		if (localPosition.x > 0.3f)
		{
			return new Vector2i(0, 12);
		}
		return new Vector2i(13, 25);
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x00073DBE File Offset: 0x00071FBE
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x02000BDE RID: 3038
	private enum RowType
	{
		// Token: 0x04004127 RID: 16679
		Clothing,
		// Token: 0x04004128 RID: 16680
		Belt
	}

	// Token: 0x02000BDF RID: 3039
	public static class LockerFlags
	{
		// Token: 0x04004129 RID: 16681
		public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
	}
}
