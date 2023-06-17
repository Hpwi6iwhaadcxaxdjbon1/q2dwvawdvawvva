using System;
using System.Collections.Generic;

// Token: 0x02000455 RID: 1109
public class ServerStatistics
{
	// Token: 0x04001D34 RID: 7476
	private BasePlayer player;

	// Token: 0x04001D35 RID: 7477
	private ServerStatistics.Storage storage;

	// Token: 0x04001D36 RID: 7478
	private static Dictionary<ulong, ServerStatistics.Storage> players = new Dictionary<ulong, ServerStatistics.Storage>();

	// Token: 0x060024C8 RID: 9416 RVA: 0x000E945A File Offset: 0x000E765A
	public ServerStatistics(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000E9469 File Offset: 0x000E7669
	public void Init()
	{
		this.storage = ServerStatistics.Get(this.player.userID);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Save()
	{
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000E9481 File Offset: 0x000E7681
	public void Add(string name, int val)
	{
		if (this.storage != null)
		{
			this.storage.Add(name, val);
		}
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000E9498 File Offset: 0x000E7698
	public static ServerStatistics.Storage Get(ulong id)
	{
		ServerStatistics.Storage storage;
		if (ServerStatistics.players.TryGetValue(id, out storage))
		{
			return storage;
		}
		storage = new ServerStatistics.Storage();
		ServerStatistics.players.Add(id, storage);
		return storage;
	}

	// Token: 0x02000CE9 RID: 3305
	public class Storage
	{
		// Token: 0x04004599 RID: 17817
		private Dictionary<string, int> dict = new Dictionary<string, int>();

		// Token: 0x06004FD0 RID: 20432 RVA: 0x001A7190 File Offset: 0x001A5390
		public int Get(string name)
		{
			int result;
			this.dict.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x001A71B0 File Offset: 0x001A53B0
		public void Add(string name, int val)
		{
			if (this.dict.ContainsKey(name))
			{
				Dictionary<string, int> dictionary = this.dict;
				dictionary[name] += val;
				return;
			}
			this.dict.Add(name, val);
		}
	}
}
