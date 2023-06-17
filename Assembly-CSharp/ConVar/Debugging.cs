using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Facepunch;
using Facepunch.Unity;
using Rust;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB1 RID: 2737
	[ConsoleSystem.Factory("debug")]
	public class Debugging : ConsoleSystem
	{
		// Token: 0x04003B13 RID: 15123
		[ServerVar]
		[ClientVar]
		public static bool checktriggers = false;

		// Token: 0x04003B14 RID: 15124
		[ServerVar]
		public static bool checkparentingtriggers = true;

		// Token: 0x04003B15 RID: 15125
		[ServerVar]
		[ClientVar(Saved = false, Help = "Shows some debug info for dismount attempts.")]
		public static bool DebugDismounts = false;

		// Token: 0x04003B16 RID: 15126
		[ServerVar(Help = "Do not damage any items")]
		public static bool disablecondition = false;

		// Token: 0x04003B17 RID: 15127
		[ClientVar]
		[ServerVar]
		public static bool callbacks = false;

		// Token: 0x06004189 RID: 16777 RVA: 0x00184D6B File Offset: 0x00182F6B
		[ServerVar]
		[ClientVar]
		public static void renderinfo(ConsoleSystem.Arg arg)
		{
			RenderInfo.GenerateReport();
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x0600418B RID: 16779 RVA: 0x00184D7F File Offset: 0x00182F7F
		// (set) Token: 0x0600418A RID: 16778 RVA: 0x00184D72 File Offset: 0x00182F72
		[ServerVar]
		[ClientVar]
		public static bool log
		{
			get
			{
				return Debug.unityLogger.logEnabled;
			}
			set
			{
				Debug.unityLogger.logEnabled = value;
			}
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x00184D8C File Offset: 0x00182F8C
		[ServerVar]
		public static void enable_player_movement(ConsoleSystem.Arg arg)
		{
			if (!arg.IsAdmin)
			{
				return;
			}
			bool @bool = arg.GetBool(0, true);
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("Must be called from client with player model");
				return;
			}
			basePlayer.ClientRPCPlayer<bool>(null, basePlayer, "TogglePlayerMovement", @bool);
			arg.ReplyWith((@bool ? "enabled" : "disabled") + " player movement");
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x00184DF4 File Offset: 0x00182FF4
		[ClientVar]
		[ServerVar]
		public static void stall(ConsoleSystem.Arg arg)
		{
			float num = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
			arg.ReplyWith("Stalling for " + num + " seconds...");
			Thread.Sleep(Mathf.RoundToInt(num * 1000f));
		}

		// Token: 0x0600418E RID: 16782 RVA: 0x00184E4C File Offset: 0x0018304C
		[ServerVar(Help = "Repair all items in inventory")]
		public static void repair_inventory(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			foreach (Item item in basePlayer.inventory.AllItems())
			{
				if (item != null)
				{
					item.maxCondition = item.info.condition.max;
					item.condition = item.maxCondition;
					item.MarkDirty();
				}
				if (item.contents != null)
				{
					foreach (Item item2 in item.contents.itemList)
					{
						item2.maxCondition = item2.info.condition.max;
						item2.condition = item2.maxCondition;
						item2.MarkDirty();
					}
				}
			}
		}

		// Token: 0x0600418F RID: 16783 RVA: 0x00184F2C File Offset: 0x0018312C
		[ServerVar(Help = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
		public static void flushgroup(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
			basePlayer.UpdateNetworkGroup();
		}

		// Token: 0x06004190 RID: 16784 RVA: 0x00184F64 File Offset: 0x00183164
		[ServerVar(Help = "Break the current held object")]
		public static void breakheld(ConsoleSystem.Arg arg)
		{
			Item activeItem = arg.Player().GetActiveItem();
			if (activeItem == null)
			{
				return;
			}
			activeItem.LoseCondition(activeItem.condition * 2f);
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x00184F94 File Offset: 0x00183194
		[ServerVar(Help = "reset all puzzles")]
		public static void puzzlereset(ConsoleSystem.Arg arg)
		{
			if (arg.Player() == null)
			{
				return;
			}
			PuzzleReset[] array = UnityEngine.Object.FindObjectsOfType<PuzzleReset>();
			Debug.Log("iterating...");
			foreach (PuzzleReset puzzleReset in array)
			{
				Debug.Log("resetting puzzle at :" + puzzleReset.transform.position);
				puzzleReset.DoReset();
				puzzleReset.ResetTimer();
			}
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x00185000 File Offset: 0x00183200
		[ServerVar(EditorOnly = true, Help = "respawn all puzzles from their prefabs")]
		public static void puzzleprefabrespawn(ConsoleSystem.Arg arg)
		{
			foreach (BaseNetworkable baseNetworkable in (from x in BaseNetworkable.serverEntities
			where x is IOEntity && PrefabAttribute.server.Find<Construction>(x.prefabID) == null
			select x).ToList<BaseNetworkable>())
			{
				baseNetworkable.Kill(BaseNetworkable.DestroyMode.None);
			}
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				GameObject gameObject = GameManager.server.FindPrefab(monumentInfo.gameObject.name);
				if (!(gameObject == null))
				{
					Dictionary<IOEntity, IOEntity> dictionary = new Dictionary<IOEntity, IOEntity>();
					IOEntity[] componentsInChildren = gameObject.GetComponentsInChildren<IOEntity>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						IOEntity ioentity = componentsInChildren[i];
						Quaternion rot = monumentInfo.transform.rotation * ioentity.transform.rotation;
						Vector3 pos = monumentInfo.transform.TransformPoint(ioentity.transform.position);
						BaseEntity newEntity = GameManager.server.CreateEntity(ioentity.PrefabName, pos, rot, true);
						IOEntity ioentity2 = newEntity as IOEntity;
						if (ioentity2 != null)
						{
							dictionary.Add(ioentity, ioentity2);
							DoorManipulator doorManipulator = newEntity as DoorManipulator;
							if (doorManipulator != null)
							{
								List<Door> list = Pool.GetList<Door>();
								Vis.Entities<Door>(newEntity.transform.position, 10f, list, -1, QueryTriggerInteraction.Collide);
								Door door = (from x in list
								orderby x.Distance(newEntity.transform.position)
								select x).FirstOrDefault<Door>();
								if (door != null)
								{
									doorManipulator.targetDoor = door;
								}
								Pool.FreeList<Door>(ref list);
							}
							CardReader cardReader = newEntity as CardReader;
							if (cardReader != null)
							{
								CardReader cardReader2 = ioentity as CardReader;
								if (cardReader2 != null)
								{
									cardReader.accessLevel = cardReader2.accessLevel;
									cardReader.accessDuration = cardReader2.accessDuration;
								}
							}
							TimerSwitch timerSwitch = newEntity as TimerSwitch;
							if (timerSwitch != null)
							{
								TimerSwitch timerSwitch2 = ioentity as TimerSwitch;
								if (timerSwitch2 != null)
								{
									timerSwitch.timerLength = timerSwitch2.timerLength;
								}
							}
						}
					}
					foreach (KeyValuePair<IOEntity, IOEntity> keyValuePair in dictionary)
					{
						IOEntity key = keyValuePair.Key;
						IOEntity value = keyValuePair.Value;
						for (int j = 0; j < key.outputs.Length; j++)
						{
							if (!(key.outputs[j].connectedTo.ioEnt == null))
							{
								value.outputs[j].connectedTo.ioEnt = dictionary[key.outputs[j].connectedTo.ioEnt];
								value.outputs[j].connectedToSlot = key.outputs[j].connectedToSlot;
							}
						}
					}
					foreach (IOEntity ioentity3 in dictionary.Values)
					{
						ioentity3.Spawn();
					}
				}
			}
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x001853C8 File Offset: 0x001835C8
		[ServerVar(Help = "Break all the items in your inventory whose name match the passed string")]
		public static void breakitem(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			foreach (Item item in arg.Player().inventory.containerMain.itemList)
			{
				if (item.info.shortname.Contains(@string, CompareOptions.IgnoreCase) && item.hasCondition)
				{
					item.LoseCondition(item.condition * 2f);
				}
			}
		}

		// Token: 0x06004194 RID: 16788 RVA: 0x00185460 File Offset: 0x00183660
		[ServerVar]
		public static void refillvitals(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), 1000f, null);
			Debugging.AdjustCalories(arg.Player(), 1000f, 1f);
			Debugging.AdjustHydration(arg.Player(), 1000f, 1f);
		}

		// Token: 0x06004195 RID: 16789 RVA: 0x0018549D File Offset: 0x0018369D
		[ServerVar]
		public static void heal(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), (float)arg.GetInt(0, 1), null);
		}

		// Token: 0x06004196 RID: 16790 RVA: 0x001854B4 File Offset: 0x001836B4
		[ServerVar]
		public static void hurt(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHealth(arg.Player(), (float)(-(float)arg.GetInt(0, 1)), arg.GetString(1, string.Empty));
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x001854D7 File Offset: 0x001836D7
		[ServerVar]
		public static void eat(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustCalories(arg.Player(), (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x001854F6 File Offset: 0x001836F6
		[ServerVar]
		public static void drink(ConsoleSystem.Arg arg)
		{
			Debugging.AdjustHydration(arg.Player(), (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x00185518 File Offset: 0x00183718
		private static void AdjustHealth(BasePlayer player, float amount, string bone = null)
		{
			HitInfo hitInfo = new HitInfo(player, player, DamageType.Bullet, -amount);
			if (!string.IsNullOrEmpty(bone))
			{
				hitInfo.HitBone = StringPool.Get(bone);
			}
			player.OnAttacked(hitInfo);
		}

		// Token: 0x0600419A RID: 16794 RVA: 0x0018554C File Offset: 0x0018374C
		private static void AdjustCalories(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, amount, time);
		}

		// Token: 0x0600419B RID: 16795 RVA: 0x0018555C File Offset: 0x0018375C
		private static void AdjustHydration(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, amount, time);
		}

		// Token: 0x0600419C RID: 16796 RVA: 0x0018556C File Offset: 0x0018376C
		[ServerVar]
		public static void ResetSleepingBagTimers(ConsoleSystem.Arg arg)
		{
			SleepingBag.ResetTimersForPlayer(arg.Player());
		}

		// Token: 0x0600419D RID: 16797 RVA: 0x0018557C File Offset: 0x0018377C
		[ServerVar(Help = "Spawn lots of IO entities to lag the server")]
		public static void bench_io(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			int @int = arg.GetInt(0, 50);
			string name = arg.GetString(1, "water_catcher_small");
			List<IOEntity> list = new List<IOEntity>();
			WaterCatcher waterCatcher = null;
			Vector3 position = arg.Player().transform.position;
			string[] array = (from x in GameManifest.Current.entities
			where Path.GetFileNameWithoutExtension(x).Contains(name, CompareOptions.IgnoreCase)
			select x.ToLower()).ToArray<string>();
			if (array.Length == 0)
			{
				arg.ReplyWith("Couldn't find io prefab \"" + array[0] + "\"");
				return;
			}
			if (array.Length > 1)
			{
				string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
				if (text == null)
				{
					Debug.Log(string.Format("{0} failed to find io entity \"{1}\"", arg, name));
					arg.ReplyWith("Unknown entity - could be:\n\n" + string.Join("\n", array.Select(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>()));
					return;
				}
				array[0] = text;
			}
			for (int i = 0; i < @int; i++)
			{
				Vector3 pos = position + new Vector3((float)(i * 5), 0f, 0f);
				Quaternion identity = Quaternion.identity;
				BaseEntity baseEntity = GameManager.server.CreateEntity(array[0], pos, identity, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					WaterCatcher component = baseEntity.GetComponent<WaterCatcher>();
					if (component)
					{
						list.Add(component);
						if (waterCatcher != null)
						{
							Debugging.<bench_io>g__Connect|25_0(waterCatcher, component);
						}
						if (i == @int - 1)
						{
							Debugging.<bench_io>g__Connect|25_0(component, list.First<IOEntity>());
						}
						waterCatcher = component;
					}
				}
			}
		}

		// Token: 0x060041A0 RID: 16800 RVA: 0x00185778 File Offset: 0x00183978
		[CompilerGenerated]
		internal static void <bench_io>g__Connect|25_0(IOEntity InputIOEnt, IOEntity OutputIOEnt)
		{
			int num = 0;
			int num2 = 0;
			WireTool.WireColour wireColour = WireTool.WireColour.Default;
			IOEntity.IOSlot ioslot = InputIOEnt.inputs[num];
			IOEntity.IOSlot ioslot2 = OutputIOEnt.outputs[num2];
			ioslot.connectedTo.Set(OutputIOEnt);
			ioslot.connectedToSlot = num2;
			ioslot.wireColour = wireColour;
			ioslot.connectedTo.Init();
			ioslot2.connectedTo.Set(InputIOEnt);
			ioslot2.connectedToSlot = num;
			ioslot2.wireColour = wireColour;
			ioslot2.connectedTo.Init();
			ioslot2.linePoints = new Vector3[]
			{
				Vector3.zero,
				OutputIOEnt.transform.InverseTransformPoint(InputIOEnt.transform.TransformPoint(ioslot.handlePosition))
			};
			OutputIOEnt.MarkDirtyForceUpdateOutputs();
			OutputIOEnt.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			InputIOEnt.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			OutputIOEnt.SendChangedToRoot(true);
		}
	}
}
