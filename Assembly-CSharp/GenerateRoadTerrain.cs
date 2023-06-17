using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006CC RID: 1740
public class GenerateRoadTerrain : ProceduralComponent
{
	// Token: 0x04002862 RID: 10338
	public const int SmoothenLoops = 2;

	// Token: 0x04002863 RID: 10339
	public const int SmoothenIterations = 8;

	// Token: 0x04002864 RID: 10340
	public const int SmoothenY = 16;

	// Token: 0x04002865 RID: 10341
	public const int SmoothenXZ = 4;

	// Token: 0x060031B6 RID: 12726 RVA: 0x0012FE68 File Offset: 0x0012E068
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologymap = TerrainMeta.TopologyMap;
		for (int l = 0; l < 2; l++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
			{
				PathInterpolator path = pathList.Path;
				Vector3[] points = path.Points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector3 vector = points[j];
					vector.y = heightMap.GetHeight(vector);
					points[j] = vector;
				}
				path.Smoothen(8, Vector3.up, delegate(int i)
				{
					int topology = topologymap.GetTopology(path.Points[i]);
					if ((topology & 524288) != 0)
					{
						return 0f;
					}
					if ((topology & 1048576) != 0)
					{
						return 0.5f;
					}
					return 1f;
				});
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(2f, 0f, (float)l);
				pathList.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
			foreach (PathList pathList2 in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
			{
				heightMap.Push();
				float intensity2 = 1f;
				float num = Mathf.InverseLerp(2f, 0f, (float)l);
				pathList2.AdjustTerrainHeight(intensity2, num / 4f);
				heightMap.Pop();
			}
		}
		foreach (PathList pathList3 in TerrainMeta.Path.Roads)
		{
			PathInterpolator path2 = pathList3.Path;
			Vector3[] points2 = path2.Points;
			for (int k = 0; k < points2.Length; k++)
			{
				Vector3 vector2 = points2[k];
				vector2.y = heightMap.GetHeight(vector2);
				points2[k] = vector2;
			}
			path2.RecalculateTangents();
		}
	}
}
