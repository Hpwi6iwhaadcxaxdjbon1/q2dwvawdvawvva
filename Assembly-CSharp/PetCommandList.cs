using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000395 RID: 917
public class PetCommandList : PrefabAttribute
{
	// Token: 0x0400195C RID: 6492
	public List<PetCommandList.PetCommandDesc> Commands;

	// Token: 0x06002057 RID: 8279 RVA: 0x000D68BD File Offset: 0x000D4ABD
	protected override Type GetIndexedType()
	{
		return typeof(PetCommandList);
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x000D68C9 File Offset: 0x000D4AC9
	public List<PetCommandList.PetCommandDesc> GetCommandDescriptions()
	{
		return this.Commands;
	}

	// Token: 0x02000CB2 RID: 3250
	[Serializable]
	public struct PetCommandDesc
	{
		// Token: 0x04004483 RID: 17539
		public PetCommandType CommandType;

		// Token: 0x04004484 RID: 17540
		public Translate.Phrase Title;

		// Token: 0x04004485 RID: 17541
		public Translate.Phrase Description;

		// Token: 0x04004486 RID: 17542
		public Sprite Icon;

		// Token: 0x04004487 RID: 17543
		public int CommandIndex;

		// Token: 0x04004488 RID: 17544
		public bool Raycast;

		// Token: 0x04004489 RID: 17545
		public int CommandWheelOrder;
	}
}
