using System;
using UnityEngine;

// Token: 0x02000578 RID: 1400
public class VehicleSpawnPoint : SpaceCheckingSpawnPoint
{
	// Token: 0x06002ADF RID: 10975 RVA: 0x001047D6 File Offset: 0x001029D6
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		base.ObjectSpawned(instance);
		VehicleSpawnPoint.AddStartingFuel(instance.gameObject.ToBaseEntity() as BaseVehicle);
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x001047F4 File Offset: 0x001029F4
	public static void AddStartingFuel(BaseVehicle vehicle)
	{
		if (vehicle == null)
		{
			return;
		}
		EntityFuelSystem fuelSystem = vehicle.GetFuelSystem();
		if (fuelSystem != null)
		{
			fuelSystem.AddStartingFuel((float)vehicle.StartingFuelUnits());
		}
	}
}
