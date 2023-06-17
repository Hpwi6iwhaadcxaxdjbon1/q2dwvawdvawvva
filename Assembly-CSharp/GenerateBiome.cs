using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006B2 RID: 1714
public class GenerateBiome : ProceduralComponent
{
	// Token: 0x0600315E RID: 12638
	[DllImport("RustNative", EntryPoint = "generate_biome")]
	public static extern void Native_GenerateBiome(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres);

	// Token: 0x0600315F RID: 12639 RVA: 0x001274E4 File Offset: 0x001256E4
	public override void Process(uint seed)
	{
		byte[] dst = TerrainMeta.BiomeMap.dst;
		int res = TerrainMeta.BiomeMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		GenerateBiome.Native_GenerateBiome(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2);
	}
}
