using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F9 RID: 2553
	public class EntityInfo : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003D16 RID: 15638 RVA: 0x00166B68 File Offset: 0x00164D68
		public override void Execute()
		{
			AppEntityInfo appEntityInfo = Pool.Get<AppEntityInfo>();
			appEntityInfo.type = base.Entity.Type;
			appEntityInfo.payload = Pool.Get<AppEntityPayload>();
			base.Entity.FillEntityPayload(appEntityInfo.payload);
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.entityInfo = appEntityInfo;
			base.Send(appResponse);
		}
	}
}
