using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A8 RID: 168
public class PagerEntity : global::BaseEntity, IRFObject
{
	// Token: 0x04000A26 RID: 2598
	public static global::BaseEntity.Flags Flag_Silent = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000A27 RID: 2599
	private int frequency = 55;

	// Token: 0x04000A28 RID: 2600
	public float beepRepeat = 2f;

	// Token: 0x04000A29 RID: 2601
	public GameObjectRef pagerEffect;

	// Token: 0x04000A2A RID: 2602
	public GameObjectRef silentEffect;

	// Token: 0x04000A2B RID: 2603
	private float nextChangeTime;

	// Token: 0x06000F6A RID: 3946 RVA: 0x000819E4 File Offset: 0x0007FBE4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PagerEntity.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2778616053U, "ServerSetFrequency", this, player, 3f))
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

	// Token: 0x06000F6B RID: 3947 RVA: 0x00081B4C File Offset: 0x0007FD4C
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x00081B54 File Offset: 0x0007FD54
	public override void SwitchParent(global::BaseEntity ent)
	{
		base.SetParent(ent, false, true);
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x00081B5F File Offset: 0x0007FD5F
	public override void ServerInit()
	{
		base.ServerInit();
		RFManager.AddListener(this.frequency, this);
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x00081B73 File Offset: 0x0007FD73
	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x00081B87 File Offset: 0x0007FD87
	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x00081B90 File Offset: 0x0007FD90
	public void RFSignalUpdate(bool on)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		bool flag = base.IsOn();
		if (on != flag)
		{
			base.SetFlag(global::BaseEntity.Flags.On, on, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x00081BC2 File Offset: 0x0007FDC2
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(PagerEntity.Flag_Silent, wantsSilent, false, true);
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x00062769 File Offset: 0x00060969
	public void SetOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x00081BD2 File Offset: 0x0007FDD2
	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.frequency, newFreq, this, true, true);
		this.frequency = newFreq;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x00081BEC File Offset: 0x0007FDEC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int newFrequency = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, newFrequency, this, true, true);
		this.frequency = newFrequency;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x00081C5D File Offset: 0x0007FE5D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x00081C8C File Offset: 0x0007FE8C
	internal override void OnParentRemoved()
	{
		base.SetParent(null, false, true);
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x00081C97 File Offset: 0x0007FE97
	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x00081CB0 File Offset: 0x0007FEB0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
		if (base.isServer && info.fromDisk)
		{
			this.ChangeFrequency(this.frequency);
		}
	}
}
