using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.ServerAdmin
{
	// Token: 0x02000B20 RID: 2848
	public class ServerAdminUI : SingletonComponent<ServerAdminUI>
	{
		// Token: 0x04003D88 RID: 15752
		public GameObjectRef PlayerEntry;

		// Token: 0x04003D89 RID: 15753
		public RectTransform PlayerInfoParent;

		// Token: 0x04003D8A RID: 15754
		public RustText PlayerCount;

		// Token: 0x04003D8B RID: 15755
		public RustInput PlayerNameFilter;

		// Token: 0x04003D8C RID: 15756
		public GameObjectRef ServerInfoEntry;

		// Token: 0x04003D8D RID: 15757
		public RectTransform ServerInfoParent;

		// Token: 0x04003D8E RID: 15758
		public GameObjectRef ConvarInfoEntry;

		// Token: 0x04003D8F RID: 15759
		public GameObjectRef ConvarInfoLongEntry;

		// Token: 0x04003D90 RID: 15760
		public RectTransform ConvarInfoParent;

		// Token: 0x04003D91 RID: 15761
		public ServerAdminPlayerInfo PlayerInfo;

		// Token: 0x04003D92 RID: 15762
		public RustInput UgcNameFilter;

		// Token: 0x04003D93 RID: 15763
		public GameObjectRef ImageEntry;

		// Token: 0x04003D94 RID: 15764
		public GameObjectRef PatternEntry;

		// Token: 0x04003D95 RID: 15765
		public GameObjectRef SoundEntry;

		// Token: 0x04003D96 RID: 15766
		public VirtualScroll UgcVirtualScroll;

		// Token: 0x04003D97 RID: 15767
		public GameObject ExpandedUgcRoot;

		// Token: 0x04003D98 RID: 15768
		public RawImage ExpandedImage;

		// Token: 0x04003D99 RID: 15769
		public RectTransform ExpandedImageBacking;
	}
}
