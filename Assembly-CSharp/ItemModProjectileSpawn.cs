using System;
using ConVar;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
public class ItemModProjectileSpawn : ItemModProjectile
{
	// Token: 0x0400250D RID: 9485
	public float createOnImpactChance;

	// Token: 0x0400250E RID: 9486
	public GameObjectRef createOnImpact = new GameObjectRef();

	// Token: 0x0400250F RID: 9487
	public float spreadAngle = 30f;

	// Token: 0x04002510 RID: 9488
	public float spreadVelocityMin = 1f;

	// Token: 0x04002511 RID: 9489
	public float spreadVelocityMax = 3f;

	// Token: 0x04002512 RID: 9490
	public int numToCreateChances = 1;

	// Token: 0x06002D75 RID: 11637 RVA: 0x001117B8 File Offset: 0x0010F9B8
	public override void ServerProjectileHit(HitInfo info)
	{
		for (int i = 0; i < this.numToCreateChances; i++)
		{
			if (this.createOnImpact.isValid && UnityEngine.Random.Range(0f, 1f) < this.createOnImpactChance)
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 pointStart = info.PointStart;
				Vector3 normalized = info.ProjectileVelocity.normalized;
				Vector3 normalized2 = info.HitNormalWorld.normalized;
				Vector3 vector = hitPositionWorld - normalized * 0.1f;
				Quaternion rotation = Quaternion.LookRotation(-normalized);
				int layerMask = ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688;
				if (GamePhysics.LineOfSight(pointStart, vector, layerMask, null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnImpact.resourcePath, default(Vector3), default(Quaternion), true);
					if (baseEntity)
					{
						baseEntity.transform.position = vector;
						baseEntity.transform.rotation = rotation;
						baseEntity.Spawn();
						if (this.spreadAngle > 0f)
						{
							Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, normalized2, true);
							baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(1f, 3f));
						}
					}
				}
			}
		}
		base.ServerProjectileHit(info);
	}
}
