using System;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class AITraversalWaitPoint : MonoBehaviour
{
	// Token: 0x04001256 RID: 4694
	public float nextFreeTime;

	// Token: 0x06001997 RID: 6551 RVA: 0x000BB83E File Offset: 0x000B9A3E
	public bool Occupied()
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x000BB84D File Offset: 0x000B9A4D
	public void Occupy(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}
}
