using System;
using System.Collections.Generic;

namespace CompanionServer
{
	// Token: 0x020009EC RID: 2540
	public interface IBroadcastSender<TTarget, TMessage> where TTarget : class
	{
		// Token: 0x06003CCA RID: 15562
		void BroadcastTo(List<TTarget> targets, TMessage message);
	}
}
