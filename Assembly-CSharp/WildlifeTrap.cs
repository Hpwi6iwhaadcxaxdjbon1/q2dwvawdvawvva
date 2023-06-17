using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class WildlifeTrap : StorageContainer
{
	// Token: 0x04001D82 RID: 7554
	public float tickRate = 60f;

	// Token: 0x04001D83 RID: 7555
	public GameObjectRef trappedEffect;

	// Token: 0x04001D84 RID: 7556
	public float trappedEffectRepeatRate = 30f;

	// Token: 0x04001D85 RID: 7557
	public float trapSuccessRate = 0.5f;

	// Token: 0x04001D86 RID: 7558
	public List<ItemDefinition> ignoreBait;

	// Token: 0x04001D87 RID: 7559
	public List<WildlifeTrap.WildlifeWeight> targetWildlife;

	// Token: 0x06002536 RID: 9526 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool HasCatch()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsTrapActive()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000898D0 File Offset: 0x00087AD0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000EB248 File Offset: 0x000E9448
	public void SetTrapActive(bool trapOn)
	{
		if (trapOn == this.IsTrapActive())
		{
			return;
		}
		base.CancelInvoke(new Action(this.TrapThink));
		base.SetFlag(BaseEntity.Flags.On, trapOn, false, true);
		if (trapOn)
		{
			base.InvokeRepeating(new Action(this.TrapThink), this.tickRate * 0.8f + this.tickRate * UnityEngine.Random.Range(0f, 0.4f), this.tickRate);
		}
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x000EB2BC File Offset: 0x000E94BC
	public int GetBaitCalories()
	{
		int num = 0;
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
			if (!(component == null) && !this.ignoreBait.Contains(item.info))
			{
				foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
				{
					if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
					{
						num += Mathf.CeilToInt(consumableEffect.amount * (float)item.amount);
					}
				}
			}
		}
		return num;
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x000EB3AC File Offset: 0x000E95AC
	public void DestroyRandomFoodItem()
	{
		int count = base.inventory.itemList.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = num + i;
			if (num2 >= count)
			{
				num2 -= count;
			}
			Item item = base.inventory.itemList[num2];
			if (item != null && !(item.info.GetComponent<ItemModConsumable>() == null))
			{
				item.UseItem(1);
				return;
			}
		}
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x000EB420 File Offset: 0x000E9620
	public void UseBaitCalories(int numToUse)
	{
		foreach (Item item in base.inventory.itemList)
		{
			int itemCalories = this.GetItemCalories(item);
			if (itemCalories > 0)
			{
				numToUse -= itemCalories;
				item.UseItem(1);
				if (numToUse <= 0)
				{
					break;
				}
			}
		}
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x000EB490 File Offset: 0x000E9690
	public int GetItemCalories(Item item)
	{
		ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
		if (component == null)
		{
			return 0;
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
			{
				return Mathf.CeilToInt(consumableEffect.amount);
			}
		}
		return 0;
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000EB51C File Offset: 0x000E971C
	public virtual void TrapThink()
	{
		int baitCalories = this.GetBaitCalories();
		if (baitCalories <= 0)
		{
			return;
		}
		TrappableWildlife randomWildlife = this.GetRandomWildlife();
		if (baitCalories >= randomWildlife.caloriesForInterest && UnityEngine.Random.Range(0f, 1f) <= randomWildlife.successRate)
		{
			this.UseBaitCalories(randomWildlife.caloriesForInterest);
			if (UnityEngine.Random.Range(0f, 1f) <= this.trapSuccessRate)
			{
				this.TrapWildlife(randomWildlife);
			}
		}
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000EB588 File Offset: 0x000E9788
	public void TrapWildlife(TrappableWildlife trapped)
	{
		Item item = ItemManager.Create(trapped.inventoryObject, UnityEngine.Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Remove(0f);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		this.SetTrapActive(false);
		base.Hurt(this.StartMaxHealth() * 0.1f, DamageType.Decay, null, false);
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x00073A10 File Offset: 0x00071C10
	public void ClearTrap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000EB601 File Offset: 0x000E9801
	public bool HasBait()
	{
		return this.GetBaitCalories() > 0;
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000EB60C File Offset: 0x000E980C
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		this.SetTrapActive(this.HasBait());
		this.ClearTrap();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000EB627 File Offset: 0x000E9827
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		this.ClearTrap();
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000EB638 File Offset: 0x000E9838
	public TrappableWildlife GetRandomWildlife()
	{
		int num = this.targetWildlife.Sum((WildlifeTrap.WildlifeWeight x) => x.weight);
		int num2 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < this.targetWildlife.Count; i++)
		{
			num -= this.targetWildlife[i].weight;
			if (num2 >= num)
			{
				return this.targetWildlife[i].wildlife;
			}
		}
		return null;
	}

	// Token: 0x02000CF2 RID: 3314
	public static class WildlifeTrapFlags
	{
		// Token: 0x040045AA RID: 17834
		public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
	}

	// Token: 0x02000CF3 RID: 3315
	[Serializable]
	public class WildlifeWeight
	{
		// Token: 0x040045AB RID: 17835
		public TrappableWildlife wildlife;

		// Token: 0x040045AC RID: 17836
		public int weight;
	}
}
