using System;

namespace Rust.Modular
{
	// Token: 0x02000B26 RID: 2854
	public static class EngineItemTypeEx
	{
		// Token: 0x0600452A RID: 17706 RVA: 0x00195203 File Offset: 0x00193403
		public static bool BoostsAcceleration(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.SparkPlug || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x0600452B RID: 17707 RVA: 0x0019520F File Offset: 0x0019340F
		public static bool BoostsTopSpeed(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Crankshaft || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x0600452C RID: 17708 RVA: 0x0015A01E File Offset: 0x0015821E
		public static bool BoostsFuelEconomy(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Valve;
		}
	}
}
