using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000836 RID: 2102
public class LootPanelIndustrialCrafter : LootPanel
{
	// Token: 0x04002F12 RID: 12050
	public GameObject CraftingRoot;

	// Token: 0x04002F13 RID: 12051
	public RustSlider ProgressSlider;

	// Token: 0x04002F14 RID: 12052
	public Transform Spinner;

	// Token: 0x04002F15 RID: 12053
	public float SpinSpeed = 90f;

	// Token: 0x04002F16 RID: 12054
	public GameObject WorkbenchLevelRoot;
}
