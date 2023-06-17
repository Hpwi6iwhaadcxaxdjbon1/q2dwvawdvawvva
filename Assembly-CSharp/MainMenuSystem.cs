using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000878 RID: 2168
public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	// Token: 0x040030CF RID: 12495
	public static bool isOpen = true;

	// Token: 0x040030D0 RID: 12496
	public static Action OnOpenStateChanged;

	// Token: 0x040030D1 RID: 12497
	public RustButton SessionButton;

	// Token: 0x040030D2 RID: 12498
	public GameObject SessionPanel;

	// Token: 0x040030D3 RID: 12499
	public GameObject NewsStoriesAlert;

	// Token: 0x040030D4 RID: 12500
	public GameObject ItemStoreAlert;

	// Token: 0x040030D5 RID: 12501
	public GameObject CompanionAlert;

	// Token: 0x040030D6 RID: 12502
	public GameObject DemoBrowser;

	// Token: 0x040030D7 RID: 12503
	public GameObject DemoBrowserButton;

	// Token: 0x040030D8 RID: 12504
	public GameObject SuicideButton;

	// Token: 0x040030D9 RID: 12505
	public GameObject EndDemoButton;

	// Token: 0x040030DA RID: 12506
	public GameObject ReflexModeOption;

	// Token: 0x040030DB RID: 12507
	public GameObject ReflexLatencyMarkerOption;
}
