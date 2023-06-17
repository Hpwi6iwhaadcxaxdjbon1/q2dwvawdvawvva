using System;
using UnityEngine;

// Token: 0x02000666 RID: 1638
[RequireComponent(typeof(DungeonBaseLink))]
public class DungeonBaseLandmarkInfo : LandmarkInfo
{
	// Token: 0x040026CB RID: 9931
	private DungeonBaseLink baseLink;

	// Token: 0x040026CC RID: 9932
	private MapLayer? layer;

	// Token: 0x06002F80 RID: 12160 RVA: 0x0011DECB File Offset: 0x0011C0CB
	protected override void Awake()
	{
		base.Awake();
		this.baseLink = base.GetComponent<DungeonBaseLink>();
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002F81 RID: 12161 RVA: 0x0011DEE0 File Offset: 0x0011C0E0
	public override MapLayer MapLayer
	{
		get
		{
			if (this.layer != null)
			{
				return this.layer.Value;
			}
			DungeonBaseInfo dungeonBaseInfo = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, this.baseLink.transform.position);
			if (dungeonBaseInfo == null)
			{
				Debug.LogWarning("Couldn't determine which underwater lab a DungeonBaseLandmarkInfo belongs to", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
				return this.layer.Value;
			}
			int num = -1;
			for (int i = 0; i < dungeonBaseInfo.Floors.Count; i++)
			{
				if (dungeonBaseInfo.Floors[i].Links.Contains(this.baseLink))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				this.layer = new MapLayer?(MapLayer.Underwater1 + num);
			}
			else
			{
				Debug.LogWarning("Couldn't determine the floor of a DungeonBaseLandmarkInfo", this);
				this.shouldDisplayOnMap = false;
				this.layer = new MapLayer?(MapLayer.Overworld);
			}
			return this.layer.Value;
		}
	}
}
