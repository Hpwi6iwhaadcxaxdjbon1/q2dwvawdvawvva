using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

// Token: 0x020006DD RID: 1757
public class PlacePowerlineObjects : ProceduralComponent
{
	// Token: 0x040028B2 RID: 10418
	public PathList.BasicObject[] Start;

	// Token: 0x040028B3 RID: 10419
	public PathList.BasicObject[] End;

	// Token: 0x040028B4 RID: 10420
	public PathList.SideObject[] Side;

	// Token: 0x040028B5 RID: 10421
	[FormerlySerializedAs("PowerlineObjects")]
	public PathList.PathObject[] Path;

	// Token: 0x060031E8 RID: 12776 RVA: 0x00133AEC File Offset: 0x00131CEC
	public override void Process(uint seed)
	{
		List<PathList> powerlines = TerrainMeta.Path.Powerlines;
		if (World.Networked)
		{
			using (List<PathList>.Enumerator enumerator = powerlines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PathList pathList = enumerator.Current;
					World.Spawn(pathList.Name, "assets/bundled/prefabs/autospawn/");
				}
				return;
			}
		}
		foreach (PathList pathList2 in powerlines)
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
