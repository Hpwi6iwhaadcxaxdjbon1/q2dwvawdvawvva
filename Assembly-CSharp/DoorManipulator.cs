using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004C5 RID: 1221
public class DoorManipulator : IOEntity
{
	// Token: 0x04002033 RID: 8243
	public EntityRef entityRef;

	// Token: 0x04002034 RID: 8244
	public Door targetDoor;

	// Token: 0x04002035 RID: 8245
	public DoorManipulator.DoorEffect powerAction;

	// Token: 0x04002036 RID: 8246
	private bool toggle = true;

	// Token: 0x060027CB RID: 10187 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000F7C7C File Offset: 0x000F5E7C
	public virtual void SetTargetDoor(Door newTargetDoor)
	{
		UnityEngine.Object x = this.targetDoor;
		this.targetDoor = newTargetDoor;
		base.SetFlag(BaseEntity.Flags.On, this.targetDoor != null, false, true);
		this.entityRef.Set(newTargetDoor);
		if (x != this.targetDoor && this.targetDoor != null)
		{
			this.DoAction();
		}
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000F7CD8 File Offset: 0x000F5ED8
	public virtual void SetupInitialDoorConnection()
	{
		if (this.targetDoor == null && !this.entityRef.IsValid(true))
		{
			this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
		}
		if (this.targetDoor != null && !this.entityRef.IsValid(true))
		{
			this.entityRef.Set(this.targetDoor);
		}
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000F7D73 File Offset: 0x000F5F73
	public override void Init()
	{
		base.Init();
		this.SetupInitialDoorConnection();
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000F7D84 File Offset: 0x000F5F84
	public Door FindDoor(bool allowLocked = true)
	{
		List<Door> list = Pool.GetList<Door>();
		Vis.Entities<Door>(base.transform.position, 1f, list, 2097152, QueryTriggerInteraction.Ignore);
		Door result = null;
		float num = float.PositiveInfinity;
		foreach (Door door in list)
		{
			if (door.isServer)
			{
				if (!allowLocked)
				{
					BaseLock baseLock = door.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
					if (baseLock != null && baseLock.IsLocked())
					{
						continue;
					}
				}
				float num2 = Vector3.Distance(door.transform.position, base.transform.position);
				if (num2 < num)
				{
					result = door;
					num = num2;
				}
			}
		}
		Pool.FreeList<Door>(ref list);
		return result;
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000F7E58 File Offset: 0x000F6058
	public virtual void DoActionDoorMissing()
	{
		this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x000F7E6C File Offset: 0x000F606C
	public void DoAction()
	{
		bool flag = this.IsPowered();
		if (this.targetDoor == null)
		{
			this.DoActionDoorMissing();
		}
		if (this.targetDoor != null)
		{
			if (this.targetDoor.IsBusy())
			{
				base.Invoke(new Action(this.DoAction), 1f);
				return;
			}
			if (this.powerAction == DoorManipulator.DoorEffect.Open)
			{
				if (flag)
				{
					if (!this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(true, false);
						return;
					}
				}
				else if (this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(false, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Close)
			{
				if (flag)
				{
					if (this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(false, false);
						return;
					}
				}
				else if (!this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(true, false);
					return;
				}
			}
			else if (this.powerAction == DoorManipulator.DoorEffect.Toggle)
			{
				if (flag && this.toggle)
				{
					this.targetDoor.SetOpen(!this.targetDoor.IsOpen(), false);
					this.toggle = false;
					return;
				}
				if (!this.toggle)
				{
					this.toggle = true;
				}
			}
		}
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x000F7F98 File Offset: 0x000F6198
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.DoAction();
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000F7FA8 File Offset: 0x000F61A8
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericEntRef1 = this.entityRef.uid;
		info.msg.ioEntity.genericInt1 = (int)this.powerAction;
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000F7FE4 File Offset: 0x000F61E4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.entityRef.uid = info.msg.ioEntity.genericEntRef1;
			this.powerAction = (DoorManipulator.DoorEffect)info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x02000D1B RID: 3355
	public enum DoorEffect
	{
		// Token: 0x04004636 RID: 17974
		Close,
		// Token: 0x04004637 RID: 17975
		Open,
		// Token: 0x04004638 RID: 17976
		Toggle
	}
}
