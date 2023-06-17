using System;
using Rust;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
[RequireComponent(typeof(Collider))]
public class TakeCollisionDamage : FacepunchBehaviour
{
	// Token: 0x04001F14 RID: 7956
	[SerializeField]
	private BaseCombatEntity entity;

	// Token: 0x04001F15 RID: 7957
	[SerializeField]
	private float minDamage = 1f;

	// Token: 0x04001F16 RID: 7958
	[SerializeField]
	private float maxDamage = 250f;

	// Token: 0x04001F17 RID: 7959
	[SerializeField]
	private float forceForAnyDamage = 20000f;

	// Token: 0x04001F18 RID: 7960
	[SerializeField]
	private float forceForMaxDamage = 1000000f;

	// Token: 0x04001F19 RID: 7961
	[SerializeField]
	private float velocityRestorePercent = 0.75f;

	// Token: 0x04001F1A RID: 7962
	private float pendingDamage;

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x060026E3 RID: 9955 RVA: 0x000F2CC3 File Offset: 0x000F0EC3
	private bool IsServer
	{
		get
		{
			return this.entity.isServer;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x060026E4 RID: 9956 RVA: 0x000F2CD0 File Offset: 0x000F0ED0
	private bool IsClient
	{
		get
		{
			return this.entity.isClient;
		}
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000F2CE0 File Offset: 0x000F0EE0
	protected void OnCollisionEnter(Collision collision)
	{
		if (this.IsClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		Rigidbody rigidbody = collision.rigidbody;
		float num = (rigidbody == null) ? 100f : rigidbody.mass;
		float value = collision.relativeVelocity.magnitude * (this.entity.RealisticMass + num) / Time.fixedDeltaTime;
		float num2 = Mathf.InverseLerp(this.forceForAnyDamage, this.forceForMaxDamage, value);
		if (num2 > 0f)
		{
			this.pendingDamage = Mathf.Max(this.pendingDamage, Mathf.Lerp(this.minDamage, this.maxDamage, num2));
			if (this.pendingDamage > this.entity.Health())
			{
				TakeCollisionDamage.ICanRestoreVelocity canRestoreVelocity = collision.gameObject.ToBaseEntity() as TakeCollisionDamage.ICanRestoreVelocity;
				if (canRestoreVelocity != null)
				{
					canRestoreVelocity.RestoreVelocity(collision.relativeVelocity * this.velocityRestorePercent);
				}
			}
			base.Invoke(new Action(this.DoDamage), 0f);
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000F2DED File Offset: 0x000F0FED
	protected void OnDestroy()
	{
		base.CancelInvoke(new Action(this.DoDamage));
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000F2E04 File Offset: 0x000F1004
	private void DoDamage()
	{
		if (this.entity == null || this.entity.IsDead() || this.entity.IsDestroyed)
		{
			return;
		}
		if (this.pendingDamage > 0f)
		{
			this.entity.Hurt(this.pendingDamage, DamageType.Collision, null, false);
			this.pendingDamage = 0f;
		}
	}

	// Token: 0x02000D07 RID: 3335
	public interface ICanRestoreVelocity
	{
		// Token: 0x06005017 RID: 20503
		void RestoreVelocity(Vector3 amount);
	}
}
