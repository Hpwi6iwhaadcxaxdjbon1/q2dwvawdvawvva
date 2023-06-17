using System;
using System.Collections.Generic;

// Token: 0x02000759 RID: 1881
public static class RecipeDictionary
{
	// Token: 0x04002AA4 RID: 10916
	private static Dictionary<uint, Dictionary<int, List<Recipe>>> recipeListsDict = new Dictionary<uint, Dictionary<int, List<Recipe>>>();

	// Token: 0x0600348F RID: 13455 RVA: 0x001456D0 File Offset: 0x001438D0
	public static void CacheRecipes(RecipeList recipeList)
	{
		if (recipeList == null)
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary2 = new Dictionary<int, List<Recipe>>();
		RecipeDictionary.recipeListsDict.Add(recipeList.FilenameStringId, dictionary2);
		foreach (Recipe recipe in recipeList.Recipes)
		{
			List<Recipe> list = null;
			if (!dictionary2.TryGetValue(recipe.Ingredients[0].Ingredient.itemid, out list))
			{
				list = new List<Recipe>();
				dictionary2.Add(recipe.Ingredients[0].Ingredient.itemid, list);
			}
			list.Add(recipe);
		}
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x00145780 File Offset: 0x00143980
	public static Recipe GetMatchingRecipeAndQuantity(RecipeList recipeList, List<Item> orderedIngredients, out int quantity)
	{
		quantity = 0;
		if (recipeList == null)
		{
			return null;
		}
		if (orderedIngredients == null || orderedIngredients.Count == 0)
		{
			return null;
		}
		List<Recipe> recipesByFirstIngredient = RecipeDictionary.GetRecipesByFirstIngredient(recipeList, orderedIngredients[0]);
		if (recipesByFirstIngredient == null)
		{
			return null;
		}
		foreach (Recipe recipe in recipesByFirstIngredient)
		{
			if (!(recipe == null) && recipe.Ingredients.Length == orderedIngredients.Count)
			{
				bool flag = true;
				int num = int.MaxValue;
				int num2 = 0;
				foreach (Recipe.RecipeIngredient recipeIngredient in recipe.Ingredients)
				{
					Item item = orderedIngredients[num2];
					if (recipeIngredient.Ingredient != item.info || item.amount < recipeIngredient.Count)
					{
						flag = false;
						break;
					}
					int num3 = item.amount / recipeIngredient.Count;
					if (num2 == 0)
					{
						num = num3;
					}
					else if (num3 < num)
					{
						num = num3;
					}
					num2++;
				}
				if (flag)
				{
					quantity = num;
					if (quantity > 1 && !recipe.CanQueueMultiple)
					{
						quantity = 1;
					}
					return recipe;
				}
			}
		}
		return null;
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x001458CC File Offset: 0x00143ACC
	private static List<Recipe> GetRecipesByFirstIngredient(RecipeList recipeList, Item firstIngredient)
	{
		if (recipeList == null)
		{
			return null;
		}
		if (firstIngredient == null)
		{
			return null;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (!RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			RecipeDictionary.CacheRecipes(recipeList);
		}
		if (dictionary == null)
		{
			return null;
		}
		List<Recipe> result;
		if (!dictionary.TryGetValue(firstIngredient.info.itemid, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x00145920 File Offset: 0x00143B20
	public static bool ValidIngredientForARecipe(Item ingredient, RecipeList recipeList)
	{
		if (ingredient == null)
		{
			return false;
		}
		if (recipeList == null)
		{
			return false;
		}
		foreach (Recipe recipe in recipeList.Recipes)
		{
			if (!(recipe == null) && recipe.ContainsItem(ingredient))
			{
				return true;
			}
		}
		return false;
	}
}
