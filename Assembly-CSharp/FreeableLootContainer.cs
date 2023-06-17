using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007A RID: 122
public class FreeableLootContainer : LootContainer
{
	// Token: 0x0400077A RID: 1914
	private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;

	// Token: 0x0400077B RID: 1915
	public Buoyancy buoyancy;

	// Token: 0x0400077C RID: 1916
	public GameObjectRef freedEffect;

	// Token: 0x0400077D RID: 1917
	private Rigidbody rb;

	// Token: 0x0400077E RID: 1918
	public uint skinOverride;

	// Token: 0x06000B76 RID: 2934 RVA: 0x00065FF8 File Offset: 0x000641F8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0))
		{
			if (rpc == 2202685945U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_FreeCrate ");
				}
				using (TimeWarning.New("RPC_FreeCrate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2202685945U, "RPC_FreeCrate", this, player, 3f))
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
							this.RPC_FreeCrate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_FreeCrate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00066160 File Offset: 0x00064360
	public Rigidbody GetRB()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		return this.rb;
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsTiedDown()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x00066184 File Offset: 0x00064384
	public override void ServerInit()
	{
		this.GetRB().isKinematic = true;
		this.buoyancy.buoyancyScale = 0f;
		this.buoyancy.enabled = false;
		base.ServerInit();
		if (this.skinOverride != 0U)
		{
			this.skinID = (ulong)this.skinOverride;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x000661DC File Offset: 0x000643DC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_FreeCrate(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTiedDown())
		{
			return;
		}
		this.GetRB().isKinematic = false;
		this.buoyancy.enabled = true;
		this.buoyancy.buoyancyScale = 1f;
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		if (this.freedEffect.isValid)
		{
			Effect.server.Run(this.freedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		BasePlayer player = msg.player;
		if (player)
		{
			player.ProcessMissionEvent(BaseMission.MissionEventType.FREE_CRATE, "", 1f);
			Analytics.Server.FreeUnderwaterCrate();
			Analytics.Azure.OnFreeUnderwaterCrate(msg.player, this);
		}
	}
}
