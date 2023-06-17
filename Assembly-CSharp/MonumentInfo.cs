using System;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public class MonumentInfo : LandmarkInfo, IPrefabPreProcess
{
	// Token: 0x04002225 RID: 8741
	[Header("MonumentInfo")]
	public MonumentType Type = MonumentType.Building;

	// Token: 0x04002226 RID: 8742
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x04002227 RID: 8743
	public int MinWorldSize;

	// Token: 0x04002228 RID: 8744
	public Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04002229 RID: 8745
	public bool HasNavmesh;

	// Token: 0x0400222A RID: 8746
	public bool IsSafeZone;

	// Token: 0x0400222B RID: 8747
	[HideInInspector]
	public bool WantsDungeonLink;

	// Token: 0x0400222C RID: 8748
	[HideInInspector]
	public bool HasDungeonLink;

	// Token: 0x0400222D RID: 8749
	[HideInInspector]
	public DungeonGridInfo DungeonEntrance;

	// Token: 0x0400222E RID: 8750
	private OBB obbBounds;

	// Token: 0x060029EC RID: 10732 RVA: 0x000FFE44 File Offset: 0x000FE044
	protected override void Awake()
	{
		base.Awake();
		this.obbBounds = new OBB(base.transform.position, base.transform.rotation, this.Bounds);
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x000FFE9C File Offset: 0x000FE09C
	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		OBB obb = new OBB(pos, scale, rot, this.Bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = MonumentInfo.TierToMask(this.Tier);
		int num2 = 0;
		if ((num & topology) != 0)
		{
			num2++;
		}
		if ((num & topology2) != 0)
		{
			num2++;
		}
		if ((num & topology3) != 0)
		{
			num2++;
		}
		if ((num & topology4) != 0)
		{
			num2++;
		}
		return num2 >= 3;
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x000FFF90 File Offset: 0x000FE190
	public float Distance(Vector3 position)
	{
		return this.obbBounds.Distance(position);
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x000FFF9E File Offset: 0x000FE19E
	public float SqrDistance(Vector3 position)
	{
		return this.obbBounds.SqrDistance(position);
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000FFFAC File Offset: 0x000FE1AC
	public float Distance(OBB obb)
	{
		return this.obbBounds.Distance(obb);
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000FFFBA File Offset: 0x000FE1BA
	public float SqrDistance(OBB obb)
	{
		return this.obbBounds.SqrDistance(obb);
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000FFFC8 File Offset: 0x000FE1C8
	public bool IsInBounds(Vector3 position)
	{
		return this.obbBounds.Contains(position);
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000FFFD6 File Offset: 0x000FE1D6
	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		return this.obbBounds.ClosestPoint(position);
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000FFFE4 File Offset: 0x000FE1E4
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		Vector3 position = base.transform.position;
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x00100048 File Offset: 0x000FE248
	public int GetPathFinderRadius(int res)
	{
		float a = this.Bounds.extents.x * TerrainMeta.OneOverSize.x;
		float b = this.Bounds.extents.z * TerrainMeta.OneOverSize.z;
		return Mathf.CeilToInt(Mathf.Max(a, b) * (float)res);
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x0010009C File Offset: 0x000FE29C
	protected void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(this.Bounds.center, this.Bounds.size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(this.Bounds.center, this.Bounds.size);
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x0010012B File Offset: 0x000FE32B
	public MonumentNavMesh GetMonumentNavMesh()
	{
		return base.GetComponent<MonumentNavMesh>();
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x00100134 File Offset: 0x000FE334
	public static int TierToMask(MonumentTier tier)
	{
		int num = 0;
		if ((tier & MonumentTier.Tier0) != (MonumentTier)0)
		{
			num |= 67108864;
		}
		if ((tier & MonumentTier.Tier1) != (MonumentTier)0)
		{
			num |= 134217728;
		}
		if ((tier & MonumentTier.Tier2) != (MonumentTier)0)
		{
			num |= 268435456;
		}
		return num;
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x0010016B File Offset: 0x000FE36B
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.HasDungeonLink = this.DetermineHasDungeonLink();
		this.WantsDungeonLink = this.DetermineWantsDungeonLink();
		this.DungeonEntrance = this.FindDungeonEntrance();
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x00100191 File Offset: 0x000FE391
	private DungeonGridInfo FindDungeonEntrance()
	{
		return base.GetComponentInChildren<DungeonGridInfo>();
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x00100199 File Offset: 0x000FE399
	private bool DetermineHasDungeonLink()
	{
		return base.GetComponentInChildren<DungeonGridLink>() != null;
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x001001A8 File Offset: 0x000FE3A8
	private bool DetermineWantsDungeonLink()
	{
		return this.Type != MonumentType.WaterWell && (this.Type != MonumentType.Building || !this.displayPhrase.token.StartsWith("mining_quarry")) && (this.Type != MonumentType.Radtown || !this.displayPhrase.token.StartsWith("swamp"));
	}
}
