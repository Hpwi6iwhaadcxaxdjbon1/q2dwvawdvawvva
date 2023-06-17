using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000748 RID: 1864
public static class AssetNameCache
{
	// Token: 0x04002A46 RID: 10822
	private static Dictionary<UnityEngine.Object, string> mixed = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x04002A47 RID: 10823
	private static Dictionary<UnityEngine.Object, string> lower = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x04002A48 RID: 10824
	private static Dictionary<UnityEngine.Object, string> upper = new Dictionary<UnityEngine.Object, string>();

	// Token: 0x06003439 RID: 13369 RVA: 0x00143A74 File Offset: 0x00141C74
	private static string LookupName(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string name;
		if (!AssetNameCache.mixed.TryGetValue(obj, out name))
		{
			name = obj.name;
			AssetNameCache.mixed.Add(obj, name);
		}
		return name;
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x00143AB4 File Offset: 0x00141CB4
	private static string LookupNameLower(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.lower.TryGetValue(obj, out text))
		{
			text = obj.name.ToLower();
			AssetNameCache.lower.Add(obj, text);
		}
		return text;
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x00143AF8 File Offset: 0x00141CF8
	private static string LookupNameUpper(UnityEngine.Object obj)
	{
		if (obj == null)
		{
			return string.Empty;
		}
		string text;
		if (!AssetNameCache.upper.TryGetValue(obj, out text))
		{
			text = obj.name.ToUpper();
			AssetNameCache.upper.Add(obj, text);
		}
		return text;
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x00143B3C File Offset: 0x00141D3C
	public static string GetName(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x0600343D RID: 13373 RVA: 0x00143B44 File Offset: 0x00141D44
	public static string GetNameLower(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x00143B4C File Offset: 0x00141D4C
	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x00143B3C File Offset: 0x00141D3C
	public static string GetName(this Material mat)
	{
		return AssetNameCache.LookupName(mat);
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x00143B44 File Offset: 0x00141D44
	public static string GetNameLower(this Material mat)
	{
		return AssetNameCache.LookupNameLower(mat);
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x00143B4C File Offset: 0x00141D4C
	public static string GetNameUpper(this Material mat)
	{
		return AssetNameCache.LookupNameUpper(mat);
	}
}
