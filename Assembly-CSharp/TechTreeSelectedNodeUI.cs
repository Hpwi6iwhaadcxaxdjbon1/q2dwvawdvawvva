using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007D9 RID: 2009
public class TechTreeSelectedNodeUI : MonoBehaviour
{
	// Token: 0x04002CF0 RID: 11504
	public RustText selectedTitle;

	// Token: 0x04002CF1 RID: 11505
	public RawImage selectedIcon;

	// Token: 0x04002CF2 RID: 11506
	public RustText selectedDescription;

	// Token: 0x04002CF3 RID: 11507
	public RustText costText;

	// Token: 0x04002CF4 RID: 11508
	public RustText craftingCostText;

	// Token: 0x04002CF5 RID: 11509
	public GameObject costObject;

	// Token: 0x04002CF6 RID: 11510
	public GameObject cantAffordObject;

	// Token: 0x04002CF7 RID: 11511
	public GameObject unlockedObject;

	// Token: 0x04002CF8 RID: 11512
	public GameObject unlockButton;

	// Token: 0x04002CF9 RID: 11513
	public GameObject noPathObject;

	// Token: 0x04002CFA RID: 11514
	public TechTreeDialog dialog;

	// Token: 0x04002CFB RID: 11515
	public Color ColorAfford;

	// Token: 0x04002CFC RID: 11516
	public Color ColorCantAfford;

	// Token: 0x04002CFD RID: 11517
	public GameObject totalRequiredRoot;

	// Token: 0x04002CFE RID: 11518
	public RustText totalRequiredText;

	// Token: 0x04002CFF RID: 11519
	public ItemInformationPanel[] informationPanels;
}
