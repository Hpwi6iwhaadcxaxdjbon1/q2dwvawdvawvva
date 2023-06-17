using System;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FA RID: 2554
	public enum ValidationResult
	{
		// Token: 0x040036E9 RID: 14057
		Success,
		// Token: 0x040036EA RID: 14058
		NotFound,
		// Token: 0x040036EB RID: 14059
		RateLimit,
		// Token: 0x040036EC RID: 14060
		Banned,
		// Token: 0x040036ED RID: 14061
		Rejected
	}
}
