using System;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class Sandstorm : MonoBehaviour
{
	// Token: 0x04001885 RID: 6277
	public ParticleSystem m_psSandStorm;

	// Token: 0x04001886 RID: 6278
	public float m_flSpeed;

	// Token: 0x04001887 RID: 6279
	public float m_flSwirl;

	// Token: 0x04001888 RID: 6280
	public float m_flEmissionRate;

	// Token: 0x06001F1E RID: 7966 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000D31F0 File Offset: 0x000D13F0
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = -7f + Mathf.Sin(Time.time * 2.5f) * 7f;
		base.transform.eulerAngles = eulerAngles;
		if (this.m_psSandStorm != null)
		{
			this.m_psSandStorm.startSpeed = this.m_flSpeed;
			this.m_psSandStorm.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psSandStorm.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}
