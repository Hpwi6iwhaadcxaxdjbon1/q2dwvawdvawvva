using System;
using UnityEngine;

// Token: 0x0200051A RID: 1306
public class HitTest
{
	// Token: 0x0400219D RID: 8605
	public HitTest.Type type;

	// Token: 0x0400219E RID: 8606
	public Ray AttackRay;

	// Token: 0x0400219F RID: 8607
	public float Radius;

	// Token: 0x040021A0 RID: 8608
	public float Forgiveness;

	// Token: 0x040021A1 RID: 8609
	public float MaxDistance;

	// Token: 0x040021A2 RID: 8610
	public RaycastHit RayHit;

	// Token: 0x040021A3 RID: 8611
	public bool MultiHit;

	// Token: 0x040021A4 RID: 8612
	public bool BestHit;

	// Token: 0x040021A5 RID: 8613
	public bool DidHit;

	// Token: 0x040021A6 RID: 8614
	public DamageProperties damageProperties;

	// Token: 0x040021A7 RID: 8615
	public GameObject gameObject;

	// Token: 0x040021A8 RID: 8616
	public Collider collider;

	// Token: 0x040021A9 RID: 8617
	public BaseEntity ignoreEntity;

	// Token: 0x040021AA RID: 8618
	public BaseEntity HitEntity;

	// Token: 0x040021AB RID: 8619
	public Vector3 HitPoint;

	// Token: 0x040021AC RID: 8620
	public Vector3 HitNormal;

	// Token: 0x040021AD RID: 8621
	public float HitDistance;

	// Token: 0x040021AE RID: 8622
	public Transform HitTransform;

	// Token: 0x040021AF RID: 8623
	public uint HitPart;

	// Token: 0x040021B0 RID: 8624
	public string HitMaterial;

	// Token: 0x0600299A RID: 10650 RVA: 0x000FF0AC File Offset: 0x000FD2AC
	public void CopyFrom(HitTest other, bool copyHitInfo = false)
	{
		this.type = other.type;
		this.AttackRay = other.AttackRay;
		this.Radius = other.Radius;
		this.Forgiveness = other.Forgiveness;
		this.MaxDistance = other.MaxDistance;
		this.RayHit = other.RayHit;
		this.damageProperties = other.damageProperties;
		this.ignoreEntity = other.ignoreEntity;
		if (copyHitInfo)
		{
			this.HitEntity = other.HitEntity;
			this.HitPoint = other.HitPoint;
			this.HitNormal = other.HitNormal;
			this.HitDistance = other.HitDistance;
			this.HitTransform = other.HitTransform;
			this.HitPart = other.HitPart;
			this.HitMaterial = other.HitMaterial;
			this.MultiHit = other.MultiHit;
			this.BestHit = other.BestHit;
			this.DidHit = other.DidHit;
		}
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000FF194 File Offset: 0x000FD394
	public Vector3 HitPointWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformPoint(this.HitPoint);
		}
		return this.HitPoint;
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000FF1E0 File Offset: 0x000FD3E0
	public Vector3 HitNormalWorld()
	{
		if (this.HitEntity != null)
		{
			Transform transform = this.HitTransform;
			if (!transform)
			{
				transform = this.HitEntity.transform;
			}
			return transform.TransformDirection(this.HitNormal);
		}
		return this.HitNormal;
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x000FF22C File Offset: 0x000FD42C
	public void Clear()
	{
		this.type = HitTest.Type.Generic;
		this.AttackRay = default(Ray);
		this.Radius = 0f;
		this.Forgiveness = 0f;
		this.MaxDistance = 0f;
		this.RayHit = default(RaycastHit);
		this.MultiHit = false;
		this.BestHit = false;
		this.DidHit = false;
		this.damageProperties = null;
		this.gameObject = null;
		this.collider = null;
		this.ignoreEntity = null;
		this.HitEntity = null;
		this.HitPoint = default(Vector3);
		this.HitNormal = default(Vector3);
		this.HitDistance = 0f;
		this.HitTransform = null;
		this.HitPart = 0U;
		this.HitMaterial = null;
	}

	// Token: 0x02000D3C RID: 3388
	public enum Type
	{
		// Token: 0x040046AA RID: 18090
		Generic,
		// Token: 0x040046AB RID: 18091
		ProjectileEffect,
		// Token: 0x040046AC RID: 18092
		Projectile,
		// Token: 0x040046AD RID: 18093
		MeleeAttack,
		// Token: 0x040046AE RID: 18094
		Use
	}
}
