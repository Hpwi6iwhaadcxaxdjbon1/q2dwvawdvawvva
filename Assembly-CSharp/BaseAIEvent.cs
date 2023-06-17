using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class BaseAIEvent
{
	// Token: 0x0400192F RID: 6447
	private float executeTimer;

	// Token: 0x04001930 RID: 6448
	protected float deltaTime;

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06001FA0 RID: 8096 RVA: 0x000D534D File Offset: 0x000D354D
	// (set) Token: 0x06001FA1 RID: 8097 RVA: 0x000D5355 File Offset: 0x000D3555
	public AIEventType EventType { get; private set; }

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06001FA2 RID: 8098 RVA: 0x000D535E File Offset: 0x000D355E
	// (set) Token: 0x06001FA3 RID: 8099 RVA: 0x000D5366 File Offset: 0x000D3566
	public int TriggerStateContainerID { get; private set; } = -1;

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001FA4 RID: 8100 RVA: 0x000D536F File Offset: 0x000D356F
	// (set) Token: 0x06001FA5 RID: 8101 RVA: 0x000D5377 File Offset: 0x000D3577
	public BaseAIEvent.ExecuteRate Rate { get; protected set; } = BaseAIEvent.ExecuteRate.Normal;

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001FA6 RID: 8102 RVA: 0x000D5380 File Offset: 0x000D3580
	public float ExecutionRate
	{
		get
		{
			switch (this.Rate)
			{
			case BaseAIEvent.ExecuteRate.Slow:
				return 1f;
			case BaseAIEvent.ExecuteRate.Normal:
				return 0.5f;
			case BaseAIEvent.ExecuteRate.Fast:
				return 0.25f;
			case BaseAIEvent.ExecuteRate.VeryFast:
				return 0.1f;
			default:
				return 0.5f;
			}
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001FA7 RID: 8103 RVA: 0x000D53C9 File Offset: 0x000D35C9
	// (set) Token: 0x06001FA8 RID: 8104 RVA: 0x000D53D1 File Offset: 0x000D35D1
	public bool ShouldExecute { get; protected set; }

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001FA9 RID: 8105 RVA: 0x000D53DA File Offset: 0x000D35DA
	// (set) Token: 0x06001FAA RID: 8106 RVA: 0x000D53E2 File Offset: 0x000D35E2
	public bool Result { get; protected set; }

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06001FAB RID: 8107 RVA: 0x000D53EB File Offset: 0x000D35EB
	// (set) Token: 0x06001FAC RID: 8108 RVA: 0x000D53F3 File Offset: 0x000D35F3
	public bool Inverted { get; private set; }

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06001FAD RID: 8109 RVA: 0x000D53FC File Offset: 0x000D35FC
	// (set) Token: 0x06001FAE RID: 8110 RVA: 0x000D5404 File Offset: 0x000D3604
	public int OutputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06001FAF RID: 8111 RVA: 0x000D540D File Offset: 0x000D360D
	public bool ShouldSetOutputEntityMemory
	{
		get
		{
			return this.OutputEntityMemorySlot > -1;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06001FB0 RID: 8112 RVA: 0x000D5418 File Offset: 0x000D3618
	// (set) Token: 0x06001FB1 RID: 8113 RVA: 0x000D5420 File Offset: 0x000D3620
	public int InputEntityMemorySlot { get; protected set; } = -1;

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06001FB2 RID: 8114 RVA: 0x000D5429 File Offset: 0x000D3629
	// (set) Token: 0x06001FB3 RID: 8115 RVA: 0x000D5431 File Offset: 0x000D3631
	public int ID { get; protected set; }

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001FB4 RID: 8116 RVA: 0x000D543A File Offset: 0x000D363A
	// (set) Token: 0x06001FB5 RID: 8117 RVA: 0x000D5442 File Offset: 0x000D3642
	public global::BaseEntity Owner { get; private set; }

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000D544B File Offset: 0x000D364B
	public bool HasValidTriggerState
	{
		get
		{
			return this.TriggerStateContainerID != -1;
		}
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000D5459 File Offset: 0x000D3659
	public BaseAIEvent(AIEventType type)
	{
		this.EventType = type;
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000D5484 File Offset: 0x000D3684
	public virtual void Init(AIEventData data, global::BaseEntity owner)
	{
		this.Init(data.triggerStateContainer, data.id, owner, data.inputMemorySlot, data.outputMemorySlot, data.inverted);
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000D54AB File Offset: 0x000D36AB
	public virtual void Init(int triggerStateContainer, int id, global::BaseEntity owner, int inputMemorySlot, int outputMemorySlot, bool inverted)
	{
		this.TriggerStateContainerID = triggerStateContainer;
		this.ID = id;
		this.Owner = owner;
		this.InputEntityMemorySlot = inputMemorySlot;
		this.OutputEntityMemorySlot = outputMemorySlot;
		this.Inverted = inverted;
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000D54DC File Offset: 0x000D36DC
	public virtual AIEventData ToProto()
	{
		return new AIEventData
		{
			id = this.ID,
			eventType = (int)this.EventType,
			triggerStateContainer = this.TriggerStateContainerID,
			outputMemorySlot = this.OutputEntityMemorySlot,
			inputMemorySlot = this.InputEntityMemorySlot,
			inverted = this.Inverted
		};
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000D5536 File Offset: 0x000D3736
	public virtual void Reset()
	{
		this.executeTimer = 0f;
		this.deltaTime = 0f;
		this.Result = false;
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000D5558 File Offset: 0x000D3758
	public void Tick(float deltaTime, IAIEventListener listener)
	{
		this.deltaTime += deltaTime;
		this.executeTimer += deltaTime;
		float executionRate = this.ExecutionRate;
		if (this.executeTimer >= executionRate)
		{
			this.executeTimer = 0f;
			this.ShouldExecute = true;
			return;
		}
		this.ShouldExecute = false;
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000D55AB File Offset: 0x000D37AB
	public virtual void PostExecute()
	{
		this.deltaTime = 0f;
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000D55B8 File Offset: 0x000D37B8
	public void TriggerStateChange(IAIEventListener listener, int sourceEventID)
	{
		listener.EventTriggeredStateChange(this.TriggerStateContainerID, sourceEventID);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000D55C8 File Offset: 0x000D37C8
	public static BaseAIEvent CreateEvent(AIEventType eventType)
	{
		switch (eventType)
		{
		case AIEventType.Timer:
			return new TimerAIEvent();
		case AIEventType.PlayerDetected:
			return new PlayerDetectedAIEvent();
		case AIEventType.StateError:
			return new StateErrorAIEvent();
		case AIEventType.Attacked:
			return new AttackedAIEvent();
		case AIEventType.StateFinished:
			return new StateFinishedAIEvent();
		case AIEventType.InAttackRange:
			return new InAttackRangeAIEvent();
		case AIEventType.HealthBelow:
			return new HealthBelowAIEvent();
		case AIEventType.InRange:
			return new InRangeAIEvent();
		case AIEventType.PerformedAttack:
			return new PerformedAttackAIEvent();
		case AIEventType.TirednessAbove:
			return new TirednessAboveAIEvent();
		case AIEventType.HungerAbove:
			return new HungerAboveAIEvent();
		case AIEventType.ThreatDetected:
			return new ThreatDetectedAIEvent();
		case AIEventType.TargetDetected:
			return new TargetDetectedAIEvent();
		case AIEventType.AmmoBelow:
			return new AmmoBelowAIEvent();
		case AIEventType.BestTargetDetected:
			return new BestTargetDetectedAIEvent();
		case AIEventType.IsVisible:
			return new IsVisibleAIEvent();
		case AIEventType.AttackTick:
			return new AttackTickAIEvent();
		case AIEventType.IsMounted:
			return new IsMountedAIEvent();
		case AIEventType.And:
			return new AndAIEvent();
		case AIEventType.Chance:
			return new ChanceAIEvent();
		case AIEventType.TargetLost:
			return new TargetLostAIEvent();
		case AIEventType.TimeSinceThreat:
			return new TimeSinceThreatAIEvent();
		case AIEventType.OnPositionMemorySet:
			return new OnPositionMemorySetAIEvent();
		case AIEventType.AggressionTimer:
			return new AggressionTimerAIEvent();
		case AIEventType.Reloading:
			return new ReloadingAIEvent();
		case AIEventType.InRangeOfHome:
			return new InRangeOfHomeAIEvent();
		case AIEventType.IsBlinded:
			return new IsBlindedAIEvent();
		default:
			Debug.LogWarning("No case for " + eventType + " event in BaseAIEvent.CreateEvent()!");
			return null;
		}
	}

	// Token: 0x02000CB1 RID: 3249
	public enum ExecuteRate
	{
		// Token: 0x0400447F RID: 17535
		Slow,
		// Token: 0x04004480 RID: 17536
		Normal,
		// Token: 0x04004481 RID: 17537
		Fast,
		// Token: 0x04004482 RID: 17538
		VeryFast
	}
}
