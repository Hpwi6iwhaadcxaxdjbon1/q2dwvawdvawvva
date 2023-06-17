using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class Elevator : global::IOEntity, IFlagNotify
{
	// Token: 0x04000E50 RID: 3664
	public Transform LiftRoot;

	// Token: 0x04000E51 RID: 3665
	public GameObjectRef LiftEntityPrefab;

	// Token: 0x04000E52 RID: 3666
	public GameObjectRef IoEntityPrefab;

	// Token: 0x04000E53 RID: 3667
	public Transform IoEntitySpawnPoint;

	// Token: 0x04000E54 RID: 3668
	public GameObject FloorBlockerVolume;

	// Token: 0x04000E55 RID: 3669
	public float LiftSpeedPerMetre = 1f;

	// Token: 0x04000E56 RID: 3670
	public GameObject[] PoweredObjects;

	// Token: 0x04000E57 RID: 3671
	public MeshRenderer PoweredMesh;

	// Token: 0x04000E58 RID: 3672
	[ColorUsage(true, true)]
	public Color PoweredLightColour;

	// Token: 0x04000E59 RID: 3673
	[ColorUsage(true, true)]
	public Color UnpoweredLightColour;

	// Token: 0x04000E5A RID: 3674
	public SkinnedMeshRenderer[] CableRenderers;

	// Token: 0x04000E5B RID: 3675
	public LODGroup CableLod;

	// Token: 0x04000E5C RID: 3676
	public Transform CableRoot;

	// Token: 0x04000E5D RID: 3677
	public float LiftMoveDelay;

	// Token: 0x04000E5F RID: 3679
	protected const global::BaseEntity.Flags TopFloorFlag = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000E60 RID: 3680
	public const global::BaseEntity.Flags ElevatorPowered = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000E61 RID: 3681
	private ElevatorLift liftEntity;

	// Token: 0x04000E62 RID: 3682
	private global::IOEntity ioEntity;

	// Token: 0x04000E63 RID: 3683
	private int[] previousPowerAmount = new int[2];

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x060015D5 RID: 5589 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool IsStatic
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x060015D6 RID: 5590 RVA: 0x000ABB82 File Offset: 0x000A9D82
	// (set) Token: 0x060015D7 RID: 5591 RVA: 0x000ABB8A File Offset: 0x000A9D8A
	public int Floor { get; set; }

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x060015D8 RID: 5592 RVA: 0x000231B4 File Offset: 0x000213B4
	protected bool IsTop
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved1);
		}
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000ABB94 File Offset: 0x000A9D94
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.elevator != null)
		{
			this.Floor = info.msg.elevator.floor;
		}
		if (this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(this.Floor > 0);
		}
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x000ABBF0 File Offset: 0x000A9DF0
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
			this.Floor = elevatorInDirection.Floor + 1;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x000ABC41 File Offset: 0x000A9E41
	protected virtual void CallElevator()
	{
		base.EntityLinkBroadcast<global::Elevator, ConstructionSocket>(delegate(global::Elevator elevatorEnt)
		{
			if (elevatorEnt.IsTop)
			{
				float num;
				elevatorEnt.RequestMoveLiftTo(this.Floor, out num, this);
			}
		}, (ConstructionSocket socket) => socket.socketType == ConstructionSocket.Type.Elevator);
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000ABC74 File Offset: 0x000A9E74
	public void Server_RaiseLowerElevator(global::Elevator.Direction dir, bool goTopBottom)
	{
		if (base.IsBusy())
		{
			return;
		}
		int num = this.LiftPositionToFloor();
		if (dir != global::Elevator.Direction.Up)
		{
			if (dir == global::Elevator.Direction.Down)
			{
				num--;
				if (goTopBottom)
				{
					num = 0;
				}
			}
		}
		else
		{
			num++;
			if (goTopBottom)
			{
				num = this.Floor;
			}
		}
		float num2;
		this.RequestMoveLiftTo(num, out num2, this);
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x000ABCC0 File Offset: 0x000A9EC0
	protected bool RequestMoveLiftTo(int targetFloor, out float timeToTravel, global::Elevator fromElevator)
	{
		timeToTravel = 0f;
		if (base.IsBusy())
		{
			return false;
		}
		if (!this.IsStatic && this.ioEntity != null && !this.ioEntity.IsPowered())
		{
			return false;
		}
		if (!this.IsValidFloor(targetFloor))
		{
			return false;
		}
		if (!this.liftEntity.CanMove())
		{
			return false;
		}
		int num = this.LiftPositionToFloor();
		if (num == targetFloor)
		{
			this.OpenLiftDoors();
			this.OpenDoorsAtFloor(num);
			fromElevator.OpenLiftDoors();
			return false;
		}
		Vector3 worldSpaceFloorPosition = this.GetWorldSpaceFloorPosition(targetFloor);
		if (!GamePhysics.LineOfSight(this.liftEntity.transform.position, worldSpaceFloorPosition, 2097152, null))
		{
			return false;
		}
		this.OnMoveBegin();
		Vector3 vector = base.transform.InverseTransformPoint(worldSpaceFloorPosition);
		timeToTravel = this.TimeToTravelDistance(Mathf.Abs(this.liftEntity.transform.localPosition.y - vector.y));
		LeanTween.moveLocalY(this.liftEntity.gameObject, vector.y, timeToTravel).delay = this.LiftMoveDelay;
		timeToTravel += this.LiftMoveDelay;
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		if (targetFloor < this.Floor)
		{
			this.liftEntity.ToggleHurtTrigger(true);
		}
		base.Invoke(new Action(this.ClearBusy), timeToTravel + 1f);
		this.liftEntity.NotifyNewFloor(targetFloor, this.Floor);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
		return true;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x000ABE4D File Offset: 0x000AA04D
	protected virtual void OpenLiftDoors()
	{
		this.NotifyLiftEntityDoorsOpen(true);
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnMoveBegin()
	{
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x000ABE56 File Offset: 0x000AA056
	private float TimeToTravelDistance(float distance)
	{
		return distance / this.LiftSpeedPerMetre;
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x000ABE60 File Offset: 0x000AA060
	protected virtual Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		int num = this.Floor - targetFloor;
		Vector3 b = Vector3.up * ((float)num * this.FloorHeight);
		b.y -= 1f;
		return base.transform.position - b;
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x060015E2 RID: 5602 RVA: 0x000ABEAB File Offset: 0x000AA0AB
	protected virtual float FloorHeight
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x000ABEB4 File Offset: 0x000AA0B4
	protected virtual void ClearBusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		if (this.liftEntity != null)
		{
			this.liftEntity.ToggleHurtTrigger(false);
		}
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			this.ioEntity.SendChangedToRoot(true);
		}
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000ABF16 File Offset: 0x000AA116
	protected virtual bool IsValidFloor(int targetFloor)
	{
		return targetFloor <= this.Floor && targetFloor >= 0;
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x000ABF2C File Offset: 0x000AA12C
	private global::Elevator GetElevatorInDirection(global::Elevator.Direction dir)
	{
		EntityLink entityLink = base.FindLink((dir == global::Elevator.Direction.Down) ? "elevator/sockets/elevator-male" : "elevator/sockets/elevator-female");
		if (entityLink != null && !entityLink.IsEmpty())
		{
			global::BaseEntity owner = entityLink.connections[0].owner;
			global::Elevator elevator;
			if (owner != null && owner.isServer && (elevator = (owner as global::Elevator)) != null && elevator != this)
			{
				return elevator;
			}
		}
		return null;
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x000ABF94 File Offset: 0x000AA194
	public void UpdateChildEntities(bool isTop)
	{
		if (isTop)
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity == null)
			{
				this.liftEntity = (GameManager.server.CreateEntity(this.LiftEntityPrefab.resourcePath, this.GetWorldSpaceFloorPosition(this.Floor), this.LiftRoot.rotation, true) as ElevatorLift);
				this.liftEntity.SetParent(this, true, false);
				this.liftEntity.Spawn();
			}
			if (this.ioEntity == null)
			{
				this.FindExistingIOChild();
			}
			if (this.ioEntity == null && this.IoEntityPrefab.isValid)
			{
				this.ioEntity = (GameManager.server.CreateEntity(this.IoEntityPrefab.resourcePath, this.IoEntitySpawnPoint.position, this.IoEntitySpawnPoint.rotation, true) as global::IOEntity);
				this.ioEntity.SetParent(this, true, false);
				this.ioEntity.Spawn();
				return;
			}
		}
		else
		{
			if (this.liftEntity != null)
			{
				this.liftEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			if (this.ioEntity != null)
			{
				this.ioEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x000AC0D4 File Offset: 0x000AA2D4
	private void FindExistingIOChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::IOEntity ioentity;
				if ((ioentity = (enumerator.Current as global::IOEntity)) != null)
				{
					this.ioEntity = ioentity;
					break;
				}
			}
		}
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x000AC134 File Offset: 0x000AA334
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.elevator == null)
		{
			info.msg.elevator = Pool.Get<ProtoBuf.Elevator>();
		}
		info.msg.elevator.floor = this.Floor;
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x000AC170 File Offset: 0x000AA370
	protected int LiftPositionToFloor()
	{
		Vector3 position = this.liftEntity.transform.position;
		int result = -1;
		float num = float.MaxValue;
		for (int i = 0; i <= this.Floor; i++)
		{
			float num2 = Vector3.Distance(this.GetWorldSpaceFloorPosition(i), position);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x000AC1C1 File Offset: 0x000AA3C1
	public override void DestroyShared()
	{
		this.Cleanup();
		base.DestroyShared();
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x000AC1D0 File Offset: 0x000AA3D0
	private void Cleanup()
	{
		global::Elevator elevatorInDirection = this.GetElevatorInDirection(global::Elevator.Direction.Down);
		if (elevatorInDirection != null)
		{
			elevatorInDirection.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		}
		global::Elevator elevatorInDirection2 = this.GetElevatorInDirection(global::Elevator.Direction.Up);
		if (elevatorInDirection2 != null)
		{
			elevatorInDirection2.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x000AC214 File Offset: 0x000AA414
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		this.UpdateChildEntities(this.IsTop);
		if (this.ioEntity != null)
		{
			this.ioEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x000AC262 File Offset: 0x000AA462
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0 && this.previousPowerAmount[inputSlot] == 0)
		{
			this.CallElevator();
		}
		this.previousPowerAmount[inputSlot] = inputAmount;
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x000AC289 File Offset: 0x000AA489
	private void OnPhysicsNeighbourChanged()
	{
		if (this.IsStatic)
		{
			return;
		}
		if (this.GetElevatorInDirection(global::Elevator.Direction.Down) == null && !this.HasFloorSocketConnection())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x000AC2B4 File Offset: 0x000AA4B4
	private bool HasFloorSocketConnection()
	{
		EntityLink entityLink = base.FindLink("elevator/sockets/block-male");
		return entityLink != null && !entityLink.IsEmpty();
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x000AC2DC File Offset: 0x000AA4DC
	public void NotifyLiftEntityDoorsOpen(bool state)
	{
		if (this.liftEntity != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = this.liftEntity.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Door door;
					if ((door = (enumerator.Current as Door)) != null)
					{
						door.SetOpen(state, false);
					}
				}
			}
		}
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OpenDoorsAtFloor(int floor)
	{
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000AC34C File Offset: 0x000AA54C
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (!Rust.Application.isLoading && base.isServer && old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			this.UpdateChildEntities(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
		if (old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
		{
			if (this.liftEntity == null)
			{
				this.FindExistingLiftChild();
			}
			if (this.liftEntity != null)
			{
				this.liftEntity.ToggleMovementCollider(!next.HasFlag(global::BaseEntity.Flags.Busy));
			}
		}
		if (old.HasFlag(global::BaseEntity.Flags.Reserved1) != next.HasFlag(global::BaseEntity.Flags.Reserved1) && this.FloorBlockerVolume != null)
		{
			this.FloorBlockerVolume.SetActive(next.HasFlag(global::BaseEntity.Flags.Reserved1));
		}
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000AC484 File Offset: 0x000AA684
	private void FindExistingLiftChild()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ElevatorLift elevatorLift;
				if ((elevatorLift = (enumerator.Current as ElevatorLift)) != null)
				{
					this.liftEntity = elevatorLift;
					break;
				}
			}
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x000AC4E4 File Offset: 0x000AA6E4
	public void OnFlagToggled(bool state)
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, state, false, true);
		}
	}

	// Token: 0x02000C1F RID: 3103
	public enum Direction
	{
		// Token: 0x04004229 RID: 16937
		Up,
		// Token: 0x0400422A RID: 16938
		Down
	}
}
