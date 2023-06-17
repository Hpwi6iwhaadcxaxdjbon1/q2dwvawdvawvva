using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class Effect : EffectData
{
	// Token: 0x04001822 RID: 6178
	public Vector3 Up;

	// Token: 0x04001823 RID: 6179
	public Vector3 worldPos;

	// Token: 0x04001824 RID: 6180
	public Vector3 worldNrm;

	// Token: 0x04001825 RID: 6181
	public bool attached;

	// Token: 0x04001826 RID: 6182
	public Transform transform;

	// Token: 0x04001827 RID: 6183
	public GameObject gameObject;

	// Token: 0x04001828 RID: 6184
	public string pooledString;

	// Token: 0x04001829 RID: 6185
	public bool broadcast;

	// Token: 0x0400182A RID: 6186
	private static Effect reusableInstace = new Effect();

	// Token: 0x06001EFB RID: 7931 RVA: 0x000D29A2 File Offset: 0x000D0BA2
	public Effect()
	{
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000D29AA File Offset: 0x000D0BAA
	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000D29C4 File Offset: 0x000D0BC4
	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		this.pooledString = effectName;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000D29E4 File Offset: 0x000D0BE4
	public void Init(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = true;
		this.origin = posLocal;
		this.normal = normLocal;
		this.gameObject = null;
		this.Up = Vector3.zero;
		if (ent != null && !ent.IsValid())
		{
			Debug.LogWarning("Effect.Init - invalid entity");
		}
		this.entity = (ent.IsValid() ? ent.net.ID : default(NetworkableId));
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
		this.bone = boneID;
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000D2A84 File Offset: 0x000D0C84
	public void Init(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = false;
		this.worldPos = posWorld;
		this.worldNrm = normWorld;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.entity = default(NetworkableId);
		this.origin = this.worldPos;
		this.normal = this.worldNrm;
		this.bone = 0U;
		this.source = ((sourceConnection != null) ? sourceConnection.userid : 0UL);
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000D2B05 File Offset: 0x000D0D05
	public void Clear()
	{
		this.worldPos = Vector3.zero;
		this.worldNrm = Vector3.zero;
		this.attached = false;
		this.transform = null;
		this.gameObject = null;
		this.pooledString = null;
		this.broadcast = false;
	}

	// Token: 0x02000CAA RID: 3242
	public enum Type : uint
	{
		// Token: 0x04004465 RID: 17509
		Generic,
		// Token: 0x04004466 RID: 17510
		Projectile,
		// Token: 0x04004467 RID: 17511
		GenericGlobal
	}

	// Token: 0x02000CAB RID: 3243
	public static class client
	{
		// Token: 0x06004F76 RID: 20342 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x001A61AA File Offset: 0x001A43AA
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x000063A5 File Offset: 0x000045A5
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = default(Vector3))
		{
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x001A61AA File Offset: 0x001A43AA
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Vector3 up = default(Vector3), Effect.Type overrideType = Effect.Type.Generic)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004F7A RID: 20346 RVA: 0x001A61AA File Offset: 0x001A43AA
		public static void Run(string strName, GameObject obj)
		{
			string.IsNullOrEmpty(strName);
		}

		// Token: 0x06004F7B RID: 20347 RVA: 0x001A61B4 File Offset: 0x001A43B4
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.client.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + info.HitNormalLocal * 0.1f, info.HitNormalLocal);
				return;
			}
			Effect.client.Run(effectName, info.HitPositionWorld + info.HitNormalWorld * 0.1f, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x001A6234 File Offset: 0x001A4434
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, null))
			{
				return;
			}
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Effect.client.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			else
			{
				Effect.Type overrideType = Effect.Type.Generic;
				Effect.client.Run(strName, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
				Effect.client.Run(decal, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
					else
					{
						Effect.client.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), Effect.Type.Generic);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	// Token: 0x02000CAC RID: 3244
	public static class server
	{
		// Token: 0x06004F7D RID: 20349 RVA: 0x001A6429 File Offset: 0x001A4629
		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004F7E RID: 20350 RVA: 0x001A6453 File Offset: 0x001A4653
		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004F7F RID: 20351 RVA: 0x001A6491 File Offset: 0x001A4691
		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004F80 RID: 20352 RVA: 0x001A64B7 File Offset: 0x001A46B7
		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		// Token: 0x06004F81 RID: 20353 RVA: 0x001A64F4 File Offset: 0x001A46F4
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (info.HitEntity.IsValid())
			{
				Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				return;
			}
			Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
		}

		// Token: 0x06004F82 RID: 20354 RVA: 0x001A6550 File Offset: 0x001A4750
		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, false, null))
			{
				return;
			}
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Bounds bounds = info.HitEntity.bounds;
				float num = info.HitEntity.BoundsPadding();
				bounds.extents += new Vector3(num, num, num);
				if (!bounds.Contains(info.HitPositionLocal))
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					if (initiatorPlayer != null && initiatorPlayer.GetType() == typeof(BasePlayer))
					{
						float num2 = Mathf.Sqrt(bounds.SqrDistance(info.HitPositionLocal));
						AntiHack.Log(initiatorPlayer, AntiHackType.EffectHack, string.Format("Tried to run an impact effect outside of entity '{0}' bounds by {1}m", info.HitEntity.ShortPrefabName, num2));
					}
					return;
				}
				Effect.server.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
			}
			else
			{
				Effect.server.Run(strName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if (baseMelee != null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
					}
					else
					{
						Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}
}
