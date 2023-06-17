using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BF RID: 1983
public class CardGamePlayerWidget : MonoBehaviour
{
	// Token: 0x04002C06 RID: 11270
	[SerializeField]
	private GameObjectRef cardImageSmallPrefab;

	// Token: 0x04002C07 RID: 11271
	[SerializeField]
	private RawImage avatar;

	// Token: 0x04002C08 RID: 11272
	[SerializeField]
	private RustText playerName;

	// Token: 0x04002C09 RID: 11273
	[SerializeField]
	private RustText scrapTotal;

	// Token: 0x04002C0A RID: 11274
	[SerializeField]
	private RustText betText;

	// Token: 0x04002C0B RID: 11275
	[SerializeField]
	private Image background;

	// Token: 0x04002C0C RID: 11276
	[SerializeField]
	private Color inactiveBackground;

	// Token: 0x04002C0D RID: 11277
	[SerializeField]
	private Color activeBackground;

	// Token: 0x04002C0E RID: 11278
	[SerializeField]
	private Color foldedBackground;

	// Token: 0x04002C0F RID: 11279
	[SerializeField]
	private Color winnerBackground;

	// Token: 0x04002C10 RID: 11280
	[SerializeField]
	private Animation actionShowAnimation;

	// Token: 0x04002C11 RID: 11281
	[SerializeField]
	private RustText actionText;

	// Token: 0x04002C12 RID: 11282
	[SerializeField]
	private Sprite canSeeIcon;

	// Token: 0x04002C13 RID: 11283
	[SerializeField]
	private Sprite cannotSeeIcon;

	// Token: 0x04002C14 RID: 11284
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002C15 RID: 11285
	[SerializeField]
	private Image cornerIcon;

	// Token: 0x04002C16 RID: 11286
	[SerializeField]
	private Transform cardDisplayParent;

	// Token: 0x04002C17 RID: 11287
	[SerializeField]
	private GridLayoutGroup cardDisplayGridLayout;

	// Token: 0x04002C18 RID: 11288
	[SerializeField]
	private GameObject circle;

	// Token: 0x04002C19 RID: 11289
	[SerializeField]
	private RustText circleText;
}
