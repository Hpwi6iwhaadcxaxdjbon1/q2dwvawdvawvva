using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class ElevatorStatic : Elevator
{
	// Token: 0x04000E69 RID: 3689
	public bool StaticTop;

	// Token: 0x04000E6A RID: 3690
	private const BaseEntity.Flags LiftRecentlyArrived = BaseEntity.Flags.Reserved3;

	// Token: 0x04000E6B RID: 3691
	private List<ElevatorStatic> floorPositions = new List<ElevatorStatic>();

	// Token: 0x04000E6C RID: 3692
	private ElevatorStatic ownerElevator;

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060015FB RID: 5627 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool IsStatic
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x000AC64C File Offset: 0x000AA84C
	public override void Spawn()
	{
		base.Spawn();
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, this.StaticTop, false, true);
		if (!base.IsTop)
		{
			return;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(base.transform.position, -Vector3.up), 0f, list, 200f, 262144, QueryTriggerInteraction.Collide, null);
		foreach (RaycastHit raycastHit in list)
		{
			if (raycastHit.transform.parent != null)
			{
				ElevatorStatic component = raycastHit.transform.parent.GetComponent<ElevatorStatic>();
				if (component != null && component != this && component.isServer)
				{
					this.floorPositions.Add(component);
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		this.floorPositions.Reverse();
		base.Floor = this.floorPositions.Count;
		for (int i = 0; i < this.floorPositions.Count; i++)
		{
			this.floorPositions[i].SetFloorDetails(i, this);
		}
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x000AC798 File Offset: 0x000AA998
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		base.UpdateChildEntities(base.IsTop);
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x000AC7AC File Offset: 0x000AA9AC
	protected override bool IsValidFloor(int targetFloor)
	{
		return targetFloor >= 0 && targetFloor <= base.Floor;
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x000AC7C0 File Offset: 0x000AA9C0
	protected override Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		if (targetFloor == base.Floor)
		{
			return base.transform.position + Vector3.up * 1f;
		}
		Vector3 position = base.transform.position;
		position.y = this.floorPositions[targetFloor].transform.position.y + 1f;
		return position;
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000AC82B File Offset: 0x000AAA2B
	public void SetFloorDetails(int floor, ElevatorStatic owner)
	{
		this.ownerElevator = owner;
		base.Floor = floor;
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000AC83C File Offset: 0x000AAA3C
	protected override void CallElevator()
	{
		if (this.ownerElevator != null)
		{
			float num;
			this.ownerElevator.RequestMoveLiftTo(base.Floor, out num, this);
			return;
		}
		if (base.IsTop)
		{
			float num2;
			base.RequestMoveLiftTo(base.Floor, out num2, this);
		}
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000AC885 File Offset: 0x000AAA85
	private ElevatorStatic ElevatorAtFloor(int floor)
	{
		if (floor == base.Floor)
		{
			return this;
		}
		if (floor >= 0 && floor < this.floorPositions.Count)
		{
			return this.floorPositions[floor];
		}
		return null;
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x000AC8B2 File Offset: 0x000AAAB2
	protected override void OpenDoorsAtFloor(int floor)
	{
		base.OpenDoorsAtFloor(floor);
		if (floor == this.floorPositions.Count)
		{
			this.OpenLiftDoors();
			return;
		}
		this.floorPositions[floor].OpenLiftDoors();
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x000AC8E4 File Offset: 0x000AAAE4
	protected override void OnMoveBegin()
	{
		base.OnMoveBegin();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftLeavingFloor();
		}
		base.NotifyLiftEntityDoorsOpen(false);
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x000AC91A File Offset: 0x000AAB1A
	private void OnLiftLeavingFloor()
	{
		this.ClearPowerOutput();
		if (base.IsInvoking(new Action(this.ClearPowerOutput)))
		{
			base.CancelInvoke(new Action(this.ClearPowerOutput));
		}
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000AC948 File Offset: 0x000AAB48
	protected override void ClearBusy()
	{
		base.ClearBusy();
		ElevatorStatic elevatorStatic = this.ElevatorAtFloor(base.LiftPositionToFloor());
		if (elevatorStatic != null)
		{
			elevatorStatic.OnLiftArrivedAtFloor();
		}
		base.NotifyLiftEntityDoorsOpen(true);
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000AC97E File Offset: 0x000AAB7E
	protected override void OpenLiftDoors()
	{
		base.OpenLiftDoors();
		this.OnLiftArrivedAtFloor();
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000AC98C File Offset: 0x000AAB8C
	private void OnLiftArrivedAtFloor()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		this.MarkDirty();
		base.Invoke(new Action(this.ClearPowerOutput), 10f);
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000885FE File Offset: 0x000867FE
	private void ClearPowerOutput()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000AC9B9 File Offset: 0x000AABB9
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000AC9CB File Offset: 0x000AABCB
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		}
	}
}
