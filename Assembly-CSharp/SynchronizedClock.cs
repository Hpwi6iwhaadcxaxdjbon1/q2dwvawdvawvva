using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200094E RID: 2382
public class SynchronizedClock
{
	// Token: 0x0400337A RID: 13178
	public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06003940 RID: 14656 RVA: 0x000E4933 File Offset: 0x000E2B33
	private static float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x00154A68 File Offset: 0x00152C68
	public void Add(float delta, float variance, Action<uint> action)
	{
		SynchronizedClock.TimedEvent item = default(SynchronizedClock.TimedEvent);
		item.time = SynchronizedClock.CurrentTime;
		item.delta = delta;
		item.variance = variance;
		item.action = action;
		this.events.Add(item);
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x00154AB0 File Offset: 0x00152CB0
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			SynchronizedClock.TimedEvent timedEvent = this.events[i];
			float time = timedEvent.time;
			float currentTime = SynchronizedClock.CurrentTime;
			float delta = timedEvent.delta;
			float num = time - time % delta;
			uint obj = (uint)(time / delta);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			SeedRandom.Wanghash(ref obj);
			float num2 = SeedRandom.Range(ref obj, -timedEvent.variance, timedEvent.variance);
			float num3 = num + delta + num2;
			if (time < num3 && currentTime >= num3)
			{
				timedEvent.action(obj);
				timedEvent.time = currentTime;
			}
			else if (currentTime > time || currentTime < num - 5f)
			{
				timedEvent.time = currentTime;
			}
			this.events[i] = timedEvent;
		}
	}

	// Token: 0x02000EC0 RID: 3776
	public struct TimedEvent
	{
		// Token: 0x04004CD4 RID: 19668
		public float time;

		// Token: 0x04004CD5 RID: 19669
		public float delta;

		// Token: 0x04004CD6 RID: 19670
		public float variance;

		// Token: 0x04004CD7 RID: 19671
		public Action<uint> action;
	}
}
