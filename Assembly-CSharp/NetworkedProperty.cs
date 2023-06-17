using System;

// Token: 0x02000963 RID: 2403
public class NetworkedProperty<T> where T : IEquatable<T>
{
	// Token: 0x040033C4 RID: 13252
	private T val;

	// Token: 0x040033C5 RID: 13253
	private BaseEntity entity;

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x060039B8 RID: 14776 RVA: 0x00156C19 File Offset: 0x00154E19
	// (set) Token: 0x060039B9 RID: 14777 RVA: 0x00156C21 File Offset: 0x00154E21
	public T Value
	{
		get
		{
			return this.val;
		}
		set
		{
			if (!this.val.Equals(value))
			{
				this.val = value;
				if (this.entity.isServer)
				{
					this.entity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				}
			}
		}
	}

	// Token: 0x060039BA RID: 14778 RVA: 0x00156C57 File Offset: 0x00154E57
	public NetworkedProperty(BaseEntity entity)
	{
		this.entity = entity;
	}

	// Token: 0x060039BB RID: 14779 RVA: 0x00156C66 File Offset: 0x00154E66
	public static implicit operator T(NetworkedProperty<T> value)
	{
		return value.Value;
	}
}
