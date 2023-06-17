using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006D RID: 109
public class DudTimedExplosive : global::TimedExplosive, IIgniteable, ISplashable
{
	// Token: 0x040006FA RID: 1786
	public GameObjectRef fizzleEffect;

	// Token: 0x040006FB RID: 1787
	public GameObject wickSpark;

	// Token: 0x040006FC RID: 1788
	public AudioSource wickSound;

	// Token: 0x040006FD RID: 1789
	public float dudChance = 0.4f;

	// Token: 0x040006FE RID: 1790
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	// Token: 0x040006FF RID: 1791
	[NonSerialized]
	private float explodeTime;

	// Token: 0x04000700 RID: 1792
	public bool becomeDudInWater;

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00061A68 File Offset: 0x0005FC68
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pickup ");
				}
				using (TimeWarning.New("RPC_Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2436818324U, "RPC_Pickup", this, player, 3f))
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
							this.RPC_Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Pickup");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0002A4EC File Offset: 0x000286EC
	private bool IsWickBurning()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x00061BD0 File Offset: 0x0005FDD0
	protected override bool AlwaysRunWaterCheck
	{
		get
		{
			return this.becomeDudInWater;
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00061BD8 File Offset: 0x0005FDD8
	public override void WaterCheck()
	{
		if (!this.becomeDudInWater || this.WaterFactor() < 0.5f)
		{
			base.WaterCheck();
			return;
		}
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.WaterCheck)))
		{
			base.CancelInvoke(new Action(this.WaterCheck));
		}
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00061C7C File Offset: 0x0005FE7C
	public override float GetRandomTimerTime()
	{
		float randomTimerTime = base.GetRandomTimerTime();
		float num = 1f;
		if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 0.334f;
		}
		else if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			num = 3f;
		}
		return randomTimerTime * num;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00061CD4 File Offset: 0x0005FED4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pickup(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsWickBurning())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (UnityEngine.Random.Range(0f, 1f) >= 0.5f && base.HasParent())
		{
			this.SetFuse(UnityEngine.Random.Range(2.5f, 3f));
			return;
		}
		player.GiveItem(ItemManager.Create(this.itemToGive, 1, this.skinID), global::BaseEntity.GiveItemReason.Generic);
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00061D45 File Offset: 0x0005FF45
	public override void SetFuse(float fuseLength)
	{
		base.SetFuse(fuseLength);
		this.explodeTime = UnityEngine.Time.realtimeSinceStartup + fuseLength;
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00061D80 File Offset: 0x0005FF80
	public override void Explode()
	{
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		if (UnityEngine.Random.Range(0f, 1f) < this.dudChance)
		{
			this.BecomeDud();
			return;
		}
		base.Explode();
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00061DD3 File Offset: 0x0005FFD3
	public override bool CanStickTo(global::BaseEntity entity)
	{
		return base.CanStickTo(entity) && this.IsWickBurning();
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00061DE8 File Offset: 0x0005FFE8
	public virtual void BecomeDud()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		bool flag = false;
		EntityRef parentEntity = this.parentEntity;
		while (parentEntity.IsValid(base.isServer) && !flag)
		{
			global::BaseEntity baseEntity = parentEntity.Get(base.isServer);
			if (baseEntity.syncPosition)
			{
				flag = true;
			}
			parentEntity = baseEntity.parentEntity;
		}
		if (flag)
		{
			base.SetParent(null, false, false);
		}
		base.transform.SetPositionAndRotation(position, rotation);
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		this.SetCollisionEnabled(true);
		if (flag)
		{
			this.SetMotionEnabled(true);
		}
		Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(base.KillMessage));
		base.Invoke(new Action(base.KillMessage), 1200f);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x00061EC3 File Offset: 0x000600C3
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.dudExplosive = Facepunch.Pool.Get<DudExplosive>();
		info.msg.dudExplosive.fuseTimeLeft = this.explodeTime - UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00061EF8 File Offset: 0x000600F8
	public void Ignite(Vector3 fromPos)
	{
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00061F32 File Offset: 0x00060132
	public bool CanIgnite()
	{
		return !base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00061F3E File Offset: 0x0006013E
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00061F51 File Offset: 0x00060151
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.BecomeDud();
		if (base.IsInvoking(new Action(this.Explode)))
		{
			base.CancelInvoke(new Action(this.Explode));
		}
		return 1;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00061F82 File Offset: 0x00060182
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.dudExplosive != null)
		{
			this.explodeTime = UnityEngine.Time.realtimeSinceStartup + info.msg.dudExplosive.fuseTimeLeft;
		}
	}
}
