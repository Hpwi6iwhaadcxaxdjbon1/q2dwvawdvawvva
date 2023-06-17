using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
public class ItemModCookable : ItemMod
{
	// Token: 0x040024CE RID: 9422
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	// Token: 0x040024CF RID: 9423
	public float cookTime = 30f;

	// Token: 0x040024D0 RID: 9424
	public int amountOfBecome = 1;

	// Token: 0x040024D1 RID: 9425
	public int lowTemp;

	// Token: 0x040024D2 RID: 9426
	public int highTemp;

	// Token: 0x040024D3 RID: 9427
	public bool setCookingFlag;

	// Token: 0x06002D3F RID: 11583 RVA: 0x00110B24 File Offset: 0x0010ED24
	public void OnValidate()
	{
		if (this.amountOfBecome < 1)
		{
			this.amountOfBecome = 1;
		}
		if (this.becomeOnCooked == null)
		{
			Debug.LogWarning("[ItemModCookable] becomeOnCooked is unset! [" + base.name + "]", base.gameObject);
		}
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x00110B64 File Offset: 0x0010ED64
	public bool CanBeCookedByAtTemperature(float temperature)
	{
		return temperature > (float)this.lowTemp && temperature < (float)this.highTemp;
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x00110B7C File Offset: 0x0010ED7C
	private void CycleCooking(Item item, float delta)
	{
		if (!this.CanBeCookedByAtTemperature(item.temperature) || item.cookTimeLeft < 0f)
		{
			if (this.setCookingFlag && item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, false);
				item.MarkDirty();
			}
			return;
		}
		if (this.setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
		{
			item.SetFlag(Item.Flag.Cooking, true);
			item.MarkDirty();
		}
		item.cookTimeLeft -= delta;
		if (item.cookTimeLeft > 0f)
		{
			item.MarkDirty();
			return;
		}
		float num = item.cookTimeLeft * -1f;
		int num2 = 1 + Mathf.FloorToInt(num / this.cookTime);
		item.cookTimeLeft = this.cookTime - num % this.cookTime;
		BaseOven baseOven = item.GetEntityOwner() as BaseOven;
		num2 = Mathf.Min(num2, item.amount);
		if (item.amount > num2)
		{
			item.amount -= num2;
			item.MarkDirty();
		}
		else
		{
			item.Remove(0f);
		}
		Analytics.Azure.AddPendingItems(baseOven, item.info.shortname, num2, "smelt", true, false);
		if (this.becomeOnCooked != null)
		{
			Item item2 = ItemManager.Create(this.becomeOnCooked, this.amountOfBecome * num2, 0UL);
			Analytics.Azure.AddPendingItems(baseOven, item2.info.shortname, item2.amount, "smelt", false, false);
			if (item2 != null && !item2.MoveToContainer(item.parent, -1, true, false, null, true) && !item2.MoveToContainer(item.parent, -1, true, false, null, true))
			{
				item2.Drop(item.parent.dropPosition, item.parent.dropVelocity, default(Quaternion));
				if (item.parent.entityOwner && baseOven != null)
				{
					baseOven.OvenFull();
				}
			}
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x00110D4D File Offset: 0x0010EF4D
	public override void OnItemCreated(Item itemcreated)
	{
		itemcreated.cookTimeLeft = this.cookTime;
		itemcreated.onCycle += this.CycleCooking;
	}
}
