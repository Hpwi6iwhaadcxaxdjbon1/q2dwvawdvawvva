using System;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
public class ItemModCycle : ItemMod
{
	// Token: 0x040024D4 RID: 9428
	public ItemMod[] actions;

	// Token: 0x040024D5 RID: 9429
	public float timeBetweenCycles = 1f;

	// Token: 0x040024D6 RID: 9430
	public float timerStart;

	// Token: 0x040024D7 RID: 9431
	public bool onlyAdvanceTimerWhenPass;

	// Token: 0x06002D44 RID: 11588 RVA: 0x00110D88 File Offset: 0x0010EF88
	public override void OnItemCreated(Item itemcreated)
	{
		float timeTaken = this.timerStart;
		itemcreated.onCycle += delegate(Item item, float delta)
		{
			if (this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			timeTaken += delta;
			if (timeTaken < this.timeBetweenCycles)
			{
				return;
			}
			timeTaken = 0f;
			if (!this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			this.CustomCycle(item, delta);
		};
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x00110DC0 File Offset: 0x0010EFC0
	private bool CanCycle(Item item)
	{
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CanDoAction(item, item.GetOwnerPlayer()))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x00110DF8 File Offset: 0x0010EFF8
	public void CustomCycle(Item item, float delta)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		ItemMod[] array = this.actions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DoAction(item, ownerPlayer);
		}
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x00110E2B File Offset: 0x0010F02B
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null", base.gameObject);
		}
	}
}
