using System;
using UnityEngine;

// Token: 0x02000460 RID: 1120
public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x04001D52 RID: 7506
	public Vector3 initialVelocity;

	// Token: 0x04001D53 RID: 7507
	public float drag;

	// Token: 0x04001D54 RID: 7508
	public float gravityModifier = 1f;

	// Token: 0x04001D55 RID: 7509
	public float speed = 15f;

	// Token: 0x04001D56 RID: 7510
	public float scanRange;

	// Token: 0x04001D57 RID: 7511
	public Vector3 swimScale;

	// Token: 0x04001D58 RID: 7512
	public Vector3 swimSpeed;

	// Token: 0x04001D59 RID: 7513
	public float radius;

	// Token: 0x04001D5A RID: 7514
	private bool impacted;

	// Token: 0x04001D5B RID: 7515
	private float swimRandom;

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x060024F5 RID: 9461 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool HasRangeLimit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x000E9CD4 File Offset: 0x000E7ED4
	public float GetMaxRange(float maxFuseTime)
	{
		if (this.gravityModifier == 0f)
		{
			return float.PositiveInfinity;
		}
		float a = Mathf.Sin(1.5707964f) * this.speed * this.speed / -(Physics.gravity.y * this.gravityModifier);
		float b = this.speed * maxFuseTime;
		return Mathf.Min(a, b);
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x060024F7 RID: 9463 RVA: 0x000E9D2E File Offset: 0x000E7F2E
	protected virtual int mask
	{
		get
		{
			return 1236478737;
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x060024F8 RID: 9464 RVA: 0x000E9D35 File Offset: 0x000E7F35
	// (set) Token: 0x060024F9 RID: 9465 RVA: 0x000E9D3D File Offset: 0x000E7F3D
	public Vector3 CurrentVelocity { get; protected set; }

	// Token: 0x060024FA RID: 9466 RVA: 0x000E9D46 File Offset: 0x000E7F46
	protected void FixedUpdate()
	{
		if (base.baseEntity != null && base.baseEntity.isServer)
		{
			this.DoMovement();
		}
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000E9D6C File Offset: 0x000E7F6C
	public virtual bool DoMovement()
	{
		if (this.impacted)
		{
			return false;
		}
		this.CurrentVelocity += Physics.gravity * this.gravityModifier * Time.fixedDeltaTime * Time.timeScale;
		Vector3 a = this.CurrentVelocity;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float num = Time.time + this.swimRandom;
			Vector3 vector = new Vector3(Mathf.Sin(num * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(num * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(num * this.swimSpeed.z) * this.swimScale.z);
			vector = base.transform.InverseTransformDirection(vector);
			a += vector;
		}
		float num2 = a.magnitude * Time.fixedDeltaTime;
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		if (GamePhysics.Trace(new Ray(position, a.normalized), this.radius, out raycastHit, num2 + this.scanRange, this.mask, QueryTriggerInteraction.Ignore, null))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (this.IsAValidHit(entity))
			{
				base.transform.position += base.transform.forward * Mathf.Max(0f, raycastHit.distance - 0.1f);
				ServerProjectile.IProjectileImpact component = base.GetComponent<ServerProjectile.IProjectileImpact>();
				if (component != null)
				{
					component.ProjectileImpact(raycastHit, position);
				}
				this.impacted = true;
				return false;
			}
		}
		base.transform.position += base.transform.forward * num2;
		base.transform.rotation = Quaternion.LookRotation(a.normalized);
		return true;
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x000E9F74 File Offset: 0x000E8174
	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		return !hitEnt.IsValid() || !base.baseEntity.creatorEntity.IsValid() || hitEnt.net.ID != base.baseEntity.creatorEntity.net.ID;
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000E9FC2 File Offset: 0x000E81C2
	public virtual void InitializeVelocity(Vector3 overrideVel)
	{
		base.transform.rotation = Quaternion.LookRotation(overrideVel.normalized);
		this.initialVelocity = overrideVel;
		this.CurrentVelocity = overrideVel;
	}

	// Token: 0x02000CED RID: 3309
	public interface IProjectileImpact
	{
		// Token: 0x06004FD3 RID: 20435
		void ProjectileImpact(RaycastHit hitInfo, Vector3 rayOrigin);
	}
}
