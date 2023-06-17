using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200035B RID: 859
public class Profile
{
	// Token: 0x040018BF RID: 6335
	public Stopwatch watch = new Stopwatch();

	// Token: 0x040018C0 RID: 6336
	public string category;

	// Token: 0x040018C1 RID: 6337
	public string name;

	// Token: 0x040018C2 RID: 6338
	public float warnTime;

	// Token: 0x06001F56 RID: 8022 RVA: 0x000D3F77 File Offset: 0x000D2177
	public Profile(string cat, string nam, float WarnTime = 1f)
	{
		this.category = cat;
		this.name = nam;
		this.warnTime = WarnTime;
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000D3F9F File Offset: 0x000D219F
	public void Start()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000D3FB8 File Offset: 0x000D21B8
	public void Stop()
	{
		this.watch.Stop();
		if ((float)this.watch.Elapsed.Seconds > this.warnTime)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				this.category,
				".",
				this.name,
				": Took ",
				this.watch.Elapsed.Seconds,
				" seconds"
			}));
		}
	}
}
