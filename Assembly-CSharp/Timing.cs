using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000952 RID: 2386
public struct Timing
{
	// Token: 0x04003389 RID: 13193
	private Stopwatch sw;

	// Token: 0x0400338A RID: 13194
	private string name;

	// Token: 0x06003956 RID: 14678 RVA: 0x00154F9B File Offset: 0x0015319B
	public static Timing Start(string name)
	{
		return new Timing(name);
	}

	// Token: 0x06003957 RID: 14679 RVA: 0x00154FA4 File Offset: 0x001531A4
	public void End()
	{
		if (this.sw.Elapsed.TotalSeconds > 0.30000001192092896)
		{
			UnityEngine.Debug.Log("[" + this.sw.Elapsed.TotalSeconds.ToString("0.0") + "s] " + this.name);
		}
	}

	// Token: 0x06003958 RID: 14680 RVA: 0x00155009 File Offset: 0x00153209
	public Timing(string name)
	{
		this.sw = Stopwatch.StartNew();
		this.name = name;
	}
}
