using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000092 RID: 146
public class LiquidWeapon : BaseLiquidVessel
{
	// Token: 0x0400089A RID: 2202
	[Header("Liquid Weapon")]
	public float FireRate = 0.2f;

	// Token: 0x0400089B RID: 2203
	public float MaxRange = 10f;

	// Token: 0x0400089C RID: 2204
	public int FireAmountML = 100;

	// Token: 0x0400089D RID: 2205
	public int MaxPressure = 100;

	// Token: 0x0400089E RID: 2206
	public int PressureLossPerTick = 5;

	// Token: 0x0400089F RID: 2207
	public int PressureGainedPerPump = 25;

	// Token: 0x040008A0 RID: 2208
	public float MinDmgRadius = 0.15f;

	// Token: 0x040008A1 RID: 2209
	public float MaxDmgRadius = 0.15f;

	// Token: 0x040008A2 RID: 2210
	public float SplashRadius = 2f;

	// Token: 0x040008A3 RID: 2211
	public GameObjectRef ImpactSplashEffect;

	// Token: 0x040008A4 RID: 2212
	public AnimationCurve PowerCurve;

	// Token: 0x040008A5 RID: 2213
	public List<DamageTypeEntry> Damage;

	// Token: 0x040008A6 RID: 2214
	public LiquidWeaponEffects EntityWeaponEffects;

	// Token: 0x040008A7 RID: 2215
	public bool RequiresPumping;

	// Token: 0x040008A8 RID: 2216
	public bool AutoPump;

	// Token: 0x040008A9 RID: 2217
	public bool WaitForFillAnim;

	// Token: 0x040008AA RID: 2218
	public bool UseFalloffCurve;

	// Token: 0x040008AB RID: 2219
	public AnimationCurve FalloffCurve;

	// Token: 0x040008AC RID: 2220
	public float PumpingBlockDuration = 0.5f;

	// Token: 0x040008AD RID: 2221
	public float StartFillingBlockDuration = 2f;

	// Token: 0x040008AE RID: 2222
	public float StopFillingBlockDuration = 1f;

	// Token: 0x040008AF RID: 2223
	private float cooldownTime;

	// Token: 0x040008B0 RID: 2224
	private int pressure;

	// Token: 0x040008B1 RID: 2225
	public const string RadiationFightAchievement = "SUMMER_RADICAL";

	// Token: 0x040008B2 RID: 2226
	public const string SoakedAchievement = "SUMMER_SOAKED";

	// Token: 0x040008B3 RID: 2227
	public const string LiquidatorAchievement = "SUMMER_LIQUIDATOR";

	// Token: 0x040008B4 RID: 2228
	public const string NoPressureAchievement = "SUMMER_NO_PRESSURE";

	// Token: 0x06000D7F RID: 3455 RVA: 0x00072F8C File Offset: 0x0007118C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidWeapon.OnRpcMessage", 0))
		{
			if (rpc == 1600824953U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PumpWater ");
				}
				using (TimeWarning.New("PumpWater", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1600824953U, "PumpWater", this, player))
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
							this.PumpWater(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in PumpWater");
					}
				}
				return true;
			}
			if (rpc == 3724096303U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartFiring ");
				}
				using (TimeWarning.New("StartFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3724096303U, "StartFiring", this, player))
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
							this.StartFiring(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StartFiring");
					}
				}
				return true;
			}
			if (rpc == 789289044U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopFiring ");
				}
				using (TimeWarning.New("StopFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(789289044U, "StopFiring", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							this.StopFiring();
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in StopFiring");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x000733A4 File Offset: 0x000715A4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StartFiring(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.OnCooldown())
		{
			return;
		}
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		if (!this.CanFire(player))
		{
			return;
		}
		base.CancelInvoke("FireTick");
		base.InvokeRepeating("FireTick", 0f, this.FireRate);
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		this.StartCooldown(this.FireRate);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00073425 File Offset: 0x00071625
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void StopFiring()
	{
		base.CancelInvoke("FireTick");
		if (!this.RequiresPumping)
		{
			this.pressure = this.MaxPressure;
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00073460 File Offset: 0x00071660
	private bool CanFire(global::BasePlayer player)
	{
		if (this.RequiresPumping && this.pressure < this.PressureLossPerTick)
		{
			return false;
		}
		if (player == null)
		{
			return false;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Open))
		{
			return false;
		}
		if (base.AmountHeld() <= 0)
		{
			return false;
		}
		if (!player.CanInteract())
		{
			return false;
		}
		if (!player.CanAttack() || player.IsRunning())
		{
			return false;
		}
		global::Item item = this.GetItem();
		return item != null && item.contents != null;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000734D7 File Offset: 0x000716D7
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void PumpWater(global::BaseEntity.RPCMessage msg)
	{
		this.PumpWater();
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x000734E0 File Offset: 0x000716E0
	private void PumpWater()
	{
		if (base.GetOwnerPlayer() == null)
		{
			return;
		}
		if (this.OnCooldown())
		{
			return;
		}
		if (this.Firing())
		{
			return;
		}
		this.pressure += this.PressureGainedPerPump;
		this.pressure = Mathf.Min(this.pressure, this.MaxPressure);
		this.StartCooldown(this.PumpingBlockDuration);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00073558 File Offset: 0x00071758
	private void FireTick()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!this.CanFire(ownerPlayer))
		{
			this.StopFiring();
			return;
		}
		int num = Mathf.Min(this.FireAmountML, base.AmountHeld());
		if (num == 0)
		{
			this.StopFiring();
			return;
		}
		base.LoseWater(num);
		float currentRange = this.CurrentRange;
		this.pressure -= this.PressureLossPerTick;
		if (this.pressure <= 0)
		{
			this.StopFiring();
		}
		Ray ray = ownerPlayer.eyes.BodyRay();
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * currentRange, Color.blue, 1f);
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(ray, out raycastHit, currentRange, 1218652417))
		{
			this.DoSplash(ownerPlayer, raycastHit.point, ray.direction, num);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00073634 File Offset: 0x00071834
	private void DoSplash(global::BasePlayer attacker, Vector3 position, Vector3 direction, int amount)
	{
		global::Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		global::Item slot = item.contents.GetSlot(0);
		if (slot == null || slot.amount <= 0)
		{
			return;
		}
		if (slot.info == null)
		{
			return;
		}
		WaterBall.DoSplash(position, this.SplashRadius, slot.info, amount);
		DamageUtil.RadiusDamage(attacker, base.LookupPrefab(), position, this.MinDmgRadius, this.MaxDmgRadius, this.Damage, 131072, true);
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x000736B8 File Offset: 0x000718B8
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		this.StopFiring();
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D88 RID: 3464 RVA: 0x000736C6 File Offset: 0x000718C6
	public float PressureFraction
	{
		get
		{
			return (float)this.pressure / (float)this.MaxPressure;
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D89 RID: 3465 RVA: 0x000736D7 File Offset: 0x000718D7
	public float MinimumPressureFraction
	{
		get
		{
			return (float)this.PressureGainedPerPump / (float)this.MaxPressure;
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000D8A RID: 3466 RVA: 0x000736E8 File Offset: 0x000718E8
	public float CurrentRange
	{
		get
		{
			if (!this.UseFalloffCurve)
			{
				return this.MaxRange;
			}
			return this.MaxRange * this.FalloffCurve.Evaluate((float)(this.MaxPressure - this.pressure) / (float)this.MaxPressure);
		}
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00073721 File Offset: 0x00071921
	private void StartCooldown(float duration)
	{
		if (UnityEngine.Time.realtimeSinceStartup + duration > this.cooldownTime)
		{
			this.cooldownTime = UnityEngine.Time.realtimeSinceStartup + duration;
		}
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x0007373F File Offset: 0x0007193F
	private bool OnCooldown()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.cooldownTime;
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x0002A4EC File Offset: 0x000286EC
	private bool Firing()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x00073750 File Offset: 0x00071950
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.pressure;
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x000737A4 File Offset: 0x000719A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.pressure = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}
}
