using System;

// Token: 0x020004E4 RID: 1252
public class EventScheduleWipeOffset : EventSchedule
{
	// Token: 0x040020A6 RID: 8358
	[ServerVar(Name = "event_hours_before_wipe")]
	public static float hoursBeforeWipeRealtime = 24f;

	// Token: 0x0600287C RID: 10364 RVA: 0x000F9FCC File Offset: 0x000F81CC
	public override void RunSchedule()
	{
		if (WipeTimer.serverinstance == null)
		{
			return;
		}
		if (WipeTimer.serverinstance.GetTimeSpanUntilWipe().TotalHours > (double)EventScheduleWipeOffset.hoursBeforeWipeRealtime)
		{
			return;
		}
		base.RunSchedule();
	}
}
