using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class Projectile : BaseMonoBehaviour
{
	// Token: 0x040016EE RID: 5870
	public const float moveDeltaTime = 0.03125f;

	// Token: 0x040016EF RID: 5871
	public const float lifeTime = 8f;

	// Token: 0x040016F0 RID: 5872
	[Header("Attributes")]
	public Vector3 initialVelocity;

	// Token: 0x040016F1 RID: 5873
	public float drag;

	// Token: 0x040016F2 RID: 5874
	public float gravityModifier = 1f;

	// Token: 0x040016F3 RID: 5875
	public float thickness;

	// Token: 0x040016F4 RID: 5876
	[Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
	public float initialDistance;

	// Token: 0x040016F5 RID: 5877
	[Header("Impact Rules")]
	public bool remainInWorld;

	// Token: 0x040016F6 RID: 5878
	[Range(0f, 1f)]
	public float stickProbability = 1f;

	// Token: 0x040016F7 RID: 5879
	[Range(0f, 1f)]
	public float breakProbability;

	// Token: 0x040016F8 RID: 5880
	[Range(0f, 1f)]
	public float conditionLoss;

	// Token: 0x040016F9 RID: 5881
	[Range(0f, 1f)]
	public float ricochetChance;

	// Token: 0x040016FA RID: 5882
	public float penetrationPower = 1f;

	// Token: 0x040016FB RID: 5883
	[Header("Damage")]
	public DamageProperties damageProperties;

	// Token: 0x040016FC RID: 5884
	[Horizontal(2, -1)]
	public MinMax damageDistances = new MinMax(10f, 100f);

	// Token: 0x040016FD RID: 5885
	[Horizontal(2, -1)]
	public MinMax damageMultipliers = new MinMax(1f, 0.8f);

	// Token: 0x040016FE RID: 5886
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x040016FF RID: 5887
	[Header("Rendering")]
	public ScaleRenderer rendererToScale;

	// Token: 0x04001700 RID: 5888
	public ScaleRenderer firstPersonRenderer;

	// Token: 0x04001701 RID: 5889
	public bool createDecals = true;

	// Token: 0x04001702 RID: 5890
	[Header("Effects")]
	public bool doDefaultHitEffects = true;

	// Token: 0x04001703 RID: 5891
	[Header("Audio")]
	public SoundDefinition flybySound;

	// Token: 0x04001704 RID: 5892
	public float flybySoundDistance = 7f;

	// Token: 0x04001705 RID: 5893
	public SoundDefinition closeFlybySound;

	// Token: 0x04001706 RID: 5894
	public float closeFlybyDistance = 3f;

	// Token: 0x04001707 RID: 5895
	[Header("Tumble")]
	public float tumbleSpeed;

	// Token: 0x04001708 RID: 5896
	public Vector3 tumbleAxis = Vector3.right;

	// Token: 0x04001709 RID: 5897
	[Header("Swim")]
	public Vector3 swimScale;

	// Token: 0x0400170A RID: 5898
	public Vector3 swimSpeed;

	// Token: 0x0400170B RID: 5899
	[NonSerialized]
	public BasePlayer owner;

	// Token: 0x0400170C RID: 5900
	[NonSerialized]
	public AttackEntity sourceWeaponPrefab;

	// Token: 0x0400170D RID: 5901
	[NonSerialized]
	public Projectile sourceProjectilePrefab;

	// Token: 0x0400170E RID: 5902
	[NonSerialized]
	public ItemModProjectile mod;

	// Token: 0x0400170F RID: 5903
	[NonSerialized]
	public int projectileID;

	// Token: 0x04001710 RID: 5904
	[NonSerialized]
	public int seed;

	// Token: 0x04001711 RID: 5905
	[NonSerialized]
	public bool clientsideEffect;

	// Token: 0x04001712 RID: 5906
	[NonSerialized]
	public bool clientsideAttack;

	// Token: 0x04001713 RID: 5907
	[NonSerialized]
	public float integrity = 1f;

	// Token: 0x04001714 RID: 5908
	[NonSerialized]
	public float maxDistance = float.PositiveInfinity;

	// Token: 0x04001715 RID: 5909
	[NonSerialized]
	public Projectile.Modifier modifier = Projectile.Modifier.Default;

	// Token: 0x04001716 RID: 5910
	[NonSerialized]
	public bool invisible;

	// Token: 0x04001717 RID: 5911
	private static uint _fleshMaterialID;

	// Token: 0x04001718 RID: 5912
	private static uint _waterMaterialID;

	// Token: 0x04001719 RID: 5913
	private static uint cachedWaterString;

	// Token: 0x06001DDA RID: 7642 RVA: 0x000CC2C8 File Offset: 0x000CA4C8
	public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
	{
		float num = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
		float num2 = scale * (mod.damageOffset + mod.damageScale * num);
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			info.damageTypes.Add(damageTypeEntry.type, damageTypeEntry.amount * num2);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				" Projectile damage: ",
				info.damageTypes.Total(),
				" (scalar=",
				num2,
				")"
			}));
		}
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x000CC3D0 File Offset: 0x000CA5D0
	public static uint FleshMaterialID()
	{
		if (Projectile._fleshMaterialID == 0U)
		{
			Projectile._fleshMaterialID = StringPool.Get("flesh");
		}
		return Projectile._fleshMaterialID;
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x000CC3ED File Offset: 0x000CA5ED
	public static uint WaterMaterialID()
	{
		if (Projectile._waterMaterialID == 0U)
		{
			Projectile._waterMaterialID = StringPool.Get("Water");
		}
		return Projectile._waterMaterialID;
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x000CC40A File Offset: 0x000CA60A
	public static bool IsWaterMaterial(string hitMaterial)
	{
		if (Projectile.cachedWaterString == 0U)
		{
			Projectile.cachedWaterString = StringPool.Get("Water");
		}
		return StringPool.Get(hitMaterial) == Projectile.cachedWaterString;
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x000CC434 File Offset: 0x000CA634
	public static bool ShouldStopProjectile(RaycastHit hit)
	{
		BaseEntity entity = hit.GetEntity();
		return !(entity != null) || entity.ShouldBlockProjectiles();
	}

	// Token: 0x02000C98 RID: 3224
	public struct Modifier
	{
		// Token: 0x04004400 RID: 17408
		public float damageScale;

		// Token: 0x04004401 RID: 17409
		public float damageOffset;

		// Token: 0x04004402 RID: 17410
		public float distanceScale;

		// Token: 0x04004403 RID: 17411
		public float distanceOffset;

		// Token: 0x04004404 RID: 17412
		public static Projectile.Modifier Default = new Projectile.Modifier
		{
			damageScale = 1f,
			damageOffset = 0f,
			distanceScale = 1f,
			distanceOffset = 0f
		};
	}
}
