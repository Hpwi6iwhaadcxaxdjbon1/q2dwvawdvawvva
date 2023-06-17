using System;
using UnityEngine;

// Token: 0x020006A1 RID: 1697
public class TerrainMargin
{
	// Token: 0x0400278E RID: 10126
	private static MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x060030D3 RID: 12499 RVA: 0x00124C9C File Offset: 0x00122E9C
	public static void Create()
	{
		Material marginMaterial = TerrainMeta.Config.MarginMaterial;
		Vector3 center = TerrainMeta.Center;
		Vector3 size = TerrainMeta.Size;
		Vector3 b = new Vector3(size.x, 0f, 0f);
		Vector3 b2 = new Vector3(0f, 0f, size.z);
		center.y = TerrainMeta.HeightMap.GetHeight(0, 0);
		TerrainMargin.Create(center - b2, size, marginMaterial);
		TerrainMargin.Create(center - b2 - b, size, marginMaterial);
		TerrainMargin.Create(center - b2 + b, size, marginMaterial);
		TerrainMargin.Create(center - b, size, marginMaterial);
		TerrainMargin.Create(center + b, size, marginMaterial);
		TerrainMargin.Create(center + b2, size, marginMaterial);
		TerrainMargin.Create(center + b2 - b, size, marginMaterial);
		TerrainMargin.Create(center + b2 + b, size, marginMaterial);
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x00124D90 File Offset: 0x00122F90
	private static void Create(Vector3 position, Vector3 size, Material material)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "TerrainMargin";
		gameObject.layer = 16;
		gameObject.transform.position = position;
		gameObject.transform.localScale = size * 0.1f;
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<MeshFilter>());
	}
}
