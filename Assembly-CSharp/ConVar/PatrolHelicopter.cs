using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AC2 RID: 2754
	[ConsoleSystem.Factory("heli")]
	public class PatrolHelicopter : ConsoleSystem
	{
		// Token: 0x04003B78 RID: 15224
		private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

		// Token: 0x04003B79 RID: 15225
		[ServerVar]
		public static float lifetimeMinutes = 15f;

		// Token: 0x04003B7A RID: 15226
		[ServerVar]
		public static int guns = 1;

		// Token: 0x04003B7B RID: 15227
		[ServerVar]
		public static float bulletDamageScale = 1f;

		// Token: 0x04003B7C RID: 15228
		[ServerVar]
		public static float bulletAccuracy = 2f;

		// Token: 0x06004241 RID: 16961 RVA: 0x00188360 File Offset: 0x00186560
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004242 RID: 16962 RVA: 0x00188404 File Offset: 0x00186604
		[ServerVar]
		public static void calltome(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0.25f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004243 RID: 16963 RVA: 0x001884A8 File Offset: 0x001866A8
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Helicopter inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004244 RID: 16964 RVA: 0x00188500 File Offset: 0x00186700
		[ServerVar]
		public static void strafe(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			PatrolHelicopterAI heliInstance = PatrolHelicopterAI.heliInstance;
			if (heliInstance == null)
			{
				Debug.Log("no heli instance");
				return;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(basePlayer.eyes.HeadRay(), out raycastHit, 1000f, 1218652417))
			{
				Debug.Log("strafing :" + raycastHit.point);
				heliInstance.interestZoneOrigin = raycastHit.point;
				heliInstance.ExitCurrentState();
				heliInstance.State_Strafe_Enter(raycastHit.point, false);
				return;
			}
			Debug.Log("strafe ray missed");
		}

		// Token: 0x06004245 RID: 16965 RVA: 0x0018859C File Offset: 0x0018679C
		[ServerVar]
		public static void testpuzzle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			bool isDeveloper = basePlayer.IsDeveloper;
		}
	}
}
