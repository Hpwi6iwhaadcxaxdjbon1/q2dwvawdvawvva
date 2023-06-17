using System;
using System.Net;
using ConVar;
using Facepunch;
using Network;
using Rust;
using Rust.Platform.Common;

// Token: 0x02000738 RID: 1848
public class RustPlatformHooks : IPlatformHooks
{
	// Token: 0x040029F1 RID: 10737
	public static readonly RustPlatformHooks Instance = new RustPlatformHooks();

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06003372 RID: 13170 RVA: 0x0013C177 File Offset: 0x0013A377
	public uint SteamAppId
	{
		get
		{
			return Rust.Defines.appID;
		}
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x0013C17E File Offset: 0x0013A37E
	public void Abort()
	{
		Rust.Application.Quit();
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x0013C185 File Offset: 0x0013A385
	public void OnItemDefinitionsChanged()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06003375 RID: 13173 RVA: 0x0013C18C File Offset: 0x0013A38C
	public ServerParameters? ServerParameters
	{
		get
		{
			if (Network.Net.sv == null)
			{
				return null;
			}
			IPAddress address = null;
			if (!string.IsNullOrEmpty(ConVar.Server.ip))
			{
				address = IPAddress.Parse(ConVar.Server.ip);
			}
			if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
			{
				ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
			}
			return new ServerParameters?(new ServerParameters("rust", "Rust", 2392.ToString(), ConVar.Server.secure, CommandLine.HasSwitch("-sdrnet"), address, (ushort)Network.Net.sv.port, (ushort)ConVar.Server.queryport));
		}
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x0013C22F File Offset: 0x0013A42F
	public void AuthSessionValidated(ulong userId, ulong ownerUserId, AuthResponse response)
	{
		SingletonComponent<ServerMgr>.Instance.OnValidateAuthTicketResponse(userId, ownerUserId, response);
	}
}
