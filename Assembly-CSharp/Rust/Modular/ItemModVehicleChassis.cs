using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B2E RID: 2862
	public class ItemModVehicleChassis : ItemMod, VehicleModuleInformationPanel.IVehicleModuleInfo
	{
		// Token: 0x04003DE9 RID: 15849
		public GameObjectRef entityPrefab;

		// Token: 0x04003DEA RID: 15850
		[Range(1f, 6f)]
		public int socketsTaken = 1;

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x0600456C RID: 17772 RVA: 0x0019600B File Offset: 0x0019420B
		public int SocketsTaken
		{
			get
			{
				return this.socketsTaken;
			}
		}
	}
}
