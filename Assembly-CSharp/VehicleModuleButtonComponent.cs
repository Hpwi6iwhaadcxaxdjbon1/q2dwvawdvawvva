using System;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public abstract class VehicleModuleButtonComponent : MonoBehaviour
{
	// Token: 0x04001F04 RID: 7940
	public string interactionColliderName = "MyCollider";

	// Token: 0x04001F05 RID: 7941
	public SoundDefinition pressSoundDef;

	// Token: 0x060026C4 RID: 9924
	public abstract void ServerUse(BasePlayer player, BaseVehicleModule parentModule);
}
