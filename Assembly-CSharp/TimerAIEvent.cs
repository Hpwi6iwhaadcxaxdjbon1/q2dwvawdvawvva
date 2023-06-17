using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class TimerAIEvent : BaseAIEvent
{
	// Token: 0x04001940 RID: 6464
	protected float currentDuration;

	// Token: 0x04001941 RID: 6465
	protected float elapsedDuration;

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06002013 RID: 8211 RVA: 0x000D64B9 File Offset: 0x000D46B9
	// (set) Token: 0x06002014 RID: 8212 RVA: 0x000D64C1 File Offset: 0x000D46C1
	public float DurationMin { get; set; }

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x06002015 RID: 8213 RVA: 0x000D64CA File Offset: 0x000D46CA
	// (set) Token: 0x06002016 RID: 8214 RVA: 0x000D64D2 File Offset: 0x000D46D2
	public float DurationMax { get; set; }

	// Token: 0x06002017 RID: 8215 RVA: 0x000D64DB File Offset: 0x000D46DB
	public TimerAIEvent() : base(AIEventType.Timer)
	{
		base.Rate = BaseAIEvent.ExecuteRate.Fast;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x000D64EC File Offset: 0x000D46EC
	public override void Init(AIEventData data, global::BaseEntity owner)
	{
		base.Init(data, owner);
		TimerAIEventData timerData = data.timerData;
		this.DurationMin = timerData.duration;
		this.DurationMax = timerData.durationMax;
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000D6520 File Offset: 0x000D4720
	public override AIEventData ToProto()
	{
		AIEventData aieventData = base.ToProto();
		aieventData.timerData = new TimerAIEventData();
		aieventData.timerData.duration = this.DurationMin;
		aieventData.timerData.durationMax = this.DurationMax;
		return aieventData;
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000D6555 File Offset: 0x000D4755
	public override void Reset()
	{
		base.Reset();
		this.currentDuration = UnityEngine.Random.Range(this.DurationMin, this.DurationMax);
		this.elapsedDuration = 0f;
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000D657F File Offset: 0x000D477F
	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		this.elapsedDuration += this.deltaTime;
		if (this.elapsedDuration >= this.currentDuration)
		{
			base.Result = !base.Inverted;
		}
	}
}
