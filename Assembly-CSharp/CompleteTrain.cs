using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class CompleteTrain : IDisposable
{
	// Token: 0x04001F6C RID: 8044
	private Vector3 unloaderPos;

	// Token: 0x04001F6D RID: 8045
	private float trackSpeed;

	// Token: 0x04001F6E RID: 8046
	private float prevTrackSpeed;

	// Token: 0x04001F6F RID: 8047
	private List<TrainCar> trainCars;

	// Token: 0x04001F70 RID: 8048
	private TriggerTrainCollisions frontCollisionTrigger;

	// Token: 0x04001F71 RID: 8049
	private TriggerTrainCollisions rearCollisionTrigger;

	// Token: 0x04001F72 RID: 8050
	private bool ranUpdateTick;

	// Token: 0x04001F73 RID: 8051
	private bool disposed;

	// Token: 0x04001F74 RID: 8052
	private const float IMPACT_ENERGY_FRACTION = 0.75f;

	// Token: 0x04001F75 RID: 8053
	private const float MIN_COLLISION_FORCE = 70000f;

	// Token: 0x04001F76 RID: 8054
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001F77 RID: 8055
	private const float SLEEP_SPEED = 0.1f;

	// Token: 0x04001F78 RID: 8056
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001F79 RID: 8057
	private TimeSince timeSinceLastChange;

	// Token: 0x04001F7A RID: 8058
	private bool isShunting;

	// Token: 0x04001F7B RID: 8059
	private TimeSince timeSinceShuntStart;

	// Token: 0x04001F7C RID: 8060
	private const float MAX_SHUNT_TIME = 20f;

	// Token: 0x04001F7D RID: 8061
	private const float SHUNT_SPEED = 4f;

	// Token: 0x04001F7E RID: 8062
	private const float SHUNT_SPEED_CHANGE_RATE = 10f;

	// Token: 0x04001F7F RID: 8063
	private Action<CoalingTower.ActionAttemptStatus> shuntEndCallback;

	// Token: 0x04001F80 RID: 8064
	private float shuntDistance;

	// Token: 0x04001F81 RID: 8065
	private Vector3 shuntDirection;

	// Token: 0x04001F82 RID: 8066
	private Vector2 shuntStartPos2D = Vector2.zero;

	// Token: 0x04001F83 RID: 8067
	private Vector2 shuntTargetPos2D = Vector2.zero;

	// Token: 0x04001F84 RID: 8068
	private TrainCar shuntTarget;

	// Token: 0x04001F85 RID: 8069
	private CompleteTrain.StaticCollisionState staticCollidingAtFront;

	// Token: 0x04001F86 RID: 8070
	private HashSet<GameObject> monitoredStaticContentF = new HashSet<GameObject>();

	// Token: 0x04001F87 RID: 8071
	private CompleteTrain.StaticCollisionState staticCollidingAtRear;

	// Token: 0x04001F88 RID: 8072
	private HashSet<GameObject> monitoredStaticContentR = new HashSet<GameObject>();

	// Token: 0x04001F89 RID: 8073
	private Dictionary<Rigidbody, float> prevTrackSpeeds = new Dictionary<Rigidbody, float>();

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x060026F2 RID: 9970 RVA: 0x000F3078 File Offset: 0x000F1278
	// (set) Token: 0x060026F3 RID: 9971 RVA: 0x000F3080 File Offset: 0x000F1280
	public TrainCar PrimaryTrainCar { get; private set; }

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x060026F4 RID: 9972 RVA: 0x000F3089 File Offset: 0x000F1289
	public bool TrainIsReversing
	{
		get
		{
			return this.PrimaryTrainCar != this.trainCars[0];
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x060026F5 RID: 9973 RVA: 0x000F30A2 File Offset: 0x000F12A2
	// (set) Token: 0x060026F6 RID: 9974 RVA: 0x000F30AA File Offset: 0x000F12AA
	public float TotalForces { get; private set; }

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x060026F7 RID: 9975 RVA: 0x000F30B3 File Offset: 0x000F12B3
	// (set) Token: 0x060026F8 RID: 9976 RVA: 0x000F30BB File Offset: 0x000F12BB
	public float TotalMass { get; private set; }

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x060026F9 RID: 9977 RVA: 0x000F30C4 File Offset: 0x000F12C4
	public int NumTrainCars
	{
		get
		{
			return this.trainCars.Count;
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x060026FA RID: 9978 RVA: 0x000F30D1 File Offset: 0x000F12D1
	// (set) Token: 0x060026FB RID: 9979 RVA: 0x000F30D9 File Offset: 0x000F12D9
	public int LinedUpToUnload { get; private set; } = -1;

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x060026FC RID: 9980 RVA: 0x000F30E2 File Offset: 0x000F12E2
	public bool IsLinedUpToUnload
	{
		get
		{
			return this.LinedUpToUnload >= 0;
		}
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000F30F0 File Offset: 0x000F12F0
	public CompleteTrain(TrainCar trainCar)
	{
		List<TrainCar> list = Facepunch.Pool.GetList<TrainCar>();
		list.Add(trainCar);
		this.Init(list);
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000F3160 File Offset: 0x000F1360
	public CompleteTrain(List<TrainCar> allTrainCars)
	{
		this.Init(allTrainCars);
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000F31C4 File Offset: 0x000F13C4
	private void Init(List<TrainCar> allTrainCars)
	{
		this.trainCars = allTrainCars;
		this.timeSinceLastChange = 0f;
		this.lastMovingTime = UnityEngine.Time.time;
		float num = 0f;
		this.PrimaryTrainCar = this.trainCars[0];
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.completeTrain != this)
			{
				if (trainCar.completeTrain != null)
				{
					bool flag = this.IsCoupledBackwards(i);
					bool preChangeCoupledBackwards = trainCar.coupling.PreChangeCoupledBackwards;
					float preChangeTrackSpeed = trainCar.coupling.PreChangeTrackSpeed;
					if (flag != preChangeCoupledBackwards)
					{
						num -= preChangeTrackSpeed;
					}
					else
					{
						num += preChangeTrackSpeed;
					}
				}
				trainCar.SetNewCompleteTrain(this);
			}
		}
		this.trackSpeed = num / (float)this.trainCars.Count;
		this.prevTrackSpeed = this.trackSpeed;
		this.ParamsTick();
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000F32A0 File Offset: 0x000F14A0
	~CompleteTrain()
	{
		this.Cleanup();
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000F32CC File Offset: 0x000F14CC
	public void Dispose()
	{
		this.Cleanup();
		System.GC.SuppressFinalize(this);
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000F32DA File Offset: 0x000F14DA
	private void Cleanup()
	{
		if (this.disposed)
		{
			return;
		}
		this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		this.disposed = true;
		Facepunch.Pool.FreeList<TrainCar>(ref this.trainCars);
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000F3300 File Offset: 0x000F1500
	public void RemoveTrainCar(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return;
		}
		if (this.trainCars.Count <= 1)
		{
			Debug.LogWarning(base.GetType().Name + ": Can't remove car from CompleteTrain of length one.");
			return;
		}
		int num = this.IndexOf(trainCar);
		bool flag;
		if (num == 0)
		{
			flag = this.IsCoupledBackwards(1);
		}
		else
		{
			flag = this.IsCoupledBackwards(0);
		}
		this.trainCars.RemoveAt(num);
		this.timeSinceLastChange = 0f;
		this.LinedUpToUnload = -1;
		if (this.IsCoupledBackwards(0) != flag)
		{
			this.trackSpeed *= -1f;
		}
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000F339C File Offset: 0x000F159C
	public float GetTrackSpeedFor(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return 0f;
		}
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.trackSpeed;
		}
		return this.trackSpeed;
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000F3400 File Offset: 0x000F1600
	public float GetPrevTrackSpeedFor(TrainCar trainCar)
	{
		if (this.trainCars.IndexOf(trainCar) < 0)
		{
			Debug.LogError(base.GetType().Name + ": Train car not found in the trainCars list.");
			return 0f;
		}
		if (this.IsCoupledBackwards(trainCar))
		{
			return -this.prevTrackSpeed;
		}
		return this.prevTrackSpeed;
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000F3454 File Offset: 0x000F1654
	public void UpdateTick(float dt)
	{
		if (this.ranUpdateTick || this.disposed)
		{
			return;
		}
		this.ranUpdateTick = true;
		if (this.IsAllAsleep() && !this.HasAnyEnginesOn() && !this.HasAnyCollisions() && !this.isShunting)
		{
			this.trackSpeed = 0f;
			return;
		}
		this.ParamsTick();
		this.MovementTick(dt);
		this.LinedUpToUnload = this.CheckLinedUpToUnload(out this.unloaderPos);
		if (this.disposed)
		{
			return;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			this.lastMovingTime = UnityEngine.Time.time;
		}
		if (!this.HasAnyEnginesOn() && !this.HasAnyCollisions() && UnityEngine.Time.time > this.lastMovingTime + 10f)
		{
			this.trackSpeed = 0f;
			this.SleepAll();
		}
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000F3524 File Offset: 0x000F1724
	public bool IncludesAnEngine()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == TrainCar.TrainCarType.Engine)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000F3588 File Offset: 0x000F1788
	protected bool HasAnyCollisions()
	{
		return this.frontCollisionTrigger.HasAnyContents || this.rearCollisionTrigger.HasAnyContents;
	}

	// Token: 0x06002709 RID: 9993 RVA: 0x000F35A4 File Offset: 0x000F17A4
	private bool HasAnyEnginesOn()
	{
		if (this.disposed)
		{
			return false;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.CarType == TrainCar.TrainCarType.Engine && trainCar.IsOn())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600270A RID: 9994 RVA: 0x000F3614 File Offset: 0x000F1814
	private bool IsAllAsleep()
	{
		if (this.disposed)
		{
			return true;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.rigidBody.IsSleeping())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600270B RID: 9995 RVA: 0x000F367C File Offset: 0x000F187C
	private void SleepAll()
	{
		if (this.disposed)
		{
			return;
		}
		foreach (TrainCar trainCar in this.trainCars)
		{
			trainCar.rigidBody.Sleep();
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000F36DC File Offset: 0x000F18DC
	public bool TryShuntCarTo(Vector3 shuntDirection, float shuntDistance, TrainCar shuntTarget, Action<CoalingTower.ActionAttemptStatus> shuntEndCallback, out CoalingTower.ActionAttemptStatus status)
	{
		if (this.disposed)
		{
			status = CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (this.isShunting)
		{
			status = CoalingTower.ActionAttemptStatus.AlreadyShunting;
			return false;
		}
		if (Mathf.Abs(this.trackSpeed) > 0.1f)
		{
			status = CoalingTower.ActionAttemptStatus.TrainIsMoving;
			return false;
		}
		if (this.HasThrottleInput())
		{
			status = CoalingTower.ActionAttemptStatus.TrainHasThrottle;
			return false;
		}
		this.shuntDirection = shuntDirection;
		this.shuntDistance = shuntDistance;
		this.shuntTarget = shuntTarget;
		this.timeSinceShuntStart = 0f;
		this.shuntStartPos2D.x = shuntTarget.transform.position.x;
		this.shuntStartPos2D.y = shuntTarget.transform.position.z;
		this.isShunting = true;
		this.shuntEndCallback = shuntEndCallback;
		status = CoalingTower.ActionAttemptStatus.NoError;
		return true;
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000F379A File Offset: 0x000F199A
	private void EndShunting(CoalingTower.ActionAttemptStatus status)
	{
		this.isShunting = false;
		if (this.shuntEndCallback != null)
		{
			this.shuntEndCallback(status);
			this.shuntEndCallback = null;
		}
		this.shuntTarget = null;
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000F37C5 File Offset: 0x000F19C5
	public bool ContainsOnly(TrainCar trainCar)
	{
		return !this.disposed && this.trainCars.Count == 1 && this.trainCars[0] == trainCar;
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000F37F3 File Offset: 0x000F19F3
	public int IndexOf(TrainCar trainCar)
	{
		if (this.disposed)
		{
			return -1;
		}
		return this.trainCars.IndexOf(trainCar);
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000F380C File Offset: 0x000F1A0C
	public bool TryGetAdjacentTrainCar(TrainCar trainCar, bool next, Vector3 forwardDir, out TrainCar result)
	{
		int num = this.trainCars.IndexOf(trainCar);
		Vector3 lhs;
		if (this.IsCoupledBackwards(num))
		{
			lhs = -trainCar.transform.forward;
		}
		else
		{
			lhs = trainCar.transform.forward;
		}
		if (Vector3.Dot(lhs, forwardDir) < 0f)
		{
			next = !next;
		}
		if (num >= 0)
		{
			if (next)
			{
				num++;
			}
			else
			{
				num--;
			}
			if (num >= 0 && num < this.trainCars.Count)
			{
				result = this.trainCars[num];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000F389C File Offset: 0x000F1A9C
	private void ParamsTick()
	{
		this.TotalForces = 0f;
		this.TotalMass = 0f;
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			if (trainCar.rigidBody.mass > num2)
			{
				num2 = trainCar.rigidBody.mass;
				num = i;
			}
		}
		bool flag = false;
		for (int j = 0; j < this.trainCars.Count; j++)
		{
			TrainCar trainCar2 = this.trainCars[j];
			float forces = trainCar2.GetForces();
			this.TotalForces += (this.IsCoupledBackwards(trainCar2) ? (-forces) : forces);
			flag |= trainCar2.HasThrottleInput();
			if (j == num)
			{
				this.TotalMass += trainCar2.rigidBody.mass;
			}
			else
			{
				this.TotalMass += trainCar2.rigidBody.mass * 0.4f;
			}
		}
		if (this.isShunting && flag)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.TrainHasThrottle);
		}
		if (this.trainCars.Count == 1)
		{
			this.frontCollisionTrigger = this.trainCars[0].FrontCollisionTrigger;
			this.rearCollisionTrigger = this.trainCars[0].RearCollisionTrigger;
			return;
		}
		this.frontCollisionTrigger = (this.trainCars[0].coupling.IsRearCoupled ? this.trainCars[0].FrontCollisionTrigger : this.trainCars[0].RearCollisionTrigger);
		this.rearCollisionTrigger = (this.trainCars[this.trainCars.Count - 1].coupling.IsRearCoupled ? this.trainCars[this.trainCars.Count - 1].FrontCollisionTrigger : this.trainCars[this.trainCars.Count - 1].RearCollisionTrigger);
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000F3AA0 File Offset: 0x000F1CA0
	private void MovementTick(float dt)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (!this.isShunting)
		{
			this.trackSpeed += this.TotalForces * dt / this.TotalMass;
		}
		else
		{
			bool flag = Vector3.Dot(this.shuntDirection, this.PrimaryTrainCar.transform.forward) >= 0f;
			if (this.IsCoupledBackwards(this.PrimaryTrainCar))
			{
				flag = !flag;
			}
			if (this.shuntTarget == null || this.shuntTarget.IsDead() || this.shuntTarget.IsDestroyed)
			{
				this.EndShunting(CoalingTower.ActionAttemptStatus.NoTrainCar);
			}
			else
			{
				float num = 4f;
				this.shuntTargetPos2D.x = this.shuntTarget.transform.position.x;
				this.shuntTargetPos2D.y = this.shuntTarget.transform.position.z;
				float num2 = this.shuntDistance - Vector3.Distance(this.shuntStartPos2D, this.shuntTargetPos2D);
				if (num2 < 2f)
				{
					float t = Mathf.InverseLerp(0f, 2f, num2);
					num *= Mathf.Lerp(0.1f, 1f, t);
				}
				this.trackSpeed = Mathf.MoveTowards(this.trackSpeed, flag ? num : (-num), dt * 10f);
				if (this.timeSinceShuntStart > 20f || num2 <= 0f)
				{
					this.EndShunting(CoalingTower.ActionAttemptStatus.NoError);
					this.trackSpeed = 0f;
				}
			}
		}
		float num3 = this.trainCars[0].rigidBody.drag;
		if (this.IsLinedUpToUnload)
		{
			float num4 = Mathf.Abs(this.trackSpeed);
			if (num4 > 1f)
			{
				TrainCarUnloadable trainCarUnloadable = this.trainCars[this.LinedUpToUnload] as TrainCarUnloadable;
				if (trainCarUnloadable != null)
				{
					float value = trainCarUnloadable.MinDistToUnloadingArea(this.unloaderPos);
					float num5 = Mathf.InverseLerp(2f, 0f, value);
					if (num4 < 2f)
					{
						float num6 = (num4 - 1f) / 1f;
						num5 *= num6;
					}
					num3 = Mathf.Lerp(num3, 3.5f, num5);
				}
			}
		}
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed -= num3 * 4f * dt;
			if (this.trackSpeed < 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		else if (this.trackSpeed < 0f)
		{
			this.trackSpeed += num3 * 4f * dt;
			if (this.trackSpeed > 0f)
			{
				this.trackSpeed = 0f;
			}
		}
		float num7 = this.trackSpeed;
		this.trackSpeed = this.ApplyCollisionsToTrackSpeed(this.trackSpeed, this.TotalMass, dt);
		if (this.isShunting && this.trackSpeed != num7)
		{
			this.EndShunting(CoalingTower.ActionAttemptStatus.GenericError);
		}
		if (this.disposed)
		{
			return;
		}
		this.trackSpeed = Mathf.Clamp(this.trackSpeed, -(TrainCar.TRAINCAR_MAX_SPEED - 1f), TrainCar.TRAINCAR_MAX_SPEED - 1f);
		if (this.trackSpeed > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.trackSpeed < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else if (this.TotalForces > 0f)
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		else if (this.TotalForces < 0f)
		{
			this.PrimaryTrainCar = this.trainCars[this.trainCars.Count - 1];
		}
		else
		{
			this.PrimaryTrainCar = this.trainCars[0];
		}
		if (this.trackSpeed != 0f || this.TotalForces != 0f)
		{
			this.PrimaryTrainCar.FrontTrainCarTick(this.GetTrackSelection(), dt);
			if (this.trainCars.Count > 1)
			{
				if (this.PrimaryTrainCar == this.trainCars[0])
				{
					for (int i = 1; i < this.trainCars.Count; i++)
					{
						this.MoveOtherTrainCar(this.trainCars[i], this.trainCars[i - 1]);
					}
					return;
				}
				for (int j = this.trainCars.Count - 2; j >= 0; j--)
				{
					this.MoveOtherTrainCar(this.trainCars[j], this.trainCars[j + 1]);
				}
			}
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000F3F48 File Offset: 0x000F2148
	private void MoveOtherTrainCar(TrainCar trainCar, TrainCar prevTrainCar)
	{
		TrainTrackSpline frontTrackSection = prevTrainCar.FrontTrackSection;
		float frontWheelSplineDist = prevTrainCar.FrontWheelSplineDist;
		float num = 0f;
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		TrainCoupling coupledTo2 = trainCar.coupling.rearCoupling.CoupledTo;
		if (coupledTo == prevTrainCar.coupling.frontCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.rearCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo == prevTrainCar.coupling.rearCoupling)
		{
			num += trainCar.DistFrontWheelToFrontCoupling;
			num += prevTrainCar.DistFrontWheelToBackCoupling;
		}
		else if (coupledTo2 == prevTrainCar.coupling.frontCoupling)
		{
			num -= trainCar.DistFrontWheelToBackCoupling;
			num -= prevTrainCar.DistFrontWheelToFrontCoupling;
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": Uncoupled!");
		}
		trainCar.OtherTrainCarTick(frontTrackSection, frontWheelSplineDist, -num);
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000F403D File Offset: 0x000F223D
	public void ResetUpdateTick()
	{
		this.ranUpdateTick = false;
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000F4048 File Offset: 0x000F2248
	public bool Matches(List<TrainCar> listToCompare)
	{
		if (this.disposed)
		{
			return false;
		}
		if (listToCompare.Count != this.trainCars.Count)
		{
			return false;
		}
		for (int i = 0; i < listToCompare.Count; i++)
		{
			if (this.trainCars[i] != listToCompare[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000F40A4 File Offset: 0x000F22A4
	public void ReduceSpeedBy(float velChange)
	{
		this.prevTrackSpeed = this.trackSpeed;
		if (this.trackSpeed > 0f)
		{
			this.trackSpeed = Mathf.Max(0f, this.trackSpeed - velChange);
			return;
		}
		if (this.trackSpeed < 0f)
		{
			this.trackSpeed = Mathf.Min(0f, this.trackSpeed + velChange);
		}
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000F4108 File Offset: 0x000F2308
	public bool AnyPlayersOnTrain()
	{
		if (this.disposed)
		{
			return false;
		}
		using (List<TrainCar>.Enumerator enumerator = this.trainCars.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.AnyPlayersOnTrainCar())
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000F416C File Offset: 0x000F236C
	private int CheckLinedUpToUnload(out Vector3 unloaderPos)
	{
		if (this.disposed)
		{
			unloaderPos = Vector3.zero;
			return -1;
		}
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			TrainCar trainCar = this.trainCars[i];
			bool flag;
			if (CoalingTower.IsUnderAnUnloader(trainCar, out flag, out unloaderPos))
			{
				trainCar.SetFlag(BaseEntity.Flags.Reserved4, flag, false, true);
				if (flag)
				{
					return i;
				}
			}
		}
		unloaderPos = Vector3.zero;
		return -1;
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000F41DB File Offset: 0x000F23DB
	public bool IsCoupledBackwards(TrainCar trainCar)
	{
		return !this.disposed && this.IsCoupledBackwards(this.trainCars.IndexOf(trainCar));
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000F41FC File Offset: 0x000F23FC
	private bool IsCoupledBackwards(int trainCarIndex)
	{
		if (this.disposed || this.trainCars.Count == 1 || trainCarIndex < 0 || trainCarIndex > this.trainCars.Count - 1)
		{
			return false;
		}
		TrainCar trainCar = this.trainCars[trainCarIndex];
		if (trainCarIndex == 0)
		{
			return trainCar.coupling.IsFrontCoupled;
		}
		TrainCoupling coupledTo = trainCar.coupling.frontCoupling.CoupledTo;
		return coupledTo == null || coupledTo.owner != this.trainCars[trainCarIndex - 1];
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000F4280 File Offset: 0x000F2480
	private bool HasThrottleInput()
	{
		for (int i = 0; i < this.trainCars.Count; i++)
		{
			if (this.trainCars[i].HasThrottleInput())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000F42BC File Offset: 0x000F24BC
	private TrainTrackSpline.TrackSelection GetTrackSelection()
	{
		TrainTrackSpline.TrackSelection result = TrainTrackSpline.TrackSelection.Default;
		foreach (TrainCar trainCar in this.trainCars)
		{
			if (trainCar.localTrackSelection != TrainTrackSpline.TrackSelection.Default)
			{
				if (this.IsCoupledBackwards(trainCar) != this.IsCoupledBackwards(this.PrimaryTrainCar))
				{
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Left)
					{
						return TrainTrackSpline.TrackSelection.Right;
					}
					if (trainCar.localTrackSelection == TrainTrackSpline.TrackSelection.Right)
					{
						return TrainTrackSpline.TrackSelection.Left;
					}
				}
				return trainCar.localTrackSelection;
			}
		}
		return result;
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000F4350 File Offset: 0x000F2550
	public void FreeStaticCollision()
	{
		this.staticCollidingAtFront = CompleteTrain.StaticCollisionState.Free;
		this.staticCollidingAtRear = CompleteTrain.StaticCollisionState.Free;
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x000F4360 File Offset: 0x000F2560
	private float ApplyCollisionsToTrackSpeed(float trackSpeed, float totalMass, float deltaTime)
	{
		TrainCar owner = this.frontCollisionTrigger.owner;
		Vector3 forwardVector = this.IsCoupledBackwards(owner) ? (-owner.transform.forward) : owner.transform.forward;
		trackSpeed = this.ApplyCollisions(trackSpeed, owner, forwardVector, true, this.frontCollisionTrigger, totalMass, ref this.staticCollidingAtFront, this.staticCollidingAtRear, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		owner = this.rearCollisionTrigger.owner;
		forwardVector = (this.IsCoupledBackwards(owner) ? (-owner.transform.forward) : owner.transform.forward);
		trackSpeed = this.ApplyCollisions(trackSpeed, owner, forwardVector, false, this.rearCollisionTrigger, totalMass, ref this.staticCollidingAtRear, this.staticCollidingAtFront, deltaTime);
		if (this.disposed)
		{
			return trackSpeed;
		}
		Rigidbody rigidbody = null;
		foreach (KeyValuePair<Rigidbody, float> keyValuePair in this.prevTrackSpeeds)
		{
			if (keyValuePair.Key == null || (!this.frontCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key) && !this.rearCollisionTrigger.otherRigidbodyContents.Contains(keyValuePair.Key)))
			{
				rigidbody = keyValuePair.Key;
				break;
			}
		}
		if (rigidbody != null)
		{
			this.prevTrackSpeeds.Remove(rigidbody);
		}
		return trackSpeed;
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000F44CC File Offset: 0x000F26CC
	private float ApplyCollisions(float trackSpeed, TrainCar ourTrainCar, Vector3 forwardVector, bool atOurFront, TriggerTrainCollisions trigger, float ourTotalMass, ref CompleteTrain.StaticCollisionState wasStaticColliding, CompleteTrain.StaticCollisionState otherEndStaticColliding, float deltaTime)
	{
		Vector3 b = forwardVector * trackSpeed;
		bool flag = trigger.HasAnyStaticContents;
		if (atOurFront && ourTrainCar.FrontAtEndOfLine)
		{
			flag = true;
		}
		else if (!atOurFront && ourTrainCar.RearAtEndOfLine)
		{
			flag = true;
		}
		float num = flag ? (b.magnitude * Mathf.Clamp(ourTotalMass, 1f, 13000f)) : 0f;
		trackSpeed = this.HandleStaticCollisions(flag, atOurFront, trackSpeed, ref wasStaticColliding, trigger);
		if (!flag && otherEndStaticColliding == CompleteTrain.StaticCollisionState.Free)
		{
			foreach (TrainCar trainCar in trigger.trainContents)
			{
				Vector3 a = trainCar.transform.forward * trainCar.GetPrevTrackSpeed();
				trackSpeed = this.HandleTrainCollision(atOurFront, forwardVector, trackSpeed, ourTrainCar.transform, trainCar, deltaTime, ref wasStaticColliding);
				num += Vector3.Magnitude(a - b) * Mathf.Clamp(trainCar.rigidBody.mass, 1f, 13000f);
			}
			foreach (Rigidbody rigidbody in trigger.otherRigidbodyContents)
			{
				trackSpeed = this.HandleRigidbodyCollision(atOurFront, trackSpeed, forwardVector, ourTotalMass, rigidbody, rigidbody.mass, deltaTime, true);
				num += Vector3.Magnitude(rigidbody.velocity - b) * Mathf.Clamp(rigidbody.mass, 1f, 13000f);
			}
		}
		if (num >= 70000f && this.timeSinceLastChange > 1f && trigger.owner.ApplyCollisionDamage(num) > 5f)
		{
			foreach (Collider collider in trigger.colliderContents)
			{
				Vector3 contactPoint = collider.ClosestPointOnBounds(trigger.owner.transform.position);
				trigger.owner.TryShowCollisionFX(contactPoint, trigger.owner.collisionEffect);
			}
		}
		return trackSpeed;
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000F470C File Offset: 0x000F290C
	private float HandleStaticCollisions(bool staticColliding, bool front, float trackSpeed, ref CompleteTrain.StaticCollisionState wasStaticColliding, TriggerTrainCollisions trigger = null)
	{
		float num = front ? -5f : 5f;
		if (staticColliding && (front ? (trackSpeed > num) : (trackSpeed < num)))
		{
			trackSpeed = num;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StaticColliding;
			HashSet<GameObject> hashSet = front ? this.monitoredStaticContentF : this.monitoredStaticContentR;
			hashSet.Clear();
			if (!(trigger != null))
			{
				return trackSpeed;
			}
			using (HashSet<GameObject>.Enumerator enumerator = trigger.staticContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject item = enumerator.Current;
					hashSet.Add(item);
				}
				return trackSpeed;
			}
		}
		if (wasStaticColliding == CompleteTrain.StaticCollisionState.StaticColliding)
		{
			trackSpeed = 0f;
			wasStaticColliding = CompleteTrain.StaticCollisionState.StayingStill;
		}
		else if (wasStaticColliding == CompleteTrain.StaticCollisionState.StayingStill)
		{
			bool flag = front ? (trackSpeed > 0.01f) : (trackSpeed < -0.01f);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = (front ? (trackSpeed < -0.01f) : (trackSpeed > 0.01f));
			}
			if (flag)
			{
				HashSet<GameObject> hashSet2 = front ? this.monitoredStaticContentF : this.monitoredStaticContentR;
				if (hashSet2.Count > 0)
				{
					bool flag3 = true;
					using (HashSet<GameObject>.Enumerator enumerator = hashSet2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current != null)
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				trackSpeed = 0f;
			}
			else if (flag2)
			{
				wasStaticColliding = CompleteTrain.StaticCollisionState.Free;
			}
		}
		else if (front)
		{
			this.monitoredStaticContentF.Clear();
		}
		else
		{
			this.monitoredStaticContentR.Clear();
		}
		return trackSpeed;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000F48AC File Offset: 0x000F2AAC
	private float HandleTrainCollision(bool front, Vector3 forwardVector, float trackSpeed, Transform ourTransform, TrainCar theirTrain, float deltaTime, ref CompleteTrain.StaticCollisionState wasStaticColliding)
	{
		Vector3 vector = front ? forwardVector : (-forwardVector);
		float num = Vector3.Angle(vector, theirTrain.transform.forward);
		float f = Vector3.Dot(vector, (theirTrain.transform.position - ourTransform.position).normalized);
		if ((num > 30f && num < 150f) || Mathf.Abs(f) < 0.975f)
		{
			trackSpeed = (front ? -0.5f : 0.5f);
		}
		else
		{
			List<CompleteTrain> list = Facepunch.Pool.GetList<CompleteTrain>();
			float totalPushingMass = this.GetTotalPushingMass(vector, forwardVector, ref list);
			if (totalPushingMass < 0f)
			{
				trackSpeed = this.HandleStaticCollisions(true, front, trackSpeed, ref wasStaticColliding, null);
			}
			else
			{
				trackSpeed = this.HandleRigidbodyCollision(front, trackSpeed, forwardVector, this.TotalMass, theirTrain.rigidBody, totalPushingMass, deltaTime, false);
			}
			list.Clear();
			float num2 = this.GetTotalPushingForces(vector, forwardVector, ref list);
			if (!front)
			{
				num2 *= -1f;
			}
			if ((front && num2 <= 0f) || (!front && num2 >= 0f))
			{
				trackSpeed += num2 / this.TotalMass * deltaTime;
			}
			Facepunch.Pool.FreeList<CompleteTrain>(ref list);
		}
		return trackSpeed;
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000F49D4 File Offset: 0x000F2BD4
	private float HandleRigidbodyCollision(bool atOurFront, float trackSpeed, Vector3 forwardVector, float ourTotalMass, Rigidbody theirRB, float theirTotalMass, float deltaTime, bool calcSecondaryForces)
	{
		float num = Vector3.Dot(forwardVector, theirRB.velocity);
		float num2 = trackSpeed - num;
		if ((atOurFront && num2 <= 0f) || (!atOurFront && num2 >= 0f))
		{
			return trackSpeed;
		}
		float num3 = num2 / deltaTime * theirTotalMass * 0.75f;
		if (calcSecondaryForces)
		{
			if (this.prevTrackSpeeds.ContainsKey(theirRB))
			{
				float num4 = num2 / deltaTime * ourTotalMass * 0.75f / theirTotalMass * deltaTime;
				float num5 = this.prevTrackSpeeds[theirRB] - num;
				num3 -= Mathf.Clamp((num5 - num4) * ourTotalMass, 0f, 1000000f);
				this.prevTrackSpeeds[theirRB] = num;
			}
			else if (num != 0f)
			{
				this.prevTrackSpeeds.Add(theirRB, num);
			}
		}
		float num6 = num3 / ourTotalMass * deltaTime;
		num6 = Mathf.Clamp(num6, -Mathf.Abs(num - trackSpeed) - 0.5f, Mathf.Abs(num - trackSpeed) + 0.5f);
		trackSpeed -= num6;
		return trackSpeed;
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000F4AC4 File Offset: 0x000F2CC4
	private float GetTotalPushingMass(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingMass: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalMass;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		if ((flag ? this.staticCollidingAtFront : this.staticCollidingAtRear) != CompleteTrain.StaticCollisionState.Free)
		{
			return -1f;
		}
		TriggerTrainCollisions triggerTrainCollisions = flag ? this.frontCollisionTrigger : this.rearCollisionTrigger;
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 ourForward2 = trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward;
				float totalPushingMass = trainCar.completeTrain.GetTotalPushingMass(pushDirection, ourForward2, ref prevTrains);
				if (totalPushingMass < 0f)
				{
					return -1f;
				}
				num += totalPushingMass;
			}
		}
		foreach (Rigidbody rigidbody in triggerTrainCollisions.otherRigidbodyContents)
		{
			num += rigidbody.mass;
		}
		return num;
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x000F4C48 File Offset: 0x000F2E48
	private float GetTotalPushingForces(Vector3 pushDirection, Vector3 ourForward, ref List<CompleteTrain> prevTrains)
	{
		float num = 0f;
		if (prevTrains.Count > 0)
		{
			if (prevTrains.Contains(this))
			{
				if (Global.developer > 1 || UnityEngine.Application.isEditor)
				{
					Debug.LogWarning("GetTotalPushingForces: Recursive loop detected. Bailing out.");
				}
				return 0f;
			}
			num += this.TotalForces;
		}
		prevTrains.Add(this);
		bool flag = Vector3.Dot(ourForward, pushDirection) >= 0f;
		TriggerTrainCollisions triggerTrainCollisions = flag ? this.frontCollisionTrigger : this.rearCollisionTrigger;
		if (!flag)
		{
			num *= -1f;
		}
		foreach (TrainCar trainCar in triggerTrainCollisions.trainContents)
		{
			if (trainCar.completeTrain != this)
			{
				Vector3 ourForward2 = trainCar.completeTrain.IsCoupledBackwards(trainCar) ? (-trainCar.transform.forward) : trainCar.transform.forward;
				num += trainCar.completeTrain.GetTotalPushingForces(pushDirection, ourForward2, ref prevTrains);
			}
		}
		return num;
	}

	// Token: 0x02000D0A RID: 3338
	private enum ShuntState
	{
		// Token: 0x040045F9 RID: 17913
		None,
		// Token: 0x040045FA RID: 17914
		Forwards,
		// Token: 0x040045FB RID: 17915
		Backwards
	}

	// Token: 0x02000D0B RID: 3339
	private enum StaticCollisionState
	{
		// Token: 0x040045FD RID: 17917
		Free,
		// Token: 0x040045FE RID: 17918
		StaticColliding,
		// Token: 0x040045FF RID: 17919
		StayingStill
	}
}
