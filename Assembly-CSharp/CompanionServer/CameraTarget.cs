using System;

namespace CompanionServer
{
	// Token: 0x020009DE RID: 2526
	public readonly struct CameraTarget : IEquatable<CameraTarget>
	{
		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06003C56 RID: 15446 RVA: 0x001633BC File Offset: 0x001615BC
		public NetworkableId EntityId { get; }

		// Token: 0x06003C57 RID: 15447 RVA: 0x001633C4 File Offset: 0x001615C4
		public CameraTarget(NetworkableId entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x06003C58 RID: 15448 RVA: 0x001633CD File Offset: 0x001615CD
		public bool Equals(CameraTarget other)
		{
			return this.EntityId == other.EntityId;
		}

		// Token: 0x06003C59 RID: 15449 RVA: 0x001633E4 File Offset: 0x001615E4
		public override bool Equals(object obj)
		{
			if (obj is CameraTarget)
			{
				CameraTarget other = (CameraTarget)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003C5A RID: 15450 RVA: 0x0016340C File Offset: 0x0016160C
		public override int GetHashCode()
		{
			return this.EntityId.GetHashCode();
		}

		// Token: 0x06003C5B RID: 15451 RVA: 0x0016342D File Offset: 0x0016162D
		public static bool operator ==(CameraTarget left, CameraTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003C5C RID: 15452 RVA: 0x00163437 File Offset: 0x00161637
		public static bool operator !=(CameraTarget left, CameraTarget right)
		{
			return !left.Equals(right);
		}
	}
}
