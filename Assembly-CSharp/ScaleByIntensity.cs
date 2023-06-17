using System;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class ScaleByIntensity : MonoBehaviour
{
	// Token: 0x04001731 RID: 5937
	public Vector3 initialScale = Vector3.zero;

	// Token: 0x04001732 RID: 5938
	public Light intensitySource;

	// Token: 0x04001733 RID: 5939
	public float maxIntensity = 1f;

	// Token: 0x06001DE6 RID: 7654 RVA: 0x000CC5F1 File Offset: 0x000CA7F1
	private void Start()
	{
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000CC604 File Offset: 0x000CA804
	private void Update()
	{
		base.transform.localScale = (this.intensitySource.enabled ? (this.initialScale * this.intensitySource.intensity / this.maxIntensity) : Vector3.zero);
	}
}
