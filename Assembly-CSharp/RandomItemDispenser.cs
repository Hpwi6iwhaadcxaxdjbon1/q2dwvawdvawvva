using System;
using UnityEngine;

// Token: 0x0200045B RID: 1115
public class RandomItemDispenser : PrefabAttribute, IServerComponent
{
	// Token: 0x04001D43 RID: 7491
	public RandomItemDispenser.RandomItemChance[] Chances;

	// Token: 0x04001D44 RID: 7492
	public bool OnlyAwardOne = true;

	// Token: 0x060024E1 RID: 9441 RVA: 0x000E98C3 File Offset: 0x000E7AC3
	protected override Type GetIndexedType()
	{
		return typeof(RandomItemDispenser);
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000E98D0 File Offset: 0x000E7AD0
	public void DistributeItems(BasePlayer forPlayer, Vector3 distributorPosition)
	{
		foreach (RandomItemDispenser.RandomItemChance itemChance in this.Chances)
		{
			bool flag = this.TryAward(itemChance, forPlayer, distributorPosition);
			if (this.OnlyAwardOne && flag)
			{
				break;
			}
		}
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000E9910 File Offset: 0x000E7B10
	private bool TryAward(RandomItemDispenser.RandomItemChance itemChance, BasePlayer forPlayer, Vector3 distributorPosition)
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		if (itemChance.Chance >= num)
		{
			Item item = ItemManager.Create(itemChance.Item, itemChance.Amount, 0UL);
			if (item != null)
			{
				if (forPlayer)
				{
					forPlayer.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(distributorPosition + Vector3.up * 0.5f, Vector3.up, default(Quaternion));
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x02000CEA RID: 3306
	[Serializable]
	public struct RandomItemChance
	{
		// Token: 0x0400459A RID: 17818
		public ItemDefinition Item;

		// Token: 0x0400459B RID: 17819
		public int Amount;

		// Token: 0x0400459C RID: 17820
		[Range(0f, 1f)]
		public float Chance;
	}
}
