using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000877 RID: 2167
public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	// Token: 0x040030B3 RID: 12467
	public CanvasRenderer panel;

	// Token: 0x040030B4 RID: 12468
	public TextMeshProUGUI title;

	// Token: 0x040030B5 RID: 12469
	public TextMeshProUGUI subtitle;

	// Token: 0x040030B6 RID: 12470
	public Button skipButton;

	// Token: 0x040030B7 RID: 12471
	public Button cancelButton;

	// Token: 0x040030B8 RID: 12472
	public GameObject performanceWarning;

	// Token: 0x040030B9 RID: 12473
	public AudioSource music;

	// Token: 0x040030BA RID: 12474
	public RectTransform serverInfo;

	// Token: 0x040030BB RID: 12475
	public RustText serverName;

	// Token: 0x040030BC RID: 12476
	public RustText serverPlayers;

	// Token: 0x040030BD RID: 12477
	public RustLayout serverModeSection;

	// Token: 0x040030BE RID: 12478
	public RustText serverMode;

	// Token: 0x040030BF RID: 12479
	public RustText serverMap;

	// Token: 0x040030C0 RID: 12480
	public RustLayout serverTagsSection;

	// Token: 0x040030C1 RID: 12481
	public ServerBrowserTagList serverTags;

	// Token: 0x040030C2 RID: 12482
	public RectTransform demoInfo;

	// Token: 0x040030C3 RID: 12483
	public RustText demoName;

	// Token: 0x040030C4 RID: 12484
	public RustText demoLength;

	// Token: 0x040030C5 RID: 12485
	public RustText demoDate;

	// Token: 0x040030C6 RID: 12486
	public RustText demoMap;

	// Token: 0x040030C7 RID: 12487
	public RawImage backgroundImage;

	// Token: 0x040030C8 RID: 12488
	public Texture2D defaultBackground;

	// Token: 0x040030C9 RID: 12489
	public GameObject pingWarning;

	// Token: 0x040030CA RID: 12490
	public RustText pingWarningText;

	// Token: 0x040030CB RID: 12491
	[Tooltip("Ping must be at least this many ms higher than the server browser ping")]
	public int minPingDiffToShowWarning = 50;

	// Token: 0x040030CC RID: 12492
	[Tooltip("Ping must be this many times higher than the server browser ping")]
	public float pingDiffFactorToShowWarning = 2f;

	// Token: 0x040030CD RID: 12493
	[Tooltip("Number of ping samples required before showing the warning")]
	public int requiredPingSampleCount = 10;

	// Token: 0x040030CE RID: 12494
	public GameObject blackout;

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x06003664 RID: 13924 RVA: 0x001496CD File Offset: 0x001478CD
	public static bool isOpen
	{
		get
		{
			return SingletonComponent<LoadingScreen>.Instance && SingletonComponent<LoadingScreen>.Instance.panel && SingletonComponent<LoadingScreen>.Instance.panel.gameObject.activeSelf;
		}
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x06003665 RID: 13925 RVA: 0x00149702 File Offset: 0x00147902
	// (set) Token: 0x06003666 RID: 13926 RVA: 0x00149709 File Offset: 0x00147909
	public static bool WantsSkip { get; private set; }

	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06003668 RID: 13928 RVA: 0x00149719 File Offset: 0x00147919
	// (set) Token: 0x06003667 RID: 13927 RVA: 0x00149711 File Offset: 0x00147911
	public static string Text { get; private set; }

	// Token: 0x06003669 RID: 13929 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Update(string strType)
	{
	}

	// Token: 0x0600366A RID: 13930 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Update(string strType, string strSubtitle)
	{
	}
}
