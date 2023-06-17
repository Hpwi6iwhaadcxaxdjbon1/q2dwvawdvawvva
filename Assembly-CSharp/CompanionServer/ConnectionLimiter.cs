using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ConVar;

namespace CompanionServer
{
	// Token: 0x020009E2 RID: 2530
	public class ConnectionLimiter
	{
		// Token: 0x0400369C RID: 13980
		private readonly object _sync;

		// Token: 0x0400369D RID: 13981
		private readonly Dictionary<IPAddress, int> _addressCounts;

		// Token: 0x0400369E RID: 13982
		private int _overallCount;

		// Token: 0x06003C7F RID: 15487 RVA: 0x00163F30 File Offset: 0x00162130
		public ConnectionLimiter()
		{
			this._sync = new object();
			this._addressCounts = new Dictionary<IPAddress, int>();
			this._overallCount = 0;
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x00163F58 File Offset: 0x00162158
		public bool TryAdd(IPAddress address)
		{
			if (address == null)
			{
				return false;
			}
			object sync = this._sync;
			bool result;
			lock (sync)
			{
				if (this._overallCount >= App.maxconnections)
				{
					result = false;
				}
				else
				{
					int num;
					if (this._addressCounts.TryGetValue(address, out num))
					{
						if (num >= App.maxconnectionsperip)
						{
							return false;
						}
						this._addressCounts[address] = num + 1;
					}
					else
					{
						this._addressCounts.Add(address, 1);
					}
					this._overallCount++;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003C81 RID: 15489 RVA: 0x00163FF4 File Offset: 0x001621F4
		public void Remove(IPAddress address)
		{
			if (address == null)
			{
				return;
			}
			object sync = this._sync;
			lock (sync)
			{
				int num;
				if (this._addressCounts.TryGetValue(address, out num))
				{
					if (num <= 1)
					{
						this._addressCounts.Remove(address);
					}
					else
					{
						this._addressCounts[address] = num - 1;
					}
					this._overallCount--;
				}
			}
		}

		// Token: 0x06003C82 RID: 15490 RVA: 0x00164074 File Offset: 0x00162274
		public void Clear()
		{
			object sync = this._sync;
			lock (sync)
			{
				this._addressCounts.Clear();
				this._overallCount = 0;
			}
		}

		// Token: 0x06003C83 RID: 15491 RVA: 0x001640C0 File Offset: 0x001622C0
		public override string ToString()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"IP",
				"connections"
			});
			object sync = this._sync;
			string result;
			lock (sync)
			{
				foreach (KeyValuePair<IPAddress, int> keyValuePair in from t in this._addressCounts
				orderby t.Value descending
				select t)
				{
					textTable.AddRow(new string[]
					{
						keyValuePair.Key.ToString(),
						keyValuePair.Value.ToString()
					});
				}
				result = string.Format("{0}\n{1} total", textTable, this._overallCount);
			}
			return result;
		}
	}
}
