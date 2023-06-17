using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C3 RID: 195
public class RHIB : MotorRowboat
{
	// Token: 0x04000ADC RID: 2780
	public GameObject steeringWheel;

	// Token: 0x04000ADD RID: 2781
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float rhibpopulation;

	// Token: 0x04000ADE RID: 2782
	private float targetGasPedal;

	// Token: 0x06001173 RID: 4467 RVA: 0x0008E764 File Offset: 0x0008C964
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RHIB.OnRpcMessage", 0))
		{
			if (rpc == 1382282393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Release ");
				}
				using (TimeWarning.New("Server_Release", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1382282393U, "Server_Release", this, player, 6f))
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
							this.Server_Release(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_Release");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0008E8CC File Offset: 0x0008CACC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(6f)]
	public void Server_Release(BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() == null)
		{
			return;
		}
		base.SetParent(null, true, true);
		base.SetToNonKinematic();
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0008E8EC File Offset: 0x0008CAEC
	public override void VehicleFixedUpdate()
	{
		this.gasPedal = Mathf.MoveTowards(this.gasPedal, this.targetGasPedal, UnityEngine.Time.fixedDeltaTime * 1f);
		base.VehicleFixedUpdate();
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0008E916 File Offset: 0x0008CB16
	public override bool EngineOn()
	{
		return base.EngineOn();
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0008E920 File Offset: 0x0008CB20
	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.targetGasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.targetGasPedal = -0.5f;
		}
		else
		{
			this.targetGasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0008E9A4 File Offset: 0x0008CBA4
	public void AddFuel(int amount)
	{
		StorageContainer storageContainer = this.fuelSystem.fuelStorageInstance.Get(true);
		if (storageContainer)
		{
			storageContainer.GetComponent<StorageContainer>().inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), amount, 0UL, ItemContainer.LimitStack.Existing);
		}
	}
}
