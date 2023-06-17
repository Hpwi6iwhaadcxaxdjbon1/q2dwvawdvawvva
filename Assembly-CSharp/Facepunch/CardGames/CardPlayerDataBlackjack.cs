using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF5 RID: 2805
	public class CardPlayerDataBlackjack : CardPlayerData
	{
		// Token: 0x04003CAA RID: 15530
		public List<PlayingCard> SplitCards;

		// Token: 0x04003CAB RID: 15531
		public int splitBetThisRound;

		// Token: 0x04003CAC RID: 15532
		public int insuranceBetThisRound;

		// Token: 0x04003CAD RID: 15533
		public bool playingSplitCards;

		// Token: 0x06004460 RID: 17504 RVA: 0x00191C7A File Offset: 0x0018FE7A
		public CardPlayerDataBlackjack(int mountIndex, bool isServer) : base(mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x00191C8F File Offset: 0x0018FE8F
		public CardPlayerDataBlackjack(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer) : base(scrapItemID, getStorage, mountIndex, isServer)
		{
			this.SplitCards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x00191CA7 File Offset: 0x0018FEA7
		public override void Dispose()
		{
			base.Dispose();
			Pool.FreeList<PlayingCard>(ref this.SplitCards);
		}

		// Token: 0x06004463 RID: 17507 RVA: 0x00191CBA File Offset: 0x0018FEBA
		public override int GetTotalBetThisRound()
		{
			return this.betThisRound + this.splitBetThisRound + this.insuranceBetThisRound;
		}

		// Token: 0x06004464 RID: 17508 RVA: 0x00191CD0 File Offset: 0x0018FED0
		public override List<PlayingCard> GetSecondaryCards()
		{
			return this.SplitCards;
		}

		// Token: 0x06004465 RID: 17509 RVA: 0x00191CD8 File Offset: 0x0018FED8
		protected override void ClearPerRoundData()
		{
			base.ClearPerRoundData();
			this.SplitCards.Clear();
			this.splitBetThisRound = 0;
			this.insuranceBetThisRound = 0;
			this.playingSplitCards = false;
		}

		// Token: 0x06004466 RID: 17510 RVA: 0x00191D00 File Offset: 0x0018FF00
		public override void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!base.HasUserInCurrentRound)
			{
				return;
			}
			if (clearBets)
			{
				this.splitBetThisRound = 0;
				this.insuranceBetThisRound = 0;
			}
			base.LeaveCurrentRound(clearBets, leftRoundEarly);
		}

		// Token: 0x06004467 RID: 17511 RVA: 0x00191D24 File Offset: 0x0018FF24
		public override void LeaveGame()
		{
			base.LeaveGame();
			if (!base.HasUserInGame)
			{
				return;
			}
			this.SplitCards.Clear();
		}

		// Token: 0x06004468 RID: 17512 RVA: 0x00191D40 File Offset: 0x0018FF40
		public override void Save(CardGame syncData)
		{
			base.Save(syncData);
			CardGame.BlackjackCardPlayer blackjackCardPlayer = Pool.Get<CardGame.BlackjackCardPlayer>();
			blackjackCardPlayer.splitCards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.SplitCards)
			{
				blackjackCardPlayer.splitCards.Add(base.SendCardDetails ? playingCard.GetIndex() : -1);
			}
			blackjackCardPlayer.splitBetThisRound = this.splitBetThisRound;
			blackjackCardPlayer.insuranceBetThisRound = this.insuranceBetThisRound;
			blackjackCardPlayer.playingSplitCards = this.playingSplitCards;
			if (syncData.blackjack.players == null)
			{
				syncData.blackjack.players = Pool.GetList<CardGame.BlackjackCardPlayer>();
			}
			syncData.blackjack.players.Add(blackjackCardPlayer);
		}

		// Token: 0x06004469 RID: 17513 RVA: 0x00191E14 File Offset: 0x00190014
		public bool TrySwitchToSplitHand()
		{
			if (this.SplitCards.Count > 0 && !this.playingSplitCards)
			{
				this.SwapSplitCardsWithMain();
				this.playingSplitCards = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600446A RID: 17514 RVA: 0x00191E3C File Offset: 0x0019003C
		private void SwapSplitCardsWithMain()
		{
			List<PlayingCard> list = Pool.GetList<PlayingCard>();
			list.AddRange(this.Cards);
			this.Cards.Clear();
			this.Cards.AddRange(this.SplitCards);
			this.SplitCards.Clear();
			this.SplitCards.AddRange(list);
			Pool.FreeList<PlayingCard>(ref list);
			int betThisRound = this.betThisRound;
			int betThisRound2 = this.splitBetThisRound;
			this.splitBetThisRound = betThisRound;
			this.betThisRound = betThisRound2;
		}
	}
}
