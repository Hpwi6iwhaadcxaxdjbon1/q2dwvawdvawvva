using System;
using System.Collections.Generic;
using Facepunch;
using Network;

namespace CompanionServer
{
	// Token: 0x020009DD RID: 2525
	public class BanList<TKey>
	{
		// Token: 0x04003686 RID: 13958
		private readonly Dictionary<TKey, double> _bans;

		// Token: 0x06003C52 RID: 15442 RVA: 0x001631ED File Offset: 0x001613ED
		public BanList()
		{
			this._bans = new Dictionary<TKey, double>();
		}

		// Token: 0x06003C53 RID: 15443 RVA: 0x00163200 File Offset: 0x00161400
		public void Ban(TKey key, double timeInSeconds)
		{
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				double num = TimeEx.realtimeSinceStartup + timeInSeconds;
				double val;
				if (this._bans.TryGetValue(key, out val))
				{
					num = Math.Max(num, val);
				}
				this._bans[key] = num;
			}
		}

		// Token: 0x06003C54 RID: 15444 RVA: 0x00163268 File Offset: 0x00161468
		public bool IsBanned(TKey key)
		{
			Dictionary<TKey, double> bans = this._bans;
			bool result;
			lock (bans)
			{
				double num;
				if (!this._bans.TryGetValue(key, out num))
				{
					result = false;
				}
				else if (TimeEx.realtimeSinceStartup < num)
				{
					result = true;
				}
				else
				{
					this._bans.Remove(key);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06003C55 RID: 15445 RVA: 0x001632D4 File Offset: 0x001614D4
		public void Cleanup()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			List<TKey> list = Pool.GetList<TKey>();
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				foreach (KeyValuePair<TKey, double> keyValuePair in this._bans)
				{
					if (realtimeSinceStartup >= keyValuePair.Value)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (TKey key in list)
				{
					this._bans.Remove(key);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}
	}
}
