using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
public class GenerateRiverLayout : ProceduralComponent
{
	// Token: 0x04002834 RID: 10292
	public const float Width = 36f;

	// Token: 0x04002835 RID: 10293
	public const float InnerPadding = 1f;

	// Token: 0x04002836 RID: 10294
	public const float OuterPadding = 1f;

	// Token: 0x04002837 RID: 10295
	public const float InnerFade = 10f;

	// Token: 0x04002838 RID: 10296
	public const float OuterFade = 20f;

	// Token: 0x04002839 RID: 10297
	public const float RandomScale = 0.75f;

	// Token: 0x0400283A RID: 10298
	public const float MeshOffset = -0.5f;

	// Token: 0x0400283B RID: 10299
	public const float TerrainOffset = -1.5f;

	// Token: 0x060031A2 RID: 12706 RVA: 0x0012E044 File Offset: 0x0012C244
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rivers.Clear();
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			return;
		}
		List<PathList> list = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<Vector3> list2 = new List<Vector3>();
		for (float num = TerrainMeta.Position.z; num < TerrainMeta.Position.z + TerrainMeta.Size.z; num += 50f)
		{
			for (float num2 = TerrainMeta.Position.x; num2 < TerrainMeta.Position.x + TerrainMeta.Size.x; num2 += 50f)
			{
				Vector3 vector = new Vector3(num2, 0f, num);
				float num3 = vector.y = heightMap.GetHeight(vector);
				if (vector.y > 5f)
				{
					Vector3 normal = heightMap.GetNormal(vector);
					if (normal.y > 0.01f)
					{
						Vector2 normalized = new Vector2(normal.x, normal.z).normalized;
						list2.Add(vector);
						float radius = 18f;
						int num4 = 18;
						int i = 0;
						while (i < 10000)
						{
							vector.x += normalized.x;
							vector.z += normalized.y;
							if (heightMap.GetSlope(vector) > 30f)
							{
								break;
							}
							float height = heightMap.GetHeight(vector);
							if (height > num3 + 10f)
							{
								break;
							}
							float num5 = Mathf.Min(height, num3);
							vector.y = Mathf.Lerp(vector.y, num5, 0.15f);
							int topology = topologyMap.GetTopology(vector, radius);
							int topology2 = topologyMap.GetTopology(vector);
							int num6 = 3742724;
							int num7 = 128;
							if ((topology & num6) != 0)
							{
								list2.Add(vector);
								break;
							}
							if ((topology2 & num7) != 0 && --num4 <= 0)
							{
								list2.Add(vector);
								if (list2.Count >= 25)
								{
									int num8 = TerrainMeta.Path.Rivers.Count + list.Count;
									list.Add(new PathList("River " + num8, list2.ToArray())
									{
										Spline = true,
										Width = 36f,
										InnerPadding = 1f,
										OuterPadding = 1f,
										InnerFade = 10f,
										OuterFade = 20f,
										RandomScale = 0.75f,
										MeshOffset = -0.5f,
										TerrainOffset = -1.5f,
										Topology = 16384,
										Splat = 64,
										Start = true,
										End = true
									});
									break;
								}
								break;
							}
							else
							{
								if (i % 12 == 0)
								{
									list2.Add(vector);
								}
								normal = heightMap.GetNormal(vector);
								normalized = new Vector2(normalized.x + 0.15f * normal.x, normalized.y + 0.15f * normal.z).normalized;
								num3 = num5;
								i++;
							}
						}
						list2.Clear();
					}
				}
			}
		}
		list.Sort((PathList a, PathList b) => b.Path.Points.Length.CompareTo(a.Path.Points.Length));
		int num9 = Mathf.RoundToInt(10f * TerrainMeta.Size.x * TerrainMeta.Size.z * 1E-06f);
		int num10 = Mathf.NextPowerOfTwo((int)(World.Size / 36f));
		bool[,] array = new bool[num10, num10];
		for (int j = 0; j < list.Count; j++)
		{
			if (j >= num9)
			{
				list.RemoveUnordered(j--);
			}
			else
			{
				PathList pathList = list[j];
				bool flag = false;
				for (int k = 0; k < j; k++)
				{
					if (Vector3.Distance(list[k].Path.GetStartPoint(), pathList.Path.GetStartPoint()) < 100f)
					{
						list.RemoveUnordered(j--);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					int num11 = -1;
					int num12 = -1;
					for (int l = 0; l < pathList.Path.Points.Length; l++)
					{
						Vector3 vector2 = pathList.Path.Points[l];
						int num13 = Mathf.Clamp((int)(TerrainMeta.NormalizeX(vector2.x) * (float)num10), 0, num10 - 1);
						int num14 = Mathf.Clamp((int)(TerrainMeta.NormalizeZ(vector2.z) * (float)num10), 0, num10 - 1);
						if (num11 != num13 || num12 != num14)
						{
							if (array[num14, num13])
							{
								list.RemoveUnordered(j--);
								break;
							}
							if (num11 != num13 && num12 != num14)
							{
								if (num11 != -1)
								{
									array[num14, num11] = true;
								}
								if (num12 != -1)
								{
									array[num12, num13] = true;
								}
								num11 = num13;
								num12 = num14;
								array[num14, num13] = true;
							}
							else
							{
								num11 = num13;
								num12 = num14;
								array[num14, num13] = true;
							}
						}
					}
				}
			}
		}
		for (int m = 0; m < list.Count; m++)
		{
			list[m].Name = "River " + (TerrainMeta.Path.Rivers.Count + m);
		}
		foreach (PathList pathList2 in list)
		{
			pathList2.Path.Smoothen(4, new Vector3(1f, 0f, 1f), null);
			pathList2.Path.Smoothen(8, new Vector3(0f, 1f, 0f), null);
			pathList2.Path.Resample(7.5f);
			pathList2.Path.RecalculateTangents();
		}
		TerrainMeta.Path.Rivers.AddRange(list);
	}
}
