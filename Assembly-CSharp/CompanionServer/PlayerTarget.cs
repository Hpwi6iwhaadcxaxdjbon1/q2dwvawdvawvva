using System;

namespace CompanionServer
{
	// Token: 0x020009E9 RID: 2537
	public readonly struct PlayerTarget : IEquatable<PlayerTarget>
	{
		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06003CAC RID: 15532 RVA: 0x00164DA9 File Offset: 0x00162FA9
		public ulong SteamId { get; }

		// Token: 0x06003CAD RID: 15533 RVA: 0x00164DB1 File Offset: 0x00162FB1
		public PlayerTarget(ulong steamId)
		{
			this.SteamId = steamId;
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x00164DBA File Offset: 0x00162FBA
		public bool Equals(PlayerTarget other)
		{
			return this.SteamId == other.SteamId;
		}

		// Token: 0x06003CAF RID: 15535 RVA: 0x00164DCC File Offset: 0x00162FCC
		public override bool Equals(object obj)
		{
			if (obj is PlayerTarget)
			{
				PlayerTarget other = (PlayerTarget)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003CB0 RID: 15536 RVA: 0x00164DF4 File Offset: 0x00162FF4
		public override int GetHashCode()
		{
			return this.SteamId.GetHashCode();
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x00164E0F File Offset: 0x0016300F
		public static bool operator ==(PlayerTarget left, PlayerTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003CB2 RID: 15538 RVA: 0x00164E19 File Offset: 0x00163019
		public static bool operator !=(PlayerTarget left, PlayerTarget right)
		{
			return !left.Equals(right);
		}
	}
}
