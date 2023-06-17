using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar
{
	// Token: 0x02000ACB RID: 2763
	[ConsoleSystem.Factory("memsnap")]
	public class MemSnap : ConsoleSystem
	{
		// Token: 0x0600426F RID: 17007 RVA: 0x00189DFC File Offset: 0x00187FFC
		private static string NeedProfileFolder()
		{
			string path = "profile";
			if (!Directory.Exists(path))
			{
				return Directory.CreateDirectory(path).FullName;
			}
			return new DirectoryInfo(path).FullName;
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x00189E30 File Offset: 0x00188030
		[ClientVar]
		[ServerVar]
		public static void managed(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects);
		}

		// Token: 0x06004271 RID: 17009 RVA: 0x00189E6C File Offset: 0x0018806C
		[ClientVar]
		[ServerVar]
		public static void native(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.NativeObjects);
		}

		// Token: 0x06004272 RID: 17010 RVA: 0x00189EA8 File Offset: 0x001880A8
		[ClientVar]
		[ServerVar]
		public static void full(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces);
		}
	}
}
