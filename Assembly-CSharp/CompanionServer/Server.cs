using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009EB RID: 2539
	public static class Server
	{
		// Token: 0x040036C9 RID: 14025
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/server";

		// Token: 0x040036CA RID: 14026
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x040036CB RID: 14027
		internal static readonly ChatLog TeamChat = new ChatLog();

		// Token: 0x040036CC RID: 14028
		internal static string Token;

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06003CB6 RID: 15542 RVA: 0x00164E66 File Offset: 0x00163066
		// (set) Token: 0x06003CB7 RID: 15543 RVA: 0x00164E6D File Offset: 0x0016306D
		public static Listener Listener { get; private set; }

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06003CB8 RID: 15544 RVA: 0x00164E75 File Offset: 0x00163075
		public static bool IsEnabled
		{
			get
			{
				return App.port >= 0 && !string.IsNullOrWhiteSpace(App.serverid) && Server.Listener != null;
			}
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x00164E98 File Offset: 0x00163098
		public static void Initialize()
		{
			if (App.port < 0)
			{
				return;
			}
			if (Server.IsEnabled)
			{
				UnityEngine.Debug.LogWarning("Rust+ is already started up! Skipping second startup");
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (activeGameMode != null && !activeGameMode.rustPlus)
			{
				return;
			}
			Map.PopulateCache();
			if (App.port == 0)
			{
				App.port = Math.Max(Server.port, RCon.Port) + 67;
			}
			try
			{
				Server.Listener = new Listener(App.GetListenIP(), App.port);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Companion server failed to start: {0}", arg));
			}
			Server.PostInitializeServer();
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x00164F3C File Offset: 0x0016313C
		public static void Shutdown()
		{
			Server.SetServerId(null);
			Listener listener = Server.Listener;
			if (listener != null)
			{
				listener.Dispose();
			}
			Server.Listener = null;
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x00164F5A File Offset: 0x0016315A
		public static void Update()
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			listener.Update();
		}

		// Token: 0x06003CBC RID: 15548 RVA: 0x00164F6B File Offset: 0x0016316B
		public static void Broadcast(PlayerTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<PlayerTarget, Connection, AppBroadcast> playerSubscribers = listener.PlayerSubscribers;
			if (playerSubscribers == null)
			{
				return;
			}
			playerSubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CBD RID: 15549 RVA: 0x00164F88 File Offset: 0x00163188
		public static void Broadcast(EntityTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<EntityTarget, Connection, AppBroadcast> entitySubscribers = listener.EntitySubscribers;
			if (entitySubscribers == null)
			{
				return;
			}
			entitySubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x00164FA5 File Offset: 0x001631A5
		public static void Broadcast(CameraTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<CameraTarget, Connection, AppBroadcast> cameraSubscribers = listener.CameraSubscribers;
			if (cameraSubscribers == null)
			{
				return;
			}
			cameraSubscribers.Send(target, broadcast);
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x00164FC4 File Offset: 0x001631C4
		public static bool HasAnySubscribers(CameraTarget target)
		{
			Listener listener = Server.Listener;
			bool? flag;
			if (listener == null)
			{
				flag = null;
			}
			else
			{
				SubscriberList<CameraTarget, Connection, AppBroadcast> cameraSubscribers = listener.CameraSubscribers;
				flag = ((cameraSubscribers != null) ? new bool?(cameraSubscribers.HasAnySubscribers(target)) : null);
			}
			return flag ?? false;
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x00165017 File Offset: 0x00163217
		public static bool CanSendPairingNotification(ulong playerId)
		{
			Listener listener = Server.Listener;
			return listener != null && listener.CanSendPairingNotification(playerId);
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x0016502C File Offset: 0x0016322C
		private static async void PostInitializeServer()
		{
			await Server.SetupServerRegistration();
			await Server.CheckConnectivity();
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x00165060 File Offset: 0x00163260
		private static async Task SetupServerRegistration()
		{
			try
			{
				string text;
				string content;
				if (Server.TryLoadServerRegistration(out text, out content))
				{
					StringContent content2 = new StringContent(content, Encoding.UTF8, "text/plain");
					HttpResponseMessage httpResponseMessage = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server/refresh", content2);
					if (httpResponseMessage.IsSuccessStatusCode)
					{
						Server.SetServerRegistration(await httpResponseMessage.Content.ReadAsStringAsync());
						return;
					}
					UnityEngine.Debug.LogWarning("Failed to refresh server ID - registering a new one");
				}
				Server.SetServerRegistration(await Server.Http.GetStringAsync("https://companion-rust.facepunch.com/api/server/register"));
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to setup companion server registration: {0}", arg));
			}
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x001650A0 File Offset: 0x001632A0
		private static bool TryLoadServerRegistration(out string serverId, out string serverToken)
		{
			serverId = null;
			serverToken = null;
			string serverIdPath = Server.GetServerIdPath();
			if (!File.Exists(serverIdPath))
			{
				return false;
			}
			bool result;
			try
			{
				Server.RegisterResponse registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(File.ReadAllText(serverIdPath));
				serverId = registerResponse.ServerId;
				serverToken = registerResponse.ServerToken;
				result = true;
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to load companion server registration: {0}", arg));
				result = false;
			}
			return result;
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x0016510C File Offset: 0x0016330C
		private static void SetServerRegistration(string responseJson)
		{
			Server.RegisterResponse registerResponse = null;
			try
			{
				registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(responseJson);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to parse registration response JSON: {0}\n\n{1}", responseJson, arg));
			}
			Server.SetServerId((registerResponse != null) ? registerResponse.ServerId : null);
			Server.Token = ((registerResponse != null) ? registerResponse.ServerToken : null);
			if (registerResponse == null)
			{
				return;
			}
			try
			{
				File.WriteAllText(Server.GetServerIdPath(), responseJson);
			}
			catch (Exception arg2)
			{
				UnityEngine.Debug.LogError(string.Format("Unable to save companion app server registration - server ID may be different after restart: {0}", arg2));
			}
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x0016519C File Offset: 0x0016339C
		private static async Task CheckConnectivity()
		{
			if (!Server.IsEnabled)
			{
				Server.SetServerId(null);
			}
			else
			{
				try
				{
					string arg = await Server.GetPublicIPAsync();
					StringContent content = new StringContent("", Encoding.UTF8, "text/plain");
					HttpResponseMessage testResponse = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server" + string.Format("/test_connection?address={0}&port={1}", arg, App.port), content);
					string text = await testResponse.Content.ReadAsStringAsync();
					Server.TestConnectionResponse testConnectionResponse = null;
					try
					{
						testConnectionResponse = JsonConvert.DeserializeObject<Server.TestConnectionResponse>(text);
					}
					catch (Exception arg2)
					{
						UnityEngine.Debug.LogError(string.Format("Failed to parse connectivity test response JSON: {0}\n\n{1}", text, arg2));
					}
					if (testConnectionResponse != null)
					{
						string text2 = string.Join("\n", testConnectionResponse.Messages ?? Enumerable.Empty<string>());
						if (testResponse.StatusCode == (HttpStatusCode)555)
						{
							UnityEngine.Debug.LogError("Rust+ companion server connectivity test failed! Disabling Rust+ features.\n\n" + text2);
							Server.SetServerId(null);
						}
						else
						{
							testResponse.EnsureSuccessStatusCode();
							if (!string.IsNullOrWhiteSpace(text2))
							{
								UnityEngine.Debug.LogWarning("Rust+ companion server connectivity test has warnings:\n" + text2);
							}
						}
					}
					testResponse = null;
				}
				catch (Exception arg3)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to check connectivity to the companion server: {0}", arg3));
				}
			}
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x001651DC File Offset: 0x001633DC
		private static async Task<string> GetPublicIPAsync()
		{
			Stopwatch timer = Stopwatch.StartNew();
			string publicIP;
			for (;;)
			{
				bool flag = timer.Elapsed.TotalMinutes > 2.0;
				publicIP = App.GetPublicIP();
				if (flag || (!string.IsNullOrWhiteSpace(publicIP) && publicIP != "0.0.0.0"))
				{
					break;
				}
				await Task.Delay(10000);
			}
			return publicIP;
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x00165219 File Offset: 0x00163419
		private static void SetServerId(string serverId)
		{
			ConsoleSystem.Command command = ConsoleSystem.Index.Server.Find("app.serverid");
			if (command == null)
			{
				return;
			}
			command.Set(serverId ?? "");
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x00165239 File Offset: 0x00163439
		private static string GetServerIdPath()
		{
			return Path.Combine(Server.rootFolder, "companion.id");
		}

		// Token: 0x02000EF1 RID: 3825
		private class RegisterResponse
		{
			// Token: 0x04004DC8 RID: 19912
			public string ServerId;

			// Token: 0x04004DC9 RID: 19913
			public string ServerToken;
		}

		// Token: 0x02000EF2 RID: 3826
		private class TestConnectionResponse
		{
			// Token: 0x04004DCA RID: 19914
			public List<string> Messages;
		}
	}
}
