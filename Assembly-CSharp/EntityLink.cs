using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003EA RID: 1002
public class EntityLink : Pool.IPooled
{
	// Token: 0x04001A79 RID: 6777
	public BaseEntity owner;

	// Token: 0x04001A7A RID: 6778
	public Socket_Base socket;

	// Token: 0x04001A7B RID: 6779
	public List<EntityLink> connections = new List<EntityLink>(8);

	// Token: 0x04001A7C RID: 6780
	public int capacity = int.MaxValue;

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x0600225D RID: 8797 RVA: 0x000DDCAD File Offset: 0x000DBEAD
	public string name
	{
		get
		{
			return this.socket.socketName;
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000DDCBA File Offset: 0x000DBEBA
	public void Setup(BaseEntity owner, Socket_Base socket)
	{
		this.owner = owner;
		this.socket = socket;
		if (socket.monogamous)
		{
			this.capacity = 1;
		}
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000DDCD9 File Offset: 0x000DBED9
	public void EnterPool()
	{
		this.owner = null;
		this.socket = null;
		this.capacity = int.MaxValue;
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000063A5 File Offset: 0x000045A5
	public void LeavePool()
	{
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000DDCF4 File Offset: 0x000DBEF4
	public bool Contains(EntityLink entity)
	{
		return this.connections.Contains(entity);
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000DDD02 File Offset: 0x000DBF02
	public void Add(EntityLink entity)
	{
		this.connections.Add(entity);
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000DDD10 File Offset: 0x000DBF10
	public void Remove(EntityLink entity)
	{
		this.connections.Remove(entity);
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000DDD20 File Offset: 0x000DBF20
	public void Clear()
	{
		for (int i = 0; i < this.connections.Count; i++)
		{
			this.connections[i].Remove(this);
		}
		this.connections.Clear();
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000DDD60 File Offset: 0x000DBF60
	public bool IsEmpty()
	{
		return this.connections.Count == 0;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000DDD70 File Offset: 0x000DBF70
	public bool IsOccupied()
	{
		return this.connections.Count >= this.capacity;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000DDD88 File Offset: 0x000DBF88
	public bool IsMale()
	{
		return this.socket.male;
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000DDD95 File Offset: 0x000DBF95
	public bool IsFemale()
	{
		return this.socket.female;
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000DDDA4 File Offset: 0x000DBFA4
	public bool CanConnect(EntityLink link)
	{
		return !this.IsOccupied() && link != null && !link.IsOccupied() && this.socket.CanConnect(this.owner.transform.position, this.owner.transform.rotation, link.socket, link.owner.transform.position, link.owner.transform.rotation);
	}
}
