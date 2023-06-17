using System;

// Token: 0x020004C3 RID: 1219
public interface ISubscribable
{
	// Token: 0x060027BC RID: 10172
	bool AddSubscription(ulong steamId);

	// Token: 0x060027BD RID: 10173
	bool RemoveSubscription(ulong steamId);

	// Token: 0x060027BE RID: 10174
	bool HasSubscription(ulong steamId);
}
