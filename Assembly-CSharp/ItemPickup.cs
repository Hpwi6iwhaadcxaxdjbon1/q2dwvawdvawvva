using System;
using Rust;

// Token: 0x020004C1 RID: 1217
public class ItemPickup : DroppedItem
{
	// Token: 0x0400202B RID: 8235
	public ItemDefinition itemDef;

	// Token: 0x0400202C RID: 8236
	public int amount = 1;

	// Token: 0x0400202D RID: 8237
	public ulong skinOverride;

	// Token: 0x060027B3 RID: 10163 RVA: 0x00081B87 File Offset: 0x0007FD87
	public override float GetDespawnDuration()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000F781C File Offset: 0x000F5A1C
	public override void Spawn()
	{
		base.Spawn();
		if (Application.isLoadingSave)
		{
			return;
		}
		Item item = ItemManager.Create(this.itemDef, this.amount, this.skinOverride);
		base.InitializeItem(item);
		item.SetWorldEntity(this);
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000F785D File Offset: 0x000F5A5D
	internal override void DoServerDestroy()
	{
		if (this.item != null)
		{
			this.item.Remove(0f);
			this.item = null;
		}
		base.DoServerDestroy();
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000F7884 File Offset: 0x000F5A84
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.IdleDestroy();
	}
}
