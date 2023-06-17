using System;
using System.Diagnostics;

// Token: 0x020003C3 RID: 963
public class EntityDebug : EntityComponent<BaseEntity>
{
	// Token: 0x04001A0B RID: 6667
	internal Stopwatch stopwatch = Stopwatch.StartNew();

	// Token: 0x06002187 RID: 8583 RVA: 0x000DAAD8 File Offset: 0x000D8CD8
	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			base.enabled = false;
			return;
		}
		if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		bool isClient = base.baseEntity.isClient;
		if (base.baseEntity.isServer)
		{
			base.baseEntity.DebugServer(1, (float)this.stopwatch.Elapsed.TotalSeconds);
		}
		this.stopwatch.Reset();
		this.stopwatch.Start();
	}
}
