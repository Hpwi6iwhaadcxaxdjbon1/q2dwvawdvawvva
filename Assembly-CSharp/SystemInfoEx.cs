using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000312 RID: 786
public static class SystemInfoEx
{
	// Token: 0x040017C0 RID: 6080
	private static bool[] supportedRenderTextureFormats;

	// Token: 0x06001EB8 RID: 7864
	[DllImport("RustNative")]
	private static extern ulong System_GetMemoryUsage();

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001EB9 RID: 7865 RVA: 0x000D1373 File Offset: 0x000CF573
	public static int systemMemoryUsed
	{
		get
		{
			return (int)(SystemInfoEx.System_GetMemoryUsage() / 1024UL / 1024UL);
		}
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000D138C File Offset: 0x000CF58C
	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (SystemInfoEx.supportedRenderTextureFormats == null)
		{
			Array values = Enum.GetValues(typeof(RenderTextureFormat));
			int num = (int)values.GetValue(values.Length - 1);
			SystemInfoEx.supportedRenderTextureFormats = new bool[num + 1];
			for (int i = 0; i <= num; i++)
			{
				bool flag = Enum.IsDefined(typeof(RenderTextureFormat), i);
				SystemInfoEx.supportedRenderTextureFormats[i] = (flag && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)i));
			}
		}
		return SystemInfoEx.supportedRenderTextureFormats[(int)format];
	}
}
