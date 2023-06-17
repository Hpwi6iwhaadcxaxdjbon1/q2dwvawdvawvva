using System;
using UnityEngine;

// Token: 0x02000951 RID: 2385
public class TimeCachedValue<T>
{
	// Token: 0x04003382 RID: 13186
	public float refreshCooldown;

	// Token: 0x04003383 RID: 13187
	public float refreshRandomRange;

	// Token: 0x04003384 RID: 13188
	public Func<T> updateValue;

	// Token: 0x04003385 RID: 13189
	private T cachedValue;

	// Token: 0x04003386 RID: 13190
	private TimeSince cooldown;

	// Token: 0x04003387 RID: 13191
	private bool hasRun;

	// Token: 0x04003388 RID: 13192
	private bool forceNextRun;

	// Token: 0x06003953 RID: 14675 RVA: 0x00154EFC File Offset: 0x001530FC
	public T Get(bool force)
	{
		if (this.cooldown < this.refreshCooldown && !force && this.hasRun && !this.forceNextRun)
		{
			return this.cachedValue;
		}
		this.hasRun = true;
		this.forceNextRun = false;
		this.cooldown = 0f - UnityEngine.Random.Range(0f, this.refreshRandomRange);
		if (this.updateValue != null)
		{
			this.cachedValue = this.updateValue();
		}
		else
		{
			this.cachedValue = default(T);
		}
		return this.cachedValue;
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x00154F92 File Offset: 0x00153192
	public void ForceNextRun()
	{
		this.forceNextRun = true;
	}
}
