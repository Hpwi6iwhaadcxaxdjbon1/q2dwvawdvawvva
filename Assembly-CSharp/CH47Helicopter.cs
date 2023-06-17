using System;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class CH47Helicopter : BaseHelicopterVehicle
{
	// Token: 0x04001E43 RID: 7747
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x04001E44 RID: 7748
	private BaseEntity mapMarkerInstance;

	// Token: 0x060025F4 RID: 9716 RVA: 0x000EF9E7 File Offset: 0x000EDBE7
	public override void ServerInit()
	{
		this.rigidBody.isKinematic = false;
		base.ServerInit();
		this.CreateMapMarker();
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000EFA01 File Offset: 0x000EDC01
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000EFA0C File Offset: 0x000EDC0C
	public void CreateMapMarker()
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}
}
