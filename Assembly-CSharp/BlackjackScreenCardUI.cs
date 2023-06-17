using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BB RID: 1979
public class BlackjackScreenCardUI : FacepunchBehaviour
{
	// Token: 0x04002BC4 RID: 11204
	[SerializeField]
	private Canvas baseCanvas;

	// Token: 0x04002BC5 RID: 11205
	[SerializeField]
	private Canvas cardFront;

	// Token: 0x04002BC6 RID: 11206
	[SerializeField]
	private Canvas cardBack;

	// Token: 0x04002BC7 RID: 11207
	[SerializeField]
	private Image image;

	// Token: 0x04002BC8 RID: 11208
	[SerializeField]
	private RustText text;

	// Token: 0x04002BC9 RID: 11209
	[SerializeField]
	private Sprite heartSprite;

	// Token: 0x04002BCA RID: 11210
	[SerializeField]
	private Sprite diamondSprite;

	// Token: 0x04002BCB RID: 11211
	[SerializeField]
	private Sprite spadeSprite;

	// Token: 0x04002BCC RID: 11212
	[SerializeField]
	private Sprite clubSprite;
}
