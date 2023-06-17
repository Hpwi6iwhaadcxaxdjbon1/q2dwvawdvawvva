using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class PathSpeedZone : MonoBehaviour, IAIPathSpeedZone
{
	// Token: 0x04001160 RID: 4448
	public Bounds bounds;

	// Token: 0x04001161 RID: 4449
	public OBB obbBounds;

	// Token: 0x04001162 RID: 4450
	public float maxVelocityPerSec = 5f;

	// Token: 0x060018B5 RID: 6325 RVA: 0x000B79AE File Offset: 0x000B5BAE
	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x000B79DC File Offset: 0x000B5BDC
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x000B79E4 File Offset: 0x000B5BE4
	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}
}
