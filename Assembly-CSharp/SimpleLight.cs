using System;

// Token: 0x020004CE RID: 1230
public class SimpleLight : IOEntity
{
	// Token: 0x0600280A RID: 10250 RVA: 0x000F86B7 File Offset: 0x000F68B7
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000F8584 File Offset: 0x000F6784
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
	}
}
