using System;
using UnityEngine;

// Token: 0x02000471 RID: 1137
public class BuoyancyPoint : MonoBehaviour
{
	// Token: 0x04001DC1 RID: 7617
	public float buoyancyForce = 10f;

	// Token: 0x04001DC2 RID: 7618
	public float size = 0.1f;

	// Token: 0x04001DC3 RID: 7619
	public float waveScale = 0.2f;

	// Token: 0x04001DC4 RID: 7620
	public float waveFrequency = 1f;

	// Token: 0x04001DC5 RID: 7621
	public bool doSplashEffects = true;

	// Token: 0x04001DC6 RID: 7622
	[NonSerialized]
	public float randomOffset;

	// Token: 0x04001DC7 RID: 7623
	[NonSerialized]
	public bool wasSubmergedLastFrame;

	// Token: 0x04001DC8 RID: 7624
	[NonSerialized]
	public float nexSplashTime;

	// Token: 0x04001DC9 RID: 7625
	private static readonly Color gizmoColour = new Color(1f, 0f, 0f, 0.25f);

	// Token: 0x06002584 RID: 9604 RVA: 0x000ECC70 File Offset: 0x000EAE70
	public void Start()
	{
		this.randomOffset = UnityEngine.Random.Range(0f, 20f);
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000ECC87 File Offset: 0x000EAE87
	public void OnDrawGizmos()
	{
		Gizmos.color = BuoyancyPoint.gizmoColour;
		Gizmos.DrawSphere(base.transform.position, this.size * 0.5f);
	}
}
