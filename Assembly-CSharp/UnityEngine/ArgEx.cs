using System;

namespace UnityEngine
{
	// Token: 0x02000A1D RID: 2589
	public static class ArgEx
	{
		// Token: 0x06003D83 RID: 15747 RVA: 0x001696EE File Offset: 0x001678EE
		public static BasePlayer Player(this ConsoleSystem.Arg arg)
		{
			if (arg == null || arg.Connection == null)
			{
				return null;
			}
			return arg.Connection.player as BasePlayer;
		}

		// Token: 0x06003D84 RID: 15748 RVA: 0x00169710 File Offset: 0x00167910
		public static BasePlayer GetPlayer(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, null);
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.Find(@string);
		}

		// Token: 0x06003D85 RID: 15749 RVA: 0x00169734 File Offset: 0x00167934
		public static BasePlayer GetSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindSleeping(@string);
		}

		// Token: 0x06003D86 RID: 15750 RVA: 0x0016975C File Offset: 0x0016795C
		public static BasePlayer GetPlayerOrSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindAwakeOrSleeping(@string);
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x00169784 File Offset: 0x00167984
		public static BasePlayer GetPlayerOrSleeperOrBot(this ConsoleSystem.Arg arg, int iArgNum)
		{
			uint num;
			if (arg.TryGetUInt(iArgNum, out num))
			{
				return BasePlayer.FindBot((ulong)num);
			}
			return arg.GetPlayerOrSleeper(iArgNum);
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x001697AB File Offset: 0x001679AB
		public static NetworkableId GetEntityID(this ConsoleSystem.Arg arg, int iArg, NetworkableId def = default(NetworkableId))
		{
			return new NetworkableId(arg.GetUInt64(iArg, def.Value));
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x001697BF File Offset: 0x001679BF
		public static ItemId GetItemID(this ConsoleSystem.Arg arg, int iArg, ItemId def = default(ItemId))
		{
			return new ItemId(arg.GetUInt64(iArg, def.Value));
		}
	}
}
