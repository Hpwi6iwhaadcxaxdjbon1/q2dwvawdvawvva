using System;

// Token: 0x020001FB RID: 507
public class MapMarkerPet : MapMarker
{
	// Token: 0x06001A91 RID: 6801 RVA: 0x000B24CD File Offset: 0x000B06CD
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x000B24DC File Offset: 0x000B06DC
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
