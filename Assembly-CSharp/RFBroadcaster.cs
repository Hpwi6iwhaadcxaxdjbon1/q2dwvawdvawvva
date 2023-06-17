using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C0 RID: 192
public class RFBroadcaster : IOEntity, IRFObject
{
	// Token: 0x04000ACD RID: 2765
	public int frequency;

	// Token: 0x04000ACE RID: 2766
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x04000ACF RID: 2767
	public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;

	// Token: 0x04000AD0 RID: 2768
	public bool playerUsable = true;

	// Token: 0x04000AD1 RID: 2769
	private float nextChangeTime;

	// Token: 0x04000AD2 RID: 2770
	private float nextStopTime;

	// Token: 0x0600113F RID: 4415 RVA: 0x0008DBE4 File Offset: 0x0008BDE4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFBroadcaster.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSetFrequency ");
				}
				using (TimeWarning.New("ServerSetFrequency", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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
							this.ServerSetFrequency(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerSetFrequency");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0008DD4C File Offset: 0x0008BF4C
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0005F5B6 File Offset: 0x0005D7B6
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x000063A5 File Offset: 0x000045A5
	public void RFSignalUpdate(bool on)
	{
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x0008DD54 File Offset: 0x0008BF54
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (!this.CanChangeFrequency(msg.player))
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int newFrequency = msg.read.Int32();
		if (RFManager.IsReserved(newFrequency))
		{
			RFManager.ReserveErrorPrint(msg.player);
			return;
		}
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, false, this.IsPowered());
		this.frequency = newFrequency;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.Hurt(this.MaxHealth() * 0.01f, DamageType.Decay, this, true);
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x0008DDED File Offset: 0x0008BFED
	public override bool CanUseNetworkCache(Connection connection)
	{
		return !this.playerUsable && base.CanUseNetworkCache(connection);
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0008DE00 File Offset: 0x0008C000
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			Connection forConnection = info.forConnection;
			if (!this.CanChangeFrequency(((forConnection != null) ? forConnection.player : null) as BasePlayer))
			{
				return;
			}
		}
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0008DE54 File Offset: 0x0008C054
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputAmount > 0)
		{
			base.CancelInvoke(new Action(this.StopBroadcasting));
			RFManager.AddBroadcaster(this.frequency, this);
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.nextStopTime = UnityEngine.Time.time + 1f;
			return;
		}
		base.Invoke(new Action(this.StopBroadcasting), Mathf.Clamp01(this.nextStopTime - UnityEngine.Time.time));
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x0008DEC8 File Offset: 0x0008C0C8
	public void StopBroadcasting()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0008DEE4 File Offset: 0x0008C0E4
	internal override void DoServerDestroy()
	{
		RFManager.RemoveBroadcaster(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0008DEF8 File Offset: 0x0008C0F8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0008DF24 File Offset: 0x0008C124
	private bool CanChangeFrequency(BasePlayer player)
	{
		return this.playerUsable && player != null && player.CanBuild();
	}
}
