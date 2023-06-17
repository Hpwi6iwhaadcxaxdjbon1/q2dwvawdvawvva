using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B5 RID: 181
public class PressButton : IOEntity
{
	// Token: 0x04000A7C RID: 2684
	public float pressDuration = 5f;

	// Token: 0x04000A7D RID: 2685
	public float pressPowerTime = 0.5f;

	// Token: 0x04000A7E RID: 2686
	public int pressPowerAmount = 2;

	// Token: 0x04000A7F RID: 2687
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;

	// Token: 0x04000A80 RID: 2688
	public bool smallBurst;

	// Token: 0x0600106C RID: 4204 RVA: 0x000883F0 File Offset: 0x000865F0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PressButton.OnRpcMessage", 0))
		{
			if (rpc == 3778543711U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Press ");
				}
				using (TimeWarning.New("Press", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3778543711U, "Press", this, player, 3f))
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
							this.Press(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Press");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00088558 File Offset: 0x00086758
	public override void ResetIOState()
	{
		base.ResetIOState();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		base.CancelInvoke(new Action(this.Unpress));
		base.CancelInvoke(new Action(this.UnpowerTime));
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000885A8 File Offset: 0x000867A8
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		if (!(this.sourceItem != null) && !this.smallBurst)
		{
			return base.GetPassthroughAmount(0);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return Mathf.Max(this.pressPowerAmount, base.GetPassthroughAmount(0));
		}
		return 0;
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x000885FE File Offset: 0x000867FE
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x00088614 File Offset: 0x00086814
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x00088628 File Offset: 0x00086828
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void Press(BaseEntity.RPCMessage msg)
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.Invoke(new Action(this.Unpress), this.pressDuration);
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x00057F2F File Offset: 0x0005612F
	public void Unpress()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00088693 File Offset: 0x00086893
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericFloat1 = this.pressDuration;
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000886B2 File Offset: 0x000868B2
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.pressDuration = info.msg.ioEntity.genericFloat1;
		}
	}
}
