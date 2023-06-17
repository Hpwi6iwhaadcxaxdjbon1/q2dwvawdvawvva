using System;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	// Token: 0x02000ABC RID: 2748
	[ConsoleSystem.Factory("gc")]
	public class GC : ConsoleSystem
	{
		// Token: 0x04003B3C RID: 15164
		[ClientVar]
		public static bool buffer_enabled = true;

		// Token: 0x04003B3D RID: 15165
		[ClientVar]
		public static int debuglevel = 1;

		// Token: 0x04003B3E RID: 15166
		private static int m_buffer = 256;

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x060041DC RID: 16860 RVA: 0x00186B90 File Offset: 0x00184D90
		// (set) Token: 0x060041DD RID: 16861 RVA: 0x00186B97 File Offset: 0x00184D97
		[ClientVar]
		public static int buffer
		{
			get
			{
				return ConVar.GC.m_buffer;
			}
			set
			{
				ConVar.GC.m_buffer = Mathf.Clamp(value, 64, 4096);
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x060041DE RID: 16862 RVA: 0x00186BAB File Offset: 0x00184DAB
		// (set) Token: 0x060041DF RID: 16863 RVA: 0x00186BB2 File Offset: 0x00184DB2
		[ServerVar]
		[ClientVar]
		public static bool incremental_enabled
		{
			get
			{
				return GarbageCollector.isIncremental;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.incremental as it is read only");
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x060041E0 RID: 16864 RVA: 0x00186BBE File Offset: 0x00184DBE
		// (set) Token: 0x060041E1 RID: 16865 RVA: 0x00186BCD File Offset: 0x00184DCD
		[ServerVar]
		[ClientVar]
		public static int incremental_milliseconds
		{
			get
			{
				return (int)(GarbageCollector.incrementalTimeSliceNanoseconds / 1000000UL);
			}
			set
			{
				GarbageCollector.incrementalTimeSliceNanoseconds = (ulong)(1000000L * (long)Mathf.Max(value, 0));
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x060041E2 RID: 16866 RVA: 0x00186BE3 File Offset: 0x00184DE3
		// (set) Token: 0x060041E3 RID: 16867 RVA: 0x00186BEA File Offset: 0x00184DEA
		[ServerVar]
		[ClientVar]
		public static bool enabled
		{
			get
			{
				return Rust.GC.Enabled;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.enabled as it is read only");
			}
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x00186BF6 File Offset: 0x00184DF6
		[ServerVar]
		[ClientVar]
		public static void collect()
		{
			Rust.GC.Collect();
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x00186BFD File Offset: 0x00184DFD
		[ServerVar]
		[ClientVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x00186C08 File Offset: 0x00184E08
		[ServerVar]
		[ClientVar]
		public static void alloc(ConsoleSystem.Arg args)
		{
			byte[] array = new byte[args.GetInt(0, 1048576)];
			args.ReplyWith("Allocated " + array.Length + " bytes");
		}
	}
}
