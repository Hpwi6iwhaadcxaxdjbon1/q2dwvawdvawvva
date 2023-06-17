using System;

// Token: 0x020004CB RID: 1227
public class FuseBox : IOEntity
{
	// Token: 0x06002801 RID: 10241 RVA: 0x000F8584 File Offset: 0x000F6784
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
