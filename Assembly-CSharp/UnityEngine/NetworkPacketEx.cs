using System;
using Network;

namespace UnityEngine
{
	// Token: 0x02000A24 RID: 2596
	public static class NetworkPacketEx
	{
		// Token: 0x06003D9F RID: 15775 RVA: 0x00169B20 File Offset: 0x00167D20
		public static BasePlayer Player(this Message v)
		{
			if (v.connection == null)
			{
				return null;
			}
			return v.connection.player as BasePlayer;
		}
	}
}
