using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DC RID: 220
public class SurveyCrater : BaseCombatEntity
{
	// Token: 0x04000C1E RID: 3102
	private ResourceDispenser resourceDispenser;

	// Token: 0x06001364 RID: 4964 RVA: 0x0009AF44 File Offset: 0x00099144
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SurveyCrater.OnRpcMessage", 0))
		{
			if (rpc == 3491246334U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnalysisComplete ");
				}
				using (TimeWarning.New("AnalysisComplete", 0))
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
							this.AnalysisComplete(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AnalysisComplete");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x0009B068 File Offset: 0x00099268
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x0009B087 File Offset: 0x00099287
	public override void OnAttacked(HitInfo info)
	{
		bool isServer = base.isServer;
		base.OnAttacked(info);
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	public void AnalysisComplete(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00032C42 File Offset: 0x00030E42
	public override float BoundsPadding()
	{
		return 2f;
	}
}
