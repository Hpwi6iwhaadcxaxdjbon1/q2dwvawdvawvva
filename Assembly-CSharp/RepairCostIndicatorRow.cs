using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B2 RID: 2226
public class RepairCostIndicatorRow : MonoBehaviour
{
	// Token: 0x040031E7 RID: 12775
	public RustText ItemName;

	// Token: 0x040031E8 RID: 12776
	public Image ItemSprite;

	// Token: 0x040031E9 RID: 12777
	public RustText Amount;

	// Token: 0x040031EA RID: 12778
	public RectTransform FillRect;

	// Token: 0x040031EB RID: 12779
	public Image BackgroundImage;

	// Token: 0x040031EC RID: 12780
	public Color OkColour;

	// Token: 0x040031ED RID: 12781
	public Color MissingColour;
}
