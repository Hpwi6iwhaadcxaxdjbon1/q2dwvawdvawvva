using System;
using UnityEngine;

// Token: 0x02000993 RID: 2451
public class ExplosionsLightCurves : MonoBehaviour
{
	// Token: 0x0400349B RID: 13467
	public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x0400349C RID: 13468
	public float GraphTimeMultiplier = 1f;

	// Token: 0x0400349D RID: 13469
	public float GraphIntensityMultiplier = 1f;

	// Token: 0x0400349E RID: 13470
	private bool canUpdate;

	// Token: 0x0400349F RID: 13471
	private float startTime;

	// Token: 0x040034A0 RID: 13472
	private Light lightSource;

	// Token: 0x06003A52 RID: 14930 RVA: 0x001591EE File Offset: 0x001573EE
	private void Awake()
	{
		this.lightSource = base.GetComponent<Light>();
		this.lightSource.intensity = this.LightCurve.Evaluate(0f);
	}

	// Token: 0x06003A53 RID: 14931 RVA: 0x00159217 File Offset: 0x00157417
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x0015922C File Offset: 0x0015742C
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float intensity = this.LightCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphIntensityMultiplier;
			this.lightSource.intensity = intensity;
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}
