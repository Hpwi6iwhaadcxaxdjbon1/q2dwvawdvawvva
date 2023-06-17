using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B00 RID: 2816
	public class EventRecord : Pool.IPooled
	{
		// Token: 0x04003CDC RID: 15580
		public DateTime Timestamp;

		// Token: 0x04003CDD RID: 15581
		[NonSerialized]
		public bool IsServer;

		// Token: 0x04003CDF RID: 15583
		public List<EventRecordField> Data = new List<EventRecordField>();

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x060044C1 RID: 17601 RVA: 0x00193627 File Offset: 0x00191827
		// (set) Token: 0x060044C2 RID: 17602 RVA: 0x0019362F File Offset: 0x0019182F
		public string EventType { get; private set; }

		// Token: 0x060044C4 RID: 17604 RVA: 0x0019364B File Offset: 0x0019184B
		public void EnterPool()
		{
			this.Timestamp = default(DateTime);
			this.EventType = null;
			this.IsServer = false;
			this.Data.Clear();
		}

		// Token: 0x060044C5 RID: 17605 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x060044C6 RID: 17606 RVA: 0x00193674 File Offset: 0x00191874
		public static EventRecord New(string type, bool isServer = true)
		{
			EventRecord eventRecord = Pool.Get<EventRecord>();
			eventRecord.EventType = type;
			eventRecord.AddField("type", type);
			eventRecord.AddField("guid", Guid.NewGuid());
			eventRecord.IsServer = isServer;
			if (isServer)
			{
				eventRecord.AddField("wipe_id", SaveRestore.WipeId);
			}
			eventRecord.Timestamp = DateTime.UtcNow;
			return eventRecord;
		}

		// Token: 0x060044C7 RID: 17607 RVA: 0x001936D4 File Offset: 0x001918D4
		public EventRecord AddObject(string key, object data)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = JsonConvert.SerializeObject(data),
				IsObject = true
			});
			return this;
		}

		// Token: 0x060044C8 RID: 17608 RVA: 0x0019370B File Offset: 0x0019190B
		public EventRecord SetTimestamp(DateTime timestamp)
		{
			this.Timestamp = timestamp;
			return this;
		}

		// Token: 0x060044C9 RID: 17609 RVA: 0x00193718 File Offset: 0x00191918
		public EventRecord AddField(string key, bool value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = (value ? "true" : "false")
			});
			return this;
		}

		// Token: 0x060044CA RID: 17610 RVA: 0x00193750 File Offset: 0x00191950
		public EventRecord AddField(string key, string value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				String = value
			});
			return this;
		}

		// Token: 0x060044CB RID: 17611 RVA: 0x0019377C File Offset: 0x0019197C
		public EventRecord AddField(string key, int value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)value)
			});
			return this;
		}

		// Token: 0x060044CC RID: 17612 RVA: 0x001937AC File Offset: 0x001919AC
		public EventRecord AddField(string key, uint value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)((ulong)value))
			});
			return this;
		}

		// Token: 0x060044CD RID: 17613 RVA: 0x001937DC File Offset: 0x001919DC
		public EventRecord AddField(string key, ulong value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?((long)value)
			});
			return this;
		}

		// Token: 0x060044CE RID: 17614 RVA: 0x0019380C File Offset: 0x00191A0C
		public EventRecord AddField(string key, long value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Number = new long?(value)
			});
			return this;
		}

		// Token: 0x060044CF RID: 17615 RVA: 0x0019383C File Offset: 0x00191A3C
		public EventRecord AddField(string key, float value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?((double)value)
			});
			return this;
		}

		// Token: 0x060044D0 RID: 17616 RVA: 0x0019386C File Offset: 0x00191A6C
		public EventRecord AddField(string key, double value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?(value)
			});
			return this;
		}

		// Token: 0x060044D1 RID: 17617 RVA: 0x0019389C File Offset: 0x00191A9C
		public EventRecord AddField(string key, TimeSpan value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Float = new double?(value.TotalSeconds)
			});
			return this;
		}

		// Token: 0x060044D2 RID: 17618 RVA: 0x001938D4 File Offset: 0x00191AD4
		public EventRecord AddField(string key, Guid value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Guid = new Guid?(value)
			});
			return this;
		}

		// Token: 0x060044D3 RID: 17619 RVA: 0x00193904 File Offset: 0x00191B04
		public EventRecord AddField(string key, Vector3 value)
		{
			this.Data.Add(new EventRecordField(key)
			{
				Vector = new Vector3?(value)
			});
			return this;
		}

		// Token: 0x060044D4 RID: 17620 RVA: 0x00193934 File Offset: 0x00191B34
		public EventRecord AddField(string key, BaseEntity entity)
		{
			if (((entity != null) ? entity.net : null) == null)
			{
				return this;
			}
			BasePlayer basePlayer;
			EventRecordField item;
			if ((basePlayer = (entity as BasePlayer)) != null && !basePlayer.IsNpc && !basePlayer.IsBot)
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(basePlayer.userID);
				List<EventRecordField> data = this.Data;
				item = new EventRecordField(key, "_userid")
				{
					String = userWipeId
				};
				data.Add(item);
				if (basePlayer.isMounted)
				{
					this.AddField(key + "_mounted", basePlayer.GetMounted());
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					List<EventRecordField> data2 = this.Data;
					item = new EventRecordField(key, "_admin")
					{
						String = "true"
					};
					data2.Add(item);
				}
			}
			BaseProjectile baseProjectile;
			if ((baseProjectile = (entity as BaseProjectile)) != null)
			{
				Item item2 = baseProjectile.GetItem();
				if (item2 != null)
				{
					ItemContainer contents = item2.contents;
					int? num;
					if (contents == null)
					{
						num = null;
					}
					else
					{
						List<Item> itemList = contents.itemList;
						num = ((itemList != null) ? new int?(itemList.Count) : null);
					}
					if ((num ?? 0) > 0)
					{
						List<string> list = Pool.GetList<string>();
						foreach (Item item3 in item2.contents.itemList)
						{
							list.Add(item3.info.shortname);
						}
						this.AddObject(key + "_inventory", list);
						Pool.FreeList<string>(ref list);
					}
				}
			}
			BuildingBlock buildingBlock;
			if ((buildingBlock = (entity as BuildingBlock)) != null)
			{
				List<EventRecordField> data3 = this.Data;
				item = new EventRecordField(key, "_grade")
				{
					Number = new long?((long)buildingBlock.grade)
				};
				data3.Add(item);
			}
			List<EventRecordField> data4 = this.Data;
			item = new EventRecordField(key, "_prefab")
			{
				String = entity.ShortPrefabName
			};
			data4.Add(item);
			List<EventRecordField> data5 = this.Data;
			item = new EventRecordField(key, "_pos")
			{
				Vector = new Vector3?(entity.transform.position)
			};
			data5.Add(item);
			List<EventRecordField> data6 = this.Data;
			item = new EventRecordField(key, "_rot")
			{
				Vector = new Vector3?(entity.transform.rotation.eulerAngles)
			};
			data6.Add(item);
			List<EventRecordField> data7 = this.Data;
			item = new EventRecordField(key, "_id")
			{
				Number = new long?((long)entity.net.ID.Value)
			};
			data7.Add(item);
			return this;
		}

		// Token: 0x060044D5 RID: 17621 RVA: 0x00193BE8 File Offset: 0x00191DE8
		public EventRecord AddField(string key, Item item)
		{
			List<EventRecordField> data = this.Data;
			EventRecordField item2 = new EventRecordField(key, "_name")
			{
				String = item.info.shortname
			};
			data.Add(item2);
			List<EventRecordField> data2 = this.Data;
			item2 = new EventRecordField(key, "_amount")
			{
				Number = new long?((long)item.amount)
			};
			data2.Add(item2);
			List<EventRecordField> data3 = this.Data;
			item2 = new EventRecordField(key, "_skin")
			{
				Number = new long?((long)item.skin)
			};
			data3.Add(item2);
			List<EventRecordField> data4 = this.Data;
			item2 = new EventRecordField(key, "_condition")
			{
				Float = new double?((double)item.conditionNormalized)
			};
			data4.Add(item2);
			return this;
		}

		// Token: 0x060044D6 RID: 17622 RVA: 0x00193CA4 File Offset: 0x00191EA4
		public void Submit()
		{
			if (this.IsServer)
			{
				if (Analytics.StatsBlacklist != null && Analytics.StatsBlacklist.Contains(this.EventType))
				{
					EventRecord eventRecord = this;
					Pool.Free<EventRecord>(ref eventRecord);
					return;
				}
				Analytics.AzureWebInterface.server.EnqueueEvent(this);
			}
		}
	}
}
