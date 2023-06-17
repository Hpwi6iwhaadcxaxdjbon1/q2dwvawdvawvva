using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BA RID: 2234
public class TeamMemberElement : MonoBehaviour
{
	// Token: 0x04003227 RID: 12839
	public RustText nameText;

	// Token: 0x04003228 RID: 12840
	public RawImage icon;

	// Token: 0x04003229 RID: 12841
	public Color onlineColor;

	// Token: 0x0400322A RID: 12842
	public Color offlineColor;

	// Token: 0x0400322B RID: 12843
	public Color deadColor;

	// Token: 0x0400322C RID: 12844
	public Color woundedColor;

	// Token: 0x0400322D RID: 12845
	public GameObject hoverOverlay;

	// Token: 0x0400322E RID: 12846
	public RawImage memberIcon;

	// Token: 0x0400322F RID: 12847
	public RawImage leaderIcon;

	// Token: 0x04003230 RID: 12848
	public RawImage deadIcon;

	// Token: 0x04003231 RID: 12849
	public RawImage woundedIcon;

	// Token: 0x04003232 RID: 12850
	public int teamIndex;
}
