using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class Prefab : IComparable<Prefab>
{
	// Token: 0x0400223F RID: 8767
	public uint ID;

	// Token: 0x04002240 RID: 8768
	public string Name;

	// Token: 0x04002241 RID: 8769
	public string Folder;

	// Token: 0x04002242 RID: 8770
	public GameObject Object;

	// Token: 0x04002243 RID: 8771
	public GameManager Manager;

	// Token: 0x04002244 RID: 8772
	public PrefabAttribute.Library Attribute;

	// Token: 0x04002245 RID: 8773
	public PrefabParameters Parameters;

	// Token: 0x06002A00 RID: 10752 RVA: 0x00100250 File Offset: 0x000FE450
	public Prefab(string name, GameObject prefab, GameManager manager, PrefabAttribute.Library attribute)
	{
		this.ID = StringPool.Get(name);
		this.Name = name;
		this.Folder = (string.IsNullOrWhiteSpace(name) ? "" : Path.GetDirectoryName(name));
		this.Object = prefab;
		this.Manager = manager;
		this.Attribute = attribute;
		this.Parameters = (prefab ? prefab.GetComponent<PrefabParameters>() : null);
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x001002BE File Offset: 0x000FE4BE
	public static implicit operator GameObject(Prefab prefab)
	{
		return prefab.Object;
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x001002C8 File Offset: 0x000FE4C8
	public int CompareTo(Prefab that)
	{
		if (that == null)
		{
			return 1;
		}
		PrefabPriority prefabPriority = (this.Parameters != null) ? this.Parameters.Priority : PrefabPriority.Default;
		return ((that.Parameters != null) ? that.Parameters.Priority : PrefabPriority.Default).CompareTo(prefabPriority);
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x00100328 File Offset: 0x000FE528
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, mode, filter);
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x00100360 File Offset: 0x000FE560
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, filter);
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x00100398 File Offset: 0x000FE598
	public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainCheck[] anchors = this.Attribute.FindAll<TerrainCheck>(this.ID);
		return this.Object.transform.ApplyTerrainChecks(anchors, pos, rot, scale, filter);
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x001003D0 File Offset: 0x000FE5D0
	public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainFilter[] filters = this.Attribute.FindAll<TerrainFilter>(this.ID);
		return this.Object.transform.ApplyTerrainFilters(filters, pos, rot, scale, filter);
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x00100408 File Offset: 0x000FE608
	public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainModifier[] modifiers = this.Attribute.FindAll<TerrainModifier>(this.ID);
		this.Object.transform.ApplyTerrainModifiers(modifiers, pos, rot, scale);
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x0010043C File Offset: 0x000FE63C
	public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainPlacement[] placements = this.Attribute.FindAll<TerrainPlacement>(this.ID);
		this.Object.transform.ApplyTerrainPlacements(placements, pos, rot, scale);
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x00100470 File Offset: 0x000FE670
	public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		WaterCheck[] anchors = this.Attribute.FindAll<WaterCheck>(this.ID);
		return this.Object.transform.ApplyWaterChecks(anchors, pos, rot, scale);
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x001004A4 File Offset: 0x000FE6A4
	public bool ApplyBoundsChecks(Vector3 pos, Quaternion rot, Vector3 scale, LayerMask rejectOnLayer)
	{
		BoundsCheck[] bounds = this.Attribute.FindAll<BoundsCheck>(this.ID);
		BaseEntity component = this.Object.GetComponent<BaseEntity>();
		return !(component != null) || component.ApplyBoundsChecks(bounds, pos, rot, scale, rejectOnLayer);
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x001004E8 File Offset: 0x000FE6E8
	public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		DecorComponent[] components = this.Attribute.FindAll<DecorComponent>(this.ID);
		this.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x0010051B File Offset: 0x000FE71B
	public bool CheckEnvironmentVolumes(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		return this.Object.transform.CheckEnvironmentVolumes(pos, rot, scale, type);
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x00100532 File Offset: 0x000FE732
	public bool CheckEnvironmentVolumesInsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesInsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x0010054B File Offset: 0x000FE74B
	public bool CheckEnvironmentVolumesOutsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesOutsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x00100564 File Offset: 0x000FE764
	public void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		PathSequence pathSequence = this.Attribute.Find<PathSequence>(this.ID);
		if (pathSequence != null)
		{
			pathSequence.ApplySequenceReplacement(sequence, ref replacement, possibleReplacements, pathLength, pathIndex);
		}
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x00100599 File Offset: 0x000FE799
	public GameObject Spawn(Transform transform, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, transform, active);
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x001005AE File Offset: 0x000FE7AE
	public GameObject Spawn(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, active);
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x001005C4 File Offset: 0x000FE7C4
	public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, scale, active);
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x001005DC File Offset: 0x000FE7DC
	public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreateEntity(this.Name, pos, rot, active);
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x001005F4 File Offset: 0x000FE7F4
	public static Prefab<T> Load<T>(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x00100654 File Offset: 0x000FE854
	public static Prefab Load(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null)
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject prefab = manager.FindPrefab(text);
		return new Prefab(text, prefab, manager, attribute);
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x001006AC File Offset: 0x000FE8AC
	public static Prefab[] Load(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		Prefab[] array2 = new Prefab[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array[i];
			GameObject prefab = manager.FindPrefab(text);
			array2[i] = new Prefab(text, prefab, manager, attribute);
		}
		return array2;
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x00100711 File Offset: 0x000FE911
	public static Prefab<T>[] Load<T>(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x0010072C File Offset: 0x000FE92C
	public static Prefab<T>[] Load<T>(string[] names, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		Prefab<T>[] array = new Prefab<T>[names.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = names[i];
			GameObject gameObject = manager.FindPrefab(text);
			T component = gameObject.GetComponent<T>();
			array[i] = new Prefab<T>(text, gameObject, component, manager, attribute);
		}
		return array;
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x00100788 File Offset: 0x000FE988
	public static Prefab LoadRandom(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject prefab = manager.FindPrefab(text);
		return new Prefab(text, prefab, manager, attribute);
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x001007E0 File Offset: 0x000FE9E0
	public static Prefab<T> LoadRandom<T>(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06002A1B RID: 10779 RVA: 0x00100840 File Offset: 0x000FEA40
	public static PrefabAttribute.Library DefaultAttribute
	{
		get
		{
			return PrefabAttribute.server;
		}
	}

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002A1C RID: 10780 RVA: 0x00100847 File Offset: 0x000FEA47
	public static GameManager DefaultManager
	{
		get
		{
			return GameManager.server;
		}
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x00100850 File Offset: 0x000FEA50
	private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
	{
		strPrefab = strPrefab.TrimEnd(new char[]
		{
			'/'
		}).ToLower();
		GameObject[] array = FileSystem.LoadPrefabs(strPrefab + "/");
		List<string> list = new List<string>(array.Length);
		foreach (GameObject gameObject in array)
		{
			string item = strPrefab + "/" + gameObject.name.ToLower() + ".prefab";
			if (!useProbabilities)
			{
				list.Add(item);
			}
			else
			{
				PrefabParameters component = gameObject.GetComponent<PrefabParameters>();
				int num = component ? component.Count : 1;
				for (int j = 0; j < num; j++)
				{
					list.Add(item);
				}
			}
		}
		list.Sort();
		return list.ToArray();
	}
}
