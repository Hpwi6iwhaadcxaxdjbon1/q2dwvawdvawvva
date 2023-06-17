using System;
using Rust;
using Rust.Modular;
using UnityEngine;

// Token: 0x020004A1 RID: 1185
public class VehicleModuleEngine : VehicleModuleStorage
{
	// Token: 0x04001F06 RID: 7942
	[SerializeField]
	private VehicleModuleEngine.Engine engine;

	// Token: 0x04001F0C RID: 7948
	private const float FORCE_MULTIPLIER = 12.75f;

	// Token: 0x04001F0D RID: 7949
	private const float HEALTH_PERFORMANCE_FRACTION = 0.25f;

	// Token: 0x04001F0E RID: 7950
	private const float LOW_PERFORMANCE_THRESHOLD = 0.5f;

	// Token: 0x04001F0F RID: 7951
	private Sound badPerformanceLoop;

	// Token: 0x04001F10 RID: 7952
	private SoundModulation.Modulator badPerformancePitchModulator;

	// Token: 0x04001F11 RID: 7953
	private float prevSmokePercent;

	// Token: 0x04001F12 RID: 7954
	private const float MIN_FORCE_BIAS = 0.0002f;

	// Token: 0x04001F13 RID: 7955
	private const float MAX_FORCE_BIAS = 0.7f;

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x060026C6 RID: 9926 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool HasAnEngine
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x060026C7 RID: 9927 RVA: 0x000F290E File Offset: 0x000F0B0E
	// (set) Token: 0x060026C8 RID: 9928 RVA: 0x000F2916 File Offset: 0x000F0B16
	public bool IsUsable { get; private set; }

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x060026C9 RID: 9929 RVA: 0x000F291F File Offset: 0x000F0B1F
	// (set) Token: 0x060026CA RID: 9930 RVA: 0x000F2927 File Offset: 0x000F0B27
	public float PerformanceFractionAcceleration { get; private set; }

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x060026CB RID: 9931 RVA: 0x000F2930 File Offset: 0x000F0B30
	// (set) Token: 0x060026CC RID: 9932 RVA: 0x000F2938 File Offset: 0x000F0B38
	public float PerformanceFractionTopSpeed { get; private set; }

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x060026CD RID: 9933 RVA: 0x000F2941 File Offset: 0x000F0B41
	// (set) Token: 0x060026CE RID: 9934 RVA: 0x000F2949 File Offset: 0x000F0B49
	public float PerformanceFractionFuelEconomy { get; private set; }

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x060026CF RID: 9935 RVA: 0x000F2952 File Offset: 0x000F0B52
	// (set) Token: 0x060026D0 RID: 9936 RVA: 0x000F295A File Offset: 0x000F0B5A
	public float OverallPerformanceFraction { get; private set; }

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x060026D1 RID: 9937 RVA: 0x000F2963 File Offset: 0x000F0B63
	public bool AtLowPerformance
	{
		get
		{
			return this.OverallPerformanceFraction <= 0.5f;
		}
	}

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x060026D2 RID: 9938 RVA: 0x000F2975 File Offset: 0x000F0B75
	public bool AtPeakPerformance
	{
		get
		{
			return Mathf.Approximately(this.OverallPerformanceFraction, 1f);
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x060026D3 RID: 9939 RVA: 0x000F2987 File Offset: 0x000F0B87
	public int KW
	{
		get
		{
			return this.engine.engineKW;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x060026D4 RID: 9940 RVA: 0x000F2994 File Offset: 0x000F0B94
	public EngineAudioSet AudioSet
	{
		get
		{
			return this.engine.audioSet;
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x060026D5 RID: 9941 RVA: 0x000F29A1 File Offset: 0x000F0BA1
	private bool EngineIsOn
	{
		get
		{
			return base.Car != null && base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On;
		}
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000F29C1 File Offset: 0x000F0BC1
	public override void InitShared()
	{
		base.InitShared();
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x000F29DA File Offset: 0x000F0BDA
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x000F29F5 File Offset: 0x000F0BF5
	public override float GetMaxDriveForce()
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		return (float)this.engine.engineKW * 12.75f * this.PerformanceFractionTopSpeed;
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000F2A20 File Offset: 0x000F0C20
	public void RefreshPerformanceStats(EngineStorage engineStorage)
	{
		if (engineStorage == null)
		{
			this.IsUsable = false;
			this.PerformanceFractionAcceleration = 0f;
			this.PerformanceFractionTopSpeed = 0f;
			this.PerformanceFractionFuelEconomy = 0f;
		}
		else
		{
			this.IsUsable = engineStorage.isUsable;
			this.PerformanceFractionAcceleration = this.GetPerformanceFraction(engineStorage.accelerationBoostPercent);
			this.PerformanceFractionTopSpeed = this.GetPerformanceFraction(engineStorage.topSpeedBoostPercent);
			this.PerformanceFractionFuelEconomy = this.GetPerformanceFraction(engineStorage.fuelEconomyBoostPercent);
		}
		this.OverallPerformanceFraction = (this.PerformanceFractionAcceleration + this.PerformanceFractionTopSpeed + this.PerformanceFractionFuelEconomy) / 3f;
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000F2AC4 File Offset: 0x000F0CC4
	private float GetPerformanceFraction(float statBoostPercent)
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		float num = Mathf.Lerp(0f, 0.25f, base.healthFraction);
		float num2;
		if (base.healthFraction == 0f)
		{
			num2 = 0f;
		}
		else
		{
			num2 = statBoostPercent * 0.75f;
		}
		return num + num2;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000F2B13 File Offset: 0x000F0D13
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000F2B2D File Offset: 0x000F0D2D
	public override bool CanBeLooted(BasePlayer player)
	{
		return base.CanBeLooted(player);
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x000F2B3C File Offset: 0x000F0D3C
	public override void VehicleFixedUpdate()
	{
		if (!this.isSpawned || !base.IsOnAVehicle)
		{
			return;
		}
		base.VehicleFixedUpdate();
		if (!base.Vehicle.IsMovingOrOn || base.Car == null)
		{
			return;
		}
		if (base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On && this.IsUsable)
		{
			float num = Mathf.Lerp(this.engine.idleFuelPerSec, this.engine.maxFuelPerSec, Mathf.Abs(base.Car.GetThrottleInput()));
			num /= this.PerformanceFractionFuelEconomy;
			base.Car.TickFuel(num);
		}
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x000F2BD4 File Offset: 0x000F0DD4
	public override float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float bias = Mathf.Lerp(0.0002f, 0.7f, this.PerformanceFractionAcceleration);
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, bias);
		return maxDriveForce * num;
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000F2C10 File Offset: 0x000F0E10
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (info.damageTypes.GetMajorityDamageType() == DamageType.Decay)
		{
			return;
		}
		float num = info.damageTypes.Total();
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		if (engineStorage != null && num > 0f)
		{
			engineStorage.OnModuleDamaged(num);
		}
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000F2C64 File Offset: 0x000F0E64
	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000F2C88 File Offset: 0x000F0E88
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		engineStorage.AdminAddParts(tier);
		this.RefreshPerformanceStats(engineStorage);
		return true;
	}

	// Token: 0x02000D06 RID: 3334
	[Serializable]
	public class Engine
	{
		// Token: 0x040045EA RID: 17898
		[Header("Engine Stats")]
		public int engineKW;

		// Token: 0x040045EB RID: 17899
		public float idleFuelPerSec = 0.25f;

		// Token: 0x040045EC RID: 17900
		public float maxFuelPerSec = 0.25f;

		// Token: 0x040045ED RID: 17901
		[Header("Engine Audio")]
		public EngineAudioSet audioSet;

		// Token: 0x040045EE RID: 17902
		[Header("Engine FX")]
		public ParticleSystemContainer[] engineParticles;

		// Token: 0x040045EF RID: 17903
		public ParticleSystem[] exhaustSmoke;

		// Token: 0x040045F0 RID: 17904
		public ParticleSystem[] exhaustBackfire;

		// Token: 0x040045F1 RID: 17905
		public float exhaustSmokeMinOpacity = 0.1f;

		// Token: 0x040045F2 RID: 17906
		public float exhaustSmokeMaxOpacity = 0.7f;

		// Token: 0x040045F3 RID: 17907
		public float exhaustSmokeChangeRate = 0.5f;
	}
}
