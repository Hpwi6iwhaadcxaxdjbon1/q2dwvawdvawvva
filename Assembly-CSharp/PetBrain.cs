using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class PetBrain : BaseAIBrain
{
	// Token: 0x04000A2E RID: 2606
	[Header("Audio")]
	public SoundDefinition CommandGivenVocalSFX;

	// Token: 0x04000A2F RID: 2607
	[ServerVar]
	public static bool DrownInDeepWater = true;

	// Token: 0x04000A30 RID: 2608
	[ServerVar]
	public static bool IdleWhenOwnerOfflineOrDead = true;

	// Token: 0x04000A31 RID: 2609
	[ServerVar]
	public static bool IdleWhenOwnerMounted = true;

	// Token: 0x04000A32 RID: 2610
	[ServerVar]
	public static float DrownTimer = 15f;

	// Token: 0x04000A33 RID: 2611
	[ReplicatedVar]
	public static float ControlDistance = 100f;

	// Token: 0x04000A34 RID: 2612
	public static int Count;

	// Token: 0x06000F88 RID: 3976 RVA: 0x00082164 File Offset: 0x00080364
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PetBrain.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x000821A4 File Offset: 0x000803A4
	public override void AddStates()
	{
		base.AddStates();
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x000821AC File Offset: 0x000803AC
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		PetBrain.Count++;
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x000821FE File Offset: 0x000803FE
	public override void OnDestroy()
	{
		base.OnDestroy();
		PetBrain.Count--;
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x00082214 File Offset: 0x00080414
	public override void Think(float delta)
	{
		base.Think(delta);
		if (PetBrain.DrownInDeepWater)
		{
			BaseCombatEntity baseCombatEntity = this.GetBaseEntity() as BaseCombatEntity;
			if (baseCombatEntity != null && baseCombatEntity.WaterFactor() > 0.85f && !baseCombatEntity.IsDestroyed)
			{
				baseCombatEntity.Hurt(delta * (baseCombatEntity.MaxHealth() / PetBrain.DrownTimer), DamageType.Drowned, null, true);
			}
		}
		this.EvaluateLoadDefaultDesignTriggers();
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x00082278 File Offset: 0x00080478
	private bool EvaluateLoadDefaultDesignTriggers()
	{
		if (this.loadedDesignIndex == 0)
		{
			return true;
		}
		bool flag = false;
		if (PetBrain.IdleWhenOwnerOfflineOrDead)
		{
			flag = ((PetBrain.IdleWhenOwnerOfflineOrDead && base.OwningPlayer == null) || base.OwningPlayer.IsSleeping() || base.OwningPlayer.IsDead());
		}
		if (PetBrain.IdleWhenOwnerMounted && !flag)
		{
			flag = (base.OwningPlayer != null && base.OwningPlayer.isMounted);
		}
		if (base.OwningPlayer != null && Vector3.Distance(base.transform.position, base.OwningPlayer.transform.position) > PetBrain.ControlDistance)
		{
			flag = true;
		}
		if (flag)
		{
			base.LoadDefaultAIDesign();
			return true;
		}
		return false;
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x00082334 File Offset: 0x00080534
	public override void OnAIDesignLoadedAtIndex(int index)
	{
		base.OnAIDesignLoadedAtIndex(index);
		BaseEntity baseEntity = this.GetBaseEntity();
		if (baseEntity != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(baseEntity.OwnerID);
			if (basePlayer != null)
			{
				basePlayer.SendClientPetStateIndex();
			}
			baseEntity.ClientRPC(null, "OnCommandGiven");
		}
	}
}
