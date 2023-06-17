using System;
using ProtoBuf;

namespace CompanionServer
{
	// Token: 0x020009E4 RID: 2532
	public interface IConnection
	{
		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06003C8B RID: 15499
		long ConnectionId { get; }

		// Token: 0x06003C8C RID: 15500
		void Send(AppResponse response);

		// Token: 0x06003C8D RID: 15501
		void Subscribe(PlayerTarget target);

		// Token: 0x06003C8E RID: 15502
		void Subscribe(EntityTarget target);

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06003C8F RID: 15503
		IRemoteControllable CurrentCamera { get; }

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06003C90 RID: 15504
		bool IsControllingCamera { get; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06003C91 RID: 15505
		ulong ControllingSteamId { get; }

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06003C92 RID: 15506
		// (set) Token: 0x06003C93 RID: 15507
		InputState InputState { get; set; }

		// Token: 0x06003C94 RID: 15508
		bool BeginViewing(IRemoteControllable camera);

		// Token: 0x06003C95 RID: 15509
		void EndViewing();
	}
}
