using System;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class VehicleTerrainHandler
{
	// Token: 0x0400201E RID: 8222
	public string CurGroundPhysicsMatName;

	// Token: 0x0400201F RID: 8223
	public VehicleTerrainHandler.Surface OnSurface;

	// Token: 0x04002020 RID: 8224
	public bool IsGrounded;

	// Token: 0x04002021 RID: 8225
	public float RayLength = 1.5f;

	// Token: 0x04002022 RID: 8226
	private readonly string[] TerrainRoad = new string[]
	{
		"rock",
		"concrete",
		"gravel",
		"metal",
		"path"
	};

	// Token: 0x04002023 RID: 8227
	private const float SECONDS_BETWEEN_TERRAIN_SAMPLE = 0.25f;

	// Token: 0x04002024 RID: 8228
	private TimeSince timeSinceTerrainCheck;

	// Token: 0x04002025 RID: 8229
	private readonly BaseVehicle vehicle;

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x060027A1 RID: 10145 RVA: 0x000F7068 File Offset: 0x000F5268
	public bool IsOnSnowOrIce
	{
		get
		{
			return this.OnSurface == VehicleTerrainHandler.Surface.Snow || this.OnSurface == VehicleTerrainHandler.Surface.Ice;
		}
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000F7080 File Offset: 0x000F5280
	public VehicleTerrainHandler(BaseVehicle vehicle)
	{
		this.vehicle = vehicle;
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000F70D9 File Offset: 0x000F52D9
	public void FixedUpdate()
	{
		if (!this.vehicle.IsStationary() && this.timeSinceTerrainCheck > 0.25f)
		{
			this.DoTerrainCheck();
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000F7100 File Offset: 0x000F5300
	private void DoTerrainCheck()
	{
		this.timeSinceTerrainCheck = UnityEngine.Random.Range(-0.025f, 0.025f);
		Transform transform = this.vehicle.transform;
		RaycastHit raycastHit;
		if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out raycastHit, this.RayLength, 27328513, QueryTriggerInteraction.Ignore))
		{
			this.CurGroundPhysicsMatName = raycastHit.collider.GetMaterialAt(raycastHit.point).GetNameLower();
			if (this.GetOnRoad(this.CurGroundPhysicsMatName))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Road;
			}
			else if (this.CurGroundPhysicsMatName == "snow")
			{
				if (raycastHit.collider.CompareTag("TreatSnowAsIce"))
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Ice;
				}
				else
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Snow;
				}
			}
			else if (this.CurGroundPhysicsMatName == "sand")
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Sand;
			}
			else if (this.CurGroundPhysicsMatName.Contains("zero friction"))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Frictionless;
			}
			else
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Default;
			}
			this.IsGrounded = true;
			return;
		}
		this.CurGroundPhysicsMatName = "concrete";
		this.OnSurface = VehicleTerrainHandler.Surface.Default;
		this.IsGrounded = false;
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000F723C File Offset: 0x000F543C
	private bool GetOnRoad(string physicMat)
	{
		for (int i = 0; i < this.TerrainRoad.Length; i++)
		{
			if (this.TerrainRoad[i] == physicMat)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x02000D19 RID: 3353
	public enum Surface
	{
		// Token: 0x0400462A RID: 17962
		Default,
		// Token: 0x0400462B RID: 17963
		Road,
		// Token: 0x0400462C RID: 17964
		Snow,
		// Token: 0x0400462D RID: 17965
		Ice,
		// Token: 0x0400462E RID: 17966
		Sand,
		// Token: 0x0400462F RID: 17967
		Frictionless
	}
}
