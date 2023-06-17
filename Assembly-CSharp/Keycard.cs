using System;

// Token: 0x020003B3 RID: 947
public class Keycard : AttackEntity
{
	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06002114 RID: 8468 RVA: 0x000D93AC File Offset: 0x000D75AC
	public int accessLevel
	{
		get
		{
			Item item = this.GetItem();
			if (item == null)
			{
				return 0;
			}
			ItemModKeycard component = item.info.GetComponent<ItemModKeycard>();
			if (component == null)
			{
				return 0;
			}
			return component.accessLevel;
		}
	}
}
