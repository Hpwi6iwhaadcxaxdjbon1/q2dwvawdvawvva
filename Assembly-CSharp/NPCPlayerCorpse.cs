using System;

// Token: 0x020001B2 RID: 434
public class NPCPlayerCorpse : PlayerCorpse
{
	// Token: 0x0400117D RID: 4477
	private bool lootEnabled;

	// Token: 0x060018D5 RID: 6357 RVA: 0x000B7D12 File Offset: 0x000B5F12
	public override float GetRemovalTime()
	{
		return 600f;
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x000B7D19 File Offset: 0x000B5F19
	public override bool CanLoot()
	{
		return this.lootEnabled;
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x000B7D21 File Offset: 0x000B5F21
	public void SetLootableIn(float when)
	{
		base.Invoke(new Action(this.EnableLooting), when);
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x000B7D36 File Offset: 0x000B5F36
	public void EnableLooting()
	{
		this.lootEnabled = true;
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x000B7D3F File Offset: 0x000B5F3F
	protected override bool CanLootContainer(ItemContainer c, int index)
	{
		return index != 1 && index != 2 && base.CanLootContainer(c, index);
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x000B7D53 File Offset: 0x000B5F53
	protected override void PreDropItems()
	{
		base.PreDropItems();
		if (this.containers != null && this.containers.Length >= 2)
		{
			this.containers[1].Clear();
			ItemManager.DoRemoves();
		}
	}
}
