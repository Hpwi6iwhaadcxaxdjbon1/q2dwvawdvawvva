using System;

// Token: 0x0200051E RID: 1310
public static class HitAreaUtil
{
	// Token: 0x060029AE RID: 10670 RVA: 0x000FF773 File Offset: 0x000FD973
	public static string Format(HitArea area)
	{
		if (area == (HitArea)0)
		{
			return "None";
		}
		if (area == (HitArea)(-1))
		{
			return "Generic";
		}
		return area.ToString();
	}
}
