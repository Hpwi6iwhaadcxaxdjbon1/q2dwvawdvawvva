using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C1 RID: 193
public class RFReceiver : IOEntity, IRFObject
{
	// Token: 0x04000AD3 RID: 2771
	public int frequency;

	// Token: 0x04000AD4 RID: 2772
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x0600114E RID: 4430 RVA: 0x0008DF50 File Offset: 0x0008C150
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFReceiver.OnRpcMessage", 0))
		{
			if (rpc == 2778616053U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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

	// Token: 0x0600114F RID: 4431 RVA: 0x0008E0B8 File Offset: 0x0008C2B8
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x00007641 File Offset: 0x00005841
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x00062769 File Offset: 0x00060969
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x00062775 File Offset: 0x00060975
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0005F5B6 File Offset: 0x0005D7B6
	public float GetMaxRange()
	{
		return 100000f;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0008E0C0 File Offset: 0x0008C2C0
	public override void Init()
	{
		base.Init();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0008E0D4 File Offset: 0x0008C2D4
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0008E0E8 File Offset: 0x0008C2E8
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.IsOn() == on)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, on, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0008E114 File Offset: 0x0008C314
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		int newFrequency = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, true, true);
		this.frequency = newFrequency;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x0008E16C File Offset: 0x0008C36C
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

	// Token: 0x0600115B RID: 4443 RVA: 0x0008E1BD File Offset: 0x0008C3BD
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x000876EB File Offset: 0x000858EB
	private bool CanChangeFrequency(BasePlayer player)
	{
		return player != null && player.CanBuild();
	}
}
