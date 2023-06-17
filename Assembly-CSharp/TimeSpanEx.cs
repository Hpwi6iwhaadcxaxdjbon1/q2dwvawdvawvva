using System;

// Token: 0x02000927 RID: 2343
public static class TimeSpanEx
{
	// Token: 0x06003857 RID: 14423 RVA: 0x001501DD File Offset: 0x0014E3DD
	public static string ToShortString(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x0015020E File Offset: 0x0014E40E
	public static string ToShortStringNoHours(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	}
}
