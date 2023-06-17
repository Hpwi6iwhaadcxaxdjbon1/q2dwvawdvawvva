using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BE RID: 1982
public class BlackjackUI : MonoBehaviour
{
	// Token: 0x04002BEA RID: 11242
	[SerializeField]
	private Image[] playerCardImages;

	// Token: 0x04002BEB RID: 11243
	[SerializeField]
	private Image[] dealerCardImages;

	// Token: 0x04002BEC RID: 11244
	[SerializeField]
	private Image[] splitCardImages;

	// Token: 0x04002BED RID: 11245
	[SerializeField]
	private Image[] playerCardBackings;

	// Token: 0x04002BEE RID: 11246
	[SerializeField]
	private Image[] dealerCardBackings;

	// Token: 0x04002BEF RID: 11247
	[SerializeField]
	private Image[] splitCardBackings;

	// Token: 0x04002BF0 RID: 11248
	[SerializeField]
	private CardGamePlayerWidget[] playerWidgets;

	// Token: 0x04002BF1 RID: 11249
	[SerializeField]
	private GameObject dealerValueObj;

	// Token: 0x04002BF2 RID: 11250
	[SerializeField]
	private RustText dealerValueText;

	// Token: 0x04002BF3 RID: 11251
	[SerializeField]
	private GameObject yourValueObj;

	// Token: 0x04002BF4 RID: 11252
	[SerializeField]
	private RustText yourValueText;

	// Token: 0x04002BF5 RID: 11253
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002BF6 RID: 11254
	[SerializeField]
	private Translate.Phrase phraseHit;

	// Token: 0x04002BF7 RID: 11255
	[SerializeField]
	private Translate.Phrase phraseStand;

	// Token: 0x04002BF8 RID: 11256
	[SerializeField]
	private Translate.Phrase phraseSplit;

	// Token: 0x04002BF9 RID: 11257
	[SerializeField]
	private Translate.Phrase phraseDouble;

	// Token: 0x04002BFA RID: 11258
	[SerializeField]
	private Translate.Phrase phraseInsurance;

	// Token: 0x04002BFB RID: 11259
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002BFC RID: 11260
	[SerializeField]
	private Translate.Phrase phraseBlackjack;

	// Token: 0x04002BFD RID: 11261
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002BFE RID: 11262
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002BFF RID: 11263
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002C00 RID: 11264
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002C01 RID: 11265
	[SerializeField]
	private Translate.Phrase phraseHand;

	// Token: 0x04002C02 RID: 11266
	[SerializeField]
	private Translate.Phrase phraseInsurancePaidOut;

	// Token: 0x04002C03 RID: 11267
	[SerializeField]
	private Sprite insuranceIcon;

	// Token: 0x04002C04 RID: 11268
	[SerializeField]
	private Sprite noIcon;

	// Token: 0x04002C05 RID: 11269
	[SerializeField]
	private Color bustTextColour;
}
