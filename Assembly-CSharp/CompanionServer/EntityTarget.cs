using System;

namespace CompanionServer
{
	// Token: 0x020009E3 RID: 2531
	public readonly struct EntityTarget : IEquatable<EntityTarget>
	{
		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06003C84 RID: 15492 RVA: 0x001641C0 File Offset: 0x001623C0
		public NetworkableId EntityId { get; }

		// Token: 0x06003C85 RID: 15493 RVA: 0x001641C8 File Offset: 0x001623C8
		public EntityTarget(NetworkableId entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x001641D1 File Offset: 0x001623D1
		public bool Equals(EntityTarget other)
		{
			return this.EntityId == other.EntityId;
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x001641E8 File Offset: 0x001623E8
		public override bool Equals(object obj)
		{
			if (obj is EntityTarget)
			{
				EntityTarget other = (EntityTarget)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x00164210 File Offset: 0x00162410
		public override int GetHashCode()
		{
			return this.EntityId.GetHashCode();
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x00164231 File Offset: 0x00162431
		public static bool operator ==(EntityTarget left, EntityTarget right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x0016423B File Offset: 0x0016243B
		public static bool operator !=(EntityTarget left, EntityTarget right)
		{
			return !left.Equals(right);
		}
	}
}
