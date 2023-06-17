using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000134 RID: 308
public class VendingMachineScreen : MonoBehaviour
{
	// Token: 0x04000EF8 RID: 3832
	public RawImage largeIcon;

	// Token: 0x04000EF9 RID: 3833
	public RawImage blueprintIcon;

	// Token: 0x04000EFA RID: 3834
	public Text mainText;

	// Token: 0x04000EFB RID: 3835
	public Text lowerText;

	// Token: 0x04000EFC RID: 3836
	public Text centerText;

	// Token: 0x04000EFD RID: 3837
	public RawImage smallIcon;

	// Token: 0x04000EFE RID: 3838
	public VendingMachine vendingMachine;

	// Token: 0x04000EFF RID: 3839
	public Sprite outOfStockSprite;

	// Token: 0x04000F00 RID: 3840
	public Renderer fadeoutMesh;

	// Token: 0x04000F01 RID: 3841
	public CanvasGroup screenCanvas;

	// Token: 0x04000F02 RID: 3842
	public Renderer light1;

	// Token: 0x04000F03 RID: 3843
	public Renderer light2;

	// Token: 0x04000F04 RID: 3844
	public float nextImageTime;

	// Token: 0x04000F05 RID: 3845
	public int currentImageIndex;

	// Token: 0x02000C23 RID: 3107
	public enum vmScreenState
	{
		// Token: 0x0400423A RID: 16954
		ItemScroll,
		// Token: 0x0400423B RID: 16955
		Vending,
		// Token: 0x0400423C RID: 16956
		Message,
		// Token: 0x0400423D RID: 16957
		ShopName,
		// Token: 0x0400423E RID: 16958
		OutOfStock
	}
}
