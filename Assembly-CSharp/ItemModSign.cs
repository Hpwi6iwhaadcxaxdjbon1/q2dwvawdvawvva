using System;

// Token: 0x020003D6 RID: 982
public class ItemModSign : ItemModAssociatedEntity<SignContent>
{
	// Token: 0x170002CD RID: 717
	// (get) Token: 0x060021C8 RID: 8648 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x060021C9 RID: 8649 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool ShouldAutoCreateEntity
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000DBD64 File Offset: 0x000D9F64
	public void OnSignPickedUp(ISignage s, IUGCBrowserEntity ugc, Item toItem)
	{
		SignContent signContent = base.CreateAssociatedEntity(toItem);
		if (signContent != null)
		{
			signContent.CopyInfoFromSign(s, ugc);
		}
	}
}
