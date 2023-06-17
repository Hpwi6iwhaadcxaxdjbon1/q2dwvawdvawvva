using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public class DungeonBaseInfo : LandmarkInfo
{
	// Token: 0x040026C8 RID: 9928
	internal List<GameObject> Links = new List<GameObject>();

	// Token: 0x040026C9 RID: 9929
	internal List<DungeonBaseFloor> Floors = new List<DungeonBaseFloor>();

	// Token: 0x06002F76 RID: 12150 RVA: 0x0011DCD0 File Offset: 0x0011BED0
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x0011DCF8 File Offset: 0x0011BEF8
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x0011DD20 File Offset: 0x0011BF20
	public void Add(DungeonBaseLink link)
	{
		this.Links.Add(link.gameObject);
		if (link.Type == DungeonBaseLinkType.End)
		{
			return;
		}
		DungeonBaseFloor dungeonBaseFloor = null;
		float num = float.MaxValue;
		for (int i = 0; i < this.Floors.Count; i++)
		{
			DungeonBaseFloor dungeonBaseFloor2 = this.Floors[i];
			float num2 = dungeonBaseFloor2.Distance(link.transform.position);
			if (num2 < 1f && num2 < num)
			{
				dungeonBaseFloor = dungeonBaseFloor2;
				num = num2;
			}
		}
		if (dungeonBaseFloor == null)
		{
			dungeonBaseFloor = new DungeonBaseFloor();
			dungeonBaseFloor.Links.Add(link);
			this.Floors.Add(dungeonBaseFloor);
			this.Floors.Sort((DungeonBaseFloor l, DungeonBaseFloor r) => l.SignedDistance(base.transform.position).CompareTo(r.SignedDistance(base.transform.position)));
			return;
		}
		dungeonBaseFloor.Links.Add(link);
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x0011DDDE File Offset: 0x0011BFDE
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonBaseEntrances.Add(this);
		}
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x0011DE02 File Offset: 0x0011C002
	protected void Start()
	{
		base.transform.SetHierarchyGroup("DungeonBase", true, false);
	}
}
