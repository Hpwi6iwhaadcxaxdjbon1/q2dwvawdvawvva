using System;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class ProceduralDungeonCell : BaseMonoBehaviour
{
	// Token: 0x040010F8 RID: 4344
	public bool north;

	// Token: 0x040010F9 RID: 4345
	public bool east;

	// Token: 0x040010FA RID: 4346
	public bool south;

	// Token: 0x040010FB RID: 4347
	public bool west;

	// Token: 0x040010FC RID: 4348
	public bool entrance;

	// Token: 0x040010FD RID: 4349
	public bool hasSpawn;

	// Token: 0x040010FE RID: 4350
	public Transform exitPointHack;

	// Token: 0x040010FF RID: 4351
	public SpawnGroup[] spawnGroups;

	// Token: 0x04001100 RID: 4352
	public MeshRenderer[] mapRenderers;

	// Token: 0x06001815 RID: 6165 RVA: 0x000B4F14 File Offset: 0x000B3114
	public void Awake()
	{
		this.spawnGroups = base.GetComponentsInChildren<SpawnGroup>();
	}
}
