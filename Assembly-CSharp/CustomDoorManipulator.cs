using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000062 RID: 98
public class CustomDoorManipulator : DoorManipulator
{
	// Token: 0x06000A1B RID: 2587 RVA: 0x0005DE30 File Offset: 0x0005C030
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0))
		{
			if (rpc == 1224330484U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoPair ");
				}
				using (TimeWarning.New("DoPair", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1224330484U, "DoPair", this, player, 3f))
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
							this.DoPair(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoPair");
					}
				}
				return true;
			}
			if (rpc == 3800726972U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerActionChange ");
				}
				using (TimeWarning.New("ServerActionChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3800726972U, "ServerActionChange", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerActionChange(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ServerActionChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool PairWithLockedDoors()
	{
		return false;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0005E130 File Offset: 0x0005C330
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0005E14E File Offset: 0x0005C34E
	public bool IsPaired()
	{
		return this.targetDoor != null;
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0005E15C File Offset: 0x0005C35C
	public void RefreshDoor()
	{
		this.SetTargetDoor(this.targetDoor);
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0005E16A File Offset: 0x0005C36A
	private void OnPhysicsNeighbourChanged()
	{
		this.SetTargetDoor(this.targetDoor);
		base.Invoke(new Action(this.RefreshDoor), 0.1f);
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0005E18F File Offset: 0x0005C38F
	public override void SetupInitialDoorConnection()
	{
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0005E1C4 File Offset: 0x0005C3C4
	public override void DoActionDoorMissing()
	{
		this.SetTargetDoor(null);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0005E1D0 File Offset: 0x0005C3D0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoPair(BaseEntity.RPCMessage msg)
	{
		Door targetDoor = this.targetDoor;
		Door door = base.FindDoor(this.PairWithLockedDoors());
		if (door != targetDoor)
		{
			this.SetTargetDoor(door);
		}
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerActionChange(BaseEntity.RPCMessage msg)
	{
	}
}
