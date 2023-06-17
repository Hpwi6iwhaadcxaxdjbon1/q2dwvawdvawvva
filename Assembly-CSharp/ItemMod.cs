using System;
using UnityEngine;

// Token: 0x020005CF RID: 1487
public class ItemMod : MonoBehaviour
{
	// Token: 0x04002495 RID: 9365
	protected ItemMod[] siblingMods;

	// Token: 0x06002CE6 RID: 11494 RVA: 0x0010FBB4 File Offset: 0x0010DDB4
	public virtual void ModInit()
	{
		this.siblingMods = base.GetComponents<ItemMod>();
	}

	// Token: 0x06002CE7 RID: 11495 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnItemCreated(Item item)
	{
	}

	// Token: 0x06002CE8 RID: 11496 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnVirginItem(Item item)
	{
	}

	// Token: 0x06002CE9 RID: 11497 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerCommand(Item item, string command, BasePlayer player)
	{
	}

	// Token: 0x06002CEA RID: 11498 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06002CEB RID: 11499 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnRemove(Item item)
	{
	}

	// Token: 0x06002CEC RID: 11500 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnParentChanged(Item item)
	{
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAttacked(Item item, HitInfo info)
	{
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnChanged(Item item)
	{
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x0010FBC4 File Offset: 0x0010DDC4
	public virtual bool CanDoAction(Item item, BasePlayer player)
	{
		ItemMod[] array = this.siblingMods;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Passes(item))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool Passes(Item item)
	{
		return true;
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnRemovedFromWorld(Item item)
	{
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnMovedToWorld(Item item)
	{
	}
}
