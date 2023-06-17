using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BD RID: 1981
public class BlackjackSmallScreenUI : FacepunchBehaviour
{
	// Token: 0x04002BD0 RID: 11216
	[SerializeField]
	private Canvas notInGameDisplay;

	// Token: 0x04002BD1 RID: 11217
	[SerializeField]
	private Canvas inGameDisplay;

	// Token: 0x04002BD2 RID: 11218
	[SerializeField]
	private RustText cardCountText;

	// Token: 0x04002BD3 RID: 11219
	[SerializeField]
	private RustText betText;

	// Token: 0x04002BD4 RID: 11220
	[SerializeField]
	private RustText splitBetText;

	// Token: 0x04002BD5 RID: 11221
	[SerializeField]
	private RustText insuranceText;

	// Token: 0x04002BD6 RID: 11222
	[SerializeField]
	private RustText bankText;

	// Token: 0x04002BD7 RID: 11223
	[SerializeField]
	private RustText splitText;

	// Token: 0x04002BD8 RID: 11224
	[SerializeField]
	private Canvas infoTextCanvas;

	// Token: 0x04002BD9 RID: 11225
	[SerializeField]
	private RustText inGameText;

	// Token: 0x04002BDA RID: 11226
	[SerializeField]
	private RustText notInGameText;

	// Token: 0x04002BDB RID: 11227
	[SerializeField]
	private HorizontalLayoutGroup cardsLayout;

	// Token: 0x04002BDC RID: 11228
	[SerializeField]
	private BlackjackScreenCardUI[] cards;

	// Token: 0x04002BDD RID: 11229
	[SerializeField]
	private BlackjackScreenInputUI[] inputs;

	// Token: 0x04002BDE RID: 11230
	[SerializeField]
	private Translate.Phrase phraseBust;

	// Token: 0x04002BDF RID: 11231
	[SerializeField]
	private Translate.Phrase phraseBet;

	// Token: 0x04002BE0 RID: 11232
	[SerializeField]
	private Translate.Phrase phrasePlaceYourBet;

	// Token: 0x04002BE1 RID: 11233
	[SerializeField]
	private Translate.Phrase phraseStandoff;

	// Token: 0x04002BE2 RID: 11234
	[SerializeField]
	private Translate.Phrase phraseYouWin;

	// Token: 0x04002BE3 RID: 11235
	[SerializeField]
	private Translate.Phrase phraseYouLose;

	// Token: 0x04002BE4 RID: 11236
	[SerializeField]
	private Translate.Phrase phraseWaitingForOtherPlayers;

	// Token: 0x04002BE5 RID: 11237
	[SerializeField]
	private Translate.Phrase phraseAddFunds;

	// Token: 0x04002BE6 RID: 11238
	[SerializeField]
	private Translate.Phrase phraseWaitingForPlayer;

	// Token: 0x04002BE7 RID: 11239
	[SerializeField]
	private Translate.Phrase phraseSplitStored;

	// Token: 0x04002BE8 RID: 11240
	[SerializeField]
	private Translate.Phrase phraseSplitActive;

	// Token: 0x04002BE9 RID: 11241
	[SerializeField]
	private Translate.Phrase phraseHand;
}
