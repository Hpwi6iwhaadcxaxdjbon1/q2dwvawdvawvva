using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A4 RID: 164
public class NeonSign : Signage
{
	// Token: 0x040009FD RID: 2557
	private const float FastSpeed = 0.5f;

	// Token: 0x040009FE RID: 2558
	private const float MediumSpeed = 1f;

	// Token: 0x040009FF RID: 2559
	private const float SlowSpeed = 2f;

	// Token: 0x04000A00 RID: 2560
	private const float MinSpeed = 0.5f;

	// Token: 0x04000A01 RID: 2561
	private const float MaxSpeed = 5f;

	// Token: 0x04000A02 RID: 2562
	[Header("Neon Sign")]
	public Light topLeft;

	// Token: 0x04000A03 RID: 2563
	public Light topRight;

	// Token: 0x04000A04 RID: 2564
	public Light bottomLeft;

	// Token: 0x04000A05 RID: 2565
	public Light bottomRight;

	// Token: 0x04000A06 RID: 2566
	public float lightIntensity = 2f;

	// Token: 0x04000A07 RID: 2567
	[Range(1f, 100f)]
	public int powerConsumption = 10;

	// Token: 0x04000A08 RID: 2568
	public Material activeMaterial;

	// Token: 0x04000A09 RID: 2569
	public Material inactiveMaterial;

	// Token: 0x04000A0A RID: 2570
	private float animationSpeed = 1f;

	// Token: 0x04000A0B RID: 2571
	private int currentFrame;

	// Token: 0x04000A0C RID: 2572
	private List<ProtoBuf.NeonSign.Lights> frameLighting;

	// Token: 0x04000A0D RID: 2573
	private bool isAnimating;

	// Token: 0x04000A0E RID: 2574
	private Action animationLoopAction;

	// Token: 0x04000A0F RID: 2575
	public AmbienceEmitter ambientSoundEmitter;

	// Token: 0x04000A10 RID: 2576
	public SoundDefinition switchSoundDef;

	// Token: 0x06000F33 RID: 3891 RVA: 0x0007FE48 File Offset: 0x0007E048
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NeonSign.OnRpcMessage", 0))
		{
			if (rpc == 2433901419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationSpeed ");
				}
				using (TimeWarning.New("SetAnimationSpeed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2433901419U, "SetAnimationSpeed", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2433901419U, "SetAnimationSpeed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetAnimationSpeed(rpcmessage);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetAnimationSpeed");
					}
				}
				return true;
			}
			if (rpc == 1919786296U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateNeonColors ");
				}
				using (TimeWarning.New("UpdateNeonColors", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1919786296U, "UpdateNeonColors", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1919786296U, "UpdateNeonColors", this, player, 3f))
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
							this.UpdateNeonColors(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in UpdateNeonColors");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x00080180 File Offset: 0x0007E380
	public override int ConsumptionAmount()
	{
		return this.powerConsumption;
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x00080188 File Offset: 0x0007E388
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.neonSign != null)
		{
			if (this.frameLighting != null)
			{
				foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
				{
					Facepunch.Pool.Free<ProtoBuf.NeonSign.Lights>(ref lights);
				}
				Facepunch.Pool.FreeList<ProtoBuf.NeonSign.Lights>(ref this.frameLighting);
			}
			this.frameLighting = info.msg.neonSign.frameLighting;
			info.msg.neonSign.frameLighting = null;
			this.currentFrame = Mathf.Clamp(info.msg.neonSign.currentFrame, 0, this.paintableSources.Length);
			this.animationSpeed = Mathf.Clamp(info.msg.neonSign.animationSpeed, 0.5f, 5f);
		}
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x00080278 File Offset: 0x0007E478
	public override void ServerInit()
	{
		base.ServerInit();
		this.animationLoopAction = new Action(this.SwitchToNextFrame);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00080292 File Offset: 0x0007E492
	public override void ResetState()
	{
		base.ResetState();
		base.CancelInvoke(this.animationLoopAction);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x000802A8 File Offset: 0x0007E4A8
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (this.paintableSources.Length <= 1)
		{
			return;
		}
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		if (flag && !this.isAnimating)
		{
			if (this.currentFrame != 0)
			{
				this.currentFrame = 0;
				base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
			}
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
			this.isAnimating = true;
			return;
		}
		if (!flag && this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			this.isAnimating = false;
		}
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x00080340 File Offset: 0x0007E540
	private void SwitchToNextFrame()
	{
		int num = this.currentFrame;
		for (int i = 0; i < this.paintableSources.Length; i++)
		{
			this.currentFrame++;
			if (this.currentFrame >= this.paintableSources.Length)
			{
				this.currentFrame = 0;
			}
			if (this.textureIDs[this.currentFrame] != 0U)
			{
				break;
			}
		}
		if (this.currentFrame != num)
		{
			base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
		}
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x000803B8 File Offset: 0x0007E5B8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		List<ProtoBuf.NeonSign.Lights> list = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		if (this.frameLighting != null)
		{
			foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
			{
				list.Add(lights.Copy());
			}
		}
		info.msg.neonSign = Facepunch.Pool.Get<ProtoBuf.NeonSign>();
		info.msg.neonSign.frameLighting = list;
		info.msg.neonSign.currentFrame = this.currentFrame;
		info.msg.neonSign.animationSpeed = this.animationSpeed;
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00080474 File Offset: 0x0007E674
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SetAnimationSpeed(global::BaseEntity.RPCMessage msg)
	{
		float num = Mathf.Clamp(msg.read.Float(), 0.5f, 5f);
		this.animationSpeed = num;
		if (this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x000804D8 File Offset: 0x0007E6D8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void UpdateNeonColors(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num < 0 || num >= this.paintableSources.Length)
		{
			return;
		}
		this.EnsureInitialized();
		this.frameLighting[num].topLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].topRight = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomRight = global::NeonSign.ClampColor(msg.read.Color());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x000805A4 File Offset: 0x0007E7A4
	private void EnsureInitialized()
	{
		if (this.frameLighting == null)
		{
			this.frameLighting = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		}
		while (this.frameLighting.Count < this.paintableSources.Length)
		{
			ProtoBuf.NeonSign.Lights lights = Facepunch.Pool.Get<ProtoBuf.NeonSign.Lights>();
			lights.topLeft = Color.clear;
			lights.topRight = Color.clear;
			lights.bottomLeft = Color.clear;
			lights.bottomRight = Color.clear;
			this.frameLighting.Add(lights);
		}
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x00080619 File Offset: 0x0007E819
	private static Color ClampColor(Color color)
	{
		return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
	}
}
