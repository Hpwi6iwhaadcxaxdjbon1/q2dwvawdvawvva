using System;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
public abstract class TerrainPlacement : PrefabAttribute
{
	// Token: 0x040028DC RID: 10460
	[ReadOnly]
	public Vector3 size = Vector3.zero;

	// Token: 0x040028DD RID: 10461
	[ReadOnly]
	public Vector3 extents = Vector3.zero;

	// Token: 0x040028DE RID: 10462
	[ReadOnly]
	public Vector3 offset = Vector3.zero;

	// Token: 0x040028DF RID: 10463
	public bool HeightMap = true;

	// Token: 0x040028E0 RID: 10464
	public bool AlphaMap = true;

	// Token: 0x040028E1 RID: 10465
	public bool WaterMap;

	// Token: 0x040028E2 RID: 10466
	[InspectorFlags]
	public TerrainSplat.Enum SplatMask;

	// Token: 0x040028E3 RID: 10467
	[InspectorFlags]
	public TerrainBiome.Enum BiomeMask;

	// Token: 0x040028E4 RID: 10468
	[InspectorFlags]
	public TerrainTopology.Enum TopologyMask;

	// Token: 0x040028E5 RID: 10469
	[HideInInspector]
	public Texture2DRef heightmap;

	// Token: 0x040028E6 RID: 10470
	[HideInInspector]
	public Texture2DRef splatmap0;

	// Token: 0x040028E7 RID: 10471
	[HideInInspector]
	public Texture2DRef splatmap1;

	// Token: 0x040028E8 RID: 10472
	[HideInInspector]
	public Texture2DRef alphamap;

	// Token: 0x040028E9 RID: 10473
	[HideInInspector]
	public Texture2DRef biomemap;

	// Token: 0x040028EA RID: 10474
	[HideInInspector]
	public Texture2DRef topologymap;

	// Token: 0x040028EB RID: 10475
	[HideInInspector]
	public Texture2DRef watermap;

	// Token: 0x040028EC RID: 10476
	[HideInInspector]
	public Texture2DRef blendmap;

	// Token: 0x06003235 RID: 12853 RVA: 0x00135C8C File Offset: 0x00133E8C
	public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.ShouldHeight())
		{
			this.ApplyHeight(localToWorld, worldToLocal);
		}
		if (this.ShouldSplat(-1))
		{
			this.ApplySplat(localToWorld, worldToLocal);
		}
		if (this.ShouldAlpha())
		{
			this.ApplyAlpha(localToWorld, worldToLocal);
		}
		if (this.ShouldBiome(-1))
		{
			this.ApplyBiome(localToWorld, worldToLocal);
		}
		if (this.ShouldTopology(-1))
		{
			this.ApplyTopology(localToWorld, worldToLocal);
		}
		if (this.ShouldWater())
		{
			this.ApplyWater(localToWorld, worldToLocal);
		}
	}

	// Token: 0x06003236 RID: 12854 RVA: 0x00135CFC File Offset: 0x00133EFC
	protected bool ShouldHeight()
	{
		return this.heightmap.isValid && this.HeightMap;
	}

	// Token: 0x06003237 RID: 12855 RVA: 0x00135D13 File Offset: 0x00133F13
	protected bool ShouldSplat(int id = -1)
	{
		return this.splatmap0.isValid && this.splatmap1.isValid && (this.SplatMask & (TerrainSplat.Enum)id) > (TerrainSplat.Enum)0;
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x00135D3C File Offset: 0x00133F3C
	protected bool ShouldAlpha()
	{
		return this.alphamap.isValid && this.AlphaMap;
	}

	// Token: 0x06003239 RID: 12857 RVA: 0x00135D53 File Offset: 0x00133F53
	protected bool ShouldBiome(int id = -1)
	{
		return this.biomemap.isValid && (this.BiomeMask & (TerrainBiome.Enum)id) > (TerrainBiome.Enum)0;
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x00135D6F File Offset: 0x00133F6F
	protected bool ShouldTopology(int id = -1)
	{
		return this.topologymap.isValid && (this.TopologyMask & (TerrainTopology.Enum)id) > (TerrainTopology.Enum)0;
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x00135D8B File Offset: 0x00133F8B
	protected bool ShouldWater()
	{
		return this.watermap.isValid && this.WaterMap;
	}

	// Token: 0x0600323C RID: 12860
	protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600323D RID: 12861
	protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600323E RID: 12862
	protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x0600323F RID: 12863
	protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06003240 RID: 12864
	protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06003241 RID: 12865
	protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

	// Token: 0x06003242 RID: 12866 RVA: 0x00135DA2 File Offset: 0x00133FA2
	protected override Type GetIndexedType()
	{
		return typeof(TerrainPlacement);
	}
}
