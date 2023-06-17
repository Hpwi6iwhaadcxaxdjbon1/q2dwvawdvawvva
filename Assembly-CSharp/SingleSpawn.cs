using System;

// Token: 0x02000574 RID: 1396
public class SingleSpawn : SpawnGroup
{
	// Token: 0x06002ABE RID: 10942 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool WantsInitialSpawn()
	{
		return false;
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x0010403B File Offset: 0x0010223B
	public void FillDelay(float delay)
	{
		base.Invoke(new Action(this.Fill), delay);
	}
}
