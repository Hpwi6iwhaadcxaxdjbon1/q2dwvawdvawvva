using System;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B0F RID: 2831
	public class ReportPlayer : UIDialog
	{
		// Token: 0x04003D38 RID: 15672
		public GameObject FindPlayer;

		// Token: 0x04003D39 RID: 15673
		public GameObject GetInformation;

		// Token: 0x04003D3A RID: 15674
		public GameObject Finished;

		// Token: 0x04003D3B RID: 15675
		public GameObject RecentlyReported;

		// Token: 0x04003D3C RID: 15676
		public Dropdown ReasonDropdown;

		// Token: 0x04003D3D RID: 15677
		public RustInput Subject;

		// Token: 0x04003D3E RID: 15678
		public RustInput Message;

		// Token: 0x04003D3F RID: 15679
		public RustButton ReportButton;

		// Token: 0x04003D40 RID: 15680
		public SteamUserButton SteamUserButton;

		// Token: 0x04003D41 RID: 15681
		public RustIcon ProgressIcon;

		// Token: 0x04003D42 RID: 15682
		public RustText ProgressText;

		// Token: 0x04003D43 RID: 15683
		public static Option[] ReportReasons = new Option[]
		{
			new Option(new Translate.Phrase("report.reason.none", "Select an option"), "none", false, Icons.Bars),
			new Option(new Translate.Phrase("report.reason.abuse", "Racism/Sexism/Abusive"), "abusive", false, Icons.Angry),
			new Option(new Translate.Phrase("report.reason.cheat", "Cheating"), "cheat", false, Icons.Crosshairs),
			new Option(new Translate.Phrase("report.reason.spam", "Spamming"), "spam", false, Icons.Bullhorn),
			new Option(new Translate.Phrase("report.reason.name", "Offensive Name"), "name", false, Icons.Radiation)
		};
	}
}
