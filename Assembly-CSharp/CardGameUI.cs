using System;
using Facepunch.CardGames;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C0 RID: 1984
public class CardGameUI : UIDialog
{
	// Token: 0x04002C1A RID: 11290
	[Header("Card Game")]
	[SerializeField]
	private CardGameUI.InfoTextUI primaryInfo;

	// Token: 0x04002C1B RID: 11291
	[SerializeField]
	private CardGameUI.InfoTextUI secondaryInfo;

	// Token: 0x04002C1C RID: 11292
	[SerializeField]
	private CardGameUI.InfoTextUI playerLeaveInfo;

	// Token: 0x04002C1D RID: 11293
	[SerializeField]
	private GameObject playingUI;

	// Token: 0x04002C1E RID: 11294
	[SerializeField]
	private CardGameUI.PlayingCardImage[] cardImages;

	// Token: 0x04002C1F RID: 11295
	[SerializeField]
	private CardInputWidget[] inputWidgets;

	// Token: 0x04002C20 RID: 11296
	[SerializeField]
	private RustSlider dismountProgressSlider;

	// Token: 0x04002C21 RID: 11297
	[SerializeField]
	private Translate.Phrase phraseLoading;

	// Token: 0x04002C22 RID: 11298
	[SerializeField]
	private Translate.Phrase phraseWaitingForNextRound;

	// Token: 0x04002C23 RID: 11299
	[SerializeField]
	private Translate.Phrase phraseNotEnoughPlayers;

	// Token: 0x04002C24 RID: 11300
	[SerializeField]
	private Translate.Phrase phrasePlayerLeftGame;

	// Token: 0x04002C25 RID: 11301
	[SerializeField]
	private Translate.Phrase phraseNotEnoughBuyIn;

	// Token: 0x04002C26 RID: 11302
	[SerializeField]
	private Translate.Phrase phraseTooMuchBuyIn;

	// Token: 0x04002C27 RID: 11303
	public Translate.Phrase phraseYourTurn;

	// Token: 0x04002C28 RID: 11304
	public Translate.Phrase phraseYouWinTheRound;

	// Token: 0x04002C29 RID: 11305
	public Translate.Phrase phraseRoundWinner;

	// Token: 0x04002C2A RID: 11306
	public Translate.Phrase phraseRoundWinners;

	// Token: 0x04002C2B RID: 11307
	public Translate.Phrase phraseScrapWon;

	// Token: 0x04002C2C RID: 11308
	public Translate.Phrase phraseScrapReturned;

	// Token: 0x04002C2D RID: 11309
	public Translate.Phrase phraseChangeBetAmount;

	// Token: 0x04002C2E RID: 11310
	public Translate.Phrase phraseBet;

	// Token: 0x04002C2F RID: 11311
	public Translate.Phrase phraseBetAdd;

	// Token: 0x04002C30 RID: 11312
	public Translate.Phrase phraseAllIn;

	// Token: 0x04002C31 RID: 11313
	public GameObject amountChangeRoot;

	// Token: 0x04002C32 RID: 11314
	public RustText amountChangeText;

	// Token: 0x04002C33 RID: 11315
	public Color colourNeutralUI;

	// Token: 0x04002C34 RID: 11316
	public Color colourGoodUI;

	// Token: 0x04002C35 RID: 11317
	public Color colourBadUI;

	// Token: 0x04002C36 RID: 11318
	[SerializeField]
	private CanvasGroup timerCanvas;

	// Token: 0x04002C37 RID: 11319
	[SerializeField]
	private RustSlider timerSlider;

	// Token: 0x04002C38 RID: 11320
	[SerializeField]
	private UIChat chat;

	// Token: 0x04002C39 RID: 11321
	[SerializeField]
	private HudElement Hunger;

	// Token: 0x04002C3A RID: 11322
	[SerializeField]
	private HudElement Thirst;

	// Token: 0x04002C3B RID: 11323
	[SerializeField]
	private HudElement Health;

	// Token: 0x04002C3C RID: 11324
	[SerializeField]
	private HudElement PendingHealth;

	// Token: 0x04002C3D RID: 11325
	public Sprite cardNone;

	// Token: 0x04002C3E RID: 11326
	public Sprite cardBackLarge;

	// Token: 0x04002C3F RID: 11327
	public Sprite cardBackSmall;

	// Token: 0x04002C40 RID: 11328
	private static Sprite cardBackLargeStatic;

	// Token: 0x04002C41 RID: 11329
	private static Sprite cardBackSmallStatic;

	// Token: 0x04002C42 RID: 11330
	[SerializeField]
	private TexasHoldEmUI texasHoldEmUI;

	// Token: 0x04002C43 RID: 11331
	[SerializeField]
	private BlackjackUI blackjackUI;

	// Token: 0x02000E77 RID: 3703
	[Serializable]
	public class PlayingCardImage
	{
		// Token: 0x04004BCF RID: 19407
		public Rank rank;

		// Token: 0x04004BD0 RID: 19408
		public Suit suit;

		// Token: 0x04004BD1 RID: 19409
		public Sprite image;

		// Token: 0x04004BD2 RID: 19410
		public Sprite imageSmall;

		// Token: 0x04004BD3 RID: 19411
		public Sprite imageTransparent;
	}

	// Token: 0x02000E78 RID: 3704
	[Serializable]
	public class InfoTextUI
	{
		// Token: 0x04004BD4 RID: 19412
		public GameObject gameObj;

		// Token: 0x04004BD5 RID: 19413
		public RustText rustText;

		// Token: 0x04004BD6 RID: 19414
		public Image background;

		// Token: 0x02000FD4 RID: 4052
		public enum Attitude
		{
			// Token: 0x040050FE RID: 20734
			Neutral,
			// Token: 0x040050FF RID: 20735
			Good,
			// Token: 0x04005100 RID: 20736
			Bad
		}
	}

	// Token: 0x02000E79 RID: 3705
	public interface ICardGameSubUI
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060052BA RID: 21178
		int DynamicBetAmount { get; }

		// Token: 0x060052BB RID: 21179
		void UpdateInGameUI(CardGameUI ui, CardGameController game);

		// Token: 0x060052BC RID: 21180
		string GetSecondaryInfo(CardGameUI ui, CardGameController game, out CardGameUI.InfoTextUI.Attitude attitude);

		// Token: 0x060052BD RID: 21181
		void UpdateInGameUI_NoPlayer(CardGameUI ui);
	}
}
