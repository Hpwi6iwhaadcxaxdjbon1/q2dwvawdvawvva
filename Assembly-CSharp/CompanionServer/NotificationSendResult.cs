using System;

namespace CompanionServer
{
	// Token: 0x020009E7 RID: 2535
	public enum NotificationSendResult
	{
		// Token: 0x040036B6 RID: 14006
		Failed,
		// Token: 0x040036B7 RID: 14007
		Sent,
		// Token: 0x040036B8 RID: 14008
		Empty,
		// Token: 0x040036B9 RID: 14009
		Disabled,
		// Token: 0x040036BA RID: 14010
		RateLimited,
		// Token: 0x040036BB RID: 14011
		ServerError,
		// Token: 0x040036BC RID: 14012
		NoTargetsFound,
		// Token: 0x040036BD RID: 14013
		TooManySubscribers
	}
}
