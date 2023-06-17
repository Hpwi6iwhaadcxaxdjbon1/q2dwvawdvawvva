using System;
using UnityEngine;

// Token: 0x02000496 RID: 1174
[Serializable]
public class ModularCarCentralLockingSwitch : VehicleModuleButtonComponent
{
	// Token: 0x04001EE1 RID: 7905
	public Transform centralLockingSwitch;

	// Token: 0x04001EE2 RID: 7906
	public Vector3 switchOffPos;

	// Token: 0x04001EE3 RID: 7907
	public Vector3 switchOnPos;

	// Token: 0x06002696 RID: 9878 RVA: 0x000F2230 File Offset: 0x000F0430
	public override void ServerUse(BasePlayer player, BaseVehicleModule parentModule)
	{
		ModularCar modularCar;
		if ((modularCar = (parentModule.Vehicle as ModularCar)) != null)
		{
			modularCar.CarLock.ToggleCentralLocking();
		}
	}
}
