using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	// Token: 0x040022DA RID: 8922
	public float TickInterval = 60f;

	// Token: 0x040022DB RID: 8923
	public int MinSpawnsPerTick = 100;

	// Token: 0x040022DC RID: 8924
	public int MaxSpawnsPerTick = 100;

	// Token: 0x040022DD RID: 8925
	public LayerMask PlacementMask;

	// Token: 0x040022DE RID: 8926
	public LayerMask PlacementCheckMask;

	// Token: 0x040022DF RID: 8927
	public float PlacementCheckHeight = 25f;

	// Token: 0x040022E0 RID: 8928
	public LayerMask RadiusCheckMask;

	// Token: 0x040022E1 RID: 8929
	public float RadiusCheckDistance = 5f;

	// Token: 0x040022E2 RID: 8930
	public LayerMask BoundsCheckMask;

	// Token: 0x040022E3 RID: 8931
	public SpawnFilter CharacterSpawn;

	// Token: 0x040022E4 RID: 8932
	public float CharacterSpawnCutoff;

	// Token: 0x040022E5 RID: 8933
	public SpawnPopulation[] SpawnPopulations;

	// Token: 0x040022E6 RID: 8934
	internal SpawnDistribution[] SpawnDistributions;

	// Token: 0x040022E7 RID: 8935
	internal SpawnDistribution CharDistribution;

	// Token: 0x040022E8 RID: 8936
	internal ListHashSet<ISpawnGroup> SpawnGroups = new ListHashSet<ISpawnGroup>(8);

	// Token: 0x040022E9 RID: 8937
	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	// Token: 0x040022EA RID: 8938
	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;

	// Token: 0x040022EB RID: 8939
	private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;

	// Token: 0x040022EC RID: 8940
	private bool spawnTick;

	// Token: 0x040022ED RID: 8941
	private SpawnPopulation[] AllSpawnPopulations;

	// Token: 0x06002AE2 RID: 10978 RVA: 0x0010482C File Offset: 0x00102A2C
	protected void OnEnable()
	{
		this.AllSpawnPopulations = this.SpawnPopulations.Concat(this.ConvarSpawnPopulations).ToArray<SpawnPopulation>();
		base.StartCoroutine(this.SpawnTick());
		base.StartCoroutine(this.SpawnGroupTick());
		base.StartCoroutine(this.SpawnIndividualTick());
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0010487C File Offset: 0x00102A7C
	public static BasePlayer.SpawnPoint GetSpawnPoint()
	{
		if (SingletonComponent<SpawnHandler>.Instance == null || SingletonComponent<SpawnHandler>.Instance.CharDistribution == null)
		{
			return null;
		}
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		if (!((WaterSystem.OceanLevel < 0.5f) ? SpawnHandler.GetSpawnPointStandard(spawnPoint) : FloodedSpawnHandler.GetSpawnPoint(spawnPoint, WaterSystem.OceanLevel + 1f)))
		{
			return null;
		}
		return spawnPoint;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x001048D4 File Offset: 0x00102AD4
	private static bool GetSpawnPointStandard(BasePlayer.SpawnPoint spawnPoint)
	{
		for (int i = 0; i < 60; i++)
		{
			if (SingletonComponent<SpawnHandler>.Instance.CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot, false, 0f))
			{
				bool flag = true;
				if (TerrainMeta.Path != null)
				{
					using (List<MonumentInfo>.Enumerator enumerator = TerrainMeta.Path.Monuments.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Distance(spawnPoint.pos) < 50f)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x00104984 File Offset: 0x00102B84
	public void UpdateDistributions()
	{
		SpawnHandler.<>c__DisplayClass23_0 CS$<>8__locals1 = new SpawnHandler.<>c__DisplayClass23_0();
		if (global::World.Size == 0U)
		{
			return;
		}
		this.SpawnDistributions = new SpawnDistribution[this.AllSpawnPopulations.Length];
		this.population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
		Vector3 size = TerrainMeta.Size;
		Vector3 position = TerrainMeta.Position;
		CS$<>8__locals1.pop_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.25f));
		SpawnFilter filter;
		byte[] map;
		float cutoff;
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
			if (spawnPopulation == null)
			{
				Debug.LogError("Spawn handler contains null spawn population.");
			}
			else
			{
				byte[] map = new byte[CS$<>8__locals1.pop_res * CS$<>8__locals1.pop_res];
				SpawnFilter filter = spawnPopulation.Filter;
				float cutoff = spawnPopulation.FilterCutoff;
				Parallel.For(0, CS$<>8__locals1.pop_res, delegate(int z)
				{
					for (int j = 0; j < CS$<>8__locals1.pop_res; j++)
					{
						float normX = ((float)j + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float normZ = ((float)z + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float factor = filter.GetFactor(normX, normZ, true);
						map[z * CS$<>8__locals1.pop_res + j] = (byte)((factor >= cutoff) ? (255f * factor) : 0f);
					}
				});
				SpawnDistribution value = this.SpawnDistributions[i] = new SpawnDistribution(this, map, position, size);
				this.population2distribution.Add(spawnPopulation, value);
			}
		}
		CS$<>8__locals1.char_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.5f));
		map = new byte[CS$<>8__locals1.char_res * CS$<>8__locals1.char_res];
		filter = this.CharacterSpawn;
		cutoff = this.CharacterSpawnCutoff;
		Parallel.For(0, CS$<>8__locals1.char_res, delegate(int z)
		{
			for (int j = 0; j < CS$<>8__locals1.char_res; j++)
			{
				float normX = ((float)j + 0.5f) / (float)CS$<>8__locals1.char_res;
				float normZ = ((float)z + 0.5f) / (float)CS$<>8__locals1.char_res;
				float factor = filter.GetFactor(normX, normZ, true);
				map[z * CS$<>8__locals1.char_res + j] = (byte)((factor >= cutoff) ? (255f * factor) : 0f);
			}
		});
		this.CharDistribution = new SpawnDistribution(this, map, position, size);
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x00104B58 File Offset: 0x00102D58
	public void FillPopulations()
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
			}
		}
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x00104BA8 File Offset: 0x00102DA8
	public void FillGroups()
	{
		for (int i = 0; i < this.SpawnGroups.Count; i++)
		{
			this.SpawnGroups[i].Fill();
		}
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x00104BDC File Offset: 0x00102DDC
	public void FillIndividuals()
	{
		for (int i = 0; i < this.SpawnIndividuals.Count; i++)
		{
			SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
			this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x00104C2C File Offset: 0x00102E2C
	public void InitialSpawn()
	{
		if (ConVar.Spawn.respawn_populations && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
				}
			}
		}
		if (ConVar.Spawn.respawn_groups)
		{
			for (int j = 0; j < this.SpawnGroups.Count; j++)
			{
				this.SpawnGroups[j].SpawnInitial();
			}
		}
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x00104CAF File Offset: 0x00102EAF
	public void StartSpawnTick()
	{
		this.spawnTick = true;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x00104CB8 File Offset: 0x00102EB8
	private IEnumerator SpawnTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_populations)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_populations);
				int num;
				for (int i = 0; i < this.AllSpawnPopulations.Length; i = num + 1)
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					if (!(spawnPopulation == null))
					{
						SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
						if (spawnDistribution != null)
						{
							try
							{
								if (this.SpawnDistributions != null)
								{
									this.SpawnRepeating(spawnPopulation, spawnDistribution);
								}
							}
							catch (Exception message)
							{
								Debug.LogError(message);
							}
							yield return CoroutineEx.waitForEndOfFrame;
						}
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x00104CC7 File Offset: 0x00102EC7
	private IEnumerator SpawnGroupTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_groups)
			{
				yield return CoroutineEx.waitForSeconds(1f);
				int num;
				for (int i = 0; i < this.SpawnGroups.Count; i = num + 1)
				{
					ISpawnGroup spawnGroup = this.SpawnGroups[i];
					if (spawnGroup != null)
					{
						try
						{
							spawnGroup.SpawnRepeating();
						}
						catch (Exception message)
						{
							Debug.LogError(message);
						}
						yield return CoroutineEx.waitForEndOfFrame;
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x00104CD6 File Offset: 0x00102ED6
	private IEnumerator SpawnIndividualTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_individuals)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_individuals);
				int num;
				for (int i = 0; i < this.SpawnIndividuals.Count; i = num + 1)
				{
					SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
					try
					{
						this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
					yield return CoroutineEx.waitForEndOfFrame;
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x00104CE8 File Offset: 0x00102EE8
	public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsInitial);
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x00104D20 File Offset: 0x00102F20
	public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		num = Mathf.RoundToInt((float)num * population.GetCurrentSpawnRate());
		num = UnityEngine.Random.Range(Mathf.Min(num, this.MinSpawnsPerTick), Mathf.Min(num, this.MaxSpawnsPerTick));
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsRepeating);
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x00104D84 File Offset: 0x00102F84
	private void Fill(SpawnPopulation population, SpawnDistribution distribution, int targetCount, int numToFill, int numToTry)
	{
		if (targetCount == 0)
		{
			return;
		}
		if (!population.Initialize())
		{
			Debug.LogError("[Spawn] No prefabs to spawn in " + population.ResourceFolder, population);
			return;
		}
		if (Global.developer > 1)
		{
			Debug.Log(string.Concat(new object[]
			{
				"[Spawn] Population ",
				population.ResourceFolder,
				" needs to spawn ",
				numToFill
			}));
		}
		float num = Mathf.Max((float)population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
		population.UpdateWeights(distribution, targetCount);
		while (numToFill >= population.ClusterSizeMin && numToTry > 0)
		{
			ByteQuadtree.Element node = distribution.SampleNode();
			int num2 = UnityEngine.Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
			num2 = Mathx.Min(numToTry, numToFill, num2);
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector;
				Quaternion rot;
				bool flag = distribution.Sample(out vector, out rot, node, population.AlignToNormal, (float)population.ClusterDithering) && population.Filter.GetFactor(vector, true) > 0f;
				if (flag && population.FilterRadius > 0f)
				{
					flag = (population.Filter.GetFactor(vector + Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector + Vector3.right * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.right * population.FilterRadius, true) > 0f);
				}
				Prefab<Spawnable> prefab;
				if (flag && population.TryTakeRandomPrefab(out prefab))
				{
					if (population.GetSpawnPosOverride(prefab, ref vector, ref rot) && (float)distribution.GetCount(vector) < num)
					{
						this.Spawn(population, prefab, vector, rot);
						numToFill--;
					}
					else
					{
						population.ReturnPrefab(prefab);
					}
				}
				numToTry--;
			}
		}
		population.OnPostFill(this);
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x00104FA8 File Offset: 0x001031A8
	public GameObject Spawn(SpawnPopulation population, Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (prefab == null)
		{
			return null;
		}
		if (prefab.Component == null)
		{
			Debug.LogError("[Spawn] Missing component 'Spawnable' on " + prefab.Name);
			return null;
		}
		Vector3 one = Vector3.one;
		DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(prefab.ID);
		prefab.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref one);
		if (!prefab.ApplyTerrainAnchors(ref pos, rot, one, TerrainAnchorMode.MinimizeMovement, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainChecks(pos, rot, one, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainFilters(pos, rot, one, null))
		{
			return null;
		}
		if (!prefab.ApplyWaterChecks(pos, rot, one))
		{
			return null;
		}
		if (!prefab.ApplyBoundsChecks(pos, rot, one, this.BoundsCheckMask))
		{
			return null;
		}
		if (Global.developer > 1)
		{
			Debug.Log("[Spawn] Spawning " + prefab.Name);
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, false);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		Spawnable component = baseEntity.GetComponent<Spawnable>();
		if (component.Population != population)
		{
			component.Population = population;
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x001050E4 File Offset: 0x001032E4
	private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (!this.CheckBounds(prefab.Object, pos, rot, Vector3.one))
		{
			return null;
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x0010513E File Offset: 0x0010333E
	public bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		return SpawnHandler.CheckBounds(gameObject, pos, rot, scale, this.BoundsCheckMask);
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x00105150 File Offset: 0x00103350
	public static bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale, LayerMask mask)
	{
		if (gameObject == null)
		{
			return true;
		}
		if (mask != 0)
		{
			BaseEntity component = gameObject.GetComponent<BaseEntity>();
			if (component != null && UnityEngine.Physics.CheckBox(pos + rot * Vector3.Scale(component.bounds.center, scale), Vector3.Scale(component.bounds.extents, scale), rot, mask))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x001051C4 File Offset: 0x001033C4
	public void EnforceLimits(bool forceAll = false)
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
				SpawnDistribution distribution = this.SpawnDistributions[i];
				if (forceAll || spawnPopulation.EnforcePopulationLimits)
				{
					this.EnforceLimits(spawnPopulation, distribution);
				}
			}
		}
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x00105224 File Offset: 0x00103424
	private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		Spawnable[] array = this.FindAll(population);
		if (array.Length <= targetCount)
		{
			return;
		}
		Debug.Log(string.Concat(new object[]
		{
			population,
			" has ",
			array.Length,
			" objects, but max allowed is ",
			targetCount
		}));
		int num = array.Length - targetCount;
		Debug.Log(" - deleting " + num + " objects");
		foreach (Spawnable spawnable in array.Take(num))
		{
			BaseEntity baseEntity = spawnable.gameObject.ToBaseEntity();
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(spawnable.gameObject, 0f);
			}
		}
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x00105310 File Offset: 0x00103510
	public Spawnable[] FindAll(SpawnPopulation population)
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Spawnable>()
		where x.gameObject.activeInHierarchy && x.Population == population
		select x).ToArray<Spawnable>();
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x00105348 File Offset: 0x00103548
	public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		float num = TerrainMeta.Size.x * TerrainMeta.Size.z;
		float num2 = population.GetCurrentSpawnDensity();
		if (!population.ScaleWithLargeMaps)
		{
			num = Mathf.Min(num, 16000000f);
		}
		if (population.ScaleWithSpawnFilter)
		{
			num2 *= distribution.Density;
		}
		return Mathf.RoundToInt(num * num2);
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x0010539F File Offset: 0x0010359F
	public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		return distribution.Count;
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x001053A7 File Offset: 0x001035A7
	public void AddRespawn(SpawnIndividual individual)
	{
		this.SpawnIndividuals.Add(individual);
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x001053B8 File Offset: 0x001035B8
	public void AddInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to add instance to invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.AddInstance(spawnable);
		}
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x00105408 File Offset: 0x00103608
	public void RemoveInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to remove instance from invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.RemoveInstance(spawnable);
		}
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x00105458 File Offset: 0x00103658
	public static float PlayerFraction()
	{
		float num = (float)Mathf.Max(Server.maxplayers, 1);
		return Mathf.Clamp01((float)BasePlayer.activePlayerList.Count / num);
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x00105484 File Offset: 0x00103684
	public static float PlayerLerp(float min, float max)
	{
		return Mathf.Lerp(min, max, SpawnHandler.PlayerFraction());
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x00105494 File Offset: 0x00103694
	public static float PlayerExcess()
	{
		float num = Mathf.Max(ConVar.Spawn.player_base, 1f);
		float num2 = (float)BasePlayer.activePlayerList.Count;
		if (num2 <= num)
		{
			return 0f;
		}
		return (num2 - num) / num;
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x001054CC File Offset: 0x001036CC
	public static float PlayerScale(float scalar)
	{
		return Mathf.Max(1f, SpawnHandler.PlayerExcess() * scalar);
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x001054DF File Offset: 0x001036DF
	public void DumpReport(string filename)
	{
		File.AppendAllText(filename, "\r\n\r\nSpawnHandler Report:\r\n\r\n" + this.GetReport(true));
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x001054F8 File Offset: 0x001036F8
	public string GetReport(bool detailed = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.AllSpawnPopulations == null)
		{
			stringBuilder.AppendLine("Spawn population array is null.");
		}
		if (this.SpawnDistributions == null)
		{
			stringBuilder.AppendLine("Spawn distribution array is null.");
		}
		if (this.AllSpawnPopulations != null && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
					if (spawnPopulation != null)
					{
						if (!string.IsNullOrEmpty(spawnPopulation.ResourceFolder))
						{
							stringBuilder.AppendLine(spawnPopulation.name + " (autospawn/" + spawnPopulation.ResourceFolder + ")");
						}
						else
						{
							stringBuilder.AppendLine(spawnPopulation.name);
						}
						if (detailed)
						{
							stringBuilder.AppendLine("\tPrefabs:");
							if (spawnPopulation.Prefabs != null)
							{
								foreach (Prefab<Spawnable> prefab in spawnPopulation.Prefabs)
								{
									stringBuilder.AppendLine(string.Concat(new object[]
									{
										"\t\t",
										prefab.Name,
										" - ",
										prefab.Object
									}));
								}
							}
							else
							{
								stringBuilder.AppendLine("\t\tN/A");
							}
						}
						if (spawnDistribution != null)
						{
							int currentCount = this.GetCurrentCount(spawnPopulation, spawnDistribution);
							int targetCount = this.GetTargetCount(spawnPopulation, spawnDistribution);
							stringBuilder.AppendLine(string.Concat(new object[]
							{
								"\tPopulation: ",
								currentCount,
								"/",
								targetCount
							}));
						}
						else
						{
							stringBuilder.AppendLine("\tDistribution #" + i + " is not set.");
						}
					}
					else
					{
						stringBuilder.AppendLine("Population #" + i + " is not set.");
					}
					stringBuilder.AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}
}
