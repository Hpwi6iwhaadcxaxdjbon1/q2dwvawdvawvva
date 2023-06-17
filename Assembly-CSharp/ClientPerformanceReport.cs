using System;

// Token: 0x02000315 RID: 789
public struct ClientPerformanceReport
{
	// Token: 0x040017C5 RID: 6085
	public int request_id;

	// Token: 0x040017C6 RID: 6086
	public string user_id;

	// Token: 0x040017C7 RID: 6087
	public float fps_average;

	// Token: 0x040017C8 RID: 6088
	public int fps;

	// Token: 0x040017C9 RID: 6089
	public int frame_id;

	// Token: 0x040017CA RID: 6090
	public float frame_time;

	// Token: 0x040017CB RID: 6091
	public float frame_time_average;

	// Token: 0x040017CC RID: 6092
	public long memory_system;

	// Token: 0x040017CD RID: 6093
	public long memory_collections;

	// Token: 0x040017CE RID: 6094
	public long memory_managed_heap;

	// Token: 0x040017CF RID: 6095
	public float realtime_since_startup;

	// Token: 0x040017D0 RID: 6096
	public bool streamer_mode;

	// Token: 0x040017D1 RID: 6097
	public int ping;

	// Token: 0x040017D2 RID: 6098
	public int tasks_invokes;

	// Token: 0x040017D3 RID: 6099
	public int tasks_load_balancer;

	// Token: 0x040017D4 RID: 6100
	public int workshop_skins_queued;
}
