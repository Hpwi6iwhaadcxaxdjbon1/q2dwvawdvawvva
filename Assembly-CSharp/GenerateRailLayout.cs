﻿using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public class GenerateRailLayout : ProceduralComponent
{
	// Token: 0x0400280A RID: 10250
	public const float Width = 4f;

	// Token: 0x0400280B RID: 10251
	public const float InnerPadding = 1f;

	// Token: 0x0400280C RID: 10252
	public const float OuterPadding = 1f;

	// Token: 0x0400280D RID: 10253
	public const float InnerFade = 1f;

	// Token: 0x0400280E RID: 10254
	public const float OuterFade = 32f;

	// Token: 0x0400280F RID: 10255
	public const float RandomScale = 1f;

	// Token: 0x04002810 RID: 10256
	public const float MeshOffset = 0f;

	// Token: 0x04002811 RID: 10257
	public const float TerrainOffset = -0.125f;

	// Token: 0x04002812 RID: 10258
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x04002813 RID: 10259
	private const int MaxDepth = 250000;

	// Token: 0x0600318C RID: 12684 RVA: 0x0012C2B4 File Offset: 0x0012A4B4
	private PathList CreateSegment(int number, Vector3[] points)
	{
		return new PathList("Rail " + number, points)
		{
			Spline = true,
			Width = 4f,
			InnerPadding = 1f,
			OuterPadding = 1f,
			InnerFade = 1f,
			OuterFade = 32f,
			RandomScale = 1f,
			MeshOffset = 0f,
			TerrainOffset = -0.125f,
			Topology = 524288,
			Splat = 128,
			Hierarchy = 1
		};
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x0012C354 File Offset: 0x0012A554
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		new List<GenerateRailLayout.PathSegment>();
		List<PathFinder.Node> list2 = new List<PathFinder.Node>();
		List<PathFinder.Point> list3 = new List<PathFinder.Point>();
		List<PathFinder.Point> list4 = new List<PathFinder.Point>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector3> list6 = new List<Vector3>();
		List<Vector3> list7 = new List<Vector3>();
		List<Vector3> list8 = new List<Vector3>();
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			if (pathList.ProcgenStartNode != null && pathList.ProcgenEndNode != null)
			{
				for (PathFinder.Node node = pathList.ProcgenStartNode; node != null; node = node.next)
				{
					list2.Add(node);
				}
			}
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			pathFinder.PushPoint = monumentInfo.GetPathFinderPoint(length);
			pathFinder.PushRadius = monumentInfo.GetPathFinderRadius(length);
			pathFinder.PushDistance = 60;
			pathFinder.PushMultiplier = 1;
			foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetComponentsInChildren<TerrainPathConnect>(true))
			{
				list5.Clear();
				list6.Clear();
				list7.Clear();
				list8.Clear();
				if (terrainPathConnect.Type == InfrastructureType.Rail)
				{
					Vector3 vector = terrainPathConnect.transform.position;
					Vector3 b = terrainPathConnect.transform.forward * 7.5f;
					PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length, vector);
					int num = 0;
					while (num < 8 || !pathFinder.IsWalkable(pathFinderPoint))
					{
						list5.Add(vector);
						vector += b;
						pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length, vector);
						num++;
					}
					list3.Clear();
					list3.Add(pathFinderPoint);
					list4.Clear();
					foreach (PathFinder.Node node2 in list2)
					{
						if (pathFinder.Distance(node2.point, pathFinder.PushPoint) >= (float)(pathFinder.PushRadius + pathFinder.PushDistance / 2))
						{
							list4.Add(node2.point);
						}
					}
					PathFinder.Node node3 = pathFinder.FindPathDirected(list3, list4, 250000);
					if (node3 != null)
					{
						PathFinder.Node node4 = null;
						PathFinder.Node node5 = null;
						PathFinder.Node node6 = node3;
						while (node6 != null && node6.next != null)
						{
							if (node6 == node3.next)
							{
								node4 = node6;
							}
							if (node6.next.next == null)
							{
								node5 = node6;
							}
							node6 = node6.next;
						}
						if (node4 != null && node5 != null)
						{
							node5.next = null;
							for (PathFinder.Node node7 = node4; node7 != null; node7 = node7.next)
							{
								float normX = ((float)node7.point.x + 0.5f) / (float)length;
								float normZ = ((float)node7.point.y + 0.5f) / (float)length;
								float x = TerrainMeta.DenormalizeX(normX);
								float z = TerrainMeta.DenormalizeZ(normZ);
								float y = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
								list6.Add(new Vector3(x, y, z));
							}
							if (list6.Count != 0)
							{
								Vector3 b2 = list5[0];
								Vector3 a = list6[list6.Count - 1];
								Vector3 normalized = (a - b2).normalized;
								PathList pathList2 = null;
								float num2 = float.MaxValue;
								int num3 = -1;
								foreach (PathList pathList3 in TerrainMeta.Path.Rails)
								{
									Vector3[] points = pathList3.Path.Points;
									for (int j = 0; j < points.Length; j++)
									{
										float num4 = Vector3.Distance(a, points[j]);
										if (num4 < num2)
										{
											num2 = num4;
											pathList2 = pathList3;
											num3 = j;
										}
									}
								}
								Vector3[] points2 = pathList2.Path.Points;
								Vector3 vector2 = pathList2.Path.Tangents[num3];
								int num5 = (Vector3.Angle(vector2, normalized) < Vector3.Angle(-vector2, normalized)) ? 1 : -1;
								Vector3 to = Vector3.Normalize(list6[list6.Count - 1] - list6[Mathf.Max(0, list6.Count - 1 - 16)]);
								Vector3 vector3 = Vector3.Normalize(points2[(num3 + num5 * 8 * 2 + points2.Length) % points2.Length] - points2[num3]);
								float num6 = -Vector3.SignedAngle(vector3, to, Vector3.up);
								Vector3 a2 = GenerateRailLayout.rot90 * vector3;
								if (num6 < 0f)
								{
									a2 = -a2;
								}
								for (int k = 0; k < 8; k++)
								{
									float t = Mathf.InverseLerp(7f, 0f, (float)k);
									float d = Mathf.SmoothStep(0f, 2f, t) * 4f;
									list7.Add(points2[(num3 + num5 * k + points2.Length) % points2.Length] + a2 * d);
								}
								list8.AddRange(list5);
								list8.AddRange(list6);
								list8.AddRange(list7);
								if (list8.Count >= 2)
								{
									int number = TerrainMeta.Path.Rails.Count + list.Count;
									PathList pathList4 = this.CreateSegment(number, list8.ToArray());
									pathList4.Start = true;
									pathList4.End = false;
									pathList4.ProcgenStartNode = node4;
									pathList4.ProcgenEndNode = node5;
									list.Add(pathList4);
								}
							}
						}
					}
				}
			}
		}
		using (List<PathList>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathList rail = enumerator.Current;
				Func<int, float> filter = delegate(int i)
				{
					float a3 = Mathf.InverseLerp(0f, 8f, (float)i);
					float b3 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
					return Mathf.SmoothStep(0f, 1f, Mathf.Min(a3, b3));
				};
				rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
				rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
				rail.Path.Resample(7.5f);
				rail.Path.RecalculateTangents();
				rail.AdjustPlacementMap(20f);
			}
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	// Token: 0x02000DF7 RID: 3575
	private class PathNode
	{
		// Token: 0x04004983 RID: 18819
		public MonumentInfo monument;

		// Token: 0x04004984 RID: 18820
		public TerrainPathConnect target;

		// Token: 0x04004985 RID: 18821
		public PathFinder.Node node;
	}

	// Token: 0x02000DF8 RID: 3576
	private class PathSegment
	{
		// Token: 0x04004986 RID: 18822
		public PathFinder.Node start;

		// Token: 0x04004987 RID: 18823
		public PathFinder.Node end;

		// Token: 0x04004988 RID: 18824
		public TerrainPathConnect origin;

		// Token: 0x04004989 RID: 18825
		public TerrainPathConnect target;
	}
}
