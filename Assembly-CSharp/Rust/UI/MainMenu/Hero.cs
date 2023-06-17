using System;
using UnityEngine;

namespace Rust.UI.MainMenu
{
	// Token: 0x02000B21 RID: 2849
	public class Hero : SingletonComponent<Hero>
	{
		// Token: 0x04003D9A RID: 15770
		public CanvasGroup CanvasGroup;

		// Token: 0x04003D9B RID: 15771
		public Video VideoPlayer;

		// Token: 0x04003D9C RID: 15772
		public RustText TitleText;

		// Token: 0x04003D9D RID: 15773
		public RustText ButtonText;

		// Token: 0x04003D9E RID: 15774
		public HttpImage TitleImage;

		// Token: 0x04003D9F RID: 15775
		[Header("Item Store Links")]
		public RustButton ItemStoreButton;

		// Token: 0x04003DA0 RID: 15776
		public RustButton LimitedTabButton;

		// Token: 0x04003DA1 RID: 15777
		public RustButton GeneralTabButton;
	}
}
