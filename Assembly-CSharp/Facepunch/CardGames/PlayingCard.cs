using System;
using PokerEvaluator;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF9 RID: 2809
	public class PlayingCard
	{
		// Token: 0x04003CC2 RID: 15554
		public readonly bool IsUnknownCard;

		// Token: 0x04003CC3 RID: 15555
		public readonly Suit Suit;

		// Token: 0x04003CC4 RID: 15556
		public readonly Rank Rank;

		// Token: 0x04003CC5 RID: 15557
		public static PlayingCard[] cards = PlayingCard.GenerateAllCards();

		// Token: 0x04003CC6 RID: 15558
		public static PlayingCard unknownCard = new PlayingCard();

		// Token: 0x06004470 RID: 17520 RVA: 0x00192068 File Offset: 0x00190268
		private PlayingCard(Suit suit, Rank rank)
		{
			this.IsUnknownCard = false;
			this.Suit = suit;
			this.Rank = rank;
		}

		// Token: 0x06004471 RID: 17521 RVA: 0x00192085 File Offset: 0x00190285
		private PlayingCard()
		{
			this.IsUnknownCard = true;
			this.Suit = Suit.Spades;
			this.Rank = Rank.Two;
		}

		// Token: 0x06004472 RID: 17522 RVA: 0x001920A2 File Offset: 0x001902A2
		public static PlayingCard GetCard(Suit suit, Rank rank)
		{
			return PlayingCard.GetCard((int)suit, (int)rank);
		}

		// Token: 0x06004473 RID: 17523 RVA: 0x001920AB File Offset: 0x001902AB
		public static PlayingCard GetCard(int suit, int rank)
		{
			return PlayingCard.cards[suit * 13 + rank];
		}

		// Token: 0x06004474 RID: 17524 RVA: 0x001920B9 File Offset: 0x001902B9
		public static PlayingCard GetCard(int index)
		{
			if (index == -1)
			{
				return PlayingCard.unknownCard;
			}
			return PlayingCard.cards[index];
		}

		// Token: 0x06004475 RID: 17525 RVA: 0x001920CC File Offset: 0x001902CC
		public int GetIndex()
		{
			if (this.IsUnknownCard)
			{
				return -1;
			}
			return PlayingCard.GetIndex(this.Suit, this.Rank);
		}

		// Token: 0x06004476 RID: 17526 RVA: 0x001920E9 File Offset: 0x001902E9
		public static int GetIndex(Suit suit, Rank rank)
		{
			return (int)(suit * (Suit)13 + (int)rank);
		}

		// Token: 0x06004477 RID: 17527 RVA: 0x001920F4 File Offset: 0x001902F4
		public int GetPokerEvaluationValue()
		{
			if (this.IsUnknownCard)
			{
				Debug.LogWarning(base.GetType().Name + ": Called GetPokerEvaluationValue on unknown card.");
			}
			return Arrays.primes[(int)this.Rank] | (int)((int)this.Rank << 8) | this.GetPokerSuitCode() | 1 << (int)(16 + this.Rank);
		}

		// Token: 0x06004478 RID: 17528 RVA: 0x00192150 File Offset: 0x00190350
		private int GetPokerSuitCode()
		{
			if (this.IsUnknownCard)
			{
				Debug.LogWarning(base.GetType().Name + ": Called GetPokerSuitCode on unknown card.");
			}
			switch (this.Suit)
			{
			case Suit.Spades:
				return 4096;
			case Suit.Hearts:
				return 8192;
			case Suit.Diamonds:
				return 16384;
			case Suit.Clubs:
				return 32768;
			default:
				return 4096;
			}
		}

		// Token: 0x06004479 RID: 17529 RVA: 0x001921BC File Offset: 0x001903BC
		private static PlayingCard[] GenerateAllCards()
		{
			PlayingCard[] array = new PlayingCard[52];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					array[i * 13 + j] = new PlayingCard((Suit)i, (Rank)j);
				}
			}
			return array;
		}
	}
}
