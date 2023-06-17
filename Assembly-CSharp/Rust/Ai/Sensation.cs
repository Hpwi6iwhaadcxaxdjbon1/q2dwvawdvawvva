using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B40 RID: 2880
	public struct Sensation
	{
		// Token: 0x04003E40 RID: 15936
		public SensationType Type;

		// Token: 0x04003E41 RID: 15937
		public Vector3 Position;

		// Token: 0x04003E42 RID: 15938
		public float Radius;

		// Token: 0x04003E43 RID: 15939
		public float DamagePotential;

		// Token: 0x04003E44 RID: 15940
		public BaseEntity Initiator;

		// Token: 0x04003E45 RID: 15941
		public BasePlayer InitiatorPlayer;

		// Token: 0x04003E46 RID: 15942
		public BaseEntity UsedEntity;
	}
}
