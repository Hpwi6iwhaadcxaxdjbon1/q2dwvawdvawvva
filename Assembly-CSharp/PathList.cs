using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class PathList
{
	// Token: 0x04002663 RID: 9827
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x04002664 RID: 9828
	private static Quaternion rot180 = Quaternion.Euler(0f, 180f, 0f);

	// Token: 0x04002665 RID: 9829
	private static Quaternion rot270 = Quaternion.Euler(0f, 270f, 0f);

	// Token: 0x04002666 RID: 9830
	public string Name;

	// Token: 0x04002667 RID: 9831
	public PathInterpolator Path;

	// Token: 0x04002668 RID: 9832
	public bool Spline;

	// Token: 0x04002669 RID: 9833
	public bool Start;

	// Token: 0x0400266A RID: 9834
	public bool End;

	// Token: 0x0400266B RID: 9835
	public float Width;

	// Token: 0x0400266C RID: 9836
	public float InnerPadding;

	// Token: 0x0400266D RID: 9837
	public float OuterPadding;

	// Token: 0x0400266E RID: 9838
	public float InnerFade;

	// Token: 0x0400266F RID: 9839
	public float OuterFade;

	// Token: 0x04002670 RID: 9840
	public float RandomScale;

	// Token: 0x04002671 RID: 9841
	public float MeshOffset;

	// Token: 0x04002672 RID: 9842
	public float TerrainOffset;

	// Token: 0x04002673 RID: 9843
	public int Topology;

	// Token: 0x04002674 RID: 9844
	public int Splat;

	// Token: 0x04002675 RID: 9845
	public int Hierarchy;

	// Token: 0x04002676 RID: 9846
	public PathFinder.Node ProcgenStartNode;

	// Token: 0x04002677 RID: 9847
	public PathFinder.Node ProcgenEndNode;

	// Token: 0x04002678 RID: 9848
	public const float StepSize = 1f;

	// Token: 0x04002679 RID: 9849
	private static float[] placements = new float[]
	{
		0f,
		-1f,
		1f
	};

	// Token: 0x06002ED3 RID: 11987 RVA: 0x00119AA9 File Offset: 0x00117CA9
	public PathList(string name, Vector3[] points)
	{
		this.Name = name;
		this.Path = new PathInterpolator(points);
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x00119AC4 File Offset: 0x00117CC4
	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		if (positions.Count < 2)
		{
			return;
		}
		List<Prefab> list = Pool.GetList<Prefab>();
		for (int i = 0; i < positions.Count; i++)
		{
			int index = Mathf.Max(i - 1, 0);
			int index2 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 position = positions[i];
			Quaternion rotation = Quaternion.LookRotation((positions[index2] - positions[index]).XZ3D());
			Prefab prefab;
			this.SpawnObject(ref seed, prefabs, position, rotation, list, out prefab, positions.Count, i, filter);
			if (prefab != null)
			{
				list.Add(prefab);
			}
		}
		Pool.FreeList<Prefab>(ref list);
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x00119B64 File Offset: 0x00117D64
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		Prefab random = prefabs.GetRandom(ref seed);
		Vector3 position2 = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position2, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref position2, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, position2, quaternion, localScale);
		return true;
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x00119BBC File Offset: 0x00117DBC
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, List<Prefab> previousSpawns, out Prefab spawned, int pathLength, int index, SpawnFilter filter = null)
	{
		spawned = null;
		Prefab random = prefabs.GetRandom(ref seed);
		random.ApplySequenceReplacement(previousSpawns, ref random, prefabs, pathLength, index);
		Vector3 position2 = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position2, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref position2, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, position2, quaternion, localScale);
		spawned = random;
		return true;
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x00119C2C File Offset: 0x00117E2C
	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		foreach (Prefab prefab in prefabs)
		{
			Vector3 vector = position;
			Vector3 localScale = prefab.Object.transform.localScale;
			if (!prefab.ApplyTerrainAnchors(ref vector, rotation, localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x00119C70 File Offset: 0x00117E70
	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.SpawnObject(ref seed, prefabs, vector, rotation, filter))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x00119D60 File Offset: 0x00117F60
	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.CheckObjects(prefabs, vector, rotation, filter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x00119E50 File Offset: 0x00118050
	public void SpawnSide(ref uint seed, PathList.SideObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		PathList.Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float num = this.Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] array2 = new float[]
		{
			-num,
			num
		};
		int num2 = 0;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num3 = distance * 0.25f;
		float num4 = distance * 0.5f;
		float num5 = this.Path.StartOffset + num4;
		float num6 = this.Path.Length - this.Path.EndOffset - num4;
		for (float num7 = num5; num7 <= num6; num7 += num3)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num7) : this.Path.GetPoint(num7);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num7);
				Vector3 vector2 = PathList.rot90 * tangent;
				for (int i = 0; i < array2.Length; i++)
				{
					int num8 = (num2 + i) % array2.Length;
					if ((side != PathList.Side.Left || num8 == 0) && (side != PathList.Side.Right || num8 == 1))
					{
						float num9 = array2[num8];
						Vector3 vector3 = vector;
						vector3.x += vector2.x * num9;
						vector3.z += vector2.z * num9;
						float normX = TerrainMeta.NormalizeX(vector3.x);
						float normZ = TerrainMeta.NormalizeZ(vector3.z);
						if (filter.GetFactor(normX, normZ, true) >= SeedRandom.Value(ref seed))
						{
							if (density >= SeedRandom.Value(ref seed))
							{
								vector3.y = heightMap.GetHeight(normX, normZ);
								if (obj.Alignment == PathList.Alignment.None)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(Vector3.zero), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Forward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(tangent * num9), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Inward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(tangent * num9) * PathList.rot270, filter))
									{
										goto IL_284;
									}
								}
								else
								{
									list.Add(vector3);
								}
							}
							num2 = num8;
							b = vector;
							if (side == PathList.Side.Any)
							{
								break;
							}
						}
					}
					IL_284:;
				}
			}
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x0011A118 File Offset: 0x00118318
	public void SpawnAlong(ref uint seed, PathList.PathObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num = distance * 0.25f;
		float num2 = distance * 0.5f;
		float num3 = this.Path.StartOffset + num2;
		float num4 = this.Path.Length - this.Path.EndOffset - num2;
		for (float num5 = num3; num5 <= num4; num5 += num)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num5) : this.Path.GetPoint(num5);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num5);
				Vector3 forward = PathList.rot90 * tangent;
				Vector3 vector2 = vector;
				vector2.x += SeedRandom.Range(ref seed, -dithering, dithering);
				vector2.z += SeedRandom.Range(ref seed, -dithering, dithering);
				float normX = TerrainMeta.NormalizeX(vector2.x);
				float normZ = TerrainMeta.NormalizeZ(vector2.z);
				if (filter.GetFactor(normX, normZ, true) >= SeedRandom.Value(ref seed))
				{
					if (density >= SeedRandom.Value(ref seed))
					{
						vector2.y = heightMap.GetHeight(normX, normZ);
						if (obj.Alignment == PathList.Alignment.None)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.identity, filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Forward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(tangent), filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Inward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(forward), filter))
							{
								goto IL_1FE;
							}
						}
						else
						{
							list.Add(vector2);
						}
					}
					b = vector;
				}
			}
			IL_1FE:;
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x0011A348 File Offset: 0x00118548
	public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 a = this.Path.GetEndPoint() - startPoint;
		float magnitude = a.magnitude;
		Vector3 vector = a / magnitude;
		float num = magnitude / obj.Distance;
		int num2 = Mathf.RoundToInt(num);
		float num3 = 0.5f * (num - (float)num2);
		Vector3 vector2 = obj.Distance * vector;
		Vector3 vector3 = startPoint + (0.5f + num3) * vector2;
		Quaternion rotation = Quaternion.LookRotation(vector);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num2; i++)
		{
			float num4 = Mathf.Max(heightMap.GetHeight(vector3), waterMap.GetHeight(vector3)) - 1f;
			if (vector3.y > num4)
			{
				this.SpawnObject(ref seed, array, vector3, rotation, null);
			}
			vector3 += vector2;
		}
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x0011A47C File Offset: 0x0011867C
	public void SpawnStart(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		this.SpawnObject(ref seed, array, startPoint, startTangent, obj);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x0011A4FC File Offset: 0x001186FC
	public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 dir = -this.Path.GetEndTangent();
		this.SpawnObject(ref seed, array, endPoint, dir, obj);
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x0011A580 File Offset: 0x00118780
	public void TrimStart(PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MinIndex + i];
			Vector3 dir = tangents[this.Path.MinIndex + i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MinIndex += i;
				return;
			}
		}
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x0011A658 File Offset: 0x00118858
	public void TrimEnd(PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MaxIndex - i];
			Vector3 dir = -tangents[this.Path.MaxIndex - i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MaxIndex -= i;
				return;
			}
		}
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x0011A734 File Offset: 0x00118934
	public void TrimTopology(int topology)
	{
		Vector3[] points = this.Path.Points;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 worldPos = points[this.Path.MinIndex + i];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos, topology))
			{
				this.Path.MinIndex += i;
				break;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 worldPos2 = points[this.Path.MaxIndex - j];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos2, topology))
			{
				this.Path.MaxIndex -= j;
				return;
			}
		}
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x0011A7E0 File Offset: 0x001189E0
	public void ResetTrims()
	{
		this.Path.MinIndex = this.Path.DefaultMinIndex;
		this.Path.MaxIndex = this.Path.DefaultMaxIndex;
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x0011A810 File Offset: 0x00118A10
	public void AdjustTerrainHeight(float intensity = 1f, float fade = 1f)
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float outerFade = this.OuterFade * fade;
		float innerFade = this.InnerFade;
		float offset01 = this.TerrainOffset * TerrainMeta.OneOverSize.y;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 normalized = startTangent.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 vector = startPoint;
		Vector3 vector2 = startTangent;
		Line prev_line = new Line(startPoint, startPoint + startTangent * num);
		Vector3 v = startPoint - a * (num2 + outerPadding + outerFade);
		Vector3 v2 = startPoint + a * (num2 + outerPadding + outerFade);
		Vector3 vector3 = vector;
		Vector3 v3 = vector2;
		Line cur_line = prev_line;
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector4 = this.Spline ? this.Path.GetPointCubicHermite(num4 + num) : this.Path.GetPoint(num4 + num);
			Vector3 tangent = this.Path.GetTangent(num4 + num);
			Line next_line = new Line(vector4, vector4 + tangent * num);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector3.x, vector3.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector3).Magnitude2D();
				float b = (endPoint - vector3).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			normalized = v3.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Vector3 vector5 = vector3 - a * (radius + outerPadding + outerFade);
			Vector3 vector6 = vector3 + a * (radius + outerPadding + outerFade);
			float yn = TerrainMeta.NormalizeY((vector3.y + vector.y) * 0.5f);
			heightmap.ForEach(v, v2, vector5, vector6, delegate(int x, int z)
			{
				float x2 = heightmap.Coordinate(x);
				float z2 = heightmap.Coordinate(z);
				Vector3 vector7 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 vector8 = prev_line.ClosestPoint2D(vector7);
				Vector3 vector9 = cur_line.ClosestPoint2D(vector7);
				Vector3 vector10 = next_line.ClosestPoint2D(vector7);
				float num5 = (vector7 - vector8).Magnitude2D();
				float num6 = (vector7 - vector9).Magnitude2D();
				float num7 = (vector7 - vector10).Magnitude2D();
				float value = num6;
				Vector3 vector11 = vector9;
				if (num6 > num5 || num6 > num7)
				{
					if (num5 <= num7)
					{
						value = num5;
						vector11 = vector8;
					}
					else
					{
						value = num7;
						vector11 = vector10;
					}
				}
				float num8 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, value);
				float t = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, value);
				float num9 = TerrainMeta.NormalizeY(vector11.y);
				heightmap.SetHeight(x, z, num9 + Mathf.SmoothStep(0f, offset01, t), intensity * opacity * num8);
			});
			vector = vector3;
			v = vector5;
			v2 = vector6;
			prev_line = cur_line;
			vector3 = vector4;
			v3 = tangent;
			cur_line = next_line;
		}
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x0011AB84 File Offset: 0x00118D84
	public void AdjustTerrainTexture()
	{
		if (this.Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatmap = TerrainMeta.SplatMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector2).Magnitude2D();
				float b = (endPoint - vector2).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			vector = this.Path.GetTangent(num4);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			splatmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = splatmap.Coordinate(x);
				float z2 = splatmap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = (vector5 - b2).Magnitude2D();
				float num5 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value);
				splatmap.SetSplat(x, z, this.Splat, num5 * opacity);
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x0011AE10 File Offset: 0x00119010
	public void AdjustTerrainTopology()
	{
		if (this.Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector2).Magnitude2D();
				float b = (endPoint - vector2).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			vector = this.Path.GetTangent(num4);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			topomap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = topomap.Coordinate(x);
				float z2 = topomap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = (vector5 - b2).Magnitude2D();
				if (Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value) * opacity > 0.3f)
				{
					topomap.AddTopology(x, z, this.Topology);
				}
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x0011B09C File Offset: 0x0011929C
	public void AdjustPlacementMap(float width)
	{
		TerrainPlacementMap placementmap = TerrainMeta.PlacementMap;
		float num = 1f;
		float radius = width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * radius;
		Vector3 v2 = startPoint + a * radius;
		float num2 = this.Path.Length + num;
		for (float num3 = 0f; num3 < num2; num3 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num3) : this.Path.GetPoint(num3);
			vector = this.Path.GetTangent(num3);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * radius;
			Vector3 vector4 = vector2 + a * radius;
			float yn = TerrainMeta.NormalizeY(vector2.y);
			placementmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = placementmap.Coordinate(x);
				float z2 = placementmap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b = ray.ClosestPoint(vector5);
				if ((vector5 - b).Magnitude2D() <= radius)
				{
					placementmap.SetBlocked(x, z);
				}
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x0011B248 File Offset: 0x00119448
	public List<PathList.MeshObject> CreateMesh(Mesh[] meshes, float normalSmoothing, bool snapToTerrain, bool snapStartToTerrain, bool snapEndToTerrain)
	{
		MeshCache.Data[] array = new MeshCache.Data[meshes.Length];
		MeshData[] array2 = new MeshData[meshes.Length];
		for (int i = 0; i < meshes.Length; i++)
		{
			array[i] = MeshCache.Get(meshes[i]);
			array2[i] = new MeshData();
		}
		MeshData[] array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].AllocMinimal();
		}
		Bounds bounds = meshes[meshes.Length - 1].bounds;
		Vector3 min = bounds.min;
		Vector3 size = bounds.size;
		float num = this.Width / bounds.size.x;
		List<PathList.MeshObject> list = new List<PathList.MeshObject>();
		int num2 = (int)(this.Path.Length / (num * bounds.size.z));
		int num3 = 5;
		float num4 = this.Path.Length / (float)num2;
		float randomScale = this.RandomScale;
		float meshOffset = this.MeshOffset;
		float num5 = this.Width * 0.5f;
		int num6 = array[0].vertices.Length;
		int num7 = array[0].triangles.Length;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int k = 0; k < num2; k += num3)
		{
			float distance = (float)k * num4 + 0.5f * (float)num3 * num4;
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
			int num8 = 0;
			while (num8 < num3 && k + num8 < num2)
			{
				float num9 = (float)(k + num8) * num4;
				for (int l = 0; l < meshes.Length; l++)
				{
					MeshCache.Data data = array[l];
					MeshData meshData = array2[l];
					int count = meshData.vertices.Count;
					for (int m = 0; m < data.vertices.Length; m++)
					{
						Vector2 item = data.uv[m];
						Vector3 vector2 = data.vertices[m];
						Vector3 vector3 = data.normals[m];
						Vector4 vector4 = data.tangents[m];
						float t = (vector2.x - min.x) / size.x;
						float num10 = vector2.y - min.y;
						float num11 = (vector2.z - min.z) / size.z;
						float num12 = num9 + num11 * num4;
						Vector3 vector5 = this.Spline ? this.Path.GetPointCubicHermite(num12) : this.Path.GetPoint(num12);
						Vector3 tangent = this.Path.GetTangent(num12);
						Vector3 normalized = tangent.XZ3D().normalized;
						Vector3 vector6 = PathList.rot90 * normalized;
						Vector3 vector7 = Vector3.Cross(tangent, vector6);
						Quaternion rotation = Quaternion.LookRotation(normalized, vector7);
						float d = Mathf.Lerp(num5, num5 * randomScale, Noise.Billow(vector5.x, vector5.z, 2, 0.005f, 1f, 2f, 0.5f));
						Vector3 vector8 = vector5 - vector6 * d;
						Vector3 vector9 = vector5 + vector6 * d;
						if (snapToTerrain)
						{
							vector8.y = heightMap.GetHeight(vector8);
							vector9.y = heightMap.GetHeight(vector9);
						}
						vector8 += vector7 * meshOffset;
						vector9 += vector7 * meshOffset;
						vector2 = Vector3.Lerp(vector8, vector9, t);
						if ((snapStartToTerrain && num12 < 0.1f) || (snapEndToTerrain && num12 > this.Path.Length - 0.1f))
						{
							vector2.y = heightMap.GetHeight(vector2);
						}
						else
						{
							vector2.y += num10;
						}
						vector2 -= vector;
						vector3 = rotation * vector3;
						vector4 = rotation * vector4;
						if (normalSmoothing > 0f)
						{
							vector3 = Vector3.Slerp(vector3, Vector3.up, normalSmoothing);
						}
						meshData.vertices.Add(vector2);
						meshData.normals.Add(vector3);
						meshData.tangents.Add(vector4);
						meshData.uv.Add(item);
					}
					for (int n = 0; n < data.triangles.Length; n++)
					{
						int num13 = data.triangles[n];
						meshData.triangles.Add(count + num13);
					}
				}
				num8++;
			}
			list.Add(new PathList.MeshObject(vector, array2));
			array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j].Clear();
			}
		}
		array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Free();
		}
		return list;
	}

	// Token: 0x02000D9F RID: 3487
	public enum Side
	{
		// Token: 0x04004847 RID: 18503
		Both,
		// Token: 0x04004848 RID: 18504
		Left,
		// Token: 0x04004849 RID: 18505
		Right,
		// Token: 0x0400484A RID: 18506
		Any
	}

	// Token: 0x02000DA0 RID: 3488
	public enum Placement
	{
		// Token: 0x0400484C RID: 18508
		Center,
		// Token: 0x0400484D RID: 18509
		Side
	}

	// Token: 0x02000DA1 RID: 3489
	public enum Alignment
	{
		// Token: 0x0400484F RID: 18511
		None,
		// Token: 0x04004850 RID: 18512
		Neighbor,
		// Token: 0x04004851 RID: 18513
		Forward,
		// Token: 0x04004852 RID: 18514
		Inward
	}

	// Token: 0x02000DA2 RID: 3490
	[Serializable]
	public class BasicObject
	{
		// Token: 0x04004853 RID: 18515
		public string Folder;

		// Token: 0x04004854 RID: 18516
		public SpawnFilter Filter;

		// Token: 0x04004855 RID: 18517
		public PathList.Placement Placement;

		// Token: 0x04004856 RID: 18518
		public bool AlignToNormal = true;

		// Token: 0x04004857 RID: 18519
		public bool HeightToTerrain = true;

		// Token: 0x04004858 RID: 18520
		public float Offset;
	}

	// Token: 0x02000DA3 RID: 3491
	[Serializable]
	public class SideObject
	{
		// Token: 0x04004859 RID: 18521
		public string Folder;

		// Token: 0x0400485A RID: 18522
		public SpawnFilter Filter;

		// Token: 0x0400485B RID: 18523
		public PathList.Side Side;

		// Token: 0x0400485C RID: 18524
		public PathList.Alignment Alignment;

		// Token: 0x0400485D RID: 18525
		public float Density = 1f;

		// Token: 0x0400485E RID: 18526
		public float Distance = 25f;

		// Token: 0x0400485F RID: 18527
		public float Offset = 2f;
	}

	// Token: 0x02000DA4 RID: 3492
	[Serializable]
	public class PathObject
	{
		// Token: 0x04004860 RID: 18528
		public string Folder;

		// Token: 0x04004861 RID: 18529
		public SpawnFilter Filter;

		// Token: 0x04004862 RID: 18530
		public PathList.Alignment Alignment;

		// Token: 0x04004863 RID: 18531
		public float Density = 1f;

		// Token: 0x04004864 RID: 18532
		public float Distance = 5f;

		// Token: 0x04004865 RID: 18533
		public float Dithering = 5f;
	}

	// Token: 0x02000DA5 RID: 3493
	[Serializable]
	public class BridgeObject
	{
		// Token: 0x04004866 RID: 18534
		public string Folder;

		// Token: 0x04004867 RID: 18535
		public float Distance = 10f;
	}

	// Token: 0x02000DA6 RID: 3494
	public class MeshObject
	{
		// Token: 0x04004868 RID: 18536
		public Vector3 Position;

		// Token: 0x04004869 RID: 18537
		public Mesh[] Meshes;

		// Token: 0x06005130 RID: 20784 RVA: 0x001ABA40 File Offset: 0x001A9C40
		public MeshObject(Vector3 meshPivot, MeshData[] meshData)
		{
			this.Position = meshPivot;
			this.Meshes = new Mesh[meshData.Length];
			for (int i = 0; i < this.Meshes.Length; i++)
			{
				MeshData meshData2 = meshData[i];
				Mesh mesh = this.Meshes[i] = new Mesh();
				meshData2.Apply(mesh);
			}
		}
	}
}
