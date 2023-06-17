using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x02000074 RID: 116
public class ExcavatorSignalComputer : BaseCombatEntity
{
	// Token: 0x04000731 RID: 1841
	public float chargePower;

	// Token: 0x04000732 RID: 1842
	public const global::BaseEntity.Flags Flag_Ready = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000733 RID: 1843
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000734 RID: 1844
	public GameObjectRef supplyPlanePrefab;

	// Token: 0x04000735 RID: 1845
	public Transform[] dropPoints;

	// Token: 0x04000736 RID: 1846
	public Text statusText;

	// Token: 0x04000737 RID: 1847
	public Text timerText;

	// Token: 0x04000738 RID: 1848
	public static readonly Translate.Phrase readyphrase = new Translate.Phrase("excavator.signal.ready", "READY");

	// Token: 0x04000739 RID: 1849
	public static readonly Translate.Phrase chargephrase = new Translate.Phrase("excavator.signal.charging", "COMSYS CHARGING");

	// Token: 0x0400073A RID: 1850
	[ServerVar]
	public static float chargeNeededForSupplies = 600f;

	// Token: 0x0400073B RID: 1851
	private float lastChargeTime;

	// Token: 0x06000B04 RID: 2820 RVA: 0x00063818 File Offset: 0x00061A18
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ExcavatorSignalComputer.OnRpcMessage", 0))
		{
			if (rpc == 1824723998U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestSupplies ");
				}
				using (TimeWarning.New("RequestSupplies", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1824723998U, "RequestSupplies", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1824723998U, "RequestSupplies", this, player, 3f))
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
							this.RequestSupplies(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RequestSupplies");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x000639D8 File Offset: 0x00061BD8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.chargePower;
		info.msg.ioEntity.genericFloat2 = ExcavatorSignalComputer.chargeNeededForSupplies;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00063A27 File Offset: 0x00061C27
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastChargeTime = UnityEngine.Time.time;
		base.InvokeRepeating(new Action(this.ChargeThink), 0f, 1f);
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00063A56 File Offset: 0x00061C56
	public override void PostServerLoad()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x00063A74 File Offset: 0x00061C74
	public void ChargeThink()
	{
		float num = this.chargePower;
		float num2 = UnityEngine.Time.time - this.lastChargeTime;
		this.lastChargeTime = UnityEngine.Time.time;
		if (this.IsPowered())
		{
			this.chargePower += num2;
		}
		this.chargePower = Mathf.Clamp(this.chargePower, 0f, ExcavatorSignalComputer.chargeNeededForSupplies);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, this.chargePower >= ExcavatorSignalComputer.chargeNeededForSupplies, false, true);
		if (num != this.chargePower)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x00063AFC File Offset: 0x00061CFC
	public override void OnEntityMessage(global::BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "DieselEngineOn")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
			return;
		}
		if (msg == "DieselEngineOff")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x00063B48 File Offset: 0x00061D48
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void RequestSupplies(global::BaseEntity.RPCMessage rpc)
	{
		if (base.HasFlag(global::BaseEntity.Flags.Reserved7) && this.IsPowered() && this.chargePower >= ExcavatorSignalComputer.chargeNeededForSupplies)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.supplyPlanePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				Vector3 position = this.dropPoints[UnityEngine.Random.Range(0, this.dropPoints.Length)].position;
				Vector3 b = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
				baseEntity.SendMessage("InitDropPosition", position + b, SendMessageOptions.DontRequireReceiver);
				baseEntity.Spawn();
			}
			this.chargePower -= ExcavatorSignalComputer.chargeNeededForSupplies;
			base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x00063C3C File Offset: 0x00061E3C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.chargePower = info.msg.ioEntity.genericFloat1;
		}
	}
}
