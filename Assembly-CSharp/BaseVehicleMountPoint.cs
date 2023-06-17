using System;

// Token: 0x0200046C RID: 1132
public class BaseVehicleMountPoint : BaseMountable
{
	// Token: 0x0600254C RID: 9548 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool DirectlyMountable()
	{
		return false;
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x000EB720 File Offset: 0x000E9920
	public override BaseVehicle VehicleParent()
	{
		BaseVehicle baseVehicle = base.GetParentEntity() as BaseVehicle;
		while (baseVehicle != null && !baseVehicle.IsVehicleRoot())
		{
			BaseVehicle baseVehicle2 = baseVehicle.GetParentEntity() as BaseVehicle;
			if (baseVehicle2 == null)
			{
				return baseVehicle;
			}
			baseVehicle = baseVehicle2;
		}
		return baseVehicle;
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x000EB768 File Offset: 0x000E9968
	public override bool BlocksWaterFor(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle == null) && baseVehicle.BlocksWaterFor(player);
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000EB790 File Offset: 0x000E9990
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.WaterFactorForPlayer(player);
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000EB7BC File Offset: 0x000E99BC
	public override float AirFactor()
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.AirFactor();
	}
}
