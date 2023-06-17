using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008F RID: 143
public class Lift : AnimatedBuildingBlock
{
	// Token: 0x04000885 RID: 2181
	public GameObjectRef triggerPrefab;

	// Token: 0x04000886 RID: 2182
	public string triggerBone;

	// Token: 0x04000887 RID: 2183
	public float resetDelay = 5f;

	// Token: 0x06000D51 RID: 3409 RVA: 0x00071D64 File Offset: 0x0006FF64
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Lift.OnRpcMessage", 0))
		{
			if (rpc == 2657791441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLift ");
				}
				using (TimeWarning.New("RPC_UseLift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UseLift(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00071ECC File Offset: 0x000700CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		this.MoveUp();
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00071EE2 File Offset: 0x000700E2
	private void MoveUp()
	{
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00071F07 File Offset: 0x00070107
	private void MoveDown()
	{
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00071F2C File Offset: 0x0007012C
	protected override void OnAnimatorDisabled()
	{
		if (base.isServer && base.IsOpen())
		{
			base.Invoke(new Action(this.MoveDown), this.resetDelay);
		}
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00071F58 File Offset: 0x00070158
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}
}
