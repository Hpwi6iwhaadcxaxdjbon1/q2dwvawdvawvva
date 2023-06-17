using System;

// Token: 0x0200085B RID: 2139
public class VehicleModuleInformationPanel : ItemInformationPanel
{
	// Token: 0x0400300A RID: 12298
	public ItemStatValue socketsDisplay;

	// Token: 0x0400300B RID: 12299
	public ItemStatValue hpDisplay;

	// Token: 0x02000E89 RID: 3721
	public interface IVehicleModuleInfo
	{
		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x060052CC RID: 21196
		int SocketsTaken { get; }
	}
}
