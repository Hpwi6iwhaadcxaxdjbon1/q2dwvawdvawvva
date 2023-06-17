using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003EB RID: 1003
public static class EntityLinkEx
{
	// Token: 0x0600226B RID: 8811 RVA: 0x000DDE3C File Offset: 0x000DC03C
	public static void FreeLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			EntityLink entityLink = links[i];
			entityLink.Clear();
			Pool.Free<EntityLink>(ref entityLink);
		}
		links.Clear();
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000DDE78 File Offset: 0x000DC078
	public static void ClearLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			links[i].Clear();
		}
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000DDEA4 File Offset: 0x000DC0A4
	public static void AddLinks(this List<EntityLink> links, BaseEntity entity, Socket_Base[] sockets)
	{
		foreach (Socket_Base socket in sockets)
		{
			EntityLink entityLink = Pool.Get<EntityLink>();
			entityLink.Setup(entity, socket);
			links.Add(entityLink);
		}
	}
}
