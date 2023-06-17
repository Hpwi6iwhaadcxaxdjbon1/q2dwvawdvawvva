using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001ED RID: 493
public class BasePet : NPCPlayer, IThinker
{
	// Token: 0x0400128C RID: 4748
	public static Dictionary<ulong, BasePet> ActivePetByOwnerID = new Dictionary<ulong, BasePet>();

	// Token: 0x0400128D RID: 4749
	[ServerVar]
	public static bool queuedMovementsAllowed = true;

	// Token: 0x0400128E RID: 4750
	[ServerVar]
	public static bool onlyQueueBaseNavMovements = true;

	// Token: 0x0400128F RID: 4751
	[ServerVar]
	[Help("How many miliseconds to budget for processing pet movements per frame")]
	public static float movementupdatebudgetms = 1f;

	// Token: 0x04001290 RID: 4752
	public float BaseAttackRate = 2f;

	// Token: 0x04001291 RID: 4753
	public float BaseAttackDamge = 20f;

	// Token: 0x04001292 RID: 4754
	public DamageType AttackDamageType = DamageType.Slash;

	// Token: 0x04001294 RID: 4756
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04001295 RID: 4757
	private BaseEntity _mapMarkerInstance;

	// Token: 0x04001296 RID: 4758
	[HideInInspector]
	public bool inQueue;

	// Token: 0x04001297 RID: 4759
	public static Queue<BasePet> _movementProcessQueue = new Queue<BasePet>();

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x060019E3 RID: 6627 RVA: 0x000BC91D File Offset: 0x000BAB1D
	// (set) Token: 0x060019E4 RID: 6628 RVA: 0x000BC925 File Offset: 0x000BAB25
	public PetBrain Brain { get; protected set; }

	// Token: 0x060019E5 RID: 6629 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x00029E79 File Offset: 0x00028079
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x00029E71 File Offset: 0x00028071
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x000BC930 File Offset: 0x000BAB30
	public static void ProcessMovementQueue()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = BasePet.movementupdatebudgetms / 1000f;
		while (BasePet._movementProcessQueue.Count > 0 && Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BasePet basePet = BasePet._movementProcessQueue.Dequeue();
			if (basePet != null)
			{
				basePet.DoBudgetedMoveUpdate();
				basePet.inQueue = false;
			}
		}
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x000BC98A File Offset: 0x000BAB8A
	public void DoBudgetedMoveUpdate()
	{
		if (this.Brain != null)
		{
			this.Brain.DoMovementTick();
		}
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsLoadBalanced()
	{
		return true;
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x000BC9A5 File Offset: 0x000BABA5
	public override void ServerInit()
	{
		base.ServerInit();
		this.Brain = base.GetComponent<PetBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddPet(this);
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000BC9C8 File Offset: 0x000BABC8
	public void CreateMapMarker()
	{
		if (this._mapMarkerInstance != null)
		{
			this._mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, Vector3.zero, Quaternion.identity, true);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		this._mapMarkerInstance = baseEntity;
	}

	// Token: 0x060019ED RID: 6637 RVA: 0x000BCA39 File Offset: 0x000BAC39
	internal override void DoServerDestroy()
	{
		if (this.Brain.OwningPlayer != null)
		{
			this.Brain.OwningPlayer.ClearClientPetLink();
		}
		AIThinkManager.RemovePet(this);
		base.DoServerDestroy();
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x000BCA6A File Offset: 0x000BAC6A
	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	// Token: 0x060019EF RID: 6639 RVA: 0x000BCA72 File Offset: 0x000BAC72
	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this.Brain.ShouldServerThink())
		{
			this.Brain.DoThink();
		}
	}

	// Token: 0x060019F0 RID: 6640 RVA: 0x000BCA94 File Offset: 0x000BAC94
	public void ApplyPetStatModifiers()
	{
		if (this.inventory == null)
		{
			return;
		}
		for (int i = 0; i < this.inventory.containerWear.capacity; i++)
		{
			Item slot = this.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				ItemModPetStats component = slot.info.GetComponent<ItemModPetStats>();
				if (component != null)
				{
					component.Apply(this);
				}
			}
		}
		this.Heal(this.MaxHealth());
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000BCB08 File Offset: 0x000BAD08
	private void OnPhysicsNeighbourChanged()
	{
		if (this.Brain != null && this.Brain.Navigator != null)
		{
			this.Brain.Navigator.ForceToGround();
		}
	}
}
