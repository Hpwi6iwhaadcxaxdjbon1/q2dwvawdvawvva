using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class ElevatorLiftStatic : ElevatorLift
{
	// Token: 0x04000E64 RID: 3684
	public GameObjectRef ElevatorDoorRef;

	// Token: 0x04000E65 RID: 3685
	public Transform ElevatorDoorLocation;

	// Token: 0x04000E66 RID: 3686
	public bool BlockPerFloorMovement;

	// Token: 0x04000E67 RID: 3687
	private const BaseEntity.Flags CanGoUp = BaseEntity.Flags.Reserved3;

	// Token: 0x04000E68 RID: 3688
	private const BaseEntity.Flags CanGoDown = BaseEntity.Flags.Reserved4;

	// Token: 0x060015F8 RID: 5624 RVA: 0x000AC54C File Offset: 0x000AA74C
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.ElevatorDoorRef.isValid && this.ElevatorDoorLocation != null)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is Door)
					{
						return;
					}
				}
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.ElevatorDoorRef.resourcePath, this.ElevatorDoorLocation.localPosition, this.ElevatorDoorLocation.localRotation, true);
			baseEntity.SetParent(this, false, false);
			baseEntity.Spawn();
			base.SetFlag(BaseEntity.Flags.Reserved3, false, false, false);
			base.SetFlag(BaseEntity.Flags.Reserved4, true, false, true);
		}
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x000AC620 File Offset: 0x000AA820
	public override void NotifyNewFloor(int newFloor, int totalFloors)
	{
		base.NotifyNewFloor(newFloor, totalFloors);
		base.SetFlag(BaseEntity.Flags.Reserved3, newFloor < totalFloors, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, newFloor > 0, false, true);
	}
}
