using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000361 RID: 865
public class AIEvents
{
	// Token: 0x04001905 RID: 6405
	public AIMemory Memory = new AIMemory();

	// Token: 0x04001907 RID: 6407
	private List<BaseAIEvent> events = new List<BaseAIEvent>();

	// Token: 0x04001908 RID: 6408
	private IAIEventListener eventListener;

	// Token: 0x04001909 RID: 6409
	private AIBrainSenses senses;

	// Token: 0x0400190A RID: 6410
	private int currentEventIndex;

	// Token: 0x0400190B RID: 6411
	private bool inBlock;

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06001F81 RID: 8065 RVA: 0x000D4C20 File Offset: 0x000D2E20
	// (set) Token: 0x06001F82 RID: 8066 RVA: 0x000D4C28 File Offset: 0x000D2E28
	public int CurrentInputMemorySlot { get; private set; } = -1;

	// Token: 0x06001F83 RID: 8067 RVA: 0x000D4C34 File Offset: 0x000D2E34
	public void Init(IAIEventListener listener, AIStateContainer stateContainer, BaseEntity owner, AIBrainSenses senses)
	{
		this.CurrentInputMemorySlot = stateContainer.InputMemorySlot;
		this.eventListener = listener;
		this.RemoveAll();
		this.AddStateEvents(stateContainer.Events, owner);
		this.Memory.Entity.Set(owner, 4);
		this.senses = senses;
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000D4C81 File Offset: 0x000D2E81
	private void RemoveAll()
	{
		this.events.Clear();
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000D4C90 File Offset: 0x000D2E90
	private void AddStateEvents(List<BaseAIEvent> events, BaseEntity owner)
	{
		foreach (BaseAIEvent aiEvent in events)
		{
			this.Add(aiEvent);
		}
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000D4CE0 File Offset: 0x000D2EE0
	private void Add(BaseAIEvent aiEvent)
	{
		if (this.events.Contains(aiEvent))
		{
			Debug.LogWarning("Attempting to add duplicate AI event: " + aiEvent.EventType);
			return;
		}
		aiEvent.Reset();
		this.events.Add(aiEvent);
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000D4D20 File Offset: 0x000D2F20
	public void Tick(float deltaTime, StateStatus stateStatus)
	{
		foreach (BaseAIEvent baseAIEvent in this.events)
		{
			baseAIEvent.Tick(deltaTime, this.eventListener);
		}
		this.inBlock = false;
		this.currentEventIndex = 0;
		this.currentEventIndex = 0;
		while (this.currentEventIndex < this.events.Count)
		{
			BaseAIEvent baseAIEvent2 = this.events[this.currentEventIndex];
			BaseAIEvent baseAIEvent3 = (this.currentEventIndex < this.events.Count - 1) ? this.events[this.currentEventIndex + 1] : null;
			if (baseAIEvent3 != null && baseAIEvent3.EventType == AIEventType.And && !this.inBlock)
			{
				this.inBlock = true;
			}
			if (baseAIEvent2.EventType != AIEventType.And)
			{
				if (baseAIEvent2.ShouldExecute)
				{
					baseAIEvent2.Execute(this.Memory, this.senses, stateStatus);
					baseAIEvent2.PostExecute();
				}
				bool result = baseAIEvent2.Result;
				if (this.inBlock)
				{
					if (result)
					{
						if ((baseAIEvent3 != null && baseAIEvent3.EventType != AIEventType.And) || baseAIEvent3 == null)
						{
							this.inBlock = false;
							if (baseAIEvent2.HasValidTriggerState)
							{
								baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
								return;
							}
						}
					}
					else
					{
						this.inBlock = false;
						this.currentEventIndex = this.FindNextEventBlock() - 1;
					}
				}
				else if (result && baseAIEvent2.HasValidTriggerState)
				{
					baseAIEvent2.TriggerStateChange(this.eventListener, baseAIEvent2.ID);
					return;
				}
			}
			this.currentEventIndex++;
		}
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000D4EB8 File Offset: 0x000D30B8
	private int FindNextEventBlock()
	{
		for (int i = this.currentEventIndex; i < this.events.Count; i++)
		{
			BaseAIEvent baseAIEvent = this.events[i];
			BaseAIEvent baseAIEvent2 = (i < this.events.Count - 1) ? this.events[i + 1] : null;
			if (baseAIEvent2 != null && baseAIEvent2.EventType != AIEventType.And && baseAIEvent.EventType != AIEventType.And)
			{
				return i + 1;
			}
		}
		return this.events.Count + 1;
	}
}
