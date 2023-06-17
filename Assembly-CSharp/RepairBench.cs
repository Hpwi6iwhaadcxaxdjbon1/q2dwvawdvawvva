using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BD RID: 189
public class RepairBench : StorageContainer
{
	// Token: 0x04000ABD RID: 2749
	public float maxConditionLostOnRepair = 0.2f;

	// Token: 0x04000ABE RID: 2750
	public GameObjectRef skinchangeEffect;

	// Token: 0x04000ABF RID: 2751
	public const float REPAIR_COST_FRACTION = 0.2f;

	// Token: 0x04000AC0 RID: 2752
	private float nextSkinChangeTime;

	// Token: 0x06001117 RID: 4375 RVA: 0x0008C59C File Offset: 0x0008A79C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepairBench.OnRpcMessage", 0))
		{
			if (rpc == 1942825351U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ChangeSkin ");
				}
				using (TimeWarning.New("ChangeSkin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1942825351U, "ChangeSkin", this, player, 3f))
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
							this.ChangeSkin(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ChangeSkin");
					}
				}
				return true;
			}
			if (rpc == 1178348163U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RepairItem ");
				}
				using (TimeWarning.New("RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1178348163U, "RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RepairItem(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RepairItem");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x0008C89C File Offset: 0x0008AA9C
	public static float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0008C8B1 File Offset: 0x0008AAB1
	public static float RepairCostFraction(Item itemToRepair)
	{
		return RepairBench.GetRepairFraction(itemToRepair) * 0.2f;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x0008C8C0 File Offset: 0x0008AAC0
	public static void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
		}
		RepairBench.StripComponentRepairCost(allIngredients);
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x0008C92C File Offset: 0x0008AB2C
	public static void StripComponentRepairCost(List<ItemAmount> allIngredients)
	{
		if (allIngredients == null)
		{
			return;
		}
		for (int i = 0; i < allIngredients.Count; i++)
		{
			ItemAmount itemAmount = allIngredients[i];
			if (itemAmount.itemDef.category == ItemCategory.Component)
			{
				if (itemAmount.itemDef.Blueprint != null)
				{
					bool flag = false;
					ItemAmount itemAmount2 = itemAmount.itemDef.Blueprint.ingredients[0];
					foreach (ItemAmount itemAmount3 in allIngredients)
					{
						if (itemAmount3.itemDef == itemAmount2.itemDef)
						{
							itemAmount3.amount += itemAmount2.amount * itemAmount.amount;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						allIngredients.Add(new ItemAmount(itemAmount2.itemDef, itemAmount2.amount * itemAmount.amount));
					}
				}
				allIngredients.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x0008CA38 File Offset: 0x0008AC38
	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning(toPrint);
		}
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x0008CA48 File Offset: 0x0008AC48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ChangeSkin(BaseEntity.RPCMessage msg)
	{
		if (UnityEngine.Time.realtimeSinceStartup < this.nextSkinChangeTime)
		{
			return;
		}
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		bool flag = false;
		if (msg.player.UnlockAllSkins)
		{
			flag = true;
		}
		if (num != 0 && !flag && !player.blueprints.CheckSkinOwnership(num, player.userID))
		{
			this.debugprint("RepairBench.ChangeSkin player does not have item :" + num + ":");
			return;
		}
		ulong Skin = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (Skin == slot.skin && slot.info.isRedirectOf == null)
		{
			this.debugprint(string.Concat(new object[]
			{
				"RepairBench.ChangeSkin cannot apply same skin twice : ",
				Skin,
				": ",
				slot.skin
			}));
			return;
		}
		this.nextSkinChangeTime = UnityEngine.Time.realtimeSinceStartup + 0.75f;
		ItemSkinDirectory.Skin skin = slot.info.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		if (slot.info.isRedirectOf != null)
		{
			Skin = ItemDefinition.FindSkin(slot.info.isRedirectOf.itemid, num);
			skin = slot.info.isRedirectOf.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (long)x.id == (long)Skin);
		}
		ItemSkin itemSkin = (skin.id == 0) ? null : (skin.invItem as ItemSkin);
		if ((itemSkin && (itemSkin.Redirect != null || slot.info.isRedirectOf != null)) || (!itemSkin && slot.info.isRedirectOf != null))
		{
			ItemDefinition template = (itemSkin != null) ? itemSkin.Redirect : slot.info.isRedirectOf;
			bool flag2 = false;
			if (itemSkin != null && itemSkin.Redirect == null && slot.info.isRedirectOf != null)
			{
				template = slot.info.isRedirectOf;
				flag2 = (num != 0);
			}
			float condition = slot.condition;
			float maxCondition = slot.maxCondition;
			int amount = slot.amount;
			int contents = 0;
			ItemDefinition ammoType = null;
			BaseProjectile baseProjectile;
			if (slot.GetHeldEntity() != null && (baseProjectile = (slot.GetHeldEntity() as BaseProjectile)) != null && baseProjectile.primaryMagazine != null)
			{
				contents = baseProjectile.primaryMagazine.contents;
				ammoType = baseProjectile.primaryMagazine.ammoType;
			}
			List<Item> list = Facepunch.Pool.GetList<Item>();
			if (slot.contents != null && slot.contents.itemList != null && slot.contents.itemList.Count > 0)
			{
				foreach (Item item in slot.contents.itemList)
				{
					list.Add(item);
				}
				foreach (Item item2 in list)
				{
					item2.RemoveFromContainer();
				}
			}
			slot.Remove(0f);
			ItemManager.DoRemoves();
			Item item3 = ItemManager.Create(template, 1, 0UL);
			item3.MoveToContainer(base.inventory, 0, false, false, null, true);
			item3.maxCondition = maxCondition;
			item3.condition = condition;
			item3.amount = amount;
			BaseProjectile baseProjectile2;
			if (item3.GetHeldEntity() != null && (baseProjectile2 = (item3.GetHeldEntity() as BaseProjectile)) != null)
			{
				if (baseProjectile2.primaryMagazine != null)
				{
					baseProjectile2.primaryMagazine.contents = contents;
					baseProjectile2.primaryMagazine.ammoType = ammoType;
				}
				baseProjectile2.ForceModsChanged();
			}
			if (list.Count > 0 && item3.contents != null)
			{
				foreach (Item item4 in list)
				{
					item4.MoveToContainer(item3.contents, -1, true, false, null, true);
				}
			}
			Facepunch.Pool.FreeList<Item>(ref list);
			if (flag2)
			{
				this.ApplySkinToItem(item3, Skin);
			}
			Analytics.Server.SkinUsed(item3.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, item3, Skin);
		}
		else
		{
			this.ApplySkinToItem(slot, Skin);
			Analytics.Server.SkinUsed(slot.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, slot, Skin);
		}
		if (this.skinchangeEffect.isValid)
		{
			Effect.server.Run(this.skinchangeEffect.resourcePath, this, 0U, new Vector3(0f, 1.5f, 0f), Vector3.zero, null, false);
		}
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x0008CF4C File Offset: 0x0008B14C
	private void ApplySkinToItem(Item item, ulong Skin)
	{
		item.skin = Skin;
		item.MarkDirty();
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.skinID = Skin;
			heldEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x0008CF84 File Offset: 0x0008B184
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RepairItem(BaseEntity.RPCMessage msg)
	{
		Item slot = base.inventory.GetSlot(0);
		BasePlayer player = msg.player;
		RepairBench.RepairAnItem(slot, player, this, this.maxConditionLostOnRepair, true);
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		return 0;
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0008CFB4 File Offset: 0x0008B1B4
	public static void RepairAnItem(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
	{
		if (itemToRepair == null)
		{
			return;
		}
		ItemDefinition info = itemToRepair.info;
		ItemBlueprint component = info.GetComponent<ItemBlueprint>();
		if (!component)
		{
			return;
		}
		if (!info.condition.repairable)
		{
			return;
		}
		if (itemToRepair.condition == itemToRepair.maxCondition)
		{
			return;
		}
		if (mustKnowBlueprint)
		{
			ItemDefinition itemDefinition = (info.isRedirectOf != null) ? info.isRedirectOf : info;
			if (!player.blueprints.HasUnlocked(itemDefinition) && (!(itemDefinition.Blueprint != null) || itemDefinition.Blueprint.isResearchable))
			{
				return;
			}
		}
		float num = RepairBench.RepairCostFraction(itemToRepair);
		bool flag = false;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		RepairBench.GetRepairCostList(component, list);
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category != ItemCategory.Component)
			{
				int amount = player.inventory.GetAmount(itemAmount.itemDef.itemid);
				if (Mathf.CeilToInt(itemAmount.amount * num) > amount)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Facepunch.Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount itemAmount2 in list)
		{
			if (itemAmount2.itemDef.category != ItemCategory.Component)
			{
				int amount2 = Mathf.CeilToInt(itemAmount2.amount * num);
				player.inventory.Take(null, itemAmount2.itemid, amount2);
				Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "repair", itemAmount2.itemDef.shortname, amount2, repairBenchEntity, null, false, null, 0UL, null, itemToRepair, null);
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		float conditionNormalized = itemToRepair.conditionNormalized;
		float maxConditionNormalized = itemToRepair.maxConditionNormalized;
		itemToRepair.DoRepair(maxConditionLostOnRepair);
		Analytics.Azure.OnItemRepaired(player, repairBenchEntity, itemToRepair, conditionNormalized, maxConditionNormalized);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Item repaired! condition : ",
				itemToRepair.condition,
				"/",
				itemToRepair.maxCondition
			}));
		}
		Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", repairBenchEntity, 0U, Vector3.zero, Vector3.zero, null, false);
	}
}
