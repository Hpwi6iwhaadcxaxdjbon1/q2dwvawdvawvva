using System;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF4 RID: 2804
	public class CardPlayerData : IDisposable
	{
		// Token: 0x04003C9A RID: 15514
		public List<PlayingCard> Cards;

		// Token: 0x04003C9C RID: 15516
		public readonly int mountIndex;

		// Token: 0x04003C9D RID: 15517
		private readonly bool isServer;

		// Token: 0x04003C9E RID: 15518
		public int availableInputs;

		// Token: 0x04003C9F RID: 15519
		public int betThisRound;

		// Token: 0x04003CA0 RID: 15520
		public int betThisTurn;

		// Token: 0x04003CA3 RID: 15523
		public int finalScore;

		// Token: 0x04003CA5 RID: 15525
		public float lastActionTime;

		// Token: 0x04003CA6 RID: 15526
		public int remainingToPayOut;

		// Token: 0x04003CA7 RID: 15527
		private Func<int, StorageContainer> getStorage;

		// Token: 0x04003CA8 RID: 15528
		private readonly int scrapItemID;

		// Token: 0x04003CA9 RID: 15529
		private Action<CardPlayerData> turnTimerCallback;

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06004439 RID: 17465 RVA: 0x00191791 File Offset: 0x0018F991
		// (set) Token: 0x0600443A RID: 17466 RVA: 0x00191799 File Offset: 0x0018F999
		public ulong UserID { get; private set; }

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x0600443B RID: 17467 RVA: 0x001917A2 File Offset: 0x0018F9A2
		// (set) Token: 0x0600443C RID: 17468 RVA: 0x001917AA File Offset: 0x0018F9AA
		public CardPlayerData.CardPlayerState State { get; private set; }

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x0600443D RID: 17469 RVA: 0x001917B3 File Offset: 0x0018F9B3
		public bool HasUser
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.WantsToPlay;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600443E RID: 17470 RVA: 0x001917C1 File Offset: 0x0018F9C1
		public bool HasUserInGame
		{
			get
			{
				return this.State >= CardPlayerData.CardPlayerState.InGame;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x0600443F RID: 17471 RVA: 0x001917CF File Offset: 0x0018F9CF
		public bool HasUserInCurrentRound
		{
			get
			{
				return this.State == CardPlayerData.CardPlayerState.InCurrentRound;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06004440 RID: 17472 RVA: 0x001917DA File Offset: 0x0018F9DA
		public bool HasAvailableInputs
		{
			get
			{
				return this.availableInputs > 0;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06004441 RID: 17473 RVA: 0x001917E8 File Offset: 0x0018F9E8
		public bool AllCardsAreKnown
		{
			get
			{
				if (this.Cards.Count == 0)
				{
					return false;
				}
				using (List<PlayingCard>.Enumerator enumerator = this.Cards.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsUnknownCard)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06004442 RID: 17474 RVA: 0x00191850 File Offset: 0x0018FA50
		private bool IsClient
		{
			get
			{
				return !this.isServer;
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06004443 RID: 17475 RVA: 0x0019185B File Offset: 0x0018FA5B
		// (set) Token: 0x06004444 RID: 17476 RVA: 0x00191863 File Offset: 0x0018FA63
		public bool LeftRoundEarly { get; private set; }

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06004445 RID: 17477 RVA: 0x0019186C File Offset: 0x0018FA6C
		// (set) Token: 0x06004446 RID: 17478 RVA: 0x00191874 File Offset: 0x0018FA74
		public bool SendCardDetails { get; private set; }

		// Token: 0x06004447 RID: 17479 RVA: 0x0019187D File Offset: 0x0018FA7D
		public CardPlayerData(int mountIndex, bool isServer)
		{
			this.isServer = isServer;
			this.mountIndex = mountIndex;
			this.Cards = Pool.GetList<PlayingCard>();
		}

		// Token: 0x06004448 RID: 17480 RVA: 0x0019189E File Offset: 0x0018FA9E
		public CardPlayerData(int scrapItemID, Func<int, StorageContainer> getStorage, int mountIndex, bool isServer) : this(mountIndex, isServer)
		{
			this.scrapItemID = scrapItemID;
			this.getStorage = getStorage;
		}

		// Token: 0x06004449 RID: 17481 RVA: 0x001918B7 File Offset: 0x0018FAB7
		public virtual void Dispose()
		{
			Pool.FreeList<PlayingCard>(ref this.Cards);
			if (this.isServer)
			{
				this.CancelTurnTimer();
			}
		}

		// Token: 0x0600444A RID: 17482 RVA: 0x001918D4 File Offset: 0x0018FAD4
		public int GetScrapAmount()
		{
			if (this.isServer)
			{
				StorageContainer storage = this.GetStorage();
				if (storage != null)
				{
					return storage.inventory.GetAmount(this.scrapItemID, true);
				}
				Debug.LogError(base.GetType().Name + ": Couldn't get player storage.");
			}
			return 0;
		}

		// Token: 0x0600444B RID: 17483 RVA: 0x00191927 File Offset: 0x0018FB27
		public virtual int GetTotalBetThisRound()
		{
			return this.betThisRound;
		}

		// Token: 0x0600444C RID: 17484 RVA: 0x0019192F File Offset: 0x0018FB2F
		public virtual List<PlayingCard> GetMainCards()
		{
			return this.Cards;
		}

		// Token: 0x0600444D RID: 17485 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
		public virtual List<PlayingCard> GetSecondaryCards()
		{
			return null;
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x0600444E RID: 17486 RVA: 0x00191937 File Offset: 0x0018FB37
		// (set) Token: 0x0600444F RID: 17487 RVA: 0x0019193F File Offset: 0x0018FB3F
		public bool hasCompletedTurn { get; private set; }

		// Token: 0x06004450 RID: 17488 RVA: 0x00191948 File Offset: 0x0018FB48
		public void SetHasCompletedTurn(bool hasActed)
		{
			this.hasCompletedTurn = hasActed;
			if (!hasActed)
			{
				this.betThisTurn = 0;
			}
		}

		// Token: 0x06004451 RID: 17489 RVA: 0x0019195B File Offset: 0x0018FB5B
		public bool HasBeenIdleFor(int seconds)
		{
			return this.HasUserInGame && Time.unscaledTime > this.lastActionTime + (float)seconds;
		}

		// Token: 0x06004452 RID: 17490 RVA: 0x00191977 File Offset: 0x0018FB77
		public StorageContainer GetStorage()
		{
			return this.getStorage(this.mountIndex);
		}

		// Token: 0x06004453 RID: 17491 RVA: 0x0019198A File Offset: 0x0018FB8A
		public void AddUser(ulong userID)
		{
			this.ClearAllData();
			this.UserID = userID;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
			this.lastActionTime = Time.unscaledTime;
		}

		// Token: 0x06004454 RID: 17492 RVA: 0x001919AB File Offset: 0x0018FBAB
		public void ClearAllData()
		{
			this.UserID = 0UL;
			this.availableInputs = 0;
			this.State = CardPlayerData.CardPlayerState.None;
			this.ClearPerRoundData();
		}

		// Token: 0x06004455 RID: 17493 RVA: 0x001919C9 File Offset: 0x0018FBC9
		public void JoinRound()
		{
			if (!this.HasUser)
			{
				return;
			}
			this.State = CardPlayerData.CardPlayerState.InCurrentRound;
			this.ClearPerRoundData();
		}

		// Token: 0x06004456 RID: 17494 RVA: 0x001919E1 File Offset: 0x0018FBE1
		protected virtual void ClearPerRoundData()
		{
			this.Cards.Clear();
			this.betThisRound = 0;
			this.betThisTurn = 0;
			this.finalScore = 0;
			this.LeftRoundEarly = false;
			this.hasCompletedTurn = false;
			this.SendCardDetails = false;
		}

		// Token: 0x06004457 RID: 17495 RVA: 0x00191A18 File Offset: 0x0018FC18
		public virtual void LeaveCurrentRound(bool clearBets, bool leftRoundEarly)
		{
			if (!this.HasUserInCurrentRound)
			{
				return;
			}
			this.availableInputs = 0;
			this.finalScore = 0;
			this.hasCompletedTurn = false;
			if (clearBets)
			{
				this.betThisRound = 0;
				this.betThisTurn = 0;
			}
			this.State = CardPlayerData.CardPlayerState.InGame;
			this.LeftRoundEarly = leftRoundEarly;
			this.CancelTurnTimer();
		}

		// Token: 0x06004458 RID: 17496 RVA: 0x00191A68 File Offset: 0x0018FC68
		public virtual void LeaveGame()
		{
			if (!this.HasUserInGame)
			{
				return;
			}
			this.Cards.Clear();
			this.availableInputs = 0;
			this.finalScore = 0;
			this.SendCardDetails = false;
			this.LeftRoundEarly = false;
			this.State = CardPlayerData.CardPlayerState.WantsToPlay;
		}

		// Token: 0x06004459 RID: 17497 RVA: 0x00191AA1 File Offset: 0x0018FCA1
		public void EnableSendingCards()
		{
			this.SendCardDetails = true;
		}

		// Token: 0x0600445A RID: 17498 RVA: 0x00191AAA File Offset: 0x0018FCAA
		public string HandToString()
		{
			return CardPlayerData.HandToString(this.Cards);
		}

		// Token: 0x0600445B RID: 17499 RVA: 0x00191AB8 File Offset: 0x0018FCB8
		public static string HandToString(List<PlayingCard> cards)
		{
			string text = string.Empty;
			foreach (PlayingCard playingCard in cards)
			{
				text = text + "23456789TJQKA"[(int)playingCard.Rank].ToString() + "♠♥♦♣"[(int)playingCard.Suit].ToString() + " ";
			}
			return text;
		}

		// Token: 0x0600445C RID: 17500 RVA: 0x00191B44 File Offset: 0x0018FD44
		public virtual void Save(CardGame syncData)
		{
			CardGame.CardPlayer cardPlayer = Pool.Get<CardGame.CardPlayer>();
			cardPlayer.userid = this.UserID;
			cardPlayer.cards = Pool.GetList<int>();
			foreach (PlayingCard playingCard in this.Cards)
			{
				cardPlayer.cards.Add(this.SendCardDetails ? playingCard.GetIndex() : -1);
			}
			cardPlayer.scrap = this.GetScrapAmount();
			cardPlayer.state = (int)this.State;
			cardPlayer.availableInputs = this.availableInputs;
			cardPlayer.betThisRound = this.betThisRound;
			cardPlayer.betThisTurn = this.betThisTurn;
			cardPlayer.leftRoundEarly = this.LeftRoundEarly;
			cardPlayer.sendCardDetails = this.SendCardDetails;
			syncData.players.Add(cardPlayer);
		}

		// Token: 0x0600445D RID: 17501 RVA: 0x00191C2C File Offset: 0x0018FE2C
		public void StartTurnTimer(Action<CardPlayerData> callback, float maxTurnTime)
		{
			this.turnTimerCallback = callback;
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.TimeoutTurn), maxTurnTime);
		}

		// Token: 0x0600445E RID: 17502 RVA: 0x00191C4C File Offset: 0x0018FE4C
		public void CancelTurnTimer()
		{
			SingletonComponent<InvokeHandler>.Instance.CancelInvoke(new Action(this.TimeoutTurn));
		}

		// Token: 0x0600445F RID: 17503 RVA: 0x00191C64 File Offset: 0x0018FE64
		public void TimeoutTurn()
		{
			if (this.turnTimerCallback != null)
			{
				this.turnTimerCallback(this);
			}
		}

		// Token: 0x02000F7F RID: 3967
		public enum CardPlayerState
		{
			// Token: 0x04004FE7 RID: 20455
			None,
			// Token: 0x04004FE8 RID: 20456
			WantsToPlay,
			// Token: 0x04004FE9 RID: 20457
			InGame,
			// Token: 0x04004FEA RID: 20458
			InCurrentRound
		}
	}
}
