using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009E8 RID: 2536
	public class NotificationList
	{
		// Token: 0x040036BE RID: 14014
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/push/send";

		// Token: 0x040036BF RID: 14015
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x040036C0 RID: 14016
		private readonly HashSet<ulong> _subscriptions = new HashSet<ulong>();

		// Token: 0x040036C1 RID: 14017
		private double _lastSend;

		// Token: 0x06003CA0 RID: 15520 RVA: 0x00164A85 File Offset: 0x00162C85
		public bool AddSubscription(ulong steamId)
		{
			return steamId != 0UL && this._subscriptions.Count < 50 && this._subscriptions.Add(steamId);
		}

		// Token: 0x06003CA1 RID: 15521 RVA: 0x00164AA9 File Offset: 0x00162CA9
		public bool RemoveSubscription(ulong steamId)
		{
			return this._subscriptions.Remove(steamId);
		}

		// Token: 0x06003CA2 RID: 15522 RVA: 0x00164AB7 File Offset: 0x00162CB7
		public bool HasSubscription(ulong steamId)
		{
			return this._subscriptions.Contains(steamId);
		}

		// Token: 0x06003CA3 RID: 15523 RVA: 0x00164AC8 File Offset: 0x00162CC8
		public List<ulong> ToList()
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item in this._subscriptions)
			{
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06003CA4 RID: 15524 RVA: 0x00164B24 File Offset: 0x00162D24
		public void LoadFrom(List<ulong> steamIds)
		{
			this._subscriptions.Clear();
			if (steamIds == null)
			{
				return;
			}
			foreach (ulong item in steamIds)
			{
				this._subscriptions.Add(item);
			}
		}

		// Token: 0x06003CA5 RID: 15525 RVA: 0x00164B88 File Offset: 0x00162D88
		public void IntersectWith(List<PlayerNameID> players)
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (PlayerNameID playerNameID in players)
			{
				list.Add(playerNameID.userid);
			}
			this._subscriptions.IntersectWith(list);
			Facepunch.Pool.FreeList<ulong>(ref list);
		}

		// Token: 0x06003CA6 RID: 15526 RVA: 0x00164BF4 File Offset: 0x00162DF4
		public Task<NotificationSendResult> SendNotification(NotificationChannel channel, string title, string body, string type)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (realtimeSinceStartup - this._lastSend < 15.0)
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.RateLimited);
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			if (!string.IsNullOrWhiteSpace(type))
			{
				serverPairingData["type"] = type;
			}
			this._lastSend = realtimeSinceStartup;
			return NotificationList.SendNotificationImpl(this._subscriptions, channel, title, body, serverPairingData);
		}

		// Token: 0x06003CA7 RID: 15527 RVA: 0x00164C54 File Offset: 0x00162E54
		public static async Task<NotificationSendResult> SendNotificationTo(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult notificationSendResult = await NotificationList.SendNotificationImpl(steamIds, channel, title, body, data);
			if (notificationSendResult == NotificationSendResult.NoTargetsFound)
			{
				notificationSendResult = NotificationSendResult.Sent;
			}
			return notificationSendResult;
		}

		// Token: 0x06003CA8 RID: 15528 RVA: 0x00164CBC File Offset: 0x00162EBC
		public static async Task<NotificationSendResult> SendNotificationTo(ulong steamId, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			HashSet<ulong> set = Facepunch.Pool.Get<HashSet<ulong>>();
			set.Clear();
			set.Add(steamId);
			NotificationSendResult result = await NotificationList.SendNotificationImpl(set, channel, title, body, data);
			set.Clear();
			Facepunch.Pool.Free<HashSet<ulong>>(ref set);
			return result;
		}

		// Token: 0x06003CA9 RID: 15529 RVA: 0x00164D24 File Offset: 0x00162F24
		private static async Task<NotificationSendResult> SendNotificationImpl(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult result;
			if (!CompanionServer.Server.IsEnabled || !App.notifications)
			{
				result = NotificationSendResult.Disabled;
			}
			else if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
			{
				result = NotificationSendResult.Empty;
			}
			else if (steamIds.Count == 0)
			{
				result = NotificationSendResult.Sent;
			}
			else
			{
				PushRequest pushRequest = Facepunch.Pool.Get<PushRequest>();
				pushRequest.ServerToken = CompanionServer.Server.Token;
				pushRequest.Channel = channel;
				pushRequest.Title = title;
				pushRequest.Body = body;
				pushRequest.Data = data;
				pushRequest.SteamIds = Facepunch.Pool.GetList<ulong>();
				foreach (ulong item in steamIds)
				{
					pushRequest.SteamIds.Add(item);
				}
				string content = JsonConvert.SerializeObject(pushRequest);
				Facepunch.Pool.Free<PushRequest>(ref pushRequest);
				try
				{
					StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
					HttpResponseMessage httpResponseMessage = await NotificationList.Http.PostAsync("https://companion-rust.facepunch.com/api/push/send", content2);
					if (!httpResponseMessage.IsSuccessStatusCode)
					{
						DebugEx.LogWarning(string.Format("Failed to send notification: {0}", httpResponseMessage.StatusCode), StackTraceLogType.None);
						result = NotificationSendResult.ServerError;
					}
					else if (httpResponseMessage.StatusCode == HttpStatusCode.Accepted)
					{
						result = NotificationSendResult.NoTargetsFound;
					}
					else
					{
						result = NotificationSendResult.Sent;
					}
				}
				catch (Exception arg)
				{
					DebugEx.LogWarning(string.Format("Exception thrown when sending notification: {0}", arg), StackTraceLogType.None);
					result = NotificationSendResult.Failed;
				}
			}
			return result;
		}
	}
}
