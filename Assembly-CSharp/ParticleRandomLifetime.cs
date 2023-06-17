using System;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class ParticleRandomLifetime : MonoBehaviour
{
	// Token: 0x040016CA RID: 5834
	public ParticleSystem mySystem;

	// Token: 0x040016CB RID: 5835
	public float minScale = 0.5f;

	// Token: 0x040016CC RID: 5836
	public float maxScale = 1f;

	// Token: 0x06001DA9 RID: 7593 RVA: 0x000CB624 File Offset: 0x000C9824
	public void Awake()
	{
		if (!this.mySystem)
		{
			return;
		}
		float startLifetime = UnityEngine.Random.Range(this.minScale, this.maxScale);
		this.mySystem.startLifetime = startLifetime;
	}
}
