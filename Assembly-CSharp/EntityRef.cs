using System;
using UnityEngine;

// Token: 0x020003EC RID: 1004
public struct EntityRef
{
	// Token: 0x04001A7D RID: 6781
	internal BaseEntity ent_cached;

	// Token: 0x04001A7E RID: 6782
	internal NetworkableId id_cached;

	// Token: 0x0600226E RID: 8814 RVA: 0x000DDED8 File Offset: 0x000DC0D8
	public bool IsSet()
	{
		return this.id_cached.IsValid;
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000DDEE5 File Offset: 0x000DC0E5
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000DDEF3 File Offset: 0x000DC0F3
	public void Set(BaseEntity ent)
	{
		this.ent_cached = ent;
		this.id_cached = default(NetworkableId);
		if (this.ent_cached.IsValid())
		{
			this.id_cached = this.ent_cached.net.ID;
		}
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000DDF2C File Offset: 0x000DC12C
	public BaseEntity Get(bool serverside)
	{
		if (this.ent_cached == null && this.id_cached.IsValid)
		{
			if (serverside)
			{
				this.ent_cached = (BaseNetworkable.serverEntities.Find(this.id_cached) as BaseEntity);
			}
			else
			{
				Debug.LogWarning("EntityRef: Looking for clientside entities on pure server!");
			}
		}
		if (!this.ent_cached.IsValid())
		{
			this.ent_cached = null;
		}
		return this.ent_cached;
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06002272 RID: 8818 RVA: 0x000DDF98 File Offset: 0x000DC198
	// (set) Token: 0x06002273 RID: 8819 RVA: 0x000DDFC4 File Offset: 0x000DC1C4
	public NetworkableId uid
	{
		get
		{
			if (this.ent_cached.IsValid())
			{
				this.id_cached = this.ent_cached.net.ID;
			}
			return this.id_cached;
		}
		set
		{
			this.id_cached = value;
			if (!this.id_cached.IsValid)
			{
				this.ent_cached = null;
				return;
			}
			if (this.ent_cached.IsValid() && this.ent_cached.net.ID == this.id_cached)
			{
				return;
			}
			this.ent_cached = null;
		}
	}
}
