using System;
using UnityEngine;

// Token: 0x02000426 RID: 1062
public class SupplySignal : TimedExplosive
{
	// Token: 0x04001C08 RID: 7176
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001C09 RID: 7177
	public GameObjectRef EntityToCreate;

	// Token: 0x04001C0A RID: 7178
	[NonSerialized]
	public GameObject smokeEffect;

	// Token: 0x060023E7 RID: 9191 RVA: 0x000E552C File Offset: 0x000E372C
	public override void Explode()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.EntityToCreate.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			Vector3 b = new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
			baseEntity.SendMessage("InitDropPosition", base.transform.position + b, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
		base.Invoke(new Action(this.FinishUp), 210f);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x00003384 File Offset: 0x00001584
	public void FinishUp()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}
