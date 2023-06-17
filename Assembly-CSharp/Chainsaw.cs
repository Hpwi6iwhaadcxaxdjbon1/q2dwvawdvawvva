using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000059 RID: 89
public class Chainsaw : BaseMelee
{
	// Token: 0x04000653 RID: 1619
	public float attackFadeInTime = 0.1f;

	// Token: 0x04000654 RID: 1620
	public float attackFadeInDelay = 0.1f;

	// Token: 0x04000655 RID: 1621
	public float attackFadeOutTime = 0.1f;

	// Token: 0x04000656 RID: 1622
	public float idleFadeInTimeFromOff = 0.1f;

	// Token: 0x04000657 RID: 1623
	public float idleFadeInTimeFromAttack = 0.3f;

	// Token: 0x04000658 RID: 1624
	public float idleFadeInDelay = 0.1f;

	// Token: 0x04000659 RID: 1625
	public float idleFadeOutTime = 0.1f;

	// Token: 0x0400065A RID: 1626
	public Renderer chainRenderer;

	// Token: 0x0400065B RID: 1627
	private MaterialPropertyBlock block;

	// Token: 0x0400065C RID: 1628
	private Vector2 saveST;

	// Token: 0x0400065D RID: 1629
	[Header("Chainsaw")]
	public float fuelPerSec = 1f;

	// Token: 0x0400065E RID: 1630
	public int maxAmmo = 100;

	// Token: 0x0400065F RID: 1631
	public int ammo = 100;

	// Token: 0x04000660 RID: 1632
	public ItemDefinition fuelType;

	// Token: 0x04000661 RID: 1633
	public float reloadDuration = 2.5f;

	// Token: 0x04000662 RID: 1634
	[Header("Sounds")]
	public SoundPlayer idleLoop;

	// Token: 0x04000663 RID: 1635
	public SoundPlayer attackLoopAir;

	// Token: 0x04000664 RID: 1636
	public SoundPlayer revUp;

	// Token: 0x04000665 RID: 1637
	public SoundPlayer revDown;

	// Token: 0x04000666 RID: 1638
	public SoundPlayer offSound;

	// Token: 0x04000667 RID: 1639
	private int failedAttempts;

	// Token: 0x04000668 RID: 1640
	public float engineStartChance = 0.33f;

	// Token: 0x04000669 RID: 1641
	private float ammoRemainder;

	// Token: 0x06000992 RID: 2450 RVA: 0x0005A00C File Offset: 0x0005820C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Chainsaw.OnRpcMessage", 0))
		{
			if (rpc == 3381353917U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoReload ");
				}
				using (TimeWarning.New("DoReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3381353917U, "DoReload", this, player))
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
							this.DoReload(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoReload");
					}
				}
				return true;
			}
			if (rpc == 706698034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetAttacking ");
				}
				using (TimeWarning.New("Server_SetAttacking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(706698034U, "Server_SetAttacking", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_SetAttacking(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_SetAttacking");
					}
				}
				return true;
			}
			if (rpc == 3881794867U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StartEngine ");
				}
				using (TimeWarning.New("Server_StartEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3881794867U, "Server_StartEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StartEngine(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_StartEngine");
					}
				}
				return true;
			}
			if (rpc == 841093980U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopEngine ");
				}
				using (TimeWarning.New("Server_StopEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(841093980U, "Server_StopEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopEngine(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_StopEngine");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool EngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0002A535 File Offset: 0x00028735
	public bool IsAttacking()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0005A5B0 File Offset: 0x000587B0
	public void ServerNPCStart()
	{
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			this.DoReload(default(global::BaseEntity.RPCMessage));
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0005A5FC File Offset: 0x000587FC
	public override void ServerUse()
	{
		base.ServerUse();
		this.SetAttackStatus(true);
		base.Invoke(new Action(this.DelayedStopAttack), this.attackSpacing + 0.5f);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0005A629 File Offset: 0x00058829
	public override void ServerUse_OnHit(HitInfo info)
	{
		this.EnableHitEffect(info.HitMaterial);
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0005A637 File Offset: 0x00058837
	private void DelayedStopAttack()
	{
		this.SetAttackStatus(false);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0005A640 File Offset: 0x00058840
	protected override bool VerifyClientAttack(global::BasePlayer player)
	{
		return this.EngineOn() && this.IsAttacking() && base.VerifyClientAttack(player);
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0005A65B File Offset: 0x0005885B
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0005A66A File Offset: 0x0005886A
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0005A680 File Offset: 0x00058880
	public void ReduceAmmo(float firingTime)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			return;
		}
		this.ammoRemainder += firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
		if ((float)this.ammo <= 0f)
		{
			this.SetEngineStatus(false);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0005A73C File Offset: 0x0005893C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void DoReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (this.IsAttacking())
		{
			return;
		}
		global::Item item;
		while (this.ammo < this.maxAmmo && (item = this.GetAmmo()) != null && item.amount > 0)
		{
			int num = Mathf.Min(this.maxAmmo - this.ammo, item.amount);
			this.ammo += num;
			item.UseItem(num);
		}
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0005A7D0 File Offset: 0x000589D0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0005A824 File Offset: 0x00058A24
	public void SetEngineStatus(bool status)
	{
		base.SetFlag(global::BaseEntity.Flags.On, status, false, true);
		if (!status)
		{
			this.SetAttackStatus(false);
		}
		base.CancelInvoke(new Action(this.EngineTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.EngineTick), 0f, 1f);
		}
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0005A878 File Offset: 0x00058A78
	public void SetAttackStatus(bool status)
	{
		if (!this.EngineOn())
		{
			status = false;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, status, false, true);
		base.CancelInvoke(new Action(this.AttackTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.AttackTick), 0f, 1f);
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0005A8CF File Offset: 0x00058ACF
	public void EngineTick()
	{
		this.ReduceAmmo(0.05f);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0005A8DC File Offset: 0x00058ADC
	public void AttackTick()
	{
		this.ReduceAmmo(this.fuelPerSec);
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0005A8EC File Offset: 0x00058AEC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StartEngine(global::BaseEntity.RPCMessage msg)
	{
		if (this.ammo <= 0 || this.EngineOn())
		{
			return;
		}
		this.ReduceAmmo(0.25f);
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.engineStartChance;
		if (!flag)
		{
			this.failedAttempts++;
		}
		if (flag || this.failedAttempts >= 3)
		{
			this.failedAttempts = 0;
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0005A961 File Offset: 0x00058B61
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StopEngine(global::BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0005A974 File Offset: 0x00058B74
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_SetAttacking(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (this.IsAttacking() == flag)
		{
			return;
		}
		if (!this.EngineOn())
		{
			return;
		}
		this.SetAttackStatus(flag);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0005A9B0 File Offset: 0x00058BB0
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo")
		{
			int num = this.ammo;
			if (num > 0)
			{
				this.ammo = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(this.fuelType, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
			}
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0005AA2A File Offset: 0x00058C2A
	public void DisableHitEffects()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0005AA60 File Offset: 0x00058C60
	public void EnableHitEffect(uint hitMaterial)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		if (hitMaterial == StringPool.Get("Flesh"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
		}
		else if (hitMaterial == StringPool.Get("Wood"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
		}
		base.SendNetworkUpdateImmediate(false);
		base.CancelInvoke(new Action(this.DisableHitEffects));
		base.Invoke(new Action(this.DisableHitEffects), 0.5f);
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0005AB0F File Offset: 0x00058D0F
	public override void DoAttackShared(HitInfo info)
	{
		base.DoAttackShared(info);
		if (base.isServer)
		{
			this.EnableHitEffect(info.HitMaterial);
		}
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0005AB2C File Offset: 0x00058D2C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0005AB7A File Offset: 0x00058D7A
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0005AB88 File Offset: 0x00058D88
	public global::Item GetAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}
}
