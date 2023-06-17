using System;
using System.Collections.Generic;
using PokerEvaluator;
using ProtoBuf;
using Rust;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AFA RID: 2810
	public class TexasHoldEmController : CardGameController
	{
		// Token: 0x04003CC7 RID: 15559
		public List<PlayingCard> communityCards = new List<PlayingCard>();

		// Token: 0x04003CC8 RID: 15560
		public const int SMALL_BLIND = 5;

		// Token: 0x04003CC9 RID: 15561
		public const int BIG_BLIND = 10;

		// Token: 0x04003CCA RID: 15562
		public const string WON_HAND_STAT = "won_hand_texas_holdem";

		// Token: 0x04003CCF RID: 15567
		private int dealerIndex;

		// Token: 0x04003CD0 RID: 15568
		private StackOfCards deck = new StackOfCards(1);

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x0600447B RID: 17531 RVA: 0x0004E73F File Offset: 0x0004C93F
		public override int MinPlayers
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x0600447C RID: 17532 RVA: 0x000070B1 File Offset: 0x000052B1
		public override int MinBuyIn
		{
			get
			{
				return 100;
			}
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x0600447D RID: 17533 RVA: 0x00192210 File Offset: 0x00190410
		public override int MaxBuyIn
		{
			get
			{
				return 1000;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x0600447E RID: 17534 RVA: 0x0002364E File Offset: 0x0002184E
		public override int MinToPlay
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x0600447F RID: 17535 RVA: 0x00192217 File Offset: 0x00190417
		// (set) Token: 0x06004480 RID: 17536 RVA: 0x0019221F File Offset: 0x0019041F
		public TexasHoldEmController.PokerInputOption LastAction { get; private set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06004481 RID: 17537 RVA: 0x00192228 File Offset: 0x00190428
		// (set) Token: 0x06004482 RID: 17538 RVA: 0x00192230 File Offset: 0x00190430
		public ulong LastActionTarget { get; private set; }

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06004483 RID: 17539 RVA: 0x00192239 File Offset: 0x00190439
		// (set) Token: 0x06004484 RID: 17540 RVA: 0x00192241 File Offset: 0x00190441
		public int LastActionValue { get; private set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06004485 RID: 17541 RVA: 0x0019224A File Offset: 0x0019044A
		// (set) Token: 0x06004486 RID: 17542 RVA: 0x00192252 File Offset: 0x00190452
		public int BiggestRaiseThisTurn { get; private set; }

		// Token: 0x06004487 RID: 17543 RVA: 0x0019225B File Offset: 0x0019045B
		public TexasHoldEmController(BaseCardGameEntity owner) : base(owner)
		{
		}

		// Token: 0x06004488 RID: 17544 RVA: 0x0019227C File Offset: 0x0019047C
		public int GetCurrentBet()
		{
			int num = 0;
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				num = Mathf.Max(num, cardPlayerData.betThisTurn);
			}
			return num;
		}

		// Token: 0x06004489 RID: 17545 RVA: 0x001922D4 File Offset: 0x001904D4
		public bool TryGetDealer(out CardPlayerData dealer)
		{
			return base.ToCardPlayerData(this.dealerIndex, true, out dealer);
		}

		// Token: 0x0600448A RID: 17546 RVA: 0x001922E4 File Offset: 0x001904E4
		public bool TryGetSmallBlind(out CardPlayerData smallBlind)
		{
			int relIndex = (base.NumPlayersInGame() < 3) ? this.dealerIndex : (this.dealerIndex + 1);
			return base.ToCardPlayerData(relIndex, true, out smallBlind);
		}

		// Token: 0x0600448B RID: 17547 RVA: 0x00192314 File Offset: 0x00190514
		public bool TryGetBigBlind(out CardPlayerData bigBlind)
		{
			int relIndex = (base.NumPlayersInGame() < 3) ? (this.dealerIndex + 1) : (this.dealerIndex + 2);
			return base.ToCardPlayerData(relIndex, true, out bigBlind);
		}

		// Token: 0x0600448C RID: 17548 RVA: 0x00192348 File Offset: 0x00190548
		protected override int GetFirstPlayerRelIndex(bool startOfRound)
		{
			int num = base.NumPlayersInGame();
			if (startOfRound && num == 2)
			{
				return this.dealerIndex;
			}
			return (this.dealerIndex + 1) % num;
		}

		// Token: 0x0600448D RID: 17549 RVA: 0x00192374 File Offset: 0x00190574
		public static ushort EvaluatePokerHand(List<PlayingCard> cards)
		{
			ushort result = 0;
			int[] array = new int[cards.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = cards[i].GetPokerEvaluationValue();
			}
			if (cards.Count == 5)
			{
				result = PokerLib.Eval5Hand(array);
			}
			else if (cards.Count == 7)
			{
				result = PokerLib.Eval7Hand(array);
			}
			else
			{
				Debug.LogError("Currently we can only evaluate five or seven card hands.");
			}
			return result;
		}

		// Token: 0x0600448E RID: 17550 RVA: 0x001923DB File Offset: 0x001905DB
		public int GetCurrentMinRaise(CardPlayerData playerData)
		{
			return Mathf.Max(10, this.GetCurrentBet() - playerData.betThisTurn + this.BiggestRaiseThisTurn);
		}

		// Token: 0x0600448F RID: 17551 RVA: 0x001923F8 File Offset: 0x001905F8
		public override List<PlayingCard> GetTableCards()
		{
			return this.communityCards;
		}

		// Token: 0x06004490 RID: 17552 RVA: 0x00192400 File Offset: 0x00190600
		public void InputsToList(int availableInputs, List<TexasHoldEmController.PokerInputOption> result)
		{
			foreach (TexasHoldEmController.PokerInputOption pokerInputOption in (TexasHoldEmController.PokerInputOption[])Enum.GetValues(typeof(TexasHoldEmController.PokerInputOption)))
			{
				if (pokerInputOption != TexasHoldEmController.PokerInputOption.None && (availableInputs & (int)pokerInputOption) == (int)pokerInputOption)
				{
					result.Add(pokerInputOption);
				}
			}
		}

		// Token: 0x06004491 RID: 17553 RVA: 0x00192444 File Offset: 0x00190644
		protected override CardPlayerData GetNewCardPlayerData(int mountIndex)
		{
			if (base.IsServer)
			{
				return new CardPlayerData(base.ScrapItemID, new Func<int, StorageContainer>(base.Owner.GetPlayerStorage), mountIndex, base.IsServer);
			}
			return new CardPlayerData(mountIndex, base.IsServer);
		}

		// Token: 0x06004492 RID: 17554 RVA: 0x00192480 File Offset: 0x00190680
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			syncData.texasHoldEm = Pool.Get<CardGame.TexasHoldEm>();
			syncData.texasHoldEm.dealerIndex = this.dealerIndex;
			syncData.texasHoldEm.communityCards = Pool.GetList<int>();
			syncData.texasHoldEm.biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
			syncData.lastActionId = (int)this.LastAction;
			syncData.lastActionTarget = this.LastActionTarget;
			syncData.lastActionValue = this.LastActionValue;
			foreach (PlayingCard playingCard in this.communityCards)
			{
				syncData.texasHoldEm.communityCards.Add(playingCard.GetIndex());
			}
			this.ClearLastAction();
		}

		// Token: 0x06004493 RID: 17555 RVA: 0x00192550 File Offset: 0x00190750
		protected override void SubStartRound()
		{
			this.communityCards.Clear();
			this.deck = new StackOfCards(1);
			this.BiggestRaiseThisTurn = 0;
			this.ClearLastAction();
			this.IncrementDealer();
			this.DealHoleCards();
			this.activePlayerIndex = this.GetFirstPlayerRelIndex(true);
			base.ServerPlaySound(CardGameSounds.SoundType.Shuffle);
			CardPlayerData cardPlayerData;
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 32) == 32)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 32, false, 5, false);
			}
			else
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 5, false);
			}
			base.TryGetActivePlayer(out cardPlayerData);
			cardPlayerData.availableInputs = this.GetAvailableInputsForPlayer(cardPlayerData);
			if ((cardPlayerData.availableInputs & 16) == 16)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 16, false, 10, false);
				return;
			}
			base.ReceivedInputFromPlayer(cardPlayerData, 4, false, 10, false);
		}

		// Token: 0x06004494 RID: 17556 RVA: 0x0019261C File Offset: 0x0019081C
		protected override void SubEndRound()
		{
			int num = 0;
			List<CardPlayerData> list = Pool.GetList<CardPlayerData>();
			foreach (CardPlayerData cardPlayerData in base.PlayerData)
			{
				if (cardPlayerData.betThisRound > 0)
				{
					list.Add(cardPlayerData);
				}
				if (cardPlayerData.HasUserInCurrentRound)
				{
					num++;
				}
			}
			if (list.Count == 0)
			{
				base.Owner.GetPot().inventory.Clear();
				return;
			}
			bool flag = num > 1;
			int num2 = base.GetScrapInPot();
			foreach (CardPlayerData cardPlayerData2 in base.PlayerData)
			{
				if (cardPlayerData2.HasUserInGame)
				{
					num2 -= cardPlayerData2.betThisRound;
				}
			}
			bool flag2 = true;
			foreach (CardPlayerData cardPlayerData3 in base.PlayerData)
			{
				cardPlayerData3.remainingToPayOut = cardPlayerData3.betThisRound;
			}
			while (list.Count > 1)
			{
				int num3 = int.MaxValue;
				int num4 = 0;
				foreach (CardPlayerData cardPlayerData4 in base.PlayerData)
				{
					if (cardPlayerData4.betThisRound > 0)
					{
						if (cardPlayerData4.betThisRound < num3)
						{
							num3 = cardPlayerData4.betThisRound;
						}
						num4++;
					}
				}
				int num5 = num3 * num4;
				foreach (CardPlayerData cardPlayerData5 in list)
				{
					cardPlayerData5.betThisRound -= num3;
				}
				int num6 = int.MaxValue;
				foreach (CardPlayerData cardPlayerData6 in base.PlayersInRound())
				{
					if (cardPlayerData6.finalScore < num6)
					{
						num6 = cardPlayerData6.finalScore;
					}
				}
				if (flag2)
				{
					base.resultInfo.winningScore = num6;
				}
				int num7 = 0;
				using (IEnumerator<CardPlayerData> enumerator2 = base.PlayersInRound().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.finalScore == num6)
						{
							num7++;
						}
					}
				}
				int num8 = Mathf.CeilToInt((float)(num5 + num2) / (float)num7);
				num2 = 0;
				foreach (CardPlayerData cardPlayerData7 in base.PlayersInRound())
				{
					if (cardPlayerData7.finalScore == num6)
					{
						if (flag)
						{
							cardPlayerData7.EnableSendingCards();
						}
						base.PayOutFromPot(cardPlayerData7, num8);
						TexasHoldEmController.PokerRoundResult resultCode = flag2 ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner;
						this.AddRoundResult(cardPlayerData7, num8, (int)resultCode);
					}
				}
				for (int j = list.Count - 1; j >= 0; j--)
				{
					if (list[j].betThisRound == 0)
					{
						list.RemoveAt(j);
					}
				}
				flag2 = false;
			}
			if (list.Count == 1)
			{
				int num9 = list[0].betThisRound + num2;
				num2 = 0;
				base.PayOutFromPot(list[0], num9);
				TexasHoldEmController.PokerRoundResult resultCode2 = (base.resultInfo.results.Count == 0) ? TexasHoldEmController.PokerRoundResult.PrimaryWinner : TexasHoldEmController.PokerRoundResult.SecondaryWinner;
				this.AddRoundResult(list[0], num9, (int)resultCode2);
			}
			base.Owner.ClientRPC<CardGame.RoundResults>(null, "OnResultsDeclared", base.resultInfo);
			StorageContainer pot = base.Owner.GetPot();
			int amount = pot.inventory.GetAmount(base.ScrapItemID, true);
			if (amount > 0)
			{
				Debug.LogError(string.Format("{0}: Something went wrong in the winner calculation. Pot still has {1} scrap left over after payouts. Expected 0. Clearing it.", base.GetType().Name, amount));
				pot.inventory.Clear();
			}
			Pool.FreeList<CardPlayerData>(ref list);
		}

		// Token: 0x06004495 RID: 17557 RVA: 0x001929DC File Offset: 0x00190BDC
		protected override void AddRoundResult(CardPlayerData pData, int winnings, int winState)
		{
			base.AddRoundResult(pData, winnings, winState);
			if (GameInfo.HasAchievements)
			{
				global::BasePlayer basePlayer = base.Owner.IDToPlayer(pData.UserID);
				if (basePlayer != null)
				{
					basePlayer.stats.Add("won_hand_texas_holdem", 1, Stats.Steam);
					basePlayer.stats.Save(true);
				}
			}
		}

		// Token: 0x06004496 RID: 17558 RVA: 0x00192A32 File Offset: 0x00190C32
		protected override void SubEndGameplay()
		{
			this.communityCards.Clear();
		}

		// Token: 0x06004497 RID: 17559 RVA: 0x00192A40 File Offset: 0x00190C40
		private void IncrementDealer()
		{
			int num = base.NumPlayersInGame();
			if (num == 0)
			{
				this.dealerIndex = 0;
				return;
			}
			this.dealerIndex = Mathf.Clamp(this.dealerIndex, 0, num - 1);
			int num2 = this.dealerIndex + 1;
			this.dealerIndex = num2;
			this.dealerIndex = num2 % num;
		}

		// Token: 0x06004498 RID: 17560 RVA: 0x00192A90 File Offset: 0x00190C90
		private void DealHoleCards()
		{
			for (int i = 0; i < 2; i++)
			{
				foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
				{
					PlayingCard item;
					if (this.deck.TryTakeCard(out item))
					{
						cardPlayerData.Cards.Add(item);
					}
					else
					{
						Debug.LogError(base.GetType().Name + ": No more cards in the deck to deal!");
					}
				}
			}
			base.SyncAllLocalPlayerCards();
		}

		// Token: 0x06004499 RID: 17561 RVA: 0x00192B20 File Offset: 0x00190D20
		private bool DealCommunityCards()
		{
			if (!base.HasActiveRound)
			{
				return false;
			}
			if (this.communityCards.Count == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					PlayingCard item;
					if (this.deck.TryTakeCard(out item))
					{
						this.communityCards.Add(item);
					}
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			if (this.communityCards.Count == 3 || this.communityCards.Count == 4)
			{
				PlayingCard item2;
				if (this.deck.TryTakeCard(out item2))
				{
					this.communityCards.Add(item2);
				}
				base.ServerPlaySound(CardGameSounds.SoundType.Draw);
				return true;
			}
			return false;
		}

		// Token: 0x0600449A RID: 17562 RVA: 0x00192BB5 File Offset: 0x00190DB5
		private void ClearLastAction()
		{
			this.LastAction = TexasHoldEmController.PokerInputOption.None;
			this.LastActionTarget = 0UL;
			this.LastActionValue = 0;
		}

		// Token: 0x0600449B RID: 17563 RVA: 0x00192BD0 File Offset: 0x00190DD0
		protected override void OnTurnTimeout(CardPlayerData pData)
		{
			CardPlayerData cardPlayerData;
			if (base.TryGetActivePlayer(out cardPlayerData) && cardPlayerData == pData)
			{
				base.ReceivedInputFromPlayer(cardPlayerData, 1, true, 0, false);
			}
		}

		// Token: 0x0600449C RID: 17564 RVA: 0x00192BF8 File Offset: 0x00190DF8
		protected override void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction)
		{
			if (!Enum.IsDefined(typeof(TexasHoldEmController.PokerInputOption), input))
			{
				return;
			}
			if (!base.HasActiveRound)
			{
				if (input == 64)
				{
					playerData.EnableSendingCards();
					playerData.availableInputs = this.GetAvailableInputsForPlayer(playerData);
					base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				}
				this.LastActionTarget = playerData.UserID;
				this.LastAction = (TexasHoldEmController.PokerInputOption)input;
				this.LastActionValue = 0;
				return;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData))
			{
				return;
			}
			if (cardPlayerData != playerData)
			{
				return;
			}
			bool flag = false;
			if ((playerData.availableInputs & input) != input)
			{
				return;
			}
			if (input <= 8)
			{
				switch (input)
				{
				case 1:
					playerData.LeaveCurrentRound(false, true);
					flag = true;
					this.LastActionValue = 0;
					break;
				case 2:
				{
					int currentBet = this.GetCurrentBet();
					int num = base.TryAddBet(playerData, currentBet - playerData.betThisTurn);
					this.LastActionValue = num;
					break;
				}
				case 3:
					break;
				case 4:
				{
					int currentBet = this.GetCurrentBet();
					int num = base.GoAllIn(playerData);
					this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num - currentBet);
					this.LastActionValue = num;
					break;
				}
				default:
					if (input == 8)
					{
						this.LastActionValue = 0;
					}
					break;
				}
			}
			else if (input == 16 || input == 32)
			{
				int currentBet = this.GetCurrentBet();
				int biggestRaiseThisTurn = this.BiggestRaiseThisTurn;
				if (playerData.betThisTurn + value < currentBet + biggestRaiseThisTurn)
				{
					value = currentBet + biggestRaiseThisTurn - playerData.betThisTurn;
				}
				int num = base.TryAddBet(playerData, value);
				this.BiggestRaiseThisTurn = Mathf.Max(this.BiggestRaiseThisTurn, num - currentBet);
				this.LastActionValue = num;
			}
			if (countAsAction && input != 0)
			{
				playerData.SetHasCompletedTurn(true);
			}
			this.LastActionTarget = playerData.UserID;
			this.LastAction = (TexasHoldEmController.PokerInputOption)input;
			if (flag && base.NumPlayersInCurrentRound() == 1)
			{
				base.EndRoundWithDelay();
				return;
			}
			int startIndex = this.activePlayerIndex;
			if (flag)
			{
				if (this.activePlayerIndex > base.NumPlayersInCurrentRound() - 1)
				{
					startIndex = 0;
				}
			}
			else
			{
				startIndex = (this.activePlayerIndex + 1) % base.NumPlayersInCurrentRound();
			}
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData pData;
			if (base.TryMoveToNextPlayerWithInputs(startIndex, out pData))
			{
				base.StartTurnTimer(pData, this.MaxTurnTime);
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x0600449D RID: 17565 RVA: 0x00192E24 File Offset: 0x00191024
		protected override void StartNextCycle()
		{
			base.StartNextCycle();
			int num = this.GetFirstPlayerRelIndex(false);
			int num2 = base.NumPlayersInGame();
			int num3 = 0;
			CardPlayerData cardPlayerData;
			while (!base.ToCardPlayerData(num, true, out cardPlayerData) || !cardPlayerData.HasUserInCurrentRound)
			{
				num = (num + 1) % num2;
				num3++;
				if (num3 > num2)
				{
					Debug.LogError(base.GetType().Name + ": This should never happen. Ended turn with no players in game?.");
					base.EndRoundWithDelay();
					return;
				}
			}
			int num4 = base.GameToRoundIndex(num);
			if (num4 < 0 || num4 > base.NumPlayersInCurrentRound())
			{
				Debug.LogError(string.Format("StartNextCycle NewActiveIndex is out of range: {0}. Clamping it to between 0 and {1}.", num4, base.NumPlayersInCurrentRound()));
				num4 = Mathf.Clamp(num4, 0, base.NumPlayersInCurrentRound());
			}
			int startIndex = num4;
			if (this.ShouldEndCycle())
			{
				this.EndCycle();
				return;
			}
			CardPlayerData pData;
			if (base.TryMoveToNextPlayerWithInputs(startIndex, out pData))
			{
				base.StartTurnTimer(pData, this.MaxTurnTime);
				base.UpdateAllAvailableInputs();
				base.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			this.EndCycle();
		}

		// Token: 0x0600449E RID: 17566 RVA: 0x00192F1C File Offset: 0x0019111C
		protected override bool ShouldEndCycle()
		{
			int num = 0;
			using (IEnumerator<CardPlayerData> enumerator = base.PlayersInRound().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetScrapAmount() > 0)
					{
						num++;
					}
				}
			}
			if (num == 1)
			{
				return true;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				if (cardPlayerData.GetScrapAmount() > 0 && (cardPlayerData.betThisTurn != this.GetCurrentBet() || !cardPlayerData.hasCompletedTurn))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600449F RID: 17567 RVA: 0x00192FD0 File Offset: 0x001911D0
		protected override void EndCycle()
		{
			CardPlayerData[] playerData = base.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].SetHasCompletedTurn(false);
			}
			this.BiggestRaiseThisTurn = 0;
			if (this.DealCommunityCards())
			{
				base.QueueNextCycleInvoke();
				return;
			}
			foreach (CardPlayerData cardPlayerData in base.PlayersInRound())
			{
				List<PlayingCard> list = Pool.GetList<PlayingCard>();
				list.AddRange(cardPlayerData.Cards);
				list.AddRange(this.communityCards);
				ushort finalScore = TexasHoldEmController.EvaluatePokerHand(list);
				Pool.FreeList<PlayingCard>(ref list);
				cardPlayerData.finalScore = (int)finalScore;
			}
			base.EndRoundWithDelay();
		}

		// Token: 0x060044A0 RID: 17568 RVA: 0x0019308C File Offset: 0x0019128C
		protected override int GetAvailableInputsForPlayer(CardPlayerData playerData)
		{
			TexasHoldEmController.PokerInputOption pokerInputOption = TexasHoldEmController.PokerInputOption.None;
			if (playerData == null || this.isWaitingBetweenTurns)
			{
				return (int)pokerInputOption;
			}
			if (!base.HasActiveRound)
			{
				if (!playerData.LeftRoundEarly && playerData.Cards.Count > 0 && !playerData.SendCardDetails)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.RevealHand;
				}
				return (int)pokerInputOption;
			}
			CardPlayerData cardPlayerData;
			if (!base.TryGetActivePlayer(out cardPlayerData) || playerData != cardPlayerData)
			{
				return (int)pokerInputOption;
			}
			int scrapAmount = playerData.GetScrapAmount();
			if (scrapAmount > 0)
			{
				pokerInputOption |= TexasHoldEmController.PokerInputOption.AllIn;
				pokerInputOption |= TexasHoldEmController.PokerInputOption.Fold;
				int currentBet = this.GetCurrentBet();
				if (playerData.betThisTurn >= currentBet)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Check;
				}
				if (currentBet > playerData.betThisTurn && scrapAmount >= currentBet - playerData.betThisTurn)
				{
					pokerInputOption |= TexasHoldEmController.PokerInputOption.Call;
				}
				if (scrapAmount >= this.GetCurrentMinRaise(playerData))
				{
					if (this.BiggestRaiseThisTurn == 0)
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Bet;
					}
					else
					{
						pokerInputOption |= TexasHoldEmController.PokerInputOption.Raise;
					}
				}
			}
			return (int)pokerInputOption;
		}

		// Token: 0x060044A1 RID: 17569 RVA: 0x00193143 File Offset: 0x00191343
		protected override void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData)
		{
			base.ReceivedInputFromPlayer(pData, 1, true, 0, false);
		}

		// Token: 0x02000F80 RID: 3968
		[Flags]
		public enum PokerInputOption
		{
			// Token: 0x04004FEC RID: 20460
			None = 0,
			// Token: 0x04004FED RID: 20461
			Fold = 1,
			// Token: 0x04004FEE RID: 20462
			Call = 2,
			// Token: 0x04004FEF RID: 20463
			AllIn = 4,
			// Token: 0x04004FF0 RID: 20464
			Check = 8,
			// Token: 0x04004FF1 RID: 20465
			Raise = 16,
			// Token: 0x04004FF2 RID: 20466
			Bet = 32,
			// Token: 0x04004FF3 RID: 20467
			RevealHand = 64
		}

		// Token: 0x02000F81 RID: 3969
		public enum PokerRoundResult
		{
			// Token: 0x04004FF5 RID: 20469
			Loss,
			// Token: 0x04004FF6 RID: 20470
			PrimaryWinner,
			// Token: 0x04004FF7 RID: 20471
			SecondaryWinner
		}
	}
}
