using System;

// Token: 0x02000472 RID: 1138
public class KayakSeat : BaseVehicleSeat
{
	// Token: 0x04001DCA RID: 7626
	public ItemDefinition PaddleItem;

	// Token: 0x06002588 RID: 9608 RVA: 0x000ECD0A File Offset: 0x000EAF0A
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerMounted();
		}
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000ECD2B File Offset: 0x000EAF2B
	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (this.VehicleParent() != null)
		{
			this.VehicleParent().OnPlayerDismounted(player);
		}
	}
}
