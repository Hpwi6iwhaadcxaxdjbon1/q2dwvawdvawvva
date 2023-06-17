using System;

// Token: 0x020005ED RID: 1517
public interface IAirSupply
{
	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x06002D57 RID: 11607
	ItemModGiveOxygen.AirSupplyType AirType { get; }

	// Token: 0x06002D58 RID: 11608
	float GetAirTimeRemaining();
}
