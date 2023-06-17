using System;

// Token: 0x02000169 RID: 361
public class JunkPileWaterSpawner : SpawnGroup
{
	// Token: 0x04001013 RID: 4115
	public BaseEntity attachToParent;

	// Token: 0x06001758 RID: 5976 RVA: 0x000B18E2 File Offset: 0x000AFAE2
	protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
		base.PostSpawnProcess(entity, spawnPoint);
		if (this.attachToParent != null)
		{
			entity.SetParent(this.attachToParent, true, false);
		}
	}
}
