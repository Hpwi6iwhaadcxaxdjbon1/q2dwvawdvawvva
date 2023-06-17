using System;
using UnityEngine;

// Token: 0x02000259 RID: 601
public class ConstructionSocket_Elevator : ConstructionSocket
{
	// Token: 0x04001513 RID: 5395
	public int MaxFloor = 5;

	// Token: 0x06001C57 RID: 7255 RVA: 0x000C58DC File Offset: 0x000C3ADC
	protected override bool CanConnectToEntity(Construction.Target target)
	{
		Elevator elevator;
		if ((elevator = (target.entity as Elevator)) != null && elevator.Floor >= this.MaxFloor)
		{
			return false;
		}
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		return !GamePhysics.CheckOBB(new OBB(worldPosition, new Vector3(2f, 0.5f, 2f), worldRotation), 2097152, QueryTriggerInteraction.UseGlobal) && base.CanConnectToEntity(target);
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x000C594C File Offset: 0x000C3B4C
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Vector3 position2 = Matrix4x4.TRS(position, rotation, Vector3.one).MultiplyPoint3x4(this.worldPosition);
		return !GamePhysics.CheckOBB(new OBB(position2, new Vector3(2f, 0.5f, 2f), rotation), 2097152, QueryTriggerInteraction.UseGlobal);
	}
}
