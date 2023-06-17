using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000817 RID: 2071
public class SelectedBlueprint : SingletonComponent<SelectedBlueprint>, IInventoryChanged
{
	// Token: 0x04002E6C RID: 11884
	public ItemBlueprint blueprint;

	// Token: 0x04002E6D RID: 11885
	public InputField craftAmountText;

	// Token: 0x04002E6E RID: 11886
	public GameObject ingredientGrid;

	// Token: 0x04002E6F RID: 11887
	public IconSkinPicker skinPicker;

	// Token: 0x04002E70 RID: 11888
	public Image iconImage;

	// Token: 0x04002E71 RID: 11889
	public RustText titleText;

	// Token: 0x04002E72 RID: 11890
	public RustText descriptionText;

	// Token: 0x04002E73 RID: 11891
	public CanvasGroup CraftArea;

	// Token: 0x04002E74 RID: 11892
	public Button CraftButton;

	// Token: 0x04002E75 RID: 11893
	public RustText CraftingTime;

	// Token: 0x04002E76 RID: 11894
	public RustText CraftingAmount;

	// Token: 0x04002E77 RID: 11895
	public Sprite FavouriteOnSprite;

	// Token: 0x04002E78 RID: 11896
	public Sprite FavouriteOffSprite;

	// Token: 0x04002E79 RID: 11897
	public Image FavouriteButtonStatusMarker;

	// Token: 0x04002E7A RID: 11898
	public GameObject[] workbenchReqs;

	// Token: 0x04002E7B RID: 11899
	private ItemInformationPanel[] informationPanels;

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x060035D4 RID: 13780 RVA: 0x0014830C File Offset: 0x0014650C
	public static bool isOpen
	{
		get
		{
			return !(SingletonComponent<SelectedBlueprint>.Instance == null) && SingletonComponent<SelectedBlueprint>.Instance.blueprint != null;
		}
	}
}
