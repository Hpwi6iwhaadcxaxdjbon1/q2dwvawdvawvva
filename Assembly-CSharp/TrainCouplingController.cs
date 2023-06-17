using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class TrainCouplingController
{
	// Token: 0x04001FD0 RID: 8144
	public const BaseEntity.Flags Flag_CouplingFront = BaseEntity.Flags.Reserved2;

	// Token: 0x04001FD1 RID: 8145
	public const BaseEntity.Flags Flag_CouplingRear = BaseEntity.Flags.Reserved3;

	// Token: 0x04001FD2 RID: 8146
	public readonly TrainCoupling frontCoupling;

	// Token: 0x04001FD3 RID: 8147
	public readonly TrainCoupling rearCoupling;

	// Token: 0x04001FD4 RID: 8148
	private readonly TrainCar owner;

	// Token: 0x04001FD5 RID: 8149
	[ServerVar(Help = "Maximum difference in velocity for train cars to couple")]
	public static float max_couple_speed = 9f;

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x0600274E RID: 10062 RVA: 0x000F5717 File Offset: 0x000F3917
	public bool IsCoupled
	{
		get
		{
			return this.IsFrontCoupled || this.IsRearCoupled;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x0600274F RID: 10063 RVA: 0x000F5729 File Offset: 0x000F3929
	public bool IsFrontCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000353 RID: 851
	// (get) Token: 0x06002750 RID: 10064 RVA: 0x000F573B File Offset: 0x000F393B
	public bool IsRearCoupled
	{
		get
		{
			return this.owner.HasFlag(BaseEntity.Flags.Reserved3);
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06002751 RID: 10065 RVA: 0x000F574D File Offset: 0x000F394D
	// (set) Token: 0x06002752 RID: 10066 RVA: 0x000F5755 File Offset: 0x000F3955
	public float PreChangeTrackSpeed { get; private set; }

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06002753 RID: 10067 RVA: 0x000F575E File Offset: 0x000F395E
	// (set) Token: 0x06002754 RID: 10068 RVA: 0x000F5766 File Offset: 0x000F3966
	public bool PreChangeCoupledBackwards { get; private set; }

	// Token: 0x06002755 RID: 10069 RVA: 0x000F5770 File Offset: 0x000F3970
	public TrainCouplingController(TrainCar owner)
	{
		this.owner = owner;
		this.frontCoupling = new TrainCoupling(owner, true, this, owner.frontCoupling, owner.frontCouplingPivot, BaseEntity.Flags.Reserved2);
		this.rearCoupling = new TrainCoupling(owner, false, this, owner.rearCoupling, owner.rearCouplingPivot, BaseEntity.Flags.Reserved3);
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000F57C8 File Offset: 0x000F39C8
	public bool IsCoupledTo(TrainCar them)
	{
		return this.frontCoupling.IsCoupledTo(them) || this.rearCoupling.IsCoupledTo(them);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000F57E8 File Offset: 0x000F39E8
	public bool TryCouple(TrainCar them, TriggerTrainCollisions.Location ourLocation)
	{
		TrainCoupling trainCoupling = (ourLocation == TriggerTrainCollisions.Location.Front) ? this.frontCoupling : this.rearCoupling;
		if (!trainCoupling.isValid)
		{
			return false;
		}
		if (trainCoupling.IsCoupled)
		{
			return false;
		}
		if (trainCoupling.timeSinceCouplingBlock < 1.5f)
		{
			return false;
		}
		float num = Vector3.Angle(this.owner.transform.forward, them.transform.forward);
		if (num > 25f && num < 155f)
		{
			return false;
		}
		bool flag = num < 90f;
		TrainCoupling trainCoupling2;
		if (flag)
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.rearCoupling : them.coupling.frontCoupling);
		}
		else
		{
			trainCoupling2 = ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.frontCoupling : them.coupling.rearCoupling);
		}
		float num2 = them.GetTrackSpeed();
		if (!flag)
		{
			num2 = -num2;
		}
		if (Mathf.Abs(num2 - this.owner.GetTrackSpeed()) > TrainCouplingController.max_couple_speed)
		{
			trainCoupling.timeSinceCouplingBlock = 0f;
			trainCoupling2.timeSinceCouplingBlock = 0f;
			return false;
		}
		if (!trainCoupling2.isValid)
		{
			return false;
		}
		if (Vector3.SqrMagnitude(trainCoupling.couplingPoint.position - trainCoupling2.couplingPoint.position) > 0.5f)
		{
			return false;
		}
		TrainTrackSpline frontTrackSection = this.owner.FrontTrackSection;
		TrainTrackSpline frontTrackSection2 = them.FrontTrackSection;
		return (!(frontTrackSection2 != frontTrackSection) || frontTrackSection.HasConnectedTrack(frontTrackSection2)) && trainCoupling.TryCouple(trainCoupling2, true);
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000F5956 File Offset: 0x000F3B56
	public void Uncouple(bool front)
	{
		if (front)
		{
			this.frontCoupling.Uncouple(true);
			return;
		}
		this.rearCoupling.Uncouple(true);
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000F5974 File Offset: 0x000F3B74
	public void GetAll(ref List<TrainCar> result)
	{
		result.Add(this.owner);
		TrainCoupling trainCoupling = this.rearCoupling.CoupledTo;
		while (trainCoupling != null && trainCoupling.IsCoupled && !result.Contains(trainCoupling.owner))
		{
			result.Insert(0, trainCoupling.owner);
			trainCoupling = trainCoupling.GetOppositeCoupling();
			trainCoupling = trainCoupling.CoupledTo;
		}
		TrainCoupling trainCoupling2 = this.frontCoupling.CoupledTo;
		while (trainCoupling2 != null && trainCoupling2.IsCoupled && !result.Contains(trainCoupling2.owner))
		{
			result.Add(trainCoupling2.owner);
			trainCoupling2 = trainCoupling2.GetOppositeCoupling();
			trainCoupling2 = trainCoupling2.CoupledTo;
		}
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000F5A15 File Offset: 0x000F3C15
	public void OnPreCouplingChange()
	{
		this.PreChangeCoupledBackwards = this.owner.IsCoupledBackwards();
		this.PreChangeTrackSpeed = this.owner.GetTrackSpeed();
		if (this.PreChangeCoupledBackwards)
		{
			this.PreChangeTrackSpeed = -this.PreChangeTrackSpeed;
		}
	}
}
