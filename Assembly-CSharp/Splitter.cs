using System;

// Token: 0x020004CF RID: 1231
public class Splitter : IOEntity
{
	// Token: 0x17000369 RID: 873
	// (get) Token: 0x0600280D RID: 10253 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000F86C9 File Offset: 0x000F68C9
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.MarkDirtyForceUpdateOutputs();
	}
}
