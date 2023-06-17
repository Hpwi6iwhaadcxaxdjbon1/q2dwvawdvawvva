using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000857 RID: 2135
public class UIMixingTableItem : MonoBehaviour
{
	// Token: 0x04002FDA RID: 12250
	public Image ItemIcon;

	// Token: 0x04002FDB RID: 12251
	public Tooltip ItemTooltip;

	// Token: 0x04002FDC RID: 12252
	public RustText TextItemNameAndQuantity;

	// Token: 0x04002FDD RID: 12253
	public UIMixingTableItemIngredient[] Ingredients;

	// Token: 0x0600362D RID: 13869 RVA: 0x0014905C File Offset: 0x0014725C
	public void Init(Recipe recipe)
	{
		if (recipe == null)
		{
			return;
		}
		this.ItemIcon.sprite = recipe.DisplayIcon;
		this.TextItemNameAndQuantity.text = recipe.ProducedItemCount.ToString() + " x " + recipe.DisplayName;
		this.ItemTooltip.Text = recipe.DisplayDescription;
		for (int i = 0; i < this.Ingredients.Length; i++)
		{
			if (i >= recipe.Ingredients.Length)
			{
				this.Ingredients[i].InitBlank();
			}
			else
			{
				this.Ingredients[i].Init(recipe.Ingredients[i]);
			}
		}
	}
}
