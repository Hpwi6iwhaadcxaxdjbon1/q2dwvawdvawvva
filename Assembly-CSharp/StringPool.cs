using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public class StringPool
{
	// Token: 0x04002AE5 RID: 10981
	private static Dictionary<uint, string> toString;

	// Token: 0x04002AE6 RID: 10982
	private static Dictionary<string, uint> toNumber;

	// Token: 0x04002AE7 RID: 10983
	private static bool initialized;

	// Token: 0x04002AE8 RID: 10984
	public static uint closest;

	// Token: 0x060034A9 RID: 13481 RVA: 0x00145D08 File Offset: 0x00143F08
	private static void Init()
	{
		if (StringPool.initialized)
		{
			return;
		}
		StringPool.toString = new Dictionary<uint, string>();
		StringPool.toNumber = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
		GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		uint num = 0U;
		while ((ulong)num < (ulong)((long)gameManifest.pooledStrings.Length))
		{
			string str = gameManifest.pooledStrings[(int)num].str;
			uint hash = gameManifest.pooledStrings[(int)num].hash;
			string text;
			if (StringPool.toString.TryGetValue(hash, out text))
			{
				if (str != text)
				{
					Debug.LogWarning(string.Format("Hash collision: {0} already exists in string pool. `{1}` and `{2}` have the same hash.", hash, str, text));
				}
			}
			else
			{
				StringPool.toString.Add(hash, str);
				StringPool.toNumber.Add(str, hash);
			}
			num += 1U;
		}
		StringPool.initialized = true;
		StringPool.closest = StringPool.Get("closest");
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x00145DDC File Offset: 0x00143FDC
	public static string Get(uint i)
	{
		if (i == 0U)
		{
			return string.Empty;
		}
		StringPool.Init();
		string result;
		if (StringPool.toString.TryGetValue(i, out result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetString - no string for ID" + i);
		return "";
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x00145E24 File Offset: 0x00144024
	public static uint Get(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0U;
		}
		StringPool.Init();
		uint result;
		if (StringPool.toNumber.TryGetValue(str, out result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetNumber - no number for string " + str);
		return 0U;
	}

	// Token: 0x060034AC RID: 13484 RVA: 0x00145E64 File Offset: 0x00144064
	public static uint Add(string str)
	{
		uint num = 0U;
		if (!StringPool.toNumber.TryGetValue(str, out num))
		{
			num = str.ManifestHash();
			StringPool.toString.Add(num, str);
			StringPool.toNumber.Add(str, num);
		}
		return num;
	}
}
