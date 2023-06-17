using System;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public abstract class VehicleChassisVisuals<T> : MonoBehaviour where T : BaseVehicle, VehicleChassisVisuals<T>.IClientWheelUser
{
	// Token: 0x02000D17 RID: 3351
	public interface IClientWheelUser
	{
		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06005023 RID: 20515
		Vector3 Velocity { get; }

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06005024 RID: 20516
		float DriveWheelVelocity { get; }

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06005025 RID: 20517
		float SteerAngle { get; }

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06005026 RID: 20518
		float MaxSteerAngle { get; }

		// Token: 0x06005027 RID: 20519
		float GetThrottleInput();
	}
}
