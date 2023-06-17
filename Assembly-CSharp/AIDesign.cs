using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x0200035E RID: 862
public class AIDesign
{
	// Token: 0x040018E4 RID: 6372
	public List<AIState> AvailableStates = new List<AIState>();

	// Token: 0x040018E5 RID: 6373
	public int DefaultStateContainerID;

	// Token: 0x040018E6 RID: 6374
	private Dictionary<int, global::AIStateContainer> stateContainers = new Dictionary<int, global::AIStateContainer>();

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001F71 RID: 8049 RVA: 0x000D489D File Offset: 0x000D2A9D
	// (set) Token: 0x06001F72 RID: 8050 RVA: 0x000D48A5 File Offset: 0x000D2AA5
	public AIDesignScope Scope { get; private set; }

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06001F73 RID: 8051 RVA: 0x000D48AE File Offset: 0x000D2AAE
	// (set) Token: 0x06001F74 RID: 8052 RVA: 0x000D48B6 File Offset: 0x000D2AB6
	public string Description { get; private set; }

	// Token: 0x06001F75 RID: 8053 RVA: 0x000D48BF File Offset: 0x000D2ABF
	public void SetAvailableStates(List<AIState> states)
	{
		this.AvailableStates = new List<AIState>();
		this.AvailableStates.AddRange(states);
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000D48D8 File Offset: 0x000D2AD8
	public void Load(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.Scope = (AIDesignScope)design.scope;
		this.DefaultStateContainerID = design.defaultStateContainer;
		this.Description = design.description;
		this.InitStateContainers(design, owner);
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000D4908 File Offset: 0x000D2B08
	private void InitStateContainers(ProtoBuf.AIDesign design, global::BaseEntity owner)
	{
		this.stateContainers = new Dictionary<int, global::AIStateContainer>();
		if (design.stateContainers == null)
		{
			return;
		}
		foreach (ProtoBuf.AIStateContainer container in design.stateContainers)
		{
			global::AIStateContainer aistateContainer = new global::AIStateContainer();
			aistateContainer.Init(container, owner);
			this.stateContainers.Add(aistateContainer.ID, aistateContainer);
		}
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000D4988 File Offset: 0x000D2B88
	public global::AIStateContainer GetDefaultStateContainer()
	{
		return this.GetStateContainerByID(this.DefaultStateContainerID);
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000D4996 File Offset: 0x000D2B96
	public global::AIStateContainer GetStateContainerByID(int id)
	{
		if (!this.stateContainers.ContainsKey(id))
		{
			return null;
		}
		return this.stateContainers[id];
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000D49B4 File Offset: 0x000D2BB4
	public global::AIStateContainer GetFirstStateContainerOfType(AIState stateType)
	{
		foreach (global::AIStateContainer aistateContainer in this.stateContainers.Values)
		{
			if (aistateContainer.State == stateType)
			{
				return aistateContainer;
			}
		}
		return null;
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000D4A18 File Offset: 0x000D2C18
	public ProtoBuf.AIDesign ToProto(int currentStateID)
	{
		ProtoBuf.AIDesign aidesign = new ProtoBuf.AIDesign();
		aidesign.description = this.Description;
		aidesign.scope = (int)this.Scope;
		aidesign.defaultStateContainer = this.DefaultStateContainerID;
		aidesign.availableStates = new List<int>();
		foreach (AIState item in this.AvailableStates)
		{
			aidesign.availableStates.Add((int)item);
		}
		aidesign.stateContainers = new List<ProtoBuf.AIStateContainer>();
		foreach (global::AIStateContainer aistateContainer in this.stateContainers.Values)
		{
			aidesign.stateContainers.Add(aistateContainer.ToProto());
		}
		aidesign.intialViewStateID = currentStateID;
		return aidesign;
	}
}
