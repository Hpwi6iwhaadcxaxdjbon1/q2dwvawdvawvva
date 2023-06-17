using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000810 RID: 2064
public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged
{
	// Token: 0x04002E4A RID: 11850
	public Image image;

	// Token: 0x04002E4B RID: 11851
	public Image imageFavourite;

	// Token: 0x04002E4C RID: 11852
	public Button button;

	// Token: 0x04002E4D RID: 11853
	public CanvasGroup group;

	// Token: 0x04002E4E RID: 11854
	public GameObject newNotification;

	// Token: 0x04002E4F RID: 11855
	public GameObject lockedOverlay;

	// Token: 0x04002E50 RID: 11856
	public Tooltip Tip;

	// Token: 0x04002E51 RID: 11857
	public Image FavouriteIcon;
}
