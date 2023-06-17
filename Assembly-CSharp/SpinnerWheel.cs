using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D2 RID: 210
public class SpinnerWheel : Signage
{
	// Token: 0x04000BC1 RID: 3009
	public Transform wheel;

	// Token: 0x04000BC2 RID: 3010
	public float velocity;

	// Token: 0x04000BC3 RID: 3011
	public Quaternion targetRotation = Quaternion.identity;

	// Token: 0x04000BC4 RID: 3012
	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	// Token: 0x04000BC5 RID: 3013
	public SoundDefinition spinStartSoundDef;

	// Token: 0x04000BC6 RID: 3014
	public SoundDefinition spinAccentSoundDef;

	// Token: 0x04000BC7 RID: 3015
	public SoundDefinition spinStopSoundDef;

	// Token: 0x04000BC8 RID: 3016
	public float minTimeBetweenSpinAccentSounds = 0.3f;

	// Token: 0x04000BC9 RID: 3017
	public float spinAccentAngleDelta = 180f;

	// Token: 0x04000BCA RID: 3018
	private Sound spinSound;

	// Token: 0x04000BCB RID: 3019
	private SoundModulation.Modulator spinSoundGain;

	// Token: 0x060012C8 RID: 4808 RVA: 0x00096E24 File Offset: 0x00095024
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpinnerWheel.OnRpcMessage", 0))
		{
			if (rpc == 3019675107U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AnyoneSpin ");
				}
				using (TimeWarning.New("RPC_AnyoneSpin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3019675107U, "RPC_AnyoneSpin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_AnyoneSpin(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_AnyoneSpin");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455840454U, "RPC_Spin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Spin(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	// Token: 0x060012CA RID: 4810 RVA: 0x00097124 File Offset: 0x00095324
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.spinnerWheel = Facepunch.Pool.Get<ProtoBuf.SpinnerWheel>();
		info.msg.spinnerWheel.spin = this.wheel.rotation.eulerAngles;
	}

	// Token: 0x060012CB RID: 4811 RVA: 0x0009716C File Offset: 0x0009536C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion rotation = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isServer)
			{
				this.wheel.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x060012CC RID: 4812 RVA: 0x000971BC File Offset: 0x000953BC
	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	// Token: 0x060012CD RID: 4813 RVA: 0x000971C4 File Offset: 0x000953C4
	public virtual void Update_Server()
	{
		if (this.velocity > 0f)
		{
			float num = Mathf.Clamp(this.GetMaxSpinSpeed() * this.velocity, 0f, this.GetMaxSpinSpeed());
			this.velocity -= UnityEngine.Time.deltaTime * Mathf.Clamp(this.velocity / 2f, 0.1f, 1f);
			if (this.velocity < 0f)
			{
				this.velocity = 0f;
			}
			this.wheel.Rotate(Vector3.up, num * UnityEngine.Time.deltaTime, Space.Self);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060012CE RID: 4814 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Update_Client()
	{
	}

	// Token: 0x060012CF RID: 4815 RVA: 0x00097264 File Offset: 0x00095464
	public void Update()
	{
		if (base.isClient)
		{
			this.Update_Client();
		}
		if (base.isServer)
		{
			this.Update_Server();
		}
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x00097284 File Offset: 0x00095484
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.AllowPlayerSpins())
		{
			return;
		}
		if (this.AnyoneSpin() || rpc.player.CanBuild())
		{
			if (this.velocity > 15f)
			{
				return;
			}
			this.velocity += UnityEngine.Random.Range(4f, 7f);
		}
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000972E7 File Offset: 0x000954E7
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_AnyoneSpin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, rpc.read.Bit(), false, true);
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x00030086 File Offset: 0x0002E286
	public bool AnyoneSpin()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}
}
