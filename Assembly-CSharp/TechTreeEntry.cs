using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007D6 RID: 2006
public class TechTreeEntry : TechTreeWidget
{
	// Token: 0x04002CDC RID: 11484
	public RawImage icon;

	// Token: 0x04002CDD RID: 11485
	public GameObject ableToUnlockBackground;

	// Token: 0x04002CDE RID: 11486
	public GameObject unlockedBackground;

	// Token: 0x04002CDF RID: 11487
	public GameObject lockedBackground;

	// Token: 0x04002CE0 RID: 11488
	public GameObject lockOverlay;

	// Token: 0x04002CE1 RID: 11489
	public GameObject selectedBackground;

	// Token: 0x04002CE2 RID: 11490
	public Image radialUnlock;

	// Token: 0x04002CE3 RID: 11491
	public float holdTime = 1f;
}
