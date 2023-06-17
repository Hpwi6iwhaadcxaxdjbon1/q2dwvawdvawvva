using System;

// Token: 0x0200048C RID: 1164
public class MapMarkerMLRSRocket : MapMarker
{
	// Token: 0x06002641 RID: 9793 RVA: 0x000B24CD File Offset: 0x000B06CD
	public override void ServerInit()
	{
		base.ServerInit();
		base.limitNetworking = true;
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000B24DC File Offset: 0x000B06DC
	public override bool ShouldNetworkTo(BasePlayer player)
	{
		return player.userID == base.OwnerID;
	}
}
