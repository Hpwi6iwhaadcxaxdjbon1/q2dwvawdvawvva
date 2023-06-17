using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008A RID: 138
public class Jackhammer : BaseMelee
{
	// Token: 0x04000861 RID: 2145
	public float HotspotBonusScale = 1f;

	// Token: 0x06000D10 RID: 3344 RVA: 0x00070660 File Offset: 0x0006E860
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Jackhammer.OnRpcMessage", 0))
		{
			if (rpc == 1699910227U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetEngineStatus ");
				}
				using (TimeWarning.New("Server_SetEngineStatus", 0))
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
							this.Server_SetEngineStatus(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetEngineStatus");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x0000441C File Offset: 0x0000261C
	public bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00070784 File Offset: 0x0006E984
	[BaseEntity.RPC_Server]
	public void Server_SetEngineStatus(BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(msg.read.Bit());
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00070797 File Offset: 0x0006E997
	public void SetEngineStatus(bool on)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, on, false, true);
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x000707A7 File Offset: 0x0006E9A7
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}
}
