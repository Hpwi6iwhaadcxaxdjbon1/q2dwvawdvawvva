using System;
using System.Runtime.CompilerServices;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class DeliveryDrone : global::Drone
{
	// Token: 0x04001014 RID: 4116
	[Header("Delivery Drone")]
	public float stateTimeout = 300f;

	// Token: 0x04001015 RID: 4117
	public float targetPositionTolerance = 1f;

	// Token: 0x04001016 RID: 4118
	public float preferredCruiseHeight = 20f;

	// Token: 0x04001017 RID: 4119
	public float preferredHeightAboveObstacle = 5f;

	// Token: 0x04001018 RID: 4120
	public float marginAbovePreferredHeight = 3f;

	// Token: 0x04001019 RID: 4121
	public float obstacleHeightLockDuration = 3f;

	// Token: 0x0400101A RID: 4122
	public int pickUpDelayInTicks = 3;

	// Token: 0x0400101B RID: 4123
	public DeliveryDroneConfig config;

	// Token: 0x0400101C RID: 4124
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x0400101D RID: 4125
	public EntityRef<Marketplace> sourceMarketplace;

	// Token: 0x0400101E RID: 4126
	public EntityRef<global::MarketTerminal> sourceTerminal;

	// Token: 0x0400101F RID: 4127
	public EntityRef<global::VendingMachine> targetVendingMachine;

	// Token: 0x04001020 RID: 4128
	private global::DeliveryDrone.State _state;

	// Token: 0x04001021 RID: 4129
	private RealTimeSince _sinceLastStateChange;

	// Token: 0x04001022 RID: 4130
	private Vector3? _stateGoalPosition;

	// Token: 0x04001023 RID: 4131
	private float? _goToY;

	// Token: 0x04001024 RID: 4132
	private TimeSince _sinceLastObstacleBlock;

	// Token: 0x04001025 RID: 4133
	private float? _minimumYLock;

	// Token: 0x04001026 RID: 4134
	private int _pickUpTicks;

	// Token: 0x04001027 RID: 4135
	private global::BaseEntity _mapMarkerInstance;

	// Token: 0x0600175A RID: 5978 RVA: 0x000B1910 File Offset: 0x000AFB10
	public void Setup(Marketplace marketplace, global::MarketTerminal terminal, global::VendingMachine vendingMachine)
	{
		this.sourceMarketplace.Set(marketplace);
		this.sourceTerminal.Set(terminal);
		this.targetVendingMachine.Set(vendingMachine);
		this._state = global::DeliveryDrone.State.Takeoff;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000B195F File Offset: 0x000AFB5F
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.Think), 0f, 0.5f, 0.25f);
		this.CreateMapMarker();
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x000B1990 File Offset: 0x000AFB90
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		global::BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x000B1A04 File Offset: 0x000AFC04
	private void Think()
	{
		global::DeliveryDrone.<>c__DisplayClass24_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this._sinceLastStateChange > this.stateTimeout)
		{
			Debug.LogError("Delivery drone hasn't change state in too long, killing", this);
			this.ForceRemove();
			return;
		}
		global::MarketTerminal marketTerminal;
		if (!this.sourceMarketplace.TryGet(true, out CS$<>8__locals1.marketplace) || !this.sourceTerminal.TryGet(true, out marketTerminal))
		{
			Debug.LogError("Delivery drone's marketplace or terminal was destroyed, killing", this);
			this.ForceRemove();
			return;
		}
		global::VendingMachine vendingMachine;
		if (!this.targetVendingMachine.TryGet(true, out vendingMachine) && this._state <= global::DeliveryDrone.State.AscendBeforeReturn)
		{
			this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
		}
		CS$<>8__locals1.currentPosition = base.transform.position;
		float num = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref CS$<>8__locals1);
		if (this._goToY != null)
		{
			if (!this.<Think>g__IsAtGoToY|24_6(ref CS$<>8__locals1))
			{
				this.targetPosition = new Vector3?(CS$<>8__locals1.currentPosition.WithY(this._goToY.Value));
				return;
			}
			this._goToY = null;
			this._sinceLastObstacleBlock = 0f;
			this._minimumYLock = new float?(CS$<>8__locals1.currentPosition.y);
		}
		switch (this._state)
		{
		case global::DeliveryDrone.State.Takeoff:
			this.<Think>g__SetGoalPosition|24_3(CS$<>8__locals1.marketplace.droneLaunchPoint.position + Vector3.up * 15f, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.FlyToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.FlyToVendingMachine:
		{
			bool flag;
			float num2 = this.<Think>g__CalculatePreferredY|24_0(out flag, ref CS$<>8__locals1);
			if (flag && CS$<>8__locals1.currentPosition.y < num2)
			{
				this.<Think>g__SetGoToY|24_5(num2 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector;
			Vector3 position;
			this.config.FindDescentPoints(vendingMachine, num2 + this.marginAbovePreferredHeight, out vector, out position);
			this.<Think>g__SetGoalPosition|24_3(position, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.DescendToVendingMachine, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.DescendToVendingMachine:
		{
			Vector3 vector;
			Vector3 position2;
			this.config.FindDescentPoints(vendingMachine, CS$<>8__locals1.currentPosition.y, out position2, out vector);
			this.<Think>g__SetGoalPosition|24_3(position2, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.PickUpItems, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.PickUpItems:
			this._pickUpTicks++;
			if (this._pickUpTicks >= this.pickUpDelayInTicks)
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.AscendBeforeReturn, ref CS$<>8__locals1);
			}
			break;
		case global::DeliveryDrone.State.AscendBeforeReturn:
		{
			Vector3 vector;
			Vector3 position3;
			this.config.FindDescentPoints(vendingMachine, num + this.preferredCruiseHeight, out vector, out position3);
			this.<Think>g__SetGoalPosition|24_3(position3, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.ReturnToTerminal, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.ReturnToTerminal:
		{
			bool flag2;
			float num3 = this.<Think>g__CalculatePreferredY|24_0(out flag2, ref CS$<>8__locals1);
			if (flag2 && CS$<>8__locals1.currentPosition.y < num3)
			{
				this.<Think>g__SetGoToY|24_5(num3 + this.marginAbovePreferredHeight, ref CS$<>8__locals1);
				return;
			}
			Vector3 vector2 = this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1);
			if (Vector3Ex.Distance2D(CS$<>8__locals1.currentPosition, vector2) < 30f)
			{
				vector2.y = Mathf.Max(vector2.y, num3 + this.marginAbovePreferredHeight);
			}
			else
			{
				vector2.y = num3 + this.marginAbovePreferredHeight;
			}
			this.<Think>g__SetGoalPosition|24_3(vector2, ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Landing, ref CS$<>8__locals1);
			}
			break;
		}
		case global::DeliveryDrone.State.Landing:
			this.<Think>g__SetGoalPosition|24_3(this.<Think>g__LandingPosition|24_2(ref CS$<>8__locals1), ref CS$<>8__locals1);
			if (this.<Think>g__IsAtGoalPosition|24_4(ref CS$<>8__locals1))
			{
				CS$<>8__locals1.marketplace.ReturnDrone(this);
				this.<Think>g__SetState|24_7(global::DeliveryDrone.State.Invalid, ref CS$<>8__locals1);
			}
			break;
		default:
			this.ForceRemove();
			break;
		}
		if (this._minimumYLock != null)
		{
			if (this._sinceLastObstacleBlock > this.obstacleHeightLockDuration)
			{
				this._minimumYLock = null;
				return;
			}
			if (this.targetPosition != null && this.targetPosition.Value.y < this._minimumYLock.Value)
			{
				this.targetPosition = new Vector3?(this.targetPosition.Value.WithY(this._minimumYLock.Value));
			}
		}
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x000B1E08 File Offset: 0x000B0008
	private void ForceRemove()
	{
		Marketplace marketplace;
		if (this.sourceMarketplace.TryGet(true, out marketplace))
		{
			marketplace.ReturnDrone(this);
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x000B1E34 File Offset: 0x000B0034
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.deliveryDrone = Pool.Get<ProtoBuf.DeliveryDrone>();
			info.msg.deliveryDrone.marketplaceId = this.sourceMarketplace.uid;
			info.msg.deliveryDrone.terminalId = this.sourceTerminal.uid;
			info.msg.deliveryDrone.vendingMachineId = this.targetVendingMachine.uid;
			info.msg.deliveryDrone.state = (int)this._state;
		}
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000B1EC8 File Offset: 0x000B00C8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.deliveryDrone != null)
		{
			this.sourceMarketplace = new EntityRef<Marketplace>(info.msg.deliveryDrone.marketplaceId);
			this.sourceTerminal = new EntityRef<global::MarketTerminal>(info.msg.deliveryDrone.terminalId);
			this.targetVendingMachine = new EntityRef<global::VendingMachine>(info.msg.deliveryDrone.vendingMachineId);
			this._state = (global::DeliveryDrone.State)info.msg.deliveryDrone.state;
		}
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanControl(ulong playerID)
	{
		return false;
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x000B1FAC File Offset: 0x000B01AC
	[CompilerGenerated]
	private float <Think>g__CalculatePreferredY|24_0(out bool isBlocked, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 toDirection;
		float num;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out toDirection, out num);
		if (num >= 0.5f)
		{
			float num2 = num * 2f;
			float a = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2);
			float b = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2 / 2f), ref A_2);
			float b2 = this.<Think>g__GetMinimumHeight|24_1(new Vector3(0f, 0f, num2), ref A_2);
			float num3 = Mathf.Max(Mathf.Max(a, b), b2) + this.preferredCruiseHeight;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, toDirection);
			Vector3 vector = this.config.halfExtents.WithZ(num2 / 2f);
			Vector3 vector2 = (A_2.currentPosition.WithY(num3) + quaternion * new Vector3(0f, 0f, vector.z / 2f)).WithY(num3 + 1000f);
			RaycastHit raycastHit;
			isBlocked = Physics.BoxCast(vector2, vector, Vector3.down, out raycastHit, quaternion, 1000f, this.config.layerMask);
			float result;
			if (isBlocked)
			{
				Ray ray = new Ray(vector2, Vector3.down);
				Vector3 b3 = ray.ClosestPoint(raycastHit.point);
				float num4 = Vector3.Distance(ray.origin, b3);
				result = num3 + (1000f - num4) + this.preferredHeightAboveObstacle;
			}
			else
			{
				result = num3;
			}
			return result;
		}
		float num5 = this.<Think>g__GetMinimumHeight|24_1(Vector3.zero, ref A_2) + this.preferredCruiseHeight;
		Vector3 origin = A_2.currentPosition.WithY(num5 + 1000f);
		A_2.currentPosition.WithY(num5);
		RaycastHit raycastHit2;
		isBlocked = Physics.Raycast(origin, Vector3.down, out raycastHit2, 1000f, this.config.layerMask);
		if (!isBlocked)
		{
			return num5;
		}
		return num5 + (1000f - raycastHit2.distance) + this.preferredHeightAboveObstacle;
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x000B219C File Offset: 0x000B039C
	[CompilerGenerated]
	private float <Think>g__GetMinimumHeight|24_1(Vector3 offset, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		Vector3 vector = base.transform.TransformPoint(offset);
		float height = TerrainMeta.HeightMap.GetHeight(vector);
		float height2 = WaterSystem.GetHeight(vector);
		return Mathf.Max(height, height2);
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x000B21CE File Offset: 0x000B03CE
	[CompilerGenerated]
	private Vector3 <Think>g__LandingPosition|24_2(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return A_1.marketplace.droneLaunchPoint.position;
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x000B21E0 File Offset: 0x000B03E0
	[CompilerGenerated]
	private void <Think>g__SetGoalPosition|24_3(Vector3 position, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = null;
		this._stateGoalPosition = new Vector3?(position);
		this.targetPosition = new Vector3?(position);
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x000B2206 File Offset: 0x000B0406
	[CompilerGenerated]
	private bool <Think>g__IsAtGoalPosition|24_4(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._stateGoalPosition != null && Vector3.Distance(this._stateGoalPosition.Value, A_1.currentPosition) < this.targetPositionTolerance;
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x000B2235 File Offset: 0x000B0435
	[CompilerGenerated]
	private void <Think>g__SetGoToY|24_5(float y, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._goToY = new float?(y);
		this.targetPosition = new Vector3?(A_2.currentPosition.WithY(y));
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x000B225A File Offset: 0x000B045A
	[CompilerGenerated]
	private bool <Think>g__IsAtGoToY|24_6(ref global::DeliveryDrone.<>c__DisplayClass24_0 A_1)
	{
		return this._goToY != null && Mathf.Abs(this._goToY.Value - A_1.currentPosition.y) < this.targetPositionTolerance;
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x000B2290 File Offset: 0x000B0490
	[CompilerGenerated]
	private void <Think>g__SetState|24_7(global::DeliveryDrone.State newState, ref global::DeliveryDrone.<>c__DisplayClass24_0 A_2)
	{
		this._state = newState;
		this._sinceLastStateChange = 0f;
		this._pickUpTicks = 0;
		this._stateGoalPosition = null;
		this._goToY = null;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this._state >= global::DeliveryDrone.State.AscendBeforeReturn, false, true);
	}

	// Token: 0x02000C2C RID: 3116
	private enum State
	{
		// Token: 0x0400425C RID: 16988
		Invalid,
		// Token: 0x0400425D RID: 16989
		Takeoff,
		// Token: 0x0400425E RID: 16990
		FlyToVendingMachine,
		// Token: 0x0400425F RID: 16991
		DescendToVendingMachine,
		// Token: 0x04004260 RID: 16992
		PickUpItems,
		// Token: 0x04004261 RID: 16993
		AscendBeforeReturn,
		// Token: 0x04004262 RID: 16994
		ReturnToTerminal,
		// Token: 0x04004263 RID: 16995
		Landing
	}
}
