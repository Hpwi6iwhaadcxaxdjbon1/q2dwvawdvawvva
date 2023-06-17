using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020007C3 RID: 1987
public class TexasHoldEmUI : MonoBehaviour
{
	// Token: 0x04002C49 RID: 11337
	[SerializeField]
	private Image[] holeCardImages;

	// Token: 0x04002C4A RID: 11338
	[SerializeField]
	private Image[] holeCardBackings;

	// Token: 0x04002C4B RID: 11339
	[FormerlySerializedAs("flopCardImages")]
	[SerializeField]
	private Image[] communityCardImages;

	// Token: 0x04002C4C RID: 11340
	[SerializeField]
	private Image[] communityCardBackings;

	// Token: 0x04002C4D RID: 11341
	[SerializeField]
	private RustText potText;

	// Token: 0x04002C4E RID: 11342
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002C4F RID: 11343
	[SerializeField]
	private Translate.Phrase phraseWinningHand;

	// Token: 0x04002C50 RID: 11344
	[SerializeField]
	private Translate.Phrase foldPhrase;

	// Token: 0x04002C51 RID: 11345
	[SerializeField]
	private Translate.Phrase raisePhrase;

	// Token: 0x04002C52 RID: 11346
	[SerializeField]
	private Translate.Phrase checkPhrase;

	// Token: 0x04002C53 RID: 11347
	[SerializeField]
	private Translate.Phrase callPhrase;

	// Token: 0x04002C54 RID: 11348
	[SerializeField]
	private Translate.Phrase phraseRoyalFlush;

	// Token: 0x04002C55 RID: 11349
	[SerializeField]
	private Translate.Phrase phraseStraightFlush;

	// Token: 0x04002C56 RID: 11350
	[SerializeField]
	private Translate.Phrase phraseFourOfAKind;

	// Token: 0x04002C57 RID: 11351
	[SerializeField]
	private Translate.Phrase phraseFullHouse;

	// Token: 0x04002C58 RID: 11352
	[SerializeField]
	private Translate.Phrase phraseFlush;

	// Token: 0x04002C59 RID: 11353
	[SerializeField]
	private Translate.Phrase phraseStraight;

	// Token: 0x04002C5A RID: 11354
	[SerializeField]
	private Translate.Phrase phraseThreeOfAKind;

	// Token: 0x04002C5B RID: 11355
	[SerializeField]
	private Translate.Phrase phraseTwoPair;

	// Token: 0x04002C5C RID: 11356
	[SerializeField]
	private Translate.Phrase phrasePair;

	// Token: 0x04002C5D RID: 11357
	[SerializeField]
	private Translate.Phrase phraseHighCard;

	// Token: 0x04002C5E RID: 11358
	[SerializeField]
	private Translate.Phrase phraseRaiseAmount;

	// Token: 0x04002C5F RID: 11359
	[SerializeField]
	private Sprite dealerChip;

	// Token: 0x04002C60 RID: 11360
	[SerializeField]
	private Sprite smallBlindChip;

	// Token: 0x04002C61 RID: 11361
	[SerializeField]
	private Sprite bigBlindChip;

	// Token: 0x04002C62 RID: 11362
	[SerializeField]
	private Sprite noIcon;
}
