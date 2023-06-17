using System;

namespace Rust.Modular
{
	// Token: 0x02000B24 RID: 2852
	[Serializable]
	public class ConditionalSocketSettings
	{
		// Token: 0x04003DB3 RID: 15795
		public bool restrictOnLocation;

		// Token: 0x04003DB4 RID: 15796
		public ConditionalSocketSettings.LocationCondition locationRestriction;

		// Token: 0x04003DB5 RID: 15797
		public bool restrictOnWheel;

		// Token: 0x04003DB6 RID: 15798
		public ModularVehicleSocket.SocketWheelType wheelRestriction;

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06004525 RID: 17701 RVA: 0x001951C7 File Offset: 0x001933C7
		public bool HasSocketRestrictions
		{
			get
			{
				return this.restrictOnLocation || this.restrictOnWheel;
			}
		}

		// Token: 0x02000F93 RID: 3987
		public enum LocationCondition
		{
			// Token: 0x04005044 RID: 20548
			Middle,
			// Token: 0x04005045 RID: 20549
			Front,
			// Token: 0x04005046 RID: 20550
			Back,
			// Token: 0x04005047 RID: 20551
			NotMiddle,
			// Token: 0x04005048 RID: 20552
			NotFront,
			// Token: 0x04005049 RID: 20553
			NotBack
		}
	}
}
