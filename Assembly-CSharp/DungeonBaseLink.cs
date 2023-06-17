using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000667 RID: 1639
public class DungeonBaseLink : MonoBehaviour
{
	// Token: 0x040026CD RID: 9933
	public DungeonBaseLinkType Type;

	// Token: 0x040026CE RID: 9934
	public int Cost = 1;

	// Token: 0x040026CF RID: 9935
	public int MaxFloor = -1;

	// Token: 0x040026D0 RID: 9936
	public int MaxCountLocal = -1;

	// Token: 0x040026D1 RID: 9937
	public int MaxCountGlobal = -1;

	// Token: 0x040026D2 RID: 9938
	[Tooltip("If set to a positive number, all segments with the same MaxCountIdentifier are counted towards MaxCountLocal and MaxCountGlobal")]
	public int MaxCountIdentifier = -1;

	// Token: 0x040026D3 RID: 9939
	internal DungeonBaseInfo Dungeon;

	// Token: 0x040026D4 RID: 9940
	public MeshRenderer[] MapRenderers;

	// Token: 0x040026D5 RID: 9941
	private List<DungeonBaseSocket> sockets;

	// Token: 0x040026D6 RID: 9942
	private List<DungeonVolume> volumes;

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002F83 RID: 12163 RVA: 0x0011DFDB File Offset: 0x0011C1DB
	internal List<DungeonBaseSocket> Sockets
	{
		get
		{
			if (this.sockets == null)
			{
				this.sockets = new List<DungeonBaseSocket>();
				base.GetComponentsInChildren<DungeonBaseSocket>(true, this.sockets);
			}
			return this.sockets;
		}
	}

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06002F84 RID: 12164 RVA: 0x0011E003 File Offset: 0x0011C203
	internal List<DungeonVolume> Volumes
	{
		get
		{
			if (this.volumes == null)
			{
				this.volumes = new List<DungeonVolume>();
				base.GetComponentsInChildren<DungeonVolume>(true, this.volumes);
			}
			return this.volumes;
		}
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x0011E02C File Offset: 0x0011C22C
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		this.Dungeon = TerrainMeta.Path.FindClosest<DungeonBaseInfo>(TerrainMeta.Path.DungeonBaseEntrances, base.transform.position);
		if (this.Dungeon == null)
		{
			return;
		}
		this.Dungeon.Add(this);
	}
}
