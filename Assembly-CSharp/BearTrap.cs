using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004C RID: 76
public class BearTrap : BaseTrap
{
	// Token: 0x04000590 RID: 1424
	protected Animator animator;

	// Token: 0x04000591 RID: 1425
	private GameObject hurtTarget;

	// Token: 0x06000863 RID: 2147 RVA: 0x00051644 File Offset: 0x0004F844
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BearTrap.OnRpcMessage", 0))
		{
			if (rpc == 547827602U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Arm ");
				}
				using (TimeWarning.New("RPC_Arm", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(547827602U, "RPC_Arm", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Arm(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Arm");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x000517AC File Offset: 0x0004F9AC
	public override void InitShared()
	{
		this.animator = base.GetComponent<Animator>();
		base.InitShared();
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x000517C0 File Offset: 0x0004F9C0
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.Armed() && player.CanBuild();
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x000517DB File Offset: 0x0004F9DB
	public override void ServerInit()
	{
		base.ServerInit();
		this.Arm();
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x000517E9 File Offset: 0x0004F9E9
	public override void Arm()
	{
		base.Arm();
		this.RadialResetCorpses(120f);
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000517FC File Offset: 0x0004F9FC
	public void Fire()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0005180F File Offset: 0x0004FA0F
	public override void ObjectEntered(GameObject obj)
	{
		if (!this.Armed())
		{
			return;
		}
		this.hurtTarget = obj;
		base.Invoke(new Action(this.DelayedFire), 0.05f);
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00051838 File Offset: 0x0004FA38
	public void DelayedFire()
	{
		if (this.hurtTarget)
		{
			BaseEntity baseEntity = this.hurtTarget.ToBaseEntity();
			if (baseEntity != null)
			{
				HitInfo hitInfo = new HitInfo(this, baseEntity, DamageType.Bite, 50f, base.transform.position);
				hitInfo.damageTypes.Add(DamageType.Stab, 30f);
				baseEntity.OnAttacked(hitInfo);
			}
			this.hurtTarget = null;
		}
		this.RadialResetCorpses(1800f);
		this.Fire();
		base.Hurt(25f);
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x000518C0 File Offset: 0x0004FAC0
	public void RadialResetCorpses(float duration)
	{
		List<BaseCorpse> list = Facepunch.Pool.GetList<BaseCorpse>();
		global::Vis.Entities<BaseCorpse>(base.transform.position, 5f, list, 512, QueryTriggerInteraction.Collide);
		foreach (BaseCorpse baseCorpse in list)
		{
			baseCorpse.ResetRemovalTime(duration);
		}
		Facepunch.Pool.FreeList<BaseCorpse>(ref list);
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00051938 File Offset: 0x0004FB38
	public override void OnAttacked(HitInfo info)
	{
		float num = info.damageTypes.Total();
		if ((info.damageTypes.IsMeleeType() && num > 20f) || num > 30f)
		{
			this.Fire();
		}
		base.OnAttacked(info);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0005197B File Offset: 0x0004FB7B
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Arm(BaseEntity.RPCMessage rpc)
	{
		if (this.Armed())
		{
			return;
		}
		this.Arm();
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0005198C File Offset: 0x0004FB8C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && this.animator.isInitialized)
		{
			this.animator.SetBool("armed", this.Armed());
		}
	}
}
