using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007CC RID: 1996
public class VoicemailDialog : MonoBehaviour
{
	// Token: 0x04002CA6 RID: 11430
	public GameObject RecordingRoot;

	// Token: 0x04002CA7 RID: 11431
	public RustSlider RecordingProgress;

	// Token: 0x04002CA8 RID: 11432
	public GameObject BrowsingRoot;

	// Token: 0x04002CA9 RID: 11433
	public PhoneDialler ParentDialler;

	// Token: 0x04002CAA RID: 11434
	public GameObjectRef VoicemailEntry;

	// Token: 0x04002CAB RID: 11435
	public Transform VoicemailEntriesRoot;

	// Token: 0x04002CAC RID: 11436
	public GameObject NoVoicemailRoot;

	// Token: 0x04002CAD RID: 11437
	public GameObject NoCassetteRoot;
}
