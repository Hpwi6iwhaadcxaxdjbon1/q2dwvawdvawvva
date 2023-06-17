using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007AC RID: 1964
public class DemoPlaybackWidget : MonoBehaviour
{
	// Token: 0x04002B62 RID: 11106
	public RustSlider DemoProgress;

	// Token: 0x04002B63 RID: 11107
	public RustText DemoName;

	// Token: 0x04002B64 RID: 11108
	public RustText DemoDuration;

	// Token: 0x04002B65 RID: 11109
	public RustText DemoCurrentTime;

	// Token: 0x04002B66 RID: 11110
	public GameObject PausedRoot;

	// Token: 0x04002B67 RID: 11111
	public GameObject PlayingRoot;

	// Token: 0x04002B68 RID: 11112
	public RectTransform DemoPlaybackHandle;

	// Token: 0x04002B69 RID: 11113
	public RectTransform ShotPlaybackWindow;

	// Token: 0x04002B6A RID: 11114
	public RustButton LoopButton;

	// Token: 0x04002B6B RID: 11115
	public GameObject ShotButtonRoot;

	// Token: 0x04002B6C RID: 11116
	public RustText ShotNameText;

	// Token: 0x04002B6D RID: 11117
	public GameObject ShotNameRoot;

	// Token: 0x04002B6E RID: 11118
	public RectTransform ShotRecordWindow;
}
