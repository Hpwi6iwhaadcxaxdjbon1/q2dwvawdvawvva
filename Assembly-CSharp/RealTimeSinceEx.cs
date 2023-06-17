using System;
using Network;

// Token: 0x0200094C RID: 2380
public struct RealTimeSinceEx
{
	// Token: 0x04003375 RID: 13173
	private double time;

	// Token: 0x0600392F RID: 14639 RVA: 0x00154840 File Offset: 0x00152A40
	public static implicit operator double(RealTimeSinceEx ts)
	{
		return TimeEx.realtimeSinceStartup - ts.time;
	}

	// Token: 0x06003930 RID: 14640 RVA: 0x00154850 File Offset: 0x00152A50
	public static implicit operator RealTimeSinceEx(double ts)
	{
		return new RealTimeSinceEx
		{
			time = TimeEx.realtimeSinceStartup - ts
		};
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x00154874 File Offset: 0x00152A74
	public override string ToString()
	{
		return (TimeEx.realtimeSinceStartup - this.time).ToString();
	}
}
