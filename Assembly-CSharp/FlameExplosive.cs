using System;
using UnityEngine;

// Token: 0x020003F0 RID: 1008
public class FlameExplosive : TimedExplosive
{
	// Token: 0x04001A93 RID: 6803
	public GameObjectRef createOnExplode;

	// Token: 0x04001A94 RID: 6804
	public bool blockCreateUnderwater;

	// Token: 0x04001A95 RID: 6805
	public float numToCreate = 10f;

	// Token: 0x04001A96 RID: 6806
	public float minVelocity = 2f;

	// Token: 0x04001A97 RID: 6807
	public float maxVelocity = 5f;

	// Token: 0x04001A98 RID: 6808
	public float spreadAngle = 90f;

	// Token: 0x04001A99 RID: 6809
	public bool forceUpForExplosion;

	// Token: 0x04001A9A RID: 6810
	public AnimationCurve velocityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04001A9B RID: 6811
	public AnimationCurve spreadCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x06002292 RID: 8850 RVA: 0x000DE814 File Offset: 0x000DCA14
	public override void Explode()
	{
		this.FlameExplode(this.forceUpForExplosion ? Vector3.up : (-base.transform.forward));
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000DE83C File Offset: 0x000DCA3C
	public void FlameExplode(Vector3 surfaceNormal)
	{
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (this.blockCreateUnderwater && WaterLevel.Test(position, true, null))
		{
			base.Explode();
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		int num = 0;
		while ((float)num < this.numToCreate)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnExplode.resourcePath, position, default(Quaternion), true);
			if (baseEntity)
			{
				float num2 = (float)num / this.numToCreate;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle * this.spreadCurve.Evaluate(num2), surfaceNormal, true);
				baseEntity.transform.SetPositionAndRotation(position, Quaternion.LookRotation(modifiedAimConeDirection));
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.Spawn();
				Vector3 vector = modifiedAimConeDirection.normalized * UnityEngine.Random.Range(this.minVelocity, this.maxVelocity) * this.velocityCurve.Evaluate(num2 * UnityEngine.Random.Range(1f, 1.1f));
				FireBall component2 = baseEntity.GetComponent<FireBall>();
				if (component2 != null)
				{
					component2.SetDelayedVelocity(vector);
				}
				else
				{
					baseEntity.SetVelocity(vector);
				}
			}
			num++;
		}
		base.Explode();
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000DE99C File Offset: 0x000DCB9C
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.FlameExplode(info.normal);
	}
}
