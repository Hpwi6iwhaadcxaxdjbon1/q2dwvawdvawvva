using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200042B RID: 1067
public class WaterBall : BaseEntity
{
	// Token: 0x04001C18 RID: 7192
	public ItemDefinition liquidType;

	// Token: 0x04001C19 RID: 7193
	public int waterAmount;

	// Token: 0x04001C1A RID: 7194
	public GameObjectRef waterExplosion;

	// Token: 0x04001C1B RID: 7195
	public Rigidbody myRigidBody;

	// Token: 0x060023FD RID: 9213 RVA: 0x000E5D4C File Offset: 0x000E3F4C
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.Extinguish), 10f);
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000E5D6B File Offset: 0x000E3F6B
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000E5D8E File Offset: 0x000E3F8E
	public void FixedUpdate()
	{
		if (base.isServer)
		{
			base.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
		}
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000E5DAC File Offset: 0x000E3FAC
	public static bool DoSplash(Vector3 position, float radius, ItemDefinition liquidDef, int amount)
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, radius, list, 1219701523, QueryTriggerInteraction.Collide);
		int num = 0;
		int num2 = amount;
		while (amount > 0 && num < 3)
		{
			List<ISplashable> list2 = Pool.GetList<ISplashable>();
			foreach (BaseEntity baseEntity in list)
			{
				if (!baseEntity.isClient)
				{
					ISplashable splashable = baseEntity as ISplashable;
					if (splashable != null && !list2.Contains(splashable) && splashable.WantsSplash(liquidDef, amount))
					{
						list2.Add(splashable);
					}
				}
			}
			if (list2.Count == 0)
			{
				break;
			}
			int b = Mathf.CeilToInt((float)(amount / list2.Count));
			foreach (ISplashable splashable2 in list2)
			{
				int num3 = splashable2.DoSplash(liquidDef, Mathf.Min(amount, b));
				amount -= num3;
				if (amount <= 0)
				{
					break;
				}
			}
			Pool.FreeList<ISplashable>(ref list2);
			num++;
		}
		Pool.FreeList<BaseEntity>(ref list);
		return amount < num2;
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000E5ED8 File Offset: 0x000E40D8
	private void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (this.myRigidBody.isKinematic)
		{
			return;
		}
		float num = 2.5f;
		WaterBall.DoSplash(base.transform.position + new Vector3(0f, num * 0.75f, 0f), num, this.liquidType, this.waterAmount);
		Effect.server.Run(this.waterExplosion.resourcePath, base.transform.position + new Vector3(0f, 0f, 0f), Vector3.up, null, false);
		this.myRigidBody.isKinematic = true;
		base.Invoke(new Action(this.Extinguish), 2f);
	}
}
