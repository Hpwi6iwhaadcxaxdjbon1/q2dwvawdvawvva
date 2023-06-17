using System;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x020009C8 RID: 2504
	public static class Extensions
	{
		// Token: 0x06003BDC RID: 15324 RVA: 0x00161F98 File Offset: 0x00160198
		public static bool AnyOfType<TSource>(this IEnumerable<TSource> source, Type expectedType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (expectedType == null)
			{
				throw new ArgumentNullException("expectedType");
			}
			foreach (TSource tsource in source)
			{
				if (expectedType.IsInstanceOfType(tsource))
				{
					return true;
				}
			}
			return false;
		}
	}
}
