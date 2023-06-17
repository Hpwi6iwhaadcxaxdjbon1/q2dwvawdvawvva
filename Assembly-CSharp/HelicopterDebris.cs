using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200041B RID: 1051
public class HelicopterDebris : ServerGib
{
	// Token: 0x04001B9E RID: 7070
	public ItemDefinition metalFragments;

	// Token: 0x04001B9F RID: 7071
	public ItemDefinition hqMetal;

	// Token: 0x04001BA0 RID: 7072
	public ItemDefinition charcoal;

	// Token: 0x04001BA1 RID: 7073
	[Tooltip("Divide mass by this amount to produce a scalar of resources, default = 5")]
	public float massReductionScalar = 5f;

	// Token: 0x04001BA2 RID: 7074
	private ResourceDispenser resourceDispenser;

	// Token: 0x04001BA3 RID: 7075
	private float tooHotUntil;

	// Token: 0x0600236A RID: 9066 RVA: 0x000E270F File Offset: 0x000E090F
	public override void ServerInit()
	{
		base.ServerInit();
		this.tooHotUntil = Time.realtimeSinceStartup + 480f;
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000E2728 File Offset: 0x000E0928
	public override void PhysicsInit(Mesh mesh)
	{
		base.PhysicsInit(mesh);
		if (base.isServer)
		{
			this.resourceDispenser = base.GetComponent<ResourceDispenser>();
			float num = Mathf.Clamp01(base.GetComponent<Rigidbody>().mass / this.massReductionScalar);
			this.resourceDispenser.containedItems = new List<ItemAmount>();
			if (num > 0.75f && this.hqMetal != null)
			{
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.hqMetal, (float)Mathf.CeilToInt(7f * num)));
			}
			if (num > 0f)
			{
				if (this.metalFragments != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.metalFragments, (float)Mathf.CeilToInt(150f * num)));
				}
				if (this.charcoal != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.charcoal, (float)Mathf.CeilToInt(80f * num)));
				}
			}
			this.resourceDispenser.Initialize();
		}
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000E2838 File Offset: 0x000E0A38
	public bool IsTooHot()
	{
		return this.tooHotUntil > Time.realtimeSinceStartup;
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x000E2848 File Offset: 0x000E0A48
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsTooHot() && info.WeaponPrefab is BaseMelee)
		{
			if (info.Initiator is BasePlayer)
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo.damageTypes.Add(DamageType.Heat, 5f);
				hitInfo.DoHitEffects = true;
				hitInfo.DidHit = true;
				hitInfo.HitBone = 0U;
				hitInfo.Initiator = this;
				hitInfo.PointStart = base.transform.position;
				Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
				return;
			}
		}
		else
		{
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			base.OnAttacked(info);
		}
	}
}
