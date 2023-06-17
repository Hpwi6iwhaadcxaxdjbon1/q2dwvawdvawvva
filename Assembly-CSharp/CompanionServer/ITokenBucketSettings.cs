using System;

namespace CompanionServer
{
	// Token: 0x020009F0 RID: 2544
	public interface ITokenBucketSettings
	{
		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06003CDD RID: 15581
		double MaxTokens { get; }

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06003CDE RID: 15582
		double TokensPerSec { get; }
	}
}
