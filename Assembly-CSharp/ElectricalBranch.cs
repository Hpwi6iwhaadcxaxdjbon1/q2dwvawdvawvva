using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006F RID: 111
public class ElectricalBranch : IOEntity
{
	// Token: 0x04000703 RID: 1795
	public int branchAmount = 2;

	// Token: 0x04000704 RID: 1796
	public GameObjectRef branchPanelPrefab;

	// Token: 0x04000705 RID: 1797
	private float nextChangeTime;

	// Token: 0x06000ACC RID: 2764 RVA: 0x00062360 File Offset: 0x00060560
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricalBranch.OnRpcMessage", 0))
		{
			if (rpc == 643124146U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetBranchOffPower ");
				}
				using (TimeWarning.New("SetBranchOffPower", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(643124146U, "SetBranchOffPower", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage branchOffPower = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetBranchOffPower(branchOffPower);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetBranchOffPower");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x000624C8 File Offset: 0x000606C8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetBranchOffPower(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player == null || !player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 1f;
		int value = msg.read.Int32();
		value = Mathf.Clamp(value, 2, 10000000);
		this.branchAmount = value;
		base.MarkDirtyForceUpdateOutputs();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x0006253A File Offset: 0x0006073A
	public override bool AllowDrainFrom(int outputSlot)
	{
		return outputSlot != 1;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00062543 File Offset: 0x00060743
	public override int DesiredPower()
	{
		return this.branchAmount;
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0006254B File Offset: 0x0006074B
	public void SetBranchAmount(int newAmount)
	{
		newAmount = Mathf.Clamp(newAmount, 2, 100000000);
		this.branchAmount = newAmount;
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00062562 File Offset: 0x00060762
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot == 0)
		{
			return Mathf.Clamp(this.GetCurrentEnergy() - this.branchAmount, 0, this.GetCurrentEnergy());
		}
		if (outputSlot == 1)
		{
			return Mathf.Min(this.GetCurrentEnergy(), this.branchAmount);
		}
		return 0;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00062598 File Offset: 0x00060798
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.branchAmount;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x000625B7 File Offset: 0x000607B7
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.branchAmount = info.msg.ioEntity.genericInt1;
		}
	}
}
