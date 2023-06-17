using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000568 RID: 1384
[CreateAssetMenu(menuName = "Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
	// Token: 0x04002286 RID: 8838
	[Header("Spawnables")]
	public string ResourceFolder = string.Empty;

	// Token: 0x04002287 RID: 8839
	public GameObjectRef[] ResourceList;

	// Token: 0x04002288 RID: 8840
	[Header("Spawn Info")]
	[Tooltip("Usually per square km")]
	[SerializeField]
	[FormerlySerializedAs("TargetDensity")]
	private float _targetDensity = 1f;

	// Token: 0x04002289 RID: 8841
	public float SpawnRate = 1f;

	// Token: 0x0400228A RID: 8842
	public int ClusterSizeMin = 1;

	// Token: 0x0400228B RID: 8843
	public int ClusterSizeMax = 1;

	// Token: 0x0400228C RID: 8844
	public int ClusterDithering;

	// Token: 0x0400228D RID: 8845
	public int SpawnAttemptsInitial = 20;

	// Token: 0x0400228E RID: 8846
	public int SpawnAttemptsRepeating = 10;

	// Token: 0x0400228F RID: 8847
	public bool EnforcePopulationLimits = true;

	// Token: 0x04002290 RID: 8848
	public bool ScaleWithLargeMaps = true;

	// Token: 0x04002291 RID: 8849
	public bool ScaleWithSpawnFilter = true;

	// Token: 0x04002292 RID: 8850
	public bool ScaleWithServerPopulation;

	// Token: 0x04002293 RID: 8851
	public bool AlignToNormal;

	// Token: 0x04002294 RID: 8852
	public SpawnFilter Filter = new SpawnFilter();

	// Token: 0x04002295 RID: 8853
	public float FilterCutoff;

	// Token: 0x04002296 RID: 8854
	public float FilterRadius;

	// Token: 0x04002297 RID: 8855
	internal Prefab<Spawnable>[] Prefabs;

	// Token: 0x04002298 RID: 8856
	private int[] numToSpawn;

	// Token: 0x04002299 RID: 8857
	private int sumToSpawn;

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002A72 RID: 10866 RVA: 0x00102FB3 File Offset: 0x001011B3
	public virtual float TargetDensity
	{
		get
		{
			return this._targetDensity;
		}
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x00102FBC File Offset: 0x001011BC
	public bool Initialize()
	{
		if (this.Prefabs == null || this.Prefabs.Length == 0)
		{
			if (!string.IsNullOrEmpty(this.ResourceFolder))
			{
				this.Prefabs = Prefab.Load<Spawnable>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, GameManager.server, PrefabAttribute.server, false);
			}
			if (this.ResourceList != null && this.ResourceList.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (GameObjectRef gameObjectRef in this.ResourceList)
				{
					string resourcePath = gameObjectRef.resourcePath;
					if (string.IsNullOrEmpty(resourcePath))
					{
						Debug.LogWarning(base.name + " resource list contains invalid resource path for GUID " + gameObjectRef.guid, this);
					}
					else
					{
						list.Add(resourcePath);
					}
				}
				this.Prefabs = Prefab.Load<Spawnable>(list.ToArray(), GameManager.server, PrefabAttribute.server);
			}
			if (this.Prefabs == null || this.Prefabs.Length == 0)
			{
				return false;
			}
			this.numToSpawn = new int[this.Prefabs.Length];
		}
		return true;
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x001030BC File Offset: 0x001012BC
	public void UpdateWeights(SpawnDistribution distribution, int targetCount)
	{
		int num = 0;
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			Prefab<Spawnable> prefab = this.Prefabs[i];
			int prefabWeight = this.GetPrefabWeight(prefab);
			num += prefabWeight;
		}
		int num2 = Mathf.CeilToInt((float)targetCount / (float)num);
		this.sumToSpawn = 0;
		for (int j = 0; j < this.Prefabs.Length; j++)
		{
			Prefab<Spawnable> prefab2 = this.Prefabs[j];
			int prefabWeight2 = this.GetPrefabWeight(prefab2);
			int count = distribution.GetCount(prefab2.ID);
			int num3 = Mathf.Max(prefabWeight2 * num2 - count, 0);
			this.numToSpawn[j] = num3;
			this.sumToSpawn += num3;
		}
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x00103167 File Offset: 0x00101367
	protected virtual int GetPrefabWeight(Prefab<Spawnable> prefab)
	{
		if (!prefab.Parameters)
		{
			return 1;
		}
		return prefab.Parameters.Count;
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x00103184 File Offset: 0x00101384
	public bool TryTakeRandomPrefab(out Prefab<Spawnable> result)
	{
		int num = UnityEngine.Random.Range(0, this.sumToSpawn);
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if ((num -= this.numToSpawn[i]) < 0)
			{
				this.numToSpawn[i]--;
				this.sumToSpawn--;
				result = this.Prefabs[i];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x001031F0 File Offset: 0x001013F0
	public void ReturnPrefab(Prefab<Spawnable> prefab)
	{
		if (prefab == null)
		{
			return;
		}
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if (this.Prefabs[i] == prefab)
			{
				this.numToSpawn[i]++;
				this.sumToSpawn++;
			}
		}
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x0010323E File Offset: 0x0010143E
	public float GetCurrentSpawnRate()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.SpawnRate * SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
		}
		return this.SpawnRate * Spawn.max_rate;
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x0010326B File Offset: 0x0010146B
	public float GetCurrentSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x001032A4 File Offset: 0x001014A4
	public float GetMaximumSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return 2f * this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return 2f * this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		return true;
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPostFill(SpawnHandler spawnHandler)
	{
	}
}
