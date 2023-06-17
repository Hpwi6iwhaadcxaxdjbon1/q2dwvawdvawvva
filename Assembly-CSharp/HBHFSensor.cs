using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007F RID: 127
public class HBHFSensor : BaseDetector
{
	// Token: 0x040007C1 RID: 1985
	public GameObjectRef detectUp;

	// Token: 0x040007C2 RID: 1986
	public GameObjectRef detectDown;

	// Token: 0x040007C3 RID: 1987
	public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;

	// Token: 0x040007C4 RID: 1988
	public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;

	// Token: 0x040007C5 RID: 1989
	private int detectedPlayers;

	// Token: 0x06000C01 RID: 3073 RVA: 0x00069504 File Offset: 0x00067704
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HBHFSensor.OnRpcMessage", 0))
		{
			if (rpc == 3206885720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeAuth ");
				}
				using (TimeWarning.New("SetIncludeAuth", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3206885720U, "SetIncludeAuth", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage includeAuth = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetIncludeAuth(includeAuth);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetIncludeAuth");
					}
				}
				return true;
			}
			if (rpc == 2223203375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeOthers ");
				}
				using (TimeWarning.New("SetIncludeOthers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2223203375U, "SetIncludeOthers", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage includeOthers = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetIncludeOthers(includeOthers);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetIncludeOthers");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00069804 File Offset: 0x00067A04
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(this.detectedPlayers, this.GetCurrentEnergy());
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00069817 File Offset: 0x00067A17
	public override void OnObjects()
	{
		base.OnObjects();
		this.UpdatePassthroughAmount();
		base.InvokeRandomized(new Action(this.UpdatePassthroughAmount), 0f, 1f, 0.1f);
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00069846 File Offset: 0x00067A46
	public override void OnEmpty()
	{
		base.OnEmpty();
		this.UpdatePassthroughAmount();
		base.CancelInvoke(new Action(this.UpdatePassthroughAmount));
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x00069868 File Offset: 0x00067A68
	public void UpdatePassthroughAmount()
	{
		if (base.isClient)
		{
			return;
		}
		int num = this.detectedPlayers;
		this.detectedPlayers = 0;
		if (this.myTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.myTrigger.entityContents)
			{
				if (!(baseEntity == null) && baseEntity.IsVisible(base.transform.position + base.transform.forward * 0.1f, 10f))
				{
					BasePlayer component = baseEntity.GetComponent<BasePlayer>();
					bool flag = component.CanBuild();
					if ((!flag || this.ShouldIncludeAuthorized()) && (flag || this.ShouldIncludeOthers()) && component != null && component.IsAlive() && !component.IsSleeping() && component.isServer)
					{
						this.detectedPlayers++;
					}
				}
			}
		}
		if (num != this.detectedPlayers && this.IsPowered())
		{
			this.MarkDirty();
			if (this.detectedPlayers > num)
			{
				Effect.server.Run(this.detectUp.resourcePath, base.transform.position, Vector3.up, null, false);
				return;
			}
			if (this.detectedPlayers < num)
			{
				Effect.server.Run(this.detectDown.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x000699E8 File Offset: 0x00067BE8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeAuth(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, b, false, true);
		}
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00069A24 File Offset: 0x00067C24
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeOthers(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, b, false, true);
		}
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00030086 File Offset: 0x0002E286
	public bool ShouldIncludeAuthorized()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0000564C File Offset: 0x0000384C
	public bool ShouldIncludeOthers()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}
}
