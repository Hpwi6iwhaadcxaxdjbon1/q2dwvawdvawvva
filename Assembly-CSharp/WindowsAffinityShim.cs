using System;
using System.Runtime.InteropServices;

// Token: 0x020002FE RID: 766
public static class WindowsAffinityShim
{
	// Token: 0x06001E5A RID: 7770
	[DllImport("kernel32.dll")]
	public static extern bool SetProcessAffinityMask(IntPtr process, IntPtr mask);

	// Token: 0x06001E5B RID: 7771
	[DllImport("kernel32.dll")]
	public static extern bool SetPriorityClass(IntPtr process, uint mask);
}
