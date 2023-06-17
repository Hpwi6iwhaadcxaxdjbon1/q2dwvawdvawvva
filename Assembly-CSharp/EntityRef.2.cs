using System;

// Token: 0x020003ED RID: 1005
public struct EntityRef<T> where T : BaseEntity
{
	// Token: 0x04001A7F RID: 6783
	private EntityRef entityRef;

	// Token: 0x06002274 RID: 8820 RVA: 0x000DE020 File Offset: 0x000DC220
	public EntityRef(NetworkableId uid)
	{
		this.entityRef = new EntityRef
		{
			uid = uid
		};
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06002275 RID: 8821 RVA: 0x000DE044 File Offset: 0x000DC244
	public bool IsSet
	{
		get
		{
			return this.entityRef.IsSet();
		}
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000DE051 File Offset: 0x000DC251
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x000DE064 File Offset: 0x000DC264
	public void Set(T entity)
	{
		this.entityRef.Set(entity);
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000DE078 File Offset: 0x000DC278
	public T Get(bool serverside)
	{
		BaseEntity baseEntity = this.entityRef.Get(serverside);
		if (baseEntity == null)
		{
			return default(T);
		}
		T result;
		if ((result = (baseEntity as T)) == null)
		{
			this.Set(default(T));
			return default(T);
		}
		return result;
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000DE0CD File Offset: 0x000DC2CD
	public bool TryGet(bool serverside, out T entity)
	{
		entity = this.Get(serverside);
		return entity != null;
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x0600227A RID: 8826 RVA: 0x000DE0EA File Offset: 0x000DC2EA
	// (set) Token: 0x0600227B RID: 8827 RVA: 0x000DE0F7 File Offset: 0x000DC2F7
	public NetworkableId uid
	{
		get
		{
			return this.entityRef.uid;
		}
		set
		{
			this.entityRef.uid = value;
		}
	}
}
