using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DF RID: 223
public class TimerSwitch : IOEntity
{
	// Token: 0x04000C32 RID: 3122
	public float timerLength = 10f;

	// Token: 0x04000C33 RID: 3123
	public Transform timerDrum;

	// Token: 0x04000C34 RID: 3124
	private float timePassed = -1f;

	// Token: 0x06001392 RID: 5010 RVA: 0x0009D460 File Offset: 0x0009B660
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TimerSwitch.OnRpcMessage", 0))
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

	// Token: 0x06001393 RID: 5011 RVA: 0x0009D5C8 File Offset: 0x0009B7C8
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.AdvanceTime)))
		{
			this.EndTimer();
		}
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x0009D5F4 File Offset: 0x0009B7F4
	public override bool WantsPassthroughPower()
	{
		return this.IsPowered() && base.IsOn();
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x0009D606 File Offset: 0x0009B806
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!this.IsPowered() || !base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x0009D624 File Offset: 0x0009B824
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, inputAmount > 0, false, false);
		}
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x0009D63C File Offset: 0x0009B83C
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsInvoking(new Action(this.AdvanceTime)))
			{
				this.EndTimer();
				return;
			}
			if (this.timePassed != -1f)
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, false);
				this.SwitchPressed();
				return;
			}
		}
		else if (inputSlot == 1 && inputAmount > 0)
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0009D6A3 File Offset: 0x0009B8A3
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SVSwitch(BaseEntity.RPCMessage msg)
	{
		this.SwitchPressed();
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x0009D6AC File Offset: 0x0009B8AC
	public void SwitchPressed()
	{
		if (base.IsOn())
		{
			return;
		}
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		base.InvokeRepeating(new Action(this.AdvanceTime), 0f, 0.1f);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0009D700 File Offset: 0x0009B900
	public void AdvanceTime()
	{
		if (this.timePassed < 0f)
		{
			this.timePassed = 0f;
		}
		this.timePassed += 0.1f;
		if (this.timePassed >= this.timerLength)
		{
			this.EndTimer();
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0009D753 File Offset: 0x0009B953
	public void EndTimer()
	{
		base.CancelInvoke(new Action(this.AdvanceTime));
		this.timePassed = -1f;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0009D789 File Offset: 0x0009B989
	public float GetPassedTime()
	{
		return this.timePassed;
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0009D791 File Offset: 0x0009B991
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.timePassed == -1f)
		{
			if (base.IsOn())
			{
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				return;
			}
		}
		else
		{
			this.SwitchPressed();
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0009D7BF File Offset: 0x0009B9BF
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.GetPassedTime();
		info.msg.ioEntity.genericFloat2 = this.timerLength;
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0009D7F4 File Offset: 0x0009B9F4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.timerLength = info.msg.ioEntity.genericFloat2;
			this.timePassed = info.msg.ioEntity.genericFloat1;
		}
	}
}
