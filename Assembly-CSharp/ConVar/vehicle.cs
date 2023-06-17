using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE4 RID: 2788
	[ConsoleSystem.Factory("vehicle")]
	public class vehicle : ConsoleSystem
	{
		// Token: 0x04003C33 RID: 15411
		[ServerVar]
		[Help("how long until boat corpses despawn")]
		public static float boat_corpse_seconds = 300f;

		// Token: 0x04003C34 RID: 15412
		[ServerVar(Help = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false")]
		public static bool cinematictrains = false;

		// Token: 0x04003C35 RID: 15413
		[ServerVar(Help = "Determines whether trains stop automatically when there's no-one on them. Default: false")]
		public static bool trainskeeprunning = false;

		// Token: 0x04003C36 RID: 15414
		[ServerVar(Help = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true")]
		public static bool carwrecks = true;

		// Token: 0x04003C37 RID: 15415
		[ServerVar(Help = "Determines whether vehicles drop storage items when destroyed. Default: true")]
		public static bool vehiclesdroploot = true;

		// Token: 0x06004325 RID: 17189 RVA: 0x0018D3A4 File Offset: 0x0018B5A4
		[ServerUserVar]
		public static void swapseats(ConsoleSystem.Arg arg)
		{
			int targetSeat = 0;
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.SwapSeatCooldown())
			{
				return;
			}
			BaseMountable mounted = basePlayer.GetMounted();
			if (mounted == null)
			{
				return;
			}
			BaseVehicle baseVehicle = mounted.GetComponent<BaseVehicle>();
			if (baseVehicle == null)
			{
				baseVehicle = mounted.VehicleParent();
			}
			if (baseVehicle == null)
			{
				return;
			}
			baseVehicle.SwapSeats(basePlayer, targetSeat);
		}

		// Token: 0x06004326 RID: 17190 RVA: 0x0018D408 File Offset: 0x0018B608
		[ServerVar]
		public static void fixcars(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("Null player.");
				return;
			}
			if (!basePlayer.IsAdmin)
			{
				arg.ReplyWith("Must be an admin to use fixcars.");
				return;
			}
			int num = arg.GetInt(0, 2);
			num = Mathf.Clamp(num, 1, 3);
			BaseVehicle[] array = UnityEngine.Object.FindObjectsOfType<BaseVehicle>();
			int num2 = 0;
			foreach (BaseVehicle baseVehicle in array)
			{
				if (baseVehicle.isServer && Vector3.Distance(baseVehicle.transform.position, basePlayer.transform.position) <= 10f && baseVehicle.AdminFixUp(num))
				{
					num2++;
				}
			}
			foreach (MLRS mlrs in UnityEngine.Object.FindObjectsOfType<MLRS>())
			{
				if (mlrs.isServer && Vector3.Distance(mlrs.transform.position, basePlayer.transform.position) <= 10f && mlrs.AdminFixUp())
				{
					num2++;
				}
			}
			arg.ReplyWith(string.Format("Fixed up {0} vehicles.", num2));
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x0018D524 File Offset: 0x0018B724
		[ServerVar]
		public static void stop_all_trains(ConsoleSystem.Arg arg)
		{
			TrainEngine[] array = UnityEngine.Object.FindObjectsOfType<TrainEngine>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StopEngine();
			}
			arg.ReplyWith("All trains stopped.");
		}

		// Token: 0x06004328 RID: 17192 RVA: 0x0018D558 File Offset: 0x0018B758
		[ServerVar]
		public static void killcars(ConsoleSystem.Arg args)
		{
			ModularCar[] array = BaseEntity.Util.FindAll<ModularCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004329 RID: 17193 RVA: 0x0018D584 File Offset: 0x0018B784
		[ServerVar]
		public static void killminis(ConsoleSystem.Arg args)
		{
			foreach (MiniCopter miniCopter in BaseEntity.Util.FindAll<MiniCopter>())
			{
				if (miniCopter.name.ToLower().Contains("minicopter"))
				{
					miniCopter.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}

		// Token: 0x0600432A RID: 17194 RVA: 0x0018D5C8 File Offset: 0x0018B7C8
		[ServerVar]
		public static void killscraphelis(ConsoleSystem.Arg args)
		{
			ScrapTransportHelicopter[] array = BaseEntity.Util.FindAll<ScrapTransportHelicopter>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x0600432B RID: 17195 RVA: 0x0018D5F4 File Offset: 0x0018B7F4
		[ServerVar]
		public static void killtrains(ConsoleSystem.Arg args)
		{
			TrainCar[] array = BaseEntity.Util.FindAll<TrainCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x0600432C RID: 17196 RVA: 0x0018D620 File Offset: 0x0018B820
		[ServerVar]
		public static void killboats(ConsoleSystem.Arg args)
		{
			BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x0600432D RID: 17197 RVA: 0x0018D64C File Offset: 0x0018B84C
		[ServerVar]
		public static void killdrones(ConsoleSystem.Arg args)
		{
			foreach (Drone drone in BaseEntity.Util.FindAll<Drone>())
			{
				if (!(drone is DeliveryDrone))
				{
					drone.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}
	}
}
