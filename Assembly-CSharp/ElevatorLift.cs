using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000071 RID: 113
public class ElevatorLift : BaseCombatEntity
{
	// Token: 0x04000707 RID: 1799
	public GameObject DescendingHurtTrigger;

	// Token: 0x04000708 RID: 1800
	public GameObject MovementCollider;

	// Token: 0x04000709 RID: 1801
	public Transform UpButtonPoint;

	// Token: 0x0400070A RID: 1802
	public Transform DownButtonPoint;

	// Token: 0x0400070B RID: 1803
	public TriggerNotify VehicleTrigger;

	// Token: 0x0400070C RID: 1804
	public GameObjectRef LiftArrivalScreenBounce;

	// Token: 0x0400070D RID: 1805
	public SoundDefinition liftMovementLoopDef;

	// Token: 0x0400070E RID: 1806
	public SoundDefinition liftMovementStartDef;

	// Token: 0x0400070F RID: 1807
	public SoundDefinition liftMovementStopDef;

	// Token: 0x04000710 RID: 1808
	public SoundDefinition liftMovementAccentSoundDef;

	// Token: 0x04000711 RID: 1809
	public GameObjectRef liftButtonPressedEffect;

	// Token: 0x04000712 RID: 1810
	public float movementAccentMinInterval = 0.75f;

	// Token: 0x04000713 RID: 1811
	public float movementAccentMaxInterval = 3f;

	// Token: 0x04000714 RID: 1812
	private Sound liftMovementLoopSound;

	// Token: 0x04000715 RID: 1813
	private float nextMovementAccent;

	// Token: 0x04000716 RID: 1814
	public Vector3 lastPosition;

	// Token: 0x04000717 RID: 1815
	private const BaseEntity.Flags PressedUp = BaseEntity.Flags.Reserved1;

	// Token: 0x04000718 RID: 1816
	private const BaseEntity.Flags PressedDown = BaseEntity.Flags.Reserved2;

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0006283C File Offset: 0x00060A3C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElevatorLift.OnRpcMessage", 0))
		{
			if (rpc == 4061236510U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RaiseLowerFloor ");
				}
				using (TimeWarning.New("Server_RaiseLowerFloor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4061236510U, "Server_RaiseLowerFloor", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RaiseLowerFloor(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RaiseLowerFloor");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x000629A4 File Offset: 0x00060BA4
	private Elevator owner
	{
		get
		{
			return base.GetParentEntity() as Elevator;
		}
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x000629B1 File Offset: 0x00060BB1
	public override void ServerInit()
	{
		base.ServerInit();
		this.ToggleHurtTrigger(false);
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x000629C0 File Offset: 0x00060BC0
	public void ToggleHurtTrigger(bool state)
	{
		if (this.DescendingHurtTrigger != null)
		{
			this.DescendingHurtTrigger.SetActive(state);
		}
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000629DC File Offset: 0x00060BDC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void Server_RaiseLowerFloor(BaseEntity.RPCMessage msg)
	{
		if (!this.CanMove())
		{
			return;
		}
		Elevator.Direction direction = (Elevator.Direction)msg.read.Int32();
		bool goTopBottom = msg.read.Bit();
		base.SetFlag((direction == Elevator.Direction.Up) ? BaseEntity.Flags.Reserved1 : BaseEntity.Flags.Reserved2, true, false, true);
		this.owner.Server_RaiseLowerElevator(direction, goTopBottom);
		base.Invoke(new Action(this.ClearDirection), 0.7f);
		if (this.liftButtonPressedEffect.isValid)
		{
			Effect.server.Run(this.liftButtonPressedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x00062A75 File Offset: 0x00060C75
	private void ClearDirection()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00062A94 File Offset: 0x00060C94
	public override void Hurt(HitInfo info)
	{
		BaseCombatEntity baseCombatEntity;
		if (base.HasParent() && (baseCombatEntity = (base.GetParentEntity() as BaseCombatEntity)) != null)
		{
			baseCombatEntity.Hurt(info);
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00062ABF File Offset: 0x00060CBF
	public override void AdminKill()
	{
		if (base.HasParent())
		{
			base.GetParentEntity().AdminKill();
			return;
		}
		base.AdminKill();
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x00062ADB File Offset: 0x00060CDB
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.ClearDirection();
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00062AEC File Offset: 0x00060CEC
	public bool CanMove()
	{
		if (this.VehicleTrigger.HasContents && this.VehicleTrigger.entityContents != null)
		{
			using (HashSet<BaseEntity>.Enumerator enumerator = this.VehicleTrigger.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!(enumerator.Current is Drone))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void NotifyNewFloor(int newFloor, int totalFloors)
	{
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00062B64 File Offset: 0x00060D64
	public void ToggleMovementCollider(bool state)
	{
		if (this.MovementCollider != null)
		{
			this.MovementCollider.SetActive(state);
		}
	}
}
