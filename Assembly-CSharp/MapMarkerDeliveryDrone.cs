using System;

// Token: 0x0200016C RID: 364
public class MapMarkerDeliveryDrone : MapMarker
{
	// Token: 0x0600176E RID: 5998 RVA: 0x000B24CD File Offset: 0x000B06CD
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x000B24DC File Offset: 0x000B06DC
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
