using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D5 RID: 213
public class SprayCanSpray : global::DecayEntity, ISplashable
{
	// Token: 0x04000BEA RID: 3050
	public DateTime sprayTimestamp;

	// Token: 0x04000BEB RID: 3051
	public ulong sprayedByPlayer;

	// Token: 0x04000BEC RID: 3052
	public static ListHashSet<SprayCanSpray> AllSprays = new ListHashSet<SprayCanSpray>(8);

	// Token: 0x060012EE RID: 4846 RVA: 0x00098A60 File Offset: 0x00096C60
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray.OnRpcMessage", 0))
		{
			if (rpc == 2774110739U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestWaterClear ");
				}
				using (TimeWarning.New("Server_RequestWaterClear", 0))
				{
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
							this.Server_RequestWaterClear(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RequestWaterClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00098B84 File Offset: 0x00096D84
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.spray == null)
		{
			info.msg.spray = Facepunch.Pool.Get<Spray>();
		}
		info.msg.spray.sprayedBy = this.sprayedByPlayer;
		info.msg.spray.timestamp = this.sprayTimestamp.ToBinary();
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00098BE8 File Offset: 0x00096DE8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spray != null)
		{
			this.sprayedByPlayer = info.msg.spray.sprayedBy;
			this.sprayTimestamp = DateTime.FromBinary(info.msg.spray.timestamp);
		}
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00098C3C File Offset: 0x00096E3C
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.sprayTimestamp = DateTime.Now;
		this.sprayedByPlayer = deployedBy.userID;
		if (ConVar.Global.MaxSpraysPerPlayer > 0 && this.sprayedByPlayer != 0UL)
		{
			int num = -1;
			DateTime now = DateTime.Now;
			int num2 = 0;
			for (int i = 0; i < SprayCanSpray.AllSprays.Count; i++)
			{
				if (SprayCanSpray.AllSprays[i].sprayedByPlayer == this.sprayedByPlayer)
				{
					num2++;
					if (num == -1 || SprayCanSpray.AllSprays[i].sprayTimestamp < now)
					{
						num = i;
						now = SprayCanSpray.AllSprays[i].sprayTimestamp;
					}
				}
			}
			if (num2 >= ConVar.Global.MaxSpraysPerPlayer && num != -1)
			{
				SprayCanSpray.AllSprays[num].Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		if (deployedBy == null || !deployedBy.IsBuildingAuthed())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
		}
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00098D30 File Offset: 0x00096F30
	private void ApplyOutOfAuthConditionPenalty()
	{
		if (!base.IsFullySpawned())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
			return;
		}
		float amount = this.MaxHealth() * (1f - ConVar.Global.SprayOutOfAuthMultiplier);
		base.Hurt(amount, DamageType.Decay, null, true);
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00098D7C File Offset: 0x00096F7C
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RainCheck), 60f, 180f, 30f);
		if (!SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Add(this);
		}
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00098DC8 File Offset: 0x00096FC8
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Remove(this);
		}
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00098DE9 File Offset: 0x00096FE9
	private void RainCheck()
	{
		if (Climate.GetRain(base.transform.position) > 0f && this.IsOutside())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00098E11 File Offset: 0x00097011
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return amount > 0;
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00098E17 File Offset: 0x00097017
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		return 1;
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00098E2C File Offset: 0x0009702C
	[global::BaseEntity.RPC_Server]
	private void Server_RequestWaterClear(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.Menu_WaterClear_ShowIf(player))
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060012F9 RID: 4857 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BypassInsideDecayMultiplier
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00098E5C File Offset: 0x0009705C
	private bool Menu_WaterClear_ShowIf(global::BasePlayer player)
	{
		BaseLiquidVessel baseLiquidVessel;
		return player.GetHeldEntity() != null && (baseLiquidVessel = (player.GetHeldEntity() as BaseLiquidVessel)) != null && baseLiquidVessel.AmountHeld() > 0;
	}
}
