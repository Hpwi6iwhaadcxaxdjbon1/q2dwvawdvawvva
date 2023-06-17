using System;

// Token: 0x0200046D RID: 1133
public class BaseVehicleSeat : BaseVehicleMountPoint
{
	// Token: 0x04001D8A RID: 7562
	public float mountedAnimationSpeed;

	// Token: 0x04001D8B RID: 7563
	public bool sendClientInputToVehicleParent;

	// Token: 0x04001D8C RID: 7564
	public bool forcePlayerModelUpdate;

	// Token: 0x06002552 RID: 9554 RVA: 0x000EB7E8 File Offset: 0x000E99E8
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000EB810 File Offset: 0x000E9A10
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.MounteeTookDamage(mountee, info);
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000EB838 File Offset: 0x000E9A38
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.PlayerServerInput(inputState, player);
		}
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x000EB868 File Offset: 0x000E9A68
	public override void LightToggle(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.LightToggle(player);
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SwitchParent(BaseEntity ent)
	{
	}
}
