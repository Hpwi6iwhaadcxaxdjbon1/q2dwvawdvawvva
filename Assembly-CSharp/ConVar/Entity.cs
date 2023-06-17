using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB7 RID: 2743
	[ConsoleSystem.Factory("entity")]
	public class Entity : ConsoleSystem
	{
		// Token: 0x060041AC RID: 16812 RVA: 0x00185A4C File Offset: 0x00183C4C
		private static TextTable GetEntityTable(Func<Entity.EntityInfo, bool> filter)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("entity");
			textTable.AddColumn("group");
			textTable.AddColumn("parent");
			textTable.AddColumn("name");
			textTable.AddColumn("position");
			textTable.AddColumn("local");
			textTable.AddColumn("rotation");
			textTable.AddColumn("local");
			textTable.AddColumn("status");
			textTable.AddColumn("invokes");
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
			{
				if (!(baseNetworkable == null))
				{
					Entity.EntityInfo entityInfo = new Entity.EntityInfo(baseNetworkable);
					if (filter(entityInfo))
					{
						textTable.AddRow(new string[]
						{
							"sv",
							entityInfo.entityID.Value.ToString(),
							entityInfo.groupID.ToString(),
							entityInfo.parentID.Value.ToString(),
							entityInfo.entity.ShortPrefabName,
							entityInfo.entity.transform.position.ToString(),
							entityInfo.entity.transform.localPosition.ToString(),
							entityInfo.entity.transform.rotation.eulerAngles.ToString(),
							entityInfo.entity.transform.localRotation.eulerAngles.ToString(),
							entityInfo.status,
							entityInfo.entity.InvokeString()
						});
					}
				}
			}
			return textTable;
		}

		// Token: 0x060041AD RID: 16813 RVA: 0x00185C50 File Offset: 0x00183E50
		[ServerVar]
		[ClientVar]
		public static void find_entity(ConsoleSystem.Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.entity.PrefabName.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041AE RID: 16814 RVA: 0x00185C94 File Offset: 0x00183E94
		[ServerVar]
		[ClientVar]
		public static void find_id(ConsoleSystem.Arg args)
		{
			NetworkableId filter = args.GetEntityID(0, default(NetworkableId));
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041AF RID: 16815 RVA: 0x00185CDC File Offset: 0x00183EDC
		[ServerVar]
		[ClientVar]
		public static void find_group(ConsoleSystem.Arg args)
		{
			uint filter = args.GetUInt(0, 0U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.groupID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041B0 RID: 16816 RVA: 0x00185D1C File Offset: 0x00183F1C
		[ServerVar]
		[ClientVar]
		public static void find_parent(ConsoleSystem.Arg args)
		{
			NetworkableId filter = args.GetEntityID(0, default(NetworkableId));
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.parentID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041B1 RID: 16817 RVA: 0x00185D64 File Offset: 0x00183F64
		[ServerVar]
		[ClientVar]
		public static void find_status(ConsoleSystem.Arg args)
		{
			string filter = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => string.IsNullOrEmpty(filter) || info.status.Contains(filter));
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041B2 RID: 16818 RVA: 0x00185DA8 File Offset: 0x00183FA8
		[ServerVar]
		[ClientVar]
		public static void find_radius(ConsoleSystem.Arg args)
		{
			BasePlayer player = args.Player();
			if (player == null)
			{
				return;
			}
			uint filter = args.GetUInt(0, 10U);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => Vector3.Distance(info.entity.transform.position, player.transform.position) <= filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041B3 RID: 16819 RVA: 0x00185E04 File Offset: 0x00184004
		[ServerVar]
		[ClientVar]
		public static void find_self(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.net == null)
			{
				return;
			}
			NetworkableId filter = basePlayer.net.ID;
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == filter);
			args.ReplyWith(entityTable.ToString());
		}

		// Token: 0x060041B4 RID: 16820 RVA: 0x00185E60 File Offset: 0x00184060
		[ServerVar]
		public static void debug_toggle(ConsoleSystem.Arg args)
		{
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging(), false, true);
			if (baseEntity.IsDebugging())
			{
				baseEntity.OnDebugStart();
			}
			args.ReplyWith(string.Concat(new object[]
			{
				"Debugging for ",
				baseEntity.net.ID,
				" ",
				baseEntity.IsDebugging() ? "enabled" : "disabled"
			}));
		}

		// Token: 0x060041B5 RID: 16821 RVA: 0x00185F10 File Offset: 0x00184110
		[ServerVar]
		public static void nudge(ConsoleSystem.Arg args)
		{
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.BroadcastMessage("DebugNudge", SendMessageOptions.DontRequireReceiver);
		}

		// Token: 0x060041B6 RID: 16822 RVA: 0x00185F60 File Offset: 0x00184160
		private static Entity.EntitySpawnRequest GetSpawnEntityFromName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Entity.EntitySpawnRequest
				{
					Error = "No entity name provided"
				};
			}
			string[] array = (from x in GameManifest.Current.entities
			where Path.GetFileNameWithoutExtension(x).Contains(name, CompareOptions.IgnoreCase)
			select x.ToLower()).ToArray<string>();
			if (array.Length == 0)
			{
				return new Entity.EntitySpawnRequest
				{
					Error = "Entity type not found"
				};
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					return new Entity.EntitySpawnRequest
					{
						Error = "Unknown entity - could be:\n\n" + string.Join("\n", array.Select(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>())
					};
				}
				array[0] = text;
			}
			return new Entity.EntitySpawnRequest
			{
				PrefabName = array[0]
			};
		}

		// Token: 0x060041B7 RID: 16823 RVA: 0x00186070 File Offset: 0x00184270
		[ServerVar(Name = "spawn")]
		public static string svspawn(string name, Vector3 pos, Vector3 dir)
		{
			BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
			Entity.EntitySpawnRequest spawnEntityFromName = Entity.GetSpawnEntityFromName(name);
			if (!spawnEntityFromName.Valid)
			{
				return spawnEntityFromName.Error;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, pos, Quaternion.LookRotation(dir, Vector3.up), true);
			if (baseEntity == null)
			{
				Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawn \"{2}\")", arg, spawnEntityFromName.PrefabName, name));
				return "Couldn't spawn " + name;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null)
			{
				basePlayer.OverrideViewAngles(Quaternion.LookRotation(dir, Vector3.up).eulerAngles);
			}
			baseEntity.Spawn();
			Debug.Log(string.Format("{0} spawned \"{1}\" at {2}", arg, baseEntity, pos));
			return string.Concat(new object[]
			{
				"spawned ",
				baseEntity,
				" at ",
				pos
			});
		}

		// Token: 0x060041B8 RID: 16824 RVA: 0x00186158 File Offset: 0x00184358
		[ServerVar(Name = "spawnitem")]
		public static string svspawnitem(string name, Vector3 pos)
		{
			BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = (from x in ItemManager.itemList
			select x.shortname into x
			where x.Contains(name, CompareOptions.IgnoreCase)
			select x).ToArray<string>();
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					Debug.Log(string.Format("{0} failed to spawn \"{1}\"", arg, name));
					return "Unknown entity - could be:\n\n" + string.Join("\n", array);
				}
				array[0] = text;
			}
			Item item = ItemManager.CreateByName(array[0], 1, 0UL);
			if (item == null)
			{
				Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawnitem \"{2}\")", arg, array[0], name));
				return "Couldn't spawn " + name;
			}
			BaseEntity arg2 = item.CreateWorldObject(pos, default(Quaternion), null, 0U);
			Debug.Log(string.Format("{0} spawned \"{1}\" at {2} (via spawnitem)", arg, arg2, pos));
			return string.Concat(new object[]
			{
				"spawned ",
				item,
				" at ",
				pos
			});
		}

		// Token: 0x060041B9 RID: 16825 RVA: 0x001862BC File Offset: 0x001844BC
		[ServerVar(Name = "spawngrid")]
		public static string svspawngrid(string name, int width = 5, int height = 5, int spacing = 5)
		{
			BasePlayer basePlayer = ConsoleSystem.CurrentArgs.Player();
			Entity.EntitySpawnRequest spawnEntityFromName = Entity.GetSpawnEntityFromName(name);
			if (!spawnEntityFromName.Valid)
			{
				return spawnEntityFromName.Error;
			}
			Quaternion rotation = basePlayer.transform.rotation;
			rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
			Matrix4x4 matrix4x = Matrix4x4.TRS(basePlayer.transform.position, basePlayer.transform.rotation, Vector3.one);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					Vector3 pos = matrix4x.MultiplyPoint(new Vector3((float)(i * spacing), 0f, (float)(j * spacing)));
					BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, pos, rotation, true);
					if (baseEntity == null)
					{
						Debug.Log(string.Format("{0} failed to spawn \"{1}\" (tried to spawn \"{2}\")", basePlayer, spawnEntityFromName.PrefabName, name));
						return "Couldn't spawn " + name;
					}
					baseEntity.Spawn();
				}
			}
			Debug.Log(string.Format("{0} spawned ({1}) ", basePlayer, width * height) + spawnEntityFromName.PrefabName);
			return string.Format("spawned ({0}) ", width * height) + spawnEntityFromName.PrefabName;
		}

		// Token: 0x060041BA RID: 16826 RVA: 0x00186404 File Offset: 0x00184604
		[ServerVar]
		public static void spawnlootfrom(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			string @string = args.GetString(0, string.Empty);
			int @int = args.GetInt(1, 1);
			Vector3 vector = args.GetVector3(1, basePlayer ? basePlayer.CenterPoint() : Vector3.zero);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(@string, vector, default(Quaternion), true);
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.Spawn();
			basePlayer.ChatMessage(string.Concat(new object[]
			{
				"Contents of ",
				@string,
				" spawned ",
				@int,
				" times"
			}));
			LootContainer component = baseEntity.GetComponent<LootContainer>();
			if (component != null)
			{
				for (int i = 0; i < @int * component.maxDefinitionsToSpawn; i++)
				{
					component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
				}
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}

		// Token: 0x060041BB RID: 16827 RVA: 0x00186500 File Offset: 0x00184700
		public static int DeleteBy(ulong id)
		{
			List<ulong> list = Pool.GetList<ulong>();
			list.Add(id);
			int result = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return result;
		}

		// Token: 0x060041BC RID: 16828 RVA: 0x00186528 File Offset: 0x00184728
		[ServerVar(Help = "Destroy all entities created by provided users (separate users by space)")]
		public static int DeleteBy(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return 0;
			}
			List<ulong> list = Pool.GetList<ulong>();
			string[] args = arg.Args;
			for (int i = 0; i < args.Length; i++)
			{
				ulong item;
				if (ulong.TryParse(args[i], out item))
				{
					list.Add(item);
				}
			}
			int result = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			return result;
		}

		// Token: 0x060041BD RID: 16829 RVA: 0x0018657C File Offset: 0x0018477C
		private static int DeleteBy(List<ulong> ids)
		{
			int num = 0;
			foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities)
			{
				BaseEntity baseEntity = (BaseEntity)baseNetworkable;
				if (!(baseEntity == null))
				{
					bool flag = false;
					foreach (ulong num2 in ids)
					{
						if (baseEntity.OwnerID == num2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						baseEntity.Invoke(new Action(baseEntity.KillMessage), (float)num * 0.2f);
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060041BE RID: 16830 RVA: 0x0018663C File Offset: 0x0018483C
		[ServerVar(Help = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)")]
		public static void DeleteByTextBlock(ConsoleSystem.Arg arg)
		{
			if (arg.Args.Length != 1)
			{
				arg.ReplyWith("Invalid arguments, provide a text block surrounded by \" and listing player id's at the start of each line");
				return;
			}
			MatchCollection matchCollection = Regex.Matches(arg.GetString(0, ""), "^\\b\\d{17}", RegexOptions.Multiline);
			List<ulong> list = Pool.GetList<ulong>();
			using (IEnumerator enumerator = matchCollection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ulong item;
					if (ulong.TryParse(((Match)enumerator.Current).Value, out item))
					{
						list.Add(item);
					}
				}
			}
			int num = Entity.DeleteBy(list);
			Pool.FreeList<ulong>(ref list);
			arg.ReplyWith(string.Format("Destroyed {0} entities", num));
		}

		// Token: 0x02000F52 RID: 3922
		private struct EntityInfo
		{
			// Token: 0x04004F66 RID: 20326
			public BaseNetworkable entity;

			// Token: 0x04004F67 RID: 20327
			public NetworkableId entityID;

			// Token: 0x04004F68 RID: 20328
			public uint groupID;

			// Token: 0x04004F69 RID: 20329
			public NetworkableId parentID;

			// Token: 0x04004F6A RID: 20330
			public string status;

			// Token: 0x06005475 RID: 21621 RVA: 0x001B5500 File Offset: 0x001B3700
			public EntityInfo(BaseNetworkable src)
			{
				this.entity = src;
				BaseEntity baseEntity = this.entity as BaseEntity;
				BaseEntity x = (baseEntity != null) ? baseEntity.GetParentEntity() : null;
				this.entityID = ((this.entity != null && this.entity.net != null) ? this.entity.net.ID : default(NetworkableId));
				this.groupID = ((this.entity != null && this.entity.net != null && this.entity.net.group != null) ? this.entity.net.group.ID : 0U);
				this.parentID = ((baseEntity != null) ? baseEntity.parentEntity.uid : default(NetworkableId));
				if (!(baseEntity != null) || !baseEntity.parentEntity.uid.IsValid)
				{
					this.status = string.Empty;
					return;
				}
				if (x == null)
				{
					this.status = "orphan";
					return;
				}
				this.status = "child";
			}
		}

		// Token: 0x02000F53 RID: 3923
		private struct EntitySpawnRequest
		{
			// Token: 0x04004F6B RID: 20331
			public string PrefabName;

			// Token: 0x04004F6C RID: 20332
			public string Error;

			// Token: 0x17000735 RID: 1845
			// (get) Token: 0x06005476 RID: 21622 RVA: 0x001B5629 File Offset: 0x001B3829
			public bool Valid
			{
				get
				{
					return string.IsNullOrEmpty(this.Error);
				}
			}
		}
	}
}
