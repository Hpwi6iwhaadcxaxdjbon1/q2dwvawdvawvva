using System;

// Token: 0x02000179 RID: 377
public class ItemModPhoto : ItemModAssociatedEntity<PhotoEntity>
{
	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x0600178E RID: 6030 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}
}
