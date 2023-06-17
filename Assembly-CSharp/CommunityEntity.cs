using System;
using Network;

// Token: 0x0200005D RID: 93
public class CommunityEntity : PointEntity
{
	// Token: 0x04000686 RID: 1670
	public static CommunityEntity ServerInstance;

	// Token: 0x04000687 RID: 1671
	public static CommunityEntity ClientInstance;

	// Token: 0x060009CA RID: 2506 RVA: 0x0005C38C File Offset: 0x0005A58C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CommunityEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0005C3CC File Offset: 0x0005A5CC
	public override void InitShared()
	{
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = this;
		}
		else
		{
			CommunityEntity.ClientInstance = this;
		}
		base.InitShared();
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0005C3EA File Offset: 0x0005A5EA
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			CommunityEntity.ServerInstance = null;
			return;
		}
		CommunityEntity.ClientInstance = null;
	}
}
