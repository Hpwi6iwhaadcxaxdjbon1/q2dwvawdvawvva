using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000925 RID: 2341
public static class RendererEx
{
	// Token: 0x04003336 RID: 13110
	private static readonly Memoized<Material[], int> ArrayCache = new Memoized<Material[], int>((int n) => new Material[n]);

	// Token: 0x06003853 RID: 14419 RVA: 0x0015011C File Offset: 0x0014E31C
	public static void SetSharedMaterials(this Renderer renderer, List<Material> materials)
	{
		if (materials.Count == 0)
		{
			return;
		}
		if (materials.Count > 10)
		{
			throw new ArgumentOutOfRangeException("materials");
		}
		Material[] array = RendererEx.ArrayCache.Get(materials.Count);
		for (int i = 0; i < materials.Count; i++)
		{
			array[i] = materials[i];
		}
		renderer.sharedMaterials = array;
	}
}
