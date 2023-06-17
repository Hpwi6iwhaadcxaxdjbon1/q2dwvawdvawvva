using System;
using UnityEngine;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000B1F RID: 2847
	public class ServerAdminUGCEntryStub : MonoBehaviour
	{
		// Token: 0x04003D82 RID: 15746
		public ServerAdminUGCEntryAudio AudioWidget;

		// Token: 0x04003D83 RID: 15747
		public ServerAdminUGCEntryImage ImageWidget;

		// Token: 0x04003D84 RID: 15748
		public ServerAdminUGCEntryPattern PatternWidget;

		// Token: 0x04003D85 RID: 15749
		public RustText PrefabName;

		// Token: 0x04003D86 RID: 15750
		public RustButton HistoryButton;

		// Token: 0x04003D87 RID: 15751
		public ServerAdminPlayerId[] HistoryIds = new ServerAdminPlayerId[0];
	}
}
