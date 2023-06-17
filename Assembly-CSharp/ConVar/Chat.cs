using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CircularBuffer;
using CompanionServer;
using Facepunch;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA9 RID: 2729
	[ConsoleSystem.Factory("chat")]
	public class Chat : ConsoleSystem
	{
		// Token: 0x04003B09 RID: 15113
		[ServerVar]
		public static float localChatRange = 100f;

		// Token: 0x04003B0A RID: 15114
		[ReplicatedVar]
		public static bool globalchat = true;

		// Token: 0x04003B0B RID: 15115
		[ReplicatedVar]
		public static bool localchat = false;

		// Token: 0x04003B0C RID: 15116
		private const float textVolumeBoost = 0.2f;

		// Token: 0x04003B0D RID: 15117
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003B0E RID: 15118
		[ServerVar(Help = "Number of messages to keep in memory for chat history")]
		public static int historysize = 1000;

		// Token: 0x04003B0F RID: 15119
		private static CircularBuffer<Chat.ChatEntry> History = new CircularBuffer<Chat.ChatEntry>(Chat.historysize);

		// Token: 0x04003B10 RID: 15120
		[ServerVar]
		public static bool serverlog = true;

		// Token: 0x0600416D RID: 16749 RVA: 0x00184184 File Offset: 0x00182384
		public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0UL)
		{
			string text = username.EscapeRichText();
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				string.Concat(new string[]
				{
					"<color=",
					color,
					">",
					text,
					"</color> ",
					message
				})
			});
			Chat.Record(new Chat.ChatEntry
			{
				Channel = Chat.ChatChannel.Server,
				Message = message,
				UserId = userid.ToString(),
				Username = username,
				Color = color,
				Time = Epoch.Current
			});
		}

		// Token: 0x0600416E RID: 16750 RVA: 0x00184234 File Offset: 0x00182434
		[ServerUserVar]
		public static void say(ConsoleSystem.Arg arg)
		{
			if (Chat.globalchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Global, arg);
			}
		}

		// Token: 0x0600416F RID: 16751 RVA: 0x00184244 File Offset: 0x00182444
		[ServerUserVar]
		public static void localsay(ConsoleSystem.Arg arg)
		{
			if (Chat.localchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Local, arg);
			}
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x00184254 File Offset: 0x00182454
		[ServerUserVar]
		public static void teamsay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Team, arg);
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x0018425D File Offset: 0x0018245D
		[ServerUserVar]
		public static void cardgamesay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Cards, arg);
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x00184268 File Offset: 0x00182468
		private static void sayImpl(Chat.ChatChannel targetChannel, ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				arg.ReplyWith("Chat is disabled.");
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
			{
				if (basePlayer.NextChatTime == 0f)
				{
					basePlayer.NextChatTime = Time.realtimeSinceStartup - 30f;
				}
				if (basePlayer.NextChatTime > Time.realtimeSinceStartup)
				{
					basePlayer.NextChatTime += 2f;
					float num = basePlayer.NextChatTime - Time.realtimeSinceStartup;
					ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add", new object[]
					{
						2,
						0,
						"You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds"
					});
					if (num > 120f)
					{
						basePlayer.Kick("Chatting too fast");
					}
					return;
				}
			}
			string @string = arg.GetString(0, "text");
			bool flag = Chat.sayAs(targetChannel, basePlayer.userID, basePlayer.displayName, @string, basePlayer);
			Analytics.Azure.OnChatMessage(basePlayer, @string, (int)targetChannel);
			if (flag)
			{
				basePlayer.NextChatTime = Time.realtimeSinceStartup + 1.5f;
			}
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x001843AC File Offset: 0x001825AC
		internal static bool sayAs(Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			if (!player)
			{
				player = null;
			}
			if (!Chat.enabled)
			{
				return false;
			}
			if (player != null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return false;
			}
			ServerUsers.User user = ServerUsers.Get(userId);
			ServerUsers.UserGroup userGroup = (user != null) ? user.group : ServerUsers.UserGroup.None;
			if (userGroup == ServerUsers.UserGroup.Banned)
			{
				return false;
			}
			string text = message.Replace("\n", "").Replace("\r", "").Trim();
			if (text.Length > 128)
			{
				text = text.Substring(0, 128);
			}
			if (text.Length <= 0)
			{
				return false;
			}
			if (text.StartsWith("/") || text.StartsWith("\\"))
			{
				return false;
			}
			text = text.EscapeRichText();
			if (Chat.serverlog)
			{
				ServerConsole.PrintColoured(new object[]
				{
					ConsoleColor.DarkYellow,
					string.Concat(new object[]
					{
						"[",
						targetChannel,
						"] ",
						username,
						": "
					}),
					ConsoleColor.DarkGreen,
					text
				});
				string str = ((player != null) ? player.ToString() : null) ?? string.Format("{0}[{1}]", username, userId);
				if (targetChannel == Chat.ChatChannel.Team)
				{
					DebugEx.Log("[TEAM CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
				else if (targetChannel == Chat.ChatChannel.Cards)
				{
					DebugEx.Log("[CARDS CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
				else
				{
					DebugEx.Log("[CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
			}
			bool flag = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
			bool flag2 = (player != null) ? player.IsDeveloper : DeveloperList.Contains(userId);
			string text2 = "#5af";
			if (flag)
			{
				text2 = "#af5";
			}
			if (flag2)
			{
				text2 = "#fa5";
			}
			string text3 = username.EscapeRichText();
			Chat.Record(new Chat.ChatEntry
			{
				Channel = targetChannel,
				Message = text,
				UserId = ((player != null) ? player.UserIDString : userId.ToString()),
				Username = username,
				Color = text2,
				Time = Epoch.Current
			});
			switch (targetChannel)
			{
			case Chat.ChatChannel.Global:
				ConsoleNetwork.BroadcastToAllClients("chat.add2", new object[]
				{
					0,
					userId,
					text,
					text3,
					text2,
					1f
				});
				return true;
			case Chat.ChatChannel.Team:
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
				if (playerTeam == null)
				{
					return false;
				}
				List<Network.Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
				if (onlineMemberConnections != null)
				{
					ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", new object[]
					{
						1,
						userId,
						text,
						text3,
						text2,
						1f
					});
				}
				playerTeam.BroadcastTeamChat(userId, text3, text, text2);
				return true;
			}
			case Chat.ChatChannel.Cards:
			{
				if (player == null)
				{
					return false;
				}
				if (!player.isMounted)
				{
					return false;
				}
				BaseCardGameEntity baseCardGameEntity = player.GetMountedVehicle() as BaseCardGameEntity;
				if (baseCardGameEntity == null || !baseCardGameEntity.GameController.IsAtTable(player))
				{
					return false;
				}
				List<Network.Connection> list = Pool.GetList<Network.Connection>();
				baseCardGameEntity.GameController.GetConnectionsInGame(list);
				if (list.Count > 0)
				{
					ConsoleNetwork.SendClientCommand(list, "chat.add2", new object[]
					{
						3,
						userId,
						text,
						text3,
						text2,
						1f
					});
				}
				Pool.FreeList<Network.Connection>(ref list);
				return true;
			}
			case Chat.ChatChannel.Local:
				if (player != null)
				{
					float num = Chat.localChatRange * Chat.localChatRange;
					foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
					{
						float sqrMagnitude = (basePlayer.transform.position - player.transform.position).sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add2", new object[]
							{
								4,
								userId,
								text,
								text3,
								text2,
								Mathf.Clamp01(sqrMagnitude / num + 0.2f)
							});
						}
					}
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x0018483C File Offset: 0x00182A3C
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Chat.ChatEntry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Chat.History.Size - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Chat.History.Skip(num);
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x00184874 File Offset: 0x00182A74
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Chat.ChatEntry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Chat.ChatEntry>();
			}
			return from x in Chat.History
			where x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase)
			select x;
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x001848BC File Offset: 0x00182ABC
		private static void Record(Chat.ChatEntry ce)
		{
			int num = Mathf.Max(Chat.historysize, 10);
			if (Chat.History.Capacity != num)
			{
				CircularBuffer<Chat.ChatEntry> circularBuffer = new CircularBuffer<Chat.ChatEntry>(num);
				foreach (Chat.ChatEntry item in Chat.History)
				{
					circularBuffer.PushBack(item);
				}
				Chat.History = circularBuffer;
			}
			Chat.History.PushBack(ce);
			RCon.Broadcast(RCon.LogType.Chat, ce);
		}

		// Token: 0x02000F4A RID: 3914
		public enum ChatChannel
		{
			// Token: 0x04004F54 RID: 20308
			Global,
			// Token: 0x04004F55 RID: 20309
			Team,
			// Token: 0x04004F56 RID: 20310
			Server,
			// Token: 0x04004F57 RID: 20311
			Cards,
			// Token: 0x04004F58 RID: 20312
			Local
		}

		// Token: 0x02000F4B RID: 3915
		public struct ChatEntry
		{
			// Token: 0x1700072E RID: 1838
			// (get) Token: 0x06005458 RID: 21592 RVA: 0x001B537E File Offset: 0x001B357E
			// (set) Token: 0x06005459 RID: 21593 RVA: 0x001B5386 File Offset: 0x001B3586
			public Chat.ChatChannel Channel { get; set; }

			// Token: 0x1700072F RID: 1839
			// (get) Token: 0x0600545A RID: 21594 RVA: 0x001B538F File Offset: 0x001B358F
			// (set) Token: 0x0600545B RID: 21595 RVA: 0x001B5397 File Offset: 0x001B3597
			public string Message { get; set; }

			// Token: 0x17000730 RID: 1840
			// (get) Token: 0x0600545C RID: 21596 RVA: 0x001B53A0 File Offset: 0x001B35A0
			// (set) Token: 0x0600545D RID: 21597 RVA: 0x001B53A8 File Offset: 0x001B35A8
			public string UserId { get; set; }

			// Token: 0x17000731 RID: 1841
			// (get) Token: 0x0600545E RID: 21598 RVA: 0x001B53B1 File Offset: 0x001B35B1
			// (set) Token: 0x0600545F RID: 21599 RVA: 0x001B53B9 File Offset: 0x001B35B9
			public string Username { get; set; }

			// Token: 0x17000732 RID: 1842
			// (get) Token: 0x06005460 RID: 21600 RVA: 0x001B53C2 File Offset: 0x001B35C2
			// (set) Token: 0x06005461 RID: 21601 RVA: 0x001B53CA File Offset: 0x001B35CA
			public string Color { get; set; }

			// Token: 0x17000733 RID: 1843
			// (get) Token: 0x06005462 RID: 21602 RVA: 0x001B53D3 File Offset: 0x001B35D3
			// (set) Token: 0x06005463 RID: 21603 RVA: 0x001B53DB File Offset: 0x001B35DB
			public int Time { get; set; }
		}
	}
}
