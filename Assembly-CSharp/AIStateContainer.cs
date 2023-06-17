using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x02000389 RID: 905
public class AIStateContainer
{
	// Token: 0x04001945 RID: 6469
	public List<BaseAIEvent> Events;

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06002029 RID: 8233 RVA: 0x000D667C File Offset: 0x000D487C
	// (set) Token: 0x0600202A RID: 8234 RVA: 0x000D6684 File Offset: 0x000D4884
	public int ID { get; private set; }

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x0600202B RID: 8235 RVA: 0x000D668D File Offset: 0x000D488D
	// (set) Token: 0x0600202C RID: 8236 RVA: 0x000D6695 File Offset: 0x000D4895
	public AIState State { get; private set; }

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x0600202D RID: 8237 RVA: 0x000D669E File Offset: 0x000D489E
	// (set) Token: 0x0600202E RID: 8238 RVA: 0x000D66A6 File Offset: 0x000D48A6
	public int InputMemorySlot { get; private set; } = -1;

	// Token: 0x0600202F RID: 8239 RVA: 0x000D66B0 File Offset: 0x000D48B0
	public void Init(ProtoBuf.AIStateContainer container, global::BaseEntity owner)
	{
		this.ID = container.id;
		this.State = (AIState)container.state;
		this.InputMemorySlot = container.inputMemorySlot;
		this.Events = new List<BaseAIEvent>();
		if (container.events == null)
		{
			return;
		}
		foreach (AIEventData aieventData in container.events)
		{
			BaseAIEvent baseAIEvent = BaseAIEvent.CreateEvent((AIEventType)aieventData.eventType);
			baseAIEvent.Init(aieventData, owner);
			baseAIEvent.Reset();
			this.Events.Add(baseAIEvent);
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000D675C File Offset: 0x000D495C
	public ProtoBuf.AIStateContainer ToProto()
	{
		ProtoBuf.AIStateContainer aistateContainer = new ProtoBuf.AIStateContainer();
		aistateContainer.id = this.ID;
		aistateContainer.state = (int)this.State;
		aistateContainer.events = new List<AIEventData>();
		aistateContainer.inputMemorySlot = this.InputMemorySlot;
		foreach (BaseAIEvent baseAIEvent in this.Events)
		{
			aistateContainer.events.Add(baseAIEvent.ToProto());
		}
		return aistateContainer;
	}
}
