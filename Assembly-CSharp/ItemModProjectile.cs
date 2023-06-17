using System;
using Rust;
using UnityEngine;

// Token: 0x020005F3 RID: 1523
public class ItemModProjectile : MonoBehaviour
{
	// Token: 0x040024FD RID: 9469
	public GameObjectRef projectileObject = new GameObjectRef();

	// Token: 0x040024FE RID: 9470
	public ItemModProjectileMod[] mods;

	// Token: 0x040024FF RID: 9471
	public AmmoTypes ammoType;

	// Token: 0x04002500 RID: 9472
	public int numProjectiles = 1;

	// Token: 0x04002501 RID: 9473
	public float projectileSpread;

	// Token: 0x04002502 RID: 9474
	public float projectileVelocity = 100f;

	// Token: 0x04002503 RID: 9475
	public float projectileVelocitySpread;

	// Token: 0x04002504 RID: 9476
	public bool useCurve;

	// Token: 0x04002505 RID: 9477
	public AnimationCurve spreadScalar;

	// Token: 0x04002506 RID: 9478
	public GameObjectRef attackEffectOverride;

	// Token: 0x04002507 RID: 9479
	public float barrelConditionLoss;

	// Token: 0x04002508 RID: 9480
	public string category = "bullet";

	// Token: 0x06002D68 RID: 11624 RVA: 0x001114AE File Offset: 0x0010F6AE
	public float GetRandomVelocity()
	{
		return this.projectileVelocity + UnityEngine.Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x001114C9 File Offset: 0x0010F6C9
	public float GetSpreadScalar()
	{
		if (this.useCurve)
		{
			return this.spreadScalar.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		return 1f;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x001114F4 File Offset: 0x0010F6F4
	public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
	{
		float time;
		if (shotIndex != -1)
		{
			float num = 1f / (float)maxShots;
			time = (float)shotIndex * num;
		}
		else
		{
			time = UnityEngine.Random.Range(0f, 1f);
		}
		return this.spreadScalar.Evaluate(time);
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x00111537 File Offset: 0x0010F737
	public float GetAverageVelocity()
	{
		return this.projectileVelocity;
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x0011153F File Offset: 0x0010F73F
	public float GetMinVelocity()
	{
		return this.projectileVelocity - this.projectileVelocitySpread;
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x0011154E File Offset: 0x0010F74E
	public float GetMaxVelocity()
	{
		return this.projectileVelocity + this.projectileVelocitySpread;
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x0011155D File Offset: 0x0010F75D
	public bool IsAmmo(AmmoTypes ammo)
	{
		return (this.ammoType & ammo) > (AmmoTypes)0;
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x0011156C File Offset: 0x0010F76C
	public virtual void ServerProjectileHit(HitInfo info)
	{
		if (this.mods == null)
		{
			return;
		}
		foreach (ItemModProjectileMod itemModProjectileMod in this.mods)
		{
			if (!(itemModProjectileMod == null))
			{
				itemModProjectileMod.ServerProjectileHit(info);
			}
		}
	}
}
