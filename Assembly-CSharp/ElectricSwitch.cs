using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000070 RID: 112
public class ElectricSwitch : IOEntity
{
	// Token: 0x04000706 RID: 1798
	public bool isToggleSwitch;

	// Token: 0x06000AD5 RID: 2773 RVA: 0x000625F4 File Offset: 0x000607F4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricSwitch.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00007641 File Offset: 0x00005841
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0006275C File Offset: 0x0006095C
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00062769 File Offset: 0x00060969
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x00062775 File Offset: 0x00060975
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00062787 File Offset: 0x00060987
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x000627AF File Offset: 0x000609AF
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x000627C8 File Offset: 0x000609C8
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0006281B File Offset: 0x00060A1B
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0006282C File Offset: 0x00060A2C
	public void Unbusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}
}
