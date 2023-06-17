using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000864 RID: 2148
public class LookAtIOEnt : MonoBehaviour
{
	// Token: 0x0400303C RID: 12348
	public Text objectTitle;

	// Token: 0x0400303D RID: 12349
	public RectTransform slotToolTip;

	// Token: 0x0400303E RID: 12350
	public Text slotTitle;

	// Token: 0x0400303F RID: 12351
	public Text slotConnection;

	// Token: 0x04003040 RID: 12352
	public Text slotPower;

	// Token: 0x04003041 RID: 12353
	public Text powerText;

	// Token: 0x04003042 RID: 12354
	public Text passthroughText;

	// Token: 0x04003043 RID: 12355
	public Text chargeLeftText;

	// Token: 0x04003044 RID: 12356
	public Text capacityText;

	// Token: 0x04003045 RID: 12357
	public Text maxOutputText;

	// Token: 0x04003046 RID: 12358
	public Text activeOutputText;

	// Token: 0x04003047 RID: 12359
	public IOEntityUISlotEntry[] inputEntries;

	// Token: 0x04003048 RID: 12360
	public IOEntityUISlotEntry[] outputEntries;

	// Token: 0x04003049 RID: 12361
	public Color NoPowerColor;

	// Token: 0x0400304A RID: 12362
	public GameObject GravityWarning;

	// Token: 0x0400304B RID: 12363
	public GameObject DistanceWarning;

	// Token: 0x0400304C RID: 12364
	public GameObject LineOfSightWarning;

	// Token: 0x0400304D RID: 12365
	public GameObject TooManyInputsWarning;

	// Token: 0x0400304E RID: 12366
	public GameObject TooManyOutputsWarning;

	// Token: 0x0400304F RID: 12367
	public GameObject BuildPrivilegeWarning;

	// Token: 0x04003050 RID: 12368
	public CanvasGroup group;

	// Token: 0x04003051 RID: 12369
	public LookAtIOEnt.HandleSet[] handleSets;

	// Token: 0x04003052 RID: 12370
	public RectTransform clearNotification;

	// Token: 0x04003053 RID: 12371
	public CanvasGroup wireInfoGroup;

	// Token: 0x04003054 RID: 12372
	public Text wireLengthText;

	// Token: 0x04003055 RID: 12373
	public Text wireClipsText;

	// Token: 0x04003056 RID: 12374
	public Text errorReasonTextTooFar;

	// Token: 0x04003057 RID: 12375
	public Text errorReasonTextNoSurface;

	// Token: 0x04003058 RID: 12376
	public Text errorShortCircuit;

	// Token: 0x04003059 RID: 12377
	public RawImage ConnectionTypeIcon;

	// Token: 0x0400305A RID: 12378
	public Texture ElectricSprite;

	// Token: 0x0400305B RID: 12379
	public Texture FluidSprite;

	// Token: 0x0400305C RID: 12380
	public Texture IndustrialSprite;

	// Token: 0x02000E8E RID: 3726
	[Serializable]
	public struct HandleSet
	{
		// Token: 0x04004C1A RID: 19482
		public IOEntity.IOType ForIO;

		// Token: 0x04004C1B RID: 19483
		public GameObjectRef handlePrefab;

		// Token: 0x04004C1C RID: 19484
		public GameObjectRef handleOccupiedPrefab;

		// Token: 0x04004C1D RID: 19485
		public GameObjectRef selectedHandlePrefab;

		// Token: 0x04004C1E RID: 19486
		public GameObjectRef pluggedHandlePrefab;
	}
}
