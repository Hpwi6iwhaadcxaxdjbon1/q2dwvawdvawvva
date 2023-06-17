using System;
using CompanionServer.Cameras;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F7 RID: 2551
	public class CameraUnsubscribe : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D12 RID: 15634 RVA: 0x00166AF3 File Offset: 0x00164CF3
		public override void Execute()
		{
			if (!CameraRenderer.enabled)
			{
				base.SendError("not_enabled");
				return;
			}
			base.Client.EndViewing();
			base.SendSuccess();
		}
	}
}
