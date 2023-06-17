using System;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class BunkerEntrance : BaseEntity, IMissionEntityListener
{
	// Token: 0x040010D2 RID: 4306
	public GameObjectRef portalPrefab;

	// Token: 0x040010D3 RID: 4307
	public GameObjectRef doorPrefab;

	// Token: 0x040010D4 RID: 4308
	public Transform portalSpawnPoint;

	// Token: 0x040010D5 RID: 4309
	public Transform doorSpawnPoint;

	// Token: 0x040010D6 RID: 4310
	public Door doorInstance;

	// Token: 0x040010D7 RID: 4311
	public BasePortal portalInstance;

	// Token: 0x060017EE RID: 6126 RVA: 0x000B42B8 File Offset: 0x000B24B8
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.portalPrefab.isValid)
		{
			this.portalInstance = GameManager.server.CreateEntity(this.portalPrefab.resourcePath, this.portalSpawnPoint.position, this.portalSpawnPoint.rotation, true).GetComponent<BasePortal>();
			this.portalInstance.SetParent(this, true, false);
			this.portalInstance.Spawn();
		}
		if (this.doorPrefab.isValid)
		{
			this.doorInstance = GameManager.server.CreateEntity(this.doorPrefab.resourcePath, this.doorSpawnPoint.position, this.doorSpawnPoint.rotation, true).GetComponent<Door>();
			this.doorInstance.SetParent(this, true, false);
			this.doorInstance.Spawn();
		}
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x000063A5 File Offset: 0x000045A5
	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}
}
