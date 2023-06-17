using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000500 RID: 1280
public class EffectDictionary
{
	// Token: 0x0400211C RID: 8476
	private static Dictionary<string, string[]> effectDictionary;

	// Token: 0x06002920 RID: 10528 RVA: 0x000FCB72 File Offset: 0x000FAD72
	public static string GetParticle(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("impacts", impactType, materialName);
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000FCB80 File Offset: 0x000FAD80
	public static string GetParticle(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetParticle("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetParticle("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetParticle("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetParticle("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetParticle("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetParticle("blunt", materialName);
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000FCC00 File Offset: 0x000FAE00
	public static string GetDecal(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("decals", impactType, materialName);
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000FCC10 File Offset: 0x000FAE10
	public static string GetDecal(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
		case DamageType.Bullet:
			return EffectDictionary.GetDecal("bullet", materialName);
		case DamageType.Slash:
			return EffectDictionary.GetDecal("slash", materialName);
		case DamageType.Blunt:
			return EffectDictionary.GetDecal("blunt", materialName);
		case DamageType.Fall:
		case DamageType.Radiation:
		case DamageType.Bite:
			break;
		case DamageType.Stab:
			return EffectDictionary.GetDecal("stab", materialName);
		default:
			if (damageType == DamageType.Arrow)
			{
				return EffectDictionary.GetDecal("bullet", materialName);
			}
			break;
		}
		return EffectDictionary.GetDecal("blunt", materialName);
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000FCC90 File Offset: 0x000FAE90
	public static string GetDisplacement(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("displacement", impactType, materialName);
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000FCCA0 File Offset: 0x000FAEA0
	private static string LookupEffect(string category, string effect, string material)
	{
		if (EffectDictionary.effectDictionary == null)
		{
			EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
		}
		string format = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
		string[] array;
		if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, material), out array) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(format, category, effect, "generic"), out array))
		{
			return string.Empty;
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}
}
