using System;
using CompanionServer.Cameras;
using ConVar;
using Facepunch;
using Facepunch.Math;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FC RID: 2556
	public class Info : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D1C RID: 15644 RVA: 0x00166BBC File Offset: 0x00164DBC
		public override void Execute()
		{
			AppInfo appInfo = Facepunch.Pool.Get<AppInfo>();
			appInfo.name = Server.hostname;
			appInfo.headerImage = Server.headerimage;
			appInfo.logoImage = Server.logoimage;
			appInfo.url = Server.url;
			appInfo.map = global::World.Name;
			appInfo.mapSize = global::World.Size;
			appInfo.wipeTime = (uint)Epoch.FromDateTime(SaveRestore.SaveCreatedTime.ToUniversalTime());
			appInfo.players = (uint)global::BasePlayer.activePlayerList.Count;
			appInfo.maxPlayers = (uint)Server.maxplayers;
			appInfo.queuedPlayers = (uint)SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
			appInfo.seed = global::World.Seed;
			appInfo.camerasEnabled = CameraRenderer.enabled;
			AppResponse appResponse = Facepunch.Pool.Get<AppResponse>();
			appResponse.info = appInfo;
			base.Send(appResponse);
		}
	}
}
