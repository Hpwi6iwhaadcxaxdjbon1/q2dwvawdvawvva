using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class BradleySpawner : MonoBehaviour, IServerComponent
{
	// Token: 0x04001157 RID: 4439
	public BasePath path;

	// Token: 0x04001158 RID: 4440
	public GameObjectRef bradleyPrefab;

	// Token: 0x04001159 RID: 4441
	[NonSerialized]
	public BradleyAPC spawned;

	// Token: 0x0400115A RID: 4442
	public bool initialSpawn;

	// Token: 0x0400115B RID: 4443
	public float minRespawnTimeMinutes = 5f;

	// Token: 0x0400115C RID: 4444
	public float maxRespawnTimeMinutes = 5f;

	// Token: 0x0400115D RID: 4445
	public static BradleySpawner singleton;

	// Token: 0x0400115E RID: 4446
	private bool pendingRespawn;

	// Token: 0x06001898 RID: 6296 RVA: 0x000B77BC File Offset: 0x000B59BC
	public void Start()
	{
		BradleySpawner.singleton = this;
		base.Invoke("DelayedStart", 3f);
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x000B77D4 File Offset: 0x000B59D4
	public void DelayedStart()
	{
		if (this.initialSpawn)
		{
			this.DoRespawn();
		}
		base.InvokeRepeating("CheckIfRespawnNeeded", 0f, 5f);
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x000B77F9 File Offset: 0x000B59F9
	public void CheckIfRespawnNeeded()
	{
		if (!this.pendingRespawn && (this.spawned == null || !this.spawned.IsAlive()))
		{
			this.ScheduleRespawn();
		}
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x000B7824 File Offset: 0x000B5A24
	public void ScheduleRespawn()
	{
		base.CancelInvoke("DoRespawn");
		base.Invoke("DoRespawn", UnityEngine.Random.Range(Bradley.respawnDelayMinutes - Bradley.respawnDelayVariance, Bradley.respawnDelayMinutes + Bradley.respawnDelayVariance) * 60f);
		this.pendingRespawn = true;
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x000B7864 File Offset: 0x000B5A64
	public void DoRespawn()
	{
		if (!Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			this.SpawnBradley();
		}
		this.pendingRespawn = false;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000B7884 File Offset: 0x000B5A84
	public void SpawnBradley()
	{
		if (this.spawned != null)
		{
			Debug.LogWarning("Bradley attempting to spawn but one already exists!");
			return;
		}
		if (!Bradley.enabled)
		{
			return;
		}
		Vector3 position = this.path.interestZones[UnityEngine.Random.Range(0, this.path.interestZones.Count)].transform.position;
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.bradleyPrefab.resourcePath, position, default(Quaternion), true);
		BradleyAPC component = baseEntity.GetComponent<BradleyAPC>();
		if (component)
		{
			baseEntity.Spawn();
			component.InstallPatrolPath(this.path);
		}
		else
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		Debug.Log("BradleyAPC Spawned at :" + position);
		this.spawned = component;
	}
}
