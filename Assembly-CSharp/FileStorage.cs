using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using UnityEngine.Assertions;

// Token: 0x0200074A RID: 1866
public class FileStorage : IDisposable
{
	// Token: 0x04002A52 RID: 10834
	private Database db;

	// Token: 0x04002A53 RID: 10835
	private CRC32 crc = new CRC32();

	// Token: 0x04002A54 RID: 10836
	private MruDictionary<uint, FileStorage.CacheData> _cache = new MruDictionary<uint, FileStorage.CacheData>(1000, null);

	// Token: 0x04002A55 RID: 10837
	public static FileStorage server = new FileStorage("sv.files." + 238, true);

	// Token: 0x06003444 RID: 13380 RVA: 0x00143B74 File Offset: 0x00141D74
	protected FileStorage(string name, bool server)
	{
		if (server)
		{
			string path = Server.rootFolder + "/" + name + ".db";
			this.db = new Database();
			this.db.Open(path, true);
			if (!this.db.TableExists("data"))
			{
				this.db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )");
				this.db.Execute("CREATE INDEX IF NOT EXISTS entindex ON data ( entid )");
			}
		}
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x00143C08 File Offset: 0x00141E08
	~FileStorage()
	{
		this.Dispose();
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x00143C34 File Offset: 0x00141E34
	public void Dispose()
	{
		if (this.db != null)
		{
			this.db.Close();
			this.db = null;
		}
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x00143C50 File Offset: 0x00141E50
	private uint GetCRC(byte[] data, FileStorage.Type type)
	{
		uint crc32Result;
		using (TimeWarning.New("FileStorage.GetCRC", 0))
		{
			this.crc.Reset();
			this.crc.SlurpBlock(data, 0, data.Length);
			this.crc.UpdateCRC((byte)type);
			crc32Result = (uint)this.crc.Crc32Result;
		}
		return crc32Result;
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x00143CBC File Offset: 0x00141EBC
	public uint Store(byte[] data, FileStorage.Type type, NetworkableId entityID, uint numID = 0U)
	{
		uint result;
		using (TimeWarning.New("FileStorage.Store", 0))
		{
			uint num = this.GetCRC(data, type);
			if (this.db != null)
			{
				this.db.Execute<int, byte[], long, int, int>("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", (int)num, data, (long)entityID.Value, (int)type, (int)numID);
			}
			this._cache.Remove(num);
			this._cache.Add(num, new FileStorage.CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			result = num;
		}
		return result;
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x00143D54 File Offset: 0x00141F54
	public byte[] Get(uint crc, FileStorage.Type type, NetworkableId entityID, uint numID = 0U)
	{
		byte[] result;
		using (TimeWarning.New("FileStorage.Get", 0))
		{
			FileStorage.CacheData cacheData;
			if (this._cache.TryGetValue(crc, out cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				result = cacheData.data;
			}
			else if (this.db == null)
			{
				result = null;
			}
			else
			{
				byte[] array = this.db.QueryBlob<int, int, long, int>("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ? LIMIT 1", (int)crc, (int)type, (long)entityID.Value, (int)numID);
				if (array == null)
				{
					result = null;
				}
				else
				{
					this._cache.Remove(crc);
					this._cache.Add(crc, new FileStorage.CacheData
					{
						data = array,
						entityID = entityID,
						numID = 0U
					});
					result = array;
				}
			}
		}
		return result;
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x00143E18 File Offset: 0x00142018
	public void Remove(uint crc, FileStorage.Type type, NetworkableId entityID)
	{
		using (TimeWarning.New("FileStorage.Remove", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, long>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", (int)crc, (int)type, (long)entityID.Value);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x00143E7C File Offset: 0x0014207C
	public void RemoveExact(uint crc, FileStorage.Type type, NetworkableId entityID, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveExact", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<int, int, long, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ?", (int)crc, (int)type, (long)entityID.Value, (int)numid);
			}
			this._cache.Remove(crc);
		}
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x00143EE0 File Offset: 0x001420E0
	public void RemoveEntityNum(NetworkableId entityid, uint numid)
	{
		using (TimeWarning.New("FileStorage.RemoveEntityNum", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long, int>("DELETE FROM data WHERE entid = ? AND part = ?", (long)entityid.Value, (int)numid);
			}
			IEnumerable<KeyValuePair<uint, FileStorage.CacheData>> cache = this._cache;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> <>9__0;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((KeyValuePair<uint, FileStorage.CacheData> x) => x.Value.entityID == entityid && x.Value.numID == numid));
			}
			foreach (uint key in (from x in cache.Where(predicate)
			select x.Key).ToArray<uint>())
			{
				this._cache.Remove(key);
			}
		}
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x00143FC8 File Offset: 0x001421C8
	internal void RemoveAllByEntity(NetworkableId entityid)
	{
		using (TimeWarning.New("FileStorage.RemoveAllByEntity", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long>("DELETE FROM data WHERE entid = ?", (long)entityid.Value);
			}
		}
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x0014401C File Offset: 0x0014221C
	public void ReassignEntityId(NetworkableId oldId, NetworkableId newId)
	{
		using (TimeWarning.New("FileStorage.ReassignEntityId", 0))
		{
			if (this.db != null)
			{
				this.db.Execute<long, long>("UPDATE data SET entid = ? WHERE entid = ?", (long)newId.Value, (long)oldId.Value);
			}
		}
	}

	// Token: 0x02000E56 RID: 3670
	private class CacheData
	{
		// Token: 0x04004B20 RID: 19232
		public byte[] data;

		// Token: 0x04004B21 RID: 19233
		public NetworkableId entityID;

		// Token: 0x04004B22 RID: 19234
		public uint numID;
	}

	// Token: 0x02000E57 RID: 3671
	public enum Type
	{
		// Token: 0x04004B24 RID: 19236
		png,
		// Token: 0x04004B25 RID: 19237
		jpg,
		// Token: 0x04004B26 RID: 19238
		ogg
	}
}
