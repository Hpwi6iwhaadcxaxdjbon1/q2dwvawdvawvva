using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004B7 RID: 1207
[CreateAssetMenu(menuName = "Rust/Vehicles/Train Wagon Loot Data", fileName = "Train Wagon Loot Data")]
public class TrainWagonLootData : ScriptableObject
{
	// Token: 0x04001FFE RID: 8190
	[SerializeField]
	private TrainWagonLootData.LootOption[] oreOptions;

	// Token: 0x04001FFF RID: 8191
	[SerializeField]
	[ReadOnly]
	private TrainWagonLootData.LootOption lootWagonContent;

	// Token: 0x04002000 RID: 8192
	[SerializeField]
	private TrainWagonLootData.LootOption fuelWagonContent;

	// Token: 0x04002001 RID: 8193
	public static TrainWagonLootData instance;

	// Token: 0x04002002 RID: 8194
	private const int LOOT_WAGON_INDEX = 1000;

	// Token: 0x04002003 RID: 8195
	private const int FUEL_WAGON_INDEX = 1001;

	// Token: 0x06002777 RID: 10103 RVA: 0x000F6741 File Offset: 0x000F4941
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		TrainWagonLootData.instance = Resources.Load<TrainWagonLootData>("Train Wagon Loot Data");
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000F6754 File Offset: 0x000F4954
	public TrainWagonLootData.LootOption GetLootOption(TrainCarUnloadable.WagonType wagonType, out int index)
	{
		if (wagonType == TrainCarUnloadable.WagonType.Lootboxes)
		{
			index = 1000;
			return this.lootWagonContent;
		}
		if (wagonType != TrainCarUnloadable.WagonType.Fuel)
		{
			float num = 0f;
			foreach (TrainWagonLootData.LootOption lootOption in this.oreOptions)
			{
				num += lootOption.spawnWeighting;
			}
			float num2 = UnityEngine.Random.value * num;
			for (index = 0; index < this.oreOptions.Length; index++)
			{
				if ((num2 -= this.oreOptions[index].spawnWeighting) < 0f)
				{
					return this.oreOptions[index];
				}
			}
			return this.oreOptions[index];
		}
		index = 1001;
		return this.fuelWagonContent;
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000F67FC File Offset: 0x000F49FC
	public bool TryGetLootFromIndex(int index, out TrainWagonLootData.LootOption lootOption)
	{
		if (index == 1000)
		{
			lootOption = this.lootWagonContent;
			return true;
		}
		if (index != 1001)
		{
			index = Mathf.Clamp(index, 0, this.oreOptions.Length - 1);
			lootOption = this.oreOptions[index];
			return true;
		}
		lootOption = this.fuelWagonContent;
		return true;
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000F6850 File Offset: 0x000F4A50
	public bool TryGetIndexFromLoot(TrainWagonLootData.LootOption lootOption, out int index)
	{
		if (lootOption == this.lootWagonContent)
		{
			index = 1000;
			return true;
		}
		if (lootOption == this.fuelWagonContent)
		{
			index = 1001;
			return true;
		}
		for (index = 0; index < this.oreOptions.Length; index++)
		{
			if (this.oreOptions[index] == lootOption)
			{
				return true;
			}
		}
		index = -1;
		return false;
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000F68AC File Offset: 0x000F4AAC
	public static float GetOrePercent(int lootTypeIndex, StorageContainer sc)
	{
		TrainWagonLootData.LootOption lootOption;
		if (TrainWagonLootData.instance.TryGetLootFromIndex(lootTypeIndex, out lootOption))
		{
			return TrainWagonLootData.GetOrePercent(lootOption, sc);
		}
		return 0f;
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000F68D8 File Offset: 0x000F4AD8
	public static float GetOrePercent(TrainWagonLootData.LootOption lootOption, StorageContainer sc)
	{
		float result = 0f;
		if (sc.IsValid())
		{
			int maxLootAmount = lootOption.maxLootAmount;
			if ((float)maxLootAmount == 0f)
			{
				result = 0f;
			}
			else
			{
				result = Mathf.Clamp01((float)sc.inventory.GetAmount(lootOption.lootItem.itemid, false) / (float)maxLootAmount);
			}
		}
		return result;
	}

	// Token: 0x02000D15 RID: 3349
	[Serializable]
	public class LootOption
	{
		// Token: 0x0400461A RID: 17946
		public bool showsFX = true;

		// Token: 0x0400461B RID: 17947
		public ItemDefinition lootItem;

		// Token: 0x0400461C RID: 17948
		[FormerlySerializedAs("lootAmount")]
		public int maxLootAmount;

		// Token: 0x0400461D RID: 17949
		public int minLootAmount;

		// Token: 0x0400461E RID: 17950
		public Material lootMaterial;

		// Token: 0x0400461F RID: 17951
		public float spawnWeighting = 1f;

		// Token: 0x04004620 RID: 17952
		public Color fxTint;

		// Token: 0x04004621 RID: 17953
		[FormerlySerializedAs("indoorFXTint")]
		public Color particleFXTint;
	}
}
