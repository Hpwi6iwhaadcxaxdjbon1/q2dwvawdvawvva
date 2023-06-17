using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class Marketplace : global::BaseEntity
{
	// Token: 0x0400102D RID: 4141
	[Header("Marketplace")]
	public GameObjectRef terminalPrefab;

	// Token: 0x0400102E RID: 4142
	public Transform[] terminalPoints;

	// Token: 0x0400102F RID: 4143
	public Transform droneLaunchPoint;

	// Token: 0x04001030 RID: 4144
	public GameObjectRef deliveryDronePrefab;

	// Token: 0x04001031 RID: 4145
	public EntityRef<global::MarketTerminal>[] terminalEntities;

	// Token: 0x06001771 RID: 6001 RVA: 0x000B24EC File Offset: 0x000B06EC
	public NetworkableId SendDrone(global::BasePlayer player, global::MarketTerminal sourceTerminal, global::VendingMachine vendingMachine)
	{
		if (sourceTerminal == null || vendingMachine == null)
		{
			return default(NetworkableId);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.deliveryDronePrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, this.droneLaunchPoint.position, this.droneLaunchPoint.rotation, true);
		global::DeliveryDrone deliveryDrone;
		if ((deliveryDrone = (baseEntity as global::DeliveryDrone)) == null)
		{
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			return default(NetworkableId);
		}
		deliveryDrone.OwnerID = player.userID;
		deliveryDrone.Spawn();
		deliveryDrone.Setup(this, sourceTerminal, vendingMachine);
		return deliveryDrone.net.ID;
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x000B258C File Offset: 0x000B078C
	public void ReturnDrone(global::DeliveryDrone deliveryDrone)
	{
		global::MarketTerminal marketTerminal;
		if (deliveryDrone.sourceTerminal.TryGet(true, out marketTerminal))
		{
			marketTerminal.CompleteOrder(deliveryDrone.targetVendingMachine.uid);
		}
		deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x000B25C1 File Offset: 0x000B07C1
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnSubEntities();
		}
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x000B25D8 File Offset: 0x000B07D8
	private void SpawnSubEntities()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.terminalEntities != null && this.terminalEntities.Length > this.terminalPoints.Length)
		{
			for (int i = this.terminalPoints.Length; i < this.terminalEntities.Length; i++)
			{
				global::MarketTerminal marketTerminal;
				if (this.terminalEntities[i].TryGet(true, out marketTerminal))
				{
					marketTerminal.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
		}
		Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, this.terminalPoints.Length);
		for (int j = 0; j < this.terminalPoints.Length; j++)
		{
			Transform transform = this.terminalPoints[j];
			global::MarketTerminal marketTerminal2;
			if (!this.terminalEntities[j].TryGet(true, out marketTerminal2))
			{
				GameManager server = GameManager.server;
				GameObjectRef gameObjectRef = this.terminalPrefab;
				global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, transform.position, transform.rotation, true);
				baseEntity.SetParent(this, true, false);
				baseEntity.Spawn();
				global::MarketTerminal marketTerminal3;
				if ((marketTerminal3 = (baseEntity as global::MarketTerminal)) == null)
				{
					Debug.LogError("Marketplace.terminalPrefab did not spawn a MarketTerminal (it spawned " + baseEntity.GetType().FullName + ")");
					baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
				else
				{
					marketTerminal3.Setup(this);
					this.terminalEntities[j].Set(marketTerminal3);
				}
			}
		}
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x000B271C File Offset: 0x000B091C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.subEntityList != null)
		{
			List<NetworkableId> subEntityIds = info.msg.subEntityList.subEntityIds;
			Array.Resize<EntityRef<global::MarketTerminal>>(ref this.terminalEntities, subEntityIds.Count);
			for (int i = 0; i < subEntityIds.Count; i++)
			{
				this.terminalEntities[i] = new EntityRef<global::MarketTerminal>(subEntityIds[i]);
			}
		}
		this.SpawnSubEntities();
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x000B2790 File Offset: 0x000B0990
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.subEntityList = Pool.Get<SubEntityList>();
		info.msg.subEntityList.subEntityIds = Pool.GetList<NetworkableId>();
		if (this.terminalEntities != null)
		{
			for (int i = 0; i < this.terminalEntities.Length; i++)
			{
				info.msg.subEntityList.subEntityIds.Add(this.terminalEntities[i].uid);
			}
		}
	}
}
