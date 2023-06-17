using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A5 RID: 2213
public class PingWidget : MonoBehaviour
{
	// Token: 0x040031AB RID: 12715
	public RectTransform MoveTransform;

	// Token: 0x040031AC RID: 12716
	public RectTransform ScaleTransform;

	// Token: 0x040031AD RID: 12717
	public Image InnerImage;

	// Token: 0x040031AE RID: 12718
	public Image OuterImage;

	// Token: 0x040031AF RID: 12719
	public GameObject TeamLeaderRoot;

	// Token: 0x040031B0 RID: 12720
	public GameObject CancelHoverRoot;

	// Token: 0x040031B1 RID: 12721
	public SoundDefinition PingDeploySoundHostile;

	// Token: 0x040031B2 RID: 12722
	public SoundDefinition PingDeploySoundGoTo;

	// Token: 0x040031B3 RID: 12723
	public SoundDefinition PingDeploySoundDollar;

	// Token: 0x040031B4 RID: 12724
	public SoundDefinition PingDeploySoundLoot;

	// Token: 0x040031B5 RID: 12725
	public SoundDefinition PingDeploySoundNode;

	// Token: 0x040031B6 RID: 12726
	public SoundDefinition PingDeploySoundGun;

	// Token: 0x040031B7 RID: 12727
	public CanvasGroup FadeCanvas;
}
