using System;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class TriggeredEventPrefab : TriggeredEvent
{
	// Token: 0x040020A7 RID: 8359
	public GameObjectRef targetPrefab;

	// Token: 0x040020A8 RID: 8360
	public bool shouldBroadcastSpawn;

	// Token: 0x040020A9 RID: 8361
	public Translate.Phrase spawnPhrase;

	// Token: 0x06002880 RID: 10368 RVA: 0x000FA01C File Offset: 0x000F821C
	private void RunEvent()
	{
		Debug.Log("[event] " + this.targetPrefab.resourcePath);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.targetPrefab.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
			if (this.shouldBroadcastSpawn)
			{
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (basePlayer && basePlayer.IsConnected)
					{
						basePlayer.ShowToast(GameTip.Styles.Server_Event, this.spawnPhrase, Array.Empty<string>());
					}
				}
			}
		}
	}
}
