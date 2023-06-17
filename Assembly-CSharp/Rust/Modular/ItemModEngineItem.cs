using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B2D RID: 2861
	public class ItemModEngineItem : ItemMod
	{
		// Token: 0x04003DE7 RID: 15847
		public EngineStorage.EngineItemTypes engineItemType;

		// Token: 0x04003DE8 RID: 15848
		[Range(1f, 3f)]
		public int tier = 1;
	}
}
