using System;

// Token: 0x02000102 RID: 258
public struct CameraViewerId : IEquatable<CameraViewerId>
{
	// Token: 0x04000DE0 RID: 3552
	public readonly ulong SteamId;

	// Token: 0x04000DE1 RID: 3553
	public readonly long ConnectionId;

	// Token: 0x0600159B RID: 5531 RVA: 0x000AAB5D File Offset: 0x000A8D5D
	public CameraViewerId(ulong steamId, long connectionId)
	{
		this.SteamId = steamId;
		this.ConnectionId = connectionId;
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x000AAB6D File Offset: 0x000A8D6D
	public bool Equals(CameraViewerId other)
	{
		return this.SteamId == other.SteamId && this.ConnectionId == other.ConnectionId;
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x000AAB90 File Offset: 0x000A8D90
	public override bool Equals(object obj)
	{
		if (obj is CameraViewerId)
		{
			CameraViewerId other = (CameraViewerId)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000AABB8 File Offset: 0x000A8DB8
	public override int GetHashCode()
	{
		return this.SteamId.GetHashCode() * 397 ^ this.ConnectionId.GetHashCode();
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x000AABE8 File Offset: 0x000A8DE8
	public static bool operator ==(CameraViewerId left, CameraViewerId right)
	{
		return left.Equals(right);
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x000AABF2 File Offset: 0x000A8DF2
	public static bool operator !=(CameraViewerId left, CameraViewerId right)
	{
		return !left.Equals(right);
	}
}
