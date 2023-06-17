using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A2 RID: 162
public class ModularCarOven : BaseOven
{
	// Token: 0x040009C8 RID: 2504
	private BaseVehicleModule moduleParent;

	// Token: 0x06000F09 RID: 3849 RVA: 0x0007EC1C File Offset: 0x0007CE1C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarOven.OnRpcMessage", 0))
		{
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVSwitch ");
				}
				using (TimeWarning.New("SVSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4167839872U, "SVSwitch", this, player, 3f))
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
							this.SVSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0007ED84 File Offset: 0x0007CF84
	private BaseVehicleModule ModuleParent
	{
		get
		{
			if (this.moduleParent != null)
			{
				return this.moduleParent;
			}
			this.moduleParent = (base.GetParentEntity() as BaseVehicleModule);
			return this.moduleParent;
		}
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0007EDB2 File Offset: 0x0007CFB2
	public override void ResetState()
	{
		base.ResetState();
		this.moduleParent = null;
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0007EDC1 File Offset: 0x0007CFC1
	protected override void SVSwitch(BaseEntity.RPCMessage msg)
	{
		if (this.ModuleParent == null || !this.ModuleParent.CanBeLooted(msg.player) || WaterLevel.Test(base.transform.position, true, null))
		{
			return;
		}
		base.SVSwitch(msg);
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0007EE00 File Offset: 0x0007D000
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		return !(this.ModuleParent == null) && this.ModuleParent.CanBeLooted(player) && base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x0007EE29 File Offset: 0x0007D029
	protected override void OnCooked()
	{
		base.OnCooked();
		if (WaterLevel.Test(base.transform.position, true, null))
		{
			this.StopCooking();
		}
	}
}
