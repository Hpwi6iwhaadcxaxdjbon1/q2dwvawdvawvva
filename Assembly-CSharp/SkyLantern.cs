using System;
using Rust;
using UnityEngine;

// Token: 0x02000143 RID: 323
public class SkyLantern : StorageContainer, IIgniteable
{
	// Token: 0x04000F59 RID: 3929
	public float gravityScale = -0.1f;

	// Token: 0x04000F5A RID: 3930
	public float travelSpeed = 2f;

	// Token: 0x04000F5B RID: 3931
	public float collisionRadius = 0.5f;

	// Token: 0x04000F5C RID: 3932
	public float rotationSpeed = 5f;

	// Token: 0x04000F5D RID: 3933
	public float randOffset = 1f;

	// Token: 0x04000F5E RID: 3934
	public float lifeTime = 120f;

	// Token: 0x04000F5F RID: 3935
	public float hoverHeight = 14f;

	// Token: 0x04000F60 RID: 3936
	public Transform collisionCheckPoint;

	// Token: 0x04000F61 RID: 3937
	private float idealAltitude;

	// Token: 0x04000F62 RID: 3938
	private Vector3 travelVec = Vector3.forward;

	// Token: 0x060016F2 RID: 5874 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000AF9DC File Offset: 0x000ADBDC
	public override void ServerInit()
	{
		base.ServerInit();
		this.randOffset = ((UnityEngine.Random.Range(0.5f, 1f) * (float)UnityEngine.Random.Range(0, 2) == 1f) ? -1f : 1f);
		this.travelVec = (Vector3.forward + Vector3.right * this.randOffset).normalized;
		base.Invoke(new Action(this.StartSinking), this.lifeTime - 15f);
		base.Invoke(new Action(this.SelfDestroy), this.lifeTime);
		this.travelSpeed = UnityEngine.Random.Range(1.75f, 2.25f);
		this.gravityScale *= UnityEngine.Random.Range(1f, 1.25f);
		base.InvokeRepeating(new Action(this.UpdateIdealAltitude), 0f, 1f);
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x000AFACC File Offset: 0x000ADCCC
	public void Ignite(Vector3 fromPos)
	{
		base.gameObject.transform.RemoveComponent<GroundWatch>();
		base.gameObject.transform.RemoveComponent<DestroyOnGroundMissing>();
		base.gameObject.layer = 14;
		this.travelVec = Vector3Ex.Direction2D(base.transform.position, fromPos);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateIdealAltitude();
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x000AFB30 File Offset: 0x000ADD30
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isServer)
		{
			return;
		}
		if (info.damageTypes.Has(DamageType.Heat) && this.CanIgnite())
		{
			this.Ignite(info.PointStart);
			return;
		}
		if (base.IsOn() && !base.IsBroken())
		{
			this.StartSinking();
		}
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x00003384 File Offset: 0x00001584
	public void SelfDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x000AFB86 File Offset: 0x000ADD86
	public bool CanIgnite()
	{
		return !base.IsOn() && !base.IsBroken();
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x000AFB9C File Offset: 0x000ADD9C
	public void UpdateIdealAltitude()
	{
		if (!base.IsOn())
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float a = (heightMap != null) ? heightMap.GetHeight(base.transform.position) : 0f;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		float b = (waterMap != null) ? waterMap.GetHeight(base.transform.position) : 0f;
		this.idealAltitude = Mathf.Max(a, b) + this.hoverHeight;
		if (this.hoverHeight != 0f)
		{
			this.idealAltitude -= 2f * Mathf.Abs(this.randOffset);
		}
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x000AFC33 File Offset: 0x000ADE33
	public void StartSinking()
	{
		if (base.IsBroken())
		{
			return;
		}
		this.hoverHeight = 0f;
		this.travelVec = Vector3.zero;
		this.UpdateIdealAltitude();
		base.SetFlag(BaseEntity.Flags.Broken, true, false, true);
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x000AFC68 File Offset: 0x000ADE68
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (!base.IsOn())
		{
			return;
		}
		float value = Mathf.Abs(base.transform.position.y - this.idealAltitude);
		float num = (base.transform.position.y < this.idealAltitude) ? -1f : 1f;
		float d = Mathf.InverseLerp(0f, 10f, value) * num;
		if (base.IsBroken())
		{
			this.travelVec = Vector3.Lerp(this.travelVec, Vector3.zero, Time.fixedDeltaTime * 0.5f);
			d = 0.7f;
		}
		Vector3 a = Vector3.zero;
		a = Vector3.up * this.gravityScale * Physics.gravity.y * d;
		a += this.travelVec * this.travelSpeed;
		Vector3 vector = base.transform.position + a * Time.fixedDeltaTime;
		Vector3 direction = Vector3Ex.Direction(vector, base.transform.position);
		float maxDistance = Vector3.Distance(vector, base.transform.position);
		RaycastHit raycastHit;
		if (!Physics.SphereCast(this.collisionCheckPoint.position, this.collisionRadius, direction, out raycastHit, maxDistance, 1218519297))
		{
			base.transform.position = vector;
			base.transform.Rotate(Vector3.up, this.rotationSpeed * this.randOffset * Time.deltaTime, Space.Self);
			return;
		}
		this.StartSinking();
	}
}
