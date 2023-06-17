using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000940 RID: 2368
public class LocalClock
{
	// Token: 0x0400334D RID: 13133
	public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

	// Token: 0x060038B5 RID: 14517 RVA: 0x001524F0 File Offset: 0x001506F0
	public void Add(float delta, float variance, Action action)
	{
		LocalClock.TimedEvent item = default(LocalClock.TimedEvent);
		item.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
		item.delta = delta;
		item.variance = variance;
		item.action = action;
		this.events.Add(item);
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x00152540 File Offset: 0x00150740
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.events[i];
			if (Time.time > timedEvent.time)
			{
				float delta = timedEvent.delta;
				float variance = timedEvent.variance;
				timedEvent.action();
				timedEvent.time = Time.time + delta + UnityEngine.Random.Range(-variance, variance);
				this.events[i] = timedEvent;
			}
		}
	}

	// Token: 0x02000EBD RID: 3773
	public struct TimedEvent
	{
		// Token: 0x04004CC9 RID: 19657
		public float time;

		// Token: 0x04004CCA RID: 19658
		public float delta;

		// Token: 0x04004CCB RID: 19659
		public float variance;

		// Token: 0x04004CCC RID: 19660
		public Action action;
	}
}
