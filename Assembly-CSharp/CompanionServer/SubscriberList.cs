using System;
using System.Collections.Generic;
using System.Diagnostics;
using Facepunch;
using Network;

namespace CompanionServer
{
	// Token: 0x020009ED RID: 2541
	public class SubscriberList<TKey, TTarget, TMessage> where TKey : IEquatable<TKey> where TTarget : class
	{
		// Token: 0x040036CE RID: 14030
		private readonly object _syncRoot;

		// Token: 0x040036CF RID: 14031
		private readonly Dictionary<TKey, Dictionary<TTarget, double>> _subscriptions;

		// Token: 0x040036D0 RID: 14032
		private readonly IBroadcastSender<TTarget, TMessage> _sender;

		// Token: 0x040036D1 RID: 14033
		private readonly double? _timeoutSeconds;

		// Token: 0x040036D2 RID: 14034
		private readonly Stopwatch _lastCleanup;

		// Token: 0x06003CCB RID: 15563 RVA: 0x00165260 File Offset: 0x00163460
		public SubscriberList(IBroadcastSender<TTarget, TMessage> sender, double? timeoutSeconds = null)
		{
			this._syncRoot = new object();
			this._subscriptions = new Dictionary<TKey, Dictionary<TTarget, double>>();
			this._sender = sender;
			this._timeoutSeconds = timeoutSeconds;
			this._lastCleanup = Stopwatch.StartNew();
		}

		// Token: 0x06003CCC RID: 15564 RVA: 0x00165298 File Offset: 0x00163498
		public void Add(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary = new Dictionary<TTarget, double>();
					this._subscriptions.Add(key, dictionary);
				}
				dictionary[value] = TimeEx.realtimeSinceStartup;
			}
			this.CleanupExpired();
		}

		// Token: 0x06003CCD RID: 15565 RVA: 0x00165308 File Offset: 0x00163508
		public void Remove(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary.Remove(value);
					if (dictionary.Count == 0)
					{
						this._subscriptions.Remove(key);
					}
				}
			}
			this.CleanupExpired();
		}

		// Token: 0x06003CCE RID: 15566 RVA: 0x00165378 File Offset: 0x00163578
		public void Clear(TKey key)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (this._subscriptions.TryGetValue(key, out dictionary))
				{
					dictionary.Clear();
				}
			}
		}

		// Token: 0x06003CCF RID: 15567 RVA: 0x001653C8 File Offset: 0x001635C8
		public void Send(TKey key, TMessage message)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			List<TTarget> list;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					return;
				}
				list = Pool.GetList<TTarget>();
				foreach (KeyValuePair<TTarget, double> keyValuePair in dictionary)
				{
					if (this._timeoutSeconds == null || realtimeSinceStartup - keyValuePair.Value < this._timeoutSeconds.Value)
					{
						list.Add(keyValuePair.Key);
					}
				}
			}
			this._sender.BroadcastTo(list, message);
			Pool.FreeList<TTarget>(ref list);
		}

		// Token: 0x06003CD0 RID: 15568 RVA: 0x001654A0 File Offset: 0x001636A0
		public bool HasAnySubscribers(TKey key)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				if (!this._subscriptions.TryGetValue(key, out dictionary))
				{
					return false;
				}
				foreach (KeyValuePair<TTarget, double> keyValuePair in dictionary)
				{
					if (this._timeoutSeconds == null || realtimeSinceStartup - keyValuePair.Value < this._timeoutSeconds.Value)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x00165558 File Offset: 0x00163758
		public bool HasSubscriber(TKey key, TTarget target)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				Dictionary<TTarget, double> dictionary;
				double num;
				if (!this._subscriptions.TryGetValue(key, out dictionary) || !dictionary.TryGetValue(target, out num))
				{
					return false;
				}
				if (this._timeoutSeconds == null || realtimeSinceStartup - num < this._timeoutSeconds.Value)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003CD2 RID: 15570 RVA: 0x001655E0 File Offset: 0x001637E0
		private void CleanupExpired()
		{
			if (this._timeoutSeconds == null || this._lastCleanup.Elapsed.TotalMinutes < 2.0)
			{
				return;
			}
			this._lastCleanup.Restart();
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			List<ValueTuple<TKey, TTarget>> list = Pool.GetList<ValueTuple<TKey, TTarget>>();
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				foreach (KeyValuePair<TKey, Dictionary<TTarget, double>> keyValuePair in this._subscriptions)
				{
					foreach (KeyValuePair<TTarget, double> keyValuePair2 in keyValuePair.Value)
					{
						if (realtimeSinceStartup - keyValuePair2.Value >= this._timeoutSeconds.Value)
						{
							list.Add(new ValueTuple<TKey, TTarget>(keyValuePair.Key, keyValuePair2.Key));
						}
					}
				}
				foreach (ValueTuple<TKey, TTarget> valueTuple in list)
				{
					TKey item = valueTuple.Item1;
					TTarget item2 = valueTuple.Item2;
					this.Remove(item, item2);
				}
			}
			Pool.FreeList<ValueTuple<TKey, TTarget>>(ref list);
		}
	}
}
