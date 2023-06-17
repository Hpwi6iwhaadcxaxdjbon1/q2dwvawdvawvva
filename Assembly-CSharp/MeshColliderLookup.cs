using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class MeshColliderLookup
{
	// Token: 0x0400162F RID: 5679
	public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();

	// Token: 0x04001630 RID: 5680
	public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

	// Token: 0x06001D43 RID: 7491 RVA: 0x000C99C4 File Offset: 0x000C7BC4
	public void Apply()
	{
		MeshColliderLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x000C99F6 File Offset: 0x000C7BF6
	public void Add(MeshColliderInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x000C9A04 File Offset: 0x000C7C04
	public MeshColliderLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x02000C90 RID: 3216
	public class LookupGroup
	{
		// Token: 0x040043ED RID: 17389
		public List<MeshColliderLookup.LookupEntry> data = new List<MeshColliderLookup.LookupEntry>();

		// Token: 0x040043EE RID: 17390
		public List<int> indices = new List<int>();

		// Token: 0x06004F17 RID: 20247 RVA: 0x001A5A6D File Offset: 0x001A3C6D
		public void Clear()
		{
			this.data.Clear();
			this.indices.Clear();
		}

		// Token: 0x06004F18 RID: 20248 RVA: 0x001A5A88 File Offset: 0x001A3C88
		public void Add(MeshColliderInstance instance)
		{
			this.data.Add(new MeshColliderLookup.LookupEntry(instance));
			int item = this.data.Count - 1;
			int num = instance.data.triangles.Length / 3;
			for (int i = 0; i < num; i++)
			{
				this.indices.Add(item);
			}
		}

		// Token: 0x06004F19 RID: 20249 RVA: 0x001A5ADC File Offset: 0x001A3CDC
		public MeshColliderLookup.LookupEntry Get(int index)
		{
			return this.data[this.indices[index]];
		}
	}

	// Token: 0x02000C91 RID: 3217
	public struct LookupEntry
	{
		// Token: 0x040043EF RID: 17391
		public Transform transform;

		// Token: 0x040043F0 RID: 17392
		public Rigidbody rigidbody;

		// Token: 0x040043F1 RID: 17393
		public Collider collider;

		// Token: 0x040043F2 RID: 17394
		public OBB bounds;

		// Token: 0x06004F1B RID: 20251 RVA: 0x001A5B13 File Offset: 0x001A3D13
		public LookupEntry(MeshColliderInstance instance)
		{
			this.transform = instance.transform;
			this.rigidbody = instance.rigidbody;
			this.collider = instance.collider;
			this.bounds = instance.bounds;
		}
	}
}
