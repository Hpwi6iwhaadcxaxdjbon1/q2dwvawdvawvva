using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x0200074B RID: 1867
public class GameManifest : ScriptableObject
{
	// Token: 0x04002A56 RID: 10838
	internal static GameManifest loadedManifest;

	// Token: 0x04002A57 RID: 10839
	internal static Dictionary<string, string> guidToPath = new Dictionary<string, string>();

	// Token: 0x04002A58 RID: 10840
	internal static Dictionary<string, string> pathToGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x04002A59 RID: 10841
	internal static Dictionary<string, UnityEngine.Object> guidToObject = new Dictionary<string, UnityEngine.Object>();

	// Token: 0x04002A5A RID: 10842
	public GameManifest.PooledString[] pooledStrings;

	// Token: 0x04002A5B RID: 10843
	public GameManifest.PrefabProperties[] prefabProperties;

	// Token: 0x04002A5C RID: 10844
	public GameManifest.EffectCategory[] effectCategories;

	// Token: 0x04002A5D RID: 10845
	public GameManifest.GuidPath[] guidPaths;

	// Token: 0x04002A5E RID: 10846
	public string[] entities;

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06003450 RID: 13392 RVA: 0x00144099 File Offset: 0x00142299
	public static GameManifest Current
	{
		get
		{
			if (GameManifest.loadedManifest != null)
			{
				return GameManifest.loadedManifest;
			}
			GameManifest.Load();
			return GameManifest.loadedManifest;
		}
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x001440B8 File Offset: 0x001422B8
	public static void Load()
	{
		if (GameManifest.loadedManifest != null)
		{
			return;
		}
		GameManifest.loadedManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		foreach (GameManifest.PrefabProperties prefabProperties in GameManifest.loadedManifest.prefabProperties)
		{
			GameManifest.guidToPath.Add(prefabProperties.guid, prefabProperties.name);
			GameManifest.pathToGuid.Add(prefabProperties.name, prefabProperties.guid);
		}
		foreach (GameManifest.GuidPath guidPath in GameManifest.loadedManifest.guidPaths)
		{
			if (!GameManifest.guidToPath.ContainsKey(guidPath.guid))
			{
				GameManifest.guidToPath.Add(guidPath.guid, guidPath.name);
				GameManifest.pathToGuid.Add(guidPath.name, guidPath.guid);
			}
		}
		DebugEx.Log(GameManifest.GetMetadataStatus(), StackTraceLogType.None);
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x00144198 File Offset: 0x00142398
	public static void LoadAssets()
	{
		if (Skinnable.All != null)
		{
			return;
		}
		Skinnable.All = FileSystem.LoadAllFromBundle<Skinnable>("skinnables.preload.bundle", "t:Skinnable");
		if (Skinnable.All == null || Skinnable.All.Length == 0)
		{
			throw new Exception("Error loading skinnables");
		}
		DebugEx.Log(GameManifest.GetAssetStatus(), StackTraceLogType.None);
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x001441E8 File Offset: 0x001423E8
	internal static Dictionary<string, string[]> LoadEffectDictionary()
	{
		GameManifest.EffectCategory[] array = GameManifest.loadedManifest.effectCategories;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		foreach (GameManifest.EffectCategory effectCategory in array)
		{
			dictionary.Add(effectCategory.folder, effectCategory.prefabs.ToArray());
		}
		return dictionary;
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x00144230 File Offset: 0x00142430
	internal static string GUIDToPath(string guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			Debug.LogError("GUIDToPath: guid is empty");
			return string.Empty;
		}
		GameManifest.Load();
		string result;
		if (GameManifest.guidToPath.TryGetValue(guid, out result))
		{
			return result;
		}
		Debug.LogWarning("GUIDToPath: no path found for guid " + guid);
		return string.Empty;
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x00144280 File Offset: 0x00142480
	internal static UnityEngine.Object GUIDToObject(string guid)
	{
		UnityEngine.Object result = null;
		if (GameManifest.guidToObject.TryGetValue(guid, out result))
		{
			return result;
		}
		string text = GameManifest.GUIDToPath(guid);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Missing file for guid " + guid);
			GameManifest.guidToObject.Add(guid, null);
			return null;
		}
		UnityEngine.Object @object = FileSystem.Load<UnityEngine.Object>(text, true);
		GameManifest.guidToObject.Add(guid, @object);
		return @object;
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x001442E4 File Offset: 0x001424E4
	internal static void Invalidate(string path)
	{
		string key;
		if (!GameManifest.pathToGuid.TryGetValue(path, out key))
		{
			return;
		}
		UnityEngine.Object @object;
		if (!GameManifest.guidToObject.TryGetValue(key, out @object))
		{
			return;
		}
		if (@object != null)
		{
			UnityEngine.Object.DestroyImmediate(@object, true);
		}
		GameManifest.guidToObject.Remove(key);
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x00144330 File Offset: 0x00142530
	private static string GetMetadataStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Metadata Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.pooledStrings.Length.ToString());
			stringBuilder.Append(" pooled strings");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.prefabProperties.Length.ToString());
			stringBuilder.Append(" prefab properties");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.effectCategories.Length.ToString());
			stringBuilder.Append(" effect categories");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.entities.Length.ToString());
			stringBuilder.Append(" entity names");
			stringBuilder.AppendLine();
		}
		else
		{
			stringBuilder.Append("Manifest Metadata Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x00144464 File Offset: 0x00142664
	private static string GetAssetStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Assets Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append((Skinnable.All != null) ? Skinnable.All.Length.ToString() : "0");
			stringBuilder.Append(" skinnable objects");
		}
		else
		{
			stringBuilder.Append("Manifest Assets Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x02000E5A RID: 3674
	[Serializable]
	public struct PooledString
	{
		// Token: 0x04004B2C RID: 19244
		[HideInInspector]
		public string str;

		// Token: 0x04004B2D RID: 19245
		public uint hash;
	}

	// Token: 0x02000E5B RID: 3675
	[Serializable]
	public class PrefabProperties
	{
		// Token: 0x04004B2E RID: 19246
		[HideInInspector]
		public string name;

		// Token: 0x04004B2F RID: 19247
		public string guid;

		// Token: 0x04004B30 RID: 19248
		public uint hash;

		// Token: 0x04004B31 RID: 19249
		public bool pool;
	}

	// Token: 0x02000E5C RID: 3676
	[Serializable]
	public class EffectCategory
	{
		// Token: 0x04004B32 RID: 19250
		[HideInInspector]
		public string folder;

		// Token: 0x04004B33 RID: 19251
		public List<string> prefabs;
	}

	// Token: 0x02000E5D RID: 3677
	[Serializable]
	public class GuidPath
	{
		// Token: 0x04004B34 RID: 19252
		[HideInInspector]
		public string name;

		// Token: 0x04004B35 RID: 19253
		public string guid;
	}
}
