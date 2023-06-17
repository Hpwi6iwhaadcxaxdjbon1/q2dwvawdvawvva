using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class MeshRendererLookup
{
	// Token: 0x0400164B RID: 5707
	public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();

	// Token: 0x0400164C RID: 5708
	public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

	// Token: 0x06001D5B RID: 7515 RVA: 0x000CA994 File Offset: 0x000C8B94
	public void Apply()
	{
		MeshRendererLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x000CA9C6 File Offset: 0x000C8BC6
	public void Clear()
	{
		this.dst.Clear();
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x000CA9D3 File Offset: 0x000C8BD3
	public void Add(MeshRendererInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x000CA9E1 File Offset: 0x000C8BE1
	public MeshRendererLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x02000C92 RID: 3218
	public class LookupGroup
	{
		// Token: 0x040043F3 RID: 17395
		public List<MeshRendererLookup.LookupEntry> data = new List<MeshRendererLookup.LookupEntry>();

		// Token: 0x06004F1C RID: 20252 RVA: 0x001A5B45 File Offset: 0x001A3D45
		public void Clear()
		{
			this.data.Clear();
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x001A5B52 File Offset: 0x001A3D52
		public void Add(MeshRendererInstance instance)
		{
			this.data.Add(new MeshRendererLookup.LookupEntry(instance));
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x001A5B65 File Offset: 0x001A3D65
		public MeshRendererLookup.LookupEntry Get(int index)
		{
			return this.data[index];
		}
	}

	// Token: 0x02000C93 RID: 3219
	public struct LookupEntry
	{
		// Token: 0x040043F4 RID: 17396
		public Renderer renderer;

		// Token: 0x06004F20 RID: 20256 RVA: 0x001A5B86 File Offset: 0x001A3D86
		public LookupEntry(MeshRendererInstance instance)
		{
			this.renderer = instance.renderer;
		}
	}
}
