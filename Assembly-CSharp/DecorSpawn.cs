using System;
using UnityEngine;

// Token: 0x02000663 RID: 1635
public class DecorSpawn : MonoBehaviour, IClientComponent
{
	// Token: 0x040026BC RID: 9916
	public SpawnFilter Filter;

	// Token: 0x040026BD RID: 9917
	public string ResourceFolder = string.Empty;

	// Token: 0x040026BE RID: 9918
	public uint Seed;

	// Token: 0x040026BF RID: 9919
	public float ObjectCutoff = 0.2f;

	// Token: 0x040026C0 RID: 9920
	public float ObjectTapering = 0.2f;

	// Token: 0x040026C1 RID: 9921
	public int ObjectsPerPatch = 10;

	// Token: 0x040026C2 RID: 9922
	public float ClusterRadius = 2f;

	// Token: 0x040026C3 RID: 9923
	public int ClusterSizeMin = 1;

	// Token: 0x040026C4 RID: 9924
	public int ClusterSizeMax = 10;

	// Token: 0x040026C5 RID: 9925
	public int PatchCount = 8;

	// Token: 0x040026C6 RID: 9926
	public int PatchSize = 100;

	// Token: 0x040026C7 RID: 9927
	public bool LOD = true;
}
