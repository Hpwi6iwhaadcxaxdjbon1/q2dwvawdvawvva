using System;
using UnityEngine;

// Token: 0x020002B1 RID: 689
public struct MeshRendererInstance
{
	// Token: 0x04001645 RID: 5701
	public Renderer renderer;

	// Token: 0x04001646 RID: 5702
	public OBB bounds;

	// Token: 0x04001647 RID: 5703
	public Vector3 position;

	// Token: 0x04001648 RID: 5704
	public Quaternion rotation;

	// Token: 0x04001649 RID: 5705
	public Vector3 scale;

	// Token: 0x0400164A RID: 5706
	public MeshCache.Data data;

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001D59 RID: 7513 RVA: 0x000CA978 File Offset: 0x000C8B78
	// (set) Token: 0x06001D5A RID: 7514 RVA: 0x000CA985 File Offset: 0x000C8B85
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
