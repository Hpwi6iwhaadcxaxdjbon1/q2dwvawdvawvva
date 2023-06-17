using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B7 RID: 183
public class ReactiveTarget : IOEntity
{
	// Token: 0x04000A8C RID: 2700
	public Animator myAnimator;

	// Token: 0x04000A8D RID: 2701
	public GameObjectRef bullseyeEffect;

	// Token: 0x04000A8E RID: 2702
	public GameObjectRef knockdownEffect;

	// Token: 0x04000A8F RID: 2703
	public float activationPowerTime = 0.5f;

	// Token: 0x04000A90 RID: 2704
	public int activationPowerAmount = 1;

	// Token: 0x04000A91 RID: 2705
	private float lastToggleTime = float.NegativeInfinity;

	// Token: 0x04000A92 RID: 2706
	private float knockdownHealth = 100f;

	// Token: 0x06001084 RID: 4228 RVA: 0x00088B80 File Offset: 0x00086D80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReactiveTarget.OnRpcMessage", 0))
		{
			if (rpc == 1798082523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lower ");
				}
				using (TimeWarning.New("RPC_Lower", 0))
				{
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
							this.RPC_Lower(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Lower");
					}
				}
				return true;
			}
			if (rpc == 2169477377U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Reset ");
				}
				using (TimeWarning.New("RPC_Reset", 0))
				{
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
							this.RPC_Reset(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Reset");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x00088DE0 File Offset: 0x00086FE0
	public void OnHitShared(HitInfo info)
	{
		if (this.IsKnockedDown())
		{
			return;
		}
		bool flag = info.HitBone == StringPool.Get("target_collider");
		bool flag2 = info.HitBone == StringPool.Get("target_collider_bullseye");
		if (!flag && !flag2)
		{
			return;
		}
		if (base.isServer)
		{
			float num = info.damageTypes.Total();
			if (flag2)
			{
				num *= 2f;
				Effect.server.Run(this.bullseyeEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
			}
			this.knockdownHealth -= num;
			if (this.knockdownHealth <= 0f)
			{
				Effect.server.Run(this.knockdownEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				this.QueueReset();
				this.SendPowerBurst();
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			else
			{
				base.ClientRPC<NetworkableId>(null, "HitEffect", info.Initiator.net.ID);
			}
			base.Hurt(1f, DamageType.Suicide, info.Initiator, false);
		}
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x00061F32 File Offset: 0x00060132
	public bool IsKnockedDown()
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x00088EFF File Offset: 0x000870FF
	public override void OnAttacked(HitInfo info)
	{
		this.OnHitShared(info);
		base.OnAttacked(info);
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x00088F0F File Offset: 0x0008710F
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && this.CanToggle();
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x00088F22 File Offset: 0x00087122
	public bool CanToggle()
	{
		return UnityEngine.Time.time > this.lastToggleTime + 1f;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x00088F37 File Offset: 0x00087137
	public void QueueReset()
	{
		base.Invoke(new Action(this.ResetTarget), 6f);
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00088F50 File Offset: 0x00087150
	public void ResetTarget()
	{
		if (!this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.CancelInvoke(new Action(this.ResetTarget));
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.knockdownHealth = 100f;
		this.SendPowerBurst();
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x00088F90 File Offset: 0x00087190
	private void LowerTarget()
	{
		if (this.IsKnockedDown() || !this.CanToggle())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.SendPowerBurst();
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x00088FB3 File Offset: 0x000871B3
	private void SendPowerBurst()
	{
		this.lastToggleTime = UnityEngine.Time.time;
		base.MarkDirtyForceUpdateOutputs();
		base.Invoke(new Action(base.MarkDirtyForceUpdateOutputs), this.activationPowerTime * 1.01f);
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x0000441C File Offset: 0x0000261C
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x00088FE4 File Offset: 0x000871E4
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			return;
		}
		if (inputAmount > 0)
		{
			if (inputSlot == 1)
			{
				this.ResetTarget();
				return;
			}
			if (inputSlot == 2)
			{
				this.LowerTarget();
			}
		}
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0008900B File Offset: 0x0008720B
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsKnockedDown())
		{
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
			if (UnityEngine.Time.time < this.lastToggleTime + this.activationPowerTime)
			{
				return this.activationPowerAmount;
			}
		}
		return 0;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x00089041 File Offset: 0x00087241
	[BaseEntity.RPC_Server]
	public void RPC_Reset(BaseEntity.RPCMessage msg)
	{
		this.ResetTarget();
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x00089049 File Offset: 0x00087249
	[BaseEntity.RPC_Server]
	public void RPC_Lower(BaseEntity.RPCMessage msg)
	{
		this.LowerTarget();
	}
}
