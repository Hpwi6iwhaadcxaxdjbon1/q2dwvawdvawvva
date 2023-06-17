using System;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class SprayCanSpray_Decal : SprayCanSpray, ICustomMaterialReplacer, IPropRenderNotify, INotifyLOD
{
	// Token: 0x040011C0 RID: 4544
	public DeferredDecal DecalComponent;

	// Token: 0x040011C1 RID: 4545
	public GameObject IconPreviewRoot;

	// Token: 0x040011C2 RID: 4546
	public Material DefaultMaterial;
}
