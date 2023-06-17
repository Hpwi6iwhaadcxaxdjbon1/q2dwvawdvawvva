using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020003D7 RID: 983
public class PercentFullStorageContainer : StorageContainer
{
	// Token: 0x04001A3F RID: 6719
	private float prevPercentFull = -1f;

	// Token: 0x060021CC RID: 8652 RVA: 0x000DBD92 File Offset: 0x000D9F92
	public bool IsFull()
	{
		return this.GetPercentFull() == 1f;
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000DBDA1 File Offset: 0x000D9FA1
	public bool IsEmpty()
	{
		return this.GetPercentFull() == 0f;
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnPercentFullChanged(float newPercentFull)
	{
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000DBDB0 File Offset: 0x000D9FB0
	public float GetPercentFull()
	{
		if (base.isServer)
		{
			float num = 0f;
			if (base.inventory != null)
			{
				foreach (global::Item item in base.inventory.itemList)
				{
					num += (float)item.amount / (float)item.MaxStackable();
				}
				num /= (float)base.inventory.capacity;
			}
			return num;
		}
		return 0f;
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000DBE40 File Offset: 0x000DA040
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		SimpleFloat simpleFloat = info.msg.simpleFloat;
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000DBE55 File Offset: 0x000DA055
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleFloat = Pool.Get<SimpleFloat>();
		info.msg.simpleFloat.value = this.GetPercentFull();
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000DBE84 File Offset: 0x000DA084
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float percentFull = this.GetPercentFull();
		if (percentFull != this.prevPercentFull)
		{
			this.OnPercentFullChanged(percentFull);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevPercentFull = percentFull;
		}
	}
}
