using System;
using UnityEngine;

// Token: 0x020005EE RID: 1518
public class ItemModGiveOxygen : ItemMod, IAirSupply
{
	// Token: 0x040024EA RID: 9450
	public ItemModGiveOxygen.AirSupplyType airType = ItemModGiveOxygen.AirSupplyType.ScubaTank;

	// Token: 0x040024EB RID: 9451
	public int amountToConsume = 1;

	// Token: 0x040024EC RID: 9452
	public GameObjectRef inhaleEffect;

	// Token: 0x040024ED RID: 9453
	public GameObjectRef exhaleEffect;

	// Token: 0x040024EE RID: 9454
	public GameObjectRef bubblesEffect;

	// Token: 0x040024EF RID: 9455
	private float timeRemaining;

	// Token: 0x040024F0 RID: 9456
	private float cycleTime;

	// Token: 0x040024F1 RID: 9457
	private bool inhaled;

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x06002D59 RID: 11609 RVA: 0x0011119D File Offset: 0x0010F39D
	public ItemModGiveOxygen.AirSupplyType AirType
	{
		get
		{
			return this.airType;
		}
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x001111A5 File Offset: 0x0010F3A5
	public float GetAirTimeRemaining()
	{
		return this.timeRemaining;
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x001111B0 File Offset: 0x0010F3B0
	public override void ModInit()
	{
		base.ModInit();
		this.cycleTime = 1f;
		ItemMod[] siblingMods = this.siblingMods;
		for (int i = 0; i < siblingMods.Length; i++)
		{
			ItemModCycle itemModCycle;
			if ((itemModCycle = (siblingMods[i] as ItemModCycle)) != null)
			{
				this.cycleTime = itemModCycle.timeBetweenCycles;
			}
		}
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x001111FC File Offset: 0x0010F3FC
	public override void DoAction(Item item, BasePlayer player)
	{
		if (!item.hasCondition)
		{
			return;
		}
		if (item.conditionNormalized == 0f)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		float num = Mathf.Clamp01(0.525f);
		if (player.AirFactor() > num)
		{
			return;
		}
		if (item.parent == null)
		{
			return;
		}
		if (item.parent != player.inventory.containerWear)
		{
			return;
		}
		Effect.server.Run((!this.inhaled) ? this.inhaleEffect.resourcePath : this.exhaleEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		this.inhaled = !this.inhaled;
		if (!this.inhaled && WaterLevel.GetWaterDepth(player.eyes.position, player, null) > 3f)
		{
			Effect.server.Run(this.bubblesEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		}
		item.LoseCondition((float)this.amountToConsume);
		player.metabolism.oxygen.Add(1f);
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x00111316 File Offset: 0x0010F516
	public override void OnChanged(Item item)
	{
		if (item.hasCondition)
		{
			this.timeRemaining = item.condition * ((float)this.amountToConsume / this.cycleTime);
			return;
		}
		this.timeRemaining = 0f;
	}

	// Token: 0x02000D80 RID: 3456
	public enum AirSupplyType
	{
		// Token: 0x040047BA RID: 18362
		Lungs,
		// Token: 0x040047BB RID: 18363
		ScubaTank,
		// Token: 0x040047BC RID: 18364
		Submarine
	}
}
