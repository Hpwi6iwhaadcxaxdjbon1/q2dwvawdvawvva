using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADE RID: 2782
	[ConsoleSystem.Factory("supply")]
	public class Supply : ConsoleSystem
	{
		// Token: 0x04003C2E RID: 15406
		private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";

		// Token: 0x0600430F RID: 17167 RVA: 0x0018CD78 File Offset: 0x0018AF78
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<CargoPlane>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x0018CE00 File Offset: 0x0018B000
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
	}
}
