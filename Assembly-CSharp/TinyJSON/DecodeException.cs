using System;

namespace TinyJSON
{
	// Token: 0x020009D1 RID: 2513
	public sealed class DecodeException : Exception
	{
		// Token: 0x06003BE8 RID: 15336 RVA: 0x00162051 File Offset: 0x00160251
		public DecodeException(string message) : base(message)
		{
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x0016205A File Offset: 0x0016025A
		public DecodeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
