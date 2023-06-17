using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AF RID: 175
public class PlayerLoot : EntityComponent<global::BasePlayer>
{
	// Token: 0x04000A4F RID: 2639
	public global::BaseEntity entitySource;

	// Token: 0x04000A50 RID: 2640
	public global::Item itemSource;

	// Token: 0x04000A51 RID: 2641
	public List<global::ItemContainer> containers = new List<global::ItemContainer>();

	// Token: 0x04000A52 RID: 2642
	internal bool PositionChecks = true;

	// Token: 0x04000A53 RID: 2643
	private bool isInvokingSendUpdate;

	// Token: 0x06001008 RID: 4104 RVA: 0x00085D88 File Offset: 0x00083F88
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerLoot.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00085DC8 File Offset: 0x00083FC8
	public bool IsLooting()
	{
		return this.containers.Count > 0;
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00085DD8 File Offset: 0x00083FD8
	public void Clear()
	{
		if (!this.IsLooting())
		{
			return;
		}
		this.MarkDirty();
		if (this.entitySource)
		{
			this.entitySource.SendMessage("PlayerStoppedLooting", base.baseEntity, SendMessageOptions.DontRequireReceiver);
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			if (itemContainer != null)
			{
				itemContainer.onDirty -= this.MarkDirty;
			}
		}
		this.containers.Clear();
		this.entitySource = null;
		this.itemSource = null;
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x00085E88 File Offset: 0x00084088
	public global::ItemContainer FindContainer(ItemContainerId id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::ItemContainer itemContainer2 = itemContainer.FindContainer(id);
			if (itemContainer2 != null)
			{
				return itemContainer2;
			}
		}
		return null;
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x00085EF4 File Offset: 0x000840F4
	public global::Item FindItem(ItemId id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::Item item = itemContainer.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x00085F68 File Offset: 0x00084168
	public void Check()
	{
		if (!this.IsLooting())
		{
			return;
		}
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (this.entitySource == null)
		{
			base.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
			this.Clear();
			return;
		}
		if (!this.entitySource.CanBeLooted(base.baseEntity))
		{
			this.Clear();
			return;
		}
		if (this.PositionChecks)
		{
			float num = this.entitySource.Distance(base.baseEntity.eyes.position);
			if (num > 3f)
			{
				LootDistanceOverride component = this.entitySource.GetComponent<LootDistanceOverride>();
				if (component == null || num > component.amount)
				{
					this.Clear();
					return;
				}
			}
		}
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0008601C File Offset: 0x0008421C
	private void MarkDirty()
	{
		if (!this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = true;
			base.Invoke(new Action(this.SendUpdate), 0.1f);
		}
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x00086044 File Offset: 0x00084244
	public void SendImmediate()
	{
		if (this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = false;
			base.CancelInvoke(new Action(this.SendUpdate));
		}
		this.SendUpdate();
	}

	// Token: 0x06001010 RID: 4112 RVA: 0x00086070 File Offset: 0x00084270
	private void SendUpdate()
	{
		this.isInvokingSendUpdate = false;
		if (!base.baseEntity.IsValid())
		{
			return;
		}
		using (PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>())
		{
			if (this.entitySource && this.entitySource.net != null)
			{
				playerUpdateLoot.entityID = this.entitySource.net.ID;
			}
			if (this.itemSource != null)
			{
				playerUpdateLoot.itemID = this.itemSource.uid;
			}
			if (this.containers.Count > 0)
			{
				playerUpdateLoot.containers = Pool.Get<List<ProtoBuf.ItemContainer>>();
				foreach (global::ItemContainer itemContainer in this.containers)
				{
					playerUpdateLoot.containers.Add(itemContainer.Save());
				}
			}
			base.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>(null, base.baseEntity, "UpdateLoot", playerUpdateLoot);
		}
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x0008617C File Offset: 0x0008437C
	public bool StartLootingEntity(global::BaseEntity targetEntity, bool doPositionChecks = true)
	{
		this.Clear();
		if (!targetEntity)
		{
			return false;
		}
		if (!targetEntity.OnStartBeingLooted(base.baseEntity))
		{
			return false;
		}
		Assert.IsTrue(targetEntity.isServer, "Assure is server");
		this.PositionChecks = doPositionChecks;
		this.entitySource = targetEntity;
		this.itemSource = null;
		this.MarkDirty();
		ILootableEntity lootableEntity;
		if ((lootableEntity = (targetEntity as ILootableEntity)) != null)
		{
			lootableEntity.LastLootedBy = base.baseEntity.userID;
		}
		return true;
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x000861F0 File Offset: 0x000843F0
	public void AddContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		this.containers.Add(container);
		container.onDirty += this.MarkDirty;
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00086214 File Offset: 0x00084414
	public void RemoveContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		container.onDirty -= this.MarkDirty;
		this.containers.Remove(container);
	}

	// Token: 0x06001014 RID: 4116 RVA: 0x0008623C File Offset: 0x0008443C
	public bool RemoveContainerAt(int index)
	{
		if (index < 0 || index >= this.containers.Count)
		{
			return false;
		}
		if (this.containers[index] != null)
		{
			this.containers[index].onDirty -= this.MarkDirty;
		}
		this.containers.RemoveAt(index);
		return true;
	}

	// Token: 0x06001015 RID: 4117 RVA: 0x00086298 File Offset: 0x00084498
	public void StartLootingItem(global::Item item)
	{
		this.Clear();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		this.PositionChecks = true;
		this.containers.Add(item.contents);
		item.contents.onDirty += this.MarkDirty;
		this.itemSource = item;
		this.entitySource = item.GetWorldEntity();
		this.MarkDirty();
	}
}
