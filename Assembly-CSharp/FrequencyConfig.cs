using System;
using UnityEngine.UI;

// Token: 0x020004DD RID: 1245
public class FrequencyConfig : UIDialog
{
	// Token: 0x0400208B RID: 8331
	[NonSerialized]
	private IRFObject rfObject;

	// Token: 0x0400208C RID: 8332
	public InputField input;

	// Token: 0x0400208D RID: 8333
	public int target;

	// Token: 0x0400208E RID: 8334
	private ItemContainer tempContainer;

	// Token: 0x0400208F RID: 8335
	private ItemId tempItemID;
}
