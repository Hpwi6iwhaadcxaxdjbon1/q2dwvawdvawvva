using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F8 RID: 2552
	public class CheckSubscription : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003D14 RID: 15636 RVA: 0x00166B24 File Offset: 0x00164D24
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = (base.Entity as ISubscribable)) != null)
			{
				bool value = subscribable.HasSubscription(base.UserId);
				base.SendFlag(value);
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
