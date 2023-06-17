using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009F1 RID: 2545
	public class TokenBucketList<TKey> : ITokenBucketSettings
	{
		// Token: 0x040036D6 RID: 14038
		private readonly Dictionary<TKey, TokenBucket> _buckets;

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06003CDF RID: 15583 RVA: 0x00165C6D File Offset: 0x00163E6D
		public double MaxTokens { get; }

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06003CE0 RID: 15584 RVA: 0x00165C75 File Offset: 0x00163E75
		public double TokensPerSec { get; }

		// Token: 0x06003CE1 RID: 15585 RVA: 0x00165C7D File Offset: 0x00163E7D
		public TokenBucketList(double maxTokens, double tokensPerSec)
		{
			this._buckets = new Dictionary<TKey, TokenBucket>();
			this.MaxTokens = maxTokens;
			this.TokensPerSec = tokensPerSec;
		}

		// Token: 0x06003CE2 RID: 15586 RVA: 0x00165CA0 File Offset: 0x00163EA0
		public TokenBucket Get(TKey key)
		{
			TokenBucket result;
			if (this._buckets.TryGetValue(key, out result))
			{
				return result;
			}
			TokenBucket tokenBucket = Pool.Get<TokenBucket>();
			tokenBucket.Settings = this;
			tokenBucket.Reset();
			this._buckets.Add(key, tokenBucket);
			return tokenBucket;
		}

		// Token: 0x06003CE3 RID: 15587 RVA: 0x00165CE0 File Offset: 0x00163EE0
		public void Cleanup()
		{
			List<TKey> list = Pool.GetList<TKey>();
			foreach (KeyValuePair<TKey, TokenBucket> keyValuePair in this._buckets)
			{
				if (keyValuePair.Value.IsFull)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (TKey key in list)
			{
				TokenBucket tokenBucket;
				if (this._buckets.TryGetValue(key, out tokenBucket))
				{
					Pool.Free<TokenBucket>(ref tokenBucket);
					this._buckets.Remove(key);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}
	}
}
