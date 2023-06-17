using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A01 RID: 2561
	public class SetEntityValue : BaseEntityHandler<AppSetEntityValue>
	{
		// Token: 0x06003D2B RID: 15659 RVA: 0x0016715C File Offset: 0x0016535C
		public override void Execute()
		{
			SmartSwitch smartSwitch;
			if ((smartSwitch = (base.Entity as SmartSwitch)) != null)
			{
				smartSwitch.Value = base.Proto.value;
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
