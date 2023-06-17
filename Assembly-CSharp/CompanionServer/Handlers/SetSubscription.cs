using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A02 RID: 2562
	public class SetSubscription : BaseEntityHandler<AppFlag>
	{
		// Token: 0x06003D2D RID: 15661 RVA: 0x001671A4 File Offset: 0x001653A4
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = (base.Entity as ISubscribable)) != null)
			{
				if (base.Proto.value)
				{
					if (subscribable.AddSubscription(base.UserId))
					{
						base.SendSuccess();
					}
					else
					{
						base.SendError("too_many_subscribers");
					}
				}
				else
				{
					subscribable.RemoveSubscription(base.UserId);
				}
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
