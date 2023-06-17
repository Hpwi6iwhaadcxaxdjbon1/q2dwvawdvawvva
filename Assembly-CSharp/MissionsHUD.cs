using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000898 RID: 2200
public class MissionsHUD : SingletonComponent<MissionsHUD>
{
	// Token: 0x04003152 RID: 12626
	public SoundDefinition listComplete;

	// Token: 0x04003153 RID: 12627
	public SoundDefinition itemComplete;

	// Token: 0x04003154 RID: 12628
	public SoundDefinition popup;

	// Token: 0x04003155 RID: 12629
	public Canvas Canvas;

	// Token: 0x04003156 RID: 12630
	public Text titleText;

	// Token: 0x04003157 RID: 12631
	public GameObject timerObject;

	// Token: 0x04003158 RID: 12632
	public RustText timerText;
}
