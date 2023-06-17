using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006CF RID: 1743
public class GenerateSplat : ProceduralComponent
{
	// Token: 0x060031BD RID: 12733
	[DllImport("RustNative", EntryPoint = "generate_splat")]
	public static extern void Native_GenerateSplat(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres, byte[] biomemap, int biomeres, int[] topologymap, int topologyres);

	// Token: 0x060031BE RID: 12734 RVA: 0x001301E4 File Offset: 0x0012E3E4
	public override void Process(uint seed)
	{
		byte[] dst = TerrainMeta.SplatMap.dst;
		int res = TerrainMeta.SplatMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		byte[] src2 = TerrainMeta.BiomeMap.src;
		int res3 = TerrainMeta.BiomeMap.res;
		int[] src3 = TerrainMeta.TopologyMap.src;
		int res4 = TerrainMeta.TopologyMap.res;
		GenerateSplat.Native_GenerateSplat(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2, src2, res3, src3, res4);
	}
}
