using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class FileSystem_Warmup : MonoBehaviour
{
	// Token: 0x040018BB RID: 6331
	public static bool ranInBackground = false;

	// Token: 0x040018BC RID: 6332
	public static Coroutine warmupTask;

	// Token: 0x040018BD RID: 6333
	private static bool run = true;

	// Token: 0x040018BE RID: 6334
	public static string[] ExcludeFilter = new string[]
	{
		"/bundled/prefabs/autospawn/monument",
		"/bundled/prefabs/autospawn/mountain",
		"/bundled/prefabs/autospawn/canyon",
		"/bundled/prefabs/autospawn/decor",
		"/bundled/prefabs/navmesh",
		"/content/ui/",
		"/prefabs/ui/",
		"/prefabs/world/",
		"/prefabs/system/",
		"/standard assets/",
		"/third party/"
	};

	// Token: 0x06001F4C RID: 8012 RVA: 0x000D3D40 File Offset: 0x000D1F40
	public static void Run()
	{
		if (Global.skipAssetWarmup_crashes)
		{
			return;
		}
		if (!FileSystem_Warmup.run)
		{
			return;
		}
		string[] assetList = FileSystem_Warmup.GetAssetList(null);
		for (int i = 0; i < assetList.Length; i++)
		{
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		FileSystem_Warmup.run = false;
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000D3D88 File Offset: 0x000D1F88
	public static IEnumerator Run(string[] assetList, Action<string> statusFunction = null, string format = null, int priority = 0)
	{
		if (Global.warmupConcurrency <= 1)
		{
			return FileSystem_Warmup.RunImpl(assetList, statusFunction, format);
		}
		return FileSystem_Warmup.RunAsyncImpl(assetList, statusFunction, format, priority);
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000D3DA4 File Offset: 0x000D1FA4
	private static IEnumerator RunAsyncImpl(string[] assetList, Action<string> statusFunction, string format, int priority)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch statusSw = Stopwatch.StartNew();
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult preload = FileSystem.PreloadAssets(assetList, Global.warmupConcurrency, priority);
		int warmupIndex = 0;
		while (preload.MoveNext() || warmupIndex < preload.TotalCount)
		{
			float num = FileSystem_Warmup.CalculateFrameBudget();
			if (num > 0f)
			{
				while (warmupIndex < preload.Results.Count && sw.Elapsed.TotalSeconds < (double)num)
				{
					IReadOnlyList<ValueTuple<string, UnityEngine.Object>> results = preload.Results;
					int num2 = warmupIndex;
					warmupIndex = num2 + 1;
					FileSystem_Warmup.PrefabWarmup(results[num2].Item1);
				}
			}
			if (warmupIndex == 0 || warmupIndex == preload.TotalCount || statusSw.Elapsed.TotalSeconds > 1.0)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format(format ?? "{0}/{1}", warmupIndex, preload.TotalCount));
				}
				statusSw.Restart();
			}
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000D3DC8 File Offset: 0x000D1FC8
	private static IEnumerator RunImpl(string[] assetList, Action<string> statusFunction = null, string format = null)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < assetList.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)FileSystem_Warmup.CalculateFrameBudget() || i == 0 || i == assetList.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format((format != null) ? format : "{0}/{1}", i + 1, assetList.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
			num = i;
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x00032C42 File Offset: 0x00030E42
	private static float CalculateFrameBudget()
	{
		return 2f;
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000D3DE8 File Offset: 0x000D1FE8
	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < FileSystem_Warmup.ExcludeFilter.Length; i++)
		{
			if (path.Contains(FileSystem_Warmup.ExcludeFilter[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000D3E1C File Offset: 0x000D201C
	public static string[] GetAssetList(bool? poolFilter = null)
	{
		if (poolFilter == null)
		{
			return (from x in GameManifest.Current.prefabProperties
			select x.name into x
			where !FileSystem_Warmup.ShouldIgnore(x)
			select x).ToArray<string>();
		}
		return (from x in GameManifest.Current.prefabProperties.Where(delegate(GameManifest.PrefabProperties x)
		{
			if (!FileSystem_Warmup.ShouldIgnore(x.name))
			{
				bool pool = x.pool;
				bool? poolFilter2 = poolFilter;
				return pool == poolFilter2.GetValueOrDefault() & poolFilter2 != null;
			}
			return false;
		})
		select x.name).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray<string>();
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000D3EE9 File Offset: 0x000D20E9
	private static void PrefabWarmup(string path)
	{
		GameManager.server.FindPrefab(path);
	}
}
