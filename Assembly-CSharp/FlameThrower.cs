using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x02000075 RID: 117
public class FlameThrower : AttackEntity
{
	// Token: 0x0400073C RID: 1852
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	// Token: 0x0400073D RID: 1853
	public int ammo = 100;

	// Token: 0x0400073E RID: 1854
	public ItemDefinition fuelType;

	// Token: 0x0400073F RID: 1855
	public float timeSinceLastAttack;

	// Token: 0x04000740 RID: 1856
	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime;

	// Token: 0x04000741 RID: 1857
	public float flameRange = 10f;

	// Token: 0x04000742 RID: 1858
	public float flameRadius = 2.5f;

	// Token: 0x04000743 RID: 1859
	public ParticleSystem[] flameEffects;

	// Token: 0x04000744 RID: 1860
	public FlameJet jet;

	// Token: 0x04000745 RID: 1861
	public GameObjectRef fireballPrefab;

	// Token: 0x04000746 RID: 1862
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x04000747 RID: 1863
	public SoundDefinition flameStart3P;

	// Token: 0x04000748 RID: 1864
	public SoundDefinition flameLoop3P;

	// Token: 0x04000749 RID: 1865
	public SoundDefinition flameStop3P;

	// Token: 0x0400074A RID: 1866
	public SoundDefinition pilotLoopSoundDef;

	// Token: 0x0400074B RID: 1867
	private float tickRate = 0.25f;

	// Token: 0x0400074C RID: 1868
	private float lastFlameTick;

	// Token: 0x0400074D RID: 1869
	public float fuelPerSec;

	// Token: 0x0400074E RID: 1870
	private float ammoRemainder;

	// Token: 0x0400074F RID: 1871
	public float reloadDuration = 3.5f;

	// Token: 0x04000750 RID: 1872
	private float lastReloadTime = -10f;

	// Token: 0x04000751 RID: 1873
	private float nextFlameTime;

	// Token: 0x06000B0F RID: 2831 RVA: 0x00063C9C File Offset: 0x00061E9C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameThrower.OnRpcMessage", 0))
		{
			if (rpc == 3381353917U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
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
			if (rpc == 3749570935U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetFiring ");
				}
				using (TimeWarning.New("SetFiring", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3749570935U, "SetFiring", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage firing = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFiring(firing);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetFiring");
					}
				}
				return true;
			}
			if (rpc == 1057268396U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TogglePilotLight ");
				}
				using (TimeWarning.New("TogglePilotLight", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1057268396U, "TogglePilotLight", this, player))
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
							this.TogglePilotLight(msg3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in TogglePilotLight");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x000640E8 File Offset: 0x000622E8
	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextReadyTime;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x000640F7 File Offset: 0x000622F7
	private void SetBusyFor(float dur)
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00064106 File Offset: 0x00062306
	private void ClearBusy()
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0006411C File Offset: 0x0006231C
	public void ReduceAmmo(float firingTime)
	{
		if (base.UsingInfiniteAmmoCheat)
		{
			return;
		}
		this.ammoRemainder += this.fuelPerSec * firingTime;
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
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x000641B0 File Offset: 0x000623B0
	public void PilotLightToggle_Shared()
	{
		base.SetFlag(global::BaseEntity.Flags.On, !base.HasFlag(global::BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsPilotOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsFlameOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.OnFire);
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x000641D4 File Offset: 0x000623D4
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x000641E0 File Offset: 0x000623E0
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

	// Token: 0x06000B19 RID: 2841 RVA: 0x0006423C File Offset: 0x0006243C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0005A65B File Offset: 0x0005885B
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0006428C File Offset: 0x0006248C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x000642E0 File Offset: 0x000624E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void SetFiring(global::BaseEntity.RPCMessage msg)
	{
		bool flameState = msg.read.Bit();
		this.SetFlameState(flameState);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x00064300 File Offset: 0x00062500
	public override void ServerUse()
	{
		if (base.IsOnFire())
		{
			return;
		}
		this.SetFlameState(true);
		base.Invoke(new Action(this.StopFlameState), 0.2f);
		base.ServerUse();
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0006432F File Offset: 0x0006252F
	public override void TopUpAmmo()
	{
		this.ammo = this.maxAmmo;
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0006433D File Offset: 0x0006253D
	public override float AmmoFraction()
	{
		return (float)this.ammo / (float)this.maxAmmo;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0006434E File Offset: 0x0006254E
	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadDuration;
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x00064364 File Offset: 0x00062564
	public override bool CanReload()
	{
		return this.ammo < this.maxAmmo;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x00064374 File Offset: 0x00062574
	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadDuration);
		base.GetOwnerPlayer().SignalBroadcast(global::BaseEntity.Signal.Reload, null);
		this.ammo = this.maxAmmo;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x000643AF File Offset: 0x000625AF
	public void StopFlameState()
	{
		this.SetFlameState(false);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000643B8 File Offset: 0x000625B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void DoReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
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

	// Token: 0x06000B25 RID: 2853 RVA: 0x00064448 File Offset: 0x00062648
	public void SetFlameState(bool wantsOn)
	{
		if (wantsOn)
		{
			if (!base.UsingInfiniteAmmoCheat)
			{
				this.ammo--;
			}
			if (this.ammo < 0)
			{
				this.ammo = 0;
			}
		}
		if (wantsOn && this.ammo <= 0)
		{
			wantsOn = false;
		}
		base.SetFlag(global::BaseEntity.Flags.OnFire, wantsOn, false, true);
		if (this.IsFlameOn())
		{
			this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 1f;
			this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
			base.InvokeRepeating(new Action(this.FlameTick), this.tickRate, this.tickRate);
			return;
		}
		base.CancelInvoke(new Action(this.FlameTick));
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x000644EE File Offset: 0x000626EE
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void TogglePilotLight(global::BaseEntity.RPCMessage msg)
	{
		this.PilotLightToggle_Shared();
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x000644F6 File Offset: 0x000626F6
	public override void OnHeldChanged()
	{
		this.SetFlameState(false);
		base.OnHeldChanged();
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00064508 File Offset: 0x00062708
	public void FlameTick()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastFlameTick;
		this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		this.ReduceAmmo(num);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		Ray ray = ownerPlayer.eyes.BodyRay();
		Vector3 origin = ray.origin;
		RaycastHit raycastHit;
		bool flag = UnityEngine.Physics.SphereCast(ray, 0.3f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = origin + ray.direction * this.flameRange;
		}
		float num2 = ownerPlayer.IsNpc ? this.npcDamageScale : 1f;
		float amount = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = amount * num * num2;
		DamageUtil.RadiusDamage(ownerPlayer, base.LookupPrefab(), raycastHit.point - ray.direction * 0.1f, this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2279681, true);
		this.damagePerSec[0].amount = amount;
		if (flag && UnityEngine.Time.realtimeSinceStartup >= this.nextFlameTime && raycastHit.distance > 1.1f)
		{
			this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 0.45f;
			Vector3 point = raycastHit.point;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, point - ray.direction * 0.25f, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = ownerPlayer;
				baseEntity.Spawn();
			}
		}
		if (this.ammo == 0)
		{
			this.SetFlameState(false);
		}
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null && !base.UsingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(num);
		}
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x000646F0 File Offset: 0x000628F0
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
					item2.Drop(player.eyes.position, player.eyes.BodyForward() * 2f, default(Quaternion));
				}
			}
		}
	}
}
