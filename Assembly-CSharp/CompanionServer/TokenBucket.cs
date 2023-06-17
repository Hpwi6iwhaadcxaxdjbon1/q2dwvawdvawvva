using System;
using Network;

namespace CompanionServer
{
	// Token: 0x020009EF RID: 2543
	public class TokenBucket
	{
		// Token: 0x040036D3 RID: 14035
		private double _lastUpdate;

		// Token: 0x040036D4 RID: 14036
		private double _tokens;

		// Token: 0x040036D5 RID: 14037
		public ITokenBucketSettings Settings;

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06003CD7 RID: 15575 RVA: 0x00165B81 File Offset: 0x00163D81
		public bool IsFull
		{
			get
			{
				this.Update();
				return this._tokens >= this.Settings.MaxTokens;
			}
		}

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06003CD8 RID: 15576 RVA: 0x00165B9F File Offset: 0x00163D9F
		public bool IsNaughty
		{
			get
			{
				this.Update();
				return this._tokens <= -10.0;
			}
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x00165BBB File Offset: 0x00163DBB
		public void Reset()
		{
			this._lastUpdate = TimeEx.realtimeSinceStartup;
			ITokenBucketSettings settings = this.Settings;
			this._tokens = ((settings != null) ? settings.MaxTokens : 0.0);
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x00165BE8 File Offset: 0x00163DE8
		public bool TryTake(double requestedTokens)
		{
			this.Update();
			if (requestedTokens > this._tokens)
			{
				this._tokens -= 1.0;
				return false;
			}
			this._tokens -= requestedTokens;
			return true;
		}

		// Token: 0x06003CDB RID: 15579 RVA: 0x00165C20 File Offset: 0x00163E20
		private void Update()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			double num = realtimeSinceStartup - this._lastUpdate;
			this._lastUpdate = realtimeSinceStartup;
			double num2 = num * this.Settings.TokensPerSec;
			this._tokens = Math.Min(this._tokens + num2, this.Settings.MaxTokens);
		}
	}
}
