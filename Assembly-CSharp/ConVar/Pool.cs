using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AD5 RID: 2773
	[ConsoleSystem.Factory("pool")]
	public class Pool : ConsoleSystem
	{
		// Token: 0x04003B93 RID: 15251
		[ServerVar]
		[ClientVar]
		public static int mode = 2;

		// Token: 0x04003B94 RID: 15252
		[ServerVar]
		[ClientVar]
		public static bool prewarm = true;

		// Token: 0x04003B95 RID: 15253
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003B96 RID: 15254
		[ServerVar]
		[ClientVar]
		public static bool debug = false;

		// Token: 0x060042A5 RID: 17061 RVA: 0x0018B020 File Offset: 0x00189220
		[ServerVar]
		[ClientVar]
		public static void print_memory(ConsoleSystem.Arg arg)
		{
			if (Pool.Directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("capacity");
			textTable.AddColumn("pooled");
			textTable.AddColumn("active");
			textTable.AddColumn("hits");
			textTable.AddColumn("misses");
			textTable.AddColumn("spills");
			foreach (KeyValuePair<Type, Pool.IPoolCollection> keyValuePair in from x in Pool.Directory
			orderby x.Value.ItemsCreated descending
			select x)
			{
				Type key = keyValuePair.Key;
				Pool.IPoolCollection value = keyValuePair.Value;
				textTable.AddRow(new string[]
				{
					key.ToString().Replace("System.Collections.Generic.", ""),
					value.ItemsCapacity.FormatNumberShort(),
					value.ItemsInStack.FormatNumberShort(),
					value.ItemsInUse.FormatNumberShort(),
					value.ItemsTaken.FormatNumberShort(),
					value.ItemsCreated.FormatNumberShort(),
					value.ItemsSpilled.FormatNumberShort()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042A6 RID: 17062 RVA: 0x0018B1AC File Offset: 0x001893AC
		[ServerVar]
		[ClientVar]
		public static void print_arraypool(ConsoleSystem.Arg arg)
		{
			ArrayPool<byte> arrayPool = BaseNetwork.ArrayPool;
			ConcurrentQueue<byte[]>[] buffer = arrayPool.GetBuffer();
			TextTable textTable = new TextTable();
			textTable.AddColumn("index");
			textTable.AddColumn("size");
			textTable.AddColumn("bytes");
			textTable.AddColumn("count");
			textTable.AddColumn("memory");
			for (int i = 0; i < buffer.Length; i++)
			{
				int num = arrayPool.IndexToSize(i);
				int count = buffer[i].Count;
				int input = num * count;
				textTable.AddRow(new string[]
				{
					i.ToString(),
					num.ToString(),
					num.FormatBytes(false),
					count.ToString(),
					input.FormatBytes(false)
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042A7 RID: 17063 RVA: 0x0018B28C File Offset: 0x0018948C
		[ServerVar]
		[ClientVar]
		public static void print_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text2.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[]
					{
						text,
						Path.GetFileNameWithoutExtension(text2),
						text3
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042A8 RID: 17064 RVA: 0x0018B3BC File Offset: 0x001895BC
		[ServerVar]
		[ClientVar]
		public static void print_assets(ConsoleSystem.Arg arg)
		{
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("allocated");
			textTable.AddColumn("available");
			foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = keyValuePair.Value.allocated.ToString();
				string text3 = keyValuePair.Value.available.ToString();
				if (string.IsNullOrEmpty(@string) || text.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[]
					{
						text,
						text2,
						text3
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042A9 RID: 17065 RVA: 0x0018B4D8 File Offset: 0x001896D8
		[ServerVar]
		[ClientVar]
		public static void clear_memory(ConsoleSystem.Arg arg)
		{
			Pool.Clear(arg.GetString(0, string.Empty));
		}

		// Token: 0x060042AA RID: 17066 RVA: 0x0018B4EC File Offset: 0x001896EC
		[ServerVar]
		[ClientVar]
		public static void clear_prefabs(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, string.Empty);
			GameManager.server.pool.Clear(@string);
		}

		// Token: 0x060042AB RID: 17067 RVA: 0x0018B516 File Offset: 0x00189716
		[ServerVar]
		[ClientVar]
		public static void clear_assets(ConsoleSystem.Arg arg)
		{
			AssetPool.Clear(arg.GetString(0, string.Empty));
		}

		// Token: 0x060042AC RID: 17068 RVA: 0x0018B52C File Offset: 0x0018972C
		[ServerVar]
		[ClientVar]
		public static void export_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string arg2 = keyValuePair.Key.ToString();
				string text = StringPool.Get(keyValuePair.Key);
				string arg3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text.Contains(@string, CompareOptions.IgnoreCase))
				{
					stringBuilder.AppendLine(string.Format("{0},{1},{2}", arg2, Path.GetFileNameWithoutExtension(text), arg3));
				}
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}

		// Token: 0x060042AD RID: 17069 RVA: 0x0018B628 File Offset: 0x00189828
		[ServerVar]
		[ClientVar]
		public static void fill_prefabs(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, string.Empty);
			int @int = arg.GetInt(1, 0);
			PrefabPoolWarmup.Run(@string, @int);
		}
	}
}
