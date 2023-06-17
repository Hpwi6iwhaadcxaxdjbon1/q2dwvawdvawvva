using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class LaserBeam : MonoBehaviour
{
	// Token: 0x040011D8 RID: 4568
	public float scrollSpeed = 0.5f;

	// Token: 0x040011D9 RID: 4569
	public LineRenderer beamRenderer;

	// Token: 0x040011DA RID: 4570
	public GameObject dotObject;

	// Token: 0x040011DB RID: 4571
	public Renderer dotRenderer;

	// Token: 0x040011DC RID: 4572
	public GameObject dotSpotlight;

	// Token: 0x040011DD RID: 4573
	public Vector2 scrollDir;

	// Token: 0x040011DE RID: 4574
	public float maxDistance = 100f;

	// Token: 0x040011DF RID: 4575
	public float stillBlendFactor = 0.1f;

	// Token: 0x040011E0 RID: 4576
	public float movementBlendFactor = 0.5f;

	// Token: 0x040011E1 RID: 4577
	public float movementThreshhold = 0.15f;

	// Token: 0x040011E2 RID: 4578
	public bool isFirstPerson;

	// Token: 0x040011E3 RID: 4579
	public Transform emissionOverride;
}
