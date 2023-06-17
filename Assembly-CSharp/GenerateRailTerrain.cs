using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006C1 RID: 1729
public class GenerateRailTerrain : ProceduralComponent
{
	// Token: 0x0400282F RID: 10287
	public const int SmoothenLoops = 8;

	// Token: 0x04002830 RID: 10288
	public const int SmoothenIterations = 8;

	// Token: 0x04002831 RID: 10289
	public const int SmoothenY = 64;

	// Token: 0x04002832 RID: 10290
	public const int SmoothenXZ = 32;

	// Token: 0x04002833 RID: 10291
	public const int TransitionSteps = 8;

	// Token: 0x0600319B RID: 12699 RVA: 0x0012DCF8 File Offset: 0x0012BEF8
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Func<int, float> func = (int i) => Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 8f, (float)i));
		for (int l = 0; l < 8; l++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
			{
				PathInterpolator path = pathList.Path;
				Vector3[] points = path.Points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector3 vector = points[j];
					float t = pathList.Start ? func(j) : 1f;
					vector.y = Mathf.SmoothStep(vector.y, heightMap.GetHeight(vector), t);
					points[j] = vector;
				}
				path.Smoothen(8, Vector3.up, pathList.Start ? func : null);
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(8f, 0f, (float)l);
				pathList.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
		foreach (PathList pathList2 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path2 = pathList2.Path;
			Vector3[] points2 = path2.Points;
			for (int k = 0; k < points2.Length; k++)
			{
				Vector3 vector2 = points2[k];
				float t2 = pathList2.Start ? func(k) : 1f;
				vector2.y = Mathf.SmoothStep(vector2.y, heightMap.GetHeight(vector2), t2);
				points2[k] = vector2;
			}
			path2.RecalculateTangents();
		}
	}
}
