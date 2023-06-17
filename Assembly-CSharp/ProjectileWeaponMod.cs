using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000433 RID: 1075
public class ProjectileWeaponMod : BaseEntity
{
	// Token: 0x04001C43 RID: 7235
	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	// Token: 0x04001C44 RID: 7236
	public bool isSilencer;

	// Token: 0x04001C45 RID: 7237
	[Header("Weapon Basics")]
	public ProjectileWeaponMod.Modifier repeatDelay;

	// Token: 0x04001C46 RID: 7238
	public ProjectileWeaponMod.Modifier projectileVelocity;

	// Token: 0x04001C47 RID: 7239
	public ProjectileWeaponMod.Modifier projectileDamage;

	// Token: 0x04001C48 RID: 7240
	public ProjectileWeaponMod.Modifier projectileDistance;

	// Token: 0x04001C49 RID: 7241
	[Header("Recoil")]
	public ProjectileWeaponMod.Modifier aimsway;

	// Token: 0x04001C4A RID: 7242
	public ProjectileWeaponMod.Modifier aimswaySpeed;

	// Token: 0x04001C4B RID: 7243
	public ProjectileWeaponMod.Modifier recoil;

	// Token: 0x04001C4C RID: 7244
	[Header("Aim Cone")]
	public ProjectileWeaponMod.Modifier sightAimCone;

	// Token: 0x04001C4D RID: 7245
	public ProjectileWeaponMod.Modifier hipAimCone;

	// Token: 0x04001C4E RID: 7246
	[Header("Light Effects")]
	public bool isLight;

	// Token: 0x04001C4F RID: 7247
	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	// Token: 0x04001C50 RID: 7248
	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	// Token: 0x04001C51 RID: 7249
	[Header("Scope")]
	public bool isScope;

	// Token: 0x04001C52 RID: 7250
	public float zoomAmountDisplayOnly;

	// Token: 0x04001C53 RID: 7251
	[Header("Magazine")]
	public ProjectileWeaponMod.Modifier magazineCapacity;

	// Token: 0x04001C54 RID: 7252
	public bool needsOnForEffects;

	// Token: 0x04001C55 RID: 7253
	[Header("Burst")]
	public int burstCount = -1;

	// Token: 0x04001C56 RID: 7254
	public float timeBetweenBursts;

	// Token: 0x06002444 RID: 9284 RVA: 0x000E6F2E File Offset: 0x000E512E
	public override void ServerInit()
	{
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		base.ServerInit();
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000E6F41 File Offset: 0x000E5141
	public override void PostServerLoad()
	{
		base.limitNetworking = base.HasFlag(BaseEntity.Flags.Disabled);
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000E6F54 File Offset: 0x000E5154
	public static float Mult(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		float num = 1f;
		foreach (float num2 in mods)
		{
			num *= num2;
		}
		return num;
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x000E6FB4 File Offset: 0x000E51B4
	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Sum();
		}
		return def;
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x000E6FE4 File Offset: 0x000E51E4
	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Average();
		}
		return def;
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x000E7014 File Offset: 0x000E5214
	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Max();
		}
		return def;
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000E7044 File Offset: 0x000E5244
	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() != 0)
		{
			return mods.Min();
		}
		return def;
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x000E7074 File Offset: 0x000E5274
	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value)
	{
		return (from x in (from ProjectileWeaponMod x in parentEnt.children
		where x != null && (!x.needsOnForEffects || x.HasFlag(BaseEntity.Flags.On))
		select x).Select(selector_modifier)
		where x.enabled
		select x).Select(selector_value);
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000E70E0 File Offset: 0x000E52E0
	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		return parentEnt.children.Cast<ProjectileWeaponMod>().Any((ProjectileWeaponMod x) => x != null && x.IsBroken());
	}

	// Token: 0x02000CE2 RID: 3298
	[Serializable]
	public struct Modifier
	{
		// Token: 0x0400454C RID: 17740
		public bool enabled;

		// Token: 0x0400454D RID: 17741
		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		// Token: 0x0400454E RID: 17742
		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}
}
