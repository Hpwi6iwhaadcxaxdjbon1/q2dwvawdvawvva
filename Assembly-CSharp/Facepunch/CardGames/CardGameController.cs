using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using ProtoBuf;
using UnityEngine;

namespace Facepunch.CardGames
{
	// Token: 0x02000AF3 RID: 2803
	public abstract class CardGameController : IDisposable
	{
		// Token: 0x04003C92 RID: 15506
		public const int IDLE_KICK_SECONDS = 240;

		// Token: 0x04003C95 RID: 15509
		private CardGame.CardList localPlayerCards;

		// Token: 0x04003C96 RID: 15510
		protected int activePlayerIndex;

		// Token: 0x04003C97 RID: 15511
		public const int STD_RAISE_INCREMENTS = 5;

		// Token: 0x04003C98 RID: 15512
		protected bool isWaitingBetweenTurns;

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x060043E0 RID: 17376 RVA: 0x00190602 File Offset: 0x0018E802
		// (set) Token: 0x060043E1 RID: 17377 RVA: 0x0019060A File Offset: 0x0018E80A
		public CardGameController.CardGameState State { get; private set; }

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x060043E2 RID: 17378 RVA: 0x00190613 File Offset: 0x0018E813
		public bool HasGameInProgress
		{
			get
			{
				return this.State >= CardGameController.CardGameState.InGameBetweenRounds;
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x060043E3 RID: 17379 RVA: 0x00190621 File Offset: 0x0018E821
		public bool HasRoundInProgressOrEnding
		{
			get
			{
				return this.State == CardGameController.CardGameState.InGameRound || this.State == CardGameController.CardGameState.InGameRoundEnding;
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x060043E4 RID: 17380 RVA: 0x00190637 File Offset: 0x0018E837
		public bool HasActiveRound
		{
			get
			{
				return this.State == CardGameController.CardGameState.InGameRound;
			}
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x060043E5 RID: 17381 RVA: 0x00190642 File Offset: 0x0018E842
		// (set) Token: 0x060043E6 RID: 17382 RVA: 0x0019064A File Offset: 0x0018E84A
		public CardPlayerData[] PlayerData { get; private set; }

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x060043E7 RID: 17383
		public abstract int MinPlayers { get; }

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x060043E8 RID: 17384
		public abstract int MinBuyIn { get; }

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x060043E9 RID: 17385
		public abstract int MaxBuyIn { get; }

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x060043EA RID: 17386
		public abstract int MinToPlay { get; }

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x060043EB RID: 17387 RVA: 0x000AB926 File Offset: 0x000A9B26
		public virtual float MaxTurnTime
		{
			get
			{
				return 30f;
			}
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x060043EC RID: 17388 RVA: 0x00007A3C File Offset: 0x00005C3C
		public virtual int EndRoundDelay
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x060043ED RID: 17389 RVA: 0x000BEEF7 File Offset: 0x000BD0F7
		public virtual int TimeBetweenRounds
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x060043EE RID: 17390 RVA: 0x00006CA5 File Offset: 0x00004EA5
		protected virtual float TimeBetweenTurns
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x060043EF RID: 17391 RVA: 0x00190653 File Offset: 0x0018E853
		// (set) Token: 0x060043F0 RID: 17392 RVA: 0x0019065B File Offset: 0x0018E85B
		private protected BaseCardGameEntity Owner { protected get; private set; }

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x060043F1 RID: 17393 RVA: 0x00190664 File Offset: 0x0018E864
		protected int ScrapItemID
		{
			get
			{
				return this.Owner.ScrapItemID;
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x060043F2 RID: 17394 RVA: 0x00190671 File Offset: 0x0018E871
		protected bool IsServer
		{
			get
			{
				return this.Owner.isServer;
			}
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x060043F3 RID: 17395 RVA: 0x0019067E File Offset: 0x0018E87E
		protected bool IsClient
		{
			get
			{
				return this.Owner.isClient;
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x060043F4 RID: 17396 RVA: 0x0019068B File Offset: 0x0018E88B
		// (set) Token: 0x060043F5 RID: 17397 RVA: 0x00190693 File Offset: 0x0018E893
		public CardGame.RoundResults resultInfo { get; private set; }

		// Token: 0x060043F6 RID: 17398 RVA: 0x0019069C File Offset: 0x0018E89C
		public CardGameController(BaseCardGameEntity owner)
		{
			this.Owner = owner;
			this.PlayerData = new CardPlayerData[this.MaxPlayersAtTable()];
			this.resultInfo = Pool.Get<CardGame.RoundResults>();
			this.resultInfo.results = Pool.GetList<CardGame.RoundResults.Result>();
			this.localPlayerCards = Pool.Get<CardGame.CardList>();
			this.localPlayerCards.cards = Pool.GetList<int>();
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i] = this.GetNewCardPlayerData(i);
			}
		}

		// Token: 0x060043F7 RID: 17399 RVA: 0x0019071F File Offset: 0x0018E91F
		public IEnumerable<CardPlayerData> PlayersInRound()
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData.HasUserInCurrentRound)
				{
					yield return cardPlayerData;
				}
			}
			CardPlayerData[] array = null;
			yield break;
		}

		// Token: 0x060043F8 RID: 17400
		protected abstract int GetFirstPlayerRelIndex(bool startOfRound);

		// Token: 0x060043F9 RID: 17401 RVA: 0x00190730 File Offset: 0x0018E930
		public void Dispose()
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i].Dispose();
			}
			this.localPlayerCards.Dispose();
			this.resultInfo.Dispose();
		}

		// Token: 0x060043FA RID: 17402 RVA: 0x00190774 File Offset: 0x0018E974
		public int NumPlayersAllowedToPlay(CardPlayerData ignore = null)
		{
			int num = 0;
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData != ignore && this.IsAllowedToPlay(cardPlayerData))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060043FB RID: 17403 RVA: 0x001907B0 File Offset: 0x0018E9B0
		public CardGameController.Playability GetPlayabilityStatus(CardPlayerData cpd)
		{
			if (!cpd.HasUser)
			{
				return CardGameController.Playability.NoPlayer;
			}
			int scrapAmount = cpd.GetScrapAmount();
			if (cpd.HasUserInGame)
			{
				if (scrapAmount < this.MinToPlay)
				{
					return CardGameController.Playability.RanOutOfScrap;
				}
			}
			else
			{
				if (scrapAmount < this.MinBuyIn)
				{
					return CardGameController.Playability.NotEnoughBuyIn;
				}
				if (scrapAmount > this.MaxBuyIn)
				{
					return CardGameController.Playability.TooMuchBuyIn;
				}
			}
			return CardGameController.Playability.OK;
		}

		// Token: 0x060043FC RID: 17404 RVA: 0x001907F8 File Offset: 0x0018E9F8
		public bool TryGetActivePlayer(out CardPlayerData activePlayer)
		{
			return this.ToCardPlayerData(this.activePlayerIndex, false, out activePlayer);
		}

		// Token: 0x060043FD RID: 17405 RVA: 0x00190808 File Offset: 0x0018EA08
		protected bool ToCardPlayerData(int relIndex, bool includeOutOfRound, out CardPlayerData result)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogWarning(base.GetType().Name + ": Tried to call ToCardPlayerData while no round was in progress. Returning null.");
				result = null;
				return false;
			}
			int num = includeOutOfRound ? this.NumPlayersInGame() : this.NumPlayersInCurrentRound();
			int index = this.RelToAbsIndex(relIndex % num, includeOutOfRound);
			return this.TryGetCardPlayerData(index, out result);
		}

		// Token: 0x060043FE RID: 17406 RVA: 0x00190864 File Offset: 0x0018EA64
		public int RelToAbsIndex(int relIndex, bool includeFolded)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogError(base.GetType().Name + ": Called RelToAbsIndex outside of a round. No-one is playing. Returning -1.");
				return -1;
			}
			int num = 0;
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (includeFolded ? this.PlayerData[i].HasUserInGame : this.PlayerData[i].HasUserInCurrentRound)
				{
					if (num == relIndex)
					{
						return i;
					}
					num++;
				}
			}
			Debug.LogError(string.Format("{0}: No absolute index found for relative index {1}. Only {2} total players are in the round. Returning -1.", base.GetType().Name, relIndex, this.NumPlayersInCurrentRound()));
			return -1;
		}

		// Token: 0x060043FF RID: 17407 RVA: 0x00190904 File Offset: 0x0018EB04
		public int GameToRoundIndex(int gameRelIndex)
		{
			if (!this.HasRoundInProgressOrEnding)
			{
				Debug.LogError(base.GetType().Name + ": Called GameToRoundIndex outside of a round. No-one is playing. Returning 0.");
				return 0;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (this.PlayerData[i].HasUserInCurrentRound)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
					num2++;
				}
				else if (this.PlayerData[i].HasUserInGame)
				{
					if (num == gameRelIndex)
					{
						return num2;
					}
					num++;
				}
			}
			Debug.LogError(string.Format("{0}: No round index found for game index {1}. Only {2} total players are in the round. Returning 0.", base.GetType().Name, gameRelIndex, this.NumPlayersInCurrentRound()));
			return 0;
		}

		// Token: 0x06004400 RID: 17408 RVA: 0x001909B0 File Offset: 0x0018EBB0
		public int NumPlayersInGame()
		{
			int num = 0;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].HasUserInGame)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06004401 RID: 17409 RVA: 0x001909E4 File Offset: 0x0018EBE4
		public int NumPlayersInCurrentRound()
		{
			int num = 0;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				if (playerData[i].HasUserInCurrentRound)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06004402 RID: 17410 RVA: 0x00190A17 File Offset: 0x0018EC17
		public int MaxPlayersAtTable()
		{
			return this.Owner.mountPoints.Count;
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x00190A2C File Offset: 0x0018EC2C
		public bool PlayerIsInGame(global::BasePlayer player)
		{
			return this.PlayerData.Any((CardPlayerData data) => data.HasUserInGame && data.UserID == player.userID);
		}

		// Token: 0x06004404 RID: 17412 RVA: 0x00190A5D File Offset: 0x0018EC5D
		public bool IsAtTable(global::BasePlayer player)
		{
			return this.IsAtTable(player.userID);
		}

		// Token: 0x06004405 RID: 17413 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
		public virtual List<PlayingCard> GetTableCards()
		{
			return null;
		}

		// Token: 0x06004406 RID: 17414 RVA: 0x00190A6B File Offset: 0x0018EC6B
		public void StartTurnTimer(CardPlayerData pData, float turnTime)
		{
			if (this.IsServer)
			{
				pData.StartTurnTimer(new Action<CardPlayerData>(this.OnTurnTimeout), turnTime);
				this.Owner.ClientRPC<int, float>(null, "ClientStartTurnTimer", pData.mountIndex, turnTime);
			}
		}

		// Token: 0x06004407 RID: 17415 RVA: 0x00190AA4 File Offset: 0x0018ECA4
		private bool IsAtTable(ulong userID)
		{
			return this.PlayerData.Any((CardPlayerData data) => data.UserID == userID);
		}

		// Token: 0x06004408 RID: 17416 RVA: 0x00190AD8 File Offset: 0x0018ECD8
		public int GetScrapInPot()
		{
			if (!this.IsServer)
			{
				return 0;
			}
			StorageContainer pot = this.Owner.GetPot();
			if (pot != null)
			{
				return pot.inventory.GetAmount(this.ScrapItemID, true);
			}
			return 0;
		}

		// Token: 0x06004409 RID: 17417 RVA: 0x00190B18 File Offset: 0x0018ED18
		public bool TryGetCardPlayerData(int index, out CardPlayerData cardPlayer)
		{
			if (index >= 0 && index < this.PlayerData.Length)
			{
				cardPlayer = this.PlayerData[index];
				return true;
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x0600440A RID: 17418 RVA: 0x00190B3C File Offset: 0x0018ED3C
		public bool TryGetCardPlayerData(ulong forPlayer, out CardPlayerData cardPlayer)
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				if (cardPlayerData.UserID == forPlayer)
				{
					cardPlayer = cardPlayerData;
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x0600440B RID: 17419 RVA: 0x00190B74 File Offset: 0x0018ED74
		public bool TryGetCardPlayerData(global::BasePlayer forPlayer, out CardPlayerData cardPlayer)
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				if (this.PlayerData[i].UserID == forPlayer.userID)
				{
					cardPlayer = this.PlayerData[i];
					return true;
				}
			}
			cardPlayer = null;
			return false;
		}

		// Token: 0x0600440C RID: 17420 RVA: 0x00190BB9 File Offset: 0x0018EDB9
		public bool IsAllowedToPlay(CardPlayerData cpd)
		{
			return this.GetPlayabilityStatus(cpd) == CardGameController.Playability.OK;
		}

		// Token: 0x0600440D RID: 17421 RVA: 0x00190BC8 File Offset: 0x0018EDC8
		protected void ClearResultsInfo()
		{
			if (this.resultInfo.results != null)
			{
				foreach (CardGame.RoundResults.Result result in this.resultInfo.results)
				{
					if (result != null)
					{
						result.Dispose();
					}
				}
				this.resultInfo.results.Clear();
			}
		}

		// Token: 0x0600440E RID: 17422
		protected abstract CardPlayerData GetNewCardPlayerData(int mountIndex);

		// Token: 0x0600440F RID: 17423
		protected abstract void OnTurnTimeout(CardPlayerData playerData);

		// Token: 0x06004410 RID: 17424
		protected abstract void SubStartRound();

		// Token: 0x06004411 RID: 17425
		protected abstract void SubReceivedInputFromPlayer(CardPlayerData playerData, int input, int value, bool countAsAction);

		// Token: 0x06004412 RID: 17426
		protected abstract int GetAvailableInputsForPlayer(CardPlayerData playerData);

		// Token: 0x06004413 RID: 17427
		protected abstract void HandlePlayerLeavingDuringTheirTurn(CardPlayerData pData);

		// Token: 0x06004414 RID: 17428
		protected abstract void SubEndRound();

		// Token: 0x06004415 RID: 17429
		protected abstract void SubEndGameplay();

		// Token: 0x06004416 RID: 17430
		protected abstract void EndCycle();

		// Token: 0x06004417 RID: 17431
		protected abstract bool ShouldEndCycle();

		// Token: 0x06004418 RID: 17432 RVA: 0x000063A5 File Offset: 0x000045A5
		public void EditorMakeRandomMove()
		{
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x00190C40 File Offset: 0x0018EE40
		public void JoinTable(global::BasePlayer player)
		{
			this.JoinTable(player.userID);
		}

		// Token: 0x0600441A RID: 17434 RVA: 0x00190C50 File Offset: 0x0018EE50
		protected void SyncAllLocalPlayerCards()
		{
			foreach (CardPlayerData pData in this.PlayerData)
			{
				this.SyncLocalPlayerCards(pData);
			}
		}

		// Token: 0x0600441B RID: 17435 RVA: 0x00190C80 File Offset: 0x0018EE80
		protected void SyncLocalPlayerCards(CardPlayerData pData)
		{
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(pData.UserID);
			if (basePlayer == null)
			{
				return;
			}
			this.localPlayerCards.cards.Clear();
			foreach (PlayingCard playingCard in pData.Cards)
			{
				this.localPlayerCards.cards.Add(playingCard.GetIndex());
			}
			this.Owner.ClientRPCPlayer<CardGame.CardList>(null, basePlayer, "ReceiveCardsForPlayer", this.localPlayerCards);
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x00190D20 File Offset: 0x0018EF20
		private void JoinTable(ulong userID)
		{
			if (this.IsAtTable(userID))
			{
				return;
			}
			if (this.NumPlayersAllowedToPlay(null) >= this.MaxPlayersAtTable())
			{
				return;
			}
			int mountPointIndex = this.Owner.GetMountPointIndex(userID);
			if (mountPointIndex < 0)
			{
				return;
			}
			this.PlayerData[mountPointIndex].AddUser(userID);
			if (!this.HasGameInProgress)
			{
				if (!this.TryStartNewRound())
				{
					this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
					return;
				}
			}
			else
			{
				this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x0600441D RID: 17437 RVA: 0x00190D90 File Offset: 0x0018EF90
		public void LeaveTable(ulong userID)
		{
			CardPlayerData pData;
			if (this.TryGetCardPlayerData(userID, out pData))
			{
				this.LeaveTable(pData);
			}
		}

		// Token: 0x0600441E RID: 17438 RVA: 0x00190DB0 File Offset: 0x0018EFB0
		public void LeaveTable(CardPlayerData pData)
		{
			CardPlayerData cardPlayerData;
			if (this.HasActiveRound && this.TryGetActivePlayer(out cardPlayerData))
			{
				if (pData == cardPlayerData)
				{
					this.HandlePlayerLeavingDuringTheirTurn(cardPlayerData);
				}
				else if (pData.HasUserInCurrentRound && pData.mountIndex < cardPlayerData.mountIndex && this.activePlayerIndex > 0)
				{
					this.activePlayerIndex--;
				}
			}
			pData.ClearAllData();
			if (this.HasActiveRound && this.NumPlayersInCurrentRound() < this.MinPlayers)
			{
				this.EndRoundWithDelay();
			}
			if (pData.HasUserInGame)
			{
				this.Owner.ClientRPC<ulong>(null, "ClientOnPlayerLeft", pData.UserID);
			}
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x0600441F RID: 17439 RVA: 0x00190E58 File Offset: 0x0018F058
		protected int TryAddBet(CardPlayerData playerData, int maxAmount)
		{
			int num = this.TryMoveToPotStorage(playerData, maxAmount);
			playerData.betThisRound += num;
			playerData.betThisTurn += num;
			return num;
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x00190E8C File Offset: 0x0018F08C
		protected int GoAllIn(CardPlayerData playerData)
		{
			int num = this.TryMoveToPotStorage(playerData, 999999);
			playerData.betThisRound += num;
			playerData.betThisTurn += num;
			return num;
		}

		// Token: 0x06004421 RID: 17441 RVA: 0x00190EC4 File Offset: 0x0018F0C4
		protected int TryMoveToPotStorage(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = this.Owner.GetPot();
			if (storage != null && pot != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = storage.inventory.Take(list, this.ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(pot.inventory, -1, true, true, null, true))
						{
							item.MoveToContainer(storage.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": TryAddToPot: Null storage.");
			}
			return num;
		}

		// Token: 0x06004422 RID: 17442 RVA: 0x00190FA0 File Offset: 0x0018F1A0
		protected int PayOutFromPot(CardPlayerData playerData, int maxAmount)
		{
			int num = 0;
			StorageContainer storage = playerData.GetStorage();
			StorageContainer pot = this.Owner.GetPot();
			if (storage != null && pot != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = pot.inventory.Take(list, this.ScrapItemID, maxAmount);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(storage.inventory, -1, true, true, null, true))
						{
							item.MoveToContainer(pot.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": PayOut: Null storage.");
			}
			return num;
		}

		// Token: 0x06004423 RID: 17443 RVA: 0x0019107C File Offset: 0x0018F27C
		protected int PayOutAllFromPot(CardPlayerData playerData)
		{
			return this.PayOutFromPot(playerData, int.MaxValue);
		}

		// Token: 0x06004424 RID: 17444 RVA: 0x0019108C File Offset: 0x0018F28C
		protected void ClearPot()
		{
			StorageContainer pot = this.Owner.GetPot();
			if (pot != null)
			{
				pot.inventory.Clear();
			}
		}

		// Token: 0x06004425 RID: 17445 RVA: 0x001910BC File Offset: 0x0018F2BC
		protected int RemoveScrapFromStorage(CardPlayerData data)
		{
			StorageContainer storage = data.GetStorage();
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(data.UserID);
			int num = 0;
			if (basePlayer != null)
			{
				List<global::Item> list = Pool.GetList<global::Item>();
				num = storage.inventory.Take(list, this.ScrapItemID, int.MaxValue);
				if (num > 0)
				{
					foreach (global::Item item in list)
					{
						if (!item.MoveToContainer(basePlayer.inventory.containerMain, -1, true, true, null, true))
						{
							item.MoveToContainer(storage.inventory, -1, true, false, null, true);
						}
					}
				}
				Pool.FreeList<global::Item>(ref list);
			}
			return num;
		}

		// Token: 0x06004426 RID: 17446 RVA: 0x0019117C File Offset: 0x0018F37C
		public virtual void Save(CardGame syncData)
		{
			syncData.players = Pool.GetList<CardGame.CardPlayer>();
			syncData.state = (int)this.State;
			syncData.activePlayerIndex = this.activePlayerIndex;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].Save(syncData);
			}
			syncData.pot = this.GetScrapInPot();
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x001911D6 File Offset: 0x0018F3D6
		private void InvokeStartNewRound()
		{
			this.TryStartNewRound();
		}

		// Token: 0x06004428 RID: 17448 RVA: 0x001911E0 File Offset: 0x0018F3E0
		private bool TryStartNewRound()
		{
			if (this.HasRoundInProgressOrEnding)
			{
				return false;
			}
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				global::BasePlayer basePlayer;
				if (this.State == CardGameController.CardGameState.NotPlaying)
				{
					cardPlayerData.lastActionTime = Time.unscaledTime;
				}
				else if (cardPlayerData.HasBeenIdleFor(240) && global::BasePlayer.TryFindByID(cardPlayerData.UserID, out basePlayer))
				{
					basePlayer.GetMounted().DismountPlayer(basePlayer, false);
				}
			}
			if (this.NumPlayersAllowedToPlay(null) < this.MinPlayers)
			{
				this.EndGameplay();
				return false;
			}
			foreach (CardPlayerData cardPlayerData2 in this.PlayerData)
			{
				if (this.IsAllowedToPlay(cardPlayerData2))
				{
					cardPlayerData2.JoinRound();
				}
				else
				{
					cardPlayerData2.LeaveGame();
				}
			}
			this.State = CardGameController.CardGameState.InGameRound;
			this.SubStartRound();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}

		// Token: 0x06004429 RID: 17449 RVA: 0x001912B3 File Offset: 0x0018F4B3
		protected void BeginRoundEnd()
		{
			this.State = CardGameController.CardGameState.InGameRoundEnding;
			this.CancelNextCycleInvoke();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x0600442A RID: 17450 RVA: 0x001912CE File Offset: 0x0018F4CE
		protected void EndRoundWithDelay()
		{
			this.State = CardGameController.CardGameState.InGameRoundEnding;
			this.CancelNextCycleInvoke();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.Owner.Invoke(new Action(this.EndRound), (float)this.EndRoundDelay);
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x00191308 File Offset: 0x0018F508
		private void EndRound()
		{
			this.State = CardGameController.CardGameState.InGameBetweenRounds;
			this.CancelNextCycleInvoke();
			this.ClearResultsInfo();
			this.SubEndRound();
			foreach (CardPlayerData cardPlayerData in this.PlayersInRound())
			{
				global::BasePlayer basePlayer = global::BasePlayer.FindByID(cardPlayerData.UserID);
				if (basePlayer != null && basePlayer.metabolism.CanConsume())
				{
					basePlayer.metabolism.MarkConsumption();
					basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, 2f, 0f);
					basePlayer.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, 2f, 0f);
				}
				cardPlayerData.LeaveCurrentRound(true, false);
			}
			this.UpdateAllAvailableInputs();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.Owner.Invoke(new Action(this.InvokeStartNewRound), (float)this.TimeBetweenRounds);
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x001913F8 File Offset: 0x0018F5F8
		protected virtual void AddRoundResult(CardPlayerData pData, int winnings, int resultCode)
		{
			foreach (CardGame.RoundResults.Result result in this.resultInfo.results)
			{
				if (result.ID == pData.UserID)
				{
					result.winnings += winnings;
					return;
				}
			}
			CardGame.RoundResults.Result result2 = Pool.Get<CardGame.RoundResults.Result>();
			result2.ID = pData.UserID;
			result2.winnings = winnings;
			result2.resultCode = resultCode;
			this.resultInfo.results.Add(result2);
		}

		// Token: 0x0600442D RID: 17453 RVA: 0x00191498 File Offset: 0x0018F698
		protected void EndGameplay()
		{
			if (!this.HasGameInProgress)
			{
				return;
			}
			this.CancelNextCycleInvoke();
			this.SubEndGameplay();
			this.State = CardGameController.CardGameState.NotPlaying;
			CardPlayerData[] playerData = this.PlayerData;
			for (int i = 0; i < playerData.Length; i++)
			{
				playerData[i].LeaveGame();
			}
			this.SyncAllLocalPlayerCards();
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x0600442E RID: 17454 RVA: 0x001914F0 File Offset: 0x0018F6F0
		public void ReceivedInputFromPlayer(global::BasePlayer player, int input, bool countAsAction, int value = 0)
		{
			if (player == null)
			{
				return;
			}
			player.ResetInputIdleTime();
			CardPlayerData pData;
			if (this.TryGetCardPlayerData(player, out pData))
			{
				this.ReceivedInputFromPlayer(pData, input, countAsAction, value, true);
			}
		}

		// Token: 0x0600442F RID: 17455 RVA: 0x00191524 File Offset: 0x0018F724
		protected void ReceivedInputFromPlayer(CardPlayerData pData, int input, bool countAsAction, int value = 0, bool playerInitiated = true)
		{
			if (!this.HasGameInProgress)
			{
				return;
			}
			if (pData == null)
			{
				return;
			}
			if (playerInitiated)
			{
				pData.lastActionTime = Time.unscaledTime;
			}
			this.SubReceivedInputFromPlayer(pData, input, value, countAsAction);
			if (this.HasActiveRound)
			{
				this.UpdateAllAvailableInputs();
				this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x06004430 RID: 17456 RVA: 0x00191574 File Offset: 0x0018F774
		protected void UpdateAllAvailableInputs()
		{
			for (int i = 0; i < this.PlayerData.Length; i++)
			{
				this.PlayerData[i].availableInputs = this.GetAvailableInputsForPlayer(this.PlayerData[i]);
			}
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x001915AF File Offset: 0x0018F7AF
		public void PlayerStorageChanged()
		{
			if (!this.HasGameInProgress)
			{
				this.TryStartNewRound();
			}
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x001915C0 File Offset: 0x0018F7C0
		protected void ServerPlaySound(CardGameSounds.SoundType type)
		{
			this.Owner.ClientRPC<int>(null, "ClientPlaySound", (int)type);
		}

		// Token: 0x06004433 RID: 17459 RVA: 0x001915D4 File Offset: 0x0018F7D4
		public void GetConnectionsInGame(List<Connection> connections)
		{
			foreach (CardPlayerData cardPlayerData in this.PlayerData)
			{
				global::BasePlayer basePlayer;
				if (cardPlayerData.HasUserInGame && global::BasePlayer.TryFindByID(cardPlayerData.UserID, out basePlayer))
				{
					connections.Add(basePlayer.net.connection);
				}
			}
		}

		// Token: 0x06004434 RID: 17460 RVA: 0x00191624 File Offset: 0x0018F824
		public virtual void OnTableDestroyed()
		{
			if (this.HasGameInProgress)
			{
				foreach (CardPlayerData cardPlayerData in this.PlayerData)
				{
					if (cardPlayerData.HasUserInGame)
					{
						this.PayOutFromPot(cardPlayerData, cardPlayerData.GetTotalBetThisRound());
					}
				}
				if (this.GetScrapInPot() > 0)
				{
					int maxAmount = this.GetScrapInPot() / this.NumPlayersInGame();
					foreach (CardPlayerData cardPlayerData2 in this.PlayerData)
					{
						if (cardPlayerData2.HasUserInGame)
						{
							this.PayOutFromPot(cardPlayerData2, maxAmount);
						}
					}
				}
			}
			foreach (CardPlayerData cardPlayerData3 in this.PlayerData)
			{
				if (cardPlayerData3.HasUser)
				{
					this.RemoveScrapFromStorage(cardPlayerData3);
				}
			}
		}

		// Token: 0x06004435 RID: 17461 RVA: 0x001916D8 File Offset: 0x0018F8D8
		protected bool TryMoveToNextPlayerWithInputs(int startIndex, out CardPlayerData newActivePlayer)
		{
			this.activePlayerIndex = startIndex;
			this.TryGetActivePlayer(out newActivePlayer);
			int num = 0;
			bool flag = false;
			while (this.GetAvailableInputsForPlayer(newActivePlayer) == 0)
			{
				if (num == this.NumPlayersInCurrentRound())
				{
					flag = true;
					break;
				}
				this.activePlayerIndex = (this.activePlayerIndex + 1) % this.NumPlayersInCurrentRound();
				this.TryGetActivePlayer(out newActivePlayer);
				num++;
			}
			return !flag;
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x00191736 File Offset: 0x0018F936
		protected virtual void StartNextCycle()
		{
			this.isWaitingBetweenTurns = false;
		}

		// Token: 0x06004437 RID: 17463 RVA: 0x0019173F File Offset: 0x0018F93F
		protected void QueueNextCycleInvoke()
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.StartNextCycle), this.TimeBetweenTurns);
			this.isWaitingBetweenTurns = true;
			this.Owner.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}

		// Token: 0x06004438 RID: 17464 RVA: 0x00191771 File Offset: 0x0018F971
		private void CancelNextCycleInvoke()
		{
			SingletonComponent<InvokeHandler>.Instance.CancelInvoke(new Action(this.StartNextCycle));
			this.isWaitingBetweenTurns = false;
		}

		// Token: 0x02000F7A RID: 3962
		public enum CardGameState
		{
			// Token: 0x04004FD3 RID: 20435
			NotPlaying,
			// Token: 0x04004FD4 RID: 20436
			InGameBetweenRounds,
			// Token: 0x04004FD5 RID: 20437
			InGameRound,
			// Token: 0x04004FD6 RID: 20438
			InGameRoundEnding
		}

		// Token: 0x02000F7B RID: 3963
		public enum Playability
		{
			// Token: 0x04004FD8 RID: 20440
			OK,
			// Token: 0x04004FD9 RID: 20441
			NoPlayer,
			// Token: 0x04004FDA RID: 20442
			NotEnoughBuyIn,
			// Token: 0x04004FDB RID: 20443
			TooMuchBuyIn,
			// Token: 0x04004FDC RID: 20444
			RanOutOfScrap,
			// Token: 0x04004FDD RID: 20445
			Idle
		}
	}
}
