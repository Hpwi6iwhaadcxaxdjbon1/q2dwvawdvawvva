using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004AF RID: 1199
public class TrainBarricade : BaseCombatEntity, ITrainCollidable, TrainTrackSpline.ITrainTrackUser
{
	// Token: 0x04001F9A RID: 8090
	[FormerlySerializedAs("damagePerMPS")]
	[SerializeField]
	private float trainDamagePerMPS = 10f;

	// Token: 0x04001F9B RID: 8091
	[SerializeField]
	private float minVelToDestroy = 6f;

	// Token: 0x04001F9C RID: 8092
	[SerializeField]
	private float velReduction = 2f;

	// Token: 0x04001F9D RID: 8093
	[SerializeField]
	private GameObjectRef barricadeDamageEffect;

	// Token: 0x04001F9F RID: 8095
	private TrainCar hitTrain;

	// Token: 0x04001FA0 RID: 8096
	private TriggerTrainCollisions hitTrainTrigger;

	// Token: 0x04001FA1 RID: 8097
	private TrainTrackSpline track;

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x0600272D RID: 10029 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x0600272E RID: 10030 RVA: 0x000F4E9B File Offset: 0x000F309B
	// (set) Token: 0x0600272F RID: 10031 RVA: 0x000F4EA3 File Offset: 0x000F30A3
	public float FrontWheelSplineDist { get; private set; }

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06002730 RID: 10032 RVA: 0x0004E73F File Offset: 0x0004C93F
	public TrainCar.TrainCarType CarType
	{
		get
		{
			return TrainCar.TrainCarType.Other;
		}
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000F4EAC File Offset: 0x000F30AC
	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		bool result = false;
		if (base.isServer)
		{
			float num = Mathf.Abs(train.GetTrackSpeed());
			this.SetHitTrain(train, trainTrigger);
			if (num < this.minVelToDestroy && !vehicle.cinematictrains)
			{
				base.InvokeRandomized(new Action(this.PushForceTick), 0f, 0.25f, 0.025f);
			}
			else
			{
				result = true;
				base.Invoke(new Action(this.DestroyThisBarrier), 0f);
			}
		}
		return result;
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000F4F24 File Offset: 0x000F3124
	public override void ServerInit()
	{
		base.ServerInit();
		TrainTrackSpline trainTrackSpline;
		float frontWheelSplineDist;
		if (TrainTrackSpline.TryFindTrackNear(base.transform.position, 3f, out trainTrackSpline, out frontWheelSplineDist))
		{
			this.track = trainTrackSpline;
			this.FrontWheelSplineDist = frontWheelSplineDist;
			this.track.RegisterTrackUser(this);
		}
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000F4F6C File Offset: 0x000F316C
	internal override void DoServerDestroy()
	{
		if (this.track != null)
		{
			this.track.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	// Token: 0x06002734 RID: 10036 RVA: 0x000F4F8E File Offset: 0x000F318E
	private void SetHitTrain(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		this.hitTrain = train;
		this.hitTrainTrigger = trainTrigger;
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000F4F9E File Offset: 0x000F319E
	private void ClearHitTrain()
	{
		this.SetHitTrain(null, null);
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000F4FA8 File Offset: 0x000F31A8
	private void DestroyThisBarrier()
	{
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		if (this.hitTrain != null)
		{
			this.hitTrain.completeTrain.ReduceSpeedBy(this.velReduction);
			if (vehicle.cinematictrains)
			{
				this.hitTrain.Hurt(9999f, DamageType.Collision, this, false);
			}
			else
			{
				float amount = Mathf.Abs(this.hitTrain.GetTrackSpeed()) * this.trainDamagePerMPS;
				this.hitTrain.Hurt(amount, DamageType.Collision, this, false);
			}
		}
		this.ClearHitTrain();
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000F503C File Offset: 0x000F323C
	private void PushForceTick()
	{
		if (this.hitTrain == null || this.hitTrainTrigger == null || this.hitTrain.IsDead() || this.hitTrain.IsDestroyed || this.IsDead())
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
			return;
		}
		bool flag = true;
		if (!this.hitTrainTrigger.triggerCollider.bounds.Intersects(this.bounds))
		{
			Vector3 vector;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Front)
			{
				vector = this.hitTrainTrigger.owner.GetFrontOfTrainPos();
			}
			else
			{
				vector = this.hitTrainTrigger.owner.GetRearOfTrainPos();
			}
			Vector3 vector2 = base.transform.position + this.bounds.ClosestPoint(vector - base.transform.position);
			Debug.DrawRay(vector2, Vector3.up, Color.red, 10f);
			flag = (Vector3.SqrMagnitude(vector2 - vector) < 1f);
		}
		if (flag)
		{
			float num = this.hitTrainTrigger.owner.completeTrain.TotalForces;
			if (this.hitTrainTrigger.location == TriggerTrainCollisions.Location.Rear)
			{
				num *= -1f;
			}
			num = Mathf.Max(0f, num);
			base.Hurt(0.002f * num);
			if (this.IsDead())
			{
				this.hitTrain.completeTrain.FreeStaticCollision();
				return;
			}
		}
		else
		{
			this.ClearHitTrain();
			base.CancelInvoke(new Action(this.PushForceTick));
		}
	}
}
