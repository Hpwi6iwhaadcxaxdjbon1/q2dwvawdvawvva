using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008E RID: 142
public class Landmine : BaseTrap
{
	// Token: 0x0400087E RID: 2174
	public GameObjectRef explosionEffect;

	// Token: 0x0400087F RID: 2175
	public GameObjectRef triggeredEffect;

	// Token: 0x04000880 RID: 2176
	public float minExplosionRadius;

	// Token: 0x04000881 RID: 2177
	public float explosionRadius;

	// Token: 0x04000882 RID: 2178
	public bool blocked;

	// Token: 0x04000883 RID: 2179
	private ulong triggerPlayerID;

	// Token: 0x04000884 RID: 2180
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x06000D41 RID: 3393 RVA: 0x00071968 File Offset: 0x0006FB68
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Landmine.OnRpcMessage", 0))
		{
			if (rpc == 1552281787U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Disarm ");
				}
				using (TimeWarning.New("RPC_Disarm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1552281787U, "RPC_Disarm", this, player, 3f))
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
							this.RPC_Disarm(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Disarm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x0002A4F5 File Offset: 0x000286F5
	public bool Triggered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool Armed()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00071AD0 File Offset: 0x0006FCD0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = this.triggerPlayerID;
		}
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00071B07 File Offset: 0x0006FD07
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			this.triggerPlayerID = info.msg.landmine.triggeredID;
		}
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x00071B3B File Offset: 0x0006FD3B
	public override void ServerInit()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.Invoke(new Action(this.Arm), 1.5f);
		base.ServerInit();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x00071B68 File Offset: 0x0006FD68
	public override void ObjectEntered(GameObject obj)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.Armed())
		{
			base.CancelInvoke(new Action(this.Arm));
			this.blocked = true;
			return;
		}
		global::BasePlayer ply = obj.ToBaseEntity() as global::BasePlayer;
		this.Trigger(ply);
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00071BB4 File Offset: 0x0006FDB4
	public void Trigger(global::BasePlayer ply = null)
	{
		if (ply)
		{
			this.triggerPlayerID = ply.userID;
		}
		base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00071BDB File Offset: 0x0006FDDB
	public override void OnEmpty()
	{
		if (this.blocked)
		{
			this.Arm();
			this.blocked = false;
			return;
		}
		if (this.Triggered())
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00071C14 File Offset: 0x0006FE14
	public virtual void Explode()
	{
		base.health = float.PositiveInfinity;
		Effect.server.Run(this.explosionEffect.resourcePath, base.PivotPoint(), base.transform.up, null, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 2263296, true);
		if (base.IsDestroyed)
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00071C89 File Offset: 0x0006FE89
	public override void OnKilled(HitInfo info)
	{
		base.Invoke(new Action(this.Explode), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00071CAD File Offset: 0x0006FEAD
	private void OnGroundMissing()
	{
		this.Explode();
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00071CB5 File Offset: 0x0006FEB5
	private void TryExplode()
	{
		if (this.Armed())
		{
			this.Explode();
		}
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00071CC5 File Offset: 0x0006FEC5
	public override void Arm()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x00071CD8 File Offset: 0x0006FED8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Disarm(global::BaseEntity.RPCMessage rpc)
	{
		if (rpc.player.userID == this.triggerPlayerID)
		{
			return;
		}
		if (!this.Armed())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (UnityEngine.Random.Range(0, 100) < 15)
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
			return;
		}
		rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, 0UL), global::BaseEntity.GiveItemReason.PickedUp);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}
}
