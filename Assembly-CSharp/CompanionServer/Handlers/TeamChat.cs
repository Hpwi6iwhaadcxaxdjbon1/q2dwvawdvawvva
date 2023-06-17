using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A03 RID: 2563
	public class TeamChat : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D2F RID: 15663 RVA: 0x00167218 File Offset: 0x00165418
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				base.SendError("no_team");
				return;
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.teamChat = Pool.Get<AppTeamChat>();
			appResponse.teamChat.messages = Pool.GetList<AppChatMessage>();
			IReadOnlyList<ChatLog.Entry> history = Server.TeamChat.GetHistory(playerTeam.teamID);
			if (history != null)
			{
				foreach (ChatLog.Entry entry in history)
				{
					AppChatMessage appChatMessage = Pool.Get<AppChatMessage>();
					appChatMessage.steamId = entry.SteamId;
					appChatMessage.name = entry.Name;
					appChatMessage.message = entry.Message;
					appChatMessage.color = entry.Color;
					appChatMessage.time = entry.Time;
					appResponse.teamChat.messages.Add(appChatMessage);
				}
			}
			base.Send(appResponse);
		}
	}
}
