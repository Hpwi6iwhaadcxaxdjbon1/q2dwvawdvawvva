using System;
using UnityEngine;

// Token: 0x0200099B RID: 2459
public struct OccludeeSphere
{
	// Token: 0x040034D9 RID: 13529
	public int id;

	// Token: 0x040034DA RID: 13530
	public OccludeeState state;

	// Token: 0x040034DB RID: 13531
	public OcclusionCulling.Sphere sphere;

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06003A77 RID: 14967 RVA: 0x00159C98 File Offset: 0x00157E98
	public bool IsRegistered
	{
		get
		{
			return this.id >= 0;
		}
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x00159CA6 File Offset: 0x00157EA6
	public void Invalidate()
	{
		this.id = -1;
		this.state = null;
		this.sphere = default(OcclusionCulling.Sphere);
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x00159CC2 File Offset: 0x00157EC2
	public OccludeeSphere(int id)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = new OcclusionCulling.Sphere(Vector3.zero, 0f);
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x00159CF3 File Offset: 0x00157EF3
	public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = sphere;
	}
}
