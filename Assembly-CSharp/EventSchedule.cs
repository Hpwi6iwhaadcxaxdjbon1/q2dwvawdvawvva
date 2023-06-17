using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class EventSchedule : BaseMonoBehaviour
{
	// Token: 0x040020A2 RID: 8354
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	// Token: 0x040020A3 RID: 8355
	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;

	// Token: 0x040020A4 RID: 8356
	private float hoursRemaining;

	// Token: 0x040020A5 RID: 8357
	private long lastRun;

	// Token: 0x06002876 RID: 10358 RVA: 0x000F9E54 File Offset: 0x000F8054
	private void OnEnable()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		base.InvokeRepeating(new Action(this.RunSchedule), 1f, 1f);
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000F9E8A File Offset: 0x000F808A
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.RunSchedule));
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000F9EA7 File Offset: 0x000F80A7
	public virtual void RunSchedule()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!ConVar.Server.events)
		{
			return;
		}
		this.CountHours();
		if (this.hoursRemaining > 0f)
		{
			return;
		}
		this.Trigger();
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000F9ED4 File Offset: 0x000F80D4
	private void Trigger()
	{
		this.hoursRemaining = UnityEngine.Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
		TriggeredEvent[] components = base.GetComponents<TriggeredEvent>();
		if (components.Length == 0)
		{
			return;
		}
		TriggeredEvent triggeredEvent = components[UnityEngine.Random.Range(0, components.Length)];
		if (triggeredEvent == null)
		{
			return;
		}
		triggeredEvent.SendMessage("RunEvent", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000F9F28 File Offset: 0x000F8128
	private void CountHours()
	{
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (this.lastRun != 0L)
		{
			TimeSpan timeSpan = TOD_Sky.Instance.Cycle.DateTime.Subtract(DateTime.FromBinary(this.lastRun));
			this.hoursRemaining -= (float)timeSpan.TotalSeconds / 60f / 60f;
		}
		this.lastRun = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
	}
}
