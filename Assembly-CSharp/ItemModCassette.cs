using System;

// Token: 0x0200039E RID: 926
public class ItemModCassette : ItemModAssociatedEntity<Cassette>
{
	// Token: 0x0400198C RID: 6540
	public int noteSpriteIndex;

	// Token: 0x0400198D RID: 6541
	public PreloadedCassetteContent PreloadedContent;

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x0600207E RID: 8318 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x0600207F RID: 8319 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowHeldEntityParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x000D6FF3 File Offset: 0x000D51F3
	protected override void OnAssociatedItemCreated(Cassette ent)
	{
		base.OnAssociatedItemCreated(ent);
		ent.AssignPreloadContent();
	}
}
