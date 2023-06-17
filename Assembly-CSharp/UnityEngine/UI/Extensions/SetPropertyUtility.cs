using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A38 RID: 2616
	internal static class SetPropertyUtility
	{
		// Token: 0x06003E94 RID: 16020 RVA: 0x0016FEBC File Offset: 0x0016E0BC
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003E95 RID: 16021 RVA: 0x0016FF0B File Offset: 0x0016E10B
		public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003E96 RID: 16022 RVA: 0x0016FF26 File Offset: 0x0016E126
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x06003E97 RID: 16023 RVA: 0x0016FF48 File Offset: 0x0016E148
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
