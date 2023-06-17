using System;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public class ModularCarChassisVisuals : VehicleChassisVisuals<ModularCar>, IClientComponent
{
	// Token: 0x04001EE4 RID: 7908
	public Transform frontAxle;

	// Token: 0x04001EE5 RID: 7909
	public Transform rearAxle;

	// Token: 0x04001EE6 RID: 7910
	public ModularCarChassisVisuals.Steering steering;

	// Token: 0x04001EE7 RID: 7911
	public ModularCarChassisVisuals.LookAtTarget transmission;

	// Token: 0x02000D03 RID: 3331
	[Serializable]
	public class Steering
	{
		// Token: 0x040045DF RID: 17887
		public Transform steerL;

		// Token: 0x040045E0 RID: 17888
		public Transform steerR;

		// Token: 0x040045E1 RID: 17889
		public ModularCarChassisVisuals.LookAtTarget steerRodL;

		// Token: 0x040045E2 RID: 17890
		public ModularCarChassisVisuals.LookAtTarget steerRodR;

		// Token: 0x040045E3 RID: 17891
		public ModularCarChassisVisuals.LookAtTarget steeringArm;
	}

	// Token: 0x02000D04 RID: 3332
	[Serializable]
	public class LookAtTarget
	{
		// Token: 0x040045E4 RID: 17892
		public Transform aim;

		// Token: 0x040045E5 RID: 17893
		public Transform target;

		// Token: 0x040045E6 RID: 17894
		public Vector3 angleAdjust;
	}
}
