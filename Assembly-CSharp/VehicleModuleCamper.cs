using System;
using System.Collections.Generic;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E7 RID: 231
public class VehicleModuleCamper : VehicleModuleSeating
{
	// Token: 0x04000CD7 RID: 3287
	public GameObjectRef SleepingBagEntity;

	// Token: 0x04000CD8 RID: 3288
	public Transform[] SleepingBagPoints;

	// Token: 0x04000CD9 RID: 3289
	public GameObjectRef LockerEntity;

	// Token: 0x04000CDA RID: 3290
	public Transform LockerPoint;

	// Token: 0x04000CDB RID: 3291
	public GameObjectRef BbqEntity;

	// Token: 0x04000CDC RID: 3292
	public Transform BbqPoint;

	// Token: 0x04000CDD RID: 3293
	public GameObjectRef StorageEntity;

	// Token: 0x04000CDE RID: 3294
	public Transform StoragePoint;

	// Token: 0x04000CDF RID: 3295
	private EntityRef<global::BaseOven> activeBbq;

	// Token: 0x04000CE0 RID: 3296
	private EntityRef<Locker> activeLocker;

	// Token: 0x04000CE1 RID: 3297
	private EntityRef<StorageContainer> activeStorage;

	// Token: 0x04000CE2 RID: 3298
	private bool wasLoaded;

	// Token: 0x06001465 RID: 5221 RVA: 0x000A117C File Offset: 0x0009F37C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleCamper.OnRpcMessage", 0))
		{
			if (rpc == 2501069650U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLocker ");
				}
				using (TimeWarning.New("RPC_OpenLocker", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2501069650U, "RPC_OpenLocker", this, player, 3f))
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
							this.RPC_OpenLocker(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLocker");
					}
				}
				return true;
			}
			if (rpc == 4185921214U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenStorage ");
				}
				using (TimeWarning.New("RPC_OpenStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4185921214U, "RPC_OpenStorage", this, player, 3f))
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
							this.RPC_OpenStorage(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000A147C File Offset: 0x0009F67C
	public override void ResetState()
	{
		base.ResetState();
		this.activeBbq.Set(null);
		this.activeLocker.Set(null);
		this.activeStorage.Set(null);
		this.wasLoaded = false;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x000A14B0 File Offset: 0x0009F6B0
	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		base.ModuleAdded(vehicle, firstSocketIndex);
		if (base.isServer)
		{
			if (!Rust.Application.isLoadingSave && !this.wasLoaded)
			{
				for (int i = 0; i < this.SleepingBagPoints.Length; i++)
				{
					global::SleepingBagCamper sleepingBagCamper = base.gameManager.CreateEntity(this.SleepingBagEntity.resourcePath, this.SleepingBagPoints[i].localPosition, this.SleepingBagPoints[i].localRotation, true) as global::SleepingBagCamper;
					if (sleepingBagCamper != null)
					{
						sleepingBagCamper.SetParent(this, false, false);
						sleepingBagCamper.SetSeat(base.GetSeatAtIndex(i), false);
						sleepingBagCamper.Spawn();
					}
				}
				this.PostConditionalRefresh();
				return;
			}
			int num = 0;
			foreach (global::BaseEntity baseEntity in this.children)
			{
				global::SleepingBagCamper sleepingBagCamper2;
				IItemContainerEntity itemContainerEntity;
				if ((sleepingBagCamper2 = (baseEntity as global::SleepingBagCamper)) != null)
				{
					sleepingBagCamper2.SetSeat(base.GetSeatAtIndex(num++), true);
				}
				else if ((itemContainerEntity = (baseEntity as IItemContainerEntity)) != null)
				{
					global::ItemContainer inventory = itemContainerEntity.inventory;
					inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
				}
			}
		}
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x000A15F0 File Offset: 0x0009F7F0
	protected override Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		CamperSeatConfig seatConfig = this.GetSeatConfig();
		if (seatConfig != null && seatConfig.SeatPositions.Length > index)
		{
			return seatConfig.SeatPositions[index].localPosition;
		}
		return base.ModifySeatPositionLocalSpace(index, desiredPos);
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x000A162E File Offset: 0x0009F82E
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.wasLoaded = true;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000A1640 File Offset: 0x0009F840
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			Locker locker = base.gameManager.CreateEntity(this.LockerEntity.resourcePath, this.LockerPoint.localPosition, this.LockerPoint.localRotation, true) as Locker;
			locker.SetParent(this, false, false);
			locker.Spawn();
			global::ItemContainer inventory = locker.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeLocker.Set(locker);
			global::BaseOven baseOven = base.gameManager.CreateEntity(this.BbqEntity.resourcePath, this.BbqPoint.localPosition, this.BbqPoint.localRotation, true) as global::BaseOven;
			baseOven.SetParent(this, false, false);
			baseOven.Spawn();
			global::ItemContainer inventory2 = baseOven.inventory;
			inventory2.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory2.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeBbq.Set(baseOven);
			StorageContainer storageContainer = base.gameManager.CreateEntity(this.StorageEntity.resourcePath, this.StoragePoint.localPosition, this.StoragePoint.localRotation, true) as StorageContainer;
			storageContainer.SetParent(this, false, false);
			storageContainer.Spawn();
			global::ItemContainer inventory3 = storageContainer.inventory;
			inventory3.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory3.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeStorage.Set(storageContainer);
			this.PostConditionalRefresh();
		}
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x000A17C2 File Offset: 0x0009F9C2
	private void OnItemAddedRemoved(global::Item item, bool add)
	{
		global::Item associatedItemInstance = this.AssociatedItemInstance;
		if (associatedItemInstance == null)
		{
			return;
		}
		associatedItemInstance.LockUnlock(!this.CanBeMovedNowOnVehicle());
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x000A17E0 File Offset: 0x0009F9E0
	protected override bool CanBeMovedNowOnVehicle()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				IItemContainerEntity itemContainerEntity;
				if ((itemContainerEntity = (enumerator.Current as IItemContainerEntity)) != null && !itemContainerEntity.IsUnityNull<IItemContainerEntity>() && !itemContainerEntity.inventory.IsEmpty())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000A1850 File Offset: 0x0009FA50
	protected override void PostConditionalRefresh()
	{
		base.PostConditionalRefresh();
		if (base.isClient)
		{
			return;
		}
		CamperSeatConfig seatConfig = this.GetSeatConfig();
		if (seatConfig != null && this.mountPoints != null)
		{
			for (int i = 0; i < this.mountPoints.Count; i++)
			{
				if (this.mountPoints[i].mountable != null)
				{
					this.mountPoints[i].mountable.transform.position = seatConfig.SeatPositions[i].position;
					this.mountPoints[i].mountable.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				}
			}
		}
		if (this.activeBbq.IsValid(base.isServer) && seatConfig != null)
		{
			global::BaseOven baseOven = this.activeBbq.Get(true);
			baseOven.transform.position = seatConfig.StovePosition.position;
			baseOven.transform.rotation = seatConfig.StovePosition.rotation;
			baseOven.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (this.activeStorage.IsValid(base.isServer) && seatConfig != null)
		{
			StorageContainer storageContainer = this.activeStorage.Get(base.isServer);
			storageContainer.transform.position = seatConfig.StoragePosition.position;
			storageContainer.transform.rotation = seatConfig.StoragePosition.rotation;
			storageContainer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x000A19AC File Offset: 0x0009FBAC
	private CamperSeatConfig GetSeatConfig()
	{
		List<ConditionalObject> conditionals = base.GetConditionals();
		CamperSeatConfig result = null;
		foreach (ConditionalObject conditionalObject in conditionals)
		{
			CamperSeatConfig camperSeatConfig;
			if (conditionalObject.gameObject.activeSelf && conditionalObject.gameObject.TryGetComponent<CamperSeatConfig>(out camperSeatConfig))
			{
				result = camperSeatConfig;
			}
		}
		return result;
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000A1A1C File Offset: 0x0009FC1C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.camperModule == null)
		{
			info.msg.camperModule = Facepunch.Pool.Get<CamperModule>();
		}
		info.msg.camperModule.bbqId = this.activeBbq.uid;
		info.msg.camperModule.lockerId = this.activeLocker.uid;
		info.msg.camperModule.storageID = this.activeStorage.uid;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x000A1AA0 File Offset: 0x0009FCA0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenLocker(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity itemContainerEntity = this.activeLocker.Get(base.isServer);
		if (!itemContainerEntity.IsUnityNull<IItemContainerEntity>())
		{
			itemContainerEntity.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x000A1B0C File Offset: 0x0009FD0C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity itemContainerEntity = this.activeStorage.Get(base.isServer);
		if (!itemContainerEntity.IsUnityNull<IItemContainerEntity>())
		{
			itemContainerEntity.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000A1B78 File Offset: 0x0009FD78
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			if (this.activeStorage.IsValid(base.isServer))
			{
				this.activeStorage.Get(base.isServer).DropItems(null);
			}
			if (this.activeBbq.IsValid(base.isServer))
			{
				this.activeBbq.Get(base.isServer).DropItems(null);
			}
			if (this.activeLocker.IsValid(base.isServer))
			{
				this.activeLocker.Get(base.isServer).DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000A1C10 File Offset: 0x0009FE10
	public IItemContainerEntity GetContainer()
	{
		Locker locker = this.activeLocker.Get(base.isServer);
		if (locker != null && locker.IsValid() && !locker.inventory.IsEmpty())
		{
			return locker;
		}
		global::BaseOven baseOven = this.activeBbq.Get(base.isServer);
		if (baseOven != null && baseOven.IsValid() && !baseOven.inventory.IsEmpty())
		{
			return baseOven;
		}
		StorageContainer storageContainer = this.activeStorage.Get(base.isServer);
		if (storageContainer != null && storageContainer.IsValid() && !storageContainer.inventory.IsEmpty())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000A1CB4 File Offset: 0x0009FEB4
	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::SleepingBagCamper sleepingBagCamper;
				if ((sleepingBagCamper = (enumerator.Current as global::SleepingBagCamper)) != null)
				{
					stringBuilder.AppendLine(string.Format("Bag {0}:", num++));
					stringBuilder.AppendLine(sleepingBagCamper.Admin_Who());
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x000A1D40 File Offset: 0x0009FF40
	public override bool CanBeLooted(global::BasePlayer player)
	{
		if (base.IsOnAVehicle && base.Vehicle.IsDead())
		{
			return base.CanBeLooted(player);
		}
		return base.CanBeLooted(player) && this.IsOnThisModule(player);
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x000A1D74 File Offset: 0x0009FF74
	public override bool IsOnThisModule(global::BasePlayer player)
	{
		if (base.IsOnThisModule(player))
		{
			return true;
		}
		if (!player.isMounted)
		{
			return false;
		}
		OBB obb = new OBB(base.transform, this.bounds);
		return obb.Contains(player.CenterPoint());
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x000A1DB8 File Offset: 0x0009FFB8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.camperModule != null)
		{
			this.activeBbq.uid = info.msg.camperModule.bbqId;
			this.activeLocker.uid = info.msg.camperModule.lockerId;
			this.activeStorage.uid = info.msg.camperModule.storageID;
		}
	}
}
