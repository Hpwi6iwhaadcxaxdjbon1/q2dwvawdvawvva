using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BA RID: 1978
public class BlackjackMainScreenUI : FacepunchBehaviour
{
	// Token: 0x04002BB1 RID: 11185
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002BB2 RID: 11186
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002BB3 RID: 11187
	[SerializeField]
	private Sprite faceNeutral;

	// Token: 0x04002BB4 RID: 11188
	[SerializeField]
	private Sprite faceShocked;

	// Token: 0x04002BB5 RID: 11189
	[SerializeField]
	private Sprite faceSad;

	// Token: 0x04002BB6 RID: 11190
	[SerializeField]
	private Sprite faceCool;

	// Token: 0x04002BB7 RID: 11191
	[SerializeField]
	private Sprite faceHappy;

	// Token: 0x04002BB8 RID: 11192
	[SerializeField]
	private Sprite faceLove;

	// Token: 0x04002BB9 RID: 11193
	[SerializeField]
	private Image faceInGame;

	// Token: 0x04002BBA RID: 11194
	[SerializeField]
	private Image faceNotInGame;

	// Token: 0x04002BBB RID: 11195
	[SerializeField]
	private Sprite[] faceNeutralVariants;

	// Token: 0x04002BBC RID: 11196
	[SerializeField]
	private Sprite[] faceHalloweenVariants;

	// Token: 0x04002BBD RID: 11197
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002BBE RID: 11198
	[SerializeField]
	private RustText payoutText;

	// Token: 0x04002BBF RID: 11199
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002BC0 RID: 11200
	[SerializeField]
	private Canvas placeBetsCanvas;

	// Token: 0x04002BC1 RID: 11201
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002BC2 RID: 11202
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002BC3 RID: 11203
	[SerializeField]
	private Translate.Phrase phraseBust;
}
