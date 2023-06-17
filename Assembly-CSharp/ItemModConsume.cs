using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
[RequireComponent(typeof(ItemModConsumable))]
public class ItemModConsume : ItemMod
{
	// Token: 0x040024BB RID: 9403
	public GameObjectRef consumeEffect;

	// Token: 0x040024BC RID: 9404
	public string eatGesture = "eat_2hand";

	// Token: 0x040024BD RID: 9405
	[Tooltip("Items that are given on consumption of this item")]
	public ItemAmountRandom[] product;

	// Token: 0x040024BE RID: 9406
	public ItemModConsumable primaryConsumable;

	// Token: 0x06002D2A RID: 11562 RVA: 0x001103D3 File Offset: 0x0010E5D3
	public virtual ItemModConsumable GetConsumable()
	{
		if (this.primaryConsumable)
		{
			return this.primaryConsumable;
		}
		return base.GetComponent<ItemModConsumable>();
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x001103EF File Offset: 0x0010E5EF
	public virtual GameObjectRef GetConsumeEffect()
	{
		return this.consumeEffect;
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x001103F8 File Offset: 0x0010E5F8
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		GameObjectRef gameObjectRef = this.GetConsumeEffect();
		if (gameObjectRef.isValid)
		{
			Vector3 posLocal = player.IsDucked() ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 2f, 0f);
			Effect.server.Run(gameObjectRef.resourcePath, player, 0U, posLocal, Vector3.zero, null, false);
		}
		player.metabolism.MarkConsumption();
		ItemModConsumable consumable = this.GetConsumable();
		if (!string.IsNullOrEmpty(consumable.achievementWhenEaten))
		{
			player.GiveAchievement(consumable.achievementWhenEaten);
		}
		Analytics.Azure.OnConsumableUsed(player, item);
		float num = (float)Mathf.Max(consumable.amountToConsume, 1);
		float num2 = Mathf.Min((float)item.amount, num);
		float num3 = num2 / num;
		float num4 = item.conditionNormalized;
		if (consumable.conditionFractionToLose > 0f)
		{
			num4 = consumable.conditionFractionToLose;
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in consumable.effects)
		{
			if (Mathf.Clamp01(player.healthFraction + player.metabolism.pending_health.Fraction()) <= consumableEffect.onlyIfHealthLessThan)
			{
				if (consumableEffect.type == MetabolismAttribute.Type.Health)
				{
					if (consumableEffect.amount < 0f)
					{
						player.OnAttacked(new HitInfo(player, player, DamageType.Generic, -consumableEffect.amount * num3 * num4, player.transform.position + player.transform.forward * 1f));
					}
					else
					{
						player.health += consumableEffect.amount * num3 * num4;
					}
				}
				else
				{
					player.metabolism.ApplyChange(consumableEffect.type, consumableEffect.amount * num3 * num4, consumableEffect.time * num3 * num4);
				}
			}
		}
		if (player.modifiers != null)
		{
			player.modifiers.Add(consumable.modifiers);
		}
		if (this.product != null)
		{
			foreach (ItemAmountRandom itemAmountRandom in this.product)
			{
				int num5 = Mathf.RoundToInt((float)itemAmountRandom.RandomAmount() * num4);
				if (num5 > 0)
				{
					Item item2 = ItemManager.Create(itemAmountRandom.itemDef, num5, 0UL);
					player.GiveItem(item2, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
		if (string.IsNullOrEmpty(this.eatGesture))
		{
			player.SignalBroadcast(BaseEntity.Signal.Gesture, this.eatGesture, null);
		}
		Analytics.Server.Consume(base.gameObject.name);
		if (consumable.conditionFractionToLose > 0f)
		{
			item.LoseCondition(consumable.conditionFractionToLose * item.maxCondition);
			return;
		}
		item.UseItem((int)num2);
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x001106BC File Offset: 0x0010E8BC
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		return player.metabolism.CanConsume();
	}
}
