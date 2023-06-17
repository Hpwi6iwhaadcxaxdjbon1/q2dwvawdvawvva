using System;
using UnityEngine;

// Token: 0x020004B3 RID: 1203
public class TrainCoupling
{
	// Token: 0x04001FC5 RID: 8133
	public readonly TrainCar owner;

	// Token: 0x04001FC6 RID: 8134
	public readonly bool isFrontCoupling;

	// Token: 0x04001FC7 RID: 8135
	public readonly TrainCouplingController controller;

	// Token: 0x04001FC8 RID: 8136
	public readonly Transform couplingPoint;

	// Token: 0x04001FC9 RID: 8137
	public readonly Transform couplingPivot;

	// Token: 0x04001FCA RID: 8138
	public readonly BaseEntity.Flags flag;

	// Token: 0x04001FCB RID: 8139
	public readonly bool isValid;

	// Token: 0x04001FCD RID: 8141
	public TimeSince timeSinceCouplingBlock;

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06002742 RID: 10050 RVA: 0x000F54EB File Offset: 0x000F36EB
	public bool IsCoupled
	{
		get
		{
			return this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06002743 RID: 10051 RVA: 0x000F54FE File Offset: 0x000F36FE
	public bool IsUncoupled
	{
		get
		{
			return !this.owner.HasFlag(this.flag);
		}
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000F5514 File Offset: 0x000F3714
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller) : this(owner, isFrontCoupling, controller, null, null, BaseEntity.Flags.Placeholder)
	{
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000F5524 File Offset: 0x000F3724
	public TrainCoupling(TrainCar owner, bool isFrontCoupling, TrainCouplingController controller, Transform couplingPoint, Transform couplingPivot, BaseEntity.Flags flag)
	{
		this.owner = owner;
		this.isFrontCoupling = isFrontCoupling;
		this.controller = controller;
		this.couplingPoint = couplingPoint;
		this.couplingPivot = couplingPivot;
		this.flag = flag;
		this.isValid = (couplingPoint != null);
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06002746 RID: 10054 RVA: 0x000F5572 File Offset: 0x000F3772
	// (set) Token: 0x06002747 RID: 10055 RVA: 0x000F557A File Offset: 0x000F377A
	public TrainCoupling CoupledTo { get; private set; }

	// Token: 0x06002748 RID: 10056 RVA: 0x000F5583 File Offset: 0x000F3783
	public bool IsCoupledTo(TrainCar them)
	{
		return this.CoupledTo != null && this.CoupledTo.owner == them;
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x000F55A0 File Offset: 0x000F37A0
	public bool IsCoupledTo(TrainCoupling them)
	{
		return this.CoupledTo != null && this.CoupledTo == them;
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000F55B8 File Offset: 0x000F37B8
	public bool TryCouple(TrainCoupling theirCoupling, bool reflect)
	{
		if (!this.isValid)
		{
			return false;
		}
		if (this.CoupledTo == theirCoupling)
		{
			return true;
		}
		if (this.IsCoupled)
		{
			return false;
		}
		if (reflect && !theirCoupling.TryCouple(this, false))
		{
			return false;
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = theirCoupling;
		this.owner.SetFlag(this.flag, true, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000F5628 File Offset: 0x000F3828
	public void Uncouple(bool reflect)
	{
		if (this.IsUncoupled)
		{
			return;
		}
		if (reflect && this.CoupledTo != null)
		{
			this.CoupledTo.Uncouple(false);
		}
		this.controller.OnPreCouplingChange();
		this.CoupledTo = null;
		this.owner.SetFlag(this.flag, false, false, false);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.timeSinceCouplingBlock = 0f;
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000F5697 File Offset: 0x000F3897
	public TrainCoupling GetOppositeCoupling()
	{
		if (!this.isFrontCoupling)
		{
			return this.controller.frontCoupling;
		}
		return this.controller.rearCoupling;
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000F56B8 File Offset: 0x000F38B8
	public bool TryGetCoupledToID(out NetworkableId id)
	{
		if (this.CoupledTo != null && this.CoupledTo.owner != null && this.CoupledTo.owner.IsValid())
		{
			id = this.CoupledTo.owner.net.ID;
			return true;
		}
		id = default(NetworkableId);
		return false;
	}
}
