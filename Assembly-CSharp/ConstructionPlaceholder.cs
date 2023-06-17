using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000254 RID: 596
[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	// Token: 0x04001503 RID: 5379
	public Mesh mesh;

	// Token: 0x04001504 RID: 5380
	public Material material;

	// Token: 0x04001505 RID: 5381
	public bool renderer;

	// Token: 0x04001506 RID: 5382
	public bool collider;

	// Token: 0x06001C41 RID: 7233 RVA: 0x000C5220 File Offset: 0x000C3420
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			if (this.renderer)
			{
				UnityEngine.Object component = rootObj.GetComponent<MeshFilter>();
				MeshRenderer meshRenderer = rootObj.GetComponent<MeshRenderer>();
				if (!component)
				{
					rootObj.AddComponent<MeshFilter>().sharedMesh = this.mesh;
				}
				if (!meshRenderer)
				{
					meshRenderer = rootObj.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = this.material;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			if (this.collider && !rootObj.GetComponent<MeshCollider>())
			{
				rootObj.AddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x000C52B3 File Offset: 0x000C34B3
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}
}
