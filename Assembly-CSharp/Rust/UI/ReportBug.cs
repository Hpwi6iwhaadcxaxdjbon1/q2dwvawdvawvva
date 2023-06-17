using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000B0E RID: 2830
	public class ReportBug : UIDialog
	{
		// Token: 0x04003D29 RID: 15657
		public GameObject GetInformation;

		// Token: 0x04003D2A RID: 15658
		public GameObject Finished;

		// Token: 0x04003D2B RID: 15659
		public RustInput Subject;

		// Token: 0x04003D2C RID: 15660
		public RustInput Message;

		// Token: 0x04003D2D RID: 15661
		public RustButton ReportButton;

		// Token: 0x04003D2E RID: 15662
		public RustButtonGroup Category;

		// Token: 0x04003D2F RID: 15663
		public RustIcon ProgressIcon;

		// Token: 0x04003D30 RID: 15664
		public RustText ProgressText;

		// Token: 0x04003D31 RID: 15665
		public RawImage ScreenshotImage;

		// Token: 0x04003D32 RID: 15666
		public GameObject ScreenshotRoot;

		// Token: 0x04003D33 RID: 15667
		public UIBackgroundBlur BlurController;

		// Token: 0x04003D34 RID: 15668
		public RustButton SubmitButton;

		// Token: 0x04003D35 RID: 15669
		public GameObject SubmitErrorRoot;

		// Token: 0x04003D36 RID: 15670
		public RustText CooldownText;

		// Token: 0x04003D37 RID: 15671
		public RustText ContentMissingText;
	}
}
