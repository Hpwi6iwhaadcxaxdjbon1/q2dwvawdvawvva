using System;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
public class DeployShell : PrefabAttribute
{
	// Token: 0x040020FD RID: 8445
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x060028EE RID: 10478 RVA: 0x000FC2B8 File Offset: 0x000FA4B8
	public OBB WorldSpaceBounds(Transform transform)
	{
		return new OBB(transform.position, transform.lossyScale, transform.rotation, this.bounds);
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x000FC2D7 File Offset: 0x000FA4D7
	public float LineOfSightPadding()
	{
		return 0.025f;
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000FC2DE File Offset: 0x000FA4DE
	protected override Type GetIndexedType()
	{
		return typeof(DeployShell);
	}
}
