using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000858 RID: 2136
public class UIMixingTableItemIngredient : MonoBehaviour
{
	// Token: 0x04002FDE RID: 12254
	public Image ItemIcon;

	// Token: 0x04002FDF RID: 12255
	public Text ItemCount;

	// Token: 0x04002FE0 RID: 12256
	public Tooltip ToolTip;

	// Token: 0x0600362F RID: 13871 RVA: 0x00149104 File Offset: 0x00147304
	public void Init(Recipe.RecipeIngredient ingredient)
	{
		this.ItemIcon.sprite = ingredient.Ingredient.iconSprite;
		this.ItemCount.text = ingredient.Count.ToString();
		this.ItemIcon.enabled = true;
		this.ItemCount.enabled = true;
		this.ToolTip.Text = ingredient.Count.ToString() + " x " + ingredient.Ingredient.displayName.translated;
		this.ToolTip.enabled = true;
	}

	// Token: 0x06003630 RID: 13872 RVA: 0x00149193 File Offset: 0x00147393
	public void InitBlank()
	{
		this.ItemIcon.enabled = false;
		this.ItemCount.enabled = false;
		this.ToolTip.enabled = false;
	}
}
