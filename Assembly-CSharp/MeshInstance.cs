using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public struct MeshInstance
{
	// Token: 0x04001639 RID: 5689
	public Vector3 position;

	// Token: 0x0400163A RID: 5690
	public Quaternion rotation;

	// Token: 0x0400163B RID: 5691
	public Vector3 scale;

	// Token: 0x0400163C RID: 5692
	public MeshCache.Data data;

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001D4F RID: 7503 RVA: 0x000CA0CD File Offset: 0x000C82CD
	// (set) Token: 0x06001D50 RID: 7504 RVA: 0x000CA0DA File Offset: 0x000C82DA
	public Mesh mesh
	{
		get
		{
			return this.data.mesh;
		}
		set
		{
			this.data = MeshCache.Get(value);
		}
	}
}
