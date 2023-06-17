using System;

// Token: 0x020004C9 RID: 1225
public class ElevatorIOEntity : IOEntity
{
	// Token: 0x0400203C RID: 8252
	public int Consumption = 5;

	// Token: 0x060027F5 RID: 10229 RVA: 0x000F8484 File Offset: 0x000F6684
	public override int ConsumptionAmount()
	{
		return this.Consumption;
	}
}
