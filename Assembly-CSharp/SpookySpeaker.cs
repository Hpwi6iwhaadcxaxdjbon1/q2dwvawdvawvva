using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D3 RID: 211
public class SpookySpeaker : BaseCombatEntity
{
	// Token: 0x04000BCC RID: 3020
	public SoundPlayer soundPlayer;

	// Token: 0x04000BCD RID: 3021
	public float soundSpacing = 12f;

	// Token: 0x04000BCE RID: 3022
	public float soundSpacingRand = 5f;

	// Token: 0x060012D4 RID: 4820 RVA: 0x00097338 File Offset: 0x00095538
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpookySpeaker.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage wantsOn = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetWantsOn(wantsOn);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000974A0 File Offset: 0x000956A0
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000974B0 File Offset: 0x000956B0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000974DC File Offset: 0x000956DC
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.SendPlaySound), this.soundSpacing, this.soundSpacing, this.soundSpacingRand);
			base.Invoke(new Action(this.DelayedOff), 7200f);
			return;
		}
		base.CancelInvoke(new Action(this.SendPlaySound));
		base.CancelInvoke(new Action(this.DelayedOff));
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x00097551 File Offset: 0x00095751
	public void SendPlaySound()
	{
		base.ClientRPC(null, "PlaySpookySound");
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00062769 File Offset: 0x00060969
	public void DelayedOff()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}
}
