using System;
using ConVar;
using UnityEngine;

// Token: 0x0200045A RID: 1114
public class BushEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x04001D41 RID: 7489
	public GameObjectRef prefab;

	// Token: 0x04001D42 RID: 7490
	public bool globalBillboard = true;

	// Token: 0x060024DC RID: 9436 RVA: 0x000E9850 File Offset: 0x000E7A50
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(this.prefabID);
			base.transform.ApplyDecorComponentsScaleOnly(components);
		}
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000E9888 File Offset: 0x000E7A88
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeSpawned(this);
		}
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000E989E File Offset: 0x000E7A9E
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.globalBillboard)
		{
			TreeManager.OnTreeDestroyed(this);
		}
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000A0D51 File Offset: 0x0009EF51
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}
}
