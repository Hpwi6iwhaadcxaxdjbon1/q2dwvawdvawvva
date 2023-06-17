using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class Performance : SingletonComponent<global::Performance>
{
	// Token: 0x040017F5 RID: 6133
	public static global::Performance.Tick current;

	// Token: 0x040017F6 RID: 6134
	public static global::Performance.Tick report;

	// Token: 0x040017F7 RID: 6135
	public const int FrameHistoryCount = 1000;

	// Token: 0x040017F8 RID: 6136
	private const int HistoryLength = 60;

	// Token: 0x040017F9 RID: 6137
	private static long cycles = 0L;

	// Token: 0x040017FA RID: 6138
	private static int[] frameRateHistory = new int[60];

	// Token: 0x040017FB RID: 6139
	private static float[] frameTimeHistory = new float[60];

	// Token: 0x040017FC RID: 6140
	private static int[] frameTimes = new int[1000];

	// Token: 0x040017FD RID: 6141
	private int frames;

	// Token: 0x040017FE RID: 6142
	private float time;

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000D23C4 File Offset: 0x000D05C4
	private void Update()
	{
		global::Performance.frameTimes[Time.frameCount % 1000] = (int)(1000f * Time.deltaTime);
		using (TimeWarning.New("FPSTimer", 0))
		{
			this.FPSTimer();
		}
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000D241C File Offset: 0x000D061C
	public List<int> GetFrameTimes(int requestedStart, int maxCount, out int startIndex)
	{
		startIndex = Math.Max(requestedStart, Math.Max(Time.frameCount - 1000 - 1, 0));
		int num = Math.Min(Math.Min(1000, maxCount), Time.frameCount);
		List<int> list = Pool.GetList<int>();
		for (int i = 0; i < num; i++)
		{
			int num2 = (startIndex + i) % 1000;
			list.Add(global::Performance.frameTimes[num2]);
		}
		return list;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000D2488 File Offset: 0x000D0688
	private void FPSTimer()
	{
		this.frames++;
		this.time += Time.unscaledDeltaTime;
		if (this.time < 1f)
		{
			return;
		}
		long memoryCollections = global::Performance.current.memoryCollections;
		global::Performance.current.frameID = Time.frameCount;
		global::Performance.current.frameRate = this.frames;
		global::Performance.current.frameTime = this.time / (float)this.frames * 1000f;
		checked
		{
			global::Performance.frameRateHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameRateHistory.Length)))] = global::Performance.current.frameRate;
			global::Performance.frameTimeHistory[(int)((IntPtr)(global::Performance.cycles % unchecked((long)global::Performance.frameTimeHistory.Length)))] = global::Performance.current.frameTime;
			global::Performance.current.frameRateAverage = this.AverageFrameRate();
			global::Performance.current.frameTimeAverage = this.AverageFrameTime();
		}
		global::Performance.current.memoryUsageSystem = (long)SystemInfoEx.systemMemoryUsed;
		global::Performance.current.memoryAllocations = Rust.GC.GetTotalMemory();
		global::Performance.current.memoryCollections = (long)Rust.GC.CollectionCount();
		global::Performance.current.loadBalancerTasks = (long)LoadBalancer.Count();
		global::Performance.current.invokeHandlerTasks = (long)InvokeHandler.Count();
		global::Performance.current.workshopSkinsQueued = (long)Rust.Workshop.WorkshopSkin.QueuedCount;
		global::Performance.current.gcTriggered = (memoryCollections != global::Performance.current.memoryCollections);
		this.frames = 0;
		this.time = 0f;
		global::Performance.cycles += 1L;
		global::Performance.report = global::Performance.current;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000D260C File Offset: 0x000D080C
	private float AverageFrameRate()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameRateHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < num2; i++)
		{
			num += (float)global::Performance.frameRateHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000D2650 File Offset: 0x000D0850
	private float AverageFrameTime()
	{
		float num = 0f;
		int num2 = Math.Min(global::Performance.frameTimeHistory.Length, (int)global::Performance.cycles);
		for (int i = 0; i < global::Performance.frameTimeHistory.Length; i++)
		{
			num += global::Performance.frameTimeHistory[i];
		}
		return num / (float)num2;
	}

	// Token: 0x02000CA8 RID: 3240
	public struct Tick
	{
		// Token: 0x04004454 RID: 17492
		public int frameID;

		// Token: 0x04004455 RID: 17493
		public int frameRate;

		// Token: 0x04004456 RID: 17494
		public float frameTime;

		// Token: 0x04004457 RID: 17495
		public float frameRateAverage;

		// Token: 0x04004458 RID: 17496
		public float frameTimeAverage;

		// Token: 0x04004459 RID: 17497
		public long memoryUsageSystem;

		// Token: 0x0400445A RID: 17498
		public long memoryAllocations;

		// Token: 0x0400445B RID: 17499
		public long memoryCollections;

		// Token: 0x0400445C RID: 17500
		public long loadBalancerTasks;

		// Token: 0x0400445D RID: 17501
		public long invokeHandlerTasks;

		// Token: 0x0400445E RID: 17502
		public long workshopSkinsQueued;

		// Token: 0x0400445F RID: 17503
		public int ping;

		// Token: 0x04004460 RID: 17504
		public bool gcTriggered;

		// Token: 0x04004461 RID: 17505
		public PerformanceSamplePoint performanceSample;
	}

	// Token: 0x02000CA9 RID: 3241
	private struct LagSpike
	{
		// Token: 0x04004462 RID: 17506
		public int Index;

		// Token: 0x04004463 RID: 17507
		public int Time;
	}
}
