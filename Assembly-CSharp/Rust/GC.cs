using System;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000B04 RID: 2820
	public class GC : MonoBehaviour, IClientComponent
	{
		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x060044E5 RID: 17637 RVA: 0x0000441C File Offset: 0x0000261C
		public static bool Enabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060044E6 RID: 17638 RVA: 0x00194B62 File Offset: 0x00192D62
		public static void Collect()
		{
			GC.Collect();
		}

		// Token: 0x060044E7 RID: 17639 RVA: 0x00194B69 File Offset: 0x00192D69
		public static long GetTotalMemory()
		{
			return GC.GetTotalMemory(false) / 1048576L;
		}

		// Token: 0x060044E8 RID: 17640 RVA: 0x00194B78 File Offset: 0x00192D78
		public static int CollectionCount()
		{
			return GC.CollectionCount(0);
		}
	}
}
