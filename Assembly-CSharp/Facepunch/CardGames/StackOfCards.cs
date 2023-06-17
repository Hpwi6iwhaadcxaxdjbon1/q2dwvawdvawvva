using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF6 RID: 2806
	public class StackOfCards
	{
		// Token: 0x04003CAE RID: 15534
		private List<PlayingCard> cards;

		// Token: 0x0600446B RID: 17515 RVA: 0x00191EB4 File Offset: 0x001900B4
		public StackOfCards(int numDecks)
		{
			this.cards = new List<PlayingCard>(52 * numDecks);
			for (int i = 0; i < numDecks; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 13; k++)
					{
						this.cards.Add(PlayingCard.GetCard(j, k));
					}
				}
			}
			this.ShuffleDeck();
		}

		// Token: 0x0600446C RID: 17516 RVA: 0x00191F14 File Offset: 0x00190114
		public bool TryTakeCard(out PlayingCard card)
		{
			if (this.cards.Count == 0)
			{
				card = null;
				return false;
			}
			card = this.cards[this.cards.Count - 1];
			this.cards.RemoveAt(this.cards.Count - 1);
			return true;
		}

		// Token: 0x0600446D RID: 17517 RVA: 0x00191F66 File Offset: 0x00190166
		public void AddCard(PlayingCard card)
		{
			this.cards.Insert(0, card);
		}

		// Token: 0x0600446E RID: 17518 RVA: 0x00191F78 File Offset: 0x00190178
		public void ShuffleDeck()
		{
			int i = this.cards.Count;
			while (i > 1)
			{
				i--;
				int index = UnityEngine.Random.Range(0, i);
				PlayingCard value = this.cards[index];
				this.cards[index] = this.cards[i];
				this.cards[i] = value;
			}
		}

		// Token: 0x0600446F RID: 17519 RVA: 0x00191FD8 File Offset: 0x001901D8
		public void Print()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Cards in the deck: ");
			foreach (PlayingCard playingCard in this.cards)
			{
				stringBuilder.AppendLine(playingCard.Rank + " of " + playingCard.Suit);
			}
			Debug.Log(stringBuilder.ToString());
		}
	}
}
