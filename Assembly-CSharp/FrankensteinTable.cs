using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000079 RID: 121
public class FrankensteinTable : StorageContainer
{
	// Token: 0x04000770 RID: 1904
	public GameObjectRef FrankensteinPrefab;

	// Token: 0x04000771 RID: 1905
	public Transform SpawnLocation;

	// Token: 0x04000772 RID: 1906
	public ItemDefinition WeaponItem;

	// Token: 0x04000773 RID: 1907
	public List<ItemDefinition> HeadItems;

	// Token: 0x04000774 RID: 1908
	public List<ItemDefinition> TorsoItems;

	// Token: 0x04000775 RID: 1909
	public List<ItemDefinition> LegItems;

	// Token: 0x04000776 RID: 1910
	[HideInInspector]
	public List<ItemDefinition> ItemsToUse;

	// Token: 0x04000777 RID: 1911
	public FrankensteinTableVisuals TableVisuals;

	// Token: 0x04000778 RID: 1912
	[Header("Timings")]
	public float TableDownDuration = 0.9f;

	// Token: 0x04000779 RID: 1913
	private bool waking;

	// Token: 0x06000B5C RID: 2908 RVA: 0x00065584 File Offset: 0x00063784
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FrankensteinTable.OnRpcMessage", 0))
		{
			if (rpc == 629197370U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CreateFrankenstein ");
				}
				using (TimeWarning.New("CreateFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(629197370U, "CreateFrankenstein", this, player, 3f))
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
							this.CreateFrankenstein(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in CreateFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 4797457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestSleepFrankenstein ");
				}
				using (TimeWarning.New("RequestSleepFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4797457U, "RequestSleepFrankenstein", this, player, 3f))
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
							this.RequestSleepFrankenstein(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RequestSleepFrankenstein");
					}
				}
				return true;
			}
			if (rpc == 3804893505U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestWakeFrankenstein ");
				}
				using (TimeWarning.New("RequestWakeFrankenstein", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3804893505U, "RequestWakeFrankenstein", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestWakeFrankenstein(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RequestWakeFrankenstein");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x000659E0 File Offset: 0x00063BE0
	public bool IsHeadItem(ItemDefinition itemDef)
	{
		return this.HeadItems.Contains(itemDef);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x000659EE File Offset: 0x00063BEE
	public bool IsTorsoItem(ItemDefinition itemDef)
	{
		return this.TorsoItems.Contains(itemDef);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x000659FC File Offset: 0x00063BFC
	public bool IsLegsItem(ItemDefinition itemDef)
	{
		return this.LegItems.Contains(itemDef);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00065A0A File Offset: 0x00063C0A
	public bool HasValidItems(global::ItemContainer container)
	{
		return this.GetValidItems(container) != null;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00065A18 File Offset: 0x00063C18
	public List<ItemDefinition> GetValidItems(global::ItemContainer container)
	{
		if (container == null)
		{
			return null;
		}
		if (container.itemList == null)
		{
			return null;
		}
		if (container.itemList.Count == 0)
		{
			return null;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		List<ItemDefinition> list = new List<ItemDefinition>();
		for (int i = 0; i < container.capacity; i++)
		{
			global::Item slot = container.GetSlot(i);
			if (slot != null)
			{
				this.CheckItem(slot.info, list, this.HeadItems, ref flag);
				this.CheckItem(slot.info, list, this.TorsoItems, ref flag2);
				this.CheckItem(slot.info, list, this.LegItems, ref flag3);
				if (flag && flag2 && flag3)
				{
					return list;
				}
			}
		}
		return null;
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00065AC0 File Offset: 0x00063CC0
	public bool HasAllValidItems(List<ItemDefinition> items)
	{
		if (items == null)
		{
			return false;
		}
		if (items.Count < 3)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (ItemDefinition itemDefinition in items)
		{
			if (itemDefinition == null)
			{
				return false;
			}
			this.CheckItem(itemDefinition, null, this.HeadItems, ref flag);
			this.CheckItem(itemDefinition, null, this.TorsoItems, ref flag2);
			this.CheckItem(itemDefinition, null, this.LegItems, ref flag3);
		}
		return flag && flag2 && flag3;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00065B68 File Offset: 0x00063D68
	private void CheckItem(ItemDefinition item, List<ItemDefinition> itemList, List<ItemDefinition> validItems, ref bool set)
	{
		if (set)
		{
			return;
		}
		if (validItems.Contains(item))
		{
			set = true;
			if (itemList != null)
			{
				itemList.Add(item);
			}
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00065B88 File Offset: 0x00063D88
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00065BDA File Offset: 0x00063DDA
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00065BEC File Offset: 0x00063DEC
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item != null && ((this.HeadItems != null && this.IsHeadItem(item.info)) || (this.TorsoItems != null && this.IsTorsoItem(item.info)) || (this.LegItems != null && this.IsLegsItem(item.info)));
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void CreateFrankenstein(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00065C47 File Offset: 0x00063E47
	private bool CanStartCreating(global::BasePlayer player)
	{
		return !this.waking && !(player == null) && !(player.PetEntity != null) && this.HasValidItems(base.inventory);
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00065C80 File Offset: 0x00063E80
	private bool IsInventoryEmpty()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			if (base.inventory.GetSlot(i) != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00065CB4 File Offset: 0x00063EB4
	private void ConsumeInventory()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.UseItem(slot.amount);
			}
		}
		ItemManager.DoRemoves();
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00065CF8 File Offset: 0x00063EF8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestWakeFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.WakeFrankenstein(msg.player);
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00065D08 File Offset: 0x00063F08
	private void WakeFrankenstein(global::BasePlayer owner)
	{
		if (owner == null)
		{
			return;
		}
		if (!this.CanStartCreating(owner))
		{
			return;
		}
		this.waking = true;
		base.inventory.SetLocked(true);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.DelayWakeFrankenstein(owner));
		base.ClientRPC(null, "CL_WakeFrankenstein");
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00065D5D File Offset: 0x00063F5D
	private IEnumerator DelayWakeFrankenstein(global::BasePlayer owner)
	{
		yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(this.TableDownDuration);
		if (owner != null && owner.PetEntity != null)
		{
			base.inventory.SetLocked(false);
			base.SendNetworkUpdateImmediate(false);
			this.waking = false;
			yield break;
		}
		this.ItemsToUse = this.GetValidItems(base.inventory);
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.FrankensteinPrefab.resourcePath, this.SpawnLocation.position, this.SpawnLocation.rotation, false);
		baseEntity.enableSaving = false;
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		this.EquipFrankenstein(baseEntity as FrankensteinPet);
		this.ConsumeInventory();
		base.inventory.SetLocked(false);
		base.SendNetworkUpdateImmediate(false);
		base.StartCoroutine(this.WaitForFrankensteinBrainInit(baseEntity as BasePet, owner));
		this.waking = false;
		yield return null;
		yield break;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00065D74 File Offset: 0x00063F74
	private void EquipFrankenstein(FrankensteinPet frank)
	{
		if (this.ItemsToUse == null)
		{
			return;
		}
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		foreach (ItemDefinition template in this.ItemsToUse)
		{
			frank.inventory.GiveItem(ItemManager.Create(template, 1, 0UL), frank.inventory.containerWear, false);
		}
		if (this.WeaponItem != null)
		{
			base.StartCoroutine(frank.DelayEquipWeapon(this.WeaponItem, 1.5f));
		}
		this.ItemsToUse.Clear();
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x00065E34 File Offset: 0x00064034
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RequestSleepFrankenstein(global::BaseEntity.RPCMessage msg)
	{
		this.SleepFrankenstein(msg.player);
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00065E44 File Offset: 0x00064044
	private void SleepFrankenstein(global::BasePlayer owner)
	{
		if (!this.IsInventoryEmpty())
		{
			return;
		}
		if (owner == null)
		{
			return;
		}
		if (owner.PetEntity == null)
		{
			return;
		}
		FrankensteinPet frankensteinPet = owner.PetEntity as FrankensteinPet;
		if (frankensteinPet == null)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, frankensteinPet.transform.position) >= 5f)
		{
			return;
		}
		this.ReturnFrankensteinItems(frankensteinPet);
		ItemManager.DoRemoves();
		base.SendNetworkUpdateImmediate(false);
		frankensteinPet.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00065EC8 File Offset: 0x000640C8
	private void ReturnFrankensteinItems(FrankensteinPet frank)
	{
		if (frank == null)
		{
			return;
		}
		if (frank.inventory == null)
		{
			return;
		}
		if (frank.inventory.containerWear == null)
		{
			return;
		}
		for (int i = 0; i < frank.inventory.containerWear.capacity; i++)
		{
			global::Item slot = frank.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.MoveToContainer(base.inventory, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00065F3E File Offset: 0x0006413E
	private IEnumerator WaitForFrankensteinBrainInit(BasePet frankenstein, global::BasePlayer player)
	{
		yield return new WaitForEndOfFrame();
		frankenstein.ApplyPetStatModifiers();
		frankenstein.Brain.SetOwningPlayer(player);
		frankenstein.CreateMapMarker();
		player.SendClientPetLink();
		yield break;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00065F54 File Offset: 0x00064154
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			return;
		}
		info.msg.FrankensteinTable = Facepunch.Pool.Get<ProtoBuf.FrankensteinTable>();
		info.msg.FrankensteinTable.itemIds = new List<int>();
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				info.msg.FrankensteinTable.itemIds.Add(slot.info.itemid);
			}
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00065FDC File Offset: 0x000641DC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}
}
