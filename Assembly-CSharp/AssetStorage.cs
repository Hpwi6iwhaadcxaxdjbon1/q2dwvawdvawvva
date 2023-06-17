using System;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public static class AssetStorage
{
	// Token: 0x0600387C RID: 14460 RVA: 0x001511E3 File Offset: 0x0014F3E3
	public static void Save<T>(ref T asset, string path) where T : UnityEngine.Object
	{
		asset;
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Save(ref Texture2D asset)
	{
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x001511F6 File Offset: 0x0014F3F6
	public static void Save(ref Texture2D asset, string path, bool linear, bool compress)
	{
		asset;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void Load<T>(ref T asset, string path) where T : UnityEngine.Object
	{
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x00151200 File Offset: 0x0014F400
	public static void Delete<T>(ref T asset) where T : UnityEngine.Object
	{
		if (!asset)
		{
			return;
		}
		UnityEngine.Object.Destroy(asset);
		asset = default(T);
	}
}
