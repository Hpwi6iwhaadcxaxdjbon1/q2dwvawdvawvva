using System;
using UnityEngine;

// Token: 0x02000671 RID: 1649
public class DungeonGridLink : MonoBehaviour
{
	// Token: 0x040026FD RID: 9981
	public Transform UpSocket;

	// Token: 0x040026FE RID: 9982
	public Transform DownSocket;

	// Token: 0x040026FF RID: 9983
	public DungeonGridLinkType UpType;

	// Token: 0x04002700 RID: 9984
	public DungeonGridLinkType DownType;

	// Token: 0x04002701 RID: 9985
	public int Priority;

	// Token: 0x04002702 RID: 9986
	public int Rotation;

	// Token: 0x06002F95 RID: 12181 RVA: 0x0011E300 File Offset: 0x0011C500
	protected void Start()
	{
		if (TerrainMeta.Path == null)
		{
			return;
		}
		DungeonGridInfo dungeonGridInfo = TerrainMeta.Path.FindClosest<DungeonGridInfo>(TerrainMeta.Path.DungeonGridEntrances, base.transform.position);
		if (dungeonGridInfo == null)
		{
			return;
		}
		dungeonGridInfo.Links.Add(base.gameObject);
	}
}
