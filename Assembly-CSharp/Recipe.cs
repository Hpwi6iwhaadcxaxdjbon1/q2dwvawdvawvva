using System;
using UnityEngine;

// Token: 0x02000758 RID: 1880
[CreateAssetMenu(menuName = "Rust/Recipe")]
public class Recipe : ScriptableObject
{
	// Token: 0x04002A9A RID: 10906
	[Header("Produced Item")]
	public ItemDefinition ProducedItem;

	// Token: 0x04002A9B RID: 10907
	public int ProducedItemCount = 1;

	// Token: 0x04002A9C RID: 10908
	public bool CanQueueMultiple = true;

	// Token: 0x04002A9D RID: 10909
	[Header("Spawned Item")]
	public GameObjectRef SpawnedItem;

	// Token: 0x04002A9E RID: 10910
	public string SpawnedItemName;

	// Token: 0x04002A9F RID: 10911
	public string SpawnedItemDescription;

	// Token: 0x04002AA0 RID: 10912
	public Sprite SpawnedItemIcon;

	// Token: 0x04002AA1 RID: 10913
	[Header("Misc")]
	public bool RequiresBlueprint;

	// Token: 0x04002AA2 RID: 10914
	public Recipe.RecipeIngredient[] Ingredients;

	// Token: 0x04002AA3 RID: 10915
	public float MixingDuration;

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x0600348A RID: 13450 RVA: 0x001455D7 File Offset: 0x001437D7
	public string DisplayName
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayName.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemName;
			}
			return null;
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x0600348B RID: 13451 RVA: 0x00145608 File Offset: 0x00143808
	public string DisplayDescription
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayDescription.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemDescription;
			}
			return null;
		}
	}

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x0600348C RID: 13452 RVA: 0x00145639 File Offset: 0x00143839
	public Sprite DisplayIcon
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.iconSprite;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemIcon;
			}
			return null;
		}
	}

	// Token: 0x0600348D RID: 13453 RVA: 0x00145668 File Offset: 0x00143868
	public bool ContainsItem(Item item)
	{
		if (item == null)
		{
			return false;
		}
		if (this.Ingredients == null)
		{
			return false;
		}
		foreach (Recipe.RecipeIngredient recipeIngredient in this.Ingredients)
		{
			if (item.info == recipeIngredient.Ingredient)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x02000E6B RID: 3691
	[Serializable]
	public struct RecipeIngredient
	{
		// Token: 0x04004B66 RID: 19302
		public ItemDefinition Ingredient;

		// Token: 0x04004B67 RID: 19303
		public int Count;
	}
}
