using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class GenerateRiverTerrain : ProceduralComponent
{
	// Token: 0x04002842 RID: 10306
	public const int SmoothenLoops = 1;

	// Token: 0x04002843 RID: 10307
	public const int SmoothenIterations = 8;

	// Token: 0x04002844 RID: 10308
	public const int SmoothenY = 8;

	// Token: 0x04002845 RID: 10309
	public const int SmoothenXZ = 4;

	// Token: 0x060031A7 RID: 12711 RVA: 0x0012E7E0 File Offset: 0x0012C9E0
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int i = 0; i < 1; i++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
			{
				if (!World.Networked)
				{
					PathInterpolator path = pathList.Path;
					path.Smoothen(8, Vector3.up, null);
					path.RecalculateTangents();
				}
				heightMap.Push();
				float intensity = 1f;
				float fade = 1f / (1f + (float)i / 3f);
				pathList.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
	}
}
