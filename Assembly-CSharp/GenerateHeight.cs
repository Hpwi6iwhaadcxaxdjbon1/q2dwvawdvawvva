using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006B9 RID: 1721
public class GenerateHeight : ProceduralComponent
{
	// Token: 0x06003180 RID: 12672
	[DllImport("RustNative", EntryPoint = "generate_height")]
	public static extern void Native_GenerateHeight(short[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle);

	// Token: 0x06003181 RID: 12673 RVA: 0x0012B3D4 File Offset: 0x001295D4
	public override void Process(uint seed)
	{
		short[] dst = TerrainMeta.HeightMap.dst;
		int res = TerrainMeta.HeightMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		GenerateHeight.Native_GenerateHeight(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle);
	}
}
