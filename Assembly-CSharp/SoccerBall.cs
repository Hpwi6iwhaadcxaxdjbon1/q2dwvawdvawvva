using System;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class SoccerBall : BaseCombatEntity
{
	// Token: 0x04001F3E RID: 7998
	[Header("Soccer Ball")]
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04001F3F RID: 7999
	[SerializeField]
	private float additionalForceMultiplier = 0.2f;

	// Token: 0x04001F40 RID: 8000
	[SerializeField]
	private float upForceMultiplier = 0.15f;

	// Token: 0x04001F41 RID: 8001
	[SerializeField]
	private DamageRenderer damageRenderer;

	// Token: 0x04001F42 RID: 8002
	[SerializeField]
	private float explosionForceMultiplier = 40f;

	// Token: 0x04001F43 RID: 8003
	[SerializeField]
	private float otherForceMultiplier = 10f;

	// Token: 0x060026ED RID: 9965 RVA: 0x000F2EE4 File Offset: 0x000F10E4
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (collision.impulse.magnitude > 0f && collision.collider.attachedRigidbody != null && !collision.collider.attachedRigidbody.HasComponent<SoccerBall>())
		{
			Vector3 a = this.rigidBody.position - collision.collider.attachedRigidbody.position;
			float magnitude = collision.impulse.magnitude;
			this.rigidBody.AddForce(a * magnitude * this.additionalForceMultiplier + Vector3.up * magnitude * this.upForceMultiplier, ForceMode.Impulse);
		}
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000F2FA0 File Offset: 0x000F11A0
	public override void Hurt(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		float num = 0f;
		foreach (float num2 in info.damageTypes.types)
		{
			if ((int)num2 == 16 || (int)num2 == 22)
			{
				num += num2 * this.explosionForceMultiplier;
			}
			else
			{
				num += num2 * this.otherForceMultiplier;
			}
		}
		if (num > 3f)
		{
			this.rigidBody.AddExplosionForce(num, info.HitPositionWorld, 0.25f, 0.5f);
		}
		base.Hurt(info);
	}
}
