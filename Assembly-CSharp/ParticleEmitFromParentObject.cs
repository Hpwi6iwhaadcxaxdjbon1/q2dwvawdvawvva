using System;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class ParticleEmitFromParentObject : MonoBehaviour
{
	// Token: 0x04001870 RID: 6256
	public string bonename;

	// Token: 0x04001871 RID: 6257
	private Bounds bounds;

	// Token: 0x04001872 RID: 6258
	private Transform bone;

	// Token: 0x04001873 RID: 6259
	private BaseEntity entity;

	// Token: 0x04001874 RID: 6260
	private float lastBoundsUpdate;
}
