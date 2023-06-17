using System;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public class IndividualSpawner : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x040022A9 RID: 8873
	public GameObjectRef entityPrefab;

	// Token: 0x040022AA RID: 8874
	public float respawnDelayMin = 10f;

	// Token: 0x040022AB RID: 8875
	public float respawnDelayMax = 20f;

	// Token: 0x040022AC RID: 8876
	public bool useCustomBoundsCheckMask;

	// Token: 0x040022AD RID: 8877
	public LayerMask customBoundsCheckMask;

	// Token: 0x040022AE RID: 8878
	[Tooltip("Simply spawns the entity once. No respawning. Entity can be saved if desired.")]
	[SerializeField]
	private bool oneTimeSpawner;

	// Token: 0x040022AF RID: 8879
	internal bool isSpawnerActive = true;

	// Token: 0x040022B0 RID: 8880
	private SpawnPointInstance spawnInstance;

	// Token: 0x040022B1 RID: 8881
	private float nextSpawnTime = -1f;

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002AA2 RID: 10914 RVA: 0x001039D5 File Offset: 0x00101BD5
	public int currentPopulation
	{
		get
		{
			if (!(this.spawnInstance == null))
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002AA3 RID: 10915 RVA: 0x001039E8 File Offset: 0x00101BE8
	private bool IsSpawned
	{
		get
		{
			return this.spawnInstance != null;
		}
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x001039F6 File Offset: 0x00101BF6
	protected void Awake()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x00103A2F File Offset: 0x00101C2F
	protected void OnDestroy()
	{
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove(this);
			return;
		}
		Debug.LogWarning(base.GetType().Name + ": SpawnHandler instance not found.");
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x00103A6C File Offset: 0x00101C6C
	protected void OnDrawGizmosSelected()
	{
		Bounds bounds;
		if (this.TryGetEntityBounds(out bounds))
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawCube(bounds.center, bounds.size);
		}
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x00103AB0 File Offset: 0x00101CB0
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstance = instance;
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x00103AB9 File Offset: 0x00101CB9
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstance = null;
		this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x00103ADF File Offset: 0x00101CDF
	public void Fill()
	{
		if (this.oneTimeSpawner)
		{
			return;
		}
		this.TrySpawnEntity();
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x00103AF0 File Offset: 0x00101CF0
	public void SpawnInitial()
	{
		this.TrySpawnEntity();
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x00103AF8 File Offset: 0x00101CF8
	public void Clear()
	{
		if (this.IsSpawned)
		{
			BaseEntity baseEntity = this.spawnInstance.gameObject.ToBaseEntity();
			if (baseEntity != null)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x00103B2E File Offset: 0x00101D2E
	public void SpawnRepeating()
	{
		if (this.IsSpawned || this.oneTimeSpawner)
		{
			return;
		}
		if (Time.time >= this.nextSpawnTime)
		{
			this.TrySpawnEntity();
		}
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x00103B54 File Offset: 0x00101D54
	public bool HasSpaceToSpawn()
	{
		if (this.useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one, this.customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(this.entityPrefab.Get(), base.transform.position, base.transform.rotation, Vector3.one);
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x00103BCC File Offset: 0x00101DCC
	private void TrySpawnEntity()
	{
		if (!this.isSpawnerActive)
		{
			return;
		}
		if (this.IsSpawned)
		{
			return;
		}
		if (!this.HasSpaceToSpawn())
		{
			this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.respawnDelayMin, this.respawnDelayMax);
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, base.transform.position, base.transform.rotation, false);
		if (baseEntity != null)
		{
			if (!this.oneTimeSpawner)
			{
				baseEntity.enableSaving = false;
			}
			baseEntity.gameObject.AwakeFromInstantiate();
			baseEntity.Spawn();
			SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
			spawnPointInstance.parentSpawnPointUser = this;
			spawnPointInstance.Notify();
			return;
		}
		Debug.LogError("IndividualSpawner failed to spawn entity.", base.gameObject);
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x00103C90 File Offset: 0x00101E90
	private bool TryGetEntityBounds(out Bounds result)
	{
		if (this.entityPrefab != null)
		{
			GameObject gameObject = this.entityPrefab.Get();
			if (gameObject != null)
			{
				BaseEntity component = gameObject.GetComponent<BaseEntity>();
				if (component != null)
				{
					result = component.bounds;
					return true;
				}
			}
		}
		result = default(Bounds);
		return false;
	}
}
