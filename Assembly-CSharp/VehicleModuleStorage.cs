using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E9 RID: 233
public class VehicleModuleStorage : VehicleModuleSeating
{
	// Token: 0x04000CF8 RID: 3320
	[SerializeField]
	private VehicleModuleStorage.Storage storage;

	// Token: 0x04000CF9 RID: 3321
	private EntityRef storageUnitInstance;

	// Token: 0x0600148F RID: 5263 RVA: 0x000A2488 File Offset: 0x000A0688
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleStorage.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
			if (rpc == 425471188U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TryOpenWithKeycode ");
				}
				using (TimeWarning.New("RPC_TryOpenWithKeycode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(425471188U, "RPC_TryOpenWithKeycode", this, player, 3f))
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
							this.RPC_TryOpenWithKeycode(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_TryOpenWithKeycode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000A2788 File Offset: 0x000A0988
	public IItemContainerEntity GetContainer()
	{
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as IItemContainerEntity;
		}
		return null;
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x000A27C0 File Offset: 0x000A09C0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.storageUnitInstance.uid = info.msg.simpleUID.uid;
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x000A27E4 File Offset: 0x000A09E4
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave && this.storage.storageUnitPoint.gameObject.activeSelf)
		{
			this.CreateStorageEntity();
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x000A2810 File Offset: 0x000A0A10
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			global::ItemContainer inventory = container.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x000A17C2 File Offset: 0x0009F9C2
	private void OnItemAddedRemoved(global::Item item, bool add)
	{
		global::Item associatedItemInstance = this.AssociatedItemInstance;
		if (associatedItemInstance == null)
		{
			return;
		}
		associatedItemInstance.LockUnlock(!this.CanBeMovedNowOnVehicle());
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x000A285C File Offset: 0x000A0A5C
	public override void NonUserSpawn()
	{
		Rust.Modular.EngineStorage engineStorage = this.GetContainer() as Rust.Modular.EngineStorage;
		if (engineStorage != null)
		{
			engineStorage.NonUserSpawn();
		}
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x000A2884 File Offset: 0x000A0A84
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			IItemContainerEntity container = this.GetContainer();
			if (!container.IsUnityNull<IItemContainerEntity>())
			{
				container.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x000A28B4 File Offset: 0x000A0AB4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Facepunch.Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.storageUnitInstance.uid;
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000A28E8 File Offset: 0x000A0AE8
	public void CreateStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		if (!this.storageUnitInstance.IsValid(base.isServer))
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.storage.storageUnitPrefab.resourcePath, this.storage.storageUnitPoint.localPosition, this.storage.storageUnitPoint.localRotation, true);
			this.storageUnitInstance.Set(baseEntity);
			baseEntity.SetParent(this, false, false);
			baseEntity.Spawn();
			global::ItemContainer inventory = this.GetContainer().inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x000A29A0 File Offset: 0x000A0BA0
	public void DestroyStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			BaseCombatEntity baseCombatEntity;
			if ((baseCombatEntity = (baseEntity as BaseCombatEntity)) != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x000A29F2 File Offset: 0x000A0BF2
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		this.TryOpen(msg.player);
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x000A2A04 File Offset: 0x000A0C04
	private bool TryOpen(global::BasePlayer player)
	{
		if (!player.IsValid() || !this.CanBeLooted(player))
		{
			return false;
		}
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			container.PlayerOpenLoot(player, "", true);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": No container component found.");
		}
		return true;
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000A2A60 File Offset: 0x000A0C60
	protected override bool CanBeMovedNowOnVehicle()
	{
		IItemContainerEntity container = this.GetContainer();
		return container.IsUnityNull<IItemContainerEntity>() || container.inventory.IsEmpty();
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000A2A8C File Offset: 0x000A0C8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TryOpenWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		if (!base.IsOnACar)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string codeEntered = msg.read.String(256);
		if (base.Car.CarLock.TryOpenWithCode(player, codeEntered))
		{
			this.TryOpen(player);
			return;
		}
		base.Car.ClientRPC(null, "CodeEntryFailed");
	}

	// Token: 0x02000C14 RID: 3092
	[Serializable]
	public class Storage
	{
		// Token: 0x040041F2 RID: 16882
		public GameObjectRef storageUnitPrefab;

		// Token: 0x040041F3 RID: 16883
		public Transform storageUnitPoint;
	}
}
