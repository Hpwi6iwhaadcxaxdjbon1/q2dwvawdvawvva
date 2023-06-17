using System;
using UnityEngine;

// Token: 0x020006E7 RID: 1767
public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	// Token: 0x040028C0 RID: 10432
	public TerrainConfig config;

	// Token: 0x040028C1 RID: 10433
	private const float HeightMapRes = 1f;

	// Token: 0x040028C2 RID: 10434
	private const float SplatMapRes = 0.5f;

	// Token: 0x040028C3 RID: 10435
	private const float BaseMapRes = 0.01f;

	// Token: 0x060031FF RID: 12799 RVA: 0x0013454F File Offset: 0x0013274F
	public static int GetHeightMapRes()
	{
		return Mathf.Min(4096, Mathf.ClosestPowerOfTwo((int)(World.Size * 1f))) + 1;
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x00134570 File Offset: 0x00132770
	public static int GetSplatMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.5f)));
	}

	// Token: 0x06003201 RID: 12801 RVA: 0x0013458F File Offset: 0x0013278F
	public static int GetBaseMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)(World.Size * 0.01f)));
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x001345AE File Offset: 0x001327AE
	public GameObject CreateTerrain()
	{
		return this.CreateTerrain(TerrainGenerator.GetHeightMapRes(), TerrainGenerator.GetSplatMapRes());
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x001345C0 File Offset: 0x001327C0
	public GameObject CreateTerrain(int heightmapResolution, int alphamapResolution)
	{
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData
		{
			baseMapResolution = TerrainGenerator.GetBaseMapRes(),
			heightmapResolution = heightmapResolution,
			alphamapResolution = alphamapResolution,
			size = new Vector3(World.Size, 1000f, World.Size)
		}).GetComponent<Terrain>();
		component.transform.position = base.transform.position + new Vector3((float)(-(float)((ulong)World.Size)) * 0.5f, 0f, (float)(-(float)((ulong)World.Size)) * 0.5f);
		component.drawInstanced = false;
		component.castShadows = this.config.CastShadows;
		component.materialType = Terrain.MaterialType.Custom;
		component.materialTemplate = this.config.Material;
		component.gameObject.tag = base.gameObject.tag;
		component.gameObject.layer = base.gameObject.layer;
		component.gameObject.GetComponent<TerrainCollider>().sharedMaterial = this.config.GenericMaterial;
		TerrainMeta terrainMeta = component.gameObject.AddComponent<TerrainMeta>();
		component.gameObject.AddComponent<TerrainPhysics>();
		component.gameObject.AddComponent<TerrainColors>();
		component.gameObject.AddComponent<TerrainCollision>();
		component.gameObject.AddComponent<TerrainBiomeMap>();
		component.gameObject.AddComponent<TerrainAlphaMap>();
		component.gameObject.AddComponent<TerrainHeightMap>();
		component.gameObject.AddComponent<TerrainSplatMap>();
		component.gameObject.AddComponent<TerrainTopologyMap>();
		component.gameObject.AddComponent<TerrainWaterMap>();
		component.gameObject.AddComponent<TerrainPlacementMap>();
		component.gameObject.AddComponent<TerrainPath>();
		component.gameObject.AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = this.config;
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		return component.gameObject;
	}
}
