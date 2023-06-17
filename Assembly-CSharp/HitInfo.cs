using System;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200051B RID: 1307
public class HitInfo
{
	// Token: 0x040021B1 RID: 8625
	public global::BaseEntity Initiator;

	// Token: 0x040021B2 RID: 8626
	public global::BaseEntity WeaponPrefab;

	// Token: 0x040021B3 RID: 8627
	public AttackEntity Weapon;

	// Token: 0x040021B4 RID: 8628
	public bool DoHitEffects = true;

	// Token: 0x040021B5 RID: 8629
	public bool DoDecals = true;

	// Token: 0x040021B6 RID: 8630
	public bool IsPredicting;

	// Token: 0x040021B7 RID: 8631
	public bool UseProtection = true;

	// Token: 0x040021B8 RID: 8632
	public Connection Predicted;

	// Token: 0x040021B9 RID: 8633
	public bool DidHit;

	// Token: 0x040021BA RID: 8634
	public global::BaseEntity HitEntity;

	// Token: 0x040021BB RID: 8635
	public uint HitBone;

	// Token: 0x040021BC RID: 8636
	public uint HitPart;

	// Token: 0x040021BD RID: 8637
	public uint HitMaterial;

	// Token: 0x040021BE RID: 8638
	public Vector3 HitPositionWorld;

	// Token: 0x040021BF RID: 8639
	public Vector3 HitPositionLocal;

	// Token: 0x040021C0 RID: 8640
	public Vector3 HitNormalWorld;

	// Token: 0x040021C1 RID: 8641
	public Vector3 HitNormalLocal;

	// Token: 0x040021C2 RID: 8642
	public Vector3 PointStart;

	// Token: 0x040021C3 RID: 8643
	public Vector3 PointEnd;

	// Token: 0x040021C4 RID: 8644
	public int ProjectileID;

	// Token: 0x040021C5 RID: 8645
	public int ProjectileHits;

	// Token: 0x040021C6 RID: 8646
	public float ProjectileDistance;

	// Token: 0x040021C7 RID: 8647
	public float ProjectileIntegrity;

	// Token: 0x040021C8 RID: 8648
	public float ProjectileTravelTime;

	// Token: 0x040021C9 RID: 8649
	public float ProjectileTrajectoryMismatch;

	// Token: 0x040021CA RID: 8650
	public Vector3 ProjectileVelocity;

	// Token: 0x040021CB RID: 8651
	public Projectile ProjectilePrefab;

	// Token: 0x040021CC RID: 8652
	public PhysicMaterial material;

	// Token: 0x040021CD RID: 8653
	public DamageProperties damageProperties;

	// Token: 0x040021CE RID: 8654
	public DamageTypeList damageTypes = new DamageTypeList();

	// Token: 0x040021CF RID: 8655
	public bool CanGather;

	// Token: 0x040021D0 RID: 8656
	public bool DidGather;

	// Token: 0x040021D1 RID: 8657
	public float gatherScale = 1f;

	// Token: 0x0600299F RID: 10655 RVA: 0x000FF2E9 File Offset: 0x000FD4E9
	public bool IsProjectile()
	{
		return this.ProjectileID != 0;
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x060029A0 RID: 10656 RVA: 0x000FF2F4 File Offset: 0x000FD4F4
	public global::BasePlayer InitiatorPlayer
	{
		get
		{
			if (!this.Initiator)
			{
				return null;
			}
			return this.Initiator.ToPlayer();
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x060029A1 RID: 10657 RVA: 0x000FF310 File Offset: 0x000FD510
	public Vector3 attackNormal
	{
		get
		{
			return (this.PointEnd - this.PointStart).normalized;
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x060029A2 RID: 10658 RVA: 0x000FF336 File Offset: 0x000FD536
	public bool hasDamage
	{
		get
		{
			return this.damageTypes.Total() > 0f;
		}
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000FF34A File Offset: 0x000FD54A
	public HitInfo()
	{
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000FF380 File Offset: 0x000FD580
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		this.Initiator = attacker;
		this.HitEntity = target;
		this.HitPositionWorld = vhitPosition;
		if (attacker != null)
		{
			this.PointStart = attacker.transform.position;
		}
		this.damageTypes.Add(type, damageAmount);
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000FF3FC File Offset: 0x000FD5FC
	public HitInfo(global::BaseEntity attacker, global::BaseEntity target, DamageType type, float damageAmount) : this(attacker, target, type, damageAmount, target.transform.position)
	{
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000FF414 File Offset: 0x000FD614
	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		this.HitEntity = null;
		this.PointStart = attack.pointStart;
		this.PointEnd = attack.pointEnd;
		if (attack.hitID.IsValid)
		{
			this.DidHit = true;
			if (serverSide)
			{
				this.HitEntity = (global::BaseNetworkable.serverEntities.Find(attack.hitID) as global::BaseEntity);
			}
			if (this.HitEntity)
			{
				this.HitBone = attack.hitBone;
				this.HitPart = attack.hitPartID;
			}
		}
		this.DidHit = true;
		this.HitPositionLocal = attack.hitPositionLocal;
		this.HitPositionWorld = attack.hitPositionWorld;
		this.HitNormalLocal = attack.hitNormalLocal.normalized;
		this.HitNormalWorld = attack.hitNormalWorld.normalized;
		this.HitMaterial = attack.hitMaterialID;
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x060029A7 RID: 10663 RVA: 0x000FF4E4 File Offset: 0x000FD6E4
	public bool isHeadshot
	{
		get
		{
			if (this.HitEntity == null)
			{
				return false;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return false;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			return boneProperty != null && boneProperty.area == HitArea.Head;
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060029A8 RID: 10664 RVA: 0x000FF548 File Offset: 0x000FD748
	public Translate.Phrase bonePhrase
	{
		get
		{
			if (this.HitEntity == null)
			{
				return null;
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return null;
			}
			if (baseCombatEntity.skeletonProperties == null)
			{
				return null;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return null;
			}
			return boneProperty.name;
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x060029A9 RID: 10665 RVA: 0x000FF5AC File Offset: 0x000FD7AC
	public string boneName
	{
		get
		{
			Translate.Phrase bonePhrase = this.bonePhrase;
			if (bonePhrase != null)
			{
				return bonePhrase.english;
			}
			return "N/A";
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x060029AA RID: 10666 RVA: 0x000FF5D0 File Offset: 0x000FD7D0
	public HitArea boneArea
	{
		get
		{
			if (this.HitEntity == null)
			{
				return (HitArea)(-1);
			}
			BaseCombatEntity baseCombatEntity = this.HitEntity as BaseCombatEntity;
			if (baseCombatEntity == null)
			{
				return (HitArea)(-1);
			}
			return baseCombatEntity.SkeletonLookup(this.HitBone);
		}
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000FF610 File Offset: 0x000FD810
	public Vector3 PositionOnRay(Vector3 position)
	{
		Ray ray = new Ray(this.PointStart, this.attackNormal);
		if (this.ProjectilePrefab == null)
		{
			return ray.ClosestPoint(position);
		}
		Sphere sphere = new Sphere(position, this.ProjectilePrefab.thickness);
		RaycastHit raycastHit;
		if (sphere.Trace(ray, out raycastHit, float.PositiveInfinity))
		{
			return raycastHit.point;
		}
		return position;
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000FF673 File Offset: 0x000FD873
	public Vector3 HitPositionOnRay()
	{
		return this.PositionOnRay(this.HitPositionWorld);
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000FF684 File Offset: 0x000FD884
	public bool IsNaNOrInfinity()
	{
		return this.PointStart.IsNaNOrInfinity() || this.PointEnd.IsNaNOrInfinity() || this.HitPositionWorld.IsNaNOrInfinity() || this.HitPositionLocal.IsNaNOrInfinity() || this.HitNormalWorld.IsNaNOrInfinity() || this.HitNormalLocal.IsNaNOrInfinity() || this.ProjectileVelocity.IsNaNOrInfinity() || float.IsNaN(this.ProjectileDistance) || float.IsInfinity(this.ProjectileDistance) || float.IsNaN(this.ProjectileIntegrity) || float.IsInfinity(this.ProjectileIntegrity) || float.IsNaN(this.ProjectileTravelTime) || float.IsInfinity(this.ProjectileTravelTime) || float.IsNaN(this.ProjectileTrajectoryMismatch) || float.IsInfinity(this.ProjectileTrajectoryMismatch);
	}
}
