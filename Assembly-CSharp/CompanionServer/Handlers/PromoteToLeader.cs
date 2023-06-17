using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FF RID: 2559
	public class PromoteToLeader : BaseHandler<AppPromoteToLeader>
	{
		// Token: 0x06003D26 RID: 15654 RVA: 0x00167000 File Offset: 0x00165200
		public override void Execute()
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam == null)
			{
				base.SendError("no_team");
				return;
			}
			if (playerTeam.teamLeader != base.UserId)
			{
				base.SendError("access_denied");
				return;
			}
			if (playerTeam.teamLeader == base.Proto.steamId)
			{
				base.SendSuccess();
				return;
			}
			if (!playerTeam.members.Contains(base.Proto.steamId))
			{
				base.SendError("not_found");
				return;
			}
			playerTeam.SetTeamLeader(base.Proto.steamId);
			base.SendSuccess();
		}
	}
}
