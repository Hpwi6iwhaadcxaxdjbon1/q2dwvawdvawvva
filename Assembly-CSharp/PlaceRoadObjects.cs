using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006DF RID: 1759
public class PlaceRoadObjects : ProceduralComponent
{
	// Token: 0x040028BA RID: 10426
	public PathList.BasicObject[] Start;

	// Token: 0x040028BB RID: 10427
	public PathList.BasicObject[] End;

	// Token: 0x040028BC RID: 10428
	[FormerlySerializedAs("RoadsideObjects")]
	public PathList.SideObject[] Side;

	// Token: 0x040028BD RID: 10429
	[FormerlySerializedAs("RoadObjects")]
	public PathList.PathObject[] Path;

	// Token: 0x060031EC RID: 12780 RVA: 0x00133E64 File Offset: 0x00132064
	public override void Process(uint seed)
	{
		List<PathList> roads = TerrainMeta.Path.Roads;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = roads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in roads)
		{
			if (pathList2.Hierarchy < 2)
			{
				foreach (PathList.BasicObject obj in this.Start)
				{
					pathList2.TrimStart(obj);
				}
				foreach (PathList.BasicObject obj2 in this.End)
				{
					pathList2.TrimEnd(obj2);
				}
				foreach (PathList.BasicObject obj3 in this.Start)
				{
					pathList2.SpawnStart(ref seed, obj3);
				}
				foreach (PathList.BasicObject obj4 in this.End)
				{
					pathList2.SpawnEnd(ref seed, obj4);
				}
				foreach (PathList.PathObject obj5 in this.Path)
				{
					pathList2.SpawnAlong(ref seed, obj5);
				}
				foreach (PathList.SideObject obj6 in this.Side)
				{
					pathList2.SpawnSide(ref seed, obj6);
				}
				pathList2.ResetTrims();
			}
		}
	}
}
