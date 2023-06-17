using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017C RID: 380
public class SignPanel : MonoBehaviour, IImageReceiver
{
	// Token: 0x04001077 RID: 4215
	public RawImage Image;

	// Token: 0x04001078 RID: 4216
	public RectTransform ImageContainer;

	// Token: 0x04001079 RID: 4217
	public RustText DisabledSignsMessage;
}
