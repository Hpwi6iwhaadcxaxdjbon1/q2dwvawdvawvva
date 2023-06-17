using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class VehicleSpawner : BaseEntity
{
	// Token: 0x0400111E RID: 4382
	public float spawnNudgeRadius = 6f;

	// Token: 0x0400111F RID: 4383
	public float cleanupRadius = 10f;

	// Token: 0x04001120 RID: 4384
	public float occupyRadius = 5f;

	// Token: 0x04001121 RID: 4385
	public VehicleSpawner.SpawnPair[] objectsToSpawn;

	// Token: 0x04001122 RID: 4386
	public Transform spawnOffset;

	// Token: 0x04001123 RID: 4387
	public float safeRadius = 10f;

	// Token: 0x06001861 RID: 6241 RVA: 0x000B62E6 File Offset: 0x000B44E6
	public virtual int GetOccupyLayer()
	{
		return 32768;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000B62F0 File Offset: 0x000B44F0
	public BaseVehicle GetVehicleOccupying()
	{
		BaseVehicle result = null;
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, this.occupyRadius, list, this.GetOccupyLayer(), QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return result;
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x000B6344 File Offset: 0x000B4544
	public bool IsPadOccupied()
	{
		BaseVehicle vehicleOccupying = this.GetVehicleOccupying();
		return vehicleOccupying != null && !vehicleOccupying.IsDespawnEligable();
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x000B636C File Offset: 0x000B456C
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		BasePlayer newOwner = null;
		NPCTalking component = from.GetComponent<NPCTalking>();
		if (component)
		{
			newOwner = component.GetActionPlayer();
		}
		foreach (VehicleSpawner.SpawnPair spawnPair in this.objectsToSpawn)
		{
			if (msg == spawnPair.message)
			{
				this.SpawnVehicle(spawnPair.prefabToSpawn.resourcePath, newOwner);
				return;
			}
		}
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000B63D0 File Offset: 0x000B45D0
	public BaseVehicle SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		this.CleanupArea(this.cleanupRadius);
		this.NudgePlayersInRadius(this.spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, this.spawnOffset.transform.position, this.spawnOffset.transform.rotation, true);
		baseEntity.Spawn();
		BaseVehicle component = baseEntity.GetComponent<BaseVehicle>();
		if (newOwner != null)
		{
			component.SetupOwner(newOwner, this.spawnOffset.transform.position, this.safeRadius);
		}
		VehicleSpawnPoint.AddStartingFuel(component);
		if (this.LogAnalytics)
		{
			Analytics.Server.VehiclePurchased(component.ShortPrefabName);
		}
		if (newOwner != null)
		{
			Analytics.Azure.OnVehiclePurchased(newOwner, baseEntity);
		}
		return component;
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001866 RID: 6246 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool LogAnalytics
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000B6480 File Offset: 0x000B4680
	public void CleanupArea(float radius)
	{
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities<BaseVehicle>(this.spawnOffset.transform.position, radius, list, 32768, QueryTriggerInteraction.Collide);
		foreach (BaseVehicle baseVehicle in list)
		{
			if (!baseVehicle.isClient && !baseVehicle.IsDestroyed)
			{
				baseVehicle.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities<ServerGib>(this.spawnOffset.transform.position, radius, list2, 67108865, QueryTriggerInteraction.Collide);
		foreach (ServerGib serverGib in list2)
		{
			if (!serverGib.isClient)
			{
				serverGib.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x000B657C File Offset: 0x000B477C
	public void NudgePlayersInRadius(float radius)
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(this.spawnOffset.transform.position, radius, list, 131072, QueryTriggerInteraction.Collide);
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsNpc && !basePlayer.isMounted && basePlayer.IsConnected)
			{
				Vector3 vector = this.spawnOffset.transform.position;
				vector += Vector3Ex.Direction2D(basePlayer.transform.position, this.spawnOffset.transform.position) * radius;
				vector += Vector3.up * 0.1f;
				basePlayer.MovePosition(vector);
				basePlayer.ClientRPCPlayer<Vector3>(null, basePlayer, "ForcePositionTo", vector);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	// Token: 0x02000C36 RID: 3126
	[Serializable]
	public class SpawnPair
	{
		// Token: 0x04004287 RID: 17031
		public string message;

		// Token: 0x04004288 RID: 17032
		public GameObjectRef prefabToSpawn;
	}
}
