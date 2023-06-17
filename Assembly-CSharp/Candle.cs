using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000053 RID: 83
public class Candle : BaseCombatEntity, ISplashable, IIgniteable
{
	// Token: 0x0400061B RID: 1563
	private float lifeTimeSeconds = 7200f;

	// Token: 0x0400061C RID: 1564
	private float burnRate = 10f;

	// Token: 0x06000926 RID: 2342 RVA: 0x00057AC8 File Offset: 0x00055CC8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Candle.OnRpcMessage", 0))
		{
			if (rpc == 2523893445U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetWantsOn ");
				}
				using (TimeWarning.New("SetWantsOn", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2523893445U, "SetWantsOn", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage wantsOn = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetWantsOn(wantsOn);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetWantsOn");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00057C30 File Offset: 0x00055E30
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		base.SetFlag(BaseEntity.Flags.On, b, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00057C59 File Offset: 0x00055E59
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00057C67 File Offset: 0x00055E67
	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.Burn), this.burnRate, this.burnRate, 1f);
			return;
		}
		base.CancelInvoke(new Action(this.Burn));
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00057CA8 File Offset: 0x00055EA8
	public void Burn()
	{
		float num = this.burnRate / this.lifeTimeSeconds;
		base.Hurt(num * this.MaxHealth(), DamageType.Decay, this, false);
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00057CD5 File Offset: 0x00055ED5
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.damageTypes.Get(DamageType.Heat) > 0f && !base.IsOn())
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.UpdateInvokes();
		}
		base.OnAttacked(info);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00057D13 File Offset: 0x00055F13
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && amount > 1 && base.IsOn();
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00057D29 File Offset: 0x00055F29
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (amount > 1)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			this.UpdateInvokes();
			amount--;
		}
		return amount;
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00057D45 File Offset: 0x00055F45
	public void Ignite(Vector3 fromPos)
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.UpdateInvokes();
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00050870 File Offset: 0x0004EA70
	public bool CanIgnite()
	{
		return !base.IsOn();
	}
}
