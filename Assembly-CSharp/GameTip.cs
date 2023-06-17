using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007ED RID: 2029
public class GameTip : SingletonComponent<GameTip>
{
	// Token: 0x04002D62 RID: 11618
	public CanvasGroup canvasGroup;

	// Token: 0x04002D63 RID: 11619
	public RustIcon icon;

	// Token: 0x04002D64 RID: 11620
	public Image background;

	// Token: 0x04002D65 RID: 11621
	public RustText text;

	// Token: 0x04002D66 RID: 11622
	public GameTip.Theme[] themes;

	// Token: 0x02000E7D RID: 3709
	public enum Styles
	{
		// Token: 0x04004BE0 RID: 19424
		Blue_Normal,
		// Token: 0x04004BE1 RID: 19425
		Red_Normal,
		// Token: 0x04004BE2 RID: 19426
		Blue_Long,
		// Token: 0x04004BE3 RID: 19427
		Blue_Short,
		// Token: 0x04004BE4 RID: 19428
		Server_Event
	}

	// Token: 0x02000E7E RID: 3710
	[Serializable]
	public struct Theme
	{
		// Token: 0x04004BE5 RID: 19429
		public Icons Icon;

		// Token: 0x04004BE6 RID: 19430
		public Color BackgroundColor;

		// Token: 0x04004BE7 RID: 19431
		public Color ForegroundColor;

		// Token: 0x04004BE8 RID: 19432
		public float duration;
	}
}
