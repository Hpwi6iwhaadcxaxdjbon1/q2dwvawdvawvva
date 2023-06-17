using System;
using UnityEngine;

// Token: 0x020002AA RID: 682
public struct MeshColliderInstance
{
	// Token: 0x04001627 RID: 5671
	public Transform transform;

	// Token: 0x04001628 RID: 5672
	public Rigidbody rigidbody;

	// Token: 0x04001629 RID: 5673
	public Collider collider;

	// Token: 0x0400162A RID: 5674
	public OBB bounds;

	// Token: 0x0400162B RID: 5675
	public Vector3 position;

	// Token: 0x0400162C RID: 5676
	public Quaternion rotation;

	// Token: 0x0400162D RID: 5677
	public Vector3 scale;

	// Token: 0x0400162E RID: 5678
	public MeshCache.Data data;

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001D41 RID: 7489 RVA: 0x000C99A8 File Offset: 0x000C7BA8
	// (set) Token: 0x06001D42 RID: 7490 RVA: 0x000C99B5 File Offset: 0x000C7BB5
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
