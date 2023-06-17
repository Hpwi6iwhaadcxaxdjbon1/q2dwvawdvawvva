using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADA RID: 2778
	[ConsoleSystem.Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		// Token: 0x04003C18 RID: 15384
		[ServerVar]
		public static float min_rate = 0.5f;

		// Token: 0x04003C19 RID: 15385
		[ServerVar]
		public static float max_rate = 1f;

		// Token: 0x04003C1A RID: 15386
		[ServerVar]
		public static float min_density = 0.5f;

		// Token: 0x04003C1B RID: 15387
		[ServerVar]
		public static float max_density = 1f;

		// Token: 0x04003C1C RID: 15388
		[ServerVar]
		public static float player_base = 100f;

		// Token: 0x04003C1D RID: 15389
		[ServerVar]
		public static float player_scale = 2f;

		// Token: 0x04003C1E RID: 15390
		[ServerVar]
		public static bool respawn_populations = true;

		// Token: 0x04003C1F RID: 15391
		[ServerVar]
		public static bool respawn_groups = true;

		// Token: 0x04003C20 RID: 15392
		[ServerVar]
		public static bool respawn_individuals = true;

		// Token: 0x04003C21 RID: 15393
		[ServerVar]
		public static float tick_populations = 60f;

		// Token: 0x04003C22 RID: 15394
		[ServerVar]
		public static float tick_individuals = 300f;

		// Token: 0x060042FE RID: 17150 RVA: 0x0018CA57 File Offset: 0x0018AC57
		[ServerVar]
		public static void fill_populations(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillPopulations();
			}
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x0018CA6F File Offset: 0x0018AC6F
		[ServerVar]
		public static void fill_groups(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillGroups();
			}
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x0018CA87 File Offset: 0x0018AC87
		[ServerVar]
		public static void fill_individuals(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillIndividuals();
			}
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x0018CA9F File Offset: 0x0018AC9F
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				args.ReplyWith(SingletonComponent<SpawnHandler>.Instance.GetReport(false));
				return;
			}
			args.ReplyWith("No spawn handler found.");
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x0018CACC File Offset: 0x0018ACCC
		[ServerVar]
		public static void scalars(ConsoleSystem.Arg args)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("Type");
			textTable.AddColumn("Value");
			textTable.AddRow(new string[]
			{
				"Player Fraction",
				SpawnHandler.PlayerFraction().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Player Excess",
				SpawnHandler.PlayerExcess().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Rate",
				SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Density",
				SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Group Rate",
				SpawnHandler.PlayerScale(Spawn.player_scale).ToString()
			});
			args.ReplyWith(args.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x0018CBE4 File Offset: 0x0018ADE4
		[ServerVar]
		public static void cargoshipevent(ConsoleSystem.Arg args)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/vehicles/boats/cargoship/cargoshiptest.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity != null)
			{
				baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
				baseEntity.Spawn();
				args.ReplyWith("Cargo ship event has been started");
				return;
			}
			args.ReplyWith("Couldn't find cargo ship prefab - maybe it has been renamed?");
		}
	}
}
