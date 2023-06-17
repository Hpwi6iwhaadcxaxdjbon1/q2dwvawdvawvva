using System;
using UnityEngine;

// Token: 0x020006B6 RID: 1718
public class GenerateDecorTopology : ProceduralComponent
{
	// Token: 0x040027E6 RID: 10214
	public bool KeepExisting = true;

	// Token: 0x0600316A RID: 12650 RVA: 0x001277C8 File Offset: 0x001259C8
	public override void Process(uint seed)
	{
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int topores = topomap.res;
		Parallel.For(0, topores, delegate(int z)
		{
			for (int i = 0; i < topores; i++)
			{
				if (topomap.GetTopology(i, z, 4194306))
				{
					topomap.AddTopology(i, z, 512);
				}
				else if (!this.KeepExisting)
				{
					topomap.RemoveTopology(i, z, 512);
				}
			}
		});
	}
}
