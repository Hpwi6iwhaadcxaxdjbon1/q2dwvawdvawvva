using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x02000174 RID: 372
[ConsoleSystem.Factory("note")]
public class note : ConsoleSystem
{
	// Token: 0x0600177F RID: 6015 RVA: 0x000B286C File Offset: 0x000B0A6C
	[ServerUserVar]
	public static void update(ConsoleSystem.Arg arg)
	{
		ItemId itemID = arg.GetItemID(0, default(ItemId));
		string @string = arg.GetString(1, "");
		Item item = arg.Player().inventory.FindItemUID(itemID);
		if (item == null)
		{
			return;
		}
		item.text = @string.Truncate(1024, null);
		item.MarkDirty();
	}
}
