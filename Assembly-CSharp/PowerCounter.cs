using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x020000B2 RID: 178
public class PowerCounter : global::IOEntity
{
	// Token: 0x04000A65 RID: 2661
	private int counterNumber;

	// Token: 0x04000A66 RID: 2662
	private int targetCounterNumber = 10;

	// Token: 0x04000A67 RID: 2663
	public Canvas canvas;

	// Token: 0x04000A68 RID: 2664
	public CanvasGroup screenAlpha;

	// Token: 0x04000A69 RID: 2665
	public Text screenText;

	// Token: 0x04000A6A RID: 2666
	public const global::BaseEntity.Flags Flag_ShowPassthrough = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000A6B RID: 2667
	public GameObjectRef counterConfigPanel;

	// Token: 0x04000A6C RID: 2668
	public Color passthroughColor;

	// Token: 0x04000A6D RID: 2669
	public Color counterColor;

	// Token: 0x0600102F RID: 4143 RVA: 0x000873E0 File Offset: 0x000855E0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PowerCounter.OnRpcMessage", 0))
		{
			if (rpc == 3554226761U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTarget ");
				}
				using (TimeWarning.New("SERVER_SetTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3554226761U, "SERVER_SetTarget", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_SetTarget(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_SetTarget");
					}
				}
				return true;
			}
			if (rpc == 3222475159U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleDisplayMode ");
				}
				using (TimeWarning.New("ToggleDisplayMode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3222475159U, "ToggleDisplayMode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ToggleDisplayMode(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ToggleDisplayMode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0000564C File Offset: 0x0000384C
	public bool DisplayPassthrough()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x000876E0 File Offset: 0x000858E0
	public bool DisplayCounter()
	{
		return !this.DisplayPassthrough();
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x000876EB File Offset: 0x000858EB
	public bool CanPlayerAdmin(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000876FE File Offset: 0x000858FE
	public int GetTarget()
	{
		return this.targetCounterNumber;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00087706 File Offset: 0x00085906
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTarget(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		this.targetCounterNumber = msg.read.Int32();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x0008772F File Offset: 0x0008592F
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ToggleDisplayMode(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, msg.read.Bit(), false, false);
		this.MarkDirty();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x00087764 File Offset: 0x00085964
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.DisplayPassthrough() || this.counterNumber >= this.targetCounterNumber)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x00087788 File Offset: 0x00085988
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.counterNumber;
		info.msg.ioEntity.genericInt2 = this.GetPassthroughAmount(0);
		info.msg.ioEntity.genericInt3 = this.GetTarget();
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x000877FC File Offset: 0x000859FC
	public void SetCounterNumber(int newNumber)
	{
		this.counterNumber = newNumber;
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x00007D00 File Offset: 0x00005F00
	public override void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x00087805 File Offset: 0x00085A05
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00087814 File Offset: 0x00085A14
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (this.DisplayCounter() && inputAmount > 0 && inputSlot != 0)
		{
			int num = this.counterNumber;
			if (inputSlot == 1)
			{
				this.counterNumber++;
			}
			else if (inputSlot == 2)
			{
				this.counterNumber--;
				if (this.counterNumber < 0)
				{
					this.counterNumber = 0;
				}
			}
			else if (inputSlot == 3)
			{
				this.counterNumber = 0;
			}
			this.counterNumber = Mathf.Clamp(this.counterNumber, 0, 100);
			if (num != this.counterNumber)
			{
				this.MarkDirty();
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x000878B0 File Offset: 0x00085AB0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			if (base.isServer)
			{
				this.counterNumber = info.msg.ioEntity.genericInt1;
			}
			this.targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}
}
