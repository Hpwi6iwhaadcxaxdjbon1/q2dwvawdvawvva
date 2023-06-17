using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x020005C1 RID: 1473
public class ItemCraftTask
{
	// Token: 0x040023F8 RID: 9208
	public ItemBlueprint blueprint;

	// Token: 0x040023F9 RID: 9209
	public float endTime;

	// Token: 0x040023FA RID: 9210
	public int taskUID;

	// Token: 0x040023FB RID: 9211
	public global::BasePlayer owner;

	// Token: 0x040023FC RID: 9212
	public bool cancelled;

	// Token: 0x040023FD RID: 9213
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x040023FE RID: 9214
	public int amount = 1;

	// Token: 0x040023FF RID: 9215
	public int skinID;

	// Token: 0x04002400 RID: 9216
	public List<ulong> potentialOwners;

	// Token: 0x04002401 RID: 9217
	public List<global::Item> takenItems;

	// Token: 0x04002402 RID: 9218
	public int numCrafted;

	// Token: 0x04002403 RID: 9219
	public float conditionScale = 1f;

	// Token: 0x04002404 RID: 9220
	public float workSecondsComplete;

	// Token: 0x04002405 RID: 9221
	public float worksecondsRequired;

	// Token: 0x04002406 RID: 9222
	public global::BaseEntity workbenchEntity;
}
