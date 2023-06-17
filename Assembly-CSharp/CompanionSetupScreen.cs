using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200086A RID: 2154
public class CompanionSetupScreen : SingletonComponent<CompanionSetupScreen>
{
	// Token: 0x04003081 RID: 12417
	public const string PairedKey = "companionPaired";

	// Token: 0x04003082 RID: 12418
	public GameObject instructionsBody;

	// Token: 0x04003083 RID: 12419
	public GameObject detailsPanel;

	// Token: 0x04003084 RID: 12420
	public GameObject loadingMessage;

	// Token: 0x04003085 RID: 12421
	public GameObject errorMessage;

	// Token: 0x04003086 RID: 12422
	public GameObject notSupportedMessage;

	// Token: 0x04003087 RID: 12423
	public GameObject disabledMessage;

	// Token: 0x04003088 RID: 12424
	public GameObject enabledMessage;

	// Token: 0x04003089 RID: 12425
	public GameObject refreshButton;

	// Token: 0x0400308A RID: 12426
	public GameObject enableButton;

	// Token: 0x0400308B RID: 12427
	public GameObject disableButton;

	// Token: 0x0400308C RID: 12428
	public GameObject pairButton;

	// Token: 0x0400308D RID: 12429
	public RustText serverName;

	// Token: 0x0400308E RID: 12430
	public RustButton helpButton;

	// Token: 0x02000E8F RID: 3727
	public enum ScreenState
	{
		// Token: 0x04004C20 RID: 19488
		Loading,
		// Token: 0x04004C21 RID: 19489
		Error,
		// Token: 0x04004C22 RID: 19490
		NoServer,
		// Token: 0x04004C23 RID: 19491
		NotSupported,
		// Token: 0x04004C24 RID: 19492
		NotInstalled,
		// Token: 0x04004C25 RID: 19493
		Disabled,
		// Token: 0x04004C26 RID: 19494
		Enabled,
		// Token: 0x04004C27 RID: 19495
		ShowHelp
	}
}
