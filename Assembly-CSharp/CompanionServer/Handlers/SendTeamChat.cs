using System;
using ConVar;
using Facepunch.Extend;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A00 RID: 2560
	public class SendTeamChat : BaseHandler<AppSendMessage>
	{
		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06003D28 RID: 15656 RVA: 0x001670A4 File Offset: 0x001652A4
		protected override double TokenCost
		{
			get
			{
				return 2.0;
			}
		}

		// Token: 0x06003D29 RID: 15657 RVA: 0x001670B0 File Offset: 0x001652B0
		public override void Execute()
		{
			string message = base.Proto.message;
			string text = (message != null) ? message.Trim() : null;
			if (string.IsNullOrWhiteSpace(text))
			{
				base.SendSuccess();
				return;
			}
			text = text.Truncate(256, "…");
			global::BasePlayer player = base.Player;
			string text2;
			if ((text2 = ((player != null) ? player.displayName : null)) == null)
			{
				text2 = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(base.UserId) ?? "[unknown]");
			}
			string username = text2;
			if (Chat.sayAs(Chat.ChatChannel.Team, base.UserId, username, text, base.Player))
			{
				base.SendSuccess();
				return;
			}
			base.SendError("message_not_sent");
		}
	}
}
