using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public class Spawnable : MonoBehaviour, IServerComponent
{
	// Token: 0x0400229A RID: 8858
	[ReadOnly]
	public SpawnPopulation Population;

	// Token: 0x0400229B RID: 8859
	[SerializeField]
	private bool ForceSpawnOnly;

	// Token: 0x0400229C RID: 8860
	[SerializeField]
	private string ForceSpawnInfoMessage = string.Empty;

	// Token: 0x0400229D RID: 8861
	internal uint PrefabID;

	// Token: 0x0400229E RID: 8862
	internal bool SpawnIndividual;

	// Token: 0x0400229F RID: 8863
	internal Vector3 SpawnPosition;

	// Token: 0x040022A0 RID: 8864
	internal Quaternion SpawnRotation;

	// Token: 0x06002A7E RID: 10878 RVA: 0x00103366 File Offset: 0x00101566
	protected void OnEnable()
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Add();
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x00103376 File Offset: 0x00101576
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x00103390 File Offset: 0x00101590
	private void Add()
	{
		this.SpawnPosition = base.transform.position;
		this.SpawnRotation = base.transform.rotation;
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (this.Population != null)
			{
				SingletonComponent<SpawnHandler>.Instance.AddInstance(this);
				return;
			}
			if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
			{
				global::BaseEntity component = base.GetComponent<global::BaseEntity>();
				if (component != null && component.enableSaving && !component.syncPosition)
				{
					SingletonComponent<SpawnHandler>.Instance.AddRespawn(new SpawnIndividual(component.prefabID, this.SpawnPosition, this.SpawnRotation));
				}
			}
		}
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x00103434 File Offset: 0x00101634
	private void Remove()
	{
		if (SingletonComponent<SpawnHandler>.Instance && this.Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.RemoveInstance(this);
		}
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x0010345B File Offset: 0x0010165B
	internal void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.Population == null)
		{
			return;
		}
		info.msg.spawnable = Pool.Get<ProtoBuf.Spawnable>();
		info.msg.spawnable.population = this.Population.FilenameStringId;
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x00103497 File Offset: 0x00101697
	internal void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.spawnable != null)
		{
			this.Population = FileSystem.Load<SpawnPopulation>(StringPool.Get(info.msg.spawnable.population), true);
		}
		this.Add();
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x001034CD File Offset: 0x001016CD
	protected void OnValidate()
	{
		this.Population = null;
	}
}
