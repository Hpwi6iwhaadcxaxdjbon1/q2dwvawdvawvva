using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF2 RID: 2802
	public class BlackjackController : CardGameController
	{
		// Token: 0x04003C87 RID: 15495
		public List<PlayingCard> dealerCards = new List<PlayingCard>();

		// Token: 0x04003C88 RID: 15496
		public const float BLACKJACK_PAYOUT_RATIO = 1.5f;

		// Token: 0x04003C89 RID: 15497
		public const float INSURANCE_PAYOUT_RATIO = 2f;

		// Token: 0x04003C8A RID: 15498
		private const float DEALER_MOVE_TIME = 1f;

		// Token: 0x04003C8E RID: 15502
		private const int NUM_DECKS = 6;

		// Token: 0x04003C8F RID: 15503
		private StackOfCards cardStack = new StackOfCards(6);

		// Token: 0x060043AB RID: 17323 RVA: 0x0018F3FE File Offset: 0x0018D5FE
		public BlackjackController(BaseCardGameEntity owner) : base(owner)
		{
		}

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x060043AC RID: 17324 RVA: 0x0000441C File Offset: 0x0000261C
		public override int MinPlayers
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x060043AD RID: 17325 RVA: 0x0002179A File Offset: 0x0001F99A
		public override int MinBuyIn
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x060043AE RID: 17326 RVA: 0x0018F41E File Offset: 0x0018D61E
		public override int MaxBuyIn
		{
			get
			{
				return int.MaxValue;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x060043AF RID: 17327 RVA: 0x0018F425 File Offset: 0x0018D625
		public override int MinToPlay
		{
			get
			{
				return this.MinBuyIn;
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x060043B0 RID: 17328 RVA: 0x0000441C File Offset: 0x0000261C
		public override int EndRoundDelay
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x060043B1 RID: 17329 RVA: 0x0018F42D File Offset: 0x0018D62D
		public override int TimeBetweenRounds
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x060043B2 RID: 17330 RVA: 0x0018F430 File Offset: 0x0018D630
		// (set) Token: 0x060043B3 RID: 17331 RVA: 0x0018F438 File Offset: 0x0018D638
		public BlackjackController.BlackjackInputOption LastAction { get; private set; }

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x060043B4 RID: 17332 RVA: 0x0018F441 File Offset: 0x0018D641
		// (set) Token: 0x060043B5 RID: 17333 RVA: 0x0018F449 File Offset: 0x0018D649
		public ulong LastActionTarget { get; private set; }

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x060043B6 RID: 17334 RVA: 0x0018F452 File Offset: 0x0018D652
		// (set) Token: 0x060043B7 RID: 17335 RVA: 0x0018F45A File Offset: 0x0018D65A
		public int LastActionValue { get; private set; }

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x060043B8 RID: 17336 RVA: 0x0018F464 File Offset: 0x0018D664
		public bool AllBetsPlaced
		{
			get
			{
				if (!base.HasRoundInProgressOrEnding)
				{
					return false;
				}
				using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.betThisRound == 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x00007A3C File Offset: 0x00005C3C
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			return 0;
		}

		// Token: 0x060043BA RID: 17338 RVA: 0x0018F4C4 File Offset: 0x0018D6C4
		public override List<PlayingCard> GetTableCards()
		{
			return this.dealerCards;
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x0018F4CC File Offset: 0x0018D6CC
		public void InputsToList(int availableInputs, List<BlackjackController.BlackjackInputOption> result)
		{
			foreach (BlackjackController.BlackjackInputOption blackjackInputOption in (BlackjackController.BlackjackInputOption[])Enum.GetValues(typeof(BlackjackController.BlackjackInputOption)))
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.None && (availableInputs & (int)blackjackInputOption) == (int)blackjackInputOption)
				{
					result.Add(blackjackInputOption);
				}
			}
		}

		// Token: 0x060043BC RID: 17340 RVA: 0x0018F510 File Offset: 0x0018D710
		public bool WaitingForOtherPlayers(CardPlayerData pData)
		{
			if (!pData.HasUserInCurrentRound)
			{
				return false;
			}
			if (base.State == CardGameController.CardGameState.InGameRound && !pData.HasAvailableInputs)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					if (cardPlayerData != pData && cardPlayerData.HasAvailableInputs)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060043BD RID: 17341 RVA: 0x0018F584 File Offset: 0x0018D784
		public int GetCardsValue(List<PlayingCard> cards, BlackjackController.CardsValueMode mode)
		{
			int num = 0;
			foreach (PlayingCard playingCard in cards)
			{
				if (!playingCard.IsUnknownCard)
				{
					num += this.GetCardValue(playingCard, mode);
					if (playingCard.Rank == Rank.Ace)
					{
						mode = BlackjackController.CardsValueMode.Low;
					}
				}
			}
			return num;
		}

		// Token: 0x060043BE RID: 17342 RVA: 0x0018F5F0 File Offset: 0x0018D7F0
		public int GetOptimalCardsValue(List<PlayingCard> cards)
		{
			int cardsValue = this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low);
			int cardsValue2 = this.GetCardsValue(cards, BlackjackController.CardsValueMode.High);
			if (cardsValue2 <= 21)
			{
				return cardsValue2;
			}
			return cardsValue;
		}

		// Token: 0x060043BF RID: 17343 RVA: 0x0018F618 File Offset: 0x0018D818
		public int GetCardValue(PlayingCard card, BlackjackController.CardsValueMode mode)
		{
			int rank = (int)card.Rank;
			if (rank <= 8)
			{
				return rank + 2;
			}
			if (rank <= 11)
			{
				return 10;
			}
			if (mode != BlackjackController.CardsValueMode.Low)
			{
				return 11;
			}
			return 1;
		}

		// Token: 0x060043C0 RID: 17344 RVA: 0x0018F643 File Offset: 0x0018D843
		public bool Has21(List<PlayingCard> cards)
		{
			return this.GetOptimalCardsValue(cards) == 21;
		}

		// Token: 0x060043C1 RID: 17345 RVA: 0x0018F650 File Offset: 0x0018D850
		public bool HasBlackjack(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.High) == 21 && cards.Count == 2;
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x0018F669 File Offset: 0x0018D869
		public bool HasBusted(List<PlayingCard> cards)
		{
			return this.GetCardsValue(cards, BlackjackController.CardsValueMode.Low) > 21;
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x0018F678 File Offset: 0x0018D878
		private bool CanSplit(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasSplit(pData))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound && this.GetCardValue(pData.Cards[0], BlackjackController.CardsValueMode.Low) == this.GetCardValue(pData.Cards[1], BlackjackController.CardsValueMode.Low);
		}

		// Token: 0x060043C4 RID: 17348 RVA: 0x0018F6DC File Offset: 0x0018D8DC
		private bool HasAnyAces(List<PlayingCard> cards)
		{
			using (List<PlayingCard>.Enumerator enumerator = cards.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Rank == Rank.Ace)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x0018F734 File Offset: 0x0018D934
		private bool CanDoubleDown(CardPlayerDataBlackjack pData)
		{
			if (pData.Cards.Count != 2)
			{
				return false;
			}
			if (this.HasAnyAces(pData.Cards))
			{
				return false;
			}
			int betThisRound = pData.betThisRound;
			return pData.GetScrapAmount() >= betThisRound;
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x0018F774 File Offset: 0x0018D974
		private bool CanTakeInsurance(CardPlayerDataBlackjack pData)
		{
			if (this.dealerCards.Count != 2)
			{
				return false;
			}
			if (this.dealerCards[1].Rank != Rank.Ace)
			{
				return false;
			}
			if (pData.insuranceBetThisRound > 0)
			{
				return false;
			}
			int num = Mathf.FloorToInt((float)pData.betThisRound / 2f);
			return pData.GetScrapAmount() >= num;
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x0018F7D2 File Offset: 0x0018D9D2
		private bool HasSplit(CardPlayerDataBlackjack pData)
		{
			return pData.SplitCards.Count > 0;
		}

		// Token: 0x060043C8 RID: 17352 RVA: 0x0018F7E2 File Offset: 0x0018D9E2
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerDataBlackjack(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerDataBlackjack(mountIndex, base.IsServer);
		}

		// Token: 0x060043C9 RID: 17353 RVA: 0x0018F81C File Offset: 0x0018DA1C
		public bool TryGetCardPlayerDataBlackjack(int index, out CardPlayerDataBlackjack cpBlackjack)
		{
			CardPlayerData cardPlayerData;
			bool result = base.TryGetCardPlayerData(index, out cardPlayerData);
			cpBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
			return result;
		}

		// Token: 0x060043CA RID: 17354 RVA: 0x0018F83A File Offset: 0x0018DA3A
		public int ResultsToInt(BlackjackController.BlackjackRoundResult mainResult, BlackjackController.BlackjackRoundResult splitResult, int insurancePayout)
		{
			return (int)(mainResult + (int)((BlackjackController.BlackjackRoundResult)10 * splitResult) + 100 * insurancePayout);
		}

		// Token: 0x060043CB RID: 17355 RVA: 0x0018F847 File Offset: 0x0018DA47
		public void ResultsFromInt(int result, out BlackjackController.BlackjackRoundResult mainResult, out BlackjackController.BlackjackRoundResult splitResult, out int insurancePayout)
		{
			mainResult = (BlackjackController.BlackjackRoundResult)(result % 10);
			splitResult = (BlackjackController.BlackjackRoundResult)(result / 10 % 10);
			insurancePayout = (result - (int)mainResult - (int)splitResult) / 100;
		}

		// Token: 0x060043CC RID: 17356 RVA: 0x0018F868 File Offset: 0x0018DA68
		public override void Save(CardGame syncData)
		{
			syncData.blackjack = Pool.Get<CardGame.Blackjack>();
			syncData.blackjack.dealerCards = Pool.GetList<int>();
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			for (int i = 0; i < this.dealerCards.Count; i++)
			{
				PlayingCard playingCard = this.dealerCards[i];
				if (base.HasActiveRound && i == 0)
				{
					syncData.blackjack.dealerCards.Add(-1);
				}
				else
				{
					syncData.blackjack.dealerCards.Add(playingCard.GetIndex());
				}
			}
			base.Save(syncData);
			this.ClearLastAction();
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x0018F918 File Offset: 0x0018DB18
		private void EditorMakeRandomMove(CardPlayerDataBlackjack pdBlackjack)
		{
			List<BlackjackController.BlackjackInputOption> list = Pool.GetList<BlackjackController.BlackjackInputOption>();
			this.InputsToList(pdBlackjack.availableInputs, list);
			if (list.Count == 0)
			{
				Debug.Log("No moves currently available.");
				Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = list[UnityEngine.Random.Range(0, list.Count)];
			if (this.AllBetsPlaced)
			{
				if (this.GetOptimalCardsValue(pdBlackjack.Cards) < 17 && list.Contains(BlackjackController.BlackjackInputOption.Hit))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Hit;
				}
				else if (list.Contains(BlackjackController.BlackjackInputOption.Stand))
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
				}
			}
			else if (list.Contains(BlackjackController.BlackjackInputOption.SubmitBet))
			{
				blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
			}
			if (list.Count > 0)
			{
				int num = 0;
				if (blackjackInputOption == BlackjackController.BlackjackInputOption.SubmitBet)
				{
					num = this.MinBuyIn;
				}
				Debug.Log(string.Concat(new object[]
				{
					pdBlackjack.UserID,
					" Taking random action: ",
					blackjackInputOption,
					" with value ",
					num
				}));
				base.ReceivedInputFromPlayer(pdBlackjack, (int)blackjackInputOption, true, num, true);
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": No input options are available for the current player.");
			}
			Pool.FreeList<BlackjackController.BlackjackInputOption>(ref list);
		}

		// Token: 0x060043CE RID: 17358 RVA: 0x0018FA28 File Offset: 0x0018DC28
		protected override int GetAvailableInputsForPlayer(CardPlayerData pData)
		{
			BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.None;
			CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)pData;
			if (cardPlayerDataBlackjack == null || this.isWaitingBetweenTurns || cardPlayerDataBlackjack.hasCompletedTurn || !cardPlayerDataBlackjack.HasUserInCurrentRound)
			{
				return (int)blackjackInputOption;
			}
			if (!base.HasActiveRound)
			{
				return (int)blackjackInputOption;
			}
			if (this.AllBetsPlaced)
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.Stand;
				if (!this.Has21(cardPlayerDataBlackjack.Cards))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Hit;
				}
				if (this.CanSplit(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Split;
				}
				if (this.CanDoubleDown(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.DoubleDown;
				}
				if (this.CanTakeInsurance(cardPlayerDataBlackjack))
				{
					blackjackInputOption |= BlackjackController.BlackjackInputOption.Insurance;
				}
			}
			else
			{
				blackjackInputOption |= BlackjackController.BlackjackInputOption.SubmitBet;
				blackjackInputOption |= BlackjackController.BlackjackInputOption.MaxBet;
			}
			return (int)blackjackInputOption;
		}

		// Token: 0x060043CF RID: 17359 RVA: 0x0018FAB8 File Offset: 0x0018DCB8
		protected override void SubEndGameplay()
		{
			this.dealerCards.Clear();
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x0018FAC8 File Offset: 0x0018DCC8
		protected override void SubEndRound()
		{
			BlackjackController.<>c__DisplayClass59_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dealerCardsVal = this.GetOptimalCardsValue(this.dealerCards);
			if (CS$<>8__locals1.dealerCardsVal > 21)
			{
				CS$<>8__locals1.dealerCardsVal = 0;
			}
			base.resultInfo.winningScore = CS$<>8__locals1.dealerCardsVal;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
				return;
			}
			CS$<>8__locals1.dealerHasBlackjack = this.HasBlackjack(this.dealerCards);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				int num = 0;
				int num2;
				BlackjackController.BlackjackRoundResult mainResult = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.Cards, cardPlayerDataBlackjack.betThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				BlackjackController.BlackjackRoundResult splitResult = this.<SubEndRound>g__CheckResult|59_0(cardPlayerDataBlackjack.SplitCards, cardPlayerDataBlackjack.splitBetThisRound, out num2, ref CS$<>8__locals1);
				num += num2;
				int num3 = cardPlayerDataBlackjack.betThisRound + cardPlayerDataBlackjack.splitBetThisRound + cardPlayerDataBlackjack.insuranceBetThisRound;
				int insurancePayout = 0;
				if (CS$<>8__locals1.dealerHasBlackjack && cardPlayerDataBlackjack.insuranceBetThisRound > 0)
				{
					int num4 = Mathf.FloorToInt((float)cardPlayerDataBlackjack.insuranceBetThisRound * 3f);
					num += num4;
					insurancePayout = num4;
				}
				int resultCode = this.ResultsToInt(mainResult, splitResult, insurancePayout);
				this.AddRoundResult(cardPlayerDataBlackjack, num - num3, resultCode);
				this.PayOut(cardPlayerDataBlackjack, num);
			}
			base.ClearPot();
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
		}

		// Token: 0x060043D1 RID: 17361 RVA: 0x0018FC58 File Offset: 0x0018DE58
		private int PayOut(CardPlayerData pData, int winnings)
		{
			if (winnings == 0)
			{
				return 0;
			}
			StorageContainer storage = pData.GetStorage();
			if (storage == null)
			{
				return 0;
			}
			storage.inventory.AddItem(base.Owner.scrapItemDef, winnings, 0UL, global::ItemContainer.LimitStack.None);
			return winnings;
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x0018FC97 File Offset: 0x0018DE97
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 128, true, 0, false);
		}

		// Token: 0x060043D3 RID: 17363 RVA: 0x0018FCA8 File Offset: 0x0018DEA8
		protected override void SubReceivedInputFromPlayer(CardPlayerData pData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(BlackjackController.BlackjackInputOption), input))
			{
				return;
			}
			BlackjackController.BlackjackInputOption lastAction = (BlackjackController.BlackjackInputOption)input;
			CardPlayerDataBlackjack pdBlackjack = (CardPlayerDataBlackjack)pData;
			if (!base.HasActiveRound)
			{
				this.LastActionTarget = pData.UserID;
				this.LastAction = lastAction;
				this.LastActionValue = 0;
				return;
			}
			int lastActionValue = 0;
			if (this.AllBetsPlaced)
			{
				this.DoInRoundPlayerInput(pdBlackjack, ref lastAction, ref lastActionValue);
			}
			else
			{
				this.DoBettingPhasePlayerInput(pdBlackjack, value, countAsAction, ref lastAction, ref lastActionValue);
			}
			this.LastActionTarget = pData.UserID;
			this.LastAction = lastAction;
			this.LastActionValue = lastActionValue;
			if (base.NumPlayersInCurrentRound() == 0)
			{
				base.EndGameplay();
				return;
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			base.StartTurnTimer(pData, this.MaxTurnTime);
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060043D4 RID: 17364 RVA: 0x0018FD70 File Offset: 0x0018DF70
		private void DoInRoundPlayerInput(CardPlayerDataBlackjack pdBlackjack, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			BlackjackController.BlackjackInputOption blackjackInputOption = selectedMove;
			if (blackjackInputOption <= BlackjackController.BlackjackInputOption.Split)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Hit)
				{
					if (blackjackInputOption != BlackjackController.BlackjackInputOption.Stand)
					{
						if (blackjackInputOption == BlackjackController.BlackjackInputOption.Split)
						{
							PlayingCard playingCard = pdBlackjack.Cards[1];
							bool flag = playingCard.Rank == Rank.Ace;
							pdBlackjack.SplitCards.Add(playingCard);
							pdBlackjack.Cards.Remove(playingCard);
							PlayingCard item;
							this.cardStack.TryTakeCard(out item);
							pdBlackjack.Cards.Add(item);
							this.cardStack.TryTakeCard(out item);
							pdBlackjack.SplitCards.Add(item);
							selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Split);
							if (flag)
							{
								pdBlackjack.SetHasCompletedTurn(true);
							}
						}
					}
					else if (!pdBlackjack.TrySwitchToSplitHand())
					{
						pdBlackjack.SetHasCompletedTurn(true);
					}
				}
				else
				{
					PlayingCard item2;
					this.cardStack.TryTakeCard(out item2);
					pdBlackjack.Cards.Add(item2);
				}
			}
			else if (blackjackInputOption != BlackjackController.BlackjackInputOption.DoubleDown)
			{
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Insurance)
				{
					if (blackjackInputOption == BlackjackController.BlackjackInputOption.Abandon)
					{
						pdBlackjack.LeaveCurrentRound(false, true);
					}
				}
				else
				{
					int maxAmount = Mathf.FloorToInt((float)pdBlackjack.betThisRound / 2f);
					selectedMoveValue = this.TryMakeBet(pdBlackjack, maxAmount, BlackjackController.BetType.Insurance);
				}
			}
			else
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, pdBlackjack.betThisRound, BlackjackController.BetType.Main);
				PlayingCard item3;
				this.cardStack.TryTakeCard(out item3);
				pdBlackjack.Cards.Add(item3);
				if (!pdBlackjack.TrySwitchToSplitHand())
				{
					pdBlackjack.SetHasCompletedTurn(true);
				}
			}
			if (this.HasBusted(pdBlackjack.Cards) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
			if (this.Has21(pdBlackjack.Cards) && !this.CanTakeInsurance(pdBlackjack) && !this.CanDoubleDown(pdBlackjack) && !this.CanSplit(pdBlackjack) && !pdBlackjack.TrySwitchToSplitHand())
			{
				pdBlackjack.SetHasCompletedTurn(true);
			}
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x0018FF40 File Offset: 0x0018E140
		private void DoBettingPhasePlayerInput(CardPlayerDataBlackjack pdBlackjack, int value, bool countAsAction, ref BlackjackController.BlackjackInputOption selectedMove, ref int selectedMoveValue)
		{
			if (selectedMove != BlackjackController.BlackjackInputOption.Abandon && (pdBlackjack.availableInputs & (int)selectedMove) != (int)selectedMove)
			{
				return;
			}
			if (selectedMove == BlackjackController.BlackjackInputOption.SubmitBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, value, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.MaxBet)
			{
				selectedMoveValue = this.TryMakeBet(pdBlackjack, BlackjackMachine.maxbet, BlackjackController.BetType.Main);
				if (countAsAction)
				{
					pdBlackjack.SetHasCompletedTurn(true);
					return;
				}
			}
			else if (selectedMove == BlackjackController.BlackjackInputOption.Abandon)
			{
				pdBlackjack.LeaveCurrentRound(false, true);
			}
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x0018FFBC File Offset: 0x0018E1BC
		private int TryMakeBet(CardPlayerDataBlackjack pdBlackjack, int maxAmount, BlackjackController.BetType betType)
		{
			int num = base.TryMoveToPotStorage(pdBlackjack, maxAmount);
			switch (betType)
			{
			case BlackjackController.BetType.Main:
				pdBlackjack.betThisTurn += num;
				pdBlackjack.betThisRound += num;
				break;
			case BlackjackController.BetType.Split:
				pdBlackjack.splitBetThisRound += num;
				break;
			case BlackjackController.BetType.Insurance:
				pdBlackjack.insuranceBetThisRound += num;
				break;
			}
			return num;
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x00190024 File Offset: 0x0018E224
		protected override void SubStartRound()
		{
			this.dealerCards.Clear();
			this.cardStack = new StackOfCards(6);
			this.ClearLastAction();
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				cardPlayerDataBlackjack.EnableSendingCards();
				cardPlayerDataBlackjack.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerDataBlackjack);
				base.StartTurnTimer(cardPlayerDataBlackjack, this.MaxTurnTime);
			}
		}

		// Token: 0x060043D8 RID: 17368 RVA: 0x001900B4 File Offset: 0x0018E2B4
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			if (pData.HasUserInCurrentRound && !pData.hasCompletedTurn)
			{
				BlackjackController.BlackjackInputOption blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				int value = 0;
				if (this.AllBetsPlaced)
				{
					if ((pData.availableInputs & 4) == 4)
					{
						blackjackInputOption = BlackjackController.BlackjackInputOption.Stand;
						base.ReceivedInputFromPlayer(pData, 4, true, 0, false);
					}
				}
				else if ((pData.availableInputs & 1) == 1 && pData.GetScrapAmount() >= this.MinBuyIn)
				{
					blackjackInputOption = BlackjackController.BlackjackInputOption.SubmitBet;
					value = this.MinBuyIn;
				}
				if (blackjackInputOption != BlackjackController.BlackjackInputOption.Abandon)
				{
					base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, value, false);
					return;
				}
				blackjackInputOption = BlackjackController.BlackjackInputOption.Abandon;
				base.ReceivedInputFromPlayer(pData, (int)blackjackInputOption, true, 0, false);
				pData.ClearAllData();
				if (base.HasActiveRound && base.NumPlayersInCurrentRound() < this.MinPlayers)
				{
					base.BeginRoundEnd();
				}
				if (pData.HasUserInGame)
				{
					base.Owner.ClientRPC<ulong>(null, "ClientOnPlayerLeft", pData.UserID);
				}
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x0019019C File Offset: 0x0018E39C
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack pData = (CardPlayerDataBlackjack)cardPlayerData;
				base.StartTurnTimer(pData, this.MaxTurnTime);
			}
			base.UpdateAllAvailableInputs();
			base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x0019021C File Offset: 0x0018E41C
		protected override bool ShouldEndCycle()
		{
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.hasCompletedTurn)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x00190270 File Offset: 0x0018E470
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			if (this.dealerCards.Count == 0)
			{
				this.DealInitialCards();
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.QueueNextCycleInvoke();
				return;
			}
			bool flag = true;
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				CardPlayerDataBlackjack cardPlayerDataBlackjack = (CardPlayerDataBlackjack)cardPlayerData;
				if (!this.HasBusted(cardPlayerDataBlackjack.Cards))
				{
					flag = false;
				}
				if (!this.HasBlackjack(cardPlayerDataBlackjack.Cards))
				{
					flag2 = false;
				}
				if (cardPlayerDataBlackjack.SplitCards.Count > 0)
				{
					if (!this.HasBusted(cardPlayerDataBlackjack.SplitCards))
					{
						flag = false;
					}
					if (!this.HasBlackjack(cardPlayerDataBlackjack.SplitCards))
					{
						flag2 = false;
					}
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
			base.ServerPlaySound(CardGameSounds.SoundType.Draw);
			if (base.NumPlayersInCurrentRound() > 0 && !flag && !flag2)
			{
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.BeginRoundEnd();
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x001903A4 File Offset: 0x0018E5A4
		private void DealerPlayInvoke()
		{
			int cardsValue = this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.High);
			if (this.GetCardsValue(this.dealerCards, BlackjackController.CardsValueMode.Low) < 17 && (cardsValue < 17 || cardsValue > 21))
			{
				PlayingCard item;
				this.cardStack.TryTakeCard(out item);
				this.dealerCards.Add(item);
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				base.Owner.Invoke(new Action(this.DealerPlayInvoke), 1f);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x0019042C File Offset: 0x0018E62C
		private void DealInitialCards()
		{
			if (!base.HasActiveRound)
			{
				return;
			}
			PlayingCard item;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out item);
				cardPlayerData.Cards.Add(item);
			}
			this.cardStack.TryTakeCard(out item);
			this.dealerCards.Add(item);
			foreach (CardPlayerData cardPlayerData2 in base.PlayersInRound())
			{
				this.cardStack.TryTakeCard(out item);
				cardPlayerData2.Cards.Add(item);
				if (this.HasBlackjack(cardPlayerData2.Cards))
				{
					cardPlayerData2.SetHasCompletedTurn(true);
				}
			}
			this.cardStack.TryTakeCard(out item);
			this.dealerCards.Add(item);
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x0019052C File Offset: 0x0018E72C
		private void ClearLastAction()
		{
			this.LastAction = BlackjackController.BlackjackInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x00190544 File Offset: 0x0018E744
		[CompilerGenerated]
		private BlackjackController.BlackjackRoundResult <SubEndRound>g__CheckResult|59_0(List<PlayingCard> cards, int betAmount, out int winnings, ref BlackjackController.<>c__DisplayClass59_0 A_4)
		{
			if (cards.Count == 0)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.None;
			}
			int optimalCardsValue = this.GetOptimalCardsValue(cards);
			if (optimalCardsValue > 21)
			{
				winnings = 0;
				return BlackjackController.BlackjackRoundResult.Bust;
			}
			if (optimalCardsValue > base.resultInfo.winningScore)
			{
				base.resultInfo.winningScore = optimalCardsValue;
			}
			BlackjackController.BlackjackRoundResult blackjackRoundResult = BlackjackController.BlackjackRoundResult.Loss;
			bool flag = this.HasBlackjack(cards);
			if (A_4.dealerHasBlackjack)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			else if (optimalCardsValue > A_4.dealerCardsVal)
			{
				blackjackRoundResult = (flag ? BlackjackController.BlackjackRoundResult.BlackjackWin : BlackjackController.BlackjackRoundResult.Win);
			}
			else if (optimalCardsValue == A_4.dealerCardsVal)
			{
				if (flag)
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.BlackjackWin;
				}
				else
				{
					blackjackRoundResult = BlackjackController.BlackjackRoundResult.Standoff;
				}
			}
			if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.BlackjackWin)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2.5f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Win)
			{
				winnings = Mathf.FloorToInt((float)betAmount * 2f);
			}
			else if (blackjackRoundResult == BlackjackController.BlackjackRoundResult.Standoff)
			{
				winnings = betAmount;
			}
			else
			{
				winnings = 0;
			}
			return blackjackRoundResult;
		}

		// Token: 0x02000F75 RID: 3957
		[Flags]
		public enum BlackjackInputOption
		{
			// Token: 0x04004FB8 RID: 20408
			None = 0,
			// Token: 0x04004FB9 RID: 20409
			SubmitBet = 1,
			// Token: 0x04004FBA RID: 20410
			Hit = 2,
			// Token: 0x04004FBB RID: 20411
			Stand = 4,
			// Token: 0x04004FBC RID: 20412
			Split = 8,
			// Token: 0x04004FBD RID: 20413
			DoubleDown = 16,
			// Token: 0x04004FBE RID: 20414
			Insurance = 32,
			// Token: 0x04004FBF RID: 20415
			MaxBet = 64,
			// Token: 0x04004FC0 RID: 20416
			Abandon = 128
		}

		// Token: 0x02000F76 RID: 3958
		public enum BlackjackRoundResult
		{
			// Token: 0x04004FC2 RID: 20418
			None,
			// Token: 0x04004FC3 RID: 20419
			Bust,
			// Token: 0x04004FC4 RID: 20420
			Loss,
			// Token: 0x04004FC5 RID: 20421
			Standoff,
			// Token: 0x04004FC6 RID: 20422
			Win,
			// Token: 0x04004FC7 RID: 20423
			BlackjackWin
		}

		// Token: 0x02000F77 RID: 3959
		public enum CardsValueMode
		{
			// Token: 0x04004FC9 RID: 20425
			Low,
			// Token: 0x04004FCA RID: 20426
			High
		}

		// Token: 0x02000F78 RID: 3960
		private enum BetType
		{
			// Token: 0x04004FCC RID: 20428
			Main,
			// Token: 0x04004FCD RID: 20429
			Split,
			// Token: 0x04004FCE RID: 20430
			Insurance
		}
	}
}
