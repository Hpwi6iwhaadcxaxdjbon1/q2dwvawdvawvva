using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000604 RID: 1540
[Serializable]
public class ItemAmount : ISerializationCallbackReceiver
{
	// Token: 0x04002548 RID: 9544
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x04002549 RID: 9545
	public float amount;

	// Token: 0x0400254A RID: 9546
	[NonSerialized]
	public float startAmount;

	// Token: 0x06002D9C RID: 11676 RVA: 0x001125CB File Offset: 0x001107CB
	public ItemAmount(ItemDefinition item = null, float amt = 0f)
	{
		this.itemDef = item;
		this.amount = amt;
		this.startAmount = this.amount;
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x06002D9D RID: 11677 RVA: 0x001125ED File Offset: 0x001107ED
	public int itemid
	{
		get
		{
			if (this.itemDef == null)
			{
				return 0;
			}
			return this.itemDef.itemid;
		}
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x0011260A File Offset: 0x0011080A
	public virtual float GetAmount()
	{
		return this.amount;
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x00112612 File Offset: 0x00110812
	public virtual void OnAfterDeserialize()
	{
		this.startAmount = this.amount;
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnBeforeSerialize()
	{
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x00112620 File Offset: 0x00110820
	public static ItemAmountList SerialiseList(List<ItemAmount> list)
	{
		ItemAmountList itemAmountList = Pool.Get<ItemAmountList>();
		itemAmountList.amount = Pool.GetList<float>();
		itemAmountList.itemID = Pool.GetList<int>();
		foreach (ItemAmount itemAmount in list)
		{
			itemAmountList.amount.Add(itemAmount.amount);
			itemAmountList.itemID.Add(itemAmount.itemid);
		}
		return itemAmountList;
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x001126A8 File Offset: 0x001108A8
	public static void DeserialiseList(List<ItemAmount> target, ItemAmountList source)
	{
		target.Clear();
		if (source.amount.Count != source.itemID.Count)
		{
			return;
		}
		for (int i = 0; i < source.amount.Count; i++)
		{
			target.Add(new ItemAmount(ItemManager.FindItemDefinition(source.itemID[i]), source.amount[i]));
		}
	}
}
