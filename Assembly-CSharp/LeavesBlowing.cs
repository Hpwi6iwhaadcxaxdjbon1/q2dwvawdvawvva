using System;
using UnityEngine;

// Token: 0x02000608 RID: 1544
public class LeavesBlowing : MonoBehaviour
{
	// Token: 0x04002555 RID: 9557
	public ParticleSystem m_psLeaves;

	// Token: 0x04002556 RID: 9558
	public float m_flSwirl;

	// Token: 0x04002557 RID: 9559
	public float m_flSpeed;

	// Token: 0x04002558 RID: 9560
	public float m_flEmissionRate;

	// Token: 0x06002DBB RID: 11707 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x00112F64 File Offset: 0x00111164
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		if (this.m_psLeaves != null)
		{
			this.m_psLeaves.startSpeed = this.m_flSpeed;
			this.m_psLeaves.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psLeaves.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}
