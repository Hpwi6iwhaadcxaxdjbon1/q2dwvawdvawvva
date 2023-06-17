using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007B3 RID: 1971
public class DemoShotRecordWidget : MonoBehaviour
{
	// Token: 0x04002B85 RID: 11141
	public RustInput NameInput;

	// Token: 0x04002B86 RID: 11142
	public GameObject RecordingRoot;

	// Token: 0x04002B87 RID: 11143
	public GameObject PreRecordingRoot;

	// Token: 0x04002B88 RID: 11144
	public RustButton CountdownToggle;

	// Token: 0x04002B89 RID: 11145
	public RustButton PauseOnSaveToggle;

	// Token: 0x04002B8A RID: 11146
	public RustButton ReturnToStartToggle;

	// Token: 0x04002B8B RID: 11147
	public RustButton RecordDofToggle;

	// Token: 0x04002B8C RID: 11148
	public RustOption FolderDropdown;

	// Token: 0x04002B8D RID: 11149
	public GameObject RecordingUnderlay;

	// Token: 0x04002B8E RID: 11150
	public AudioSource CountdownAudio;

	// Token: 0x04002B8F RID: 11151
	public GameObject ShotRecordTime;

	// Token: 0x04002B90 RID: 11152
	public RustText ShotRecordTimeText;

	// Token: 0x04002B91 RID: 11153
	public RustText ShotNameText;

	// Token: 0x04002B92 RID: 11154
	public GameObject RecordingInProcessRoot;

	// Token: 0x04002B93 RID: 11155
	public GameObject CountdownActiveRoot;

	// Token: 0x04002B94 RID: 11156
	public GameObject CountdownActiveSliderRoot;

	// Token: 0x04002B95 RID: 11157
	public RustSlider CountdownActiveSlider;

	// Token: 0x04002B96 RID: 11158
	public RustText CountdownActiveText;
}
