using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x04001A2A RID: 6698
	public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;

	// Token: 0x04001A2B RID: 6699
	public List<ItemAmount> containedItems;

	// Token: 0x04001A2C RID: 6700
	public float maxDestroyFractionForFinishBonus = 0.2f;

	// Token: 0x04001A2D RID: 6701
	public List<ItemAmount> finishBonus;

	// Token: 0x04001A2E RID: 6702
	public float fractionRemaining = 1f;

	// Token: 0x04001A2F RID: 6703
	private float categoriesRemaining;

	// Token: 0x04001A30 RID: 6704
	private float startingItemCounts;

	// Token: 0x04001A31 RID: 6705
	private static Dictionary<ResourceDispenser.GatherType, HashSet<int>> cachedResourceItemTypes;

	// Token: 0x060021A4 RID: 8612 RVA: 0x000DAF01 File Offset: 0x000D9101
	public void Start()
	{
		this.Initialize();
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000DAF09 File Offset: 0x000D9109
	public void Initialize()
	{
		this.CacheResourceTypeItems();
		this.UpdateFraction();
		this.UpdateRemainingCategories();
		this.CountAllItems();
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000DAF24 File Offset: 0x000D9124
	private void CacheResourceTypeItems()
	{
		if (ResourceDispenser.cachedResourceItemTypes == null)
		{
			ResourceDispenser.cachedResourceItemTypes = new Dictionary<ResourceDispenser.GatherType, HashSet<int>>();
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(ItemManager.FindItemDefinition("wood").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Tree, hashSet);
			HashSet<int> hashSet2 = new HashSet<int>();
			hashSet2.Add(ItemManager.FindItemDefinition("stones").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("sulfur.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("metal.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("hq.metal.ore").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Ore, hashSet2);
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000DAFD8 File Offset: 0x000D91D8
	public void DoGather(HitInfo info)
	{
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (!info.CanGather || info.DidGather)
		{
			return;
		}
		if (this.gatherType == ResourceDispenser.GatherType.UNSET)
		{
			Debug.LogWarning("Object :" + base.gameObject.name + ": has unset gathertype!");
			return;
		}
		BaseMelee baseMelee = (info.Weapon == null) ? null : (info.Weapon as BaseMelee);
		float num;
		float num2;
		if (baseMelee != null)
		{
			ResourceDispenser.GatherPropertyEntry gatherInfoFromIndex = baseMelee.GetGatherInfoFromIndex(this.gatherType);
			num = gatherInfoFromIndex.gatherDamage * info.gatherScale;
			num2 = gatherInfoFromIndex.destroyFraction;
			if (num == 0f)
			{
				return;
			}
			baseMelee.SendPunch(new Vector3(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-0.25f, -0.5f), 0f) * -30f * (gatherInfoFromIndex.conditionLost / 6f), 0.05f);
			baseMelee.LoseCondition(gatherInfoFromIndex.conditionLost);
			if (!baseMelee.IsValid() || baseMelee.IsBroken())
			{
				return;
			}
			info.DidGather = true;
		}
		else
		{
			num = info.damageTypes.Total();
			num2 = 0.5f;
		}
		float num3 = this.fractionRemaining;
		this.GiveResources(info.InitiatorPlayer, num, num2, info.Weapon);
		this.UpdateFraction();
		float damageAmount;
		if (this.fractionRemaining <= 0f)
		{
			damageAmount = base.baseEntity.MaxHealth();
			if (info.DidGather && num2 < this.maxDestroyFractionForFinishBonus)
			{
				this.AssignFinishBonus(info.InitiatorPlayer, 1f - num2, info.Weapon);
			}
		}
		else
		{
			damageAmount = (num3 - this.fractionRemaining) * base.baseEntity.MaxHealth();
		}
		HitInfo hitInfo = new HitInfo(info.Initiator, base.baseEntity, DamageType.Generic, damageAmount, base.transform.position);
		hitInfo.gatherScale = 0f;
		hitInfo.PointStart = info.PointStart;
		hitInfo.PointEnd = info.PointEnd;
		hitInfo.WeaponPrefab = info.WeaponPrefab;
		hitInfo.Weapon = info.Weapon;
		base.baseEntity.OnAttacked(hitInfo);
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000DB20C File Offset: 0x000D940C
	public void AssignFinishBonus(BasePlayer player, float fraction, AttackEntity weapon)
	{
		base.SendMessage("FinishBonusAssigned", SendMessageOptions.DontRequireReceiver);
		if (fraction <= 0f)
		{
			return;
		}
		if (this.finishBonus != null)
		{
			foreach (ItemAmount itemAmount in this.finishBonus)
			{
				int num = Mathf.CeilToInt((float)((int)itemAmount.amount) * Mathf.Clamp01(fraction));
				int num2 = this.CalculateGatherBonus(player, itemAmount, (float)num);
				Item item = ItemManager.Create(itemAmount.itemDef, num + num2, 0UL);
				if (item != null)
				{
					Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, base.baseEntity, player, weapon);
					player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
			}
		}
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000DB2D8 File Offset: 0x000D94D8
	public void OnAttacked(HitInfo info)
	{
		this.DoGather(info);
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000DB2E4 File Offset: 0x000D94E4
	private void GiveResources(BasePlayer entity, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (!entity.IsValid())
		{
			return;
		}
		if (gatherDamage <= 0f)
		{
			return;
		}
		ItemAmount itemAmount = null;
		int i = this.containedItems.Count;
		int num = UnityEngine.Random.Range(0, this.containedItems.Count);
		while (i > 0)
		{
			if (num >= this.containedItems.Count)
			{
				num = 0;
			}
			if (this.containedItems[num].amount > 0f)
			{
				itemAmount = this.containedItems[num];
				break;
			}
			num++;
			i--;
		}
		if (itemAmount == null)
		{
			return;
		}
		this.GiveResourceFromItem(entity, itemAmount, gatherDamage, destroyFraction, attackWeapon);
		this.UpdateVars();
		if (entity)
		{
			Debug.Assert(attackWeapon.GetItem() != null, "Attack Weapon " + attackWeapon + " has no Item");
			Debug.Assert(attackWeapon.ownerItemUID.IsValid, "Attack Weapon " + attackWeapon + " ownerItemUID is 0");
			Debug.Assert(attackWeapon.GetParentEntity() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is null");
			Debug.Assert(attackWeapon.GetParentEntity().IsValid(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			Debug.Assert(attackWeapon.GetParentEntity().ToPlayer() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is not a player");
			Debug.Assert(!attackWeapon.GetParentEntity().ToPlayer().IsDead(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			BasePlayer ownerPlayer = attackWeapon.GetOwnerPlayer();
			Debug.Assert(ownerPlayer != null, "Attack Weapon " + attackWeapon + " ownerPlayer is null");
			Debug.Assert(ownerPlayer == entity, "Attack Weapon " + attackWeapon + " ownerPlayer is not player");
			if (ownerPlayer != null)
			{
				Debug.Assert(ownerPlayer.inventory != null, "Attack Weapon " + attackWeapon + " ownerPlayer inventory is null");
				Debug.Assert(ownerPlayer.inventory.FindItemUID(attackWeapon.ownerItemUID) != null, "Attack Weapon " + attackWeapon + " FindItemUID is null");
			}
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000DB4FC File Offset: 0x000D96FC
	public void DestroyFraction(float fraction)
	{
		foreach (ItemAmount itemAmount in this.containedItems)
		{
			if (itemAmount.amount > 0f)
			{
				itemAmount.amount -= fraction / this.categoriesRemaining;
			}
		}
		this.UpdateVars();
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000DB570 File Offset: 0x000D9770
	private void GiveResourceFromItem(BasePlayer entity, ItemAmount itemAmt, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (itemAmt.amount == 0f)
		{
			return;
		}
		float num = Mathf.Min(gatherDamage, base.baseEntity.Health()) / base.baseEntity.MaxHealth();
		float num2 = itemAmt.startAmount / this.startingItemCounts;
		float num3 = Mathf.Clamp(itemAmt.startAmount * num / num2, 0f, itemAmt.amount);
		num3 = Mathf.Round(num3);
		float num4 = num3 * destroyFraction * 2f;
		if (itemAmt.amount <= num3 + num4)
		{
			float num5 = (num3 + num4) / itemAmt.amount;
			num3 /= num5;
			num4 /= num5;
		}
		itemAmt.amount -= Mathf.Floor(num3);
		itemAmt.amount -= Mathf.Floor(num4);
		if (num3 < 1f)
		{
			num3 = ((UnityEngine.Random.Range(0f, 1f) <= num3) ? 1f : 0f);
			itemAmt.amount = 0f;
		}
		if (itemAmt.amount < 0f)
		{
			itemAmt.amount = 0f;
		}
		if (num3 >= 1f)
		{
			int num6 = this.CalculateGatherBonus(entity, itemAmt, num3);
			int iAmount = Mathf.FloorToInt(num3) + num6;
			Item item = ItemManager.CreateByItemID(itemAmt.itemid, iAmount, 0UL);
			if (item == null)
			{
				return;
			}
			this.OverrideOwnership(item, attackWeapon);
			Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, base.baseEntity, entity, attackWeapon);
			entity.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
		}
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000DB6E0 File Offset: 0x000D98E0
	private int CalculateGatherBonus(BaseEntity entity, ItemAmount item, float amountToGive)
	{
		if (entity == null)
		{
			return 0;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		if (basePlayer == null)
		{
			return 0;
		}
		if (basePlayer.modifiers == null)
		{
			return 0;
		}
		amountToGive = (float)Mathf.FloorToInt(amountToGive);
		float num = 1f;
		ResourceDispenser.GatherType gatherType = this.gatherType;
		Modifier.ModifierType type;
		if (gatherType != ResourceDispenser.GatherType.Tree)
		{
			if (gatherType != ResourceDispenser.GatherType.Ore)
			{
				return 0;
			}
			type = Modifier.ModifierType.Ore_Yield;
		}
		else
		{
			type = Modifier.ModifierType.Wood_Yield;
		}
		if (!this.IsProducedItemOfGatherType(item))
		{
			return 0;
		}
		num += basePlayer.modifiers.GetValue(type, 0f);
		float num2 = basePlayer.modifiers.GetVariableValue(type, 0f);
		float num3 = (num > 1f) ? Mathf.Max(amountToGive * num - amountToGive, 0f) : 0f;
		num2 += num3;
		int num4 = 0;
		if (num2 >= 1f)
		{
			num4 = (int)num2;
			num2 -= (float)num4;
		}
		basePlayer.modifiers.SetVariableValue(type, num2);
		return num4;
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000DB7C0 File Offset: 0x000D99C0
	private bool IsProducedItemOfGatherType(ItemAmount item)
	{
		if (this.gatherType == ResourceDispenser.GatherType.Tree)
		{
			return ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Tree].Contains(item.itemid);
		}
		return this.gatherType == ResourceDispenser.GatherType.Ore && ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Ore].Contains(item.itemid);
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		return false;
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000DB80D File Offset: 0x000D9A0D
	private void UpdateVars()
	{
		this.UpdateFraction();
		this.UpdateRemainingCategories();
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000DB81C File Offset: 0x000D9A1C
	public void UpdateRemainingCategories()
	{
		int num = 0;
		using (List<ItemAmount>.Enumerator enumerator = this.containedItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.amount > 0f)
				{
					num++;
				}
			}
		}
		this.categoriesRemaining = (float)num;
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000DB884 File Offset: 0x000D9A84
	public void CountAllItems()
	{
		this.startingItemCounts = this.containedItems.Sum((ItemAmount x) => x.startAmount);
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000DB8B8 File Offset: 0x000D9AB8
	private void UpdateFraction()
	{
		float num = this.containedItems.Sum((ItemAmount x) => x.startAmount);
		float num2 = this.containedItems.Sum((ItemAmount x) => x.amount);
		if (num == 0f)
		{
			this.fractionRemaining = 0f;
			return;
		}
		this.fractionRemaining = num2 / num;
	}

	// Token: 0x02000CBF RID: 3263
	public enum GatherType
	{
		// Token: 0x040044B2 RID: 17586
		Tree,
		// Token: 0x040044B3 RID: 17587
		Ore,
		// Token: 0x040044B4 RID: 17588
		Flesh,
		// Token: 0x040044B5 RID: 17589
		UNSET,
		// Token: 0x040044B6 RID: 17590
		LAST
	}

	// Token: 0x02000CC0 RID: 3264
	[Serializable]
	public class GatherPropertyEntry
	{
		// Token: 0x040044B7 RID: 17591
		public float gatherDamage;

		// Token: 0x040044B8 RID: 17592
		public float destroyFraction;

		// Token: 0x040044B9 RID: 17593
		public float conditionLost;
	}

	// Token: 0x02000CC1 RID: 3265
	[Serializable]
	public class GatherProperties
	{
		// Token: 0x040044BA RID: 17594
		public ResourceDispenser.GatherPropertyEntry Tree;

		// Token: 0x040044BB RID: 17595
		public ResourceDispenser.GatherPropertyEntry Ore;

		// Token: 0x040044BC RID: 17596
		public ResourceDispenser.GatherPropertyEntry Flesh;

		// Token: 0x06004FAA RID: 20394 RVA: 0x001A6E24 File Offset: 0x001A5024
		public float GetProficiency()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				float num2 = fromIndex.gatherDamage * fromIndex.destroyFraction;
				if (num2 > 0f)
				{
					num += fromIndex.gatherDamage / num2;
				}
			}
			return num;
		}

		// Token: 0x06004FAB RID: 20395 RVA: 0x001A6E70 File Offset: 0x001A5070
		public bool Any()
		{
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				if (fromIndex.gatherDamage > 0f || fromIndex.conditionLost > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004FAC RID: 20396 RVA: 0x001A6EAE File Offset: 0x001A50AE
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
		{
			return this.GetFromIndex((ResourceDispenser.GatherType)index);
		}

		// Token: 0x06004FAD RID: 20397 RVA: 0x001A6EB7 File Offset: 0x001A50B7
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(ResourceDispenser.GatherType index)
		{
			switch (index)
			{
			case ResourceDispenser.GatherType.Tree:
				return this.Tree;
			case ResourceDispenser.GatherType.Ore:
				return this.Ore;
			case ResourceDispenser.GatherType.Flesh:
				return this.Flesh;
			default:
				return null;
			}
		}
	}
}
