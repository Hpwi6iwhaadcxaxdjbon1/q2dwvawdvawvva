using System;

// Token: 0x020004D3 RID: 1235
public class IndustrialEntity : IOEntity
{
	// Token: 0x0400204A RID: 8266
	public static IndustrialEntity.IndustrialProcessQueue Queue = new IndustrialEntity.IndustrialProcessQueue();

	// Token: 0x0600281E RID: 10270 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void RunJob()
	{
	}

	// Token: 0x02000D1C RID: 3356
	public class IndustrialProcessQueue : ObjectWorkQueue<IndustrialEntity>
	{
		// Token: 0x06005028 RID: 20520 RVA: 0x001A7FD8 File Offset: 0x001A61D8
		protected override void RunJob(IndustrialEntity job)
		{
			if (job != null)
			{
				job.RunJob();
			}
		}
	}
}
