using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class ItemModSetFrequency : ItemMod
{
	// Token: 0x0400117E RID: 4478
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x0400117F RID: 4479
	public bool allowArmDisarm;

	// Token: 0x04001180 RID: 4480
	public bool onlyFrequency;

	// Token: 0x04001181 RID: 4481
	public int defaultFrequency = -1;

	// Token: 0x04001182 RID: 4482
	public bool loseConditionOnChange;

	// Token: 0x04001183 RID: 4483
	private static List<ItemModSetFrequency.ItemTime> itemsOnCooldown = new List<ItemModSetFrequency.ItemTime>();

	// Token: 0x060018DC RID: 6364 RVA: 0x000B7D88 File Offset: 0x000B5F88
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		if (command.Contains("SetFrequency"))
		{
			if (ItemModSetFrequency.itemsOnCooldown.Count > 0 && this.onlyFrequency)
			{
				for (int i = ItemModSetFrequency.itemsOnCooldown.Count - 1; i >= 0; i--)
				{
					if (ItemModSetFrequency.itemsOnCooldown[i].TargetItem == item && ItemModSetFrequency.itemsOnCooldown[i].TimeSinceEdit < 2f)
					{
						return;
					}
					if (ItemModSetFrequency.itemsOnCooldown[i].TimeSinceEdit > 2f)
					{
						ItemModSetFrequency.itemsOnCooldown.RemoveAt(i);
					}
				}
			}
			int num = 0;
			if (int.TryParse(command.Substring(command.IndexOf(":") + 1), out num))
			{
				global::BaseEntity heldEntity = item.GetHeldEntity();
				Detonator detonator;
				if (heldEntity != null && (detonator = (heldEntity as Detonator)) != null)
				{
					detonator.ServerSetFrequency(player, num);
				}
				else
				{
					item.instanceData.dataInt = num;
					if (this.loseConditionOnChange)
					{
						item.LoseCondition(item.maxCondition * 0.01f);
					}
					item.MarkDirty();
				}
				if (this.onlyFrequency)
				{
					ItemModSetFrequency.itemsOnCooldown.Add(new ItemModSetFrequency.ItemTime
					{
						TargetItem = item,
						TimeSinceEdit = 0f
					});
				}
			}
			else
			{
				Debug.Log("Parse fuckup");
			}
		}
		if (!this.onlyFrequency)
		{
			if (command == "rf_on")
			{
				item.SetFlag(global::Item.Flag.IsOn, true);
				item.MarkDirty();
				return;
			}
			if (command == "rf_off")
			{
				item.SetFlag(global::Item.Flag.IsOn, false);
				item.MarkDirty();
			}
		}
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x000B7F22 File Offset: 0x000B6122
	public override void OnItemCreated(global::Item item)
	{
		if (item.instanceData == null)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.ShouldPool = false;
			item.instanceData.dataInt = this.defaultFrequency;
		}
	}

	// Token: 0x02000C38 RID: 3128
	private struct ItemTime
	{
		// Token: 0x0400428D RID: 17037
		public global::Item TargetItem;

		// Token: 0x0400428E RID: 17038
		public TimeSince TimeSinceEdit;
	}
}
