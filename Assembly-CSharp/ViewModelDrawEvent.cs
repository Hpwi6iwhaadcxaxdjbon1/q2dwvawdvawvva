using System;
using UnityEngine;

// Token: 0x02000976 RID: 2422
[Serializable]
public struct ViewModelDrawEvent : IEquatable<ViewModelDrawEvent>
{
	// Token: 0x04003409 RID: 13321
	public ViewModelRenderer viewModelRenderer;

	// Token: 0x0400340A RID: 13322
	public Renderer renderer;

	// Token: 0x0400340B RID: 13323
	public bool skipDepthPrePass;

	// Token: 0x0400340C RID: 13324
	public Material material;

	// Token: 0x0400340D RID: 13325
	public int subMesh;

	// Token: 0x0400340E RID: 13326
	public int pass;

	// Token: 0x060039EA RID: 14826 RVA: 0x001575D8 File Offset: 0x001557D8
	public bool Equals(ViewModelDrawEvent other)
	{
		return object.Equals(this.viewModelRenderer, other.viewModelRenderer) && object.Equals(this.renderer, other.renderer) && this.skipDepthPrePass == other.skipDepthPrePass && object.Equals(this.material, other.material) && this.subMesh == other.subMesh && this.pass == other.pass;
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x0015764C File Offset: 0x0015584C
	public override bool Equals(object obj)
	{
		if (obj is ViewModelDrawEvent)
		{
			ViewModelDrawEvent other = (ViewModelDrawEvent)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x00157674 File Offset: 0x00155874
	public override int GetHashCode()
	{
		return ((((((this.viewModelRenderer != null) ? this.viewModelRenderer.GetHashCode() : 0) * 397 ^ ((this.renderer != null) ? this.renderer.GetHashCode() : 0)) * 397 ^ this.skipDepthPrePass.GetHashCode()) * 397 ^ ((this.material != null) ? this.material.GetHashCode() : 0)) * 397 ^ this.subMesh) * 397 ^ this.pass;
	}
}
