using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
[Serializable]
public struct StateTimer
{
	// Token: 0x04001370 RID: 4976
	public float ReleaseTime;

	// Token: 0x04001371 RID: 4977
	public Action OnFinished;

	// Token: 0x06001B9A RID: 7066 RVA: 0x000C2621 File Offset: 0x000C0821
	public void Activate(float seconds, Action onFinished = null)
	{
		this.ReleaseTime = Time.time + seconds;
		this.OnFinished = onFinished;
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x06001B9B RID: 7067 RVA: 0x000C2637 File Offset: 0x000C0837
	public bool IsActive
	{
		get
		{
			bool flag = this.ReleaseTime > Time.time;
			if (!flag && this.OnFinished != null)
			{
				this.OnFinished();
				this.OnFinished = null;
			}
			return flag;
		}
	}
}
