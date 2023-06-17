using System;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public class ModularCarSeat : MouseSteerableSeat
{
	// Token: 0x04001EFA RID: 7930
	[SerializeField]
	private Vector3 leftFootIKPos;

	// Token: 0x04001EFB RID: 7931
	[SerializeField]
	private Vector3 rightFootIKPos;

	// Token: 0x04001EFC RID: 7932
	[SerializeField]
	private Vector3 leftHandIKPos;

	// Token: 0x04001EFD RID: 7933
	[SerializeField]
	private Vector3 rightHandIKPos;

	// Token: 0x04001EFE RID: 7934
	public float providesComfort;

	// Token: 0x04001EFF RID: 7935
	public VehicleModuleSeating associatedSeatingModule;

	// Token: 0x060026BD RID: 9917 RVA: 0x000F2868 File Offset: 0x000F0A68
	public override bool CanSwapToThis(BasePlayer player)
	{
		if (this.associatedSeatingModule.DoorsAreLockable)
		{
			ModularCar modularCar = this.associatedSeatingModule.Vehicle as ModularCar;
			if (modularCar != null)
			{
				return modularCar.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
			}
		}
		return true;
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000F28A6 File Offset: 0x000F0AA6
	public override float GetComfort()
	{
		return this.providesComfort;
	}
}
