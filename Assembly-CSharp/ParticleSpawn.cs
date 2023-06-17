using System;
using UnityEngine;

// Token: 0x0200067B RID: 1659
public class ParticleSpawn : SingletonComponent<ParticleSpawn>, IClientComponent
{
	// Token: 0x0400271C RID: 10012
	public GameObjectRef[] Prefabs;

	// Token: 0x0400271D RID: 10013
	public int PatchCount = 8;

	// Token: 0x0400271E RID: 10014
	public int PatchSize = 100;

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06002FA7 RID: 12199 RVA: 0x0011E785 File Offset: 0x0011C985
	// (set) Token: 0x06002FA8 RID: 12200 RVA: 0x0011E78D File Offset: 0x0011C98D
	public Vector3 Origin { get; private set; }
}
