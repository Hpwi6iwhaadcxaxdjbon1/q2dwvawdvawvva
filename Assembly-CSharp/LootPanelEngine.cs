using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000833 RID: 2099
public class LootPanelEngine : LootPanel
{
	// Token: 0x04002F03 RID: 12035
	[SerializeField]
	private Image engineImage;

	// Token: 0x04002F04 RID: 12036
	[SerializeField]
	private ItemIcon[] icons;

	// Token: 0x04002F05 RID: 12037
	[SerializeField]
	private GameObject warning;

	// Token: 0x04002F06 RID: 12038
	[SerializeField]
	private RustText hp;

	// Token: 0x04002F07 RID: 12039
	[SerializeField]
	private RustText power;

	// Token: 0x04002F08 RID: 12040
	[SerializeField]
	private RustText acceleration;

	// Token: 0x04002F09 RID: 12041
	[SerializeField]
	private RustText topSpeed;

	// Token: 0x04002F0A RID: 12042
	[SerializeField]
	private RustText fuelEconomy;
}
