using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000665 RID: 1637
[Serializable]
public class DungeonBaseFloor
{
	// Token: 0x040026CA RID: 9930
	public List<DungeonBaseLink> Links = new List<DungeonBaseLink>();

	// Token: 0x06002F7D RID: 12157 RVA: 0x0011DE6B File Offset: 0x0011C06B
	public float Distance(Vector3 position)
	{
		return Mathf.Abs(this.Links[0].transform.position.y - position.y);
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x0011DE94 File Offset: 0x0011C094
	public float SignedDistance(Vector3 position)
	{
		return this.Links[0].transform.position.y - position.y;
	}
}
