using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000091 RID: 145
public class LiquidVessel : HeldEntity
{
	// Token: 0x06000D75 RID: 3445 RVA: 0x00072BCC File Offset: 0x00070DCC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidVessel.OnRpcMessage", 0))
		{
			if (rpc == 4034725537U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoEmpty ");
				}
				using (TimeWarning.New("DoEmpty", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4034725537U, "DoEmpty", this, player))
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
							this.DoEmpty(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoEmpty");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00072D30 File Offset: 0x00070F30
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00072D98 File Offset: 0x00070F98
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoEmpty(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		using (List<Item>.Enumerator enumerator = item.contents.itemList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.UseItem(50);
			}
		}
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00072E28 File Offset: 0x00071028
	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			Item item3 = ItemManager.Create(liquidType, amount, 0UL);
			if (item3 != null)
			{
				item3.MoveToContainer(item.contents, -1, true, false, null, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
			if (itemDefinition != item2.info)
			{
				item2.Remove(0f);
				item2 = ItemManager.Create(itemDefinition, num, 0UL);
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
			else
			{
				item2.amount = num;
			}
			item2.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00072EF4 File Offset: 0x000710F4
	public bool CanFillHere(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (double)ownerPlayer.WaterFactor() > 0.05;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00072F28 File Offset: 0x00071128
	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00072F52 File Offset: 0x00071152
	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00072F63 File Offset: 0x00071163
	public bool IsFull()
	{
		return this.HeldFraction() >= 1f;
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00072F75 File Offset: 0x00071175
	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}
}
