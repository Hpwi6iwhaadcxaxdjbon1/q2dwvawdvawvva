using System;
using System.Runtime.InteropServices;
using Facepunch;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public static class SelfCheck
{
	// Token: 0x06002894 RID: 10388 RVA: 0x000FA3E0 File Offset: 0x000F85E0
	public static bool Run()
	{
		if (FileSystem.Backend.isError)
		{
			return SelfCheck.Failed("Asset Bundle Error: " + FileSystem.Backend.loadingError);
		}
		if (FileSystem.Load<GameManifest>("Assets/manifest.asset", true) == null)
		{
			return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
		}
		if (!SelfCheck.TestRustNative())
		{
			return false;
		}
		if (CommandLine.HasSwitch("-force-feature-level-9-3"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
		}
		if (CommandLine.HasSwitch("-force-feature-level-10-0"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
		}
		return !CommandLine.HasSwitch("-force-feature-level-10-1") || SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000FA480 File Offset: 0x000F8680
	private static bool Failed(string Message)
	{
		if (SingletonComponent<Bootstrap>.Instance)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "";
			SingletonComponent<Bootstrap>.Instance.ThrowError(Message);
		}
		Debug.LogError("SelfCheck Failed: " + Message);
		return false;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000FA4BC File Offset: 0x000F86BC
	private static bool TestRustNative()
	{
		try
		{
			if (!SelfCheck.RustNative_VersionCheck(5))
			{
				return SelfCheck.Failed("RustNative is wrong version!");
			}
		}
		catch (DllNotFoundException ex)
		{
			return SelfCheck.Failed("RustNative library couldn't load! " + ex.Message);
		}
		return true;
	}

	// Token: 0x06002897 RID: 10391
	[DllImport("RustNative")]
	private static extern bool RustNative_VersionCheck(int version);
}
