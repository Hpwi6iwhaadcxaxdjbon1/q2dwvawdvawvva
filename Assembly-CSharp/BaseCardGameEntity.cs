using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.CardGames;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000038 RID: 56
public abstract class BaseCardGameEntity : global::BaseVehicle
{
	// Token: 0x04000227 RID: 551
	[Header("Card Game")]
	[SerializeField]
	private GameObjectRef uiPrefab;

	// Token: 0x04000228 RID: 552
	public ItemDefinition scrapItemDef;

	// Token: 0x04000229 RID: 553
	[SerializeField]
	private GameObjectRef potPrefab;

	// Token: 0x0400022A RID: 554
	public BaseCardGameEntity.PlayerStorageInfo[] playerStoragePoints;

	// Token: 0x0400022B RID: 555
	[SerializeField]
	private GameObjectRef playerStoragePrefab;

	// Token: 0x0400022C RID: 556
	private CardGameController _gameCont;

	// Token: 0x0400022D RID: 557
	public BaseCardGameEntity.CardGameOption gameOption;

	// Token: 0x0400022E RID: 558
	public EntityRef PotInstance;

	// Token: 0x0400022F RID: 559
	private bool storageLinked;

	// Token: 0x0600023E RID: 574 RVA: 0x00027AC0 File Offset: 0x00025CC0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCardGameEntity.OnRpcMessage", 0))
		{
			if (rpc == 2395020190U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_MakeRandomMove ");
				}
				using (TimeWarning.New("RPC_Editor_MakeRandomMove", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2395020190U, "RPC_Editor_MakeRandomMove", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Editor_MakeRandomMove(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Editor_MakeRandomMove");
					}
				}
				return true;
			}
			if (rpc == 1608700874U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Editor_SpawnTestPlayer ");
				}
				using (TimeWarning.New("RPC_Editor_SpawnTestPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1608700874U, "RPC_Editor_SpawnTestPlayer", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Editor_SpawnTestPlayer(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Editor_SpawnTestPlayer");
					}
				}
				return true;
			}
			if (rpc == 1499640189U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LeaveTable ");
				}
				using (TimeWarning.New("RPC_LeaveTable", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1499640189U, "RPC_LeaveTable", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_LeaveTable(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_LeaveTable");
					}
				}
				return true;
			}
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
			if (rpc == 2847205856U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Play ");
				}
				using (TimeWarning.New("RPC_Play", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2847205856U, "RPC_Play", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Play(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_Play");
					}
				}
				return true;
			}
			if (rpc == 2495306863U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PlayerInput ");
				}
				using (TimeWarning.New("RPC_PlayerInput", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2495306863U, "RPC_PlayerInput", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PlayerInput(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_PlayerInput");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x0600023F RID: 575 RVA: 0x0002832C File Offset: 0x0002652C
	public int ScrapItemID
	{
		get
		{
			return this.scrapItemDef.itemid;
		}
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000240 RID: 576 RVA: 0x00028339 File Offset: 0x00026539
	public CardGameController GameController
	{
		get
		{
			if (this._gameCont == null)
			{
				this._gameCont = this.GetGameController();
			}
			return this._gameCont;
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000241 RID: 577
	protected abstract float MaxStorageInteractionDist { get; }

	// Token: 0x06000242 RID: 578 RVA: 0x00028355 File Offset: 0x00026555
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			this.PotInstance.uid = info.msg.cardGame.potRef;
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00028384 File Offset: 0x00026584
	private CardGameController GetGameController()
	{
		BaseCardGameEntity.CardGameOption cardGameOption = this.gameOption;
		if (cardGameOption == BaseCardGameEntity.CardGameOption.TexasHoldEm)
		{
			return new TexasHoldEmController(this);
		}
		if (cardGameOption != BaseCardGameEntity.CardGameOption.Blackjack)
		{
			return new TexasHoldEmController(this);
		}
		return new BlackjackController(this);
	}

	// Token: 0x06000244 RID: 580 RVA: 0x000283B5 File Offset: 0x000265B5
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.GameController.Dispose();
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000245 RID: 581 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool CanSwapSeats
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x000283C8 File Offset: 0x000265C8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cardGame = Facepunch.Pool.Get<CardGame>();
		info.msg.cardGame.potRef = this.PotInstance.uid;
		if (!info.forDisk && this.storageLinked)
		{
			this.GameController.Save(info.msg.cardGame);
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00028430 File Offset: 0x00026630
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		int num = 0;
		int num2 = 0;
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardGamePlayerStorage cardGamePlayerStorage;
				if ((cardGamePlayerStorage = (enumerator.Current as CardGamePlayerStorage)) != null)
				{
					this.playerStoragePoints[num].storageInstance.Set(cardGamePlayerStorage);
					if (!cardGamePlayerStorage.inventory.IsEmpty())
					{
						num2++;
					}
					num++;
				}
			}
		}
		this.storageLinked = true;
		bool flag = true;
		StorageContainer pot = this.GetPot();
		if (pot == null)
		{
			flag = false;
		}
		else
		{
			int num3 = (num2 > 0) ? num2 : this.playerStoragePoints.Length;
			int iAmount = Mathf.CeilToInt((float)(pot.inventory.GetAmount(this.ScrapItemID, true) / num3));
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				CardGamePlayerStorage cardGamePlayerStorage2 = array[i].storageInstance.Get(base.isServer) as CardGamePlayerStorage;
				if (cardGamePlayerStorage2.IsValid() && (!cardGamePlayerStorage2.inventory.IsEmpty() || num2 == 0))
				{
					List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
					if (pot.inventory.Take(list, this.ScrapItemID, iAmount) > 0)
					{
						foreach (global::Item item in list)
						{
							if (!item.MoveToContainer(cardGamePlayerStorage2.inventory, -1, true, true, null, true))
							{
								item.Remove(0f);
							}
						}
					}
					Facepunch.Pool.FreeList<global::Item>(ref list);
				}
			}
		}
		if (flag)
		{
			BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].storageInstance.IsValid(base.isServer))
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning(base.GetType().Name + ": Card game storage didn't load in. Destroying the card game (and parent entity if there is one).");
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				parentEntity.Invoke(new Action(parentEntity.KillMessage), 0f);
				return;
			}
			base.Invoke(new Action(base.KillMessage), 0f);
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0002867C File Offset: 0x0002687C
	internal override void DoServerDestroy()
	{
		CardGameController gameController = this.GameController;
		if (gameController != null)
		{
			gameController.OnTableDestroyed();
		}
		BaseCardGameEntity.PlayerStorageInfo[] array = this.playerStoragePoints;
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayerStorage storage = array[i].GetStorage();
			if (storage != null)
			{
				storage.DropItems(null);
			}
		}
		StorageContainer pot = this.GetPot();
		if (pot != null)
		{
			pot.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000286E8 File Offset: 0x000268E8
	public override void PrePlayerDismount(global::BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		if (!Rust.Application.isLoadingSave)
		{
			CardGamePlayerStorage playerStorage = this.GetPlayerStorage(player.userID);
			if (playerStorage != null)
			{
				global::Item slot = playerStorage.inventory.GetSlot(0);
				if (slot != null)
				{
					slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
				}
			}
		}
	}

	// Token: 0x0600024A RID: 586 RVA: 0x00028741 File Offset: 0x00026941
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		this.GameController.LeaveTable(player.userID);
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0002875C File Offset: 0x0002695C
	public StorageContainer GetPot()
	{
		global::BaseEntity baseEntity = this.PotInstance.Get(true);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00028790 File Offset: 0x00026990
	public global::BasePlayer IDToPlayer(ulong id)
	{
		foreach (global::BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null && mountPointInfo.mountable.GetMounted() != null && mountPointInfo.mountable.GetMounted().userID == id)
			{
				return mountPointInfo.mountable.GetMounted();
			}
		}
		return null;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00028824 File Offset: 0x00026A24
	public virtual void PlayerStorageChanged()
	{
		this.GameController.PlayerStorageChanged();
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00028831 File Offset: 0x00026A31
	public CardGamePlayerStorage GetPlayerStorage(int storageIndex)
	{
		return this.playerStoragePoints[storageIndex].GetStorage();
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00028840 File Offset: 0x00026A40
	public CardGamePlayerStorage GetPlayerStorage(ulong playerID)
	{
		int mountPointIndex = this.GetMountPointIndex(playerID);
		if (mountPointIndex < 0)
		{
			return null;
		}
		return this.playerStoragePoints[mountPointIndex].GetStorage();
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00028868 File Offset: 0x00026A68
	public int GetMountPointIndex(ulong playerID)
	{
		int num = -1;
		for (int i = 0; i < this.mountPoints.Count; i++)
		{
			BaseMountable mountable = this.mountPoints[i].mountable;
			if (mountable != null)
			{
				global::BasePlayer mounted = mountable.GetMounted();
				if (mounted != null && mounted.userID == playerID)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't find mount point for this player.");
		}
		return num;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x000288E4 File Offset: 0x00026AE4
	public override void SpawnSubEntities()
	{
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.potPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		StorageContainer storageContainer = baseEntity as StorageContainer;
		if (storageContainer != null)
		{
			storageContainer.SetParent(this, false, false);
			storageContainer.Spawn();
			this.PotInstance.Set(baseEntity);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Spawned prefab is not a StorageContainer as expected.");
		}
		foreach (BaseCardGameEntity.PlayerStorageInfo playerStorageInfo in this.playerStoragePoints)
		{
			baseEntity = GameManager.server.CreateEntity(this.playerStoragePrefab.resourcePath, playerStorageInfo.storagePos.localPosition, playerStorageInfo.storagePos.localRotation, true);
			CardGamePlayerStorage cardGamePlayerStorage = baseEntity as CardGamePlayerStorage;
			if (cardGamePlayerStorage != null)
			{
				cardGamePlayerStorage.SetCardTable(this);
				cardGamePlayerStorage.SetParent(this, false, false);
				cardGamePlayerStorage.Spawn();
				playerStorageInfo.storageInstance.Set(baseEntity);
				this.storageLinked = true;
			}
			else
			{
				Debug.LogError(base.GetType().Name + ": Spawned prefab is not a CardTablePlayerStorage as expected.");
			}
		}
		base.SpawnSubEntities();
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00028A09 File Offset: 0x00026C09
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_PlayerInput(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.ReceivedInputFromPlayer(msg.player, msg.read.Int32(), true, msg.read.Int32());
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00028A33 File Offset: 0x00026C33
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_LeaveTable(global::BaseEntity.RPCMessage msg)
	{
		this.GameController.LeaveTable(msg.player.userID);
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00028A4C File Offset: 0x00026C4C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GetPlayerStorage(player.userID).PlayerOpenLoot(player, "", true);
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00028A8C File Offset: 0x00026C8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_SpawnTestPlayer(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		int num = this.GameController.MaxPlayersAtTable();
		if (this.GameController.NumPlayersAllowedToPlay(null) >= num || base.NumMounted() >= num)
		{
			return;
		}
		Debug.Log("Adding test NPC for card game");
		global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		global::BasePlayer basePlayer = (global::BasePlayer)baseEntity;
		this.AttemptMount(basePlayer, false);
		this.GameController.JoinTable(basePlayer);
		CardPlayerData cardPlayerData;
		if (this.GameController.TryGetCardPlayerData(basePlayer, out cardPlayerData))
		{
			int scrapAmount = cardPlayerData.GetScrapAmount();
			if (scrapAmount < 400)
			{
				StorageContainer storage = cardPlayerData.GetStorage();
				if (storage != null)
				{
					storage.inventory.AddItem(this.scrapItemDef, 400 - scrapAmount, 0UL, global::ItemContainer.LimitStack.Existing);
					return;
				}
				Debug.LogError("Couldn't get storage for NPC.");
				return;
			}
		}
		else
		{
			Debug.Log("Couldn't find player data for NPC. No scrap given.");
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00028B73 File Offset: 0x00026D73
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Editor_MakeRandomMove(global::BaseEntity.RPCMessage msg)
	{
		if (!UnityEngine.Application.isEditor)
		{
			return;
		}
		this.GameController.EditorMakeRandomMove();
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00028B88 File Offset: 0x00026D88
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_Play(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player != null && this.PlayerIsMounted(player))
		{
			this.GameController.JoinTable(player);
		}
	}

	// Token: 0x02000B6D RID: 2925
	[Serializable]
	public class PlayerStorageInfo
	{
		// Token: 0x04003EE1 RID: 16097
		public Transform storagePos;

		// Token: 0x04003EE2 RID: 16098
		public EntityRef storageInstance;

		// Token: 0x06004CEB RID: 19691 RVA: 0x0019F9B4 File Offset: 0x0019DBB4
		public CardGamePlayerStorage GetStorage()
		{
			global::BaseEntity baseEntity = this.storageInstance.Get(true);
			if (baseEntity != null && baseEntity.IsValid())
			{
				return baseEntity as CardGamePlayerStorage;
			}
			return null;
		}
	}

	// Token: 0x02000B6E RID: 2926
	public enum CardGameOption
	{
		// Token: 0x04003EE4 RID: 16100
		TexasHoldEm,
		// Token: 0x04003EE5 RID: 16101
		Blackjack
	}
}
