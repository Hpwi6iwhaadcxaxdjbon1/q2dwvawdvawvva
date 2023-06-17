using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000656 RID: 1622
public static class World
{
	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06002F17 RID: 12055 RVA: 0x0011CA98 File Offset: 0x0011AC98
	// (set) Token: 0x06002F18 RID: 12056 RVA: 0x0011CA9F File Offset: 0x0011AC9F
	public static uint Seed { get; set; }

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06002F19 RID: 12057 RVA: 0x0011CAA7 File Offset: 0x0011ACA7
	// (set) Token: 0x06002F1A RID: 12058 RVA: 0x0011CAAE File Offset: 0x0011ACAE
	public static uint Salt { get; set; }

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x06002F1B RID: 12059 RVA: 0x0011CAB6 File Offset: 0x0011ACB6
	// (set) Token: 0x06002F1C RID: 12060 RVA: 0x0011CABD File Offset: 0x0011ACBD
	public static uint Size { get; set; }

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x06002F1D RID: 12061 RVA: 0x0011CAC5 File Offset: 0x0011ACC5
	// (set) Token: 0x06002F1E RID: 12062 RVA: 0x0011CACC File Offset: 0x0011ACCC
	public static string Checksum { get; set; }

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002F1F RID: 12063 RVA: 0x0011CAD4 File Offset: 0x0011ACD4
	// (set) Token: 0x06002F20 RID: 12064 RVA: 0x0011CADB File Offset: 0x0011ACDB
	public static string Url { get; set; }

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06002F21 RID: 12065 RVA: 0x0011CAE3 File Offset: 0x0011ACE3
	// (set) Token: 0x06002F22 RID: 12066 RVA: 0x0011CAEA File Offset: 0x0011ACEA
	public static bool Procedural { get; set; }

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06002F23 RID: 12067 RVA: 0x0011CAF2 File Offset: 0x0011ACF2
	// (set) Token: 0x06002F24 RID: 12068 RVA: 0x0011CAF9 File Offset: 0x0011ACF9
	public static bool Cached { get; set; }

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002F25 RID: 12069 RVA: 0x0011CB01 File Offset: 0x0011AD01
	// (set) Token: 0x06002F26 RID: 12070 RVA: 0x0011CB08 File Offset: 0x0011AD08
	public static bool Networked { get; set; }

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06002F27 RID: 12071 RVA: 0x0011CB10 File Offset: 0x0011AD10
	// (set) Token: 0x06002F28 RID: 12072 RVA: 0x0011CB17 File Offset: 0x0011AD17
	public static bool Receiving { get; set; }

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06002F29 RID: 12073 RVA: 0x0011CB1F File Offset: 0x0011AD1F
	// (set) Token: 0x06002F2A RID: 12074 RVA: 0x0011CB26 File Offset: 0x0011AD26
	public static bool Transfer { get; set; }

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06002F2B RID: 12075 RVA: 0x0011CB2E File Offset: 0x0011AD2E
	// (set) Token: 0x06002F2C RID: 12076 RVA: 0x0011CB35 File Offset: 0x0011AD35
	public static bool LoadedFromSave { get; set; }

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002F2D RID: 12077 RVA: 0x0011CB3D File Offset: 0x0011AD3D
	// (set) Token: 0x06002F2E RID: 12078 RVA: 0x0011CB44 File Offset: 0x0011AD44
	public static int SpawnIndex { get; set; }

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06002F2F RID: 12079 RVA: 0x0011CB4C File Offset: 0x0011AD4C
	// (set) Token: 0x06002F30 RID: 12080 RVA: 0x0011CB53 File Offset: 0x0011AD53
	public static WorldSerialization Serialization { get; set; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06002F31 RID: 12081 RVA: 0x0011CB5B File Offset: 0x0011AD5B
	public static string Name
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(global::World.Url));
			}
			return Application.loadedLevelName;
		}
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x0011CB79 File Offset: 0x0011AD79
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetServerBrowserMapName()
	{
		if (!global::World.CanLoadFromUrl())
		{
			return global::World.Name;
		}
		if (global::World.Name.StartsWith("proceduralmap."))
		{
			return "Procedural Map";
		}
		return "Custom Map";
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x0011CBA4 File Offset: 0x0011ADA4
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(global::World.Url);
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x0011CBB3 File Offset: 0x0011ADB3
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromDisk()
	{
		return File.Exists(global::World.MapFolderName + "/" + global::World.MapFileName);
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x0011CBD0 File Offset: 0x0011ADD0
	public static void CleanupOldFiles()
	{
		Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + 238 + "\\.map");
		foreach (string path2 in from path in Directory.GetFiles(global::World.MapFolderName, "*.map")
		where regex1.IsMatch(path) && !regex2.IsMatch(path)
		select path)
		{
			try
			{
				File.Delete(path2);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
		}
	}

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x06002F36 RID: 12086 RVA: 0x0011CC8C File Offset: 0x0011AE8C
	public static string MapFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return global::World.Name + ".map";
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				238,
				".map"
			});
		}
	}

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x06002F37 RID: 12087 RVA: 0x0011CD1E File Offset: 0x0011AF1E
	public static string MapFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002F38 RID: 12088 RVA: 0x0011CD28 File Offset: 0x0011AF28
	public static string SaveFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return string.Concat(new object[]
				{
					global::World.Name,
					".",
					238,
					".sav"
				});
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				238,
				".sav"
			});
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002F39 RID: 12089 RVA: 0x0011CD1E File Offset: 0x0011AF1E
	public static string SaveFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x0011CDDB File Offset: 0x0011AFDB
	public static void InitSeed(int seed)
	{
		global::World.InitSeed((uint)seed);
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x0011CDE3 File Offset: 0x0011AFE3
	public static void InitSeed(uint seed)
	{
		if (seed == 0U)
		{
			seed = global::World.SeedIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (seed == 0U)
		{
			seed = 123456U;
		}
		global::World.Seed = seed;
		Server.seed = (int)seed;
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x0011CE10 File Offset: 0x0011B010
	private static string SeedIdentifier()
	{
		return string.Concat(new object[]
		{
			SystemInfo.deviceUniqueIdentifier,
			"_",
			238,
			"_",
			Server.identity
		});
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x0011CE4A File Offset: 0x0011B04A
	public static void InitSalt(int salt)
	{
		global::World.InitSalt((uint)salt);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x0011CE52 File Offset: 0x0011B052
	public static void InitSalt(uint salt)
	{
		if (salt == 0U)
		{
			salt = global::World.SaltIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (salt == 0U)
		{
			salt = 654321U;
		}
		global::World.Salt = salt;
		Server.salt = (int)salt;
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x0011CE7F File Offset: 0x0011B07F
	private static string SaltIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier + "_salt";
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x0011CE90 File Offset: 0x0011B090
	public static void InitSize(int size)
	{
		global::World.InitSize((uint)size);
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x0011CE98 File Offset: 0x0011B098
	public static void InitSize(uint size)
	{
		if (size == 0U)
		{
			size = 4500U;
		}
		if (size < 1000U)
		{
			size = 1000U;
		}
		if (size > 6000U)
		{
			size = 6000U;
		}
		global::World.Size = size;
		Server.worldsize = (int)size;
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x0011CED0 File Offset: 0x0011B0D0
	public static byte[] GetMap(string name)
	{
		MapData map = global::World.Serialization.GetMap(name);
		if (map == null)
		{
			return null;
		}
		return map.data;
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x0011CEF4 File Offset: 0x0011B0F4
	public static int GetCachedHeightMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("height").Length / 2)));
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x0011CF0F File Offset: 0x0011B10F
	public static int GetCachedSplatMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("splat").Length / 8)));
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x0011CF2A File Offset: 0x0011B12A
	public static void AddMap(string name, byte[] data)
	{
		global::World.Serialization.AddMap(name, data);
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x0011CF38 File Offset: 0x0011B138
	public static void AddPrefab(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		global::World.Serialization.AddPrefab(category, prefab.ID, position, rotation, scale);
		if (!global::World.Cached)
		{
			rotation = Quaternion.Euler(rotation.eulerAngles);
			global::World.Spawn(category, prefab, position, rotation, scale);
		}
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x0011CF70 File Offset: 0x0011B170
	public static PathData PathListToPathData(PathList src)
	{
		return new PathData
		{
			name = src.Name,
			spline = src.Spline,
			start = src.Start,
			end = src.End,
			width = src.Width,
			innerPadding = src.InnerPadding,
			outerPadding = src.OuterPadding,
			innerFade = src.InnerFade,
			outerFade = src.OuterFade,
			randomScale = src.RandomScale,
			meshOffset = src.MeshOffset,
			terrainOffset = src.TerrainOffset,
			splat = src.Splat,
			topology = src.Topology,
			hierarchy = src.Hierarchy,
			nodes = global::World.VectorArrayToList(src.Path.Points)
		};
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x0011D04C File Offset: 0x0011B24C
	public static PathList PathDataToPathList(PathData src)
	{
		PathList pathList = new PathList(src.name, global::World.VectorListToArray(src.nodes));
		pathList.Spline = src.spline;
		pathList.Start = src.start;
		pathList.End = src.end;
		pathList.Width = src.width;
		pathList.InnerPadding = src.innerPadding;
		pathList.OuterPadding = src.outerPadding;
		pathList.InnerFade = src.innerFade;
		pathList.OuterFade = src.outerFade;
		pathList.RandomScale = src.randomScale;
		pathList.MeshOffset = src.meshOffset;
		pathList.TerrainOffset = src.terrainOffset;
		pathList.Splat = src.splat;
		pathList.Topology = src.topology;
		pathList.Hierarchy = src.hierarchy;
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x0011D124 File Offset: 0x0011B324
	public static Vector3[] VectorListToArray(List<VectorData> src)
	{
		Vector3[] array = new Vector3[src.Count];
		for (int i = 0; i < array.Length; i++)
		{
			VectorData vectorData = src[i];
			array[i] = new Vector3
			{
				x = vectorData.x,
				y = vectorData.y,
				z = vectorData.z
			};
		}
		return array;
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x0011D18C File Offset: 0x0011B38C
	public static List<VectorData> VectorArrayToList(Vector3[] src)
	{
		List<VectorData> list = new List<VectorData>(src.Length);
		foreach (Vector3 vector in src)
		{
			list.Add(new VectorData
			{
				x = vector.x,
				y = vector.y,
				z = vector.z
			});
		}
		return list;
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x0011D1EF File Offset: 0x0011B3EF
	public static IEnumerable<PathList> GetPaths(string name)
	{
		return from p in global::World.Serialization.GetPaths(name)
		select global::World.PathDataToPathList(p);
	}

	// Token: 0x06002F4C RID: 12108 RVA: 0x0011D220 File Offset: 0x0011B420
	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList path in paths)
		{
			global::World.AddPath(path);
		}
	}

	// Token: 0x06002F4D RID: 12109 RVA: 0x0011D268 File Offset: 0x0011B468
	public static void AddPath(PathList path)
	{
		global::World.Serialization.AddPath(global::World.PathListToPathData(path));
	}

	// Token: 0x06002F4E RID: 12110 RVA: 0x0011D27A File Offset: 0x0011B47A
	public static IEnumerator SpawnAsync(float deltaTime, Action<string> statusFunction = null)
	{
		int totalCount = 0;
		Dictionary<string, List<PrefabData>> assetGroups = new Dictionary<string, List<PrefabData>>(StringComparer.InvariantCultureIgnoreCase);
		foreach (PrefabData prefabData in global::World.Serialization.world.prefabs)
		{
			string text = StringPool.Get(prefabData.id);
			if (string.IsNullOrWhiteSpace(text))
			{
				UnityEngine.Debug.LogWarning(string.Format("Could not find path for prefab ID {0}, skipping spawn", prefabData.id));
			}
			else
			{
				List<PrefabData> list;
				if (!assetGroups.TryGetValue(text, out list))
				{
					list = new List<PrefabData>();
					assetGroups.Add(text, list);
				}
				list.Add(prefabData);
				int num = totalCount;
				totalCount = num + 1;
			}
		}
		int spawnedCount = 0;
		int resultIndex = 0;
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult load = FileSystem.PreloadAssets(assetGroups.Keys, Global.preloadConcurrency, 10);
		while (load != null && (load.MoveNext() || assetGroups.Count > 0))
		{
			while (resultIndex < load.Results.Count && sw.Elapsed.TotalSeconds < (double)deltaTime)
			{
				string item = load.Results[resultIndex].Item1;
				List<PrefabData> list2;
				if (!assetGroups.TryGetValue(item, out list2))
				{
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else if (list2.Count == 0)
				{
					assetGroups.Remove(item);
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else
				{
					int index = list2.Count - 1;
					PrefabData prefab = list2[index];
					list2.RemoveAt(index);
					global::World.Spawn(prefab);
					int num = spawnedCount;
					spawnedCount = num + 1;
				}
			}
			global::World.Status(statusFunction, "Spawning World ({0}/{1})", spawnedCount, totalCount);
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		yield break;
	}

	// Token: 0x06002F4F RID: 12111 RVA: 0x0011D290 File Offset: 0x0011B490
	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == global::World.Serialization.world.prefabs.Count - 1)
			{
				global::World.Status(statusFunction, "Spawning World ({0}/{1})", i + 1, global::World.Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
			num = i;
		}
		yield break;
	}

	// Token: 0x06002F50 RID: 12112 RVA: 0x0011D2A8 File Offset: 0x0011B4A8
	public static void Spawn()
	{
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
		}
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x0011D2F0 File Offset: 0x0011B4F0
	public static void Spawn(string category, string folder = null)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string text = StringPool.Get(prefabData.id);
			if (!string.IsNullOrEmpty(folder) && !text.StartsWith(folder))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002F52 RID: 12114 RVA: 0x0011D374 File Offset: 0x0011B574
	public static void Spawn(string category, string[] folders)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string str = StringPool.Get(prefabData.id);
			if (folders != null && !str.StartsWithAny(folders))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x0011D3F2 File Offset: 0x0011B5F2
	private static void Spawn(PrefabData prefab)
	{
		global::World.Spawn(prefab.category, Prefab.Load(prefab.id, null, null), prefab.position, prefab.rotation, prefab.scale);
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x0011D430 File Offset: 0x0011B630
	private static void Spawn(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (prefab == null || !prefab.Object)
		{
			return;
		}
		if (!global::World.Cached)
		{
			prefab.ApplyTerrainPlacements(position, rotation, scale);
			prefab.ApplyTerrainModifiers(position, rotation, scale);
		}
		GameObject gameObject = prefab.Spawn(position, rotation, scale, true);
		if (gameObject)
		{
			gameObject.SetHierarchyGroup(category, true, false);
		}
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x0011D486 File Offset: 0x0011B686
	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1));
		}
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x0011D498 File Offset: 0x0011B698
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2));
		}
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x0011D4AB File Offset: 0x0011B6AB
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2, obj3));
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x0011D4C0 File Offset: 0x0011B6C0
	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, objs));
		}
	}
}
