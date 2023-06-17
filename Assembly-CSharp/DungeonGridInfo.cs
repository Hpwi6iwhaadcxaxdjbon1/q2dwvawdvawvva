using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class DungeonGridInfo : LandmarkInfo
{
	// Token: 0x040026F9 RID: 9977
	[Header("DungeonGridInfo")]
	public int CellSize = 216;

	// Token: 0x040026FA RID: 9978
	public float LinkHeight = 1.5f;

	// Token: 0x040026FB RID: 9979
	public float LinkRadius = 3f;

	// Token: 0x040026FC RID: 9980
	internal List<GameObject> Links = new List<GameObject>();

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002F8D RID: 12173 RVA: 0x0011E14B File Offset: 0x0011C34B
	public float MinDistance
	{
		get
		{
			return (float)this.CellSize * 2f;
		}
	}

	// Token: 0x06002F8E RID: 12174 RVA: 0x0011E15C File Offset: 0x0011C35C
	public float Distance(Vector3 position)
	{
		return (base.transform.position - position).magnitude;
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x0011E184 File Offset: 0x0011C384
	public float SqrDistance(Vector3 position)
	{
		return (base.transform.position - position).sqrMagnitude;
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x0011E1AC File Offset: 0x0011C3AC
	public bool IsValidSpawnPosition(Vector3 position)
	{
		OBB bounds = base.GetComponentInChildren<DungeonVolume>().GetBounds(position, Quaternion.identity);
		Vector3 b = WorldSpaceGrid.ClosestGridCell(bounds.position, TerrainMeta.Size.x * 2f, (float)this.CellSize);
		Vector3 vector = bounds.position - b;
		return Mathf.Abs(vector.x) > 3f || Mathf.Abs(vector.z) > 3f;
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x0011E220 File Offset: 0x0011C420
	public Vector3 SnapPosition(Vector3 pos)
	{
		pos.x = (float)Mathf.RoundToInt(pos.x / this.LinkRadius) * this.LinkRadius;
		pos.y = (float)Mathf.CeilToInt(pos.y / this.LinkHeight) * this.LinkHeight;
		pos.z = (float)Mathf.RoundToInt(pos.z / this.LinkRadius) * this.LinkRadius;
		return pos;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x0011E291 File Offset: 0x0011C491
	protected override void Awake()
	{
		base.Awake();
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridEntrances.Add(this);
		}
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x0011E2B5 File Offset: 0x0011C4B5
	protected void Start()
	{
		base.transform.SetHierarchyGroup("Dungeon", true, false);
	}
}
