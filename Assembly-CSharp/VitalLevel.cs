using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
[Serializable]
public struct VitalLevel
{
	// Token: 0x04001372 RID: 4978
	public float Level;

	// Token: 0x04001373 RID: 4979
	private float lastUsedTime;

	// Token: 0x06001B9C RID: 7068 RVA: 0x000C2663 File Offset: 0x000C0863
	internal void Add(float f)
	{
		this.Level += f;
		if (this.Level > 1f)
		{
			this.Level = 1f;
		}
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x06001B9D RID: 7069 RVA: 0x000C26A3 File Offset: 0x000C08A3
	public float TimeSinceUsed
	{
		get
		{
			return Time.time - this.lastUsedTime;
		}
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x000C26B4 File Offset: 0x000C08B4
	internal void Use(float f)
	{
		if (Mathf.Approximately(f, 0f))
		{
			return;
		}
		this.Level -= Mathf.Abs(f);
		if (this.Level < 0f)
		{
			this.Level = 0f;
		}
		this.lastUsedTime = Time.time;
	}
}
