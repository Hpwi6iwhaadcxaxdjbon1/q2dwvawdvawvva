using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CompanionServer;
using Facepunch.Extend;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA5 RID: 2725
	[ConsoleSystem.Factory("app")]
	public class App : ConsoleSystem
	{
		// Token: 0x04003AE7 RID: 15079
		[ServerVar]
		public static string listenip = "";

		// Token: 0x04003AE8 RID: 15080
		[ServerVar]
		public static int port;

		// Token: 0x04003AE9 RID: 15081
		[ServerVar]
		public static string publicip = "";

		// Token: 0x04003AEA RID: 15082
		[ServerVar(Help = "Disables updating entirely - emergency use only")]
		public static bool update = true;

		// Token: 0x04003AEB RID: 15083
		[ServerVar(Help = "Enables sending push notifications")]
		public static bool notifications = true;

		// Token: 0x04003AEC RID: 15084
		[ServerVar(Help = "Max number of queued messages - set to 0 to disable message processing")]
		public static int queuelimit = 100;

		// Token: 0x04003AED RID: 15085
		[ReplicatedVar(Default = "")]
		public static string serverid = "";

		// Token: 0x04003AEE RID: 15086
		[ServerVar(Help = "Cooldown time before alarms can send another notification (in seconds)")]
		public static float alarmcooldown = 30f;

		// Token: 0x04003AEF RID: 15087
		[ServerVar]
		public static int maxconnections = 500;

		// Token: 0x04003AF0 RID: 15088
		[ServerVar]
		public static int maxconnectionsperip = 5;

		// Token: 0x04003AF1 RID: 15089
		[ServerVar]
		public static int maxmessagesize = 1048576;

		// Token: 0x06004157 RID: 16727 RVA: 0x00183CE8 File Offset: 0x00181EE8
		[ServerUserVar]
		public static async void pair(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!(basePlayer == null))
			{
				Dictionary<string, string> playerPairingData = Util.GetPlayerPairingData(basePlayer);
				NotificationSendResult notificationSendResult = await Util.SendPairNotification("server", basePlayer, Server.hostname.Truncate(128, null), "Tap to pair with this server.", playerPairingData);
				arg.ReplyWith((notificationSendResult == NotificationSendResult.Sent) ? "Sent pairing notification." : notificationSendResult.ToErrorMessage());
			}
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x00183D24 File Offset: 0x00181F24
		[ServerUserVar]
		public static void regeneratetoken(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			SingletonComponent<ServerMgr>.Instance.persistance.RegenerateAppToken(basePlayer.userID);
			arg.ReplyWith("Regenerated Rust+ token");
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x00183D64 File Offset: 0x00181F64
		[ServerVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			if (!Server.IsEnabled)
			{
				arg.ReplyWith("Companion server is not enabled");
				return;
			}
			Listener listener = Server.Listener;
			arg.ReplyWith(string.Format("Server ID: {0}\nListening on: {1}:{2}\nApp connects to: {3}:{4}", new object[]
			{
				App.serverid,
				listener.Address,
				listener.Port,
				App.GetPublicIP(),
				App.port
			}));
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x00183DD4 File Offset: 0x00181FD4
		[ServerVar]
		public static void resetlimiter(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			ConnectionLimiter limiter = listener.Limiter;
			if (limiter == null)
			{
				return;
			}
			limiter.Clear();
		}

		// Token: 0x0600415B RID: 16731 RVA: 0x00183DF0 File Offset: 0x00181FF0
		[ServerVar]
		public static void connections(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			string text;
			if (listener == null)
			{
				text = null;
			}
			else
			{
				ConnectionLimiter limiter = listener.Limiter;
				text = ((limiter != null) ? limiter.ToString() : null);
			}
			string strValue = text ?? "Not available";
			arg.ReplyWith(strValue);
		}

		// Token: 0x0600415C RID: 16732 RVA: 0x00183E2C File Offset: 0x0018202C
		[ServerVar]
		public static void appban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appban <steamID64>");
				return;
			}
			string strValue = SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, true) ? string.Format("Banned {0} from using the companion app", @ulong) : string.Format("{0} is already banned from using the companion app", @ulong);
			arg.ReplyWith(strValue);
		}

		// Token: 0x0600415D RID: 16733 RVA: 0x00183E90 File Offset: 0x00182090
		[ServerVar]
		public static void appunban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appunban <steamID64>");
				return;
			}
			string strValue = SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, false) ? string.Format("Unbanned {0}, they can use the companion app again", @ulong) : string.Format("{0} is not banned from using the companion app", @ulong);
			arg.ReplyWith(strValue);
		}

		// Token: 0x0600415E RID: 16734 RVA: 0x00183EF4 File Offset: 0x001820F4
		public static IPAddress GetListenIP()
		{
			if (string.IsNullOrWhiteSpace(App.listenip))
			{
				return IPAddress.Any;
			}
			IPAddress ipaddress;
			if (!IPAddress.TryParse(App.listenip, out ipaddress) || ipaddress.AddressFamily != AddressFamily.InterNetwork)
			{
				Debug.LogError("Invalid app.listenip: " + App.listenip);
				return IPAddress.Any;
			}
			return ipaddress;
		}

		// Token: 0x0600415F RID: 16735 RVA: 0x00183F48 File Offset: 0x00182148
		public static string GetPublicIP()
		{
			IPAddress ipaddress;
			if (!string.IsNullOrWhiteSpace(App.publicip) && IPAddress.TryParse(App.publicip, out ipaddress) && ipaddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return App.publicip;
			}
			return SteamServer.PublicIp.ToString();
		}
	}
}
