using System;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class VehicleEngineController<TOwner> where TOwner : BaseVehicle, IEngineControllerUser
{
	// Token: 0x04002019 RID: 8217
	private readonly TOwner owner;

	// Token: 0x0400201A RID: 8218
	private readonly bool isServer;

	// Token: 0x0400201B RID: 8219
	private readonly float engineStartupTime;

	// Token: 0x0400201C RID: 8220
	private readonly Transform waterloggedPoint;

	// Token: 0x0400201D RID: 8221
	private readonly BaseEntity.Flags engineStartingFlag;

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06002790 RID: 10128 RVA: 0x000F6D53 File Offset: 0x000F4F53
	public VehicleEngineController<TOwner>.EngineState CurEngineState
	{
		get
		{
			if (this.owner.HasFlag(this.engineStartingFlag))
			{
				return VehicleEngineController<TOwner>.EngineState.Starting;
			}
			if (this.owner.HasFlag(BaseEntity.Flags.On))
			{
				return VehicleEngineController<TOwner>.EngineState.On;
			}
			return VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06002791 RID: 10129 RVA: 0x000F6D85 File Offset: 0x000F4F85
	public bool IsOn
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.On;
		}
	}

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06002792 RID: 10130 RVA: 0x000F6D90 File Offset: 0x000F4F90
	public bool IsOff
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06002793 RID: 10131 RVA: 0x000F6D9B File Offset: 0x000F4F9B
	public bool IsStarting
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Starting;
		}
	}

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x06002794 RID: 10132 RVA: 0x000F6DA6 File Offset: 0x000F4FA6
	public bool IsStartingOrOn
	{
		get
		{
			return this.CurEngineState > VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x06002795 RID: 10133 RVA: 0x000F6DB1 File Offset: 0x000F4FB1
	// (set) Token: 0x06002796 RID: 10134 RVA: 0x000F6DB9 File Offset: 0x000F4FB9
	public EntityFuelSystem FuelSystem { get; private set; }

	// Token: 0x06002797 RID: 10135 RVA: 0x000F6DC4 File Offset: 0x000F4FC4
	public VehicleEngineController(TOwner owner, bool isServer, float engineStartupTime, GameObjectRef fuelStoragePrefab, Transform waterloggedPoint = null, BaseEntity.Flags engineStartingFlag = BaseEntity.Flags.Reserved1)
	{
		this.FuelSystem = new EntityFuelSystem(isServer, fuelStoragePrefab, owner.children, true);
		this.owner = owner;
		this.isServer = isServer;
		this.engineStartupTime = engineStartupTime;
		this.waterloggedPoint = waterloggedPoint;
		this.engineStartingFlag = engineStartingFlag;
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000F6E16 File Offset: 0x000F5016
	public VehicleEngineController<TOwner>.EngineState EngineStateFrom(BaseEntity.Flags flags)
	{
		if (flags.HasFlag(this.engineStartingFlag))
		{
			return VehicleEngineController<TOwner>.EngineState.Starting;
		}
		if (flags.HasFlag(BaseEntity.Flags.On))
		{
			return VehicleEngineController<TOwner>.EngineState.On;
		}
		return VehicleEngineController<TOwner>.EngineState.Off;
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000F6E48 File Offset: 0x000F5048
	public void TryStartEngine(BasePlayer player)
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsStartingOrOn)
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (!this.CanRunEngine())
		{
			this.owner.OnEngineStartFailed();
			return;
		}
		this.owner.SetFlag(this.engineStartingFlag, true, false, true);
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.Invoke(new Action(this.FinishStartingEngine), this.engineStartupTime);
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000F6EEC File Offset: 0x000F50EC
	public void FinishStartingEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsOn)
		{
			return;
		}
		this.owner.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000F6F4C File Offset: 0x000F514C
	public void StopEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.IsOff)
		{
			return;
		}
		this.CancelEngineStart();
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000F6F9E File Offset: 0x000F519E
	public void CheckEngineState()
	{
		if (this.IsStartingOrOn && !this.CanRunEngine())
		{
			this.StopEngine();
		}
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000F6FB6 File Offset: 0x000F51B6
	public bool CanRunEngine()
	{
		return this.owner.MeetsEngineRequirements() && this.FuelSystem.HasFuel(false) && !this.IsWaterlogged() && !this.owner.IsDead();
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000F6FF5 File Offset: 0x000F51F5
	public bool IsWaterlogged()
	{
		return this.waterloggedPoint != null && WaterLevel.Test(this.waterloggedPoint.position, true, this.owner);
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000F7023 File Offset: 0x000F5223
	public int TickFuel(float fuelPerSecond)
	{
		if (this.IsOn)
		{
			return this.FuelSystem.TryUseFuel(Time.fixedDeltaTime, fuelPerSecond);
		}
		return 0;
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000F7040 File Offset: 0x000F5240
	private void CancelEngineStart()
	{
		if (this.CurEngineState != VehicleEngineController<TOwner>.EngineState.Starting)
		{
			return;
		}
		this.owner.CancelInvoke(new Action(this.FinishStartingEngine));
	}

	// Token: 0x02000D18 RID: 3352
	public enum EngineState
	{
		// Token: 0x04004626 RID: 17958
		Off,
		// Token: 0x04004627 RID: 17959
		Starting,
		// Token: 0x04004628 RID: 17960
		On
	}
}
