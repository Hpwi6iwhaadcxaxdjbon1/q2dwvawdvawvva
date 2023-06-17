using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000081 RID: 129
public class HeldEntity : global::BaseEntity
{
	// Token: 0x040007C8 RID: 1992
	public Animator worldModelAnimator;

	// Token: 0x040007C9 RID: 1993
	public SoundDefinition thirdPersonDeploySound;

	// Token: 0x040007CA RID: 1994
	public SoundDefinition thirdPersonAimSound;

	// Token: 0x040007CB RID: 1995
	public SoundDefinition thirdPersonAimEndSound;

	// Token: 0x040007CC RID: 1996
	public const global::BaseEntity.Flags Flag_ForceVisible = global::BaseEntity.Flags.Reserved8;

	// Token: 0x040007CD RID: 1997
	[Header("Held Entity")]
	public string handBone = "r_prop";

	// Token: 0x040007CE RID: 1998
	public AnimatorOverrideController HoldAnimationOverride;

	// Token: 0x040007CF RID: 1999
	public bool isBuildingTool;

	// Token: 0x040007D0 RID: 2000
	[Header("Hostility")]
	public float hostileScore;

	// Token: 0x040007D1 RID: 2001
	public global::HeldEntity.HolsterInfo holsterInfo;

	// Token: 0x040007D2 RID: 2002
	[Header("Camera")]
	public global::BasePlayer.CameraMode HeldCameraMode;

	// Token: 0x040007D3 RID: 2003
	public Vector3 FirstPersonArmOffset;

	// Token: 0x040007D4 RID: 2004
	public Vector3 FirstPersonArmRotation;

	// Token: 0x040007D5 RID: 2005
	[Range(0f, 1f)]
	public float FirstPersonRotationStrength = 1f;

	// Token: 0x040007D6 RID: 2006
	private bool holsterVisible;

	// Token: 0x040007D7 RID: 2007
	private bool genericVisible;

	// Token: 0x040007D8 RID: 2008
	private global::HeldEntity.heldEntityVisState currentVisState;

	// Token: 0x040007D9 RID: 2009
	private TimeSince lastHeldEvent;

	// Token: 0x040007DA RID: 2010
	internal ItemId ownerItemUID;

	// Token: 0x040007DB RID: 2011
	private global::Item cachedItem;

	// Token: 0x06000C18 RID: 3096 RVA: 0x00069E64 File Offset: 0x00068064
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x00069EA4 File Offset: 0x000680A4
	public void SendPunch(Vector3 amount, float duration)
	{
		base.ClientRPCPlayer<Vector3, float>(null, this.GetOwnerPlayer(), "CL_Punch", amount, duration);
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000C1A RID: 3098 RVA: 0x00069EBA File Offset: 0x000680BA
	public bool hostile
	{
		get
		{
			return this.hostileScore > 0f;
		}
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x000238E0 File Offset: 0x00021AE0
	public bool IsDeployed()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x00069ECC File Offset: 0x000680CC
	public global::BasePlayer GetOwnerPlayer()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity.IsValid())
		{
			return null;
		}
		global::BasePlayer basePlayer = parentEntity.ToPlayer();
		if (basePlayer == null)
		{
			return null;
		}
		if (basePlayer.IsDead())
		{
			return null;
		}
		return basePlayer;
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x00069F08 File Offset: 0x00068108
	public Connection GetOwnerConnection()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return null;
		}
		if (ownerPlayer.net == null)
		{
			return null;
		}
		return ownerPlayer.net.connection;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x00069F3C File Offset: 0x0006813C
	public virtual void SetOwnerPlayer(global::BasePlayer player)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		Assert.IsTrue(player.isServer, "Player should be serverside!");
		base.gameObject.Identity();
		base.SetParent(player, this.handBone, false, false);
		this.SetHeld(false);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x00069F8A File Offset: 0x0006818A
	public virtual void ClearOwnerPlayer()
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetParent(null, false, false);
		this.SetHeld(false);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x00069FAC File Offset: 0x000681AC
	public virtual void SetVisibleWhileHolstered(bool visible)
	{
		if (!this.holsterInfo.displayWhenHolstered)
		{
			return;
		}
		this.holsterVisible = visible;
		this.UpdateHeldItemVisibility();
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x00069FC9 File Offset: 0x000681C9
	public virtual void SetGenericVisible(bool wantsVis)
	{
		this.genericVisible = wantsVis;
		base.SetFlag(global::BaseEntity.Flags.Reserved8, wantsVis, false, true);
		this.UpdateHeldItemVisibility();
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x00069FE6 File Offset: 0x000681E6
	public uint GetBone(string bone)
	{
		return StringPool.Get(bone);
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00069FEE File Offset: 0x000681EE
	public virtual void SetLightsOn(bool isOn)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved5, isOn, false, true);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0006A000 File Offset: 0x00068200
	public void UpdateHeldItemVisibility()
	{
		bool flag = false;
		if (this.GetOwnerPlayer())
		{
			bool flag2 = this.GetOwnerPlayer().GetHeldEntity() == this;
			if (!ConVar.Server.showHolsteredItems && !flag2)
			{
				flag = this.UpdateVisiblity_Invis();
			}
			else if (flag2)
			{
				flag = this.UpdateVisibility_Hand();
			}
			else if (this.holsterVisible)
			{
				flag = this.UpdateVisiblity_Holster();
			}
			else
			{
				flag = this.UpdateVisiblity_Invis();
			}
		}
		else if (this.genericVisible)
		{
			flag = this.UpdateVisibility_GenericVis();
		}
		else if (!this.genericVisible)
		{
			flag = this.UpdateVisiblity_Invis();
		}
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0006A094 File Offset: 0x00068294
	public bool UpdateVisibility_Hand()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Hand)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Hand;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		return true;
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0006A0E0 File Offset: 0x000682E0
	public bool UpdateVisibility_GenericVis()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.GenericVis)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.GenericVis;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		return true;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0006A108 File Offset: 0x00068308
	public bool UpdateVisiblity_Holster()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Holster)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Holster;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.holsterInfo.holsterBone), false, false);
		return true;
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0006A15C File Offset: 0x0006835C
	public bool UpdateVisiblity_Invis()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Invis)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Invis;
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		base.limitNetworking = true;
		base.SetFlag(global::BaseEntity.Flags.Disabled, true, false, true);
		return true;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0006A1A8 File Offset: 0x000683A8
	public virtual void SetHeld(bool bHeld)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetFlag(global::BaseEntity.Flags.Reserved4, bHeld, false, true);
		if (!bHeld)
		{
			this.UpdateVisiblity_Invis();
		}
		base.limitNetworking = !bHeld;
		base.SetFlag(global::BaseEntity.Flags.Disabled, !bHeld, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (bHeld && this.lastHeldEvent > 1f && Analytics.Server.Enabled && !this.GetOwnerPlayer().IsNpc)
		{
			Analytics.Server.HeldItemDeployed(this.GetItem().info);
			this.lastHeldEvent = 0f;
		}
		this.OnHeldChanged();
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnHeldChanged()
	{
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000C2C RID: 3116 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsUsableByTurret
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000C2D RID: 3117 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public virtual Transform MuzzleTransform
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool CanBeUsedInWater()
	{
		return false;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool BlocksGestures()
	{
		return false;
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0006A24C File Offset: 0x0006844C
	protected global::Item GetOwnerItem()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null || ownerPlayer.inventory == null)
		{
			return null;
		}
		return ownerPlayer.inventory.FindItemUID(this.ownerItemUID);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0006A28A File Offset: 0x0006848A
	public override global::Item GetItem()
	{
		return this.GetOwnerItem();
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0006A294 File Offset: 0x00068494
	public ItemDefinition GetOwnerItemDefinition()
	{
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			Debug.LogWarning("GetOwnerItem - null!", this);
			return null;
		}
		return ownerItem.info;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ReturnedFromCancelledCraft(global::Item item, global::BasePlayer crafter)
	{
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0006A2BE File Offset: 0x000684BE
	public virtual void SetupHeldEntity(global::Item item)
	{
		this.ownerItemUID = item.uid;
		this.InitOwnerPlayer();
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0006A2D2 File Offset: 0x000684D2
	public global::Item GetCachedItem()
	{
		return this.cachedItem;
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0006A2DA File Offset: 0x000684DA
	public void OnItemChanged(global::Item item)
	{
		this.cachedItem = item;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0006A2E3 File Offset: 0x000684E3
	public override void PostServerLoad()
	{
		this.InitOwnerPlayer();
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0006A2EC File Offset: 0x000684EC
	private void InitOwnerPlayer()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer != null)
		{
			this.SetOwnerPlayer(ownerPlayer);
			return;
		}
		this.ClearOwnerPlayer();
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0006A317 File Offset: 0x00068517
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.heldEntity = Facepunch.Pool.Get<ProtoBuf.HeldEntity>();
		info.msg.heldEntity.itemUID = this.ownerItemUID;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x0006A348 File Offset: 0x00068548
	public void DestroyThis()
	{
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem != null)
		{
			ownerItem.Remove(0f);
		}
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0006A36C File Offset: 0x0006856C
	protected bool HasItemAmount()
	{
		global::Item ownerItem = this.GetOwnerItem();
		return ownerItem != null && ownerItem.amount > 0;
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x0006A390 File Offset: 0x00068590
	protected bool UseItemAmount(int iAmount)
	{
		if (iAmount <= 0)
		{
			return true;
		}
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			this.DestroyThis();
			return true;
		}
		ownerItem.amount -= iAmount;
		ownerItem.MarkDirty();
		if (ownerItem.amount <= 0)
		{
			this.DestroyThis();
			return true;
		}
		return false;
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ServerUse()
	{
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x0006A3DB File Offset: 0x000685DB
	public virtual void ServerUse(float damageModifier, Transform originOverride = null)
	{
		this.ServerUse();
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsInstrument()
	{
		return false;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x0006A3E3 File Offset: 0x000685E3
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.heldEntity != null)
		{
			this.ownerItemUID = info.msg.heldEntity.itemUID;
		}
	}

	// Token: 0x02000BCF RID: 3023
	[Serializable]
	public class HolsterInfo
	{
		// Token: 0x040040E6 RID: 16614
		public global::HeldEntity.HolsterInfo.HolsterSlot slot;

		// Token: 0x040040E7 RID: 16615
		public bool displayWhenHolstered;

		// Token: 0x040040E8 RID: 16616
		public string holsterBone = "spine3";

		// Token: 0x040040E9 RID: 16617
		public Vector3 holsterOffset;

		// Token: 0x040040EA RID: 16618
		public Vector3 holsterRotationOffset;

		// Token: 0x02000FC7 RID: 4039
		public enum HolsterSlot
		{
			// Token: 0x040050C6 RID: 20678
			BACK,
			// Token: 0x040050C7 RID: 20679
			RIGHT_THIGH,
			// Token: 0x040050C8 RID: 20680
			LEFT_THIGH
		}
	}

	// Token: 0x02000BD0 RID: 3024
	public static class HeldEntityFlags
	{
		// Token: 0x040040EB RID: 16619
		public const global::BaseEntity.Flags Deployed = global::BaseEntity.Flags.Reserved4;

		// Token: 0x040040EC RID: 16620
		public const global::BaseEntity.Flags LightsOn = global::BaseEntity.Flags.Reserved5;
	}

	// Token: 0x02000BD1 RID: 3025
	public enum heldEntityVisState
	{
		// Token: 0x040040EE RID: 16622
		UNSET,
		// Token: 0x040040EF RID: 16623
		Invis,
		// Token: 0x040040F0 RID: 16624
		Hand,
		// Token: 0x040040F1 RID: 16625
		Holster,
		// Token: 0x040040F2 RID: 16626
		GenericVis
	}
}
