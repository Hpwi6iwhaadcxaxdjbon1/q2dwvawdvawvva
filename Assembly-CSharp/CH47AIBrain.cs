using System;
using UnityEngine;

// Token: 0x0200047C RID: 1148
public class CH47AIBrain : BaseAIBrain
{
	// Token: 0x060025E8 RID: 9704 RVA: 0x000EF85C File Offset: 0x000EDA5C
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new CH47AIBrain.IdleState());
		base.AddState(new CH47AIBrain.PatrolState());
		base.AddState(new CH47AIBrain.OrbitState());
		base.AddState(new CH47AIBrain.EgressState());
		base.AddState(new CH47AIBrain.DropCrate());
		base.AddState(new CH47AIBrain.LandState());
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000EF8B1 File Offset: 0x000EDAB1
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.FixedUpdate;
		base.PathFinder = new CH47PathFinder();
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000EF8CB File Offset: 0x000EDACB
	public void FixedUpdate()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		this.Think(Time.fixedDeltaTime);
	}

	// Token: 0x02000CFC RID: 3324
	public class DropCrate : BaseAIBrain.BasicAIState
	{
		// Token: 0x040045D5 RID: 17877
		private float nextDropTime;

		// Token: 0x06004FF0 RID: 20464 RVA: 0x001A7479 File Offset: 0x001A5679
		public DropCrate() : base(AIState.DropCrate)
		{
		}

		// Token: 0x06004FF1 RID: 20465 RVA: 0x001A7483 File Offset: 0x001A5683
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && !this.CanDrop();
		}

		// Token: 0x06004FF2 RID: 20466 RVA: 0x001A7498 File Offset: 0x001A5698
		public bool CanDrop()
		{
			return Time.time > this.nextDropTime && (this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).CanDropCrate();
		}

		// Token: 0x06004FF3 RID: 20467 RVA: 0x001A74C0 File Offset: 0x001A56C0
		public override float GetWeight()
		{
			if (!this.CanDrop())
			{
				return 0f;
			}
			if (base.IsInState())
			{
				return 10000f;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Orbit && this.brain.CurrentState.TimeInState > 60f)
			{
				CH47DropZone closest = CH47DropZone.GetClosest(this.brain.mainInterestPoint);
				if (closest && Vector3Ex.Distance2D(closest.transform.position, this.brain.mainInterestPoint) < 200f)
				{
					CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
					if (component != null)
					{
						float num = Mathf.InverseLerp(300f, 600f, component.Age);
						return 1000f * num;
					}
				}
			}
			return 0f;
		}

		// Token: 0x06004FF4 RID: 20468 RVA: 0x001A759C File Offset: 0x001A579C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetDropDoorOpen(true);
			ch47HelicopterAIController.EnableFacingOverride(false);
			CH47DropZone closest = CH47DropZone.GetClosest(ch47HelicopterAIController.transform.position);
			if (closest == null)
			{
				this.nextDropTime = Time.time + 60f;
			}
			brain.mainInterestPoint = closest.transform.position;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004FF5 RID: 20469 RVA: 0x001A760C File Offset: 0x001A580C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			if (this.CanDrop() && Vector3Ex.Distance2D(brain.mainInterestPoint, ch47HelicopterAIController.transform.position) < 5f && ch47HelicopterAIController.rigidBody.velocity.magnitude < 5f)
			{
				ch47HelicopterAIController.DropCrate();
				this.nextDropTime = Time.time + 120f;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x001A7681 File Offset: 0x001A5881
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			(entity as CH47HelicopterAIController).SetDropDoorOpen(false);
			this.nextDropTime = Time.time + 60f;
			base.StateLeave(brain, entity);
		}
	}

	// Token: 0x02000CFD RID: 3325
	public class EgressState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040045D6 RID: 17878
		private bool killing;

		// Token: 0x040045D7 RID: 17879
		private bool egressAltitueAchieved;

		// Token: 0x06004FF7 RID: 20471 RVA: 0x001A76A8 File Offset: 0x001A58A8
		public EgressState() : base(AIState.Egress)
		{
		}

		// Token: 0x06004FF8 RID: 20472 RVA: 0x00007A3C File Offset: 0x00005C3C
		public override bool CanInterrupt()
		{
			return false;
		}

		// Token: 0x06004FF9 RID: 20473 RVA: 0x001A76B4 File Offset: 0x001A58B4
		public override float GetWeight()
		{
			CH47HelicopterAIController ch47HelicopterAIController = this.brain.GetBrainBaseEntity() as CH47HelicopterAIController;
			if (ch47HelicopterAIController.OutOfCrates() && !ch47HelicopterAIController.ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
			if (!(component != null))
			{
				return 0f;
			}
			if (component.Age <= 1800f)
			{
				return 0f;
			}
			return 10000f;
		}

		// Token: 0x06004FFA RID: 20474 RVA: 0x001A771C File Offset: 0x001A591C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			Transform transform = ch47HelicopterAIController.transform;
			Rigidbody rigidBody = ch47HelicopterAIController.rigidBody;
			Vector3 rhs = (rigidBody.velocity.magnitude < 0.1f) ? transform.forward : rigidBody.velocity.normalized;
			Vector3 a = Vector3.Cross(Vector3.Cross(transform.up, rhs), Vector3.up);
			brain.mainInterestPoint = transform.position + a * 8000f;
			brain.mainInterestPoint.y = 100f;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004FFB RID: 20475 RVA: 0x001A77C8 File Offset: 0x001A59C8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.killing)
			{
				return StateStatus.Running;
			}
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			if (position.y < 85f && !this.egressAltitueAchieved)
			{
				CH47LandingZone closest = CH47LandingZone.GetClosest(position);
				if (closest != null && Vector3Ex.Distance2D(closest.transform.position, position) < 20f)
				{
					float num = 0f;
					if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
					{
						num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(position), TerrainMeta.HeightMap.GetHeight(position));
					}
					num += 100f;
					Vector3 moveTarget = position;
					moveTarget.y = num;
					ch47HelicopterAIController.SetMoveTarget(moveTarget);
					return StateStatus.Running;
				}
			}
			this.egressAltitueAchieved = true;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			if (base.TimeInState > 300f)
			{
				ch47HelicopterAIController.Invoke("DelayedKill", 2f);
				this.killing = true;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000CFE RID: 3326
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004FFC RID: 20476 RVA: 0x0002BF84 File Offset: 0x0002A184
		public override float GetWeight()
		{
			return 0.1f;
		}

		// Token: 0x06004FFD RID: 20477 RVA: 0x001A78D0 File Offset: 0x001A5AD0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetMoveTarget(ch47HelicopterAIController.GetPosition() + ch47HelicopterAIController.rigidBody.velocity.normalized * 10f);
			base.StateEnter(brain, entity);
		}
	}

	// Token: 0x02000CFF RID: 3327
	public class LandState : BaseAIBrain.BasicAIState
	{
		// Token: 0x040045D8 RID: 17880
		private float landedForSeconds;

		// Token: 0x040045D9 RID: 17881
		private float lastLandtime;

		// Token: 0x040045DA RID: 17882
		private float landingHeight = 20f;

		// Token: 0x040045DB RID: 17883
		private float nextDismountTime;

		// Token: 0x06004FFF RID: 20479 RVA: 0x001A791A File Offset: 0x001A5B1A
		public LandState() : base(AIState.Land)
		{
		}

		// Token: 0x06005000 RID: 20480 RVA: 0x001A7930 File Offset: 0x001A5B30
		public override float GetWeight()
		{
			if (!(this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).ShouldLand())
			{
				return 0f;
			}
			float num = Time.time - this.lastLandtime;
			if (base.IsInState() && this.landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!base.IsInState() && num > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x001A79A0 File Offset: 0x001A5BA0
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			Vector3 forward = ch47HelicopterAIController.transform.forward;
			CH47LandingZone closest = CH47LandingZone.GetClosest(ch47HelicopterAIController.landingTarget);
			if (!closest)
			{
				return StateStatus.Error;
			}
			float magnitude = ch47HelicopterAIController.rigidBody.velocity.magnitude;
			float num = Vector3Ex.Distance2D(closest.transform.position, position);
			bool enabled = num < 40f;
			bool altitudeProtection = num > 15f && position.y < closest.transform.position.y + 10f;
			ch47HelicopterAIController.EnableFacingOverride(enabled);
			ch47HelicopterAIController.SetAltitudeProtection(altitudeProtection);
			bool flag = Mathf.Abs(closest.transform.position.y - position.y) < 3f && num <= 5f && magnitude < 1f;
			if (flag)
			{
				this.landedForSeconds += delta;
				if (this.lastLandtime == 0f)
				{
					this.lastLandtime = Time.time;
				}
			}
			float num2 = 1f - Mathf.InverseLerp(0f, 7f, num);
			this.landingHeight -= 4f * num2 * Time.deltaTime;
			if (this.landingHeight < -5f)
			{
				this.landingHeight = -5f;
			}
			ch47HelicopterAIController.SetAimDirection(closest.transform.forward);
			Vector3 moveTarget = brain.mainInterestPoint + new Vector3(0f, this.landingHeight, 0f);
			if (num < 100f && num > 15f)
			{
				Vector3 vector = Vector3Ex.Direction2D(closest.transform.position, position);
				RaycastHit raycastHit;
				if (Physics.SphereCast(position, 15f, vector, out raycastHit, num, 1218511105))
				{
					Vector3 a = Vector3.Cross(vector, Vector3.up);
					moveTarget = raycastHit.point + a * 50f;
				}
			}
			ch47HelicopterAIController.SetMoveTarget(moveTarget);
			if (flag)
			{
				if (this.landedForSeconds > 1f && Time.time > this.nextDismountTime)
				{
					foreach (BaseVehicle.MountPointInfo mountPointInfo in ch47HelicopterAIController.mountPoints)
					{
						if (mountPointInfo.mountable && mountPointInfo.mountable.AnyMounted())
						{
							this.nextDismountTime = Time.time + 0.5f;
							mountPointInfo.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (this.landedForSeconds > 8f)
				{
					brain.GetComponent<CH47AIBrain>().ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x06005002 RID: 20482 RVA: 0x001A7C60 File Offset: 0x001A5E60
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			brain.mainInterestPoint = (entity as CH47HelicopterAIController).landingTarget;
			this.landingHeight = 15f;
			base.StateEnter(brain, entity);
		}

		// Token: 0x06005003 RID: 20483 RVA: 0x001A7C86 File Offset: 0x001A5E86
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.SetAltitudeProtection(true);
			ch47HelicopterAIController.SetMinHoverHeight(30f);
			this.landedForSeconds = 0f;
			base.StateLeave(brain, entity);
		}

		// Token: 0x06005004 RID: 20484 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CanInterrupt()
		{
			return true;
		}
	}

	// Token: 0x02000D00 RID: 3328
	public class OrbitState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06005005 RID: 20485 RVA: 0x001A7CB9 File Offset: 0x001A5EB9
		public OrbitState() : base(AIState.Orbit)
		{
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x001A7CC3 File Offset: 0x001A5EC3
		public Vector3 GetOrbitCenter()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x001A7CD0 File Offset: 0x001A5ED0
		public override float GetWeight()
		{
			if (base.IsInState())
			{
				float num = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState);
				return 5f * num;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Patrol)
			{
				CH47AIBrain.PatrolState patrolState = this.brain.CurrentState as CH47AIBrain.PatrolState;
				if (patrolState != null && patrolState.AtPatrolDestination())
				{
					return 5f;
				}
			}
			return 0f;
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x001A7D50 File Offset: 0x001A5F50
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(true);
			ch47HelicopterAIController.InitiateAnger();
			base.StateEnter(brain, entity);
		}

		// Token: 0x06005009 RID: 20489 RVA: 0x001A7D6C File Offset: 0x001A5F6C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			Vector3 orbitCenter = this.GetOrbitCenter();
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.GetPosition();
			Vector3 vector = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
			float d = (Vector3.Dot(Vector3.Cross(ch47HelicopterAIController.transform.right, Vector3.up), vector2) < 0f) ? -1f : 1f;
			float d2 = 75f;
			Vector3 normalized = (-vector + vector2 * d * 0.6f).normalized;
			Vector3 vector3 = orbitCenter + normalized * d2;
			ch47HelicopterAIController.SetMoveTarget(vector3);
			ch47HelicopterAIController.SetAimDirection(Vector3Ex.Direction2D(vector3, position));
			base.StateThink(delta, brain, entity);
			return StateStatus.Running;
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x001A7E31 File Offset: 0x001A6031
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.CancelAnger();
			base.StateLeave(brain, entity);
		}
	}

	// Token: 0x02000D01 RID: 3329
	public class PatrolState : BaseAIBrain.BasePatrolState
	{
		// Token: 0x040045DC RID: 17884
		protected float patrolApproachDist = 75f;

		// Token: 0x0600500B RID: 20491 RVA: 0x001A7E4D File Offset: 0x001A604D
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x001A7E68 File Offset: 0x001A6068
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			(entity as CH47HelicopterAIController).SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		// Token: 0x0600500D RID: 20493 RVA: 0x001A7E86 File Offset: 0x001A6086
		public bool AtPatrolDestination()
		{
			return Vector3Ex.Distance2D(this.GetDestination(), this.brain.transform.position) < this.patrolApproachDist;
		}

		// Token: 0x0600500E RID: 20494 RVA: 0x001A7CC3 File Offset: 0x001A5EC3
		public Vector3 GetDestination()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x0600500F RID: 20495 RVA: 0x001A7EAB File Offset: 0x001A60AB
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && this.AtPatrolDestination();
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x001A7EC0 File Offset: 0x001A60C0
		public override float GetWeight()
		{
			if (!base.IsInState())
			{
				float num = Mathf.InverseLerp(70f, 120f, base.TimeSinceState()) * 5f;
				return 1f + num;
			}
			if (this.AtPatrolDestination() && base.TimeInState > 2f)
			{
				return 0f;
			}
			return 3f;
		}
	}
}
