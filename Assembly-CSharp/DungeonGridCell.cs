using System;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class DungeonGridCell : MonoBehaviour
{
	// Token: 0x040026E5 RID: 9957
	public DungeonGridConnectionType North;

	// Token: 0x040026E6 RID: 9958
	public DungeonGridConnectionType South;

	// Token: 0x040026E7 RID: 9959
	public DungeonGridConnectionType West;

	// Token: 0x040026E8 RID: 9960
	public DungeonGridConnectionType East;

	// Token: 0x040026E9 RID: 9961
	public DungeonGridConnectionVariant NorthVariant;

	// Token: 0x040026EA RID: 9962
	public DungeonGridConnectionVariant SouthVariant;

	// Token: 0x040026EB RID: 9963
	public DungeonGridConnectionVariant WestVariant;

	// Token: 0x040026EC RID: 9964
	public DungeonGridConnectionVariant EastVariant;

	// Token: 0x040026ED RID: 9965
	public GameObjectRef[] AvoidNeighbours;

	// Token: 0x040026EE RID: 9966
	public MeshRenderer[] MapRenderers;

	// Token: 0x06002F89 RID: 12169 RVA: 0x0011E0C8 File Offset: 0x0011C2C8
	public bool ShouldAvoid(uint id)
	{
		GameObjectRef[] avoidNeighbours = this.AvoidNeighbours;
		for (int i = 0; i < avoidNeighbours.Length; i++)
		{
			if (avoidNeighbours[i].resourceID == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x0011E0F8 File Offset: 0x0011C2F8
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.DungeonGridCells.Add(this);
		}
	}
}
