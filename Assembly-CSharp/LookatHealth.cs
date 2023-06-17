using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000867 RID: 2151
public class LookatHealth : MonoBehaviour
{
	// Token: 0x0400306F RID: 12399
	public static bool Enabled = true;

	// Token: 0x04003070 RID: 12400
	public GameObject container;

	// Token: 0x04003071 RID: 12401
	public Text textHealth;

	// Token: 0x04003072 RID: 12402
	public Text textStability;

	// Token: 0x04003073 RID: 12403
	public Image healthBar;

	// Token: 0x04003074 RID: 12404
	public Image healthBarBG;

	// Token: 0x04003075 RID: 12405
	public Color barBGColorNormal;

	// Token: 0x04003076 RID: 12406
	public Color barBGColorUnstable;
}
