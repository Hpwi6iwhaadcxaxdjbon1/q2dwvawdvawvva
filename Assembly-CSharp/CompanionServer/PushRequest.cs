using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009EA RID: 2538
	public class PushRequest : Pool.IPooled
	{
		// Token: 0x040036C3 RID: 14019
		public string ServerToken;

		// Token: 0x040036C4 RID: 14020
		public List<ulong> SteamIds;

		// Token: 0x040036C5 RID: 14021
		public NotificationChannel Channel;

		// Token: 0x040036C6 RID: 14022
		public string Title;

		// Token: 0x040036C7 RID: 14023
		public string Body;

		// Token: 0x040036C8 RID: 14024
		public Dictionary<string, string> Data;

		// Token: 0x06003CB3 RID: 15539 RVA: 0x00164E26 File Offset: 0x00163026
		public void EnterPool()
		{
			Pool.FreeList<ulong>(ref this.SteamIds);
			this.Channel = (NotificationChannel)0;
			this.Title = null;
			this.Body = null;
			if (this.Data != null)
			{
				this.Data.Clear();
				Pool.Free<Dictionary<string, string>>(ref this.Data);
			}
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}
	}
}
