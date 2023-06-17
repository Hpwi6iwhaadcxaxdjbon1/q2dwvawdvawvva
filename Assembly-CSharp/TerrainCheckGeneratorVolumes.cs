using System;
using UnityEngine;

// Token: 0x0200068F RID: 1679
public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002759 RID: 10073
	public float PlacementRadius;

	// Token: 0x06002FD2 RID: 12242 RVA: 0x0011F685 File Offset: 0x0011D885
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.PlacementRadius);
	}
}
