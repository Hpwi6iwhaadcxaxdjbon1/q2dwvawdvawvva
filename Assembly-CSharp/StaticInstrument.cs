using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D9 RID: 217
public class StaticInstrument : BaseMountable
{
	// Token: 0x04000C00 RID: 3072
	public AnimatorOverrideController AnimatorOverride;

	// Token: 0x04000C01 RID: 3073
	public bool ShowDeployAnimation;

	// Token: 0x04000C02 RID: 3074
	public InstrumentKeyController KeyController;

	// Token: 0x04000C03 RID: 3075
	public bool ShouldSuppressHandsAnimationLayer;

	// Token: 0x06001329 RID: 4905 RVA: 0x0009A038 File Offset: 0x00098238
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StaticInstrument.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
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
							this.Server_PlayNote(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
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
							this.Server_StopNote(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0009A298 File Offset: 0x00098498
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		float arg4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetMounted());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", arg, arg2, arg3, arg4);
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x0009A2F8 File Offset: 0x000984F8
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", arg, arg2, arg3);
	}

	// Token: 0x0600132C RID: 4908 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsInstrument()
	{
		return true;
	}
}
