using System;
using UnityEngine;

// Token: 0x02000712 RID: 1810
[ExecuteInEditMode]
public class DeferredDecal : MonoBehaviour
{
	// Token: 0x0400296A RID: 10602
	public Mesh mesh;

	// Token: 0x0400296B RID: 10603
	public Material material;

	// Token: 0x0400296C RID: 10604
	public DeferredDecalQueue queue;

	// Token: 0x0400296D RID: 10605
	public bool applyImmediately = true;
}
