using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
[Serializable]
public class CarWheel
{
	// Token: 0x04001DDF RID: 7647
	public WheelCollider wheelCollider;

	// Token: 0x04001DE0 RID: 7648
	[Range(0.1f, 3f)]
	public float tyreFriction = 1f;

	// Token: 0x04001DE1 RID: 7649
	public bool steerWheel;

	// Token: 0x04001DE2 RID: 7650
	public bool brakeWheel = true;

	// Token: 0x04001DE3 RID: 7651
	public bool powerWheel = true;
}
