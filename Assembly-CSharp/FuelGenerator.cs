using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007B RID: 123
public class FuelGenerator : ContainerIOEntity
{
	// Token: 0x0400077F RID: 1919
	public int outputEnergy = 35;

	// Token: 0x04000780 RID: 1920
	public float fuelPerSec = 1f;

	// Token: 0x04000781 RID: 1921
	protected float fuelTickRate = 3f;

	// Token: 0x04000782 RID: 1922
	private float pendingFuel;

	// Token: 0x06000B7C RID: 2940 RVA: 0x00066290 File Offset: 0x00064490
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FuelGenerator.OnRpcMessage", 0))
		{
			if (rpc == 1401355317U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EngineSwitch ");
				}
				using (TimeWarning.New("RPC_EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1401355317U, "RPC_EngineSwitch", this, player, 3f))
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
							this.RPC_EngineSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x000663F8 File Offset: 0x000645F8
	public override int MaximalPowerOutput()
	{
		return this.outputEnergy;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00066400 File Offset: 0x00064600
	public override void Init()
	{
		if (base.IsOn())
		{
			this.UpdateCurrentEnergy();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
		base.Init();
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00066434 File Offset: 0x00064634
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0 && inputAmount > 0)
		{
			this.TurnOn();
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.TurnOff();
		}
		base.UpdateFromInput(inputAmount, inputSlot);
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00066459 File Offset: 0x00064659
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.outputEnergy;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0006646B File Offset: 0x0006466B
	public void UpdateCurrentEnergy()
	{
		this.currentEnergy = this.CalculateCurrentEnergy(0, 0);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0006647B File Offset: 0x0006467B
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00066488 File Offset: 0x00064688
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_EngineSwitch(BaseEntity.RPCMessage msg)
	{
		bool generatorState = msg.read.Bit();
		this.SetGeneratorState(generatorState);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x000664A8 File Offset: 0x000646A8
	public void SetGeneratorState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x000664BC File Offset: 0x000646BC
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x000664EA File Offset: 0x000646EA
	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x000664F8 File Offset: 0x000646F8
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "generator", true, false);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00066580 File Offset: 0x00064780
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		if (this.UseFuel(1f))
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateCurrentEnergy();
			this.MarkDirty();
			base.InvokeRepeating(new Action(this.FuelConsumption), this.fuelTickRate, this.fuelTickRate);
		}
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x000665D7 File Offset: 0x000647D7
	public void FuelConsumption()
	{
		if (!this.UseFuel(this.fuelTickRate))
		{
			this.TurnOff();
		}
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x000665ED File Offset: 0x000647ED
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.UpdateCurrentEnergy();
		this.MarkDirty();
		base.CancelInvoke(new Action(this.FuelConsumption));
	}
}
