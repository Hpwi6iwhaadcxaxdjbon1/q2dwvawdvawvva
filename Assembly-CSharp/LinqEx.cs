using System;
using System.Collections.Generic;

// Token: 0x02000920 RID: 2336
public static class LinqEx
{
	// Token: 0x06003848 RID: 14408 RVA: 0x0014FC8C File Offset: 0x0014DE8C
	public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
	{
		int num = -1;
		T other = default(T);
		int num2 = 0;
		foreach (T t in sequence)
		{
			if (t.CompareTo(other) > 0 || num == -1)
			{
				num = num2;
				other = t;
			}
			num2++;
		}
		return num;
	}
}
