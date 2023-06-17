using System;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class AverageVelocity
{
	// Token: 0x040015C1 RID: 5569
	private Vector3 pos;

	// Token: 0x040015C2 RID: 5570
	private float time;

	// Token: 0x040015C3 RID: 5571
	private float lastEntry;

	// Token: 0x040015C4 RID: 5572
	private float averageSpeed;

	// Token: 0x040015C5 RID: 5573
	private Vector3 averageVelocity;

	// Token: 0x06001D05 RID: 7429 RVA: 0x000C8AE4 File Offset: 0x000C6CE4
	public void Record(Vector3 newPos)
	{
		float num = Time.time - this.time;
		if (num < 0.1f)
		{
			return;
		}
		if (this.pos.sqrMagnitude > 0f)
		{
			Vector3 a = newPos - this.pos;
			this.averageVelocity = a * (1f / num);
			this.averageSpeed = this.averageVelocity.magnitude;
		}
		this.time = Time.time;
		this.pos = newPos;
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06001D06 RID: 7430 RVA: 0x000C8B5C File Offset: 0x000C6D5C
	public float Speed
	{
		get
		{
			return this.averageSpeed;
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06001D07 RID: 7431 RVA: 0x000C8B64 File Offset: 0x000C6D64
	public Vector3 Average
	{
		get
		{
			return this.averageVelocity;
		}
	}
}
