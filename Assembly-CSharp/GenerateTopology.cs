using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006D2 RID: 1746
public class GenerateTopology : ProceduralComponent
{
	// Token: 0x060031C6 RID: 12742
	[DllImport("RustNative", EntryPoint = "generate_topology")]
	public static extern void Native_GenerateTopology(int[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres, byte[] biomemap, int biomeres);

	// Token: 0x060031C7 RID: 12743 RVA: 0x0013034C File Offset: 0x0012E54C
	public override void Process(uint seed)
	{
		int[] dst = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		byte[] src2 = TerrainMeta.BiomeMap.src;
		int res3 = TerrainMeta.BiomeMap.res;
		GenerateTopology.Native_GenerateTopology(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2, src2, res3);
	}
}
