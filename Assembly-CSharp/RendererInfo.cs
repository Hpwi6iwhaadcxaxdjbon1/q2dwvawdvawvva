using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002B3 RID: 691
public class RendererInfo : ComponentInfo<Renderer>
{
	// Token: 0x0400164D RID: 5709
	public ShadowCastingMode shadows;

	// Token: 0x0400164E RID: 5710
	public Material material;

	// Token: 0x0400164F RID: 5711
	public Mesh mesh;

	// Token: 0x04001650 RID: 5712
	public MeshFilter meshFilter;

	// Token: 0x06001D60 RID: 7520 RVA: 0x000CAA10 File Offset: 0x000C8C10
	public override void Reset()
	{
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = (this.component as SkinnedMeshRenderer)) != null)
		{
			skinnedMeshRenderer.sharedMesh = this.mesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter.sharedMesh = this.mesh;
		}
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000CAA88 File Offset: 0x000C8C88
	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = (this.component as SkinnedMeshRenderer)) != null)
		{
			this.mesh = skinnedMeshRenderer.sharedMesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.mesh = this.meshFilter.sharedMesh;
		}
	}
}
