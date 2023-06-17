using System;
using ConVar;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x0200007E RID: 126
public class HackableLockedCrate : LootContainer
{
	// Token: 0x040007B2 RID: 1970
	public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;

	// Token: 0x040007B3 RID: 1971
	public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;

	// Token: 0x040007B4 RID: 1972
	public Text timerText;

	// Token: 0x040007B5 RID: 1973
	[ServerVar(Help = "How many seconds for the crate to unlock")]
	public static float requiredHackSeconds = 900f;

	// Token: 0x040007B6 RID: 1974
	[ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds = 7200f;

	// Token: 0x040007B7 RID: 1975
	public SoundPlayer hackProgressBeep;

	// Token: 0x040007B8 RID: 1976
	private float hackSeconds;

	// Token: 0x040007B9 RID: 1977
	public GameObjectRef shockEffect;

	// Token: 0x040007BA RID: 1978
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040007BB RID: 1979
	public GameObjectRef landEffect;

	// Token: 0x040007BC RID: 1980
	public bool shouldDecay = true;

	// Token: 0x040007BD RID: 1981
	[NonSerialized]
	public ulong OriginalHackerPlayer;

	// Token: 0x040007BE RID: 1982
	private BaseEntity mapMarkerInstance;

	// Token: 0x040007BF RID: 1983
	private bool hasLanded;

	// Token: 0x040007C0 RID: 1984
	private bool wasDropped;

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00068FC0 File Offset: 0x000671C0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0))
		{
			if (rpc == 888500940U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Hack ");
				}
				using (TimeWarning.New("RPC_Hack", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(888500940U, "RPC_Hack", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Hack(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Hack");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool IsBeingHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0000564C File Offset: 0x0000384C
	public bool IsFullyHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00069128 File Offset: 0x00067328
	public override void DestroyShared()
	{
		if (base.isServer && this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.DestroyShared();
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00069154 File Offset: 0x00067354
	public void CreateMapMarker(float durationMinutes)
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		baseEntity.transform.localPosition = Vector3.zero;
		baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.mapMarkerInstance = baseEntity;
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x000691CE File Offset: 0x000673CE
	public void RefreshDecay()
	{
		base.CancelInvoke(new Action(this.DelayedDestroy));
		if (this.shouldDecay)
		{
			base.Invoke(new Action(this.DelayedDestroy), HackableLockedCrate.decaySeconds);
		}
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00069204 File Offset: 0x00067404
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			if (StringPool.Get(info.HitBone) == "laptopcollision")
			{
				Effect.server.Run(this.shockEffect.resourcePath, info.HitPositionWorld, Vector3.up, null, false);
				this.hackSeconds -= 8f * (info.damageTypes.Total() / 50f);
				if (this.hackSeconds < 0f)
				{
					this.hackSeconds = 0f;
				}
			}
			this.RefreshDecay();
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00069296 File Offset: 0x00067496
	public void SetWasDropped()
	{
		this.wasDropped = true;
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x000692A0 File Offset: 0x000674A0
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (!Rust.Application.isLoadingSave)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			if (this.wasDropped)
			{
				base.InvokeRepeating(new Action(this.LandCheck), 0f, 0.015f);
			}
			Analytics.Azure.OnEntitySpawned(this);
		}
		this.RefreshDecay();
		this.isLootable = this.IsFullyHacked();
		this.CreateMapMarker(120f);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00069328 File Offset: 0x00067528
	public void LandCheck()
	{
		if (this.hasLanded)
		{
			return;
		}
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(new Ray(base.transform.position + Vector3.up * 0.5f, Vector3.down), out raycastHit, 1f, 1218511105))
		{
			Effect.server.Run(this.landEffect.resourcePath, raycastHit.point, Vector3.up, null, false);
			this.hasLanded = true;
			base.CancelInvoke(new Action(this.LandCheck));
		}
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x000693B1 File Offset: 0x000675B1
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000693C7 File Offset: 0x000675C7
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Hack(BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingHacked())
		{
			return;
		}
		Analytics.Azure.OnLockedCrateStarted(msg.player, this);
		this.OriginalHackerPlayer = msg.player.userID;
		this.StartHacking();
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x000693F8 File Offset: 0x000675F8
	public void StartHacking()
	{
		base.BroadcastEntityMessage("HackingStarted", 20f, 256);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		base.InvokeRepeating(new Action(this.HackProgress), 1f, 1f);
		base.ClientRPC<int, int>(null, "UpdateHackProgress", 0, (int)HackableLockedCrate.requiredHackSeconds);
		this.RefreshDecay();
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00069460 File Offset: 0x00067660
	public void HackProgress()
	{
		this.hackSeconds += 1f;
		if (this.hackSeconds > HackableLockedCrate.requiredHackSeconds)
		{
			Analytics.Azure.OnLockedCrateFinished(this.OriginalHackerPlayer, this);
			this.RefreshDecay();
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.isLootable = true;
			base.CancelInvoke(new Action(this.HackProgress));
		}
		base.ClientRPC<int, int>(null, "UpdateHackProgress", (int)this.hackSeconds, (int)HackableLockedCrate.requiredHackSeconds);
	}
}
