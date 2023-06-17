using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C8 RID: 200
public class SearchLight : global::IOEntity
{
	// Token: 0x04000B3B RID: 2875
	public GameObject pitchObject;

	// Token: 0x04000B3C RID: 2876
	public GameObject yawObject;

	// Token: 0x04000B3D RID: 2877
	public GameObject eyePoint;

	// Token: 0x04000B3E RID: 2878
	public SoundPlayer turnLoop;

	// Token: 0x04000B3F RID: 2879
	public bool needsBuildingPrivilegeToUse = true;

	// Token: 0x04000B40 RID: 2880
	private Vector3 aimDir = Vector3.zero;

	// Token: 0x04000B41 RID: 2881
	private global::BasePlayer mountedPlayer;

	// Token: 0x060011E2 RID: 4578 RVA: 0x0009122C File Offset: 0x0008F42C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SearchLight.OnRpcMessage", 0))
		{
			if (rpc == 3611615802U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLight ");
				}
				using (TimeWarning.New("RPC_UseLight", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3611615802U, "RPC_UseLight", this, player, 3f))
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
							this.RPC_UseLight(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLight");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x00091394 File Offset: 0x0008F594
	public override void ResetState()
	{
		this.aimDir = Vector3.zero;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0002364E File Offset: 0x0002184E
	public override int ConsumptionAmount()
	{
		return 10;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x000913A1 File Offset: 0x0008F5A1
	public bool IsMounted()
	{
		return this.mountedPlayer != null;
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x000913AF File Offset: 0x0008F5AF
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Facepunch.Pool.Get<ProtoBuf.AutoTurret>();
		info.msg.autoturret.aimDir = this.aimDir;
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x000913DE File Offset: 0x0008F5DE
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.aimDir = info.msg.autoturret.aimDir;
		}
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0009140A File Offset: 0x0008F60A
	public void PlayerEnter(global::BasePlayer player)
	{
		if (this.IsMounted() && player != this.mountedPlayer)
		{
			return;
		}
		this.PlayerExit();
		if (player != null)
		{
			this.mountedPlayer = player;
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x00091447 File Offset: 0x0008F647
	public void PlayerExit()
	{
		if (this.mountedPlayer)
		{
			this.mountedPlayer = null;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0009146C File Offset: 0x0008F66C
	public void MountedUpdate()
	{
		if (this.mountedPlayer == null || this.mountedPlayer.IsSleeping() || !this.mountedPlayer.IsAlive() || this.mountedPlayer.IsWounded() || Vector3.Distance(this.mountedPlayer.transform.position, base.transform.position) > 2f)
		{
			this.PlayerExit();
			return;
		}
		Vector3 targetAimpoint = this.eyePoint.transform.position + this.mountedPlayer.eyes.BodyForward() * 100f;
		this.SetTargetAimpoint(targetAimpoint);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0009151C File Offset: 0x0008F71C
	public void SetTargetAimpoint(Vector3 worldPos)
	{
		this.aimDir = (worldPos - this.eyePoint.transform.position).normalized;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0009154D File Offset: 0x0008F74D
	public override int GetCurrentEnergy()
	{
		if (this.currentEnergy >= this.ConsumptionAmount())
		{
			return base.GetCurrentEnergy();
		}
		return Mathf.Clamp(this.currentEnergy - base.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x00091580 File Offset: 0x0008F780
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_UseLight(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if (flag && this.IsMounted())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.PlayerEnter(player);
			return;
		}
		this.PlayerExit();
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x000915D3 File Offset: 0x0008F7D3
	public override void OnKilled(HitInfo info)
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.OnKilled(info);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x000915E6 File Offset: 0x0008F7E6
	public void Update()
	{
		if (base.isServer && this.IsMounted())
		{
			this.MountedUpdate();
		}
	}

	// Token: 0x02000BF8 RID: 3064
	public static class SearchLightFlags
	{
		// Token: 0x04004176 RID: 16758
		public const global::BaseEntity.Flags PlayerUsing = global::BaseEntity.Flags.Reserved5;
	}
}
